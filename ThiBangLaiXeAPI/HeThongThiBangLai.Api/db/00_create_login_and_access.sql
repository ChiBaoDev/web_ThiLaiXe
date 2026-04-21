-- =========================================================
-- 00_create_login_and_access.sql
-- Muc tieu: Tao SQL Login + Database User + cap quyen cho app .NET
-- Chay bang tai khoan sysadmin (vd: sa) tren SQL Server instance muc tieu
-- Vi du server hien tai: tcp:192.168.1.3,51433
-- =========================================================

USE [master];
GO

-- 1) Tao DB neu chua co
IF DB_ID(N'he_thong_thi_bang_lai') IS NULL
BEGIN
    CREATE DATABASE [he_thong_thi_bang_lai];
END
GO

-- 2) Tao SQL Login cho ung dung
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'tblx_app')
BEGIN
    CREATE LOGIN [tblx_app]
    WITH PASSWORD = N'Replace_Strong_App_Login_Password_2026!',
         CHECK_POLICY = ON,
         CHECK_EXPIRATION = OFF,
         DEFAULT_DATABASE = [he_thong_thi_bang_lai];
END
ELSE
BEGIN
    -- Dev only: cho phep reset password de dong bo moi truong
    ALTER LOGIN [tblx_app]
    WITH PASSWORD = N'Replace_Strong_App_Login_Password_2026!';
END
GO

-- 3) Tao user trong DB map voi login
USE [he_thong_thi_bang_lai];
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'tblx_app')
BEGIN
    CREATE USER [tblx_app] FOR LOGIN [tblx_app];
END
GO

-- 4) Cap quyen cho app
-- Giai doan dev: db_owner de khoi tao/schema nhanh
IF NOT EXISTS (
    SELECT 1
    FROM sys.database_role_members drm
    JOIN sys.database_principals r ON drm.role_principal_id = r.principal_id
    JOIN sys.database_principals m ON drm.member_principal_id = m.principal_id
    WHERE r.name = N'db_owner' AND m.name = N'tblx_app'
)
BEGIN
    ALTER ROLE [db_owner] ADD MEMBER [tblx_app];
END
GO

PRINT N'OK: Da tao login tblx_app + user + quyen trong DB he_thong_thi_bang_lai. Nho dong bo mat khau nay vao file .env';
GO
