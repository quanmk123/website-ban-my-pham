using Cosmetics.ApplicationCore.Entities;

namespace Cosmetics.ApplicationCore.Specifications;

/// <summary>
/// Specification lấy chi tiết sản phẩm kèm thông tin thương hiệu và danh mục.
/// </summary>
public class CatalogItemSpecification : BaseSpecification<CatalogItem>
{
    public CatalogItemSpecification(int id)
        : base(i => i.Id == id)
    {
        AddInclude(i => i.CatalogBrand);
        AddInclude(i => i.CatalogType);
    }
}
