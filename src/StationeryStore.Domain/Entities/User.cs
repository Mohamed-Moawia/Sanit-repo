using StationeryStore.Domain.Common;
using StationeryStore.Domain.Enums;

namespace StationeryStore.Domain.Entities;

/// <summary>
/// User entity for authentication and authorization
/// </summary>
public class User : BaseEntity
{
    /// <summary>
    /// User identification
    /// </summary>
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    
    /// <summary>
    /// Personal information
    /// </summary>
    public string FullNameAr { get; set; } = string.Empty;
    public string FullNameEn { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    
    /// <summary>
    /// Role and permissions
    /// </summary>
    public Guid RoleId { get; set; }
    public Guid? DefaultBranchId { get; set; }
    
    /// <summary>
    /// Authentication tracking
    /// </summary>
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    public int FailedLoginAttempts { get; set; }
    public DateTime? LockedUntil { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public DateTime? LastPasswordChangeDate { get; set; }
    
    /// <summary>
    /// User status
    /// </summary>
    public bool IsActive { get; set; } = true;
    public bool MustChangePassword { get; set; }
    public string Language { get; set; } = "ar";  // Default Arabic
    
    // Navigation properties
    public virtual Role Role { get; set; } = null!;
    public virtual Branch? DefaultBranch { get; set; }
    public virtual ICollection<UserBranch> BranchAccess { get; set; } = new List<UserBranch>();
}

/// <summary>
/// Role entity for RBAC
/// </summary>
public class Role : BaseEntity
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<RolePermission> Permissions { get; set; } = new List<RolePermission>();
}

/// <summary>
/// Permission entity for fine-grained access control
/// </summary>
public class Permission : BaseEntity
{
    public string NameAr { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
}

/// <summary>
/// Role-Permission junction entity
/// </summary>
public class RolePermission : BaseEntity
{
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }
    
    // Navigation properties
    public virtual Role Role { get; set; } = null!;
    public virtual Permission Permission { get; set; } = null!;
}

/// <summary>
/// User-Branch access junction entity
/// </summary>
public class UserBranch : BaseEntity
{
    public Guid UserId { get; set; }
    public Guid BranchId { get; set; }
    public bool IsDefault { get; set; }
    
    // Navigation properties
    public virtual User User { get; set; } = null!;
    public virtual Branch Branch { get; set; } = null!;
}
