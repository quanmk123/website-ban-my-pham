# ĐỒ ÁN TỐT NGHIỆP / ĐỒ ÁN MÔN HỌC
# WEBSITE THƯƠNG MẠI ĐIỆN TỬ BÁN MỸ PHẨM (BEAUTY COSMETICS STORE)

> **Kiến trúc tham chiếu:** Microsoft eShopOnWeb Reference Architecture  
> **Phong cách kiến trúc:** Clean Architecture (Onion Architecture), Domain-Driven Design (DDD) Lite, Repository Pattern, Specification Pattern.

---

## 1. TỔNG QUAN HỆ THỐNG & CÔNG NGHỆ (TECH STACK)
* **Nền tảng:** .NET / C# (ASP.NET Core MVC kết hợp Razor).
* **ORM:** Entity Framework Core (Code-First Approach, tự động khởi tạo và seed dữ liệu mẫu).
* **Cơ sở dữ liệu:** Microsoft SQL Server (LocalDB `(localdb)\mssqllocaldb` hoặc SQL Server Express).
* **Xác thực & Phân quyền:** ASP.NET Core Identity (Role-based Authorization: `Admin`, `Customer`).
* **Frontend & Giao diện:** Vanilla CSS kết hợp Bootstrap 5.3, FontAwesome 6, Google Fonts (Inter). Tông màu Rose / Pastel sang trọng chuẩn ngành mỹ phẩm, Responsive đa thiết bị.
* **Testing:** xUnit kết hợp Moq với bộ kiểm thử tự động đạt 100% tỷ lệ vượt qua.

---

## 2. KIẾN TRÚC DỰ ÁN (CLEAN ARCHITECTURE)

Giải pháp phân rã theo 4 dự án độc lập, tuân thủ nguyên tắc Dependency Inversion:

```
website-ban-my-pham/
├── Cosmetics.ApplicationCore/      # LÕI NGHIỆP VỤ (Không phụ thuộc tầng ngoài)
│   ├── Entities/                   # CatalogItem, CatalogBrand, CatalogType, Basket
│   │   └── OrderAggregate/         # Order, OrderItem, Address (DDD Aggregate Root & Value Object)
│   ├── Interfaces/                 # IRepository<T>, ISpecification<T>, IBasketService, IOrderService
│   └── Specifications/             # CatalogFilterPaginatedSpecification, OrderWithItemsSpecification...
│
├── Cosmetics.Infrastructure/       # HẠ TẦNG DỮ LIỆU & DỊCH VỤ NGOÀI
│   ├── Data/                       # CatalogContext, EfRepository<T>, SpecificationEvaluator, CatalogContextSeed
│   ├── Identity/                   # AppIdentityDbContext, AppIdentityDbContextSeed
│   └── Services/                   # BasketService, OrderService
│
├── Cosmetics.Web/                  # TẦNG TRÌNH DIỄN (Presentation Layer)
│   ├── Areas/Admin/                # Phân hệ Quản trị viên (Dashboard, Quản lý SP, Thương hiệu, Loại, Đơn)
│   ├── Controllers/                # CatalogController, BasketController, OrderController, AccountController
│   ├── ViewModels/                 # Data transfer objects cho View
│   ├── Views/                      # Giao diện khách hàng và giỏ hàng / thanh toán
│   └── wwwroot/                    # CSS pastel, JS, hình ảnh mỹ phẩm chất lượng cao
│
└── Cosmetics.UnitTests/            # KIỂM THỬ ĐƠN VỊ (xUnit + Moq)
    ├── ApplicationCore/            # Kiểm thử Entities, Value Object, Specification
    └── Infrastructure/             # Kiểm thử Services, Mock Repositories
```

---

## 3. CÁC TÍNH NĂNG CHÍNH ĐÃ HOÀN THIỆN

### Phân hệ Khách hàng (Customer Storefront):
1. **Trang chủ & Danh mục sản phẩm (`/Catalog`):**
   - Banner Hero quảng bá thương hiệu mỹ phẩm chính hãng.
   - Bộ lọc đa tiêu chí: Lọc theo Thương hiệu (La Roche-Posay, Innisfree, CeraVe, MAC, Anessa, Klairs...), Danh mục (Kem chống nắng, Serum, Son môi...), Loại da (Da dầu mụn, da khô, da nhạy cảm...), Khoảng giá VNĐ.
   - Ô tìm kiếm từ khóa mỹ phẩm theo thời gian thực.
   - Thẻ sản phẩm chuẩn e-commerce: Ảnh minh họa, nhãn giảm giá (%), giá gốc/giá khuyến mãi, dung tích, badge loại da, nút thêm giỏ hàng nhanh.
   - Phân trang khoa học (Pagination).

2. **Trang chi tiết sản phẩm (`/Catalog/Details/{id}`):**
   - Ảnh sản phẩm lớn, tên thương hiệu, xuất xứ.
   - Bảng thông số chi tiết: Thành phần chính, Loại da phù hợp, Dung tích, Hạn sử dụng.
   - Bộ chọn số lượng và nút "Thêm vào giỏ hàng".
   - Cảnh báo tình trạng kho hàng thực tế.

