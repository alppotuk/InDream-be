using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InDream.Controllers;

[ApiController]
[Authorize]
public abstract class BaseController : ControllerBase
{
    protected int IdentityId
    {
        get
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(value))
            {
                throw new InvalidOperationException("User ID is not found!");
            }
            return int.Parse(value);
        }
    }
}

