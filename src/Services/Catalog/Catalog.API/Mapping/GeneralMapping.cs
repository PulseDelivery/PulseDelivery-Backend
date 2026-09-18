using AutoMapper;
using Catalog.API.DTOs.Requests;
using Catalog.API.DTOs.Responses;
using Catalog.API.Entities;

namespace Catalog.API.Mapping;

public class GeneralMapping : Profile
{
    public GeneralMapping()
    {
        // Responses
        CreateMap<Restaurant, RestaurantResponseDto>().ReverseMap();
        CreateMap<MenuCategory, MenuCategoryResponseDto>().ReverseMap();
        CreateMap<Product, ProductResponseDto>().ReverseMap();

        // Requests
        CreateMap<CreateRestaurantRequestDto, Restaurant>();
        CreateMap<UpdateRestaurantRequestDto, Restaurant>();
        CreateMap<MenuCategoryRequestDto, MenuCategory>();
        CreateMap<ProductRequestDto, Product>();
    }
}