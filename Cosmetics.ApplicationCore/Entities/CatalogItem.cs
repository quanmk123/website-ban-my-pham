namespace Cosmetics.ApplicationCore.Entities;

/// <summary>
/// Sản phẩm mỹ phẩm - Entity chính kế thừa từ eShopOnWeb CatalogItem.
/// Bổ sung các thuộc tính đặc thù cho ngành mỹ phẩm.
/// </summary>
public class CatalogItem : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    /// <summary>Giá bán hiện tại (VNĐ).</summary>
    public decimal Price { get; set; }

    /// <summary>Giá gốc trước khuyến mãi (VNĐ).</summary>
    public decimal OriginalPrice { get; set; }

    /// <summary>Đường dẫn ảnh sản phẩm.</summary>
    public string PictureUri { get; set; } = string.Empty;

    // === THUỘC TÍNH ĐẶC THÙ MỸ PHẨM ===

    /// <summary>Dung tích / Khối lượng (VD: "30ml", "50ml", "3.5g").</summary>
    public string Capacity { get; set; } = string.Empty;

    /// <summary>Loại da phù hợp (VD: "Da dầu mụn", "Da khô", "Da nhạy cảm", "Mọi loại da").</summary>
    public string SkinType { get; set; } = string.Empty;

    /// <summary>Thành phần nổi bật (AHA, BHA, Niacinamide, Retinol...).</summary>
    public string Ingredients { get; set; } = string.Empty;

    /// <summary>Hạn sử dụng (VD: "3 năm kể từ NSX, 12 tháng sau mở nắp").</summary>
    public string ExpiryNotice { get; set; } = string.Empty;

    /// <summary>Số lượng tồn kho.</summary>
    public int StockQuantity { get; set; }

    // === KHÓA NGOẠI ===
    public int CatalogBrandId { get; set; }
    public CatalogBrand CatalogBrand { get; set; } = null!;

    public int CatalogTypeId { get; set; }
    public CatalogType CatalogType { get; set; } = null!;
}
