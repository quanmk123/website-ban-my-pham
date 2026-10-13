namespace Cosmetics.ApplicationCore.Entities.OrderAggregate;

/// <summary>
/// Mục hàng trong đơn hàng - lưu snapshot thông tin sản phẩm tại thời điểm đặt hàng.
/// </summary>
public class OrderItem : BaseEntity
{
    public int CatalogItemId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string PictureUri { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Units { get; set; }

    public OrderItem() { }

    public OrderItem(int catalogItemId, string productName, string pictureUri, decimal unitPrice, int units)
    {
        CatalogItemId = catalogItemId;
        ProductName = productName;
        PictureUri = pictureUri;
        UnitPrice = unitPrice;
        Units = units;
    }
}
