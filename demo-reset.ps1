# ======================================
# Quick Demo Reset Script
# ลบข้อมูลเก่า + ใส่ข้อมูล Demo ใหม่ในคำสั่งเดียว
# ======================================

Write-Host ""
Write-Host "FlowAccount - Quick Demo Reset" -ForegroundColor Cyan
Write-Host "===============================" -ForegroundColor Cyan
Write-Host ""

# กำหนด paths
$clearDataSql = Join-Path $PSScriptRoot "database\ClearData.sql"
$seedDataSql = Join-Path $PSScriptRoot "database\SeedData_Demo.sql"
$server = "(localdb)\MSSQLLocalDB"
$database = "FlowAccountDb"

# ตรวจสอบไฟล์
if (-not (Test-Path $clearDataSql)) {
    Write-Host "Error: ClearData.sql not found" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path $seedDataSql)) {
    Write-Host "Error: SeedData.sql not found" -ForegroundColor Red
    exit 1
}

# Step 1: Clear Data
Write-Host "Step 1: Clearing old data..." -ForegroundColor Yellow
try {
    & sqlcmd -S $server -d $database -i $clearDataSql -b
    if ($LASTEXITCODE -ne 0) {
        throw "Clear data failed"
    }
    Write-Host "Old data cleared successfully" -ForegroundColor Green
} catch {
    Write-Host "Failed to clear data: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Step 2: Load Seed Data
Write-Host "Step 2: Loading demo seed data..." -ForegroundColor Yellow
try {
    & sqlcmd -S $server -d $database -i $seedDataSql -b
    if ($LASTEXITCODE -ne 0) {
        throw "Seed data failed"
    }
    Write-Host "Demo seed data loaded successfully" -ForegroundColor Green
} catch {
    Write-Host "Failed to load seed data: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "========== DEMO READY ==========" -ForegroundColor Green
Write-Host ""
Write-Host "Database Summary:" -ForegroundColor Cyan
Write-Host "  - 1 Product (T-Shirt)" -ForegroundColor White
Write-Host "  - 4 Variants (2 Colors x 2 Sizes)" -ForegroundColor White
Write-Host "  - 140 units in stock" -ForegroundColor White
Write-Host "  - IDs start from 1" -ForegroundColor White
Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Cyan
Write-Host "  1. cd src\FlowAccount.API" -ForegroundColor White
Write-Host "  2. dotnet run" -ForegroundColor White
Write-Host "  3. Open: http://localhost:5000/swagger" -ForegroundColor White
Write-Host ""
Write-Host "Ready to record!" -ForegroundColor Green
Write-Host ""
