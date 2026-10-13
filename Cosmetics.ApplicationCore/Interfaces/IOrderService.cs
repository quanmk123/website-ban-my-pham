using Cosmetics.ApplicationCore.Entities.OrderAggregate;

namespace Cosmetics.ApplicationCore.Interfaces;

/// <summary>
/// Service xử lý đơn hàng.
/// </summary>
public interface IOrderService
{
    Task<Order> CreateOrderAsync(string buyerId, Address shippingAddress, PaymentMethod paymentMethod, string? note = null);
    Task<Order?> GetOrderByIdAsync(int orderId);
    Task<IReadOnlyList<Order>> GetOrdersByBuyerIdAsync(string buyerId);
    Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus);
}
