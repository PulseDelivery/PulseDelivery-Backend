using Identity.API.DTOs.Responses;
using Identity.API.Models;

namespace Identity.API.Services;

public interface ITokenService
{
    LoginResponseDto CreateToken(AppUser user);
}