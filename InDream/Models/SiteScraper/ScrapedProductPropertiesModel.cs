namespace InDream.Models.SiteScraper;

public class ScrapedProductPropertiesModel
{
    public decimal Price { get; set; }
    public string PriceText { get; set; }
    public bool IsInStock { get; set; }
    public List<string> PropertyTexts { get; set; } = [];
}
