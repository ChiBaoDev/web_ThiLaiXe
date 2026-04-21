-- =========================================================
-- 99_verify_database_connection.sql
-- Muc tieu: Kiem tra nhanh API co the ket noi DB dung chuan hay chua
-- Chay trong SSMS tren server muc tieu: tcp:192.168.1.3,51433
-- =========================================================

USE [master];
GO

PRINT N'=== 1) Kiem tra DB ton tai ===';
SELECT name, state_desc
FROM sys.databases
WHERE name = N'he_thong_thi_bang_lai';
GO

PRINT N'=== 2) Kiem tra SQL Login tblx_app ===';
SELECT name, type_desc, is_disabled
FROM sys.server_principals
WHERE name = N'tblx_app';
GO

PRINT N'=== 3) Kiem tra DB User map voi login ===';
USE [he_thong_thi_bang_lai];
GO
SELECT dp.name AS db_user, sp.name AS login_name
FROM sys.database_principals dp
LEFT JOIN sys.server_principals sp ON dp.sid = sp.sid
WHERE dp.name = N'tblx_app';
GO

PRINT N'=== 4) Kiem tra quyen role cua tblx_app ===';
SELECT r.name AS role_name, m.name AS member_name
FROM sys.database_role_members drm
JOIN sys.database_principals r ON drm.role_principal_id = r.principal_id
JOIN sys.database_principals m ON drm.member_principal_id = m.principal_id
WHERE m.name = N'tblx_app';
GO

PRINT N'=== 5) Kiem tra bang cot loi cua auth ===';
SELECT TOP 1 ten_dang_nhap, email, trang_thai
FROM nguoi_dung;
GO

PRINT N'=== 6) Kiem tra role ADMIN/USER da seed ===';
SELECT ma_vai_tro, ten_vai_tro
FROM vai_tro
WHERE ma_vai_tro IN ('ADMIN', 'USER');
GO

PRINT N'=== 7) Neu khong login duoc: reset password login tblx_app ===';
PRINT N'-- ALTER LOGIN [tblx_app] WITH PASSWORD = N''YOUR_APP_LOGIN_PASSWORD'';';
GO

PRINT N'=== KET LUAN ===';
PRINT N'- Neu cac truy van tren tra ve day du => DB da setup dung.';
PRINT N'- Neu API van 500 va log bao login failed => password trong .env khong khop voi SQL Login.';
GO
