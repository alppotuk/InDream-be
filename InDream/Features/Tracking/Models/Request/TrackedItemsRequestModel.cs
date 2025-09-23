using InDream.Api.Common.Enums;
using InDream.Core.Pagination;

namespace InDream.Api.Features.Tracking.Models.Request;

public class TrackedItemsRequestModel : PaginationFilter
{
    public ProductBrandEnum? ProductBrand { get; set; }
    public string? SearchText { get; set; }
}
