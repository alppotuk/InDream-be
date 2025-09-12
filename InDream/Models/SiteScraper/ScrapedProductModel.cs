namespace InDream.Models.SiteScraper;

public class ScrapedProductModel
{
    public string Title { get; set; }
    public string ImageUrl { get; set; }
    public List<ScrapedProductPropertiesModel> Properties { get; set; } = [];
}
