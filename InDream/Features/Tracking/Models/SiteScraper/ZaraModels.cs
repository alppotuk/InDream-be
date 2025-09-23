using System.Text.Json.Serialization;

namespace InDream.Features.Tracking.Models.SiteScraper;

public class ZaraProductResponse
{
    [JsonPropertyName("product")]
    public ZaraProduct? Product { get; set; }
}

public class ZaraProduct
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("detail")]
    public ZaraDetail? Detail { get; set; }
}

public class ZaraDetail
{
    [JsonPropertyName("colors")]
    public List<ZaraColor>? Colors { get; set; }
}

public class ZaraColor
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("mainImgs")]
    public List<ZaraImage>? MainImgs { get; set; }

    [JsonPropertyName("sizes")]
    public List<ZaraSize>? Sizes { get; set; }
}

public class ZaraImage
{
    [JsonPropertyName("url")]
    public string? Url { get; set; }
}

public class ZaraSize
{
    [JsonPropertyName("availability")]
    public string? Availability { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("price")]
    public int Price { get; set; }
}