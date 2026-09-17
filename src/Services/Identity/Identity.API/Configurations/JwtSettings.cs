namespace Identity.API.Configurations;

public class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string SecurityKey { get; set; } = string.Empty;
    public int AccessTokenExpirationInMinutes { get; set; }
}