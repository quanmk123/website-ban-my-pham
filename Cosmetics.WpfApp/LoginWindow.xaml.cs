using System;
using System.Windows;
using System.Windows.Controls;

namespace Cosmetics.WpfApp;

public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        Loaded += LoginWindow_Loaded;
    }

    private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
    {
        TxtPassword.Password = "P@ssword123";
    }

    private void BtnQuickRole_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is string role)
        {
            switch (role)
            {
                case "Admin":
                    TxtEmail.Text = "admin@beauty.com";
                    TxtPassword.Password = "P@ssword123";
                    CboRole.SelectedIndex = 0;
                    break;
                case "Manager":
                    TxtEmail.Text = "manager@beauty.com";
                    TxtPassword.Password = "P@ssword123";
                    CboRole.SelectedIndex = 1;
                    break;
                case "Cashier":
                    TxtEmail.Text = "cashier@beauty.com";
                    TxtPassword.Password = "P@ssword123";
                    CboRole.SelectedIndex = 2;
                    break;
            }
        }
    }

    private void BtnLogin_Click(object sender, RoutedEventArgs e)
    {
        string email = TxtEmail.Text.Trim();
        string password = TxtPassword.Password.Trim();

        if (string.IsNullOrWhiteSpace(email))
        {
            MessageBox.Show("Vui lòng nhập tên đăng nhập hoặc Email!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtEmail.Focus();
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            MessageBox.Show("Vui lòng nhập mật khẩu đăng nhập!", "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
            TxtPassword.Focus();
            return;
        }

        string role = "Admin";
        string roleDisplayName = "Chủ cửa hàng (Admin)";

        if (CboRole.SelectedItem is ComboBoxItem selectedItem && selectedItem.Tag is string tag)
        {
            role = tag;
            roleDisplayName = role switch
            {
                "Admin" => "Chủ cửa hàng (Admin)",
                "Manager" => "Quản lý kho (Manager)",
                "Cashier" => "Thu ngân bán lẻ (Cashier)",
                _ => "Người dùng"
            };
        }

        // Mở MainWindow truyền thông tin phân quyền
        var mainWindow = new MainWindow(role, roleDisplayName, email);
        mainWindow.Show();

        // Đóng cửa sổ Login
        this.Close();
    }
}
