using Cosmetics.ApplicationCore.Entities;
using Cosmetics.ApplicationCore.Specifications;
using Xunit;

namespace Cosmetics.UnitTests.ApplicationCore.Specifications;

public class SpecificationTests
{
    [Fact]
    public void CatalogItemSpecification_FiltersCorrectlyById()
    {
        // Arrange
        var spec = new CatalogItemSpecification(5);

        // Assert
        Assert.NotNull(spec.Criteria);
        var predicate = spec.Criteria.Compile();

        var itemMatch = new CatalogItem { Id = 5 };
        var itemMismatch = new CatalogItem { Id = 3 };

        Assert.True(predicate(itemMatch));
        Assert.False(predicate(itemMismatch));
        Assert.Equal(2, spec.Includes.Count); // CatalogBrand and CatalogType
    }

    [Fact]
    public void CatalogFilterPaginatedSpecification_AppliesCorrectPagingAndFilters()
    {
        // Arrange
        var spec = new CatalogFilterPaginatedSpecification(
            skip: 10,
            take: 5,
            brandId: 2,
            typeId: 1,
            skinType: "Da dầu",
            minPrice: 100000m,
            maxPrice: 500000m,
            searchTerm: "Anthelios");

        // Assert
        Assert.Equal(10, spec.Skip);
        Assert.Equal(5, spec.Take);
        Assert.True(spec.IsPagingEnabled);
        Assert.NotNull(spec.Criteria);

        var predicate = spec.Criteria.Compile();

        var matchingItem = new CatalogItem
        {
            CatalogBrandId = 2,
            CatalogTypeId = 1,
            SkinType = "Da dầu mụn",
            Price = 300000m,
            Name = "La Roche-Posay Anthelios UV"
        };

        var wrongBrandItem = new CatalogItem
        {
            CatalogBrandId = 3,
            CatalogTypeId = 1,
            SkinType = "Da dầu mụn",
            Price = 300000m,
            Name = "La Roche-Posay Anthelios UV"
        };

        Assert.True(predicate(matchingItem));
        Assert.False(predicate(wrongBrandItem));
    }
}
