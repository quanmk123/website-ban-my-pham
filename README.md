# HỆ THỐNG QUẢN LÝ BÁN LẺ & THƯƠNG MẠI ĐIỆN TỬ MỸ PHẨM (BEAUTYSTORE)
> **Mô hình O2O (Online-to-Offline):** Kết hợp nền tảng Website Thương mại điện tử (E-Commerce) và Ứng dụng Desktop Bán lẻ & Quản trị tại quầy (WPF POS Management).

---

## 1. TỔNG QUAN HỆ THỐNG & CÔNG NGHỆ (TECH STACK)

* **Nền tảng phát triển:** C# / .NET (.NET 8 & .NET 10).
* **Phân hệ Website (Online):** ASP.NET Core MVC kết hợp Razor Pages, Bootstrap 5.3, FontAwesome 6, Google Fonts (Inter) với tông màu Rose Pastel sang trọng chuẩn ngành mỹ phẩm.
* **Phân hệ Desktop (Offline / Tại quầy):** WPF (Windows Presentation Foundation) trên nền **.NET 10**, thiết kế giao diện Dark/Light Sidebar hiện đại, responsive controls, tối ưu cho màn hình thu ngân và quản lý.
* **ORM & Truy vấn dữ liệu:** Entity Framework Core (Code-First Approach, tự động migration và seeding dữ liệu mẫu phong phú).
* **Cơ sở dữ liệu (Database):** Microsoft SQL Server LocalDB (`(localdb)\mssqllocaldb`) gồm 2 CSDL riêng biệt:
  - `CosmeticsShopDb`: Quản lý danh mục sản phẩm, thương hiệu, loại da, giỏ hàng, đơn hàng, khách hàng.
  - `CosmeticsShopIdentityDb`: Quản lý tài khoản, mật khẩu băm, vai trò (Roles), Claims và phân quyền đăng nhập.
* **Xác thực & Bảo mật:** ASP.NET Core Identity (Role-based & Claims-based Authorization).
* **Thư viện xuất báo cáo:** ClosedXML (Xuất dữ liệu đơn hàng và doanh thu sang định dạng Excel `.xlsx`).
* **Kiểm thử tự động:** Bộ kiểm thử đơn vị xUnit kết hợp Moq với tỷ lệ vượt qua 100%.

---

## 2. KIẾN TRÚC DỰ ÁN (CLEAN ARCHITECTURE)

Hệ thống được thiết kế theo mô hình **Clean Architecture / N-Tier**, tách biệt rõ ràng giữa Core nghiệp vụ, Hạ tầng và Trình diễn:

```text
website-ban-my-pham/
├── Cosmetics.ApplicationCore/      # LÕI NGHIỆP VỤ (Domain Entities, Value Objects, Interfaces)
│   ├── Entities/                   # CatalogItem, CatalogBrand, CatalogType, Basket
│   │   └── OrderAggregate/         # Order, OrderItem, Address (DDD Aggregate Root & Value Object)
│   ├── Interfaces/                 # IRepository<T>, ISpecification<T>, IBasketService, IOrderService
│   └── Specifications/             # CatalogFilterPaginatedSpecification, OrderWithItemsSpecification...
│
├── Cosmetics.Infrastructure/       # HẠ TẦNG DỮ LIỆU & IDENTITY
│   ├── Data/                       # CatalogContext, EfRepository<T>, CatalogContextSeed
│   ├── Identity/                   # AppIdentityDbContext, AppIdentityDbContextSeed
│   └── Services/                   # BasketService, OrderService
│
├── Cosmetics.Web/                  # TẦNG TRÌNH DIỄN WEBSITE (ASP.NET Core MVC E-Commerce)
│   ├── Areas/Admin/                # Khu vực quản trị Web (Dashboard, Sản phẩm, Đơn hàng)
│   ├── Controllers/                # CatalogController, BasketController, OrderController, AccountController
│   ├── ViewModels/                 # Data Transfer Objects & ViewModel hiển thị
│   ├── Views/                      # Giao diện mua sắm khách hàng, Giỏ hàng, Thanh toán
│   └── wwwroot/                    # CSS Rose Pastel, JavaScript, hình ảnh mỹ phẩm chính hãng
│
├── Cosmetics.WpfApp/               # TẦNG TRÌNH DIỄN DESKTOP (WPF Desktop .NET 10 - Quản lý & POS)
│   ├── Views / Windows:            # LoginWindow, MainWindow, AddProductWindow, EditProductWindow, OrderDetailWindow
│   ├── Phân hệ POS:                # Bán hàng tại quầy nhanh, tự động tính tiền, in hóa đơn
│   ├── Phân hệ Quản trị:           # Dashboard doanh thu, Quản lý kho, Quản lý đơn hàng, Danh bạ khách hàng
│   └── Phân hệ Báo cáo:            # Xuất báo cáo đơn hàng định dạng Excel chuyên nghiệp
│
└── Cosmetics.UnitTests/            # BỘ KIỂM THỬ ĐƠN VỊ (xUnit + Moq)
    ├── ApplicationCore/            # Kiểm thử Entities, Value Object, Specifications
    └── Infrastructure/             # Kiểm thử Services, Mock Repositories
```

