using InDream.Api.Common.Enums;
using InDream.Api.Features.Tracking.Data;
using InDream.Core.BaseModels;
using System.ComponentModel.DataAnnotations;

namespace InDream.Api.Features.Authentication.Data;

public class Account : EntityBase
{
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public SubscriptionTierEnum SubscriptionTier { get; set; }
    public DateTime? SubscriptionExpiryDate { get; set; }

    [MaxLength(5)]
    public string? LanguageCode { get; set; }

   

    public IList<TrackedItem> TrackedItems { get; set; }    
}
