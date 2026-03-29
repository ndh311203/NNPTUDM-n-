# PowerShell script để chạy migration SQL
# Yêu cầu: SQL Server Command Line Utilities (sqlcmd) phải được cài đặt

$server = "Admin-PC"
$database = "spatc"
$scriptPath = Join-Path $PSScriptRoot "apply_migration_manually.sql"

Write-Host "Đang kết nối đến database: $database trên server: $server" -ForegroundColor Yellow

# Kiểm tra xem sqlcmd có sẵn không
$sqlcmdPath = Get-Command sqlcmd -ErrorAction SilentlyContinue
if (-not $sqlcmdPath) {
    Write-Host "❌ Không tìm thấy sqlcmd. Vui lòng cài đặt SQL Server Command Line Utilities." -ForegroundColor Red
    Write-Host "Hoặc chạy script SQL thủ công trong SQL Server Management Studio." -ForegroundColor Yellow
    exit 1
}

# Chạy script SQL
Write-Host "Đang chạy migration script..." -ForegroundColor Cyan
sqlcmd -S $server -d $database -i $scriptPath -E

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Migration đã được apply thành công!" -ForegroundColor Green
    Write-Host "Vui lòng restart ứng dụng để áp dụng thay đổi." -ForegroundColor Yellow
} else {
    Write-Host "❌ Có lỗi xảy ra khi chạy migration." -ForegroundColor Red
    Write-Host "Vui lòng kiểm tra lại kết nối database và quyền truy cập." -ForegroundColor Yellow
}














