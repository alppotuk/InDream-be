using AngleSharp;
using InDream.Common.BaseModels;
using InDream.Common.Interfaces;
using InDream.Models.SiteScraper;
using System;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace InDream.SiteScrapers;

public class MangoScraper : ISiteScraper, IInjectAsScoped
{
    private readonly IHttpClientFactory _httpClientFactory;
    public MangoScraper(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    public bool CanHandle(Uri uri)
    {
        return uri.Host.ToLower().Contains("shop.mango.com");
    }

    public async Task<ResponseBase<ScrapedProductModel?>> ScrapeProductInfoAsync(string url)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/117.0.0.0 Safari/537.36");

            var mainPageResponse = await client.GetAsync(url);
            if (!mainPageResponse.IsSuccessStatusCode) 
                return new ResponseBase<ScrapedProductModel?>(false, null);

            var productId = url.Split('_').LastOrDefault()?.Split('?').FirstOrDefault();
            if(productId == null)
                return new ResponseBase<ScrapedProductModel?>(false, null);

            var htmlContent = await mainPageResponse.Content.ReadAsStringAsync();
            var context = BrowsingContext.New(Configuration.Default);
            var document = await context.OpenAsync(req => req.Content(htmlContent));


            var titleElement = document.QuerySelector("h1[class*='ProductDetail_title']");
            var title = titleElement?.TextContent.Trim() ?? "Başlık Bulunamadı";
            var imageUrl = document.QuerySelector("img.SlideshowWrapper_image__J48xz")?.GetAttribute("src");


            var sizeElements = document.QuerySelectorAll("ol[class^='SizesList_sizesList'] li button");
            var sizeNames = sizeElements.Select(el => el.TextContent.Trim()).ToList();

            var colorMap = new Dictionary<string, string>();
            var colorElements = document.QuerySelectorAll("ul[class*='ColorList_colorsList'] li");
            foreach (var colorLi in colorElements)
            {
                var img = colorLi.QuerySelector("img");
                var link = colorLi.QuerySelector("a");

                var colorName = img?.GetAttribute("alt")?.Replace("Seçilen renk ", "").Replace("Renk ", "").Trim();
                var href = link?.GetAttribute("href");
                var src = img?.GetAttribute("src");

                var colorCode = ExtractColorCode(href, src);

                if (colorCode != null && colorName != null && !colorMap.ContainsKey(colorCode))
                {
                    colorMap.Add(colorCode, colorName);
                }
            }

            var priceApiUrl = $"https://online-orchestrator.mango.com/v3/prices/products?countryIso=TR&channelId=shop&productId={productId}";
            var stockApiUrl = $"https://online-orchestrator.mango.com/v3/stock/products?countryIso=TR&channelId=shop&productId={productId}";

            var priceTask = client.GetStringAsync(priceApiUrl);
            var stockTask = client.GetStringAsync(stockApiUrl);

            await Task.WhenAll(priceTask, stockTask); 

            var priceJson = await priceTask;
            var stockJson = await stockTask;

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var prices = JsonSerializer.Deserialize<Dictionary<string, MangoPriceDetail>>(priceJson, options);
            var stocks = JsonSerializer.Deserialize<MangoStockResponse>(stockJson, options);

            if (prices == null || stocks == null) 
                return new ResponseBase<ScrapedProductModel?>(false, null);

            var scrapedProduct = new ScrapedProductModel
            {
                Title = title,
                ImageUrl = imageUrl
            };

            foreach (var colorStockEntry in stocks.Colors)
            {
                var colorCode = colorStockEntry.Key;

                var colorName = colorMap.TryGetValue(colorCode, out var name) ? name : colorCode;
                var price = prices.TryGetValue(colorCode, out var priceDetail) ? priceDetail.Price : 0;

                foreach (var sizeStockEntry in colorStockEntry.Value.Sizes)
                {
                    var sizeCode = sizeStockEntry.Key;
                    var sizeStockData = sizeStockEntry.Value;


                    var info = new ScrapedProductPropertiesModel
                    {
                        Price = price,
                        IsInStock = sizeStockData.Available,
                        PropertyTexts = new List<string> { colorName, sizeCode }
                    };
                    scrapedProduct.Properties.Add(info);
                }
            }

            return new ResponseBase<ScrapedProductModel?>(true, scrapedProduct);
        }
        catch (Exception ex)
        {
            //TODO : mail to admin
            return new ResponseBase<ScrapedProductModel?>(false, null);
        }
    }

    private string? ExtractColorCode(string? href, string? src)
    {
        if (!string.IsNullOrEmpty(href))
        {
            var match = Regex.Match(href, @"c=(\w+)");
            if (match.Success) return match.Groups[1].Value;
        }
        if (!string.IsNullOrEmpty(src))
        {
            var match = Regex.Match(src, @"_(\w+)_C\.png");
            if (match.Success) return match.Groups[1].Value;
        }
        return null;
    }
}
