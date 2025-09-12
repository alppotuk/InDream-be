
using AngleSharp.Dom;
using InDream.Common.BaseModels;
using InDream.Data;
using InDream.Factories;
using InDream.Models.SiteScraper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace InDream.Services;

public class ScheduledScrapingService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public ScheduledScrapingService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(5));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            using var scope = _serviceProvider.CreateScope();
            var trackedItemRepository = scope.ServiceProvider.GetRequiredService<IRepository<TrackedItem>>();
            var scraperFactory = scope.ServiceProvider.GetRequiredService<ScraperFactory>();
            // var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var trackedItems = await trackedItemRepository.Table
                .Where(p => p.IsActive)
                .ToListAsync();

            foreach (var trackedItem in trackedItems)
            {
                var uri = new Uri(trackedItem.Url);
                var scraper = scraperFactory.GetScraper(uri);
                if (scraper == null)
                    continue;


                var scrapedData = await scraper.ScrapeProductInfoAsync(trackedItem.Url);
                if (scrapedData == null || !scrapedData.Success || scrapedData.Data == null)
                    continue;

                var freshTrackedItem = scrapedData.Data.Properties.FirstOrDefault(p => JsonConvert.SerializeObject(p.PropertyTexts) == trackedItem.PropertiesSerialized);
                if(freshTrackedItem == null)
                    continue;


                

                bool priceChanged = freshTrackedItem.Price != trackedItem.Price;
                bool stockChanged = freshTrackedItem.IsInStock != trackedItem.IsInStock;

                if (priceChanged || stockChanged)
                {
                    Console.WriteLine($"DEĞİŞİKLİK TESPİT EDİLDİ: {trackedItem.Title}");

                    // TODO: Kullanıcıya email at!
                    // await emailService.SendChangeNotificationAsync(savedVariant, matchingFreshVariant);

                    trackedItem.IsActive = false;
                }

                trackedItem.LastCheckedUtc = DateTime.UtcNow;
                await trackedItemRepository.Update(trackedItem);
            }
        }
    }
}
