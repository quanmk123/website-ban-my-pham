namespace Cosmetics.Web.ViewModels;

/// <summary>
/// ViewModel chi tiết sản phẩm mỹ phẩm.
/// </summary>
public class ProductDetailViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal OriginalPrice { get; set; }
    public string PictureUri { get; set; } = string.Empty;
    public string Capacity { get; set; } = string.Empty;
    public string SkinType { get; set; } = string.Empty;
    public string Ingredients { get; set; } = string.Empty;
    public string ExpiryNotice { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public string BrandName { get; set; } = string.Empty;
    public string BrandCountry { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;

    public bool IsOnSale => OriginalPrice > Price;
    public int DiscountPercent => OriginalPrice > 0 ? (int)((OriginalPrice - Price) / OriginalPrice * 100) : 0;
    public string FormattedPrice => Price.ToString("N0") + " ₫";
    public string FormattedOriginalPrice => OriginalPrice.ToString("N0") + " ₫";
    public bool InStock => StockQuantity > 0;
}
