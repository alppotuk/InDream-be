namespace InDream.Models.TrackedItem;
public class TrackedItemCreateModel
{
    public string Url { get; set; }
    public string Title { get; set; }
    public string ImageUrl { get; set; }
    public string PriceText { get; set; }
    public string StockText { get; set; }



    public string? TitleCssSelector { get; set; }
    public string? ImageUrlCssSelector { get; set; }
    public string? PriceCssSelector { get; set; }
    public string? StockTextCssSelector { get; set; }
}
