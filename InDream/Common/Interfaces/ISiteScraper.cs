using InDream.Common.BaseModels;
using InDream.Models.SiteScraper;

namespace InDream.Common.Interfaces;

public interface ISiteScraper
{
    bool CanHandle(Uri uri);
    Task<ResponseBase<ScrapedProductModel?>> ScrapeProductInfoAsync(string url);
}
