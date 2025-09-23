using InDream.Api.Features.Authentication.Data;
using InDream.Api.Features.Tracking.Data;
using InDream.Core.Repository;
using InDream.Features.Tracking.Services;
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
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(50));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            using var scope = _serviceProvider.CreateScope();

            var trackedItemRepository = scope.ServiceProvider.GetRequiredService<IRepository<TrackedItem>>();
            var accountRepository = scope.ServiceProvider.GetRequiredService<IRepository<Account>>();
            var scraperFactory = scope.ServiceProvider.GetRequiredService<ScraperFactory>();
            var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();


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
                    var account = await accountRepository
                        .Table
                        .FirstOrDefaultAsync(p => p.Id == trackedItem.AccountId);

                    if (account == null)
                        continue;

                    string subject = $"Ürün Güncellemesi: {trackedItem.Title}";

                    string htmlBody = $@"
                    <html>
                      <head>
                        <style>
                          body {{
                            font-family: Arial, sans-serif;
                            background-color: #f9f9f9;
                            padding: 20px;
                            color: #333;
                          }}
                          .card {{
                            max-width: 600px;
                            margin: auto;
                            background: #fff;
                            border-radius: 8px;
                            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
                            overflow: hidden;
                          }}
                          .header {{
                            background-color: #4CAF50;
                            color: white;
                            padding: 12px 20px;
                            font-size: 18px;
                          }}
                          .content {{
                            padding: 20px;
                          }}
                          .content img {{
                            max-width: 200px;
                            border-radius: 4px;
                            margin-bottom: 10px;
                          }}
                          .content a {{
                            display: inline-block;
                            margin-top: 10px;
                            padding: 10px 15px;
                            background: #4CAF50;
                            color: white;
                            text-decoration: none;
                            border-radius: 5px;
                          }}
                          .change {{
                            margin-top: 15px;
                            padding: 10px;
                            background: #f1f1f1;
                            border-left: 4px solid #4CAF50;
                          }}
                          .price-old {{
                            text-decoration: line-through;
                            color: #999;
                            margin-right: 5px;
                          }}
                          .price-new {{
                            color: #d32f2f;
                            font-weight: bold;
                          }}
                          .stock {{
                            font-weight: bold;
                          }}
                        </style>
                      </head>
                      <body>
                        <div class='card'>
                          <div class='header'>Ürün Takip Güncellemesi</div>
                          <div class='content'>
                            <h2>{trackedItem.Title}</h2>
                            <img src='{trackedItem.ImageUrl}' alt='{trackedItem.Title}' />
                            <div class='change'>
                              {(trackedItem.Price != freshTrackedItem.Price ?
                                                    $"<div>Fiyat değişti: <span class='price-old'>{trackedItem.Price.ToString()}</span> → <span class='price-new'>{freshTrackedItem.Price.ToString()}</span></div>"
                                                    : "")}
                              {(trackedItem.IsInStock != freshTrackedItem.IsInStock ?
                                                    $"<div>Stok durumu değişti: <span class='stock'>{(trackedItem.IsInStock ? "Stokta Var" : "Tükendi")}</span> → <span class='stock'>{(freshTrackedItem.IsInStock ? "Stokta Var" : "Tükendi")}</span></div>"
                                                    : "")}
                            </div>
                            <a href='{trackedItem.Url}' target='_blank'>Ürünü Görüntüle</a>
                          </div>
                        </div>
                      </body>
                    </html>";

                    await emailService.SendEmailAsync(account.Email, subject, htmlBody);


                    trackedItem.IsActive = false;
                }

                trackedItem.LastCheckedUtc = DateTime.UtcNow;
                await trackedItemRepository.Update(trackedItem);
            }
        }
    }
}
