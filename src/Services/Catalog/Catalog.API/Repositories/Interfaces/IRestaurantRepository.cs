using Catalog.API.Entities;

namespace Catalog.API.Repositories;

public interface IRestaurantRepository
{
    Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync();
    Task<Restaurant?> GetRestaurantByIdAsync(string id);
    Task CreateRestaurantAsync(Restaurant restaurant);
    Task<bool> UpdateRestaurantAsync(Restaurant restaurant);
    Task<bool> DeleteRestaurantAsync(string id);
}