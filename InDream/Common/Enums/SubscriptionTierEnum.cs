using System.ComponentModel;

namespace InDream.Api.Common.Enums;

public enum SubscriptionTierEnum: int
{
    [Description("Free")]
    Free = 10,

    [Description("Premium")]
    Premium = 20,
}
