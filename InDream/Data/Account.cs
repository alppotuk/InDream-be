using InDream.Enumeration;

namespace InDream.Data
{
    public class Account
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public SubscriptionTierEnum SubscriptionTier { get; set; } = SubscriptionTierEnum.Free;
    }
}
