namespace InDream.Api.Features.Authentication.Models;

public class LoginResponseModel
{
    public string Token { get; set; }
    public DateTime Expires { get; set; }
}
