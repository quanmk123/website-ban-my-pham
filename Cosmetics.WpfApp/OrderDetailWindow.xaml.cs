using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Cosmetics.ApplicationCore.Entities.OrderAggregate;
using Cosmetics.Infrastructure.Data;

namespace Cosmetics.WpfApp;

public partial class OrderDetailWindow : Window
{
    private readonly int _orderId;
    private Order? _order;

    public OrderDetailWindow(int orderId)
    {
        InitializeComponent();
        _orderId = orderId;
        Loaded += OrderDetailWindow_Loaded;
    }

    private async void OrderDetailWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            using var scope = App.ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CatalogContext>();

            _order = await context.Orders
                .Include(o => o.OrderItems)
                .FirstOrDefaultAsync(o => o.Id == _orderId);

            if (_order == null)
            {
                MessageBox.Show("Không tìm thấy đơn hàng trong CSDL!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            TxtOrderTitle.Text = $"Đơn Hàng #{_order.Id}";
            TxtOrderDate.Text = $"Ngày đặt: {_order.OrderDate.ToLocalTime():dd/MM/yyyy HH:mm}";

            // Khách hàng & Giao hàng
            string name = _order.ShipToAddress?.ReceiverName ?? "Khách vãng lai";
            string phone = _order.ShipToAddress?.PhoneNumber ?? "Không có";
            string street = _order.ShipToAddress?.Street ?? string.Empty;
            string ward = _order.ShipToAddress?.Ward ?? string.Empty;
            string district = _order.ShipToAddress?.District ?? string.Empty;
            string city = _order.ShipToAddress?.City ?? string.Empty;
            string fullAddress = string.Join(", ", new[] { street, ward, district, city }.Where(s => !string.IsNullOrWhiteSpace(s)));

            TxtCustomerName.Text = $"Khách hàng: {name}";
            TxtCustomerPhone.Text = $"Điện thoại: {phone}";
            TxtPaymentMethod.Text = $"Phương thức: {_order.PaymentMethod}";
            TxtCustomerAddress.Text = $"Địa chỉ: {fullAddress}";
            TxtOrderNote.Text = string.IsNullOrWhiteSpace(_order.Note) ? "Ghi chú: (Không có)" : $"Ghi chú: {_order.Note}";

            // Huy hiệu trạng thái
            UpdateStatusBadge(_order.Status);

            // Chọn combobox trạng thái tương ứng
            int statusIndex = (int)_order.Status;
            if (statusIndex >= 0 && statusIndex < CboStatus.Items.Count)
            {
                CboStatus.SelectedIndex = statusIndex;
            }

            // Danh sách món hàng
            var itemsDisplay = _order.OrderItems.Select(i => new OrderItemDetailDisplay
            {
                CatalogItemId = i.CatalogItemId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Units = i.Units
            }).ToList();

            DgOrderItems.ItemsSource = itemsDisplay;
            TxtTotalAmount.Text = _order.TotalPrice.ToString("N0") + " ₫";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi tải thông tin đơn hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdateStatusBadge(OrderStatus status)
    {
        TxtCurrentStatusBadge.Text = status switch
        {
            OrderStatus.Pending => "Chờ xác nhận (Pending)",
            OrderStatus.Confirmed => "Đã xác nhận (Confirmed)",
            OrderStatus.Shipping => "Đang giao hàng (Shipping)",
            OrderStatus.Completed => "Hoàn thành (Completed)",
            OrderStatus.Cancelled => "Đã hủy (Cancelled)",
            _ => status.ToString()
        };
    }

    private async void BtnSaveStatus_Click(object sender, RoutedEventArgs e)
    {
        if (CboStatus.SelectedItem is not ComboBoxItem selectedItem || 
            !int.TryParse(selectedItem.Tag?.ToString(), out int statusCode))
        {
            return;
        }

        try
        {
            using var scope = App.ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CatalogContext>();

            var orderToUpdate = await context.Orders.FindAsync(_orderId);
            if (orderToUpdate == null)
            {
                MessageBox.Show("Đơn hàng không tồn tại!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newStatus = (OrderStatus)statusCode;
            orderToUpdate.Status = newStatus;
            await context.SaveChangesAsync();

            UpdateStatusBadge(newStatus);
            MessageBox.Show($"Đã cập nhật trạng thái đơn #{_orderId} sang [{TxtCurrentStatusBadge.Text}]!", 
                            "Cập nhật thành công", MessageBoxButton.OK, MessageBoxImage.Information);

            DialogResult = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Không thể cập nhật trạng thái đơn hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}

public class OrderItemDetailDisplay
{
    public int CatalogItemId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Units { get; set; }
    public decimal Subtotal => UnitPrice * Units;
}

