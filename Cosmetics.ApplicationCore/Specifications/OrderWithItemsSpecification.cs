using Cosmetics.ApplicationCore.Entities.OrderAggregate;

namespace Cosmetics.ApplicationCore.Specifications;

/// <summary>
/// Specification lấy đơn hàng kèm chi tiết OrderItems.
/// </summary>
public class OrderWithItemsSpecification : BaseSpecification<Order>
{
    /// <summary>
    /// Lấy tất cả đơn hàng kèm chi tiết OrderItems, sắp xếp theo thời gian mới nhất.
    /// </summary>
    public OrderWithItemsSpecification()
        : base(_ => true)
    {
        AddInclude(o => o.OrderItems);
        ApplyOrderByDescending(o => o.OrderDate);
    }

    public OrderWithItemsSpecification(string buyerId)
        : base(o => o.BuyerId == buyerId)
    {
        AddInclude(o => o.OrderItems);
        ApplyOrderByDescending(o => o.OrderDate);
    }

    public OrderWithItemsSpecification(int orderId)
        : base(o => o.Id == orderId)
    {
        AddInclude(o => o.OrderItems);
    }
}
