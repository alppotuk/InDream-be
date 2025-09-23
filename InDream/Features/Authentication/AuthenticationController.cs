using InDream.Api.Common.Enums;
using InDream.Api.Features.Authentication.Data;
using InDream.Api.Features.Authentication.Models;
using InDream.Common.Helpers;
using InDream.Core.BaseModels;
using InDream.Core.Repository;
using InDream.Core.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;

namespace InDream.Features.Authentication;

[Route("[controller]/[action]")]
public class AuthenticationController : BaseController
{
    private readonly IRepository<Account> _accountRepository;
    private readonly IConfiguration _configuration;
    private readonly IApiDescriptionGroupCollectionProvider _apiDescProvider;
    
    public AuthenticationController(IRepository<Account> accountRepository, IConfiguration configuration, IApiDescriptionGroupCollectionProvider apiDescProvider)
    {
        _accountRepository = accountRepository;
        _configuration = configuration;
        _apiDescProvider = apiDescProvider;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ResponseBase<LoginResponseModel?>> Register(RegisterModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            return new ResponseBase<LoginResponseModel?>(false, null, message: "Email and password cannot be empty.");

        if(!God.IsEmailValid(model.Email))
            return new ResponseBase<LoginResponseModel?>(false, null, message: "Invalid email format.");

        if(!God.IsPasswordValid(model.Password))
            return new ResponseBase<LoginResponseModel?>(false, null, message: "Password must be at least 8 characters and must contain at least one uppercase letter, one lower case letter and one digit.");


        var accountExists = await _accountRepository
            .Table
            .AnyAsync(p => p.Email == model.Email);
        
        if(accountExists)
            return new ResponseBase<LoginResponseModel?>(false, null, message: "Account with given email already exists!");

        var hashedPassword = AuthHelper.HashPassword(model.Password);

        var account = new Account
        {
            Email = model.Email,
            PasswordHash = hashedPassword,
            CreationDateUtc = DateTime.UtcNow,
            SubscriptionTier = SubscriptionTierEnum.Free,
        };

        await _accountRepository.Create(account);

        var tokenResponse = AuthHelper.GenerateToken(account, _configuration);

        return new ResponseBase<LoginResponseModel?>(true, tokenResponse);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ResponseBase<LoginResponseModel?>> Login(LoginModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            return new ResponseBase<LoginResponseModel?>(false, null, message: "Email and password cannot be empty.");

        var account = await _accountRepository
            .Table
            .FirstOrDefaultAsync(p => p.Email == model.Email);

        if (account == null || !AuthHelper.VerifyPassword(account.PasswordHash, model.Password))
            return new ResponseBase<LoginResponseModel?> (false, null, message: "Email or password not valid.");

        var tokenResponse = AuthHelper.GenerateToken(account, _configuration);

        return new ResponseBase<LoginResponseModel?>(true, tokenResponse);
    }

    [HttpPost]
    public async Task<ResponseBase<LoginResponseModel?>> ChangeAccountLanguage(ChangeAccountLanguageModel model)
    {
        var account = await _accountRepository
            .Table
            .FirstOrDefaultAsync(p => p.Id == IdentityId);

        if (account == null)
            return new ResponseBase<LoginResponseModel?>(false, null, message: "common_error");

        account.LanguageCode = model.LanguageCode;

        var tokenResponse = AuthHelper.GenerateToken(account, _configuration);

        return new ResponseBase<LoginResponseModel?>(true, tokenResponse);
    }

}
