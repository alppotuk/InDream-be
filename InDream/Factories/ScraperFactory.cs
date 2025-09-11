using InDream.Common.BaseModels;
using InDream.Common.Interfaces;

namespace InDream.Factories;

public class ScraperFactory : IInjectAsScoped
{
    private readonly IEnumerable<ISiteScraper> _scrapers;

    public ScraperFactory(IEnumerable<ISiteScraper> scrapers)
    {
        _scrapers = scrapers;
    }


    public ISiteScraper? GetScraper(Uri uri)
    {
        // TODO: return default scraper if null
        return _scrapers.FirstOrDefault(s => s.CanHandle(uri));
    }
}
