using Cosmetics.ApplicationCore.Entities;
using Cosmetics.ApplicationCore.Interfaces;
using Cosmetics.ApplicationCore.Specifications;
using Cosmetics.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Cosmetics.Web.Controllers;

/// <summary>
/// Controller Giỏ hàng.
/// </summary>
public class BasketController : Controller
{
    private readonly IBasketService _basketService;
    private readonly IRepository<CatalogItem> _itemRepository;

    public BasketController(IBasketService basketService, IRepository<CatalogItem> itemRepository)
    {
        _basketService = basketService;
        _itemRepository = itemRepository;
    }

    private string GetOrSetBasketCookie()
    {
        if (User.Identity?.IsAuthenticated == true)
            return User.Identity.Name!;

        if (Request.Cookies.ContainsKey("BasketId"))
            return Request.Cookies["BasketId"]!;

        var basketId = Guid.NewGuid().ToString();
        var cookieOptions = new CookieOptions
        {
            Expires = DateTimeOffset.Now.AddDays(14),
            IsEssential = true,
            HttpOnly = true
        };
        Response.Cookies.Append("BasketId", basketId, cookieOptions);
        return basketId;
    }

    public async Task<IActionResult> Index()
    {
        var buyerId = GetOrSetBasketCookie();
        var basket = await _basketService.GetOrCreateBasketAsync(buyerId);

        var viewModel = new BasketViewModel
        {
            Id = basket.Id,
            Items = new List<BasketItemViewModel>()
        };

        foreach (var item in basket.Items)
        {
            var catalogItem = await _itemRepository.GetByIdAsync(item.CatalogItemId);
            viewModel.Items.Add(new BasketItemViewModel
            {
                Id = item.Id,
                CatalogItemId = item.CatalogItemId,
                ProductName = catalogItem?.Name ?? "Sản phẩm không tồn tại",
                PictureUri = catalogItem?.PictureUri ?? "",
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity
            });
        }

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddToBasket(int catalogItemId, int quantity = 1)
    {
        var buyerId = GetOrSetBasketCookie();
        var item = await _itemRepository.GetByIdAsync(catalogItemId);

        if (item == null) return NotFound();
        if (item.StockQuantity < quantity)
        {
            TempData["Error"] = "Số lượng trong kho không đủ.";
            return RedirectToAction("Details", "Catalog", new { id = catalogItemId });
        }

        await _basketService.AddItemToBasketAsync(buyerId, catalogItemId, item.Price, quantity);

        TempData["Success"] = $"Đã thêm \"{item.Name}\" vào giỏ hàng!";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateQuantities(Dictionary<int, int> quantities)
    {
        var buyerId = GetOrSetBasketCookie();
        var basket = await _basketService.GetOrCreateBasketAsync(buyerId);
        await _basketService.SetQuantitiesAsync(basket.Id, quantities);
        return RedirectToAction("Index");
    }

    [HttpPost, HttpGet]
    public async Task<IActionResult> RemoveItem(int basketItemId)
    {
        var buyerId = GetOrSetBasketCookie();
        var basket = await _basketService.GetOrCreateBasketAsync(buyerId);
        var quantities = new Dictionary<int, int> { { basketItemId, 0 } };
        await _basketService.SetQuantitiesAsync(basket.Id, quantities);
        TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng thành công!";
        return RedirectToAction("Index");
    }

    /// <summary>
    /// Lấy số lượng sản phẩm trong giỏ hàng (cho badge trên Header).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetBasketItemCount()
    {
        var buyerId = GetOrSetBasketCookie();
        var basket = await _basketService.GetOrCreateBasketAsync(buyerId);
        return Json(new { count = basket.Items.Sum(i => i.Quantity) });
    }
}
