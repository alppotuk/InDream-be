namespace InDream.Models.SiteScraper;

public class ScrapedProdutPropertiesModel
{
    public decimal Price { get; set; }
    public bool IsInStock { get; set; }
    public List<string> PropertyTexts { get; set; } = [];
}
