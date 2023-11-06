namespace Security.Models;

public class JwtToken
{
    public int Expiration { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}