# SPATC PET SHOP - BÁO CÁO BÀI TẬP LỚN
## Hệ thống Quản lý Cửa hàng Thú cưng

---

## 1. GIỚI THIỆU ĐỀ TÀI

### 1.1 Mô tả
Hệ thống **SPATC Pet Shop** là ứng dụng web quản lý cửa hàng thú cưng, cho phép:
- Quản lý sản phẩm, dịch vụ
- Đặt lịch chăm sóc thú cưng
- Mua sắm trực tuyến
- Chat realtime với nhân viên
- Quản lý tài khoản người dùng

### 1.2 Công nghệ sử dụng

| Thành phần | Công nghệ | Phiên bản |
|------------|-----------|-----------|
| Backend | Node.js + Express.js | 4.21.0 |
| Database | MongoDB + Mongoose | 8.8.0 |
| Frontend | HTML5 + CSS3 + JavaScript | - |
| Authentication | JWT + bcryptjs | - |
| Real-time | Socket.IO | 4.8.1 |
| Security | Helmet, CORS, Rate Limit | - |

---

## 2. YÊU CẦU ĐẠT ĐƯỢC

### ✅ 2.1 RESTful API (MVC Pattern)

**Cấu trúc thư mục:**
```
api/
├── app.js                      # Express app entry
├── bin/www                     # Server entry
├── controllers/                # Business logic
│   ├── auth.controller.js
│   ├── account.controller.js
│   ├── booking.controller.js
│   ├── cart.controller.js
│   ├── chat.controller.js
│   ├── hotel.controller.js
│   ├── notification.controller.js
│   ├── order.controller.js
│   ├── pet.controller.js
│   ├── product.controller.js
│   ├── review.controller.js
│   ├── service.controller.js
│   ├── upload.controller.js
│   └── voucher.controller.js
├── routes/                     # API routes
│   ├── auth.routes.js
│   ├── account.routes.js
│   ├── booking.routes.js
│   ├── cart.routes.js
│   ├── chat.routes.js
│   ├── grooming.routes.js
│   ├── hotel.routes.js
│   ├── notification.routes.js
│   ├── pet.routes.js
│   ├── product.routes.js
│   ├── review.routes.js
│   ├── service.routes.js
│   ├── shopOrder.routes.js
│   ├── upload.routes.js
│   └── voucher.routes.js
├── schemas/                    # Database models
│   ├── TaiKhoan.js
│   ├── KhachHang.js
│   ├── ThuCung.js
│   ├── SanPham.js
│   ├── DichVu.js
│   ├── DatLich.js
│   ├── ShopOrder.js
│   ├── Voucher.js
│   ├── DanhGia.js
│   └── PhongKhachSan.js
├── middleware/                 # Middleware
│   ├── errorHandler.js
│   └── rateLimit.js
└── utils/                      # Utilities
    ├── authHandler.js
    ├── database.js
    ├── idHandler.js
    ├── uploadHandler.js
    └── validator.js
```

### ✅ 2.2 CRUD Operations

| Module | Create | Read | Update | Delete |
|--------|--------|------|--------|--------|
| Products | POST /api/products | GET /api/products | PUT /api/products/:id | DELETE /api/products/:id |
| Orders | POST /api/shop-orders | GET /api/shop-orders | PUT /api/shop-orders/:id | DELETE /api/shop-orders/:id |
| Bookings | POST /api/bookings | GET /api/bookings | PUT /api/bookings/:id | DELETE /api/bookings/:id |
| Pets | POST /api/pets | GET /api/pets | PUT /api/pets/:id | DELETE /api/pets/:id |
| Services | POST /api/services | GET /api/services | PUT /api/services/:id | DELETE /api/services/:id |
| Vouchers | POST /api/vouchers | GET /api/vouchers | PUT /api/vouchers/:id | DELETE /api/vouchers/:id |
| Reviews | POST /api/reviews | GET /api/reviews | PUT /api/reviews/:id | DELETE /api/reviews/:id |

### ✅ 2.3 Authentication (JWT)

**Các endpoint:**
- `POST /api/auth/register` - Đăng ký tài khoản mới
- `POST /api/auth/login` - Đăng nhập
- `POST /api/auth/refresh` - Làm mới token
- `POST /api/auth/logout` - Đăng xuất
- `GET /api/auth/me` - Lấy thông tin user hiện tại

**Cơ chế:**
- Access Token: expire sau 7 ngày
- Refresh Token: expire sau 30 ngày
- Password: mã hóa bcrypt (10 rounds)
- Token pair được tạo khi đăng nhập thành công

### ✅ 2.4 Authorization (Role-based)

**Phân quyền theo vai trò:**

| Role | Quyền |
|------|-------|
| user | Xem sản phẩm, đặt lịch, tạo đơn hàng, chat |
| staff | Tất cả quyền user + quản lý đơn hàng, xác nhận booking |
| admin | Toàn quyền quản lý hệ thống |

