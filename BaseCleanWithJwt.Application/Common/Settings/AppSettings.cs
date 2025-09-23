namespace BaseCleanWithJwt.Domain.Common.Settings;

public class AppSettings
{
    public string AllowOrigins { get; set; } = null!;
    public TimeSpan TimeOffset { get; set; }
}