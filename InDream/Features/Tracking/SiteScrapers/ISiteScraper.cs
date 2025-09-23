
using InDream.Core.BaseModels;
using InDream.Features.Tracking.Models.SiteScraper;

namespace InDream.Features.Tracking.SiteScrapers;

public interface ISiteScraper
{
    bool CanHandle(Uri uri);
    Task<ResponseBase<ScrapedProductModel?>> ScrapeProductInfoAsync(string url);
}
