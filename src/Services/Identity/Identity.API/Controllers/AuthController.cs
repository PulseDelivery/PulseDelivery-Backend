using Identity.API.DTOs;
using Identity.API.DTOs.Requests;
using Identity.API.DTOs.Responses;
using Identity.API.Models;
using Identity.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using PulseDelivery.Shared.Events;

namespace Identity.API.Controllers;

public class AuthController : CustomBaseController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IPublishEndpoint _publishEndpoint;

    public AuthController(UserManager<AppUser> userManager, ITokenService tokenService, IPublishEndpoint publishEndpoint)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            var responseData = new RegisterResponseDto
            {
                Id = user.Id, 
                Email = user.Email
            };
            var userRegisteredEvent = new UserRegisteredEvent
            {
                UserId = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            await _publishEndpoint.Publish(userRegisteredEvent);

            
            return CreateActionResult(ResponseDto<RegisterResponseDto>.Success(responseData, StatusCodes.Status201Created));
        }

        
        var errors = result.Errors.Select(e => e.Description).ToList();
        return CreateActionResult(ResponseDto<RegisterResponseDto>.Fail(errors, StatusCodes.Status400BadRequest));
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        
        if (user == null)
        {
            
            return CreateActionResult(ResponseDto<LoginResponseDto>.Fail("Invalid email or password.", StatusCodes.Status400BadRequest));
        }

        var checkPassword = await _userManager.CheckPasswordAsync(user, request.Password);
        
        if (!checkPassword)
        {
            return CreateActionResult(ResponseDto<LoginResponseDto>.Fail("Invalid email or password.", StatusCodes.Status400BadRequest));
        }


        var responseData = _tokenService.CreateToken(user);
        user.RefreshToken = responseData.RefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        
        await _userManager.UpdateAsync(user);

        return CreateActionResult(ResponseDto<LoginResponseDto>.Success(responseData, StatusCodes.Status200OK));
    }
    
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        
        var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);

        
        if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return CreateActionResult(ResponseDto<LoginResponseDto>.Fail("Invalid or expired refresh token. Please log in again.", StatusCodes.Status401Unauthorized));
        }

        
        var responseData = _tokenService.CreateToken(user);

        
        user.RefreshToken = responseData.RefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _userManager.UpdateAsync(user);

        return CreateActionResult(ResponseDto<LoginResponseDto>.Success(responseData, StatusCodes.Status200OK));
    }
}