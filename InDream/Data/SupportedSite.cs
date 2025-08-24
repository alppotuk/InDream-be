using InDream.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace InDream.Data
{
    public class SupportedSite : Entity
    {
        public string Domain { get; set; } 
        public string TitleSelector { get; set; }
        public string PriceSelector { get; set; }
        public string StockSelector { get; set; }
        public string ImageUrlSelector { get; set; }
    }
}
