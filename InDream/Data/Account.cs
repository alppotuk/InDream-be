using InDream.Enumeration;
using InDream.Interfaces;

namespace InDream.Data
{
    public class Account : Entity
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public SubscriptionTierEnum SubscriptionTier { get; set; }
        public DateTime? SubscriptionExpiryDate { get; set; }

       

        public IList<TrackedItem> TrackedItems { get; set; }    
    }
}
