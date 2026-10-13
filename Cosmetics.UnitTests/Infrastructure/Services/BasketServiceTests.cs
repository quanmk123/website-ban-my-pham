using Cosmetics.ApplicationCore.Entities;
using Cosmetics.ApplicationCore.Interfaces;
using Cosmetics.Infrastructure.Services;
using Moq;
using Xunit;

namespace Cosmetics.UnitTests.Infrastructure.Services;

public class BasketServiceTests
{
    private readonly Mock<IRepository<Basket>> _basketRepoMock = new();
    private readonly BasketService _basketService;

    public BasketServiceTests()
    {
        _basketService = new BasketService(_basketRepoMock.Object);
    }

    [Fact]
    public async Task GetOrCreateBasketAsync_WhenBasketDoesNotExist_CreatesNewBasket()
    {
        // Arrange
        _basketRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Basket>>()))
            .ReturnsAsync((Basket?)null);

        _basketRepoMock
            .Setup(r => r.AddAsync(It.IsAny<Basket>()))
            .ReturnsAsync((Basket b) => b);

        // Act
        var result = await _basketService.GetOrCreateBasketAsync("user-1");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("user-1", result.BuyerId);
        _basketRepoMock.Verify(r => r.AddAsync(It.IsAny<Basket>()), Times.Once);
    }

    [Fact]
    public async Task AddItemToBasketAsync_AddsItemAndUpdatesRepository()
    {
        // Arrange
        var basket = new Basket { BuyerId = "user-1" };
        _basketRepoMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Basket>>()))
            .ReturnsAsync(basket);

        // Act
        await _basketService.AddItemToBasketAsync("user-1", 10, 500000m, 2);

        // Assert
        Assert.Single(basket.Items);
        Assert.Equal(2, basket.Items.First().Quantity);
        _basketRepoMock.Verify(r => r.UpdateAsync(basket), Times.Once);
    }

    [Fact]
    public void TransferBasket_MergesAnonymousItemsIntoUserBasket()
    {
        // Arrange
        var userBasket = new Basket { BuyerId = "user@email.com" };
        userBasket.AddItem(1, 100000m, 1); // đã có 1 sản phẩm 1

        // Giả lập transfer: gộp các mặt hàng từ giỏ hàng ẩn danh
        userBasket.AddItem(1, 100000m, 2);
        userBasket.AddItem(2, 200000m, 1);

        // Assert
        Assert.Equal(3, userBasket.Items.First(i => i.CatalogItemId == 1).Quantity);
        Assert.Equal(1, userBasket.Items.First(i => i.CatalogItemId == 2).Quantity);
        Assert.Equal(500000m, userBasket.TotalPrice);
    }
}
