using Cosmetics.ApplicationCore.Entities;
using Cosmetics.ApplicationCore.Interfaces;
using Cosmetics.ApplicationCore.Specifications;
using Cosmetics.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Cosmetics.Web.Controllers;

/// <summary>
/// Controller Catalog - Trang chủ và danh sách sản phẩm.
/// </summary>
public class CatalogController : Controller
{
    private readonly IRepository<CatalogItem> _itemRepository;
    private readonly IRepository<CatalogBrand> _brandRepository;
    private readonly IRepository<CatalogType> _typeRepository;
    private const int PAGE_SIZE = 9;

    public CatalogController(
        IRepository<CatalogItem> itemRepository,
        IRepository<CatalogBrand> brandRepository,
        IRepository<CatalogType> typeRepository)
    {
        _itemRepository = itemRepository;
        _brandRepository = brandRepository;
        _typeRepository = typeRepository;
    }

    public async Task<IActionResult> Index(
        int? brandId, int? typeId, string? skinType,
        decimal? minPrice, decimal? maxPrice,
        string? searchTerm, int page = 1)
    {
        var skip = (page - 1) * PAGE_SIZE;

        var filterSpec = new CatalogFilterPaginatedSpecification(
            skip, PAGE_SIZE, brandId, typeId, skinType, minPrice, maxPrice, searchTerm);
        var countSpec = new CatalogFilterCountSpecification(
            brandId, typeId, skinType, minPrice, maxPrice, searchTerm);

        var items = await _itemRepository.ListAsync(filterSpec);
        var totalItems = await _itemRepository.CountAsync(countSpec);

        var brands = await _brandRepository.ListAllAsync();
        var types = await _typeRepository.ListAllAsync();

        var viewModel = new CatalogIndexViewModel
        {
            Items = items.Select(i => new CatalogItemViewModel
            {
                Id = i.Id,
                Name = i.Name,
                Description = i.Description,
                Price = i.Price,
                OriginalPrice = i.OriginalPrice,
                PictureUri = i.PictureUri,
                Capacity = i.Capacity,
                SkinType = i.SkinType,
                BrandName = i.CatalogBrand.Name,
                TypeName = i.CatalogType.Name,
                StockQuantity = i.StockQuantity
            }),
            Brands = brands.Select(b => new ViewModels.SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.Name,
                Selected = b.Id == brandId
            }),
            Types = types.Select(t => new ViewModels.SelectListItem
            {
                Value = t.Id.ToString(),
                Text = t.Name,
                Selected = t.Id == typeId
            }),
            SkinTypes = new[] { "Mọi loại da", "Da dầu mụn", "Da khô", "Da nhạy cảm", "Da thường" },
            BrandFilterApplied = brandId,
            TypeFilterApplied = typeId,
            SkinTypeFilterApplied = skinType,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            SearchTerm = searchTerm,
            PaginationInfo = new PaginationInfoViewModel
            {
                TotalItems = totalItems,
                ItemsPerPage = PAGE_SIZE,
                CurrentPage = page
            }
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Details(int id)
    {
        var spec = new CatalogItemSpecification(id);
        var item = await _itemRepository.FirstOrDefaultAsync(spec);

        if (item == null) return NotFound();

        var viewModel = new ProductDetailViewModel
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Price = item.Price,
            OriginalPrice = item.OriginalPrice,
            PictureUri = item.PictureUri,
            Capacity = item.Capacity,
            SkinType = item.SkinType,
            Ingredients = item.Ingredients,
            ExpiryNotice = item.ExpiryNotice,
            StockQuantity = item.StockQuantity,
            BrandName = item.CatalogBrand.Name,
            BrandCountry = item.CatalogBrand.CountryOfOrigin,
            TypeName = item.CatalogType.Name
        };

        return View(viewModel);
    }
}
