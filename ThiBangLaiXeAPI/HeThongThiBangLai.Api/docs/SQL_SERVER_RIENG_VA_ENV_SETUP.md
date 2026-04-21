# Hướng dẫn tạo "server riêng" SQL Server + kết nối `.env`

## 1) Làm rõ khái niệm “server riêng”

Trong SQL Server trên máy local, có 2 cách thường gọi là “riêng”:

1. **Riêng ở mức tài khoản + database** (nhanh, đủ cho dev):
   - vẫn dùng instance hiện tại `localhost`
   - tạo login riêng `tblx_app`
   - tạo database riêng `he_thong_thi_bang_lai`

2. **Riêng ở mức instance** (đúng nghĩa server riêng hơn):
   - cài thêm **named instance** ví dụ `localhost\THIBANGLAIXE`
   - mỗi instance có config riêng (port, memory, login, DB)

Nếu bạn muốn “server riêng” đúng nghĩa, chọn phương án 2.

---

## 2) Tạo named instance riêng `THIBANGLAIXE`

### Bước A - Mở SQL Server Installation Center
- Mở bộ cài SQL Server.
- Chọn `Installation` → `New SQL Server stand-alone installation...`.

### Bước B - Chọn Instance Name
- Chọn `Named instance` = `THIBANGLAIXE`.
- Sau khi cài xong, SSMS sẽ connect bằng `localhost\THIBANGLAIXE`.

### Bước C - Database Engine Configuration
- Chọn `Mixed Mode (SQL Server authentication and Windows authentication)`.
- Đặt password mạnh cho `sa`.
- Add user Windows của bạn vào SQL administrators.

### Bước D - Bật dịch vụ cần thiết
- Mở `SQL Server Configuration Manager`.
- Đảm bảo service `SQL Server (THIBANGLAIXE)` đang `Running`.
- Start thêm `SQL Server Browser` (khuyên dùng cho named instance).

### Bước E - Mở SSMS và kết nối
- Server name: `localhost\THIBANGLAIXE`
- Authentication: `SQL Server Authentication`
- Login: `sa`
- Password: password bạn đặt ở bước C.

---

## 3) Chạy script tạo DB riêng cho dự án

Sau khi kết nối vào `localhost\THIBANGLAIXE` bằng tài khoản `sa`:

1. Mở cửa sổ query mới.
2. Paste toàn bộ nội dung file [`01_full_setup_database.sql`](../db/01_full_setup_database.sql).
3. Nhấn `Execute`.

Script này sẽ:
- tạo DB `he_thong_thi_bang_lai` (nếu chưa có),
- tạo login app `tblx_app`,
- map DB user `tblx_app`,
- cấp quyền dev (`db_owner`),
- tạo schema + index,
- seed role + admin mẫu.

---

## 4) Kết nối API qua `.env`

Dự án đã được bổ sung đọc `.env` trong [`Program.cs`](../Program.cs:33).

### Bước 1 - Tạo file `.env`
- Copy từ [`.env.example`](../.env.example).
- Tạo file thật: `HeThongThiBangLai.Api/.env`.

### Bước 2 - Cấu hình chuỗi kết nối named instance

```env
ConnectionStrings__DefaultConnection=Server=localhost\THIBANGLAIXE;Database=he_thong_thi_bang_lai;User Id=tblx_app;Password=YOUR_STRONG_PASSWORD;TrustServerCertificate=True;MultipleActiveResultSets=True;
```

### Bước 3 - Chạy API
- Dùng script [`start-api.cmd`](../../start-api.cmd).
- Test lại login Swagger.

---

## 5) Checklist xác minh nhanh

1. SSMS connect được `localhost\THIBANGLAIXE` bằng `sa`.
2. Có DB `he_thong_thi_bang_lai` trong Object Explorer.
3. Trong `Security > Logins` có `tblx_app`.
4. API đọc đúng connection string từ `.env`.
5. `POST /api/v1/auth/login` không còn lỗi `Login failed for user ''`.

---

## 6) Gợi ý production (ngắn gọn)

- Không dùng `db_owner` cho app ở production.
- Tạo role riêng chỉ đủ quyền `SELECT/INSERT/UPDATE/DELETE` trên schema cần thiết.
- Rotate password định kỳ.
- Không commit `.env` và không commit secret vào `appsettings.json`.