**Middleware:**
```javascript
const authorizeRole = (...roles) => {
  return (req, res, next) => {
    if (!roles.includes(req.user.role)) {
      return res.status(403).json({
        success: false,
        message: "Insufficient permissions",
      });
    }
    next();
  };
};
```

### ✅ 2.5 Upload Files (Multer)

**Endpoints:**
- `POST /api/upload/single` - Upload 1 file
- `POST /api/upload/multiple` - Upload nhiều file
- `DELETE /api/upload` - Xóa file đã upload

**Cấu hình:**
- Kích thước tối đa: 50MB
- Định dạng: image/*, pdf, document
- Lưu trữ: thư mục `/uploads`

### ✅ 2.6 Transaction (MongoDB)

**Ví dụ: Tạo đơn hàng với Transaction**
```javascript
const session = await mongoose.startSession();
session.startTransaction();
try {
  // 1. Kiểm tra và trừ tồn kho
  for (const item of items) {
    const sanPham = await SanPham.findById(item.sanPhamId).session(session);
    if (!sanPham || sanPham.soLuongTon < item.soLuong) {
      throw new Error("Sản phẩm không đủ số lượng tồn kho");
    }
    sanPham.soLuongTon -= item.soLuong;
    await sanPham.save({ session });
  }
  
  // 2. Tạo đơn hàng
  const newOrder = new ShopOrder({ ... });
  await newOrder.save({ session });
  
  // 3. Commit transaction
  await session.commitTransaction();
} catch (error) {
  await session.abortTransaction();
}
```

### ✅ 2.7 Socket.IO (Real-time)

**Sự kiện Socket:**
- `join_room` - Tham gia phòng chat
- `join_user` - Đăng ký nhận thông báo
- `send_message` - Gửi tin nhắn realtime
- `new_message` - Nhận tin nhắn mới
- `notification` - Thông báo realtime

**Sử dụng:**
```javascript
// Server
io.to(newMsg.room).emit("new_message", newMsg);

// Client
socket.emit("join_room", "general");
socket.on("new_message", (msg) => {
  console.log("Tin nhắn mới:", msg);
});
```

---

## 3. API ENDPOINTS

### 3.1 Authentication APIs

| Method | Endpoint | Mô tả | Auth |
|--------|----------|--------|------|
| POST | /api/auth/register | Đăng ký | No |
| POST | /api/auth/login | Đăng nhập | No |
| POST | /api/auth/refresh | Refresh token | No |
| POST | /api/auth/logout | Đăng xuất | No |
| GET | /api/auth/me | Thông tin user | Yes |

### 3.2 Product APIs

| Method | Endpoint | Mô tả | Auth |
|--------|----------|--------|------|
| GET | /api/products | Danh sách sản phẩm | No |
| GET | /api/products/:id | Chi tiết sản phẩm | No |
| POST | /api/products | Tạo sản phẩm | Admin/Staff |
| PUT | /api/products/:id | Cập nhật | Admin/Staff |
| DELETE | /api/products/:id | Xóa sản phẩm | Admin |

### 3.3 Order APIs

| Method | Endpoint | Mô tả | Auth |
|--------|----------|--------|------|
| GET | /api/shop-orders | Danh sách đơn hàng | Admin/Staff |
| GET | /api/shop-orders/:id | Chi tiết đơn hàng | Yes |
| POST | /api/shop-orders | Tạo đơn hàng (Transaction) | Yes |
| PUT | /api/shop-orders/:id | Cập nhật đơn hàng | Admin/Staff |
| DELETE | /api/shop-orders/:id | Xóa đơn hàng (Hoàn kho) | Admin |

### 3.4 Booking APIs

| Method | Endpoint | Mô tả | Auth |
|--------|----------|--------|------|
| GET | /api/bookings | Danh sách lịch hẹn | No |
| GET | /api/bookings/:id | Chi tiết lịch hẹn | No |
| POST | /api/bookings | Tạo lịch hẹn | Yes |
| PUT | /api/bookings/:id | Cập nhật lịch hẹn | Admin/Staff |
| DELETE | /api/bookings/:id | Xóa lịch hẹn | Admin |

### 3.5 Chat APIs

| Method | Endpoint | Mô tả | Auth |
|--------|----------|--------|------|
| GET | /api/chat/messages | Lịch sử tin nhắn | No |
| POST | /api/chat/messages | Gửi tin nhắn | No |
| GET | /api/chat/conversations | Danh sách phòng | No |
| PUT | /api/chat/messages/:id | Cập nhật tin nhắn | No |
| DELETE | /api/chat/messages/:id | Xóa tin nhắn | No |

### 3.6 Upload APIs

| Method | Endpoint | Mô tả | Auth |
|--------|----------|--------|------|
| POST | /api/upload/single | Upload 1 file | Yes |
| POST | /api/upload/multiple | Upload nhiều file | Yes |
| DELETE | /api/upload | Xóa file | Yes |

---

## 4. DATABASE MODELS

### 4.1 TaiKhoan (Account)
```javascript
{
  email: String (unique),
  matKhau: String (hashed),
  hoTen: String,
  vaiTro: Enum["user", "staff", "admin"],
  trangThai: Boolean,
  provider: Enum["facebook", "google", null],
  ngayTao: Date,
  lastLogin: Date
}
```

### 4.2 KhachHang (Customer)
```javascript
{
  hoTen: String,
  soDienThoai: String,
  email: String,
  diaChi: String,
  taiKhoanId: ObjectId (ref: TaiKhoan)
}
```

### 4.3 ThuCung (Pet)
```javascript
{
  tenThuCung: String,
  loaiThuCung: Enum["dog", "cat", "bird", "fish", "other"],
  canNang: Number,
  tuoi: Number,
  khachHangId: ObjectId (ref: KhachHang)
}
```

### 4.4 SanPham (Product)
```javascript
{
  tenSanPham: String,
  phanLoai: Enum["THUC_AN", "DO_CHOI", "THUOC", "PHU_KIEN", "KHAC"],
  soLuongTon: Number,
  donGia: Number,
  hinhAnh: [String],
  moTa: String,
  nhaCungCap: String,
  trangThai: Boolean
}
```

### 4.5 ShopOrder (Order)
```javascript
{
  khachHangId: ObjectId (ref: KhachHang),
  items: [{
    sanPhamId: ObjectId (ref: SanPham),
    soLuong: Number,
    donGiaLieu: Number
  }],
  tongTien: Number,
  voucherSuDung: ObjectId (ref: Voucher),
  trangThaiThanhToan: Enum["CHUA_THANH_TOAN", "DA_THANH_TOAN", "HOAN_TIEN"],
  trangThaiGiaoHang: Enum["DANG_XU_LY", "DANG_GIAO", "HOAN_THANH", "DA_HUY"],
  diaChiGiaoHang: String
}
```

---

## 5. FRONTEND

### 5.1 Cấu trúc
```
frontend/
├── index.html          # Trang chủ
├── login.html          # Đăng nhập/Đăng ký
├── shop.html           # Cửa hàng
├── booking.html        # Đặt lịch dịch vụ
├── orders.html         # Quản lý đơn hàng
├── chat.html           # Chat realtime
├── admin.html          # Trang quản trị
├── js/
│   └── api.js          # API wrapper
└── css/
    └── style.css       # Styles
```

### 5.2 API Integration
Frontend sử dụng `api.js` để gọi tất cả API endpoints:
```javascript
// Ví dụ: Lấy danh sách sản phẩm
const products = await Products.getAll();

// Ví dụ: Tạo đơn hàng
const order = await Orders.create(orderData);
```

---

## 6. HƯỚNG DẪN SỬ DỤNG POSTMAN

### 6.1 Cài đặt Collection

1. Import file `SPATC_PetShop.postman_collection.json`
2. Thiết lập Environment variables:
   - `baseUrl`: http://localhost:3000/api
   - `accessToken`: (sẽ được set sau khi login)

### 6.2 Test các API

#### Authentication
1. **Register**: POST /auth/register với body:
```json
{
  "email": "test@example.com",
  "password": "123456",
  "hoTen": "Nguyen Van A",
  "soDienThoai": "0909123456"
}
```

2. **Login**: POST /auth/login với body:
```json
{
  "email": "test@example.com",
  "password": "123456"
}
```

3. **Get Me**: GET /auth/me với header:
```
Authorization: Bearer {{accessToken}}
```

#### Products
1. **Get All**: GET /products
2. **Create**: POST /products với body:
```json
{
  "tenSanPham": "Thức ăn cho mèo",
  "phanLoai": "THUC_AN",
  "soLuongTon": 100,
  "donGia": 150000
}
```

#### Orders (với Transaction)
1. **Create Order**: POST /shop-orders với body:
```json
{
  "khachHangId": "...",
  "items": [{
    "sanPhamId": "...",
    "soLuong": 2,
    "donGiaLieu": 150000
  }],
  "tongTien": 300000,
  "diaChiGiaoHang": "123 Đường ABC, TP.HCM"
}
```

#### Upload
1. **Upload File**: POST /upload/single
   - Body: form-data
   - Key: file (type: File)

#### Chat (Socket.IO)
1. Test trên giao diện web: http://localhost:3000/chat.html

---

## 7. KẾT LUẬN

### 7.1 Đạt được
- ✅ RESTful API với kiến trúc MVC
- ✅ CRUD đầy đủ cho tất cả modules
- ✅ Authentication với JWT
- ✅ Authorization với Role-based access
- ✅ File Upload với Multer
- ✅ Transaction với MongoDB sessions
- ✅ Real-time với Socket.IO
- ✅ Frontend tích hợp đọc API

### 7.2 Hạn chế
- Chưa có unit tests
- Chưa có CI/CD pipeline
- Chat messages lưu in-memory (nên lưu MongoDB)

### 7.3 Hướng phát triển
- Thêm unit tests
- Deploy lên cloud (Heroku, AWS)
- Thêm thanh toán online (VNPay, MoMo)
- Phát triển mobile app

---

**Ngày tạo**: 06/04/2026
**Version**: 1.0.0
