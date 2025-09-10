using InDream.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InDream.Data
{
    public class TrackedItem : EntityBase
    {
        public long AccountId { get; set; }
        [ForeignKey("AccountId")]
        public Account Account { get; set; }

        public string Url { get; set; }

        [MaxLength(100)]
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        [MaxLength(50)]
        public string PriceText { get; set; }
        [MaxLength(50)]
        public string StockText { get; set; }



        [MaxLength(50)]
        public string? TitleCssSelector { get; set; }
        [MaxLength(50)]
        public string? ImageUrlCssSelector { get; set; }
        [MaxLength(50)]
        public string? PriceCssSelector { get; set; }
        [MaxLength(50)]
        public string? StockTextCssSelector { get; set; }

        public DateTime LastTimeChecked { get; set; }

        public bool IsActive { get; set; }

    }
}
