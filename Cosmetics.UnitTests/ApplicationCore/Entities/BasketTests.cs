using Cosmetics.ApplicationCore.Entities;
using Xunit;

namespace Cosmetics.UnitTests.ApplicationCore.Entities;

public class BasketTests
{
    [Fact]
    public void AddItem_WhenItemDoesNotExist_AddsNewItem()
    {
        // Arrange
        var basket = new Basket { BuyerId = "user-1" };

        // Act
        basket.AddItem(1, 350000m, 2);

        // Assert
        Assert.Single(basket.Items);
        var item = basket.Items.First();
        Assert.Equal(1, item.CatalogItemId);
        Assert.Equal(350000m, item.UnitPrice);
        Assert.Equal(2, item.Quantity);
    }

    [Fact]
    public void AddItem_WhenItemAlreadyExists_IncrementsQuantity()
    {
        // Arrange
        var basket = new Basket { BuyerId = "user-1" };
        basket.AddItem(1, 350000m, 2);

        // Act
        basket.AddItem(1, 350000m, 3);

        // Assert
        Assert.Single(basket.Items);
        var item = basket.Items.First();
        Assert.Equal(5, item.Quantity);
    }

    [Fact]
    public void TotalPrice_CalculatesSumOfItemsCorrectly()
    {
        // Arrange
        var basket = new Basket { BuyerId = "user-1" };
        basket.AddItem(1, 100000m, 2); // 200,000
        basket.AddItem(2, 250000m, 1); // 250,000

        // Assert
        Assert.Equal(450000m, basket.TotalPrice);
    }

    [Fact]
    public void RemoveEmptyItems_RemovesItemsWithZeroQuantity()
    {
        // Arrange
        var basket = new Basket { BuyerId = "user-1" };
        basket.AddItem(1, 100000m, 2);
        basket.AddItem(2, 200000m, 1);

        // Act
        basket.Items.First(i => i.CatalogItemId == 1).SetQuantity(0);
        basket.RemoveEmptyItems();

        // Assert
        Assert.Single(basket.Items);
        Assert.Equal(2, basket.Items.First().CatalogItemId);
    }
}
