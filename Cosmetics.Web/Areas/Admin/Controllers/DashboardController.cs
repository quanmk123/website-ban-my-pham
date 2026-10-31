using Cosmetics.ApplicationCore.Entities;
using Cosmetics.ApplicationCore.Entities.OrderAggregate;
using Cosmetics.ApplicationCore.Interfaces;
using Cosmetics.ApplicationCore.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cosmetics.Web.Areas.Admin.Controllers;

/// <summary>
/// Admin Dashboard - Quản trị hệ thống.
/// </summary>
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class DashboardController : Controller
{
    private readonly IRepository<CatalogItem> _itemRepository;
    private readonly IRepository<CatalogBrand> _brandRepository;
    private readonly IRepository<CatalogType> _typeRepository;
    private readonly IRepository<Order> _orderRepository;

    public DashboardController(
        IRepository<CatalogItem> itemRepository,
        IRepository<CatalogBrand> brandRepository,
        IRepository<CatalogType> typeRepository,
        IRepository<Order> orderRepository)
    {
        _itemRepository = itemRepository;
        _brandRepository = brandRepository;
        _typeRepository = typeRepository;
        _orderRepository = orderRepository;
    }

    public async Task<IActionResult> Index()
    {
        var items = await _itemRepository.ListAllAsync();
        var spec = new OrderWithItemsSpecification();
        var orders = await _orderRepository.ListAsync(spec);

        ViewBag.TotalProducts = items.Count;
        ViewBag.LowStockProducts = items.Count(i => i.StockQuantity < 5);
        ViewBag.TotalOrders = orders.Count;
        ViewBag.CompletedOrders = orders.Count(o => o.Status == OrderStatus.Completed);
        ViewBag.PendingOrders = orders.Count(o => o.Status == OrderStatus.Pending);
        ViewBag.TotalRevenue = orders
            .Where(o => o.Status == OrderStatus.Completed)
            .Sum(o => o.TotalPrice);
        ViewBag.LowStockItems = items.Where(i => i.StockQuantity < 5).ToList();

        return View();
    }

    // === QUẢN LÝ SẢN PHẨM ===
    public async Task<IActionResult> Products(string? searchTerm, int page = 1)
    {
        int pageSize = 10;
        var skip = (page - 1) * pageSize;

        var spec = new CatalogFilterPaginatedSpecification(skip, pageSize, searchTerm: searchTerm);
        var countSpec = new CatalogFilterCountSpecification(searchTerm: searchTerm);

        var items = await _itemRepository.ListAsync(spec);
        var totalItems = await _itemRepository.CountAsync(countSpec);

        ViewBag.SearchTerm = searchTerm;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
        ViewBag.TotalItems = totalItems;

        return View(items);
    }

    public async Task<IActionResult> CreateProduct()
    {
        ViewBag.Brands = await _brandRepository.ListAllAsync();
        ViewBag.Types = await _typeRepository.ListAllAsync();
        return View(new CatalogItem());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProduct(CatalogItem item, IFormFile? imageFile)
    {
        ModelState.Remove(nameof(item.CatalogBrand));
        ModelState.Remove(nameof(item.CatalogType));
        if (string.IsNullOrWhiteSpace(item.SkinType))
        {
            item.SkinType = "Mọi loại da";
        }
        ModelState.Remove(nameof(item.SkinType));
        ModelState.Remove(nameof(item.PictureUri));

        if (ModelState.IsValid)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                using var stream = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(stream);
                item.PictureUri = $"/images/products/{fileName}";
            }
            else if (string.IsNullOrEmpty(item.PictureUri))
            {
                item.PictureUri = "/images/placeholder.png";
            }

            await _itemRepository.AddAsync(item);
            TempData["Success"] = "Thêm sản phẩm thành công!";
            return RedirectToAction("Products");
        }

        ViewBag.Brands = await _brandRepository.ListAllAsync();
        ViewBag.Types = await _typeRepository.ListAllAsync();
        return View(item);
    }

    public async Task<IActionResult> EditProduct(int id)
    {
        var spec = new CatalogItemSpecification(id);
        var item = await _itemRepository.FirstOrDefaultAsync(spec);
        if (item == null) return NotFound();

        ViewBag.Brands = await _brandRepository.ListAllAsync();
        ViewBag.Types = await _typeRepository.ListAllAsync();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(CatalogItem item, IFormFile? imageFile)
    {
        ModelState.Remove(nameof(item.CatalogBrand));
        ModelState.Remove(nameof(item.CatalogType));
        if (string.IsNullOrWhiteSpace(item.SkinType))
        {
            item.SkinType = "Mọi loại da";
        }
        ModelState.Remove(nameof(item.SkinType));
        ModelState.Remove(nameof(item.PictureUri));

        if (ModelState.IsValid)
        {
            var existingItem = await _itemRepository.GetByIdAsync(item.Id);
            if (existingItem == null) return NotFound();

            existingItem.Name = item.Name;
            existingItem.Description = item.Description ?? string.Empty;
            existingItem.Price = item.Price;
            existingItem.OriginalPrice = item.OriginalPrice;
            existingItem.Capacity = item.Capacity ?? string.Empty;
            existingItem.SkinType = item.SkinType ?? "Mọi loại da";
            existingItem.Ingredients = item.Ingredients ?? string.Empty;
            existingItem.ExpiryNotice = item.ExpiryNotice ?? string.Empty;
            existingItem.StockQuantity = item.StockQuantity;
            existingItem.CatalogBrandId = item.CatalogBrandId;
            existingItem.CatalogTypeId = item.CatalogTypeId;

            if (imageFile != null && imageFile.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/products", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
                using var stream = new FileStream(filePath, FileMode.Create);
                await imageFile.CopyToAsync(stream);
                existingItem.PictureUri = $"/images/products/{fileName}";
            }
            else if (!string.IsNullOrEmpty(item.PictureUri))
            {
                existingItem.PictureUri = item.PictureUri;
            }

            await _itemRepository.UpdateAsync(existingItem);
            TempData["Success"] = "Cập nhật sản phẩm thành công!";
            return RedirectToAction("Products");
        }

        ViewBag.Brands = await _brandRepository.ListAllAsync();
        ViewBag.Types = await _typeRepository.ListAllAsync();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var item = await _itemRepository.GetByIdAsync(id);
        if (item != null)
        {
            await _itemRepository.DeleteAsync(item);
            TempData["Success"] = "Xóa sản phẩm thành công!";
        }
        return RedirectToAction("Products");
    }

    // === QUẢN LÝ THƯƠNG HIỆU ===
    public async Task<IActionResult> Brands()
    {
        var brands = await _brandRepository.ListAllAsync();
        return View(brands);
    }

    public IActionResult CreateBrand() => View(new CatalogBrand());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateBrand(CatalogBrand brand)
    {
        if (ModelState.IsValid)
        {
            await _brandRepository.AddAsync(brand);
            TempData["Success"] = "Thêm thương hiệu thành công!";
            return RedirectToAction("Brands");
        }
        return View(brand);
    }

    // === QUẢN LÝ DANH MỤC ===
    public async Task<IActionResult> Types()
    {
        var types = await _typeRepository.ListAllAsync();
        return View(types);
    }

    public IActionResult CreateType() => View(new CatalogType());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateType(CatalogType type)
    {
        if (ModelState.IsValid)
        {
            await _typeRepository.AddAsync(type);
            TempData["Success"] = "Thêm danh mục thành công!";
            return RedirectToAction("Types");
        }
        return View(type);
    }

    // === QUẢN LÝ ĐƠN HÀNG ===
    public async Task<IActionResult> Orders(string? status)
    {
        var spec = new OrderWithItemsSpecification();
        var allOrders = await _orderRepository.ListAsync(spec);

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, out var statusEnum))
        {
            allOrders = allOrders.Where(o => o.Status == statusEnum).ToList().AsReadOnly();
        }

        // Sort by date descending
        allOrders = allOrders.OrderByDescending(o => o.OrderDate).ToList().AsReadOnly();

        ViewBag.StatusFilter = status;
        return View(allOrders);
    }

    public async Task<IActionResult> OrderDetail(int id)
    {
        var spec = new OrderWithItemsSpecification(id);
        var order = await _orderRepository.FirstOrDefaultAsync(spec);
        if (order == null) return NotFound();
        return View(order);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateOrderStatus(int orderId, OrderStatus newStatus)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order != null)
        {
            order.Status = newStatus;
            await _orderRepository.UpdateAsync(order);
            TempData["Success"] = "Cập nhật trạng thái đơn hàng thành công!";
        }
        return RedirectToAction("OrderDetail", new { id = orderId });
    }
}
