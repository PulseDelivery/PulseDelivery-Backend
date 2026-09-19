using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Identity.API.DTOs.Responses;
using Identity.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PulseDelivery.Shared.Authorization;
using PulseDelivery.Shared.Configurations;

namespace Identity.API.Services;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly UserManager<AppUser> _userManager;

    public TokenService(IOptions<JwtSettings> jwtOptions, UserManager<AppUser> userManager)
    {
        _jwtSettings = jwtOptions.Value;
        _userManager = userManager;
    }

    public async Task<LoginResponseDto> CreateTokenAsync(AppUser user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        
        if (user.RestaurantIds != null && user.RestaurantIds.Any())
        {
            foreach (var restaurantId in user.RestaurantIds)
            {
                claims.Add(new Claim("RestaurantId", restaurantId));
            }
        }

        var userRoles = await _userManager.GetRolesAsync(user);

        foreach (var role in userRoles)
        {
            if (!RolePermissions.PermissionsByRole.TryGetValue(role, out var permissions))
            {
                continue;
            }

            foreach (var permission in permissions)
            {
                claims.Add(new Claim("Permission", permission));
            }
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecurityKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationInMinutes);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expiration,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new LoginResponseDto
        {
            AccessToken = tokenHandler.WriteToken(token),
            RefreshToken = Guid.NewGuid().ToString(),
            ExpiresIn = _jwtSettings.AccessTokenExpirationInMinutes * 60
        };
    }
}