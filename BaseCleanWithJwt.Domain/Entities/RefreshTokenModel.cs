using BaseCleanWithJwt.Domain.Common;

namespace BaseCleanWithJwt.Domain.Entities;

public class RefreshTokenModel : BaseModel
{
    public string? Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public string? CreatedByIp { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
    public bool IsActive => RevokedAt == null && !IsExpired;
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}