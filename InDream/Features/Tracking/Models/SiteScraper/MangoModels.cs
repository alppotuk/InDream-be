using System.Text.Json.Serialization;

namespace InDream.Features.Tracking.Models.SiteScraper;
public class MangoPriceDetail
{
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
}


public class MangoStockResponse
{
    [JsonPropertyName("colors")]
    public Dictionary<string, MangoColorStock> Colors { get; set; } = new();
}

public class MangoColorStock
{
    [JsonPropertyName("sizes")]
    public Dictionary<string, MangoSizeStock> Sizes { get; set; } = new();
}

public class MangoSizeStock
{
    [JsonPropertyName("available")]
    public bool Available { get; set; }
}