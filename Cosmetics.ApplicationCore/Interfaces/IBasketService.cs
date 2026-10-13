using Cosmetics.ApplicationCore.Entities;

namespace Cosmetics.ApplicationCore.Interfaces;

/// <summary>
/// Service quản lý giỏ hàng.
/// </summary>
public interface IBasketService
{
    Task<Basket> GetOrCreateBasketAsync(string buyerId);
    Task<Basket> AddItemToBasketAsync(string buyerId, int catalogItemId, decimal price, int quantity = 1);
    Task SetQuantitiesAsync(int basketId, Dictionary<int, int> quantities);
    Task DeleteBasketAsync(int basketId);
    Task TransferBasketAsync(string anonymousId, string userId);
}
