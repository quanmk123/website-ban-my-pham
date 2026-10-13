using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Cosmetics.Infrastructure.Identity;

/// <summary>
/// DbContext cho ASP.NET Core Identity (Xác thực & Phân quyền).
/// </summary>
public class AppIdentityDbContext : IdentityDbContext
{
    public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options) { }
}
