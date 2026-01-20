namespace CoopCloud.GeneralSettingsAndAuth.Features.Auth.Application.DTOs;

// ==================== Authentication DTOs ====================

public record RegisterRequest(
    string Email,
    string Username,
    string FullName,
    string Password,
    string? PhoneNumber = null
);

public record LoginRequest(
    string Email,
    string Password
);

public record LoginResponse(
    string Token,
    string RefreshToken,
    DateTime ExpiresAt,
    UserDto User
);

public record RefreshTokenRequest(
    string RefreshToken
);

public record ChangePasswordRequest(
    string CurrentPassword,
    string NewPassword
);

public record ResetPasswordRequest(
    string Email
);

// ==================== Token Validation DTOs ====================

public record ValidateTokenRequest(
    string Token
);

public record ValidateTokenResponse(
    bool IsValid,
    string? UserId,
    string? Email,
    string? Username,
    string? FullName,
    IEnumerable<string> Roles,
    IEnumerable<string> Permissions,
    DateTime? ExpiresAt,
    string? Error = null
);

// ==================== User DTOs ====================

public record UserDto(
    Guid Id,
    string Email,
    string Username,
    string FullName,
    string? PhoneNumber,
    bool IsActive,
    DateTime? LastLoginAt,
    DateTime CreatedAt,
    IEnumerable<string> Roles
);

public record UserDetailDto(
    Guid Id,
    string Email,
    string Username,
    string FullName,
    string? PhoneNumber,
    bool IsActive,
    DateTime? LastLoginAt,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IEnumerable<RoleDto> Roles,
    IEnumerable<string> Permissions
);

public record CreateUserRequest(
    string Email,
    string Username,
    string FullName,
    string Password,
    string? PhoneNumber,
    IEnumerable<Guid>? RoleIds = null
);

public record UpdateUserRequest(
    string? Email,
    string? Username,
    string? FullName,
    string? PhoneNumber,
    bool? IsActive
);

// ==================== Role DTOs ====================

public record RoleDto(
    Guid Id,
    string Name,
    DateTime CreatedAt,
    int UserCount,
    int PermissionCount
);

public record RoleDetailDto(
    Guid Id,
    string Name,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    IEnumerable<PermissionDto> Permissions,
    IEnumerable<UserDto> Users
);

public record CreateRoleRequest(
    string Name,
    IEnumerable<Guid>? PermissionIds = null
);

public record UpdateRoleRequest(
    string? Name
);

// ==================== Permission DTOs ====================

public record PermissionDto(
    Guid Id,
    string Name,
    string Key,
    bool IsActive,
    DateTime CreatedAt
);

public record CreatePermissionRequest(
    string Name,
    string Key
);

public record UpdatePermissionRequest(
    string? Name,
    string? Key,
    bool? IsActive
);

// ==================== Assignment DTOs ====================

public record AssignRolesToUserRequest(
    IEnumerable<Guid> RoleIds
);

public record AssignPermissionsToRoleRequest(
    IEnumerable<Guid> PermissionIds
);
