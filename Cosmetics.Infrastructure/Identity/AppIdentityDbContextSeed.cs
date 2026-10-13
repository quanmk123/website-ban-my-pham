using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cosmetics.Infrastructure.Identity;

/// <summary>
/// Seed Data cho Identity - Tạo tài khoản Admin và Customer mặc định.
/// </summary>
public class AppIdentityDbContextSeed
{
    public static async Task SeedAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Tạo Roles
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        if (!await roleManager.RoleExistsAsync("Customer"))
        {
            await roleManager.CreateAsync(new IdentityRole("Customer"));
        }

        // Tạo Admin user
        var adminUser = await userManager.FindByEmailAsync("admin@beauty.com");
        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = "admin@beauty.com",
                Email = "admin@beauty.com",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(adminUser, "P@ssword123");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // Tạo Customer user
        var customerUser = await userManager.FindByEmailAsync("customer@beauty.com");
        if (customerUser == null)
        {
            customerUser = new IdentityUser
            {
                UserName = "customer@beauty.com",
                Email = "customer@beauty.com",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(customerUser, "P@ssword123");
            await userManager.AddToRoleAsync(customerUser, "Customer");
        }
    }
}
