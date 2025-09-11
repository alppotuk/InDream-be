namespace InDream.Models.Authentication;

public class LoginResponseModel
{
    public string Token { get; set; }
    public DateTime Expires { get; set; }
}
