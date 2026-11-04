using System.Windows;
using Cosmetics.ApplicationCore.Interfaces;
using Cosmetics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Cosmetics.WpfApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    // Biến toàn cục để mọi màn hình (Window, UserControl) sau này có thể truy cập CSDL
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        // 1. Cấu hình DbContext kết nối SQL Server LocalDB (dùng chung CSDL với dự án)
        services.AddDbContext<CatalogContext>(options =>
            options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CosmeticsShopDb;Trusted_Connection=True;MultipleActiveResultSets=true"));

        // 2. Đăng ký Generic Repository theo chuẩn Clean Architecture
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

        ServiceProvider = services.BuildServiceProvider();

        // 3. Tự động nạp sẵn 12+ sản phẩm mỹ phẩm thực tế nếu CSDL chưa có
        try
        {
            using var scope = ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CatalogContext>();
            var loggerFactory = LoggerFactory.Create(builder => { });
            await CatalogContextSeed.SeedAsync(context, loggerFactory);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi kết nối CSDL: {ex.Message}\nVui lòng đảm bảo dịch vụ SQL Server LocalDB đang chạy.",
                            "Cảnh Báo CSDL", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}