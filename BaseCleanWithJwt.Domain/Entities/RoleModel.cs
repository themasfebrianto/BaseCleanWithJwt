using BaseCleanWithJwt.Domain.Common;

namespace BaseCleanWithJwt.Domain.Entities;

public class RoleModel : BaseActivityModel
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
}