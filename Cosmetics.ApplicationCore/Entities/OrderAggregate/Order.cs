namespace Cosmetics.ApplicationCore.Entities.OrderAggregate;

/// <summary>
/// Đơn hàng Aggregate Root.
/// </summary>
public class Order : BaseEntity
{
    public string BuyerId { get; set; } = string.Empty;
    public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
    public Address ShipToAddress { get; set; } = null!;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.COD;
    public string? Note { get; set; }

    public List<OrderItem> OrderItems { get; private set; } = new();

    public decimal TotalPrice => OrderItems.Sum(i => i.UnitPrice * i.Units);

    public Order() { }

    public Order(string buyerId, Address shipToAddress, List<OrderItem> items, PaymentMethod paymentMethod, string? note = null)
    {
        BuyerId = buyerId;
        ShipToAddress = shipToAddress;
        OrderItems = items;
        PaymentMethod = paymentMethod;
        Note = note;
        OrderDate = DateTimeOffset.Now;
        Status = OrderStatus.Pending;
    }
}
