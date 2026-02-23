using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StationeryStore.Application.Common;
using StationeryStore.Application.DTOs;
using StationeryStore.Application.Interfaces;
using StationeryStore.Domain.Entities;
using StationeryStore.Infrastructure.Data;

namespace StationeryStore.API.Controllers;

/// <summary>
/// Authentication and authorization controller
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly StoreDbContext _context;
    private readonly ILogger<AuthController> _logger;
    
    public AuthController(
        IAuthenticationService authenticationService,
        StoreDbContext context,
        ILogger<AuthController> logger)
    {
        _authenticationService = authenticationService;
        _context = context;
        _logger = logger;
    }
    
    /// <summary>
    /// Login with username/email and password
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authenticationService.AuthenticateAsync(
                request.UsernameOrEmail,
                request.Password,
                HttpContext.RequestAborted);
            
            var response = new LoginResponse(
                result.UserId,
                result.Username,
                result.FullNameAr,
                result.FullNameEn,
                result.Email,
                result.Role,
                result.RoleId,
                result.DefaultBranchId,
                result.Token,
                result.RefreshToken,
                result.ExpiresInMinutes,
                result.Language);
            
            return Ok(ApiResponse<LoginResponse>.SuccessResponse(response, "Login successful"));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Login failed: {Error}", ex.Message);
            return Unauthorized(ApiResponse<LoginResponse>.FailureResponse(ex.Message));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Login error");
            return StatusCode(500, ApiResponse<LoginResponse>.FailureResponse("An error occurred during login"));
        }
    }
    
    /// <summary>
    /// Refresh access token
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> RefreshToken([FromBody] string refreshToken)
    {
        try
        {
            var result = await _authenticationService.RefreshTokenAsync(refreshToken, HttpContext.RequestAborted);
            
            var response = new LoginResponse(
                result.UserId,
                result.Username,
                result.FullNameAr,
                result.FullNameEn,
                result.Email,
                result.Role,
                result.RoleId,
                result.DefaultBranchId,
                result.Token,
                result.RefreshToken,
                result.ExpiresInMinutes,
                result.Language);
            
            return Ok(ApiResponse<LoginResponse>.SuccessResponse(response, "Token refreshed successfully"));
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(ApiResponse<LoginResponse>.FailureResponse(ex.Message));
        }
    }
    
    /// <summary>
    /// Change password
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _authenticationService.ChangePasswordAsync(
                userId,
                request.CurrentPassword,
                request.NewPassword,
                HttpContext.RequestAborted);
            
            if (result)
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Password changed successfully"));
            
            return BadRequest(ApiResponse<bool>.FailureResponse("Current password is incorrect"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Change password error");
            return StatusCode(500, ApiResponse<bool>.FailureResponse("An error occurred"));
        }
    }
    
    /// <summary>
    /// Logout
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<bool>>> Logout()
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _authenticationService.LogoutAsync(userId, HttpContext.RequestAborted);
            
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Logout successful"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Logout error");
            return StatusCode(500, ApiResponse<bool>.FailureResponse("An error occurred"));
        }
    }
    
    /// <summary>
    /// Get current user profile
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserDetailDto>>> GetCurrentUser()
    {
        try
        {
            var userId = GetCurrentUserId();
            var user = await _context.Users
                .Include(u => u.Role)
                .Include(u => u.DefaultBranch)
                .FirstOrDefaultAsync(u => u.Id == userId);
            
            if (user == null || user.IsDeleted)
                return NotFound(ApiResponse<UserDetailDto>.FailureResponse("User not found"));
            
            var response = new UserDetailDto(
                user.Id,
                user.Username,
                user.Email,
                user.FullNameAr,
                user.FullNameEn,
                user.Phone,
                user.Mobile,
                user.RoleId,
                user.Role.NameEn,
                user.Role.NameAr,
                user.DefaultBranchId,
                user.DefaultBranch?.NameAr,
                user.IsActive,
                user.MustChangePassword,
                user.Language,
                user.LastLoginDate,
                user.LastPasswordChangeDate,
                new List<Guid>(),
                user.CreatedAt,
                user.UpdatedAt);
            
            return Ok(ApiResponse<UserDetailDto>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Get current user error");
            return StatusCode(500, ApiResponse<UserDetailDto>.FailureResponse("An error occurred"));
        }
    }
    
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : throw new UnauthorizedAccessException();
    }
}
