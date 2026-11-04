using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Cosmetics.ApplicationCore.Entities;
using Cosmetics.ApplicationCore.Entities.OrderAggregate;
using Cosmetics.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cosmetics.WpfApp;

public partial class MainWindow : Window
{
    private List<CatalogItem> _allProducts = new();
    private List<Order> _allOrders = new();
    private readonly ObservableCollection<PosCartItem> _posCart = new();

    public MainWindow()
    {
        InitializeComponent();
        DgPosCart.ItemsSource = _posCart;
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            using var scope = App.ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CatalogContext>();

            // 1. Nạp danh sách sản phẩm kèm Brand & Type
            _allProducts = await context.CatalogItems
                .Include(i => i.CatalogBrand)
                .Include(i => i.CatalogType)
                .OrderByDescending(i => i.Id)
                .ToListAsync();

            // 2. Nạp danh sách đơn hàng kèm OrderItems
            _allOrders = await context.Orders
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            // 3. Cập nhật Dashboard
            UpdateDashboard();

            // 4. Cập nhật POS
            FilterPosProducts();

            // 5. Cập nhật Kho sản phẩm
            FilterCatalogProducts();

            // 6. Cập nhật danh sách đơn hàng
            FilterOrders();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // ================= 1. XỬ LÝ DASHBOARD =================
    private void UpdateDashboard()
    {
        // Doanh thu từ các đơn hoàn thành
        decimal totalRevenue = _allOrders
            .Where(o => o.Status == OrderStatus.Completed)
            .Sum(o => o.TotalPrice);
        TxtStatRevenue.Text = totalRevenue.ToString("N0") + " ₫";

        // Tổng số đơn và đơn đã hoàn thành
        TxtStatOrders.Text = _allOrders.Count.ToString();
        int completedCount = _allOrders.Count(o => o.Status == OrderStatus.Completed);
        TxtStatCompletedOrders.Text = $"Đã hoàn thành: {completedCount}";

        // Tổng sản phẩm và tồn kho thấp
        TxtStatProducts.Text = _allProducts.Count.ToString();
        int lowStockCount = _allProducts.Count(i => i.StockQuantity < 5);
        TxtStatLowStock.Text = lowStockCount.ToString();

        // 5 Đơn hàng gần nhất
        var recentOrders = _allOrders.Take(5).Select(o => new OrderDisplayItem(o)).ToList();
        DgRecentOrders.ItemsSource = recentOrders;
    }

    // ================= 2. XỬ LÝ BÁN HÀNG TẠI QUẦY (POS) =================
    private void FilterPosProducts()
    {
        string query = TxtPosSearch?.Text?.Trim().ToLower() ?? string.Empty;
        var filtered = _allProducts.Where(p =>
            string.IsNullOrEmpty(query) ||
            p.Name.ToLower().Contains(query) ||
            (p.CatalogBrand != null && p.CatalogBrand.Name.ToLower().Contains(query))
        ).ToList();

        DgPosProducts.ItemsSource = filtered;
    }

    private void TxtPosSearch_TextChanged(object sender, TextChangedEventArgs e)
    {
        FilterPosProducts();
    }

    private void DgPosProducts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DgPosProducts.SelectedItem is CatalogItem selected)
        {
            if (selected.StockQuantity <= 0)
            {
                MessageBox.Show($"Sản phẩm '{selected.Name}' đã hết hàng trong kho!", "Hết hàng", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var existing = _posCart.FirstOrDefault(c => c.ProductId == selected.Id);
            if (existing != null)
            {
                if (existing.Quantity >= selected.StockQuantity)
                {
                    MessageBox.Show($"Số lượng trong giỏ đã đạt mức tối đa trong kho ({selected.StockQuantity})!", "Cảnh báo tồn kho", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                existing.Quantity++;
            }
            else
            {
                _posCart.Add(new PosCartItem
                {
                    ProductId = selected.Id,
                    ProductName = selected.Name,
                    UnitPrice = selected.Price,
                    Quantity = 1
                });
            }

            DgPosCart.Items.Refresh();
            UpdatePosCartTotal();
        }
    }

    private void UpdatePosCartTotal()
    {
        decimal total = _posCart.Sum(c => c.Subtotal);
        TxtPosCartTotal.Text = total.ToString("N0") + " ₫";
    }

    private void BtnPosClearCart_Click(object sender, RoutedEventArgs e)
    {
        _posCart.Clear();
        UpdatePosCartTotal();
    }

    private async void BtnPosCheckout_Click(object sender, RoutedEventArgs e)
    {
        if (!_posCart.Any())
        {
            MessageBox.Show("Giỏ hàng đang trống! Vui lòng nhấp đúp vào sản phẩm để thêm vào đơn.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        string customerName = string.IsNullOrWhiteSpace(TxtPosCustomerName.Text) ? "Khách lẻ tại quầy" : TxtPosCustomerName.Text.Trim();
        string phone = string.IsNullOrWhiteSpace(TxtPosCustomerPhone.Text) ? "0901234567" : TxtPosCustomerPhone.Text.Trim();

        try
        {
            using var scope = App.ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CatalogContext>();

            // Tạo danh sách OrderItems
            var orderItems = new List<OrderItem>();
            foreach (var cartItem in _posCart)
            {
                var product = await context.CatalogItems.FindAsync(cartItem.ProductId);
                if (product != null)
                {
                    // Trừ tồn kho
                    product.StockQuantity = Math.Max(0, product.StockQuantity - cartItem.Quantity);
                    orderItems.Add(new OrderItem(product.Id, product.Name, product.PictureUri ?? string.Empty, product.Price, cartItem.Quantity));
                }
            }

            // Tạo Order mới với trạng thái Completed
            var address = new Address(customerName, phone, "Mua trực tiếp tại quầy", "Hàng Bài", "Hoàn Kiếm", "TP. Hà Nội");
            var order = new Order("pos-cashier@beauty.com", address, orderItems, PaymentMethod.COD)
            {
                Status = OrderStatus.Completed,
                Note = "Đơn bán lẻ trực tiếp tại quầy POS Desktop"
            };

            context.Orders.Add(order);
            await context.SaveChangesAsync();

            decimal orderTotal = _posCart.Sum(c => c.Subtotal);
            MessageBox.Show($"Thanh toán thành công hóa đơn #{order.Id}!\nTổng tiền: {orderTotal:N0} ₫\nĐã trừ tồn kho và cập nhật doanh thu Dashboard.", 
                            "Thanh Toán Thành Công", MessageBoxButton.OK, MessageBoxImage.Information);

            // Làm mới giỏ và nạp lại dữ liệu
            _posCart.Clear();
            UpdatePosCartTotal();
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tạo đơn hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // ================= 3. XỬ LÝ KHO SẢN PHẨM =================
    private void FilterCatalogProducts()
    {
        string query = TxtProductFilter?.Text?.Trim().ToLower() ?? string.Empty;
        var filtered = _allProducts.Where(p =>
            string.IsNullOrEmpty(query) ||
            p.Name.ToLower().Contains(query) ||
            (p.CatalogBrand != null && p.CatalogBrand.Name.ToLower().Contains(query)) ||
            (p.CatalogType != null && p.CatalogType.Name.ToLower().Contains(query))
        ).ToList();

        DgProducts.ItemsSource = filtered;
    }

    private void TxtProductFilter_TextChanged(object sender, TextChangedEventArgs e)
    {
        FilterCatalogProducts();
    }

    private void BtnAddProduct_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Chức năng thêm mới sản phẩm đang sẵn sàng. Bạn có thể thêm trực tiếp thông qua form hoặc nạp tự động từ CSDL!", "Thông Báo", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    // ================= 4. XỬ LÝ QUẢN LÝ ĐƠN HÀNG =================
    private void FilterOrders()
    {
        if (DgOrders == null) return;

        string? selectedText = (CboOrderStatusFilter.SelectedItem as ComboBoxItem)?.Content?.ToString();
        var query = _allOrders.AsEnumerable();

        if (!string.IsNullOrEmpty(selectedText) && !selectedText.Contains("Tất cả"))
        {
            if (selectedText.Contains("Hoàn thành")) query = query.Where(o => o.Status == OrderStatus.Completed);
            else if (selectedText.Contains("Chờ xác nhận")) query = query.Where(o => o.Status == OrderStatus.Pending);
            else if (selectedText.Contains("Đang giao")) query = query.Where(o => o.Status == OrderStatus.Shipping);
            else if (selectedText.Contains("Đã hủy")) query = query.Where(o => o.Status == OrderStatus.Cancelled);
        }

        DgOrders.ItemsSource = query.Select(o => new OrderDisplayItem(o)).ToList();
    }

    private void CboOrderStatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        FilterOrders();
    }

    // ================= 5. ĐIỀU HƯỚNG TAB TRÊN SIDEBAR =================
    private void NavButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string tabTag)
        {
            TabDashboard.Visibility = tabTag == "Dashboard" ? Visibility.Visible : Visibility.Collapsed;
            TabPos.Visibility = tabTag == "Pos" ? Visibility.Visible : Visibility.Collapsed;
            TabProducts.Visibility = tabTag == "Products" ? Visibility.Visible : Visibility.Collapsed;
            TabOrders.Visibility = tabTag == "Orders" ? Visibility.Visible : Visibility.Collapsed;

            switch (tabTag)
            {
                case "Dashboard":
                    TxtPageTitle.Text = "Tổng quan Cửa hàng";
                    TxtPageSubtitle.Text = "Theo dõi doanh thu, số lượng đơn hàng và tình trạng kho mỹ phẩm";
                    break;
                case "Pos":
                    TxtPageTitle.Text = "Bán Hàng Tại Quầy (POS)";
                    TxtPageSubtitle.Text = "Nhấp đúp chuột vào mỹ phẩm bên trái để thêm vào hóa đơn thanh toán";
                    break;
                case "Products":
                    TxtPageTitle.Text = "Kho Mỹ Phẩm Chính Hãng";
                    TxtPageSubtitle.Text = "Quản lý danh sách, giá cả và số lượng tồn kho sản phẩm";
                    break;
                case "Orders":
                    TxtPageTitle.Text = "Quản Lý Đơn Hàng";
                    TxtPageSubtitle.Text = "Tra cứu tiến độ giao hàng và doanh thu từng đơn của khách";
                    break;
            }
        }
    }

    private async void BtnRefreshAll_Click(object sender, RoutedEventArgs e)
    {
        await LoadDataAsync();
        MessageBox.Show("Dữ liệu đã được đồng bộ mới nhất từ CSDL SQL Server!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}

// Model hỗ trợ hiển thị giỏ hàng bán lẻ POS
public class PosCartItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public decimal Subtotal => UnitPrice * Quantity;
    public string FormattedSubtotal => Subtotal.ToString("N0") + " ₫";
}

// Model hỗ trợ hiển thị danh sách đơn hàng
public class OrderDisplayItem
{
    public OrderDisplayItem(Order o)
    {
        Id = o.Id;
        ShipToAddress = o.ShipToAddress;
        PaymentMethod = o.PaymentMethod.ToString();
        TotalPrice = o.TotalPrice;
        Status = o.Status;
        OrderDate = o.OrderDate;
    }

    public int Id { get; set; }
    public Address ShipToAddress { get; set; }
    public string PaymentMethod { get; set; }
    public decimal TotalPrice { get; set; }
    public string FormattedTotalPrice => TotalPrice.ToString("N0") + " ₫";
    public OrderStatus Status { get; set; }
    public string StatusText => Status switch
    {
        OrderStatus.Pending => "Chờ xác nhận",
        OrderStatus.Confirmed => "Đã xác nhận",
        OrderStatus.Shipping => "Đang giao",
        OrderStatus.Completed => "Hoàn thành",
        OrderStatus.Cancelled => "Đã hủy",
        _ => Status.ToString()
    };
    public DateTimeOffset OrderDate { get; set; }
    public string FormattedOrderDate => OrderDate.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
}
