namespace Cosmetics.ApplicationCore.Entities;

/// <summary>
/// Mục hàng trong giỏ hàng.
/// </summary>
public class BasketItem : BaseEntity
{
    public int CatalogItemId { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public int BasketId { get; set; }

    public BasketItem() { }

    public BasketItem(int catalogItemId, decimal unitPrice, int quantity)
    {
        CatalogItemId = catalogItemId;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }

    public void AddQuantity(int quantity)
    {
        Quantity += quantity;
    }

    public void SetQuantity(int quantity)
    {
        Quantity = quantity;
    }
}
