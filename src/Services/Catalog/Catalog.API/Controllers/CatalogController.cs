using AutoMapper;
using Catalog.API.DTOs.Requests;
using Catalog.API.DTOs.Responses;
using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using PulseDelivery.Shared.ControllerBases;
using PulseDelivery.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;

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
            return CreateActionResult(ResponseDto<RestaurantResponseDto>.Fail("Restoran bulunamadı.", 404));
        }
        
        var responseDto = _mapper.Map<RestaurantResponseDto>(restaurant);
        return CreateActionResult(ResponseDto<RestaurantResponseDto>.Success(responseDto, 200));
    }

    [HttpPost]
    [Authorize(Policy = "AdminAccess")]
    public async Task<IActionResult> CreateRestaurant([FromBody] CreateRestaurantRequestDto request)
    {
        var restaurant = _mapper.Map<Restaurant>(request);
        await _repository.CreateRestaurantAsync(restaurant);
        
        var responseDto = _mapper.Map<RestaurantResponseDto>(restaurant);
        return CreateActionResult(ResponseDto<RestaurantResponseDto>.Success(responseDto, 201));
    }

    [HttpPut]
    [Authorize(Policy = "AdminAccess")]
    public async Task<IActionResult> UpdateRestaurant([FromBody] UpdateRestaurantRequestDto request)
    {
        var restaurant = _mapper.Map<Restaurant>(request);
        var result = await _repository.UpdateRestaurantAsync(restaurant);
        
        if(!result)
            return CreateActionResult(ResponseDto<NoContent>.Fail("Kayıt bulunamadı veya güncellenemedi.", 404));

        return CreateActionResult(ResponseDto<NoContent>.Success(204));
    }

    [HttpDelete("{id:length(24)}")]
    [Authorize(Policy = "AdminAccess")]
    public async Task<IActionResult> DeleteRestaurant(string id)
    {
        var result = await _repository.DeleteRestaurantAsync(id);
        
        if(!result)
            return CreateActionResult(ResponseDto<NoContent>.Fail("Kayıt bulunamadı.", 404));

        return CreateActionResult(ResponseDto<NoContent>.Success(204));
    }
}