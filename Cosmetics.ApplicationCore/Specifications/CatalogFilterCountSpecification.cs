using Cosmetics.ApplicationCore.Entities;

namespace Cosmetics.ApplicationCore.Specifications;

/// <summary>
/// Specification đếm tổng số sản phẩm theo bộ lọc (dùng cho phân trang).
/// </summary>
public class CatalogFilterCountSpecification : BaseSpecification<CatalogItem>
{
    public CatalogFilterCountSpecification(
        int? brandId = null,
        int? typeId = null,
        string? skinType = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        string? searchTerm = null)
        : base(i =>
            (!brandId.HasValue || i.CatalogBrandId == brandId.Value) &&
            (!typeId.HasValue || i.CatalogTypeId == typeId.Value) &&
            (string.IsNullOrEmpty(skinType) || i.SkinType.Contains(skinType)) &&
            (!minPrice.HasValue || i.Price >= minPrice.Value) &&
            (!maxPrice.HasValue || i.Price <= maxPrice.Value) &&
            (string.IsNullOrEmpty(searchTerm) || i.Name.Contains(searchTerm)))
    {
    }
}
