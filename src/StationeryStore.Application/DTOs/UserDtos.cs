using StationeryStore.Domain.Enums;

namespace StationeryStore.Application.DTOs;

/// <summary>
/// User DTO for API responses
/// </summary>
public record UserDto(
    Guid Id,
    string Username,
    string Email,
    string FullNameAr,
    string FullNameEn,
    string? Phone,
    string? Mobile,
    Guid RoleId,
    string RoleName,
    Guid? DefaultBranchId,
    string? DefaultBranchName,
    bool IsActive,
    string Language,
    DateTime? LastLoginDate,
    DateTime CreatedAt
);

/// <summary>
/// User detail DTO with full information
/// </summary>
public record UserDetailDto(
    Guid Id,
    string Username,
    string Email,
    string FullNameAr,
    string FullNameEn,
    string? Phone,
    string? Mobile,
    Guid RoleId,
    string RoleName,
    string RoleNameAr,
    Guid? DefaultBranchId,
    string? DefaultBranchName,
    bool IsActive,
    bool MustChangePassword,
    string Language,
    DateTime? LastLoginDate,
    DateTime? LastPasswordChangeDate,
    List<Guid> BranchAccess,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

/// <summary>
/// Login request
/// </summary>
public record LoginRequest(
    string UsernameOrEmail,
    string Password
);

/// <summary>
/// Login response
/// </summary>
public record LoginResponse(
    Guid UserId,
    string Username,
    string FullNameAr,
    string FullNameEn,
    string Email,
    string Role,
    Guid RoleId,
    Guid? DefaultBranchId,
    string Token,
    string RefreshToken,
    int ExpiresInMinutes,
    string Language
);

/// <summary>
/// Request to create a user
/// </summary>
public record CreateUserRequest(
    string Username,
    string Email,
    string Password,
    string FullNameAr,
    string FullNameEn,
    string? Phone,
    string? Mobile,
    Guid RoleId,
    Guid? DefaultBranchId,
    string Language = "ar",
    List<Guid>? BranchAccess = null
);

/// <summary>
/// Request to update a user
/// </summary>
public record UpdateUserRequest(
    string Email,
    string FullNameAr,
    string FullNameEn,
    string? Phone,
    string? Mobile,
    Guid RoleId,
    Guid? DefaultBranchId,
    bool IsActive,
    string Language,
    List<Guid>? BranchAccess = null
);

/// <summary>
/// Change password request
/// </summary>
public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword
);

/// <summary>
/// User search parameters
/// </summary>
public record UserSearchParameters(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    Guid? RoleId = null,
    bool? IsActive = null
);

/// <summary>
/// Role DTO
/// </summary>
public record RoleDto(
    Guid Id,
    string NameAr,
    string NameEn,
    string Code,
    string? Description,
    bool IsSystemRole,
    bool IsActive,
    List<string> Permissions
);

/// <summary>
/// Permission DTO
/// </summary>
public record PermissionDto(
    Guid Id,
    string NameAr,
    string NameEn,
    string Code,
    string Resource,
    string Action,
    string? Description,
    int DisplayOrder
);
