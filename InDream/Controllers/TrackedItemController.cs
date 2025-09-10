using InDream.Data;
using InDream.Interfaces;
using InDream.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace InDream.Controllers;

[Route("[controller]/[action]")]
[Authorize]
public class TrackedItemController : BaseController
{
    private readonly IRepository<TrackedItem> _trackedItemRepository;

    public TrackedItemController(IRepository<TrackedItem> trackedItemRepository)
    {
        _trackedItemRepository = trackedItemRepository;
    }


    [HttpPost]
    public async Task<Response<List<TrackedItemModel>>> TrackedItems(PaginationFilter filter)
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

        return new Response<List<TrackedItemModel>>(true, pagedData, pagination) ;
    }

    [HttpGet]
    public async Task<Response<TrackedItemModel?>> TrackedItem(long id)
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
            return new Response<TrackedItemModel?>(false, null);

        return new Response<TrackedItemModel?>(true, item);
    }

    [HttpPost]
    public async Task<Response<TrackedItemModel?>> TrackedItem(TrackedItemCreateModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Url)
            || string.IsNullOrWhiteSpace(model.Title)
            || string.IsNullOrWhiteSpace(model.ImageUrl)
            || string.IsNullOrWhiteSpace(model.PriceText)
            || string.IsNullOrWhiteSpace(model.StockText))

            return new Response<TrackedItemModel?>(false, null);

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

            LastTimeChecked = DateTime.UtcNow,
            IsActive = true
        };

        await _trackedItemRepository.Create(trackedItem);

        return await TrackedItem(trackedItem.Id);
    }

    [HttpPost]
    public async Task<Response<bool>> DeactivateTrackItem(Entity model)
    {
        var trackedItem = await _trackedItemRepository
            .Table
            .FirstOrDefaultAsync(p => p.Id == model.Id && p.AccountId == IdentityId);

        if (trackedItem == null)
            return new Response<bool>(false, false);

        trackedItem.IsActive = false;

        await _trackedItemRepository.Update(trackedItem);

        return new Response<bool>(true, true);
    }

    [HttpPost]
    public async Task<Response<bool>> UnTrackItem(Entity model)
    {
        var trackedItem = await _trackedItemRepository
            .Table
            .FirstOrDefaultAsync(p => p.Id == model.Id && p.AccountId == IdentityId);

        if (trackedItem == null)
            return new Response<bool>(false, false);

        await _trackedItemRepository.Delete(trackedItem);

        return new Response<bool>(true, true);
    }

}
