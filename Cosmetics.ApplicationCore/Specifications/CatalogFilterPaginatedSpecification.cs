using Cosmetics.ApplicationCore.Entities;

namespace Cosmetics.ApplicationCore.Specifications;

/// <summary>
/// Specification lọc và phân trang sản phẩm mỹ phẩm.
/// Hỗ trợ lọc theo Thương hiệu, Danh mục, Khoảng giá, Loại da, và Tìm kiếm theo tên.
/// </summary>
public class CatalogFilterPaginatedSpecification : BaseSpecification<CatalogItem>
{
    public CatalogFilterPaginatedSpecification(
        int skip, int take,
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
        AddInclude(i => i.CatalogBrand);
        AddInclude(i => i.CatalogType);
        ApplyPaging(skip, take);
        ApplyOrderByDescending(i => i.Id);
    }
}
