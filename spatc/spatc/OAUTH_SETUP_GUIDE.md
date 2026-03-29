# Hướng dẫn cấu hình OAuth (Google & Facebook)

## 📋 Yêu cầu

Để sử dụng đăng nhập bằng Google và Facebook, bạn cần:
1. Tạo OAuth credentials từ Google và Facebook
2. Cấu hình các thông tin trong file `appsettings.json`

---

## 🔵 Cấu hình Google OAuth

### Bước 1: Tạo Google OAuth Credentials

1. Truy cập [Google Cloud Console](https://console.cloud.google.com/)
2. Tạo một project mới hoặc chọn project hiện có
3. Vào **APIs & Services** > **Credentials**
4. Click **Create Credentials** > **OAuth client ID**
5. Nếu chưa có, bạn cần cấu hình **OAuth consent screen** trước:
   - Chọn **External** (hoặc Internal nếu có Google Workspace)
   - Điền thông tin ứng dụng (App name, User support email, Developer contact)
   - Thêm scopes: `email`, `profile`
   - Thêm test users (nếu app chưa được verify)

6. Trong **Create OAuth client ID**:
   - Application type: **Web application**
   - Name: Tên bất kỳ (ví dụ: "Spa Thu Cung")
   - **Authorized redirect URIs**: 
     ```
     https://localhost:7227/Login/GoogleCallback
     http://localhost:5000/Login/GoogleCallback
     ```
     (Thay đổi port nếu ứng dụng của bạn chạy ở port khác)

7. Click **Create** và copy **Client ID** và **Client Secret**

### Bước 2: Cập nhật appsettings.json

Mở file `appsettings.json` và thay thế:

```json
"Authentication": {
  "Google": {
    "ClientId": "YOUR_GOOGLE_CLIENT_ID",      // ← Thay bằng Client ID của bạn
    "ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET" // ← Thay bằng Client Secret của bạn
  },
  ...
}
```

Ví dụ:
```json
"Authentication": {
  "Google": {
    "ClientId": "123456789-abcdefghijklmnop.apps.googleusercontent.com",
    "ClientSecret": "GOCSPX-abcdefghijklmnopqrstuvwxyz"
  },
  ...
}
```

---

## 🔵 Cấu hình Facebook OAuth

### Bước 1: Tạo Facebook App

1. Truy cập [Facebook Developers](https://developers.facebook.com/)
2. Click **My Apps** > **Create App**
3. Chọn **Consumer** (hoặc Business nếu cần)
4. Điền thông tin:
   - App name: Tên ứng dụng (ví dụ: "Spa Thu Cung")
   - App contact email: Email của bạn
   - Click **Create App**

5. Vào **Settings** > **Basic**:
   - Thêm **App Domains**: `localhost` (cho development)
   - Thêm **Privacy Policy URL** (bắt buộc): Có thể dùng URL tạm như `https://localhost:7227/privacy`

6. Vào **Products** > **Facebook Login** > **Settings**:
   - **Valid OAuth Redirect URIs**: 
     ```
     https://localhost:7227/Login/FacebookCallback
     http://localhost:5000/Login/FacebookCallback
     ```
   - **Use Strict Mode for Redirect URIs**: Bật ON

7. Lấy **App ID** và **App Secret**:
   - **App ID**: Có thể thấy ở **Settings** > **Basic**
   - **App Secret**: Click **Show** và copy (có thể cần nhập mật khẩu Facebook)

### Bước 2: Cập nhật appsettings.json

Mở file `appsettings.json` và thay thế:

```json
"Authentication": {
  ...
  "Facebook": {
    "AppId": "YOUR_FACEBOOK_APP_ID",      // ← Thay bằng App ID của bạn
    "AppSecret": "YOUR_FACEBOOK_APP_SECRET" // ← Thay bằng App Secret của bạn
  }
}
```

Ví dụ:
```json
"Authentication": {
  ...
  "Facebook": {
    "AppId": "123456789012345",
    "AppSecret": "abcdefghijklmnopqrstuvwxyz123456"
  }
}
```

---

## 🔧 Sau khi cấu hình

1. **Restart ứng dụng** để load cấu hình mới
2. Truy cập trang đăng nhập và test lại chức năng đăng nhập OAuth

---

## ⚠️ Lưu ý

### Cho môi trường Production:

1. **Google**:
   - Cần verify OAuth consent screen nếu có nhiều user
   - Thêm domain production vào **Authorized redirect URIs**
   - Ví dụ: `https://yourdomain.com/Login/GoogleCallback`

2. **Facebook**:
   - Cần submit app để review (nếu dùng quyền public_profile, email)
   - Thêm domain production vào **App Domains** và **Valid OAuth Redirect URIs**
   - Ví dụ: `https://yourdomain.com/Login/FacebookCallback`

3. **Security**:
   - Không commit file `appsettings.json` có chứa secret keys lên Git
   - Sử dụng **User Secrets** hoặc **Environment Variables** cho production:
     ```bash
     # User Secrets (chỉ cho development)
     dotnet user-secrets set "Authentication:Google:ClientSecret" "your-secret"
     
     # Hoặc Environment Variables
     export Authentication__Google__ClientSecret="your-secret"
     ```

---

## 🐛 Troubleshooting

### Lỗi: "redirect_uri_mismatch"
- Kiểm tra lại **Authorized redirect URIs** trong Google/Facebook console
- Đảm bảo URI khớp chính xác (bao gồm http/https, port, path)

### Lỗi: "invalid_client"
- Kiểm tra lại **Client ID** và **Client Secret** đã copy đúng chưa
- Đảm bảo không có khoảng trắng thừa

### Lỗi: "App Not Setup" (Facebook)
- Đảm bảo đã thêm **Facebook Login** product
- Kiểm tra **Valid OAuth Redirect URIs** đã được cấu hình

### Google không yêu cầu quyền email
- Kiểm tra **OAuth consent screen** đã thêm scope `email` và `profile`
- Trong Google Cloud Console, vào **APIs & Services** > **OAuth consent screen** > **Scopes**

---

## 📚 Tài liệu tham khảo

- [Google OAuth 2.0 Documentation](https://developers.google.com/identity/protocols/oauth2)
- [Facebook Login Documentation](https://developers.facebook.com/docs/facebook-login)

