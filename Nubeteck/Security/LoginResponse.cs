namespace Nubeteck.Extensions.Security;

public class LoginResponse
{
    public required string Token { get; set; }
    public required DateTime ExpiryDate { get; set; }
    public string? RefreshToken { get; set; }
}

public record LoginCommandResponse(TokenInfo Token);
