using Cosmetics.ApplicationCore.Entities;

namespace Cosmetics.ApplicationCore.Specifications;

/// <summary>
/// Specification lấy giỏ hàng theo BuyerId kèm danh sách items.
/// </summary>
public class BasketWithItemsSpecification : BaseSpecification<Basket>
{
    public BasketWithItemsSpecification(string buyerId)
        : base(b => b.BuyerId == buyerId)
    {
        AddInclude(b => b.Items);
    }

    public BasketWithItemsSpecification(int basketId)
        : base(b => b.Id == basketId)
    {
        AddInclude(b => b.Items);
    }
}
