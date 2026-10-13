using Cosmetics.ApplicationCore.Entities.OrderAggregate;
using Xunit;

namespace Cosmetics.UnitTests.ApplicationCore.Entities;

public class OrderTests
{
    [Fact]
    public void Order_CalculatesTotalPriceFromItems()
    {
        // Arrange
        var address = new Address("Nguyen Van A", "0901234567", "123 Le Loi", "Phuong Ben Nghe", "Quan 1", "TP. Ho Chi Minh");
        var items = new List<OrderItem>
        {
            new(1, "Kem chong nang La Roche-Posay", "/images/p1.jpg", 420000m, 2),
            new(2, "Serum Klairs Vitamin C", "/images/p2.jpg", 380000m, 1)
        };

        // Act
        var order = new Order("buyer@test.com", address, items, PaymentMethod.COD, "Giao gio hanh chinh");

        // Assert
        Assert.Equal(1220000m, order.TotalPrice);
        Assert.Equal(OrderStatus.Pending, order.Status);
        Assert.Equal("buyer@test.com", order.BuyerId);
        Assert.Contains("123 Le Loi", order.ShipToAddress.FullAddress);
    }
}
