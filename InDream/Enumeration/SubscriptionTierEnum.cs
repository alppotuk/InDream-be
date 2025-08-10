using System.ComponentModel;

namespace InDream.Enumeration
{
    public enum SubscriptionTierEnum: int
    {
        [Description("Free")]
        Free = 10,

        [Description("Premium")]
        Premium = 20,
    }
}
