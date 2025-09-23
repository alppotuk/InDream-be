using InDream.Api.Features.Resource.Data;
using InDream.Core.BaseModels;
using InDream.Core.DI;
using InDream.Core.Repository;
using Microsoft.EntityFrameworkCore;

namespace InDream.Api.Features.Resource;

public class ResourceService : IInjectAsScoped
{
 
    private readonly IRepository<ResourceStringText> _resourceStringTextRepository;

    public ResourceService(IRepository<ResourceStringText> resourceStringTextRepository)
    {
        _resourceStringTextRepository = resourceStringTextRepository;
    }

    public async Task<string> Localize(string resourceCode,  string languageCode)
    {
        var resourceStringText = await _resourceStringTextRepository
            .Table
            .Where(p => p.ResourceLanguage.LanguageCode == languageCode && p.ResourceString.ResourceCode == resourceCode)
            .Select(p => p.Text)
            .FirstOrDefaultAsync();

        return resourceStringText ?? resourceCode;
    }
}
