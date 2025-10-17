# ======================================
# Clear Database Script
# สคริปต์สำหรับลบข้อมูลทดสอบทั้งหมดในฐานข้อมูล
# ======================================

Write-Host "🗑️  FlowAccount Database - Clear Data Tool" -ForegroundColor Cyan
Write-Host "==========================================" -ForegroundColor Cyan
Write-Host ""

# ตรวจสอบว่ามี SQL file อยู่หรือไม่
$sqlFile = Join-Path $PSScriptRoot "database\ClearData.sql"
if (-not (Test-Path $sqlFile)) {
    Write-Host "❌ Error: ClearData.sql not found at: $sqlFile" -ForegroundColor Red
    exit 1
}

# ถามยืนยันก่อนลบข้อมูล
Write-Host "⚠️  WARNING: This will delete ALL data in the database!" -ForegroundColor Yellow
Write-Host "This includes:" -ForegroundColor Yellow
Write-Host "  - Products and Variants" -ForegroundColor Yellow
Write-Host "  - Bundles" -ForegroundColor Yellow
Write-Host "  - Stock records" -ForegroundColor Yellow
Write-Host "  - Categories and Warehouses" -ForegroundColor Yellow
Write-Host ""
$confirmation = Read-Host "Are you sure you want to continue? (yes/no)"

if ($confirmation -ne "yes") {
    Write-Host "❌ Operation cancelled" -ForegroundColor Red
    exit 0
}

Write-Host ""
Write-Host "🔄 Executing ClearData.sql..." -ForegroundColor Green

# รัน SQL script
try {
    # ใช้ sqlcmd (ต้องติดตั้ง SQL Server tools)
    sqlcmd -S "(localdb)\MSSQLLocalDB" -d FlowAccountDb -i $sqlFile
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "✅ Database cleared successfully!" -ForegroundColor Green
        Write-Host ""
        Write-Host "💡 Next steps:" -ForegroundColor Cyan
        Write-Host "  1. Run .\database\SeedData.sql to restore seed data" -ForegroundColor White
        Write-Host "  2. Or use Swagger to create new test data" -ForegroundColor White
        Write-Host ""
    } else {
        Write-Host ""
        Write-Host "❌ Error executing SQL script (Exit code: $LASTEXITCODE)" -ForegroundColor Red
        Write-Host "💡 Try running the SQL script manually in SSMS or Azure Data Studio" -ForegroundColor Yellow
        exit 1
    }
} catch {
    Write-Host ""
    Write-Host "❌ Error: $_" -ForegroundColor Red
    Write-Host ""
    Write-Host "💡 Alternative methods:" -ForegroundColor Yellow
    Write-Host "  1. Open SQL Server Management Studio (SSMS)" -ForegroundColor White
    Write-Host "  2. Connect to: (localdb)\MSSQLLocalDB" -ForegroundColor White
    Write-Host "  3. Open file: $sqlFile" -ForegroundColor White
    Write-Host "  4. Execute the script (F5)" -ForegroundColor White
    Write-Host ""
    Write-Host "  OR use Azure Data Studio:" -ForegroundColor White
    Write-Host "  1. Open Azure Data Studio" -ForegroundColor White
    Write-Host "  2. Connect to: (localdb)\MSSQLLocalDB" -ForegroundColor White
    Write-Host "  3. Open and run: $sqlFile" -ForegroundColor White
    exit 1
}
