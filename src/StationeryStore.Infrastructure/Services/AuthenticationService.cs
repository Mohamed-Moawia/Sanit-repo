using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StationeryStore.Application.Interfaces;
using StationeryStore.Domain.Entities;
using StationeryStore.Infrastructure.Data;

namespace StationeryStore.Infrastructure.Services;

/// <summary>
/// Authentication service implementation with JWT support
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly StoreDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IEgyptianVatService _vatService;
    
    public AuthenticationService(
        StoreDbContext context,
        IConfiguration configuration,
        IEgyptianVatService vatService)
    {
        _context = context;
        _configuration = configuration;
        _vatService = vatService;
    }
    
    public async Task<AuthenticationResult> AuthenticateAsync(
        string usernameOrEmail,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .Include(u => u.DefaultBranch)
            .FirstOrDefaultAsync(u =>
                (u.Username == usernameOrEmail || u.Email == usernameOrEmail) &&
                !u.IsDeleted,
                cancellationToken);
        
        if (user == null)
        {
            throw new InvalidOperationException("Invalid username or password / اسم المستخدم أو كلمة المرور غير صحيحة");
        }
        
        if (!user.IsActive)
        {
            throw new InvalidOperationException("Account is disabled / الحساب معطل");
        }
        
        // Verify password (BCrypt)
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            // Track failed login attempts
            user.FailedLoginAttempts++;
            if (user.FailedLoginAttempts >= 5)
            {
                user.LockedUntil = DateTime.UtcNow.AddMinutes(15);
            }
            await _context.SaveChangesAsync(cancellationToken);
            
            throw new InvalidOperationException("Invalid username or password / اسم المستخدم أو كلمة المرور غير صحيحة");
        }
        
        // Check if account is locked
        if (user.LockedUntil.HasValue && user.LockedUntil > DateTime.UtcNow)
        {
            throw new InvalidOperationException($"Account is locked until {user.LockedUntil.Value:HH:mm} / الحساب مقفل حتى {user.LockedUntil.Value:HH:mm}");
        }
        
        // Reset failed attempts on successful login
        user.FailedLoginAttempts = 0;
        user.LockedUntil = null;
        user.LastLoginDate = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        
        // Get user permissions
        var permissions = await GetUserPermissionsAsync(user.RoleId, cancellationToken);
        
        // Generate tokens
        var token = GenerateJwtToken(user, new[] { user.Role.Code }, permissions);
        var refreshToken = GenerateRefreshToken();
        
        // Save refresh token
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(
            _configuration.GetValue<int>("JwtSettings:RefreshTokenExpiryInDays", 7));
        await _context.SaveChangesAsync(cancellationToken);
        
        return new AuthenticationResult(
            user.Id,
            user.Username,
            user.FullNameAr,
            user.FullNameEn,
            user.Email,
            user.Role.NameEn,
            user.RoleId,
            user.DefaultBranchId,
            token,
            refreshToken,
            _configuration.GetValue<int>("JwtSettings:ExpiryInMinutes", 480),
            user.Language
        );
    }
    
    public async Task<AuthenticationResult> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default)
    {
        var user = await _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && !u.IsDeleted, cancellationToken);
        
        if (user == null || !user.IsActive)
        {
            throw new InvalidOperationException("Invalid refresh token / رمز التحديث غير صالح");
        }
        
        if (user.RefreshTokenExpiry < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Refresh token expired / رمز التحديث منتهي الصلاحية");
        }
        
        var permissions = await GetUserPermissionsAsync(user.RoleId, cancellationToken);
        var token = GenerateJwtToken(user, new[] { user.Role.Code }, permissions);
        var newRefreshToken = GenerateRefreshToken();
        
        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(
            _configuration.GetValue<int>("JwtSettings:RefreshTokenExpiryInDays", 7));
        await _context.SaveChangesAsync(cancellationToken);
        
        return new AuthenticationResult(
            user.Id,
            user.Username,
            user.FullNameAr,
            user.FullNameEn,
            user.Email,
            user.Role.NameEn,
            user.RoleId,
            user.DefaultBranchId,
            token,
            newRefreshToken,
            _configuration.GetValue<int>("JwtSettings:ExpiryInMinutes", 480),
            user.Language
        );
    }
    
    public async Task<bool> ChangePasswordAsync(
        Guid userId,
        string currentPassword,
        string newPassword,
        CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null || user.IsDeleted)
            return false;
        
        // Verify current password
        if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            return false;
        
        // Hash and save new password
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.LastPasswordChangeDate = DateTime.UtcNow;
        user.MustChangePassword = false;
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
    
    public async Task<bool> LogoutAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null || user.IsDeleted)
            return false;
        
        user.RefreshToken = null;
        user.RefreshTokenExpiry = null;
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
    
    public string GenerateJwtToken(User user, IEnumerable<string> roles, IEnumerable<string> permissions)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = Encoding.UTF8.GetBytes(jwtSettings["Secret"]!);
        
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("FullNameAr", user.FullNameAr),
            new("FullNameEn", user.FullNameEn),
            new("RoleId", user.RoleId.ToString()),
            new("BranchId", user.DefaultBranchId?.ToString() ?? string.Empty),
            new("Language", user.Language),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        
        // Add roles
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }
        
        // Add permissions
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("Permission", permission));
        }
        
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtSettings.GetValue<int>("ExpiryInMinutes", 480)),
            signingCredentials: credentials
        );
        
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
    
    private async Task<List<string>> GetUserPermissionsAsync(Guid roleId, CancellationToken cancellationToken)
    {
        return await _context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Join(_context.Permissions,
                rp => rp.PermissionId,
                p => p.Id,
                (rp, p) => p.Code)
            .ToListAsync(cancellationToken);
    }
}
