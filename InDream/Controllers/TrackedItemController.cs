using InDream.Data;
using InDream.Interfaces;
using InDream.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InDream.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TrackedItemController : ControllerBase
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
                .Where(p => p.IsActive)
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
        public async Task<Response<TrackedItemModel?>> TrackedItemSingle(long id)
        {
            var item = await _trackedItemRepository
                .Table
                .Where(p => p.IsActive && p.Id == id)
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

    }
}
