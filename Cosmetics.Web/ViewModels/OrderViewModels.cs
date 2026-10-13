namespace Cosmetics.Web.ViewModels;

/// <summary>
/// ViewModels cho đơn hàng.
/// </summary>
public class OrderViewModel
{
    public int Id { get; set; }
    public DateTimeOffset OrderDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string StatusBadgeClass { get; set; } = string.Empty;
    public string ReceiverName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string FullAddress { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string? Note { get; set; }
    public List<OrderItemViewModel> Items { get; set; } = new();
    public string FormattedTotal => Items.Sum(i => i.UnitPrice * i.Units).ToString("N0") + " ₫";
    public string FormattedDate => OrderDate.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
}

public class OrderItemViewModel
{
    public string ProductName { get; set; } = string.Empty;
    public string PictureUri { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Units { get; set; }
    public string FormattedPrice => UnitPrice.ToString("N0") + " ₫";
    public string FormattedSubtotal => (UnitPrice * Units).ToString("N0") + " ₫";
}
