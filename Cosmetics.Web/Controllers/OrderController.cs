using Cosmetics.ApplicationCore.Entities;
using Cosmetics.ApplicationCore.Entities.OrderAggregate;
using Cosmetics.ApplicationCore.Interfaces;
using Cosmetics.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cosmetics.Web.Controllers;

/// <summary>
/// Controller Đơn hàng - Checkout & Lịch sử đơn.
/// </summary>
[Authorize]
public class OrderController : Controller
{
    private readonly IOrderService _orderService;
    private readonly IBasketService _basketService;
    private readonly IRepository<CatalogItem> _itemRepository;

    public OrderController(
        IOrderService orderService,
        IBasketService basketService,
        IRepository<CatalogItem> itemRepository)
    {
        _orderService = orderService;
        _basketService = basketService;
        _itemRepository = itemRepository;
    }

    /// <summary>
    /// Trang Checkout - Nhập thông tin giao hàng.
    /// </summary>
    public async Task<IActionResult> Checkout()
    {
        var buyerId = User.Identity!.Name!;
        var basket = await _basketService.GetOrCreateBasketAsync(buyerId);

        if (!basket.Items.Any())
        {
            TempData["Error"] = "Giỏ hàng trống. Vui lòng thêm sản phẩm trước khi thanh toán.";
            return RedirectToAction("Index", "Basket");
        }

        var basketVm = new BasketViewModel { Id = basket.Id, Items = new List<BasketItemViewModel>() };
        foreach (var item in basket.Items)
        {
            var catalogItem = await _itemRepository.GetByIdAsync(item.CatalogItemId);
            basketVm.Items.Add(new BasketItemViewModel
            {
                CatalogItemId = item.CatalogItemId,
                ProductName = catalogItem?.Name ?? "",
                PictureUri = catalogItem?.PictureUri ?? "",
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity
            });
        }

        var viewModel = new CheckoutViewModel { Basket = basketVm };
        return View(viewModel);
    }

    /// <summary>
    /// Xử lý đặt hàng.
    /// </summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Reload basket cho view
            var buyerId2 = User.Identity!.Name!;
            var basket2 = await _basketService.GetOrCreateBasketAsync(buyerId2);
            var basketVm = new BasketViewModel { Id = basket2.Id, Items = new List<BasketItemViewModel>() };
            foreach (var item in basket2.Items)
            {
                var ci = await _itemRepository.GetByIdAsync(item.CatalogItemId);
                basketVm.Items.Add(new BasketItemViewModel
                {
                    CatalogItemId = item.CatalogItemId,
                    ProductName = ci?.Name ?? "",
                    PictureUri = ci?.PictureUri ?? "",
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity
                });
            }
            model.Basket = basketVm;
            return View("Checkout", model);
        }

        try
        {
            var buyerId = User.Identity!.Name!;
            var address = new Address(
                model.ReceiverName, model.PhoneNumber,
                model.Street, model.Ward, model.District, model.City);

            var paymentMethod = (PaymentMethod)model.PaymentMethod;
            var order = await _orderService.CreateOrderAsync(buyerId, address, paymentMethod, model.Note);

            return RedirectToAction("Success", new { orderId = order.Id });
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("Checkout");
        }
    }

    public async Task<IActionResult> Success(int orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(orderId);
        if (order == null) return NotFound();

        var viewModel = MapOrderToViewModel(order);
        return View(viewModel);
    }

    /// <summary>
    /// Lịch sử đơn hàng của khách hàng.
    /// </summary>
    public async Task<IActionResult> MyOrders()
    {
        var buyerId = User.Identity!.Name!;
        var orders = await _orderService.GetOrdersByBuyerIdAsync(buyerId);

        var viewModels = orders.Select(o => MapOrderToViewModel(o)).ToList();
        return View(viewModels);
    }

    public async Task<IActionResult> Details(int id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        if (order == null) return NotFound();

        // Đảm bảo user chỉ xem đơn hàng của chính mình
        if (order.BuyerId != User.Identity!.Name && !User.IsInRole("Admin"))
            return Forbid();

        var viewModel = MapOrderToViewModel(order);
        return View(viewModel);
    }

    private OrderViewModel MapOrderToViewModel(Order order)
    {
        return new OrderViewModel
        {
            Id = order.Id,
            OrderDate = order.OrderDate,
            Status = GetStatusText(order.Status),
            StatusBadgeClass = GetStatusBadgeClass(order.Status),
            ReceiverName = order.ShipToAddress.ReceiverName,
            PhoneNumber = order.ShipToAddress.PhoneNumber,
            FullAddress = order.ShipToAddress.FullAddress,
            PaymentMethod = GetPaymentText(order.PaymentMethod),
            Note = order.Note,
            Items = order.OrderItems.Select(oi => new OrderItemViewModel
            {
                ProductName = oi.ProductName,
                PictureUri = oi.PictureUri,
                UnitPrice = oi.UnitPrice,
                Units = oi.Units
            }).ToList()
        };
    }

    private static string GetStatusText(OrderStatus status) => status switch
    {
        OrderStatus.Pending => "Chờ xác nhận",
        OrderStatus.Confirmed => "Đã xác nhận",
        OrderStatus.Shipping => "Đang giao hàng",
        OrderStatus.Completed => "Hoàn thành",
        OrderStatus.Cancelled => "Đã hủy",
        _ => "Không xác định"
    };

    private static string GetStatusBadgeClass(OrderStatus status) => status switch
    {
        OrderStatus.Pending => "bg-warning",
        OrderStatus.Confirmed => "bg-info",
        OrderStatus.Shipping => "bg-primary",
        OrderStatus.Completed => "bg-success",
        OrderStatus.Cancelled => "bg-danger",
        _ => "bg-secondary"
    };

    private static string GetPaymentText(PaymentMethod method) => method switch
    {
        PaymentMethod.COD => "Thanh toán khi nhận hàng (COD)",
        PaymentMethod.BankTransfer => "Chuyển khoản ngân hàng",
        PaymentMethod.VNPay => "VNPay",
        _ => "Không xác định"
    };
}
