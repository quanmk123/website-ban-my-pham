using Cosmetics.ApplicationCore.Entities.OrderAggregate;

namespace Cosmetics.ApplicationCore.Specifications;

/// <summary>
/// Specification lấy đơn hàng theo BuyerId kèm chi tiết OrderItems.
/// </summary>
public class OrderWithItemsSpecification : BaseSpecification<Order>
{
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
