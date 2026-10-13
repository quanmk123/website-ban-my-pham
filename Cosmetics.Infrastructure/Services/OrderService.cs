using Cosmetics.ApplicationCore.Entities;
using Cosmetics.ApplicationCore.Entities.OrderAggregate;
using Cosmetics.ApplicationCore.Interfaces;
using Cosmetics.ApplicationCore.Specifications;

namespace Cosmetics.Infrastructure.Services;

/// <summary>
/// Triển khai Order Service - Xử lý đơn hàng.
/// </summary>
public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orderRepository;
    private readonly IRepository<Basket> _basketRepository;
    private readonly IRepository<CatalogItem> _itemRepository;

    public OrderService(
        IRepository<Order> orderRepository,
        IRepository<Basket> basketRepository,
        IRepository<CatalogItem> itemRepository)
    {
        _orderRepository = orderRepository;
        _basketRepository = basketRepository;
        _itemRepository = itemRepository;
    }

    public async Task<Order> CreateOrderAsync(string buyerId, Address shippingAddress, PaymentMethod paymentMethod, string? note = null)
    {
        var basketSpec = new BasketWithItemsSpecification(buyerId);
        var basket = await _basketRepository.FirstOrDefaultAsync(basketSpec);

        if (basket == null || !basket.Items.Any())
            throw new InvalidOperationException("Giỏ hàng trống, không thể tạo đơn hàng.");

        var orderItems = new List<OrderItem>();

        foreach (var basketItem in basket.Items)
        {
            var catalogItem = await _itemRepository.GetByIdAsync(basketItem.CatalogItemId);
            if (catalogItem == null) continue;

            // Kiểm tra tồn kho
            if (catalogItem.StockQuantity < basketItem.Quantity)
                throw new InvalidOperationException(
                    $"Sản phẩm '{catalogItem.Name}' chỉ còn {catalogItem.StockQuantity} trong kho.");

            // Trừ tồn kho
            catalogItem.StockQuantity -= basketItem.Quantity;
            await _itemRepository.UpdateAsync(catalogItem);

            orderItems.Add(new OrderItem(
                catalogItem.Id,
                catalogItem.Name,
                catalogItem.PictureUri,
                catalogItem.Price,
                basketItem.Quantity));
        }

        var order = new Order(buyerId, shippingAddress, orderItems, paymentMethod, note);
        order = await _orderRepository.AddAsync(order);

        // Xóa giỏ hàng sau khi đặt hàng
        await _basketRepository.DeleteAsync(basket);

        return order;
    }

    public async Task<Order?> GetOrderByIdAsync(int orderId)
    {
        var spec = new OrderWithItemsSpecification(orderId);
        return await _orderRepository.FirstOrDefaultAsync(spec);
    }

    public async Task<IReadOnlyList<Order>> GetOrdersByBuyerIdAsync(string buyerId)
    {
        var spec = new OrderWithItemsSpecification(buyerId);
        return await _orderRepository.ListAsync(spec);
    }

    public async Task UpdateOrderStatusAsync(int orderId, OrderStatus newStatus)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
            throw new InvalidOperationException("Không tìm thấy đơn hàng.");

        order.Status = newStatus;
        await _orderRepository.UpdateAsync(order);
    }
}
