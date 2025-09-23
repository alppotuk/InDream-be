using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InDream.Core.BaseModels;

[ApiController]
[Authorize]
public abstract class BaseController : ControllerBase
{
    protected int IdentityId
    {
        get
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
                throw new InvalidOperationException("User ID is not found!");
            return int.Parse(claim.Value);
        }
    }

    protected string LanguageCode
    {
        get
        {
            var claim = User.FindFirst("languageCode"); 
            if (claim == null)
                return "en";
            return claim.Value;
        }
    }
}

