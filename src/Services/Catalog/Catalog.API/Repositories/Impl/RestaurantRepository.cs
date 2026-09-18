using Catalog.API.Entities;
using Catalog.API.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Catalog.API.Repositories;

public class RestaurantRepository : IRestaurantRepository
{
    private readonly IMongoCollection<Restaurant> _restaurants;

    public RestaurantRepository(IOptions<MongoDbSettings> settings)
    {
        
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        
        _restaurants = database.GetCollection<Restaurant>(settings.Value.CollectionName);
    }

    public async Task<IEnumerable<Restaurant>> GetAllRestaurantsAsync()
    {
        return await _restaurants.Find(p => true).ToListAsync();
    }

    public async Task<Restaurant?> GetRestaurantByIdAsync(string id)
    {
        return await _restaurants.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task CreateRestaurantAsync(Restaurant restaurant)
    {
        await _restaurants.InsertOneAsync(restaurant);
    }

    public async Task<bool> UpdateRestaurantAsync(Restaurant restaurant)
    {
        var updateResult = await _restaurants.ReplaceOneAsync(filter: g => g.Id == restaurant.Id, replacement: restaurant);
        return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
    }

    public async Task<bool> DeleteRestaurantAsync(string id)
    {
        var deleteResult = await _restaurants.DeleteOneAsync(p => p.Id == id);
        return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;
    }
}