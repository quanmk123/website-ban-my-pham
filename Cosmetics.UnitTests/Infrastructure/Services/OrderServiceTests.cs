using Cosmetics.ApplicationCore.Entities;
using Cosmetics.ApplicationCore.Entities.OrderAggregate;
using Cosmetics.ApplicationCore.Interfaces;
using Cosmetics.Infrastructure.Services;
using Moq;
using Xunit;

namespace Cosmetics.UnitTests.Infrastructure.Services;

public class OrderServiceTests
{
    private readonly Mock<IRepository<Order>> _orderRepoMock = new();
    private readonly Mock<IRepository<Basket>> _basketRepoMock = new();
    private readonly Mock<IRepository<CatalogItem>> _itemRepoMock = new();
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _orderService = new OrderService(
            _orderRepoMock.Object,
            _basketRepoMock.Object,
            _itemRepoMock.Object);
    }

    [Fact]
    public async Task CreateOrder_ThrowsException_WhenBasketIsEmpty()
    {
        // Arrange
        _basketRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Basket>>()))
            .ReturnsAsync((Basket?)null);

        var address = new Address("Test", "0123456789", "Street", "Ward", "District", "City");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _orderService.CreateOrderAsync("buyer@test.com", address, PaymentMethod.COD));
    }

    [Fact]
    public async Task CreateOrder_ThrowsException_WhenStockIsInsufficient()
    {
        // Arrange
        var basket = new Basket { BuyerId = "buyer@test.com" };
        basket.AddItem(1, 100000m, 10);

        _basketRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Basket>>()))
            .ReturnsAsync(basket);

        var catalogItem = new CatalogItem
        {
            Id = 1,
            Name = "Kem dưỡng",
            Price = 100000m,
            StockQuantity = 2 // Chỉ còn 2 mà mua 10
        };

        _itemRepoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(catalogItem);

        var address = new Address("Test", "0123456789", "Street", "Ward", "District", "City");

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _orderService.CreateOrderAsync("buyer@test.com", address, PaymentMethod.COD));

        Assert.Contains("trong kho", ex.Message);
    }

    [Fact]
    public async Task CreateOrder_DeductsStockAndSavesOrder_WhenStockIsSufficient()
    {
        // Arrange
        var basket = new Basket { BuyerId = "buyer@test.com" };
        basket.AddItem(1, 200000m, 3);

        _basketRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Basket>>()))
            .ReturnsAsync(basket);

        var catalogItem = new CatalogItem
        {
            Id = 1,
            Name = "Son MAC Ruby Woo",
            Price = 200000m,
            StockQuantity = 10
        };

        _itemRepoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(catalogItem);

        _orderRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => o);

        var address = new Address("Nguyen Van A", "0901234567", "Street", "Ward", "District", "City");

        // Act
        var order = await _orderService.CreateOrderAsync("buyer@test.com", address, PaymentMethod.COD);

        // Assert
        Assert.NotNull(order);
        Assert.Equal(7, catalogItem.StockQuantity); // 10 - 3 = 7
        _itemRepoMock.Verify(r => r.UpdateAsync(catalogItem), Times.Once);
        _basketRepoMock.Verify(r => r.DeleteAsync(basket), Times.Once);
        _orderRepoMock.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
    }
}
