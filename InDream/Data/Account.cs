using InDream.Common.BaseModels;
using InDream.Enumeration;

namespace InDream.Data
{
    public class Account : EntityBase
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public SubscriptionTierEnum SubscriptionTier { get; set; }
        public DateTime? SubscriptionExpiryDate { get; set; }

       

        public IList<TrackedItem> TrackedItems { get; set; }    
    }
}
