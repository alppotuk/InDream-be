using InDream.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InDream.Models.TrackedItem
{
    public class TrackedItemModel
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string PriceText { get; set; }
        public string StockText { get; set; }

        public DateTime LastTimeChecked { get; set; }
    }
}
