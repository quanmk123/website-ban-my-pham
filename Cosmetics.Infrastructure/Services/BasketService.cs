using Cosmetics.ApplicationCore.Entities;
using Cosmetics.ApplicationCore.Interfaces;
using Cosmetics.ApplicationCore.Specifications;

namespace Cosmetics.Infrastructure.Services;

/// <summary>
/// Triển khai Basket Service - Quản lý giỏ hàng.
/// </summary>
public class BasketService : IBasketService
{
    private readonly IRepository<Basket> _basketRepository;

    public BasketService(IRepository<Basket> basketRepository)
    {
        _basketRepository = basketRepository;
    }

    public async Task<Basket> GetOrCreateBasketAsync(string buyerId)
    {
        var spec = new BasketWithItemsSpecification(buyerId);
        var basket = await _basketRepository.FirstOrDefaultAsync(spec);

        if (basket == null)
        {
            basket = new Basket { BuyerId = buyerId };
            basket = await _basketRepository.AddAsync(basket);
        }

        return basket;
    }

    public async Task<Basket> AddItemToBasketAsync(string buyerId, int catalogItemId, decimal price, int quantity = 1)
    {
        var basket = await GetOrCreateBasketAsync(buyerId);
        basket.AddItem(catalogItemId, price, quantity);
        await _basketRepository.UpdateAsync(basket);
        return basket;
    }

    public async Task SetQuantitiesAsync(int basketId, Dictionary<int, int> quantities)
    {
        var spec = new BasketWithItemsSpecification(basketId);
        var basket = await _basketRepository.FirstOrDefaultAsync(spec);

        if (basket == null) return;

        foreach (var item in basket.Items)
        {
            if (quantities.TryGetValue(item.Id, out var quantity))
            {
                item.SetQuantity(quantity);
            }
        }

        basket.RemoveEmptyItems();
        await _basketRepository.UpdateAsync(basket);
    }

    public async Task DeleteBasketAsync(int basketId)
    {
        var basket = await _basketRepository.GetByIdAsync(basketId);
        if (basket != null)
        {
            await _basketRepository.DeleteAsync(basket);
        }
    }

    public async Task TransferBasketAsync(string anonymousId, string userId)
    {
        var anonymousSpec = new BasketWithItemsSpecification(anonymousId);
        var anonymousBasket = await _basketRepository.FirstOrDefaultAsync(anonymousSpec);

        if (anonymousBasket == null) return;

        var userSpec = new BasketWithItemsSpecification(userId);
        var userBasket = await _basketRepository.FirstOrDefaultAsync(userSpec);

        if (userBasket != null)
        {
            // Gộp giỏ hàng ẩn danh vào giỏ hàng user
            foreach (var item in anonymousBasket.Items)
            {
                userBasket.AddItem(item.CatalogItemId, item.UnitPrice, item.Quantity);
            }
            await _basketRepository.UpdateAsync(userBasket);
            await _basketRepository.DeleteAsync(anonymousBasket);
        }
        else
        {
            // Chuyển giỏ hàng ẩn danh thành giỏ hàng user
            anonymousBasket.SetNewBuyerId(userId);
            await _basketRepository.UpdateAsync(anonymousBasket);
        }
    }
}
