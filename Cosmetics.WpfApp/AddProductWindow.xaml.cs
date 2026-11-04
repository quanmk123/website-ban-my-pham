using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Cosmetics.ApplicationCore.Entities;
using Cosmetics.Infrastructure.Data;

namespace Cosmetics.WpfApp;

public partial class AddProductWindow : Window
{
    public AddProductWindow()
    {
        InitializeComponent();
        Loaded += AddProductWindow_Loaded;
    }

    private async void AddProductWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            using var scope = App.ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CatalogContext>();

            var brands = await context.CatalogBrands.OrderBy(b => b.Name).ToListAsync();
            var types = await context.CatalogTypes.OrderBy(t => t.Name).ToListAsync();

            CboBrand.ItemsSource = brands;
            if (brands.Any()) CboBrand.SelectedIndex = 0;

            CboType.ItemsSource = types;
            if (types.Any()) CboType.SelectedIndex = 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi nạp danh mục thương hiệu/loại: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void BtnSave_Click(object sender, RoutedEventArgs e)
    {
        string name = TxtName.Text.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show("Vui lòng nhập tên sản phẩm mỹ phẩm!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtName.Focus();
            return;
        }

        if (CboBrand.SelectedValue is not int brandId)
        {
            MessageBox.Show("Vui lòng chọn thương hiệu cho sản phẩm!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            CboBrand.Focus();
            return;
        }

        if (CboType.SelectedValue is not int typeId)
        {
            MessageBox.Show("Vui lòng chọn danh mục loại sản phẩm!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            CboType.Focus();
            return;
        }

        if (!decimal.TryParse(TxtPrice.Text.Trim(), out decimal price) || price < 0)
        {
            MessageBox.Show("Vui lòng nhập đơn giá bán hợp lệ (số không âm)!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtPrice.Focus();
            return;
        }

        if (!decimal.TryParse(TxtOriginalPrice.Text.Trim(), out decimal originalPrice) || originalPrice < 0)
        {
            originalPrice = price;
        }

        if (!int.TryParse(TxtStockQuantity.Text.Trim(), out int stock) || stock < 0)
        {
            MessageBox.Show("Vui lòng nhập số lượng tồn kho hợp lệ (số nguyên >= 0)!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtStockQuantity.Focus();
            return;
        }

        string capacity = TxtCapacity.Text.Trim();
        string skinType = (CboSkinType.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Mọi loại da";
        string description = TxtDescription.Text.Trim();

        try
        {
            using var scope = App.ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CatalogContext>();

            var newItem = new CatalogItem
            {
                Name = name,
                CatalogBrandId = brandId,
                CatalogTypeId = typeId,
                Price = price,
                OriginalPrice = originalPrice > 0 ? originalPrice : price,
                StockQuantity = stock,
                Capacity = string.IsNullOrWhiteSpace(capacity) ? "50ml" : capacity,
                SkinType = skinType,
                Description = description,
                PictureUri = "/images/products/placeholder.png"
            };

            context.CatalogItems.Add(newItem);
            await context.SaveChangesAsync();

            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Không thể lưu sản phẩm vào CSDL: {ex.Message}", "Lỗi CSDL", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}

