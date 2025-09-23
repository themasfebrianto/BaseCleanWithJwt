namespace BaseCleanWithJwt.Domain.Common.Settings;

public class JwtSettings
{
    public string IssuerSigningKey { get; set; } = null!;
    public string ValidIssuer { get; set; } = null!;
    public string ValidAudience { get; set; } = null!;
    public TimeSpan Expiration { get; set; } = TimeSpan.FromDays(15);
    public TimeSpan RefreshTokenExpiration { get; set; } = TimeSpan.FromDays(30);
}