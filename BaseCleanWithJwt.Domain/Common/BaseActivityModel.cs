namespace BaseCleanWithJwt.Domain.Common;

public abstract class BaseActivityModel : BaseModel
{
    public bool IsActive { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public string? CreatedBy { get; set; }
    public string? UpdatedBy { get; set; }
}