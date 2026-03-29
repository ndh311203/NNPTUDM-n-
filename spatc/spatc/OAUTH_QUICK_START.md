# 🚀 Hướng dẫn nhanh cấu hình OAuth (5 phút)

## ⚡ Quick Start - Làm theo từng bước này

---

## 🔵 BƯỚC 1: Google OAuth (2 phút)

### 1.1. Mở Google Cloud Console
👉 [https://console.cloud.google.com/](https://console.cloud.google.com/)

### 1.2. Tạo OAuth Credentials
1. Click vào **dropdown project** ở trên cùng (hoặc tạo project mới nếu chưa có)
2. Vào **APIs & Services** > **Credentials** (menu bên trái)
3. Click **+ CREATE CREDENTIALS** > **OAuth client ID**

### 1.3. Nếu hỏi "OAuth consent screen":
- Click **CONFIGURE CONSENT SCREEN**
- Chọn **External** > Click **CREATE**
- **App name**: `Spa Thu Cung` (hoặc tên bạn muốn)
- **User support email**: Chọn email của bạn
- **Developer contact email**: Email của bạn
- Click **SAVE AND CONTINUE** (3 lần để bỏ qua các bước khác)
- Click **BACK TO DASHBOARD**

### 1.4. Tạo OAuth Client ID:
1. Quay lại **Credentials** > **+ CREATE CREDENTIALS** > **OAuth client ID**
2. **Application type**: Chọn **Web application**
3. **Name**: `Spa Thu Cung Web Client`
4. **Authorized redirect URIs**: Click **+ ADD URI** và thêm:
   ```
   https://localhost:7227/Login/GoogleCallback
   ```
   (Nếu port khác, thay `7227` bằng port của bạn)
5. Click **CREATE**
6. **COPY** 2 thông tin này:
   - **Your Client ID**: `123456789-xxxxx.apps.googleusercontent.com`
   - **Your Client Secret**: `GOCSPX-xxxxx`

### 1.5. Cập nhật appsettings.json
Mở file `spatc/spatc/appsettings.json`, tìm dòng:
```json
"ClientId": "YOUR_GOOGLE_CLIENT_ID",
"ClientSecret": "YOUR_GOOGLE_CLIENT_SECRET"
```

Thay bằng giá trị vừa copy:
```json
"ClientId": "123456789-xxxxx.apps.googleusercontent.com",
"ClientSecret": "GOCSPX-xxxxx"
```

✅ **Xong Google!**

---

## 🔵 BƯỚC 2: Facebook OAuth (3 phút)

### 2.1. Mở Facebook Developers
👉 [https://developers.facebook.com/](https://developers.facebook.com/)

### 2.2. Tạo App mới
1. Click **My Apps** (góc trên bên phải)
2. Click **Create App**
3. Chọn **Consumer** > Click **Next**
4. **App name**: `Spa Thu Cung` (hoặc tên bạn muốn)
5. **App contact email**: Email của bạn
6. Click **Create App**

### 2.3. Thêm Facebook Login
1. Trong dashboard, tìm **Facebook Login** > Click **Set Up**
2. Chọn **Web** > Click **Next**
3. **Site URL**: Nhập `https://localhost:7227` (hoặc port của bạn)
4. Click **Save** > Click **Continue**

### 2.4. Cấu hình Settings
1. Menu bên trái: **Settings** > **Basic**
2. **Privacy Policy URL**: Nhập `https://localhost:7227/privacy` (tạm thời)
3. **App Domains**: Nhập `localhost`
4. Click **Save Changes**

### 2.5. Cấu hình Facebook Login Settings
1. Menu bên trái: **Products** > **Facebook Login** > **Settings**
2. **Valid OAuth Redirect URIs**: Thêm:
   ```
   https://localhost:7227/Login/FacebookCallback
   ```
3. Click **Save Changes**

### 2.6. Lấy App ID và App Secret
1. Vào **Settings** > **Basic**
2. **App ID**: Copy số này (ví dụ: `123456789012345`)
3. **App Secret**: Click **Show** > Nhập mật khẩu Facebook > Copy (ví dụ: `abcdef123456789`)

### 2.7. Cập nhật appsettings.json
Mở file `spatc/spatc/appsettings.json`, tìm dòng:
```json
"AppId": "YOUR_FACEBOOK_APP_ID",
"AppSecret": "YOUR_FACEBOOK_APP_SECRET"
```

Thay bằng giá trị vừa copy:
```json
"AppId": "123456789012345",
"AppSecret": "abcdef123456789"
```

✅ **Xong Facebook!**

---

## ✅ BƯỚC 3: Restart và Test

1. **Lưu file** `appsettings.json`
2. **Restart ứng dụng** (dừng và chạy lại `dotnet run` hoặc restart trong Visual Studio)
3. **Mở trình duyệt**: `https://localhost:7227/Login`
4. **Click nút "Đăng nhập với Google"** hoặc **"Đăng nhập với Facebook"**
5. Nếu hiển thị màn hình đăng nhập Google/Facebook = **THÀNH CÔNG!** ✅

---

## ❌ Nếu gặp lỗi:

### Lỗi "redirect_uri_mismatch" (Google):
- Kiểm tra lại **Authorized redirect URIs** trong Google Cloud Console
- Đảm bảo đúng: `https://localhost:7227/Login/GoogleCallback` (không có dấu / ở cuối)

### Lỗi "App Not Setup" (Facebook):
- Kiểm tra đã thêm **Facebook Login** product chưa
- Kiểm tra **Valid OAuth Redirect URIs** đã thêm chưa

### Vẫn báo "chưa được cấu hình":
- Đảm bảo đã **restart ứng dụng** sau khi sửa `appsettings.json`
- Kiểm tra lại không còn chữ `YOUR_GOOGLE_CLIENT_ID` hay `YOUR_FACEBOOK_APP_ID` trong file

---

## 🎉 Xong rồi!

Bây giờ bạn có thể đăng nhập bằng Google và Facebook rồi! 🚀