---

## 3. CÁC TÁC NHÂN NGHIỆP VỤ (ACTORS) & PHÂN QUYỀN

Hệ thống thiết kế chuẩn hóa cho **3 Tác nhân nghiệp vụ quản lý nội bộ** trên Desktop và **1 Tác nhân khách hàng** trên Website:

| Tác nhân (Actor) | Nền tảng | Quyền hạn & Chức năng chính |
| :--- | :---: | :--- |
| **👑 Chủ cửa hàng (Admin)** | WPF Desktop / Web | Toàn quyền cao nhất: Xem Dashboard thống kê doanh thu biểu đồ, tổng hợp đơn hàng toàn hệ thống, quản lý danh bạ khách hàng VIP/Thân thiết, duyệt trạng thái đơn, xuất báo cáo Excel doanh thu. |
| **📦 Quản lý kho (Warehouse Manager)** | WPF Desktop | Quản lý kho sản phẩm mỹ phẩm: Thêm mới, chỉnh sửa thông tin, giá bán, số lượng tồn kho; theo dõi cảnh báo hết hàng; cập nhật trạng thái đơn (Đang giao hàng / Đã giao). |
| **💳 Thu ngân bán lẻ (Cashier POS)** | WPF Desktop | Vận hành quầy thu ngân (POS): Tra cứu mỹ phẩm, tạo giỏ hàng bán lẻ trực tiếp tại cửa hàng, tính tiền, tự động trừ tồn kho, tra cứu đơn hàng và xem danh bạ khách hàng. |
| **🛍️ Khách mua hàng (Customer)** | Website MVC | Đăng ký/đăng nhập tài khoản, xem danh mục mỹ phẩm, lọc theo loại da/thương hiệu, thêm vào giỏ hàng cá nhân, tiến hành đặt hàng trực tuyến (COD / VNPay) và xem lịch sử đơn hàng cá nhân. |

---

## 4. CÁC TÍNH NĂNG NỔI BẬT ĐÃ HOÀN THIỆN

### A. Ứng dụng Desktop Bán lẻ & Quản trị (Cosmetics.WpfApp - .NET 10)
1. **Màn hình Đăng nhập phân quyền (`LoginWindow`):**
   - Hỗ trợ đăng nhập bằng tài khoản ASP.NET Core Identity.
   - Nút chọn nhanh 1-Click cho 3 vai trò: **Admin**, **Quản lý kho**, **Thu ngân**.
2. **Bán hàng tại quầy POS (`Point of Sale`):**
   - Tìm kiếm mỹ phẩm theo tên/danh mục, hiển thị ảnh và tồn kho tức thì.
   - Thêm vào giỏ hàng quầy, tùy chỉnh số lượng, nhập thông tin khách mua.
   - Tự động tính tổng tiền, thanh toán tạo đơn trực tiếp và tự động giảm số lượng hàng tồn.
3. **Quản lý kho mỹ phẩm (`Catalog Management`):**
   - Bảng danh mục sản phẩm chi tiết kèm ảnh thumbnail, giá bán, tồn kho.
   - Hộp thoại Thêm mới (`AddProductWindow`) & Sửa sản phẩm (`EditProductWindow`) đầy đủ thuộc tính mỹ phẩm: loại da, dung tích, thành phần, hạn sử dụng.
   - Xóa sản phẩm an toàn với hộp thoại xác nhận.
4. **Quản lý đơn hàng (`Order Management`):**
   - Danh sách toàn bộ đơn hàng (cả đơn đặt online từ web và đơn bán tại quầy POS).
   - Bộ lọc trạng thái đơn: Chờ xác nhận, Đang giao, Hoàn thành, Đã hủy.
   - Các nút thao tác nhanh: *Chi tiết & Duyệt đơn*, *Giao Hàng*, *Hoàn Thành*, *Hủy Đơn*.
   - Cửa sổ xem chi tiết đơn hàng (`OrderDetailWindow`) hiển thị danh sách sản phẩm, số lượng, đơn giá và địa chỉ nhận hàng.
5. **Danh bạ khách hàng & Phân hạng thành viên (`Customer CRM`):**
   - Tự động tổng hợp thông tin khách hàng từ các đơn đặt hàng.
   - Phân hạng khách hàng thông minh theo tổng chi tiêu:
     - 🌟 **VIP Kim Cương** (Tổng chi tiêu $\ge$ 1.500.000 ₫)
     - 💎 **Khách Thân Thiết** (Tổng chi tiêu $\ge$ 500.000 ₫)
     - 🌸 **Khách Mới**
   - Bộ lọc tìm kiếm nhanh theo Tên, Số điện thoại hoặc Địa chỉ.