3. **Giỏ hàng thông minh (`/Basket`):**
   - Hỗ trợ giỏ hàng ẩn danh (Session/Cookie) cho khách chưa đăng nhập.
   - Tự động gộp (Transfer) giỏ hàng vào tài khoản ngay khi người dùng đăng nhập hoặc đăng ký.
   - Tăng/giảm số lượng, xóa sản phẩm, tính tổng tiền tự động.
   - Badge hiển thị số lượng sản phẩm trên Header cập nhật qua Ajax.

4. **Thanh toán & Đặt hàng (`/Order/Checkout`):**
   - Form nhập địa chỉ nhận hàng chi tiết: Người nhận, Số điện thoại, Địa chỉ, Phường/Xã, Quận/Huyện, Tỉnh/TP.
   - Lựa chọn phương thức thanh toán: COD (Tiền mặt), Chuyển khoản, VNPay.
   - Tự động kiểm tra và trừ số lượng tồn kho (Inventory Deduct).
   - Trang thông báo đặt hàng thành công (`/Order/Success`).

5. **Lịch sử đơn hàng cá nhân (`/Order/MyOrders`):**
   - Danh sách các đơn đã đặt kèm trạng thái (Chờ xác nhận, Đã xác nhận, Đang giao, Hoàn thành, Đã hủy).
   - Xem chi tiết từng đơn hàng (`/Order/Details/{id}`).

6. **Tài khoản & Xác thực (`/Account`):**
   - Đăng nhập, Đăng ký thành viên, Đăng xuất bảo mật với ASP.NET Core Identity.
   - Trang báo lỗi truy cập `AccessDenied`.

---

### Phân hệ Quản trị viên (Admin Area - `/Admin`):
1. **Bảng điều khiển (Dashboard - `/Admin/Dashboard`):**
   - Thống kê chỉ số kinh doanh: Tổng doanh thu hoàn thành, Tổng số đơn hàng, Số đơn chờ xác nhận, Tổng mặt hàng.
   - Bảng cảnh báo sản phẩm sắp hết hàng (Tồn kho < 5) kèm lối tắt nhập thêm hàng.
   - Lối tắt nhanh tới các tác vụ quản trị.

2. **Quản lý sản phẩm (`/Admin/Dashboard/Products`):**
   - Danh sách mỹ phẩm kèm thumbnail, tồn kho (badge cảnh báo màu sắc), đơn giá.
   - Tìm kiếm và phân trang sản phẩm.
   - Thêm sản phẩm mới (`CreateProduct`) hỗ trợ upload ảnh hoặc URL ảnh, đầy đủ thông tin chuyên ngành mỹ phẩm (loại da, thành phần, hạn sử dụng, dung tích).
   - Chỉnh sửa thông tin (`EditProduct`) và Xóa sản phẩm có hộp thoại xác nhận (Modal).

3. **Quản lý thương hiệu & Danh mục:**
   - Quản lý danh sách các thương hiệu mỹ phẩm (`/Admin/Dashboard/Brands`) & Thêm mới (`CreateBrand`).
   - Quản lý danh sách danh mục phân loại (`/Admin/Dashboard/Types`) & Thêm mới (`CreateType`).

4. **Quản lý đơn hàng (`/Admin/Dashboard/Orders`):**
   - Lọc đơn hàng theo trạng thái: Tất cả, Chờ xác nhận, Đã xác nhận, Đang giao hàng, Hoàn thành, Đã hủy.
   - Xem chi tiết đơn hàng (`OrderDetail`): Danh sách sản phẩm, số lượng, giá, thông tin người nhận.
   - Cập nhật trạng thái đơn hàng (Dropdown chuyển đổi trạng thái và lưu ngay vào CSDL).

---

## 4. TÀI KHOẢN ĐĂNG NHẬP MẪU (TEST ACCOUNTS)

Hệ thống đã tự động Seed sẵn các tài khoản và phân quyền mặc định:

| Vai trò | Email đăng nhập | Mật khẩu | Quyền hạn |
| :--- | :--- | :--- | :--- |
| **Quản trị viên (Admin)** | `admin@beauty.com` | `P@ssword123` | Toàn quyền Dashboard & Quản trị `/Admin` |
| **Khách hàng (Customer)** | `customer@beauty.com` | `P@ssword123` | Mua sắm, Đặt hàng, Xem lịch sử đơn |

---

## 5. HƯỚNG DẪN CÀI ĐẶT & CHẠY DỰ ÁN

### Yêu cầu môi trường:
* .NET SDK 8.0 trở lên (khuyên dùng .NET 8 / .NET 10).
* SQL Server LocalDB (đi kèm sẵn trong Visual Studio) hoặc SQL Server Express.

### Lệnh chạy:
```bash
# 1. Chạy toàn bộ Unit Tests:
dotnet test

# 2. Khởi chạy ứng dụng Web:
dotnet run --project Cosmetics.Web
```

Mở trình duyệt truy cập:
* **Cửa hàng:** `https://localhost:5001` (hoặc cổng HTTP do console hiển thị)
* **Khu vực Quản trị:** `https://localhost:5001/Admin`
