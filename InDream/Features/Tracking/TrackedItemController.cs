using InDream.Api.Common.Enums;
using InDream.Api.Features.Tracking.Data;
using InDream.Api.Features.Tracking.Models.Request;
using InDream.Api.Features.Tracking.Models.Response;
using InDream.Core.BaseModels;
using InDream.Core.Repository;
using InDream.Core.Repository.Extensions;
using InDream.Core.Utility;
using InDream.Features.Tracking.Models.SiteScraper;
using InDream.Features.Tracking.Services;
using InDream.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Web;

namespace InDream.Features.Tracking;

[Route("[controller]/[action]")]
[Authorize]
public class TrackedItemController : BaseController
{
    private readonly IRepository<TrackedItem> _trackedItemRepository;
    private readonly ScraperFactory _scraperFactory;
    private readonly EmailService _emailService;

    public TrackedItemController(IRepository<TrackedItem> trackedItemRepository, ScraperFactory scraperFactory, EmailService emailService)
    {
        _trackedItemRepository = trackedItemRepository;
        _scraperFactory = scraperFactory;
        _emailService = emailService;
    }


    [HttpPost]
    public async Task<ResponseBase<List<TrackedItemResponseModel>>> TrackedItems(TrackedItemsRequestModel filter)
    {
        var query = _trackedItemRepository.Table.Where(p => p.IsActive && p.AccountId == IdentityId);

        if (filter.ProductBrand.HasValue)
            query = query.Where(p => p.ProductBrand == filter.ProductBrand);

        if (!string.IsNullOrWhiteSpace(filter.SearchText))
            query = query.Where(p => p.Title.ToLower().Contains(filter.SearchText.ToLower()));

        var (pagedData, pagination) = await query
            .Select(p => new TrackedItemResponseModel
            {
                Id = p.Id,
                Url = p.Url,
                Title = p.Title,
                ProductBrandText = God.GetEnumDescription<ProductBrandEnum>(p.ProductBrand),
                ImageUrl = p.ImageUrl,
                Price = p.Price,
                CurrencyCode = p.CurrencyCode,
                IsInStock = p.IsInStock,
                LastCheckedUtc = p.LastCheckedUtc,
                Properties = JsonConvert.DeserializeObject<List<string>>(p.PropertiesSerialized) ?? new List<string>(),
            }).ToPagerAsync(filter);

        return new ResponseBase<List<TrackedItemResponseModel>>(true, pagedData, pagination) ;
    }

    [HttpGet]
    public async Task<ResponseBase<TrackedItemResponseModel?>> TrackedItem(long id)
    {
        var item = await _trackedItemRepository
            .Table
            .Where(p => p.IsActive && p.Id == id && p.AccountId == IdentityId)
            .Select(p => new TrackedItemResponseModel
            {
                Id = p.Id,
                Url = p.Url,
                Title = p.Title,
                ImageUrl = p.ImageUrl,
                ProductBrandText = God.GetEnumDescription<ProductBrandEnum>(p.ProductBrand),
                Price = p.Price,
                CurrencyCode = p.CurrencyCode,
                IsInStock = p.IsInStock,
                LastCheckedUtc = p.LastCheckedUtc
            }).FirstOrDefaultAsync();

        if (item == null)
            return new ResponseBase<TrackedItemResponseModel?>(false, null);

        return new ResponseBase<TrackedItemResponseModel?>(true, item);
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ResponseBase<ScrapedProductModel?>> TrackedItemInfoFromUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return new ResponseBase<ScrapedProductModel?>(false, null);

        url = HttpUtility.UrlDecode(url);

        var uri = new Uri(url);
        var scraper = _scraperFactory.GetScraper(uri);
        if(scraper == null)
            return new ResponseBase<ScrapedProductModel?>(false, null, message: "This site is not supported.");


        var result = await scraper.ScrapeProductInfoAsync(url);

        return result;
    }

    [HttpPost]
    public async Task<ResponseBase<TrackedItemResponseModel?>> TrackedItem(TrackedItemCreateRequestModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Url)
            || string.IsNullOrWhiteSpace(model.Title)
            || string.IsNullOrWhiteSpace(model.ImageUrl)
            || model.Price == 0
            || !model.PropertyTexts.Any())
            return new ResponseBase<TrackedItemResponseModel?>(false, null);

        var trackedItem = new TrackedItem
        {
            AccountId = IdentityId,
            Url = model.Url,
            Title = model.Title,
            ImageUrl = model.ImageUrl,
            ProductBrand = model.ProductBrand,
            Price = model.Price,
            CurrencyCode = model.CurrencyCode,
            IsInStock = model.IsInStock,
            PropertiesSerialized = JsonConvert.SerializeObject(model.PropertyTexts),
            IsActive = true,
            LastCheckedUtc = DateTime.UtcNow,
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

    [HttpGet]
    public async Task<ResponseBase<List<SelectListItem>>> ProductBrands()
    {
        return new ResponseBase<List<SelectListItem>>(true, God.GetEnumSelectList<ProductBrandEnum>());
    }


    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> SendTestEmail()
    {
        await _emailService.SendEmailAsync(
            "metealppotuk@gmail.com",
            "Deneme Konu",
            "<h1>Merhaba!</h1><p>Bu bir test mailidir.</p>"
        );

        return Ok("Mail gönderildi!");
    }

}
