/*
    File: 05_seed_roles_permissions_entitlements.sql
    Muc tieu:
    - Seed role, permission, role-permission theo business
    - Seed loai_nguoi_dung, goi_quyen
    - Gan default role/type cho user hien co (best effort)

    Luu y:
    - Script idempotent
*/

USE he_thong_thi_bang_lai;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    ------------------------------------------------------------
    -- 1) SEED ROLES
    ------------------------------------------------------------
    MERGE INTO dbo.vai_tro AS target
    USING
    (
        VALUES
            ('ADMIN', N'Quản trị viên', N'Toàn quyền hệ thống'),
            ('HOC_VIEN', N'Học viên', N'Người dùng học và thi'),
            ('GIAO_VIEN', N'Giáo viên', N'Giảng dạy và xác nhận kết quả thi'),
            ('STAFF_SALE', N'Nhân viên tư vấn', N'Xử lý lead, khóa học, entitlement theo thanh toán'),
            ('LEAD', N'Khách tiềm năng', N'Chỉ xem nội dung public, chưa mở quyền học/thi')
    ) AS source(ma_vai_tro, ten_vai_tro, mo_ta)
    ON target.ma_vai_tro = source.ma_vai_tro
    WHEN MATCHED THEN
        UPDATE SET
            ten_vai_tro = source.ten_vai_tro,
            mo_ta = source.mo_ta
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (ma_vai_tro, ten_vai_tro, mo_ta)
        VALUES (source.ma_vai_tro, source.ten_vai_tro, source.mo_ta);

    ------------------------------------------------------------
    -- 2) SEED PERMISSIONS
    ------------------------------------------------------------
    MERGE INTO dbo.quyen_han AS target
    USING
    (
        VALUES
            ('FULL_ACCESS', N'Toàn quyền', N'Quyền quản trị toàn hệ thống'),
            ('CMS_POST_MANAGE', N'Quản trị bài viết CMS', N'Tạo/sửa/xuất bản bài viết và danh mục'),
            ('FILE_MANAGE', N'Quản trị tệp', N'Tải lên metadata, gán file cho entity'),
            ('EXAM_ACCESS', N'Truy cập thi', N'Cho phép truy cập module thi'),
            ('CERTIFICATE_ISSUE', N'Cấp chứng chỉ', N'Xác nhận kết quả và cấp chứng chỉ'),
            ('ENTITLEMENT_GRANT', N'Cấp quyền sử dụng', N'Cấp/revoke quyền sử dụng gói dịch vụ')
    ) AS source(ma_quyen, ten_quyen, mo_ta)
    ON target.ma_quyen = source.ma_quyen
    WHEN MATCHED THEN
        UPDATE SET
            ten_quyen = source.ten_quyen,
            mo_ta = source.mo_ta
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (ma_quyen, ten_quyen, mo_ta)
        VALUES (source.ma_quyen, source.ten_quyen, source.mo_ta);

    ------------------------------------------------------------
    -- 3) ASSIGN ROLE-PERMISSION
    ------------------------------------------------------------
    DECLARE @RoleAdmin BIGINT = (SELECT id FROM dbo.vai_tro WHERE ma_vai_tro = 'ADMIN');
    DECLARE @RoleHocVien BIGINT = (SELECT id FROM dbo.vai_tro WHERE ma_vai_tro = 'HOC_VIEN');
    DECLARE @RoleGiaoVien BIGINT = (SELECT id FROM dbo.vai_tro WHERE ma_vai_tro = 'GIAO_VIEN');
    DECLARE @RoleStaffSale BIGINT = (SELECT id FROM dbo.vai_tro WHERE ma_vai_tro = 'STAFF_SALE');
    DECLARE @RoleLead BIGINT = (SELECT id FROM dbo.vai_tro WHERE ma_vai_tro = 'LEAD');

    DECLARE @PermFullAccess BIGINT = (SELECT id FROM dbo.quyen_han WHERE ma_quyen = 'FULL_ACCESS');
    DECLARE @PermCms BIGINT = (SELECT id FROM dbo.quyen_han WHERE ma_quyen = 'CMS_POST_MANAGE');
    DECLARE @PermFile BIGINT = (SELECT id FROM dbo.quyen_han WHERE ma_quyen = 'FILE_MANAGE');
    DECLARE @PermExamAccess BIGINT = (SELECT id FROM dbo.quyen_han WHERE ma_quyen = 'EXAM_ACCESS');
    DECLARE @PermCertIssue BIGINT = (SELECT id FROM dbo.quyen_han WHERE ma_quyen = 'CERTIFICATE_ISSUE');
    DECLARE @PermEntitlement BIGINT = (SELECT id FROM dbo.quyen_han WHERE ma_quyen = 'ENTITLEMENT_GRANT');

    ;WITH RolePermSeed AS
    (
        SELECT @RoleAdmin AS vai_tro_id, @PermFullAccess AS quyen_han_id
        UNION ALL SELECT @RoleAdmin, @PermCms
        UNION ALL SELECT @RoleAdmin, @PermFile
        UNION ALL SELECT @RoleAdmin, @PermExamAccess
        UNION ALL SELECT @RoleAdmin, @PermCertIssue
        UNION ALL SELECT @RoleAdmin, @PermEntitlement

        UNION ALL SELECT @RoleStaffSale, @PermCms
        UNION ALL SELECT @RoleStaffSale, @PermFile
        UNION ALL SELECT @RoleStaffSale, @PermEntitlement

        UNION ALL SELECT @RoleGiaoVien, @PermExamAccess
        UNION ALL SELECT @RoleGiaoVien, @PermCertIssue

        UNION ALL SELECT @RoleHocVien, @PermExamAccess
    )
    INSERT INTO dbo.vai_tro_quyen_han(vai_tro_id, quyen_han_id)
    SELECT rp.vai_tro_id, rp.quyen_han_id
    FROM RolePermSeed rp
    WHERE rp.vai_tro_id IS NOT NULL
      AND rp.quyen_han_id IS NOT NULL
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.vai_tro_quyen_han vtqh
          WHERE vtqh.vai_tro_id = rp.vai_tro_id
            AND vtqh.quyen_han_id = rp.quyen_han_id
      );

    ------------------------------------------------------------
    -- 4) SEED USER TYPES
    ------------------------------------------------------------
    MERGE INTO dbo.loai_nguoi_dung AS target
    USING
    (
        VALUES
            ('GUEST', N'Khách truy cập', N'Người dùng chưa đăng ký hoặc chưa đăng nhập'),
            ('LEAD', N'Khách tiềm năng', N'Đã đăng ký tài khoản nhưng chưa mở quyền học/thi'),
            ('HOC_VIEN', N'Học viên', N'Người dùng đã kích hoạt quyền học/thi'),
            ('GIAO_VIEN', N'Giáo viên', N'Tài khoản giảng dạy và chấm xác nhận'),
            ('STAFF_SALE', N'Nhân viên sale', N'Tài khoản kinh doanh và tư vấn'),
            ('ADMIN', N'Quản trị hệ thống', N'Tài khoản quản trị')
    ) AS source(ma_loai, ten_loai, mo_ta)
    ON target.ma_loai = source.ma_loai
    WHEN MATCHED THEN
        UPDATE SET
            ten_loai = source.ten_loai,
            mo_ta = source.mo_ta,
            updated_at = GETDATE()
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (ma_loai, ten_loai, mo_ta, created_at, updated_at)
        VALUES (source.ma_loai, source.ten_loai, source.mo_ta, GETDATE(), GETDATE());

    ------------------------------------------------------------
    -- 5) SEED PACKAGES
    ------------------------------------------------------------
    MERGE INTO dbo.goi_quyen AS target
    USING
    (
        VALUES
            ('LEARNING_MATERIAL', N'Gói tài liệu ôn tập', N'Cho phép truy cập tài liệu ôn tập chuyên sâu', 1),
            ('EXAM_PRACTICE', N'Gói thi thử', N'Cho phép truy cập thi thử và thống kê', 1),
            ('FULL_COURSE_A1', N'Gói học đầy đủ A1', N'Tài liệu + thi thử + quản lý hồ sơ dự thi', 1)
    ) AS source(ma_goi, ten_goi, mo_ta, is_active)
    ON target.ma_goi = source.ma_goi
    WHEN MATCHED THEN
        UPDATE SET
            ten_goi = source.ten_goi,
            mo_ta = source.mo_ta,
            is_active = source.is_active,
            updated_at = GETDATE()
    WHEN NOT MATCHED BY TARGET THEN
        INSERT (ma_goi, ten_goi, mo_ta, is_active, created_at, updated_at)
        VALUES (source.ma_goi, source.ten_goi, source.mo_ta, source.is_active, GETDATE(), GETDATE());

    ------------------------------------------------------------
    -- 6) ASSIGN DEFAULT ROLE/TYPE FOR EXISTING USERS
    ------------------------------------------------------------
    DECLARE @RoleLead BIGINT = (SELECT id FROM dbo.vai_tro WHERE ma_vai_tro = 'LEAD');
    DECLARE @TypeLead BIGINT = (SELECT id FROM dbo.loai_nguoi_dung WHERE ma_loai = 'LEAD');
    DECLARE @TypeAdmin BIGINT = (SELECT id FROM dbo.loai_nguoi_dung WHERE ma_loai = 'ADMIN');

    -- User chua co role -> gan LEAD
    INSERT INTO dbo.nguoi_dung_vai_tro(nguoi_dung_id, vai_tro_id)
    SELECT nd.id, @RoleLead
    FROM dbo.nguoi_dung nd
    WHERE @RoleLead IS NOT NULL
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.nguoi_dung_vai_tro ndvt
          WHERE ndvt.nguoi_dung_id = nd.id
      );

    -- User co role ADMIN -> loai ADMIN
    INSERT INTO dbo.nguoi_dung_loai(nguoi_dung_id, loai_nguoi_dung_id, created_at)
    SELECT DISTINCT ndvt.nguoi_dung_id, @TypeAdmin, GETDATE()
    FROM dbo.nguoi_dung_vai_tro ndvt
    INNER JOIN dbo.vai_tro vt ON vt.id = ndvt.vai_tro_id
    WHERE vt.ma_vai_tro = 'ADMIN'
      AND @TypeAdmin IS NOT NULL
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.nguoi_dung_loai ndl
          WHERE ndl.nguoi_dung_id = ndvt.nguoi_dung_id
            AND ndl.loai_nguoi_dung_id = @TypeAdmin
      );

    -- User chua co type -> gan LEAD
    INSERT INTO dbo.nguoi_dung_loai(nguoi_dung_id, loai_nguoi_dung_id, created_at)
    SELECT nd.id, @TypeLead, GETDATE()
    FROM dbo.nguoi_dung nd
    WHERE @TypeLead IS NOT NULL
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.nguoi_dung_loai ndl
          WHERE ndl.nguoi_dung_id = nd.id
      );

    COMMIT TRANSACTION;
    PRINT N'OK: 05_seed_roles_permissions_entitlements.sql completed.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrLine INT = ERROR_LINE();
    DECLARE @ErrNum INT = ERROR_NUMBER();

    RAISERROR(N'[05_seed_roles_permissions_entitlements.sql] Error %d at line %d: %s', 16, 1, @ErrNum, @ErrLine, @ErrMsg);
END CATCH;
GO
