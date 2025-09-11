using InDream.Common.BaseModels;
using InDream.Data;
using InDream.Factories;
using InDream.Models.SiteScraper;
using InDream.Models.TrackedItem;
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
    private readonly ScraperFactory _scraperFactory;

    public TrackedItemController(IRepository<TrackedItem> trackedItemRepository, ScraperFactory scraperFactory)
    {
        _trackedItemRepository = trackedItemRepository;
        _scraperFactory = scraperFactory;
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
    public async Task<ResponseBase<ScrapedProductModel?>> TrackedItemInfoFromUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return new ResponseBase<ScrapedProductModel?>(false, null);

        var uri = new Uri(url);
        var scraper = _scraperFactory.GetScraper(uri);
        if(scraper == null)
            return new ResponseBase<ScrapedProductModel?>(false, null, message: "This site is not supported.");


        var result = await scraper.ScrapeProductInfoAsync(url);

        return result;
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

}
