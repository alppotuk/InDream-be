using InDream.Api.Features.Resource.Data;
using InDream.Api.Features.Resource.Models.Response;
using InDream.Core.BaseModels;
using InDream.Core.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InDream.Api.Features.Resource;

[Route("[controller]/[action]")]
public class ResourceController : BaseController
{
    private readonly IRepository<ResourceLanguage> _resourceLanguageRepository;
    private readonly IRepository<ResourceStringText> _resourceStringTextRepository;

    public ResourceController(IRepository<ResourceStringText> resourceStringTextRepository, IRepository<ResourceLanguage> resourceLanguageRepository)
    {
        _resourceStringTextRepository = resourceStringTextRepository;
        _resourceLanguageRepository = resourceLanguageRepository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ResponseBase<List<ResourceStringModel>>> Strings(string languageCode = "en")
    {
        var resourceStrings = await _resourceStringTextRepository
            .Table
            .Where(p => p.ResourceLanguage.LanguageCode == languageCode)
            .Select(p => new ResourceStringModel
            {
                ResourceCode = p.ResourceString.ResourceCode,
                Text = p.Text
            }).ToListAsync();

        return new ResponseBase<List<ResourceStringModel>>(true, resourceStrings);
    }

    [HttpGet]
    public async Task<ResponseBase<List<SelectListItem>>> Languages()
    {
        var languages = await _resourceLanguageRepository
            .Table
            .Select(p => new SelectListItem
            {
                Text = p.LanguageName,
                Value = p.LanguageCode,
            }).ToListAsync();

        return new ResponseBase<List<SelectListItem>>(true, languages);
    }
}
