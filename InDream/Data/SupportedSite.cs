using InDream.Common.BaseModels;

namespace InDream.Data
{
    public class SupportedSite : EntityBase
    {
        public string Domain { get; set; } 
        public string TitleSelector { get; set; }
        public string PriceSelector { get; set; }
        public string StockSelector { get; set; }
        public string ImageUrlSelector { get; set; }
    }
}
