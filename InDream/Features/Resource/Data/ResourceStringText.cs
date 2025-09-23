using InDream.Core.BaseModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InDream.Api.Features.Resource.Data;

public class ResourceStringText: EntityBase
{
    public long ResourceStringId { get; set; }
    [ForeignKey("ResourceStringId")]
    public ResourceString ResourceString { get; set; }

    public long ResourceLanguageId { get; set; }
    [ForeignKey("ResourceLanguageId")]
    public ResourceLanguage ResourceLanguage { get; set; }


    public string Text { get; set; }
}
