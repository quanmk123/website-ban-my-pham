namespace Cosmetics.Web.ViewModels;

/// <summary>
/// ViewModel trang chủ & danh sách sản phẩm.
/// </summary>
public class CatalogIndexViewModel
{
    public IEnumerable<CatalogItemViewModel> Items { get; set; } = new List<CatalogItemViewModel>();
    public IEnumerable<SelectListItem> Brands { get; set; } = new List<SelectListItem>();
    public IEnumerable<SelectListItem> Types { get; set; } = new List<SelectListItem>();
    public IEnumerable<string> SkinTypes { get; set; } = new List<string>();
    public int? BrandFilterApplied { get; set; }
    public int? TypeFilterApplied { get; set; }
    public string? SkinTypeFilterApplied { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? SearchTerm { get; set; }
    public PaginationInfoViewModel PaginationInfo { get; set; } = new();
}

public class SelectListItem
{
    public string Value { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool Selected { get; set; }
}

public class CatalogItemViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal OriginalPrice { get; set; }
    public string PictureUri { get; set; } = string.Empty;
    public string Capacity { get; set; } = string.Empty;
    public string SkinType { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string TypeName { get; set; } = string.Empty;
    public int StockQuantity { get; set; }

    public bool IsOnSale => OriginalPrice > Price;
    public int DiscountPercent => OriginalPrice > 0 ? (int)((OriginalPrice - Price) / OriginalPrice * 100) : 0;
    public string FormattedPrice => Price.ToString("N0") + " ₫";
    public string FormattedOriginalPrice => OriginalPrice.ToString("N0") + " ₫";
}

public class PaginationInfoViewModel
{
    public int TotalItems { get; set; }
    public int ItemsPerPage { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}
