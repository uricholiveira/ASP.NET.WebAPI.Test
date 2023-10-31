namespace Data.Models.Response;

public class CreateUserResponse
{
    public bool HasCreated { get; set; }
}

public class LoginResponse
{
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
}