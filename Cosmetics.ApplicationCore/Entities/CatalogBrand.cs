namespace Cosmetics.ApplicationCore.Entities;

/// <summary>
/// Thương hiệu mỹ phẩm (La Roche-Posay, Innisfree, Cerave, MAC, Maybelline, Anessa...).
/// </summary>
public class CatalogBrand : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string LogoUrl { get; set; } = string.Empty;
    public string CountryOfOrigin { get; set; } = string.Empty;
}
