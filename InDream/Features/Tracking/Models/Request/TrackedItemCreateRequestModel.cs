using InDream.Api.Common.Enums;

namespace InDream.Api.Features.Tracking.Models.Request;
public class TrackedItemCreateRequestModel
{
    public string Url { get; set; }
    public string Title { get; set; }
    public string ImageUrl { get; set; }
    public ProductBrandEnum ProductBrand { get; set; }
    public decimal Price { get; set; }
    public string? CurrencyCode { get; set; }
    public bool IsInStock { get; set; }

    public List<string> PropertyTexts { get; set; } = [];
}
