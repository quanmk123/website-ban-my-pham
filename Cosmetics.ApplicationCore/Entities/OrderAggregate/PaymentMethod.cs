namespace Cosmetics.ApplicationCore.Entities.OrderAggregate;

/// <summary>
/// Phương thức thanh toán.
/// </summary>
public enum PaymentMethod
{
    /// <summary>Thanh toán khi nhận hàng (COD)</summary>
    COD = 0,
    /// <summary>Chuyển khoản ngân hàng</summary>
    BankTransfer = 1,
    /// <summary>VNPay</summary>
    VNPay = 2
}
