/*
    File: 06_verify_new_modules.sql
    Muc tieu:
    - Verify nhanh cac module moi sau khi setup
    - In ra ket qua de team check
*/

USE he_thong_thi_bang_lai;
GO

SET NOCOUNT ON;
GO

PRINT N'=== [1] VERIFY TABLE EXISTS ===';
SELECT name AS table_name
FROM sys.tables
WHERE name IN
(
    'loai_nguoi_dung',
    'nguoi_dung_loai',
    'goi_quyen',
    'quyen_su_dung',
    'files',
    'file_usages',
    'categories',
    'posts',
    'post_categories',
    'exam_results',
    'certificates'
)
ORDER BY name;
GO

PRINT N'=== [2] VERIFY ROLE/PERMISSION ===';
SELECT ma_vai_tro, ten_vai_tro
FROM dbo.vai_tro
WHERE ma_vai_tro IN ('ADMIN','HOC_VIEN','GIAO_VIEN','STAFF_SALE','LEAD')
ORDER BY ma_vai_tro;

SELECT ma_quyen, ten_quyen
FROM dbo.quyen_han
WHERE ma_quyen IN ('FULL_ACCESS','CMS_POST_MANAGE','FILE_MANAGE','EXAM_ACCESS','CERTIFICATE_ISSUE','ENTITLEMENT_GRANT')
ORDER BY ma_quyen;
GO

PRINT N'=== [3] VERIFY USER TYPES / PACKAGES ===';
SELECT ma_loai, ten_loai
FROM dbo.loai_nguoi_dung
ORDER BY ma_loai;

SELECT ma_goi, ten_goi, is_active
FROM dbo.goi_quyen
ORDER BY ma_goi;
GO

PRINT N'=== [4] VERIFY FILES/CMS/CERTIFICATE COUNTS ===';
SELECT 'files' AS entity_name, COUNT(*) AS total_count FROM dbo.files
UNION ALL
SELECT 'file_usages', COUNT(*) FROM dbo.file_usages
UNION ALL
SELECT 'categories', COUNT(*) FROM dbo.categories
UNION ALL
SELECT 'posts', COUNT(*) FROM dbo.posts
UNION ALL
SELECT 'post_categories', COUNT(*) FROM dbo.post_categories
UNION ALL
SELECT 'exam_results', COUNT(*) FROM dbo.exam_results
UNION ALL
SELECT 'certificates', COUNT(*) FROM dbo.certificates;
GO

PRINT N'=== [5] VERIFY USER DEFAULT ROLE/TYPE COVERAGE ===';
SELECT
    (SELECT COUNT(*) FROM dbo.nguoi_dung) AS total_users,
    (SELECT COUNT(DISTINCT nguoi_dung_id) FROM dbo.nguoi_dung_vai_tro) AS users_with_role,
    (SELECT COUNT(DISTINCT nguoi_dung_id) FROM dbo.nguoi_dung_loai) AS users_with_type;
GO

PRINT N'=== [6] SAMPLE JOIN CHECK ===';
SELECT TOP 20
    nd.id AS nguoi_dung_id,
    nd.ten_dang_nhap,
    vt.ma_vai_tro,
    lnd.ma_loai
FROM dbo.nguoi_dung nd
LEFT JOIN dbo.nguoi_dung_vai_tro ndvt ON ndvt.nguoi_dung_id = nd.id
LEFT JOIN dbo.vai_tro vt ON vt.id = ndvt.vai_tro_id
LEFT JOIN dbo.nguoi_dung_loai ndl ON ndl.nguoi_dung_id = nd.id
LEFT JOIN dbo.loai_nguoi_dung lnd ON lnd.id = ndl.loai_nguoi_dung_id
ORDER BY nd.id DESC;
GO

PRINT N'OK: 06_verify_new_modules.sql completed.';
GO
