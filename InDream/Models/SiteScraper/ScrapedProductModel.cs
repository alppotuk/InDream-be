namespace InDream.Models.SiteScraper;

public class ScrapedProductModel
{
    public string Title { get; set; }
    public string ImageUrl { get; set; }
    public List<ScrapedProdutPropertiesModel> Properties { get; set; } = [];
}
