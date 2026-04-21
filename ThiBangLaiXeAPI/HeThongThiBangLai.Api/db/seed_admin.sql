-- Seed data for Admin account
-- Run this query after creating the database
-- Note: Replace the mat_khau_hash with a properly hashed value.
-- You can register via API or use PasswordHasher in code to generate hash for password e.g. "Admin@123"

USE he_thong_thi_bang_lai;
GO

-- Insert roles if not exist
MERGE INTO vai_tro AS target
USING (VALUES
    ('ADMIN', N'Quản trị viên', N'Tài khoản quản trị hệ thống'),
    ('HOC_VIEN', N'Học viên', N'Tài khoản học viên'),
    ('GIAO_VIEN', N'Giáo viên', N'Tài khoản giáo viên'),
    ('STAFF_SALE', N'Nhân viên sale', N'Tài khoản tư vấn và xử lý entitlement'),
    ('LEAD', N'Khách tiềm năng', N'Tài khoản mới, chỉ xem nội dung public')
) AS source (ma_vai_tro, ten_vai_tro, mo_ta)
ON target.ma_vai_tro = source.ma_vai_tro
WHEN MATCHED THEN
    UPDATE SET
        ten_vai_tro = source.ten_vai_tro,
        mo_ta = source.mo_ta
WHEN NOT MATCHED BY TARGET THEN
    INSERT (ma_vai_tro, ten_vai_tro, mo_ta)
    VALUES (source.ma_vai_tro, source.ten_vai_tro, source.mo_ta);
GO

-- Insert admin user (change password hash as needed)
IF NOT EXISTS (SELECT 1 FROM nguoi_dung WHERE ten_dang_nhap = 'admin')
BEGIN
    INSERT INTO nguoi_dung (
        ten_dang_nhap, 
        mat_khau_hash, 
        email, 
        so_dien_thoai, 
        trang_thai
    )
    VALUES (
        'admin', 
        'AQAAAAIAAYagAAAAEMr2v6v4v5v6v7v8v9v0v1v2v3v4v5v6v7v8v9v0v', -- Example hash; generate real one using PasswordHasher.HashPassword(new nguoi_dung(), "Admin@123")
        'admin@thibanglaixe.com', 
        '0123456789', 
        'hoat_dong'
    );

    DECLARE @AdminUserId BIGINT = SCOPE_IDENTITY();

    -- Link admin to ADMIN role
    INSERT INTO nguoi_dung_vai_tro (nguoi_dung_id, vai_tro_id)
    SELECT @AdminUserId, id 
    FROM vai_tro 
    WHERE ma_vai_tro = 'ADMIN';

    PRINT 'Admin account created successfully. Use username: admin, password: Admin@123 (update hash if needed)';
END
ELSE
BEGIN
    PRINT 'Admin account already exists.';
END
GO

-- Ensure admin user has all required business roles (at least ADMIN)
INSERT INTO nguoi_dung_vai_tro (nguoi_dung_id, vai_tro_id)
SELECT nd.id, vt.id
FROM nguoi_dung nd
INNER JOIN vai_tro vt ON vt.ma_vai_tro IN ('ADMIN')
WHERE nd.ten_dang_nhap = 'admin'
  AND NOT EXISTS
  (
      SELECT 1
      FROM nguoi_dung_vai_tro ndvt
      WHERE ndvt.nguoi_dung_id = nd.id
        AND ndvt.vai_tro_id = vt.id
  );
GO

-- Optional: Add some permissions for admin role
IF NOT EXISTS (SELECT 1 FROM quyen_han WHERE ma_quyen = 'FULL_ACCESS')
BEGIN
    INSERT INTO quyen_han (ma_quyen, ten_quyen, mo_ta)
    VALUES ('FULL_ACCESS', N'Quyền truy cập đầy đủ', N'Quản trị toàn hệ thống');

    INSERT INTO vai_tro_quyen_han (vai_tro_id, quyen_han_id)
    SELECT v.id, q.id 
    FROM vai_tro v, quyen_han q 
    WHERE v.ma_vai_tro = 'ADMIN' AND q.ma_quyen = 'FULL_ACCESS';
END
GO
