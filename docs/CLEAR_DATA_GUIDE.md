# 🗑️ วิธีการลบข้อมูลทดสอบ (Clear Test Data)

เมื่อคุณรันเทส Swagger และต้องการลบข้อมูลทั้งหมดเพื่อเริ่มต้นใหม่

---

## 📋 วิธีการทั้งหมด

### **วิธีที่ 1: ใช้ PowerShell Script (แนะนำ - ง่ายที่สุด)**

```powershell
# รันคำสั่งนี้ใน Terminal
.\clear-data.ps1
```

**ข้อดี:**
- ✅ ง่ายที่สุด - รันแค่คำสั่งเดียว
- ✅ มีการยืนยันก่อนลบ (ป้องกันการลบโดยไม่ตั้งใจ)
- ✅ แสดงผลลัพธ์ชัดเจน

**ข้อกำหนด:**
- ต้องติดตั้ง SQL Server Command Line tools (sqlcmd)

---

### **วิธีที่ 2: ใช้ SQL Script โดยตรง**

#### 2.1 ใช้ SSMS (SQL Server Management Studio)

1. เปิด SQL Server Management Studio
2. Connect to: `(localdb)\MSSQLLocalDB`
3. เปิดไฟล์: `database\ClearData.sql`
4. กด F5 หรือคลิก Execute

#### 2.2 ใช้ Azure Data Studio

1. เปิด Azure Data Studio
2. Connect to: `(localdb)\MSSQLLocalDB`
3. เปิดไฟล์: `database\ClearData.sql`
4. กด F5 หรือคลิก Run

#### 2.3 ใช้ sqlcmd ใน Command Line

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -d FlowAccountDb -i database\ClearData.sql
```

---

### **วิธีที่ 3: ลบและสร้างฐานข้อมูลใหม่ทั้งหมด**

ถ้าต้องการรีเซ็ตทั้งหมดรวมถึงโครงสร้างตาราง:

```powershell
# 1. ลบ migrations เก่า (ถ้ามี)
Remove-Item -Recurse -Force .\src\FlowAccount.Infrastructure\Migrations

# 2. ลบฐานข้อมูล
dotnet ef database drop --project src/FlowAccount.Infrastructure --startup-project src/FlowAccount.API --force

# 3. สร้าง migration ใหม่
dotnet ef migrations add InitialCreate --project src/FlowAccount.Infrastructure --startup-project src/FlowAccount.API

# 4. สร้างฐานข้อมูลใหม่
dotnet ef database update --project src/FlowAccount.Infrastructure --startup-project src/FlowAccount.API

# 5. เพิ่ม seed data
sqlcmd -S "(localdb)\MSSQLLocalDB" -d FlowAccountDb -i database\SeedData.sql
```

---

## 🔄 หลังจากลบข้อมูลแล้ว

### ถ้าต้องการข้อมูลเริ่มต้น (Seed Data)

```powershell
# รัน SeedData.sql
sqlcmd -S "(localdb)\MSSQLLocalDB" -d FlowAccountDb -i database\SeedData.sql
```

หรือใช้ SSMS/Azure Data Studio เปิดและรัน `database\SeedData.sql`

### ถ้าต้องการฐานข้อมูลเปล่า

ไม่ต้องทำอะไร - ฐานข้อมูลจะว่างเปล่าพร้อมทดสอบผ่าน Swagger ใหม่

---

## 📊 ตรวจสอบจำนวนข้อมูล

### ใช้ SQL Query

```sql
-- ดูจำนวนข้อมูลในแต่ละตาราง
SELECT 'Categories' AS TableName, COUNT(*) AS RecordCount FROM Categories
UNION ALL
SELECT 'Products', COUNT(*) FROM ProductMasters
UNION ALL
SELECT 'Variants', COUNT(*) FROM ProductVariants
UNION ALL
SELECT 'Bundles', COUNT(*) FROM Bundles
UNION ALL
SELECT 'Stocks', COUNT(*) FROM Stocks;
```

---

## ⚠️ คำเตือน

1. **การลบข้อมูลไม่สามารถย้อนกลับได้** - แนะนำให้สำรองข้อมูลก่อนถ้าจำเป็น
2. **Foreign Key Constraints** - Script จะจัดการ FK อัตโนมัติ
3. **Identity Columns** - จะถูก reset เป็น 0 (ID เริ่มใหม่จาก 1)

---

## 🆘 แก้ปัญหา

### ❌ sqlcmd is not recognized

**สาเหตุ:** ไม่ได้ติดตั้ง SQL Server Command Line tools

**แก้ไข:**
1. ติดตั้ง [SQL Server Command Line Tools](https://docs.microsoft.com/en-us/sql/tools/sqlcmd-utility)
2. หรือใช้ SSMS/Azure Data Studio แทน

### ❌ Cannot open database "FlowAccountDb"

**สาเหตุ:** ฐานข้อมูลไม่มีอยู่หรือ connection string ผิด

**แก้ไข:**
```powershell
# สร้างฐานข้อมูลใหม่
dotnet ef database update --project src/FlowAccount.Infrastructure --startup-project src/FlowAccount.API
```

### ❌ Foreign Key Constraint errors

**สาเหตุ:** Script ลบข้อมูลไม่ถูกต้องลำดับ

**แก้ไข:**
- ใช้ script `ClearData.sql` ที่มี FK handling อยู่แล้ว
- หรือลบฐานข้อมูลและสร้างใหม่ (วิธีที่ 3)

---

## 🎯 สรุป Quick Commands

```powershell
# ✅ วิธีที่เร็วที่สุด
.\clear-data.ps1

# 🔄 ลบและเพิ่ม seed data กลับ
.\clear-data.ps1
sqlcmd -S "(localdb)\MSSQLLocalDB" -d FlowAccountDb -i database\SeedData.sql

# 🗑️ ลบและสร้างฐานข้อมูลใหม่ทั้งหมด
dotnet ef database drop --project src/FlowAccount.Infrastructure --startup-project src/FlowAccount.API --force
dotnet ef database update --project src/FlowAccount.Infrastructure --startup-project src/FlowAccount.API
sqlcmd -S "(localdb)\MSSQLLocalDB" -d FlowAccountDb -i database\SeedData.sql
```

---

## 📚 Related Documentation

- [Database Design](./DATABASE_DESIGN_DETAILED.md)
- [How to Test](./HOW_TO_TEST.md)
- [Swagger Documentation](./SWAGGER_DOCUMENTATION.md)
