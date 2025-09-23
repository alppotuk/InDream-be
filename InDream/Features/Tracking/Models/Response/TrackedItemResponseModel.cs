using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InDream.Api.Features.Tracking.Models.Response;

public class TrackedItemResponseModel
{
    public long Id { get; set; }
    public string Url { get; set; }
    public string Title { get; set; }
    public string ProductBrandText { get; set; }
    public string ImageUrl { get; set; }
    public decimal Price { get; set; }
    public string? CurrencyCode { get; set; }
    public bool IsInStock { get; set; }
    public List<string> Properties { get; set; } = [];

    public DateTime LastCheckedUtc { get; set; }
}
