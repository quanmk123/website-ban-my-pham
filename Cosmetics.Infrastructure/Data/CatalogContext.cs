using Cosmetics.ApplicationCore.Entities;
using Cosmetics.ApplicationCore.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;

namespace Cosmetics.Infrastructure.Data;

/// <summary>
/// DbContext chính cho Catalog, Basket, Order.
/// Cấu hình Entity theo Fluent API.
/// </summary>
public class CatalogContext : DbContext
{
    public CatalogContext(DbContextOptions<CatalogContext> options) : base(options) { }

    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();
    public DbSet<CatalogBrand> CatalogBrands => Set<CatalogBrand>();
    public DbSet<CatalogType> CatalogTypes => Set<CatalogType>();
    public DbSet<Basket> Baskets => Set<Basket>();
    public DbSet<BasketItem> BasketItems => Set<BasketItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // === CATALOG ===
        builder.Entity<CatalogItem>(entity =>
        {
            entity.ToTable("CatalogItems");
            entity.Property(ci => ci.Name).IsRequired().HasMaxLength(200);
            entity.Property(ci => ci.Price).HasColumnType("decimal(18,0)");
            entity.Property(ci => ci.OriginalPrice).HasColumnType("decimal(18,0)");
            entity.Property(ci => ci.Capacity).HasMaxLength(50);
            entity.Property(ci => ci.SkinType).HasMaxLength(100);
            entity.Property(ci => ci.Ingredients).HasMaxLength(1000);
            entity.Property(ci => ci.ExpiryNotice).HasMaxLength(200);
            entity.Property(ci => ci.PictureUri).HasMaxLength(500);

            entity.HasOne(ci => ci.CatalogBrand)
                .WithMany()
                .HasForeignKey(ci => ci.CatalogBrandId);

            entity.HasOne(ci => ci.CatalogType)
                .WithMany()
                .HasForeignKey(ci => ci.CatalogTypeId);
        });

        builder.Entity<CatalogBrand>(entity =>
        {
            entity.ToTable("CatalogBrands");
            entity.Property(cb => cb.Name).IsRequired().HasMaxLength(200);
            entity.Property(cb => cb.LogoUrl).HasMaxLength(500);
            entity.Property(cb => cb.CountryOfOrigin).HasMaxLength(100);
        });

        builder.Entity<CatalogType>(entity =>
        {
            entity.ToTable("CatalogTypes");
            entity.Property(ct => ct.Name).IsRequired().HasMaxLength(200);
            entity.Property(ct => ct.Description).HasMaxLength(500);
        });

        // === BASKET ===
        builder.Entity<Basket>(entity =>
        {
            entity.ToTable("Baskets");
            entity.Property(b => b.BuyerId).IsRequired().HasMaxLength(256);

            entity.HasMany(b => b.Items)
                .WithOne()
                .HasForeignKey(bi => bi.BasketId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<BasketItem>(entity =>
        {
            entity.ToTable("BasketItems");
            entity.Property(bi => bi.UnitPrice).HasColumnType("decimal(18,0)");
        });

        // === ORDER ===
        builder.Entity<Order>(entity =>
        {
            entity.ToTable("Orders");
            entity.Property(o => o.BuyerId).IsRequired().HasMaxLength(256);

            entity.OwnsOne(o => o.ShipToAddress, a =>
            {
                a.WithOwner();
                a.Property(p => p.ReceiverName).HasMaxLength(200);
                a.Property(p => p.PhoneNumber).HasMaxLength(20);
                a.Property(p => p.Street).HasMaxLength(300);
                a.Property(p => p.Ward).HasMaxLength(100);
                a.Property(p => p.District).HasMaxLength(100);
                a.Property(p => p.City).HasMaxLength(100);
            });

            entity.HasMany(o => o.OrderItems)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItems");
            entity.Property(oi => oi.ProductName).HasMaxLength(200);
            entity.Property(oi => oi.PictureUri).HasMaxLength(500);
            entity.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,0)");
        });
    }
}
