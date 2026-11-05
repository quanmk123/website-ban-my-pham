using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Cosmetics.ApplicationCore.Entities;
using Cosmetics.Infrastructure.Data;

namespace Cosmetics.WpfApp;

public partial class EditProductWindow : Window
{
    private readonly int _productId;
    private CatalogItem? _product;

    public EditProductWindow(int productId)
    {
        InitializeComponent();
        _productId = productId;
        Loaded += EditProductWindow_Loaded;
    }

    private async void EditProductWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            using var scope = App.ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CatalogContext>();

            // Nạp thương hiệu & loại
            var brands = await context.CatalogBrands.OrderBy(b => b.Name).ToListAsync();
            var types = await context.CatalogTypes.OrderBy(t => t.Name).ToListAsync();

            CboBrand.ItemsSource = brands;
            CboType.ItemsSource = types;

            // Nạp thông tin sản phẩm cần sửa
            _product = await context.CatalogItems.FindAsync(_productId);
            if (_product == null)
            {
                MessageBox.Show("Không tìm thấy sản phẩm trong CSDL!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                Close();
                return;
            }

            TxtSubtitle.Text = $"Mã ID: #{_product.Id} | Tên gốc: {_product.Name}";
            TxtName.Text = _product.Name;
            CboBrand.SelectedValue = _product.CatalogBrandId;
            CboType.SelectedValue = _product.CatalogTypeId;
            TxtPrice.Text = _product.Price.ToString("F0");
            TxtOriginalPrice.Text = _product.OriginalPrice.ToString("F0");
            TxtStockQuantity.Text = _product.StockQuantity.ToString();
            TxtCapacity.Text = _product.Capacity ?? string.Empty;
            TxtDescription.Text = _product.Description ?? string.Empty;

            // Chọn SkinType tương ứng
            foreach (ComboBoxItem item in CboSkinType.Items)
            {
                if (item.Content?.ToString() == _product.SkinType)
                {
                    CboSkinType.SelectedItem = item;
                    break;
                }
            }
            if (CboSkinType.SelectedItem == null)
            {
                CboSkinType.SelectedIndex = 0;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Lỗi nạp dữ liệu sản phẩm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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

            var itemToUpdate = await context.CatalogItems.FindAsync(_productId);
            if (itemToUpdate == null)
            {
                MessageBox.Show("Sản phẩm không còn tồn tại trong CSDL!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            itemToUpdate.Name = name;
            itemToUpdate.CatalogBrandId = brandId;
            itemToUpdate.CatalogTypeId = typeId;
            itemToUpdate.Price = price;
            itemToUpdate.OriginalPrice = originalPrice;
            itemToUpdate.StockQuantity = stock;
            itemToUpdate.Capacity = capacity;
            itemToUpdate.SkinType = skinType;
            itemToUpdate.Description = description;

            await context.SaveChangesAsync();

            DialogResult = true;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Không thể cập nhật sản phẩm: {ex.Message}", "Lỗi CSDL", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void BtnCancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}

