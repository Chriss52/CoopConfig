namespace Nubeteck.Extensions.Security;

public record TokenInfo
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
}
