using Cosmetics.ApplicationCore.Entities;
using Cosmetics.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Cosmetics.Web.ViewComponents;

/// <summary>
/// ViewComponent để load dữ liệu navigation menu (Brands, Types, SkinTypes) từ DB.
/// Được gọi trong _Layout.cshtml để render thanh điều hướng chính.
/// </summary>
public class NavigationMenuViewComponent : ViewComponent
{
    private readonly IRepository<CatalogBrand> _brandRepository;
    private readonly IRepository<CatalogType> _typeRepository;

    public NavigationMenuViewComponent(
        IRepository<CatalogBrand> brandRepository,
        IRepository<CatalogType> typeRepository)
    {
        _brandRepository = brandRepository;
        _typeRepository = typeRepository;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var brands = await _brandRepository.ListAllAsync();
        var types = await _typeRepository.ListAllAsync();

        var viewModel = new NavigationMenuViewModel
        {
            Brands = brands.Select(b => new NavigationItem { Id = b.Id, Name = b.Name }).ToList(),
            Types = types.Select(t => new NavigationItem { Id = t.Id, Name = t.Name }).ToList(),
            SkinTypes = new List<string> { "Mọi loại da", "Da dầu mụn", "Da khô", "Da nhạy cảm", "Da thường" }
        };

        return View(viewModel);
    }
}

/// <summary>
/// ViewModel cho NavigationMenu ViewComponent.
/// </summary>
public class NavigationMenuViewModel
{
    public List<NavigationItem> Brands { get; set; } = new();
    public List<NavigationItem> Types { get; set; } = new();
    public List<string> SkinTypes { get; set; } = new();
}

public class NavigationItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
