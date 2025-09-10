using InDream.Common.Helpers;
using InDream.Data;
using InDream.Enumeration;
using InDream.Interfaces;
using InDream.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InDream.Controllers;

[Route("[controller]/[action]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IRepository<Account> _accountRepository;
    private readonly IConfiguration _configuration;

    public AuthenticationController(IRepository<Account> accountRepository, IConfiguration configuration)
    {
        _accountRepository = accountRepository;
        _configuration = configuration;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ResponseBase<bool>> Register(RegisterModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
            return new ResponseBase<bool>(false, false, message: "Email and password cannot be empty!");

        var accountExists = await _accountRepository
            .Table
            .AnyAsync(p => p.Email == model.Email);
        
        if(accountExists)
            return new ResponseBase<bool>(false, false, message: "Account with given email already exists!");

        var hashedPassword = AuthHelper.HashPassword(model.Password);

        var account = new Account
        {
            Email = model.Email,
            PasswordHash = hashedPassword,
            CreationDateUtc = DateTime.UtcNow,
            SubscriptionTier = SubscriptionTierEnum.Free,
        };

        await _accountRepository.Create(account);

        return new ResponseBase<bool>(true, true);
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

}
