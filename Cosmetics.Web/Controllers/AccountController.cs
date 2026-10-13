using Cosmetics.ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Cosmetics.Web.Controllers;

/// <summary>
/// Controller Tài khoản - Đăng nhập, Đăng ký, Đăng xuất.
/// </summary>
public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IBasketService _basketService;

    public AccountController(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        IBasketService basketService)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _basketService = basketService;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string email, string password, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError("", "Vui lòng nhập email và mật khẩu.");
            return View();
        }

        // Lưu basket cookie trước khi login
        var anonymousId = Request.Cookies["BasketId"];

        var result = await _signInManager.PasswordSignInAsync(email, password, isPersistent: true, lockoutOnFailure: false);
        if (result.Succeeded)
        {
            // Transfer giỏ hàng ẩn danh sang tài khoản
            if (!string.IsNullOrEmpty(anonymousId))
            {
                await _basketService.TransferBasketAsync(anonymousId, email);
                Response.Cookies.Delete("BasketId");
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Catalog");
        }

        ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(string email, string password, string confirmPassword)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError("", "Vui lòng điền đầy đủ thông tin.");
            return View();
        }

        if (password != confirmPassword)
        {
            ModelState.AddModelError("", "Mật khẩu xác nhận không khớp.");
            return View();
        }

        var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
        var result = await _userManager.CreateAsync(user, password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "Customer");
            await _signInManager.SignInAsync(user, isPersistent: true);

            // Transfer giỏ hàng
            var anonymousId = Request.Cookies["BasketId"];
            if (!string.IsNullOrEmpty(anonymousId))
            {
                await _basketService.TransferBasketAsync(anonymousId, email);
                Response.Cookies.Delete("BasketId");
            }

            return RedirectToAction("Index", "Catalog");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error.Description);
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Catalog");
    }

    public IActionResult AccessDenied()
    {
        return View();
    }
}
