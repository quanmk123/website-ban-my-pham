using System.ComponentModel.DataAnnotations;

namespace Cosmetics.Web.ViewModels;

/// <summary>
/// ViewModels cho giỏ hàng.
/// </summary>
public class BasketViewModel
{
    public int Id { get; set; }
    public List<BasketItemViewModel> Items { get; set; } = new();
    public string FormattedTotal => Items.Sum(i => i.UnitPrice * i.Quantity).ToString("N0") + " ₫";
    public decimal Total => Items.Sum(i => i.UnitPrice * i.Quantity);
}

public class BasketItemViewModel
{
    public int Id { get; set; }
    public int CatalogItemId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string PictureUri { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public string FormattedPrice => UnitPrice.ToString("N0") + " ₫";
    public string FormattedSubtotal => (UnitPrice * Quantity).ToString("N0") + " ₫";
}

/// <summary>
/// ViewModel thanh toán.
/// </summary>
public class CheckoutViewModel
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên người nhận.")]
    [Display(Name = "Họ tên người nhận")]
    public string ReceiverName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ.")]
    [Display(Name = "Số điện thoại")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
    [Display(Name = "Số nhà, Tên đường")]
    public string Street { get; set; } = string.Empty;

    [Display(Name = "Phường/Xã")]
    public string Ward { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập Quận/Huyện.")]
    [Display(Name = "Quận/Huyện")]
    public string District { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập Tỉnh/Thành phố.")]
    [Display(Name = "Tỉnh/Thành phố")]
    public string City { get; set; } = string.Empty;

    [Display(Name = "Phương thức thanh toán")]
    public int PaymentMethod { get; set; }

    [Display(Name = "Ghi chú")]
    public string? Note { get; set; }

    public BasketViewModel? Basket { get; set; }
}
