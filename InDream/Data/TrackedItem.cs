using InDream.Common.BaseModels;
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
        public decimal Price { get; set; }
        [MaxLength(50)]
        public string? CurrencyCode { get; set; }

        public bool IsInStock { get; set; }

        public string PropertiesSerialized { get; set; }


        public DateTime LastCheckedUtc { get; set; }

        public bool IsActive { get; set; }

    }
}
