# DB Team Runbook (SQL Server riêng)

Tài liệu này chuẩn hóa cách toàn bộ dev BE dựng DB sạch trên server mới.

## 1) Thông tin server hiện tại

- Instance name: `THIBANGLAIXE`
- Server name: `tcp:192.168.1.3,51433`
- Authentication: SQL Server Authentication
- Admin account: `sa`

## 2) Mục tiêu chuẩn team

- Mỗi lần cần reset môi trường dev: chạy 1 script team duy nhất.
- Không chỉnh tay từng bảng trong SSMS.
- Dùng chung schema đúng backend hiện tại.
- Dùng login ứng dụng riêng (`tblx_app`) thay vì để API dùng `sa`.

## 3) Script chính đã chuẩn hóa

- Reset DB sạch: [`00_reset_database_clean.sql`](../db/00_reset_database_clean.sql)
- Tạo login/user/quyền: [`00_create_login_and_access.sql`](../db/00_create_login_and_access.sql)
- Tạo schema đầy đủ: [`new_database_moto_lise.sql`](../db/new_database_moto_lise.sql)
- Seed role/admin: [`seed_admin.sql`](../db/seed_admin.sql)
- Orchestrator chạy 1 lần: [`01_full_setup_database_team.sql`](../db/01_full_setup_database_team.sql)

## 4) Cách chạy đúng trong SSMS

1. Mở SSMS, connect vào server `tcp:192.168.1.3,51433` bằng `sa`.
2. Mở file [`00_create_login_and_access.sql`](../db/00_create_login_and_access.sql) và sửa mật khẩu app login `tblx_app` theo chính sách team.
3. Bật **SQLCMD Mode**: `Query -> SQLCMD Mode`.
4. Mở file [`01_full_setup_database_team.sql`](../db/01_full_setup_database_team.sql).
5. Nhấn Execute.
6. Kiểm tra:
   - có DB `he_thong_thi_bang_lai`
   - có Login `tblx_app`
   - có user `tblx_app` trong DB
   - có dữ liệu role `ADMIN`, `USER`

## 5) Cấu hình backend bằng `.env`

1. Copy [`.env.example`](../.env.example) thành `.env` (cùng thư mục `HeThongThiBangLai.Api`).
2. Điền đúng connection string server mới.
3. Không commit file `.env`.

Ví dụ:

```env
ConnectionStrings__DefaultConnection=Server=tcp:192.168.1.3,51433;Database=he_thong_thi_bang_lai;User Id=tblx_app;Password=YOUR_APP_LOGIN_PASSWORD;TrustServerCertificate=True;MultipleActiveResultSets=True;
```

Lưu ý: `YOUR_APP_LOGIN_PASSWORD` phải giống mật khẩu bạn đã set trong [`00_create_login_and_access.sql`](../db/00_create_login_and_access.sql).

## 6) Quy ước làm việc chung cho dev BE

- Mọi thay đổi schema mới: tạo file SQL migration thủ công trong thư mục `db/` theo dạng `NN_description.sql`.
- Không sửa trực tiếp file gốc mà không ghi rõ lý do trong PR.
- PR có thay đổi DB phải có:
  - script forward,
  - script rollback (nếu có thể),
  - hướng dẫn test nhanh.

## 7) Cảnh báo bảo mật

- Tuyệt đối không dùng `sa` cho API runtime.
- Chỉ dùng `sa` để setup/quản trị.
- Đổi mật khẩu mặc định trong [`00_create_login_and_access.sql`](../db/00_create_login_and_access.sql) trước khi dùng lâu dài.
