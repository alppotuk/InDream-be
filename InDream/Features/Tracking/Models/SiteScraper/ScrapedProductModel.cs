using InDream.Api.Common.Enums;

namespace InDream.Features.Tracking.Models.SiteScraper;

public class ScrapedProductModel
{
    public string Title { get; set; }
    public string ImageUrl { get; set; }
    public ProductBrandEnum ProductBrand { get; set; }
    public List<ScrapedProductPropertiesModel> Properties { get; set; } = [];
}
