-- =========================================================
-- 00_reset_database_clean.sql (DESTRUCTIVE)
-- Muc tieu: reset database ve trang thai sach cho team dev
-- Chay voi account co quyen sysadmin (vd: sa)
-- =========================================================

USE [master];
GO

IF DB_ID(N'he_thong_thi_bang_lai') IS NOT NULL
BEGIN
    ALTER DATABASE [he_thong_thi_bang_lai] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [he_thong_thi_bang_lai];
END
GO

CREATE DATABASE [he_thong_thi_bang_lai];
GO

PRINT N'OK: Database he_thong_thi_bang_lai da duoc reset sach.';
GO
