using InDream.Core.BaseModels;
using System.ComponentModel.DataAnnotations;

namespace InDream.Api.Features.Resource.Data;

public class ResourceString : EntityBase
{
    [MaxLength(250)]
    public string ResourceCode { get; set; }
    public IList<ResourceStringText> Texts { get; set; }
}
