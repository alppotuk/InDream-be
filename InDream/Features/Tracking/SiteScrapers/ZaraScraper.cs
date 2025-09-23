using InDream.Api.Common.Enums;
using InDream.Core.BaseModels;
using InDream.Core.DI;
using InDream.Features.Tracking.Models.SiteScraper;
using System.Text.Json;

namespace InDream.Features.Tracking.SiteScrapers;

public class ZaraScraper : ISiteScraper, IInjectAsScoped
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ZaraScraper(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public bool CanHandle(Uri uri)
    {
        return uri.Host.ToLower().Contains("zara.com");
    }

    public async Task<ResponseBase<ScrapedProductModel?>> ScrapeProductInfoAsync(string url)
    {
        try
        {
            var apiUrl = new UriBuilder(url)
            {
                Query = (new Uri(url).Query.TrimStart('?') + "&ajax=true").TrimStart('&')
            }.Uri.ToString();

            var client = _httpClientFactory.CreateClient();

            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

            var response = await client.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
                return new ResponseBase<ScrapedProductModel?>(false, null);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var zaraResponse = await JsonSerializer.DeserializeAsync<ZaraProductResponse>(await response.Content.ReadAsStreamAsync(), options);

            if (zaraResponse?.Product == null)
                return new ResponseBase<ScrapedProductModel?>(false, null);

            var product = zaraResponse.Product;
            var scrapedProduct = new ScrapedProductModel
            {
                Title = product.Name ?? "",
                ProductBrand = ProductBrandEnum.Zara,
            };

            foreach (var color in product.Detail?.Colors ?? Enumerable.Empty<ZaraColor>())
            {
                if (string.IsNullOrEmpty(scrapedProduct.ImageUrl))
                {
                    var imageUrl = color.MainImgs?.FirstOrDefault()?.Url;
                    if (!string.IsNullOrEmpty(imageUrl))
                        scrapedProduct.ImageUrl = imageUrl;
                }

                foreach (var size in color.Sizes ?? Enumerable.Empty<ZaraSize>())
                {
                    var info = new ScrapedProductPropertiesModel
                    {
                        Price = (decimal)size.Price / 100,
                        IsInStock = size.Availability == "in_stock",
                        PropertyTexts = new List<string>
                            {
                                color.Name ?? "Unknown Color",
                                size.Name ?? "Unknown Size"
                            }
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

    
}
