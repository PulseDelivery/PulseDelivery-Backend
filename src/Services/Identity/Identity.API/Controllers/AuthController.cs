using Identity.API.DTOs;
using Identity.API.DTOs.Requests;
using Identity.API.DTOs.Responses;
using Identity.API.Models;
using Identity.API.Services;

using MassTransit;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using PulseDelivery.Shared.ControllerBases;
using PulseDelivery.Shared.DTOs;
using PulseDelivery.Shared.Events;
using PulseDelivery.Shared.Authorization;

namespace Identity.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : CustomBaseController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager = null!;
    private readonly ITokenService _tokenService;
    private readonly IPublishEndpoint _publishEndpoint;

    public AuthController(
        UserManager<AppUser> userManager,
        ITokenService tokenService,
        IPublishEndpoint publishEndpoint,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _publishEndpoint = publishEndpoint;
        _roleManager = roleManager;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequestDto request)
    {
        var user = new AppUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(
            user,
            request.Password);

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
            if (!await _roleManager.RoleExistsAsync("Customer"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Customer"));
            }

            await _userManager.AddToRoleAsync(user, "Customer");await _userManager.AddToRoleAsync(user, "Customer");

            return CreateActionResult(
                ResponseDto<RegisterResponseDto>.Success(
                    responseData,
                    StatusCodes.Status201Created));
        }

        var errors = result.Errors
            .Select(e => e.Description)
            .ToList();

        return CreateActionResult(
            ResponseDto<RegisterResponseDto>.Fail(
                errors,
                StatusCodes.Status400BadRequest));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return CreateActionResult(
                ResponseDto<LoginResponseDto>.Fail(
                    "Invalid email or password.",
                    StatusCodes.Status400BadRequest));
        }

        var checkPassword = await _userManager.CheckPasswordAsync(
            user,
            request.Password);

        if (!checkPassword)
        {
            return CreateActionResult(
                ResponseDto<LoginResponseDto>.Fail(
                    "Invalid email or password.",
                    StatusCodes.Status400BadRequest));
        }

        var responseData = await _tokenService.CreateTokenAsync(user);

        user.RefreshToken = responseData.RefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _userManager.UpdateAsync(user);

        return CreateActionResult(
            ResponseDto<LoginResponseDto>.Success(
                responseData,
                StatusCodes.Status200OK));
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken(
        [FromBody] RefreshTokenRequestDto request)
    {
        var user = await _userManager.Users
            .SingleOrDefaultAsync(
                u => u.RefreshToken == request.RefreshToken);

        if (user == null ||
            user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            return CreateActionResult(
                ResponseDto<LoginResponseDto>.Fail(
                    "Invalid or expired refresh token. Please log in again.",
                    StatusCodes.Status401Unauthorized));
        }

        var responseData = await _tokenService.CreateTokenAsync(user);

        user.RefreshToken = responseData.RefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

        await _userManager.UpdateAsync(user);

        return CreateActionResult(
            ResponseDto<LoginResponseDto>.Success(
                responseData,
                StatusCodes.Status200OK));
    }
    
    [HttpPost("assign-restaurant-owner")]
    [Authorize(Policy = Permissions.SystemAdmin)] 
    public async Task<IActionResult> AssignRestaurantOwner([FromBody] AssignRestaurantOwnerRequestDto request)
    {
        // 1. Find the user
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
        {
            return CreateActionResult(ResponseDto<NoContent>.Fail("User not found.", 404));
        }

        // 2. Create the role if it does not exist and assign it to the user
        if (!await _roleManager.RoleExistsAsync("RestaurantOwner"))
        {
            await _roleManager.CreateAsync(new IdentityRole("RestaurantOwner"));
        }
        await _userManager.AddToRoleAsync(user, "RestaurantOwner");

        // 3. Add the restaurant ID to the user's list
        if (!user.RestaurantIds.Contains(request.RestaurantId))
        {
            user.RestaurantIds.Add(request.RestaurantId);
            await _userManager.UpdateAsync(user);
        }
        return CreateActionResult(ResponseDto<NoContent>.Success(204));
    }
}
