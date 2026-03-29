using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace spatc.Services
{
    public class VietQRService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;

        public VietQRService(IConfiguration configuration, IWebHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public string GenerateQRCodeImage(decimal amount, string content, int orderId)
        {
            var bankName = _configuration["VietQR:BankName"] ?? "MB Bank";
            var accountNumber = _configuration["VietQR:AccountNumber"] ?? "0842609749";
            var accountName = _configuration["VietQR:AccountName"] ?? "NGUYEN DUC HUY";

            // Tạo nội dung QR theo chuẩn VietQR
            // Format đơn giản: accountNumber|accountName|amount|content
            // Hoặc có thể dùng format: bankName|accountNumber|accountName|amount|content
            var qrContent = $"{accountNumber}|{accountName}|{amount}|{content}";

            // Tạo QR code
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    using (Bitmap qrBitmap = qrCode.GetGraphic(20))
                    {
                        // Lưu vào wwwroot/qrcodes
                        var qrCodesPath = Path.Combine(_environment.WebRootPath, "qrcodes");
                        if (!Directory.Exists(qrCodesPath))
                        {
                            Directory.CreateDirectory(qrCodesPath);
                        }

                        var fileName = $"order_{orderId}_{DateTime.Now:yyyyMMddHHmmss}.png";
                        var filePath = Path.Combine(qrCodesPath, fileName);
                        qrBitmap.Save(filePath, ImageFormat.Png);

                        return $"/qrcodes/{fileName}";
                    }
                }
            }
        }

        public string GetQRCodeDataUrl(decimal amount, string content)
        {
            var bankName = _configuration["VietQR:BankName"] ?? "MB Bank";
            var accountNumber = _configuration["VietQR:AccountNumber"] ?? "0842609749";
            var accountName = _configuration["VietQR:AccountName"] ?? "NGUYEN DUC HUY";

            // Format QR code theo chuẩn VietQR
            // Format: 00020101021238570010A00000072701270006A00000072702970108A00000072703PRODUCTION04<length>00097040<length><accountNumber>08<length><accountName>53037045404<amount>5802VN62<length><content>6304<CRC>
            // Hoặc format đơn giản hơn cho các app ngân hàng: accountNumber|accountName|amount|content
            // Format chuẩn VietQR: 00020101021238570010A00000072701270006A00000072702970108A00000072703PRODUCTION04<length>00097040<length><accountNumber>08<length><accountName>53037045404<amount>5802VN62<length><content>6304<CRC>
            
            // Sử dụng format đơn giản cho tương thích tốt với các app ngân hàng
            // Format: accountNumber|accountName|amount|content
            var qrContent = $"{accountNumber}|{accountName}|{amount}|{content}";

            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    // Tăng kích thước QR code để dễ quét hơn
                    using (Bitmap qrBitmap = qrCode.GetGraphic(25, Color.Black, Color.White, true))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            qrBitmap.Save(ms, ImageFormat.Png);
                            byte[] imageBytes = ms.ToArray();
                            string base64String = Convert.ToBase64String(imageBytes);
                            return $"data:image/png;base64,{base64String}";
                        }
                    }
                }
            }
        }

        public VietQRInfo GetVietQRInfo()
        {
            return new VietQRInfo
            {
                BankName = _configuration["VietQR:BankName"] ?? "MB Bank",
                AccountNumber = _configuration["VietQR:AccountNumber"] ?? "0842609749",
                AccountName = _configuration["VietQR:AccountName"] ?? "NGUYEN DUC HUY"
            };
        }
    }

    public class VietQRInfo
    {
        public string BankName { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
    }
}

