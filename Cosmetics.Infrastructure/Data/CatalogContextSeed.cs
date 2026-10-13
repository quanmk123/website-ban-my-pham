using Cosmetics.ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cosmetics.Infrastructure.Data;

/// <summary>
/// Seed Data cho sản phẩm mỹ phẩm.
/// Tự động khởi tạo dữ liệu mẫu khi chạy lần đầu.
/// </summary>
public class CatalogContextSeed
{
    public static async Task SeedAsync(CatalogContext context, ILoggerFactory loggerFactory)
    {
        try
        {
            await context.Database.EnsureCreatedAsync();

            if (!await context.CatalogBrands.AnyAsync())
            {
                await context.CatalogBrands.AddRangeAsync(GetPreconfiguredBrands());
                await context.SaveChangesAsync();
            }

            if (!await context.CatalogTypes.AnyAsync())
            {
                await context.CatalogTypes.AddRangeAsync(GetPreconfiguredTypes());
                await context.SaveChangesAsync();
            }

            if (!await context.CatalogItems.AnyAsync())
            {
                await context.CatalogItems.AddRangeAsync(GetPreconfiguredItems());
                await context.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            var logger = loggerFactory.CreateLogger<CatalogContextSeed>();
            logger.LogError(ex, "Lỗi khi khởi tạo dữ liệu mẫu Catalog.");
        }
    }

    /// <summary>
    /// 6 Thương hiệu mỹ phẩm nổi tiếng.
    /// </summary>
    private static IEnumerable<CatalogBrand> GetPreconfiguredBrands()
    {
        return new List<CatalogBrand>
        {
            new() { Name = "La Roche-Posay", Description = "Thương hiệu dược mỹ phẩm hàng đầu từ Pháp, chuyên sản phẩm cho da nhạy cảm.", LogoUrl = "/images/brands/la-roche-posay.png", CountryOfOrigin = "Pháp" },
            new() { Name = "Innisfree", Description = "Thương hiệu mỹ phẩm thiên nhiên từ đảo Jeju, Hàn Quốc.", LogoUrl = "/images/brands/innisfree.png", CountryOfOrigin = "Hàn Quốc" },
            new() { Name = "CeraVe", Description = "Thương hiệu dưỡng da được bác sĩ khuyên dùng, chứa Ceramides phục hồi hàng rào bảo vệ da.", LogoUrl = "/images/brands/cerave.png", CountryOfOrigin = "Mỹ" },
            new() { Name = "Anessa", Description = "Thương hiệu kem chống nắng số 1 Nhật Bản từ tập đoàn Shiseido.", LogoUrl = "/images/brands/anessa.png", CountryOfOrigin = "Nhật Bản" },
            new() { Name = "MAC", Description = "Thương hiệu trang điểm chuyên nghiệp cao cấp, được các chuyên gia makeup tin dùng.", LogoUrl = "/images/brands/mac.png", CountryOfOrigin = "Mỹ" },
            new() { Name = "Klairs", Description = "Thương hiệu mỹ phẩm thuần chay từ Hàn Quốc, an toàn cho da nhạy cảm.", LogoUrl = "/images/brands/klairs.png", CountryOfOrigin = "Hàn Quốc" },
        };
    }

    /// <summary>
    /// 5 Danh mục sản phẩm mỹ phẩm.
    /// </summary>
    private static IEnumerable<CatalogType> GetPreconfiguredTypes()
    {
        return new List<CatalogType>
        {
            new() { Name = "Kem chống nắng" },
            new() { Name = "Serum dưỡng da" },
            new() { Name = "Nước tẩy trang" },
            new() { Name = "Son môi" },
            new() { Name = "Kem dưỡng ẩm" },
        };
    }

    /// <summary>
    /// 12+ Sản phẩm mỹ phẩm thực tế với giá VNĐ.
    /// </summary>
    private static IEnumerable<CatalogItem> GetPreconfiguredItems()
    {
        return new List<CatalogItem>
        {
            // === KEM CHỐNG NẮNG (TypeId = 1) ===
            new()
            {
                Name = "La Roche-Posay Anthelios UV Mune 400",
                Description = "Kem chống nắng phổ rộng SPF50+ PA++++ bảo vệ da khỏi tia UVA dài, ngăn ngừa đốm nâu và lão hóa. Kết cấu mỏng nhẹ, thấm nhanh, không gây nhờn rít. Phù hợp mọi loại da, kể cả da nhạy cảm.",
                Price = 420000, OriginalPrice = 495000,
                PictureUri = "/images/products/lrp-anthelios.jpg",
                Capacity = "50ml", SkinType = "Mọi loại da",
                Ingredients = "Mexoryl 400, Niacinamide, Thermal Spring Water",
                ExpiryNotice = "3 năm kể từ NSX, 12 tháng sau mở nắp",
                StockQuantity = 50, CatalogBrandId = 1, CatalogTypeId = 1
            },
            new()
            {
                Name = "Anessa Perfect UV Sunscreen Skincare Milk",
                Description = "Sữa chống nắng bảo vệ hoàn hảo SPF50+ PA++++ với công nghệ Auto Booster, chống nắng mạnh mẽ hơn khi tiếp xúc với nước, mồ hôi và nhiệt. Bổ sung 50% thành phần dưỡng da.",
                Price = 550000, OriginalPrice = 650000,
                PictureUri = "/images/products/anessa-perfect-uv.jpg",
                Capacity = "60ml", SkinType = "Mọi loại da",
                Ingredients = "Hyaluronic Acid, Collagen, Super Waterproof",
                ExpiryNotice = "3 năm kể từ NSX",
                StockQuantity = 35, CatalogBrandId = 4, CatalogTypeId = 1
            },
            new()
            {
                Name = "Innisfree Daily UV Defense Sunscreen",
                Description = "Kem chống nắng hàng ngày SPF36 PA+++ với chiết xuất hoa hướng dương, bảo vệ da nhẹ nhàng. Kết cấu lotion mỏng nhẹ, dễ dàng tán đều, không bết dính.",
                Price = 250000, OriginalPrice = 320000,
                PictureUri = "/images/products/innisfree-daily-uv.jpg",
                Capacity = "50ml", SkinType = "Da thường, Da khô",
                Ingredients = "Sunflower Seed Oil, Centella Asiatica, Green Tea",
                ExpiryNotice = "30 tháng kể từ NSX",
                StockQuantity = 45, CatalogBrandId = 2, CatalogTypeId = 1
            },

            // === SERUM DƯỠNG DA (TypeId = 2) ===
            new()
            {
                Name = "La Roche-Posay Hyalu B5 Serum",
                Description = "Serum dưỡng ẩm chuyên sâu với 2 loại Hyaluronic Acid và Vitamin B5, giúp làm đầy nếp nhăn, phục hồi và tái tạo da. Da căng mọng, mịn màng sau 1 giờ sử dụng.",
                Price = 680000, OriginalPrice = 790000,
                PictureUri = "/images/products/lrp-hyalu-b5.jpg",
                Capacity = "30ml", SkinType = "Mọi loại da",
                Ingredients = "Hyaluronic Acid, Vitamin B5, Madecassoside",
                ExpiryNotice = "3 năm kể từ NSX, 6 tháng sau mở nắp",
                StockQuantity = 30, CatalogBrandId = 1, CatalogTypeId = 2
            },
            new()
            {
                Name = "Klairs Freshly Juiced Vitamin Drop",
                Description = "Serum Vitamin C 5% tinh khiết, an toàn cho da nhạy cảm. Giúp làm sáng da, mờ thâm, đều màu da mà không gây kích ứng. Kết cấu lỏng nhẹ, thấm nhanh.",
                Price = 380000, OriginalPrice = 450000,
                PictureUri = "/images/products/klairs-vitamin-c.jpg",
                Capacity = "35ml", SkinType = "Da nhạy cảm, Mọi loại da",
                Ingredients = "Ascorbic Acid 5%, Centella Asiatica, Portulaca Oleracea",
                ExpiryNotice = "24 tháng kể từ NSX, 3 tháng sau mở nắp",
                StockQuantity = 40, CatalogBrandId = 6, CatalogTypeId = 2
            },
            new()
            {
                Name = "CeraVe Hydrating Hyaluronic Acid Serum",
                Description = "Serum cấp ẩm chuyên sâu với 3 loại Ceramides thiết yếu và Hyaluronic Acid. Phục hồi hàng rào bảo vệ da, giữ ẩm suốt 24 giờ. Không chứa dầu, không gây mụn.",
                Price = 450000, OriginalPrice = 520000,
                PictureUri = "/images/products/cerave-ha-serum.jpg",
                Capacity = "30ml", SkinType = "Da khô, Da thường",
                Ingredients = "Ceramides (1, 3, 6-II), Hyaluronic Acid, MVE Technology",
                ExpiryNotice = "3 năm kể từ NSX",
                StockQuantity = 25, CatalogBrandId = 3, CatalogTypeId = 2
            },

            // === NƯỚC TẨY TRANG (TypeId = 3) ===
            new()
            {
                Name = "La Roche-Posay Micellar Water Ultra",
                Description = "Nước tẩy trang Micellar dịu nhẹ, làm sạch sâu lớp makeup và bụi bẩn mà không cần rửa lại. Chứa nước khoáng Thermal Spring Water giúp làm dịu da.",
                Price = 350000, OriginalPrice = 420000,
                PictureUri = "/images/products/lrp-micellar.jpg",
                Capacity = "400ml", SkinType = "Da nhạy cảm, Mọi loại da",
                Ingredients = "Glycerin, Thermal Spring Water, Micelles",
                ExpiryNotice = "3 năm kể từ NSX",
                StockQuantity = 60, CatalogBrandId = 1, CatalogTypeId = 3
            },
            new()
            {
                Name = "Innisfree Green Tea Cleansing Water",
                Description = "Nước tẩy trang trà xanh từ đảo Jeju, loại bỏ lớp makeup nhẹ nhàng và cấp ẩm cho da. Hương trà xanh thư giãn, không chứa cồn.",
                Price = 220000, OriginalPrice = 280000,
                PictureUri = "/images/products/innisfree-cleansing-water.jpg",
                Capacity = "300ml", SkinType = "Mọi loại da",
                Ingredients = "Green Tea Extract, Betaine, Panthenol",
                ExpiryNotice = "30 tháng kể từ NSX",
                StockQuantity = 55, CatalogBrandId = 2, CatalogTypeId = 3
            },

            // === SON MÔI (TypeId = 4) ===
            new()
            {
                Name = "MAC Matte Lipstick - Ruby Woo",
                Description = "Son lì huyền thoại với sắc đỏ Ruby cổ điển, bám màu lâu trôi suốt 8 giờ. Công thức Retro Matte siêu lì, lên màu chuẩn ngay từ lần quẹt đầu tiên. Best seller toàn cầu.",
                Price = 520000, OriginalPrice = 600000,
                PictureUri = "/images/products/mac-ruby-woo.jpg",
                Capacity = "3g", SkinType = "Mọi loại da",
                Ingredients = "Candelilla Wax, Carnauba Wax, Color Pigments",
                ExpiryNotice = "3 năm kể từ NSX",
                StockQuantity = 40, CatalogBrandId = 5, CatalogTypeId = 4
            },
            new()
            {
                Name = "MAC Powder Kiss Lipstick - Devoted To Chili",
                Description = "Son bán lì Powder Kiss với sắc đỏ gạch trendy, công thức Moisture Matte giữ ẩm cho môi mềm mượt suốt ngày. Kết cấu mịn như phấn, nhẹ như không.",
                Price = 550000, OriginalPrice = 620000,
                PictureUri = "/images/products/mac-devoted-to-chili.jpg",
                Capacity = "3g", SkinType = "Mọi loại da",
                Ingredients = "Coconut Oil, Vitamin E, Moisture Matte Complex",
                ExpiryNotice = "3 năm kể từ NSX",
                StockQuantity = 35, CatalogBrandId = 5, CatalogTypeId = 4
            },

            // === KEM DƯỠNG ẨM (TypeId = 5) ===
            new()
            {
                Name = "CeraVe Moisturizing Cream",
                Description = "Kem dưỡng ẩm phục hồi da với 3 loại Ceramides và công nghệ MVE cho hiệu quả giữ ẩm 24 giờ. Không chứa hương liệu, không gây mụn. Được bác sĩ da liễu khuyên dùng.",
                Price = 350000, OriginalPrice = 420000,
                PictureUri = "/images/products/cerave-moisturizing.jpg",
                Capacity = "340g", SkinType = "Da khô, Da thường",
                Ingredients = "Ceramides (1, 3, 6-II), Hyaluronic Acid, Petrolatum",
                ExpiryNotice = "3 năm kể từ NSX",
                StockQuantity = 45, CatalogBrandId = 3, CatalogTypeId = 5
            },
            new()
            {
                Name = "Klairs Midnight Blue Calming Cream",
                Description = "Kem dưỡng ẩm xanh dịu nhẹ với Guaiazulene chiết xuất từ cây cam cúc, giúp làm dịu da kích ứng, mẩn đỏ. An toàn tuyệt đối cho da nhạy cảm và da sau peel.",
                Price = 480000, OriginalPrice = 560000,
                PictureUri = "/images/products/klairs-midnight-blue.jpg",
                Capacity = "30ml", SkinType = "Da nhạy cảm, Da mụn",
                Ingredients = "Guaiazulene, Centella Asiatica, Ceramide NP",
                ExpiryNotice = "24 tháng kể từ NSX",
                StockQuantity = 20, CatalogBrandId = 6, CatalogTypeId = 5
            },
        };
    }
}
