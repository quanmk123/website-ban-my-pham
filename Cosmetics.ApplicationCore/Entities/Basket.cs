namespace Cosmetics.ApplicationCore.Entities;

/// <summary>
/// Giỏ hàng Aggregate Root.
/// BuyerId: Guid session cho khách vãng lai, User Id cho khách đăng nhập.
/// </summary>
public class Basket : BaseEntity
{
    public string BuyerId { get; set; } = string.Empty;
    public List<BasketItem> Items { get; private set; } = new();

    public void AddItem(int catalogItemId, decimal unitPrice, int quantity = 1)
    {
        var existingItem = Items.FirstOrDefault(i => i.CatalogItemId == catalogItemId);
        if (existingItem != null)
        {
            existingItem.AddQuantity(quantity);
        }
        else
        {
            Items.Add(new BasketItem(catalogItemId, unitPrice, quantity));
        }
    }

    public void RemoveEmptyItems()
    {
        Items.RemoveAll(i => i.Quantity <= 0);
    }

    public void SetNewBuyerId(string buyerId)
    {
        BuyerId = buyerId;
    }

    public decimal TotalPrice => Items.Sum(i => i.UnitPrice * i.Quantity);
}
