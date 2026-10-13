namespace Cosmetics.ApplicationCore.Entities.OrderAggregate;

/// <summary>
/// Trạng thái đơn hàng.
/// </summary>
public enum OrderStatus
{
    /// <summary>Chờ xác nhận</summary>
    Pending = 0,
    /// <summary>Đã xác nhận</summary>
    Confirmed = 1,
    /// <summary>Đang giao hàng</summary>
    Shipping = 2,
    /// <summary>Hoàn thành</summary>
    Completed = 3,
    /// <summary>Đã hủy</summary>
    Cancelled = 4
}
