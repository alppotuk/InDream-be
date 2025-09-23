using InDream.Core.BaseModels;
using System.ComponentModel.DataAnnotations;

namespace InDream.Api.Features.Resource.Data;

public class ResourceLanguage : EntityBase
{
    [MaxLength(100)]
    public string LanguageName { get; set; }
    [MaxLength(5)]
    public string LanguageCode { get; set; }
}