6. **Báo cáo & Xuất Excel (`ClosedXML`):**
   - Xuất dữ liệu đơn hàng và doanh số sang file Excel `.xlsx` chuyên nghiệp chỉ với 1 cú click.

### B. Website Thương mại điện tử (Cosmetics.Web - ASP.NET Core MVC)
1. **Trang chủ & Danh mục sản phẩm (`/Catalog`):**
   - Banner Hero quảng bá thương hiệu mỹ phẩm chính hãng (La Roche-Posay, Innisfree, CeraVe, MAC, Anessa, Klairs...).
   - Bộ lọc đa tiêu chí: Thương hiệu, Danh mục (Kem chống nắng, Serum, Son môi...), Loại da (Da dầu mụn, da khô, da nhạy cảm...), Khoảng giá.
   - Tìm kiếm từ khóa theo thời gian thực kết hợp phân trang khoa học.
2. **Chi tiết sản phẩm (`/Catalog/Details/{id}`):**
   - Ảnh lớn, thành phần, công dụng, loại da phù hợp, dung tích, hạn sử dụng.
   - Thêm vào giỏ hàng với số lượng tùy chọn.
3. **Giỏ hàng & Đặt hàng (`/Basket` & `/Order/Checkout`):**
   - Hỗ trợ giỏ hàng cho khách vãng lai và tự động đồng bộ khi đăng nhập.
   - Form thanh toán nhận hàng chuẩn chỉ: Họ tên, Số điện thoại, Phường/Xã, Quận/Huyện, Tỉnh/TP.
   - Phương thức thanh toán linh hoạt: COD, Chuyển khoản ngân hàng, Cổng thanh toán VNPay.
4. **Lịch sử đơn hàng cá nhân (`/Order/MyOrders`):**
   - Khách hàng tự theo dõi tình trạng xử lý đơn hàng của mình.

---

## 5. TÀI KHOẢN TRẢI NGHIỆM HỆ THỐNG (TEST ACCOUNTS)

Hệ thống đã tự động Seed sẵn các tài khoản tương ứng với từng tác nhân nghiệp vụ:

| Tác nhân | Email đăng nhập | Mật khẩu | Quyền hạn mặc định |
| :--- | :--- | :--- | :--- |
| **Chủ cửa hàng (Admin)** | `admin@beauty.com` | `P@ssword123` | Toàn quyền Dashboard, POS, Kho, Đơn hàng, Khách hàng, Excel |
| **Quản lý kho (Warehouse)** | `warehouse@beauty.com` | `P@ssword123` | Quyền Quản lý kho sản phẩm, Tồn kho, Cập nhật trạng thái đơn |
| **Thu ngân bán lẻ (Cashier)** | `cashier@beauty.com` | `P@ssword123` | Quyền Bán hàng POS tại quầy, Quản lý đơn, Tra cứu khách hàng |
| **Khách mua hàng (Customer)** | `customer@beauty.com` | `P@ssword123` | Đăng nhập mua hàng trên Website, xem lịch sử đơn |

---

## 6. HƯỚNG DẪN CÀI ĐẶT & CHẠY DỰ ÁN

### Yêu cầu hệ thống:
* **Hệ điều hành:** Windows 10/11 (để chạy WPF Desktop và Web song song).
* **SDK:** .NET SDK 8.0 trở lên (khuyên dùng .NET 10 SDK).
* **Công cụ:** Visual Studio 2022 / 2026 (với các workload *.NET desktop development* và *ASP.NET and web development*) hoặc Antigravity IDE / VS Code.
* **CSDL:** SQL Server LocalDB `(localdb)\mssqllocaldb` (tự động đi kèm Visual Studio).

### Các bước khởi chạy:

#### 1. Chạy bộ kiểm thử tự động (Unit Tests):
```bash
dotnet test
```

#### 2. Chạy Website Thương mại điện tử:
```bash
dotnet run --project Cosmetics.Web
```
*Trình duyệt truy cập:* `https://localhost:5001` (hoặc cổng HTTP hiển thị trên màn hình console).

#### 3. Chạy Ứng dụng Desktop Bán lẻ & Quản trị (WPF):
* Mở solution trong **Visual Studio**, chuột phải vào dự án **`Cosmetics.WpfApp`** $\rightarrow$ chọn **Set as Startup Project** $\rightarrow$ nhấn **F5** (hoặc nút Run).
* Hoặc chạy qua lệnh:
```bash
dotnet run --project Cosmetics.WpfApp
```
*Tại màn hình Đăng nhập, bấm nút 1-click **Admin**, **Quản lý kho** hoặc **Thu ngân** để bắt đầu trải nghiệm ngay!*
