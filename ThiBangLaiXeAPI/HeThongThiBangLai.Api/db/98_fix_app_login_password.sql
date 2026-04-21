-- =========================================================
-- 98_fix_app_login_password.sql
-- Muc tieu: Dong bo password SQL Login tblx_app voi password trong .env
-- Chay bang tai khoan sa tren server: tcp:192.168.1.3,51433
-- =========================================================

USE [master];
GO

-- 1) Dam bao login ton tai
IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = N'tblx_app')
BEGIN
    CREATE LOGIN [tblx_app]
    WITH PASSWORD = N'A9#xT!q7Lm$2',
         CHECK_POLICY = ON,
         CHECK_EXPIRATION = OFF,
         DEFAULT_DATABASE = [he_thong_thi_bang_lai];
END
ELSE
BEGIN
    ALTER LOGIN [tblx_app]
    WITH PASSWORD = N'A9#xT!q7Lm$2',
         CHECK_POLICY = ON,
         CHECK_EXPIRATION = OFF;

    ALTER LOGIN [tblx_app] ENABLE;
END
GO

-- 2) Dam bao map user trong DB
USE [he_thong_thi_bang_lai];
GO

IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = N'tblx_app')
BEGIN
    CREATE USER [tblx_app] FOR LOGIN [tblx_app];
END
GO

-- 3) Cap role dev
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

PRINT N'OK: tblx_app da duoc dong bo password + map user + cap role.';
GO
