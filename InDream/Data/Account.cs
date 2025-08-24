using InDream.Enumeration;
using InDream.Interfaces;

namespace InDream.Data
{
    public class Account : Entity
    {
        public string GoogleId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public SubscriptionTierEnum SubscriptionTier { get; set; }
        public DateTime? SubscriptionExpiryDate { get; set; }

       

        public IList<TrackedItem> TrackedItems { get; set; }    
    }
}
