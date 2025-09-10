using AngleSharp;
using AngleSharp.Dom;
using InDream.Data;
using InDream.Interfaces;
using InDream.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuppeteerSharp;
using System;

namespace InDream.Controllers;

[Route("[controller]/[action]")]
//[Authorize]
public class TrackedItemController : BaseController
{
    private readonly IRepository<TrackedItem> _trackedItemRepository;
    private readonly IBrowsingContext _context;

    public TrackedItemController(IRepository<TrackedItem> trackedItemRepository, IBrowsingContext context)
    {
        _trackedItemRepository = trackedItemRepository;
        _context = context;
    }


    [HttpPost]
    public async Task<ResponseBase<List<TrackedItemModel>>> TrackedItems(PaginationFilter filter)
    {
        var (pagedData, pagination) = await _trackedItemRepository
            .Table
            .Where(p => p.IsActive && p.AccountId == IdentityId)
            .Select(p => new TrackedItemModel
            {
                Id = p.Id,
                Url = p.Url,
                Title = p.Title,
                ImageUrl = p.ImageUrl,
                PriceText = p.ImageUrl,
                StockText = p.StockText,
                LastTimeChecked = p.LastTimeChecked
            }).ToPagerAsync(filter);

        return new ResponseBase<List<TrackedItemModel>>(true, pagedData, pagination) ;
    }

    [HttpGet]
    public async Task<ResponseBase<TrackedItemModel?>> TrackedItem(long id)
    {
        var item = await _trackedItemRepository
            .Table
            .Where(p => p.IsActive && p.Id == id && p.AccountId == IdentityId)
            .Select(p => new TrackedItemModel
            {
                Id = p.Id,
                Url = p.Url,
                Title = p.Title,
                ImageUrl = p.ImageUrl,
                PriceText = p.ImageUrl,
                StockText = p.StockText,
                LastTimeChecked = p.LastTimeChecked
            }).FirstOrDefaultAsync();

        if (item == null)
            return new ResponseBase<TrackedItemModel?>(false, null);

        return new ResponseBase<TrackedItemModel?>(true, item);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ResponseBase<TrackedItemInfoFromUrlResponseModel?>> TrackedItemInfoFromUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return new ResponseBase<TrackedItemInfoFromUrlResponseModel?>(false, null);

        try
        {
            await new BrowserFetcher().DownloadAsync();

            var launchOptions = new LaunchOptions
            {
                Headless = true,
                Args = new[]
                {
                    "--no-sandbox",
                    "--disable-setuid-sandbox",
                    "--disable-infobars",
                    "--window-position=0,0",
                    "--ignore-certifcate-errors",
                    "--ignore-certifcate-errors-spki-list"
                }
            };

            using (var browser = await Puppeteer.LaunchAsync(launchOptions))
            using (var page = await browser.NewPageAsync())
            {
                await page.SetUserAgentAsync(
                    "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 " +
                    "(KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36"
                );

                await page.GoToAsync(url, WaitUntilNavigation.Networkidle0);

                var htmlContent = await page.GetContentAsync();
                //await System.IO.File.WriteAllTextAsync("debug.html", htmlContent);

                var imageUrl = await page.EvaluateFunctionAsync<string>(@"
                    () => {
                        const el = document.querySelector('meta[property=""og:image""]');
                        return el ? el.getAttribute('content') : null;
                    }
                ");

                var priceText = await page.EvaluateFunctionAsync<string>(@"
                    () => {
                        const selectors = [
                            'meta[property=""product:price:amount""]',
                            'meta[itemprop=""price""]',
                            '.price',
                            '.product-price',
                            '.new-price',
                            '#price',
                            '#product-price',
                            '#our_price_display'
                        ];
                        for (const sel of selectors) {
                            const el = document.querySelector(sel);
                            if (el) {
                                return el.getAttribute('content') || el.textContent.trim();
                            }
                        }
                        return null;
                    }
                ");

                var stockText = await page.EvaluateFunctionAsync<string>(@"
                    () => {
                        const selectors = ['.stock', '#stock', '.availability'];
                        for (const sel of selectors) {
                            const el = document.querySelector(sel);
                            if (el) {
                                return el.getAttribute('content') || el.textContent.trim();
                            }
                        }
                        return null;
                    }
                ");

                var result = new TrackedItemInfoFromUrlResponseModel
                {
                    Url = url,
                    ImageUrl = imageUrl,
                    PriceText = priceText,
                    StockText = stockText
                };

                return new ResponseBase<TrackedItemInfoFromUrlResponseModel?>(true, result);
            }

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Scraping error: {url} - {ex.Message}");
            return new ResponseBase<TrackedItemInfoFromUrlResponseModel?>(false, null, message: ex.Message);
        }
    }

    [HttpPost]
    public async Task<ResponseBase<TrackedItemModel?>> TrackedItem(TrackedItemCreateModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Url)
            || string.IsNullOrWhiteSpace(model.Title)
            || string.IsNullOrWhiteSpace(model.ImageUrl)
            || string.IsNullOrWhiteSpace(model.PriceText)
            || string.IsNullOrWhiteSpace(model.StockText))
            return new ResponseBase<TrackedItemModel?>(false, null);

        var trackedItem = new TrackedItem
        {
            AccountId = IdentityId,
            Url = model.Url,
            Title = model.Title,
            ImageUrl = model.ImageUrl,
            PriceText = model.PriceText,
            StockText = model.StockText,
            TitleCssSelector = model.TitleCssSelector,
            ImageUrlCssSelector = model.ImageUrlCssSelector,
            PriceCssSelector = model.PriceCssSelector,
            StockTextCssSelector = model.StockTextCssSelector,
            IsActive = true,
            CreationDateUtc = DateTime.UtcNow,
        };

        await _trackedItemRepository.Create(trackedItem);

        return await TrackedItem(trackedItem.Id);
    }

    [HttpPost]
    public async Task<ResponseBase<bool>> DeactivateTrackItem(EntityBase model)
    {
        var trackedItem = await _trackedItemRepository
            .Table
            .FirstOrDefaultAsync(p => p.Id == model.Id && p.AccountId == IdentityId);

        if (trackedItem == null)
            return new ResponseBase<bool>(false, false);

        trackedItem.IsActive = false;

        await _trackedItemRepository.Update(trackedItem);

        return new ResponseBase<bool>(true, true);
    }

    [HttpPost]
    public async Task<ResponseBase<bool>> UnTrackItem(EntityBase model)
    {
        var trackedItem = await _trackedItemRepository
            .Table
            .FirstOrDefaultAsync(p => p.Id == model.Id && p.AccountId == IdentityId);

        if (trackedItem == null)
            return new ResponseBase<bool>(false, false);

        await _trackedItemRepository.Delete(trackedItem);

        return new ResponseBase<bool>(true, true);
    }



    private string? TryGetValue(IDocument document, IEnumerable<string> selectors, string attribute = "content", bool getTextContentFallback = false)
    {
        foreach (var selector in selectors)
        {
            var element = document.QuerySelector(selector);
            if (element != null)
            {
                string? value = null;
                if (attribute != "content" || element.HasAttribute(attribute))
                {
                    value = element.GetAttribute(attribute);
                }

                if (string.IsNullOrWhiteSpace(value) && getTextContentFallback)
                {
                    value = element.TextContent;
                }

                if (!string.IsNullOrWhiteSpace(value))
                {
                    return value.Trim();
                }
            }
        }
        return null; 
    }

}
