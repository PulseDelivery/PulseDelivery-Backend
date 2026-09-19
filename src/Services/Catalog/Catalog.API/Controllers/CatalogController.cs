using AutoMapper;
using Catalog.API.DTOs.Requests;
using Catalog.API.DTOs.Responses;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using PulseDelivery.Shared.ControllerBases;
using PulseDelivery.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using PulseDelivery.Shared.Authorization; // Added for PBAC constants

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class CatalogController : CustomBaseController
{
    private readonly IRestaurantRepository _repository;
    private readonly IMapper _mapper;

    public CatalogController(IRestaurantRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetRestaurants()
    {
        var restaurants = await _repository.GetAllRestaurantsAsync();
        var responseDto = _mapper.Map<List<RestaurantResponseDto>>(restaurants);
        
        return CreateActionResult(ResponseDto<List<RestaurantResponseDto>>.Success(responseDto, 200));
    }

    [HttpGet("{id:length(24)}")]
    public async Task<IActionResult> GetRestaurantById(string id)
    {
        var restaurant = await _repository.GetRestaurantByIdAsync(id);
        if (restaurant == null)
        {
            return CreateActionResult(ResponseDto<RestaurantResponseDto>.Fail("Restaurant not found.", 404));
        }
        
        var responseDto = _mapper.Map<RestaurantResponseDto>(restaurant);
        return CreateActionResult(ResponseDto<RestaurantResponseDto>.Success(responseDto, 200));
    }

    [HttpPost]
    [Authorize(Policy = Permissions.SystemAdmin)] // Only the platform owner can create a new restaurant
    public async Task<IActionResult> CreateRestaurant([FromBody] CreateRestaurantRequestDto request)
    {
        var restaurant = _mapper.Map<Restaurant>(request);
        await _repository.CreateRestaurantAsync(restaurant);
        
        var responseDto = _mapper.Map<RestaurantResponseDto>(restaurant);
        return CreateActionResult(ResponseDto<RestaurantResponseDto>.Success(responseDto, 201));
    }

    [HttpPut]
    [Authorize(Policy = Permissions.CatalogWrite)] // Gatekeeper: Checks whether the user has catalog write permission
    public async Task<IActionResult> UpdateRestaurant([FromBody] UpdateRestaurantRequestDto request)
    {
        // 1. Business rule: Who is the user and which restaurants do they have access to?
        var isSystemAdmin = User.HasClaim("Permission", Permissions.SystemAdmin);
        var userRestaurantIds = User.FindAll("RestaurantId").Select(c => c.Value).ToList();

        // 2. Business rule: If not a system admin and the restaurant is not in their list, deny access
        if (!isSystemAdmin && !userRestaurantIds.Contains(request.Id))
        {
            return CreateActionResult(ResponseDto<NoContent>.Fail(
                "You can only update restaurants or branches you are authorized to manage.", 403));
        }

        var restaurant = _mapper.Map<Restaurant>(request);
        var result = await _repository.UpdateRestaurantAsync(restaurant);
        
        if (!result)
            return CreateActionResult(ResponseDto<NoContent>.Fail(
                "The record was not found or could not be updated.", 404));

        return CreateActionResult(ResponseDto<NoContent>.Success(204));
    }

    [HttpDelete("{id:length(24)}")]
    [Authorize(Policy = Permissions.SystemAdmin)] 
    public async Task<IActionResult> DeleteRestaurant(string id)
    {
        var result = await _repository.DeleteRestaurantAsync(id);
        
        if (!result)
            return CreateActionResult(ResponseDto<NoContent>.Fail(
                "The record was not found.", 404));

        return CreateActionResult(ResponseDto<NoContent>.Success(204));
    }
}