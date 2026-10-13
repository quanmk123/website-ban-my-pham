namespace Cosmetics.ApplicationCore.Entities;

/// <summary>
/// Danh mục sản phẩm mỹ phẩm (Kem chống nắng, Serum dưỡng da, Son môi...).
/// </summary>
public class CatalogType : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
