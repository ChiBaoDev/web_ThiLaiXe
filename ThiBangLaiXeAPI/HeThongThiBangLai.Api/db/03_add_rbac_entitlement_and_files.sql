/*
    File: 03_add_rbac_entitlement_and_files.sql
    Muc tieu:
    - Mo rong RBAC theo business role (HOC_VIEN/GIAO_VIEN/STAFF_SALE/LEAD)
    - Bo sung user types + entitlement package
    - Bo sung files storage trung tam + file_usages

    Luu y:
    - Script idempotent: co the chay lap lai
    - Yeu cau DB: he_thong_thi_bang_lai
*/

USE he_thong_thi_bang_lai;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    ------------------------------------------------------------
    -- 1) USER TYPES
    ------------------------------------------------------------
    IF OBJECT_ID('dbo.loai_nguoi_dung', 'U') IS NULL
    BEGIN
        CREATE TABLE dbo.loai_nguoi_dung
        (
            id BIGINT IDENTITY(1,1) PRIMARY KEY,
            ma_loai VARCHAR(30) NOT NULL,
            ten_loai NVARCHAR(100) NOT NULL,
            mo_ta NVARCHAR(255) NULL,
            created_at DATETIME2 NOT NULL CONSTRAINT df_loai_nguoi_dung_created_at DEFAULT GETDATE(),
            updated_at DATETIME2 NOT NULL CONSTRAINT df_loai_nguoi_dung_updated_at DEFAULT GETDATE(),
            CONSTRAINT uq_loai_nguoi_dung_ma_loai UNIQUE (ma_loai)
        );
    END;

    IF OBJECT_ID('dbo.nguoi_dung_loai', 'U') IS NULL
    BEGIN
        CREATE TABLE dbo.nguoi_dung_loai
        (
            id BIGINT IDENTITY(1,1) PRIMARY KEY,
            nguoi_dung_id BIGINT NOT NULL,
            loai_nguoi_dung_id BIGINT NOT NULL,
            created_at DATETIME2 NOT NULL CONSTRAINT df_nguoi_dung_loai_created_at DEFAULT GETDATE(),
            CONSTRAINT fk_ndl_nguoi_dung FOREIGN KEY (nguoi_dung_id) REFERENCES dbo.nguoi_dung(id),
            CONSTRAINT fk_ndl_loai_nguoi_dung FOREIGN KEY (loai_nguoi_dung_id) REFERENCES dbo.loai_nguoi_dung(id),
            CONSTRAINT uq_nguoi_dung_loai UNIQUE (nguoi_dung_id, loai_nguoi_dung_id)
        );

        CREATE INDEX ix_nguoi_dung_loai_nguoi_dung_id ON dbo.nguoi_dung_loai(nguoi_dung_id);
        CREATE INDEX ix_nguoi_dung_loai_loai_id ON dbo.nguoi_dung_loai(loai_nguoi_dung_id);
    END;

    ------------------------------------------------------------
    -- 2) ENTITLEMENT PACKAGE + USER ENTITLEMENT
    ------------------------------------------------------------
    IF OBJECT_ID('dbo.goi_quyen', 'U') IS NULL
    BEGIN
        CREATE TABLE dbo.goi_quyen
        (
            id BIGINT IDENTITY(1,1) PRIMARY KEY,
            ma_goi VARCHAR(50) NOT NULL,
            ten_goi NVARCHAR(150) NOT NULL,
            mo_ta NVARCHAR(500) NULL,
            is_active BIT NOT NULL CONSTRAINT df_goi_quyen_is_active DEFAULT 1,
            created_at DATETIME2 NOT NULL CONSTRAINT df_goi_quyen_created_at DEFAULT GETDATE(),
            updated_at DATETIME2 NOT NULL CONSTRAINT df_goi_quyen_updated_at DEFAULT GETDATE(),
            CONSTRAINT uq_goi_quyen_ma_goi UNIQUE (ma_goi)
        );
    END;

    IF OBJECT_ID('dbo.quyen_su_dung', 'U') IS NULL
    BEGIN
        CREATE TABLE dbo.quyen_su_dung
        (
            id BIGINT IDENTITY(1,1) PRIMARY KEY,
            nguoi_dung_id BIGINT NOT NULL,
            goi_quyen_id BIGINT NOT NULL,
            ngay_hieu_luc DATETIME2 NOT NULL,
            ngay_het_han DATETIME2 NULL,
            nguon_cap VARCHAR(30) NOT NULL,
            trang_thai VARCHAR(30) NOT NULL,
            ghi_chu NVARCHAR(500) NULL,
            created_by BIGINT NULL,
            created_at DATETIME2 NOT NULL CONSTRAINT df_quyen_su_dung_created_at DEFAULT GETDATE(),
            updated_at DATETIME2 NOT NULL CONSTRAINT df_quyen_su_dung_updated_at DEFAULT GETDATE(),
            CONSTRAINT fk_qsd_nguoi_dung FOREIGN KEY (nguoi_dung_id) REFERENCES dbo.nguoi_dung(id),
            CONSTRAINT fk_qsd_goi_quyen FOREIGN KEY (goi_quyen_id) REFERENCES dbo.goi_quyen(id),
            CONSTRAINT fk_qsd_created_by FOREIGN KEY (created_by) REFERENCES dbo.nguoi_dung(id),
            CONSTRAINT ck_qsd_nguon_cap CHECK (nguon_cap IN ('payment','manual','promo')),
            CONSTRAINT ck_qsd_trang_thai CHECK (trang_thai IN ('active','expired','revoked')),
            CONSTRAINT ck_qsd_ngay CHECK (ngay_het_han IS NULL OR ngay_het_han >= ngay_hieu_luc)
        );

        CREATE INDEX ix_qsd_nguoi_dung_trang_thai ON dbo.quyen_su_dung(nguoi_dung_id, trang_thai);
        CREATE INDEX ix_qsd_ngay_het_han ON dbo.quyen_su_dung(ngay_het_han);
        CREATE INDEX ix_qsd_goi_quyen_id ON dbo.quyen_su_dung(goi_quyen_id);
    END;

    ------------------------------------------------------------
    -- 3) FILE STORAGE TABLES
    ------------------------------------------------------------
    IF OBJECT_ID('dbo.files', 'U') IS NULL
    BEGIN
        CREATE TABLE dbo.files
        (
            id BIGINT IDENTITY(1,1) PRIMARY KEY,
            storage_provider VARCHAR(30) NOT NULL,
            bucket_name VARCHAR(100) NULL,
            object_key VARCHAR(500) NOT NULL,
            public_url VARCHAR(1000) NOT NULL,
            file_name NVARCHAR(255) NOT NULL,
            mime_type VARCHAR(100) NOT NULL,
            size_bytes BIGINT NOT NULL,
            checksum_sha256 VARCHAR(128) NULL,
            width INT NULL,
            height INT NULL,
            duration_seconds INT NULL,
            trang_thai VARCHAR(30) NOT NULL CONSTRAINT df_files_trang_thai DEFAULT 'active',
            created_by BIGINT NULL,
            created_at DATETIME2 NOT NULL CONSTRAINT df_files_created_at DEFAULT GETDATE(),
            updated_at DATETIME2 NOT NULL CONSTRAINT df_files_updated_at DEFAULT GETDATE(),
            CONSTRAINT fk_files_created_by FOREIGN KEY (created_by) REFERENCES dbo.nguoi_dung(id),
            CONSTRAINT ck_files_size_bytes CHECK (size_bytes >= 0),
            CONSTRAINT ck_files_dimensions CHECK ((width IS NULL OR width >= 0) AND (height IS NULL OR height >= 0)),
            CONSTRAINT ck_files_duration CHECK (duration_seconds IS NULL OR duration_seconds >= 0),
            CONSTRAINT ck_files_storage_provider CHECK (storage_provider IN ('local','s3','cloudinary','azure_blob','gcs')),
            CONSTRAINT ck_files_trang_thai CHECK (trang_thai IN ('active','archived','deleted'))
        );

        CREATE INDEX ix_files_storage_provider ON dbo.files(storage_provider);
        CREATE INDEX ix_files_created_at ON dbo.files(created_at);
        CREATE INDEX ix_files_created_by ON dbo.files(created_by);
    END;

    IF OBJECT_ID('dbo.file_usages', 'U') IS NULL
    BEGIN
        CREATE TABLE dbo.file_usages
        (
            id BIGINT IDENTITY(1,1) PRIMARY KEY,
            file_id BIGINT NOT NULL,
            entity_name VARCHAR(50) NOT NULL,
            entity_id BIGINT NOT NULL,
            field_name VARCHAR(50) NOT NULL,
            is_primary BIT NOT NULL CONSTRAINT df_file_usages_is_primary DEFAULT 0,
            sort_order INT NOT NULL CONSTRAINT df_file_usages_sort_order DEFAULT 0,
            created_at DATETIME2 NOT NULL CONSTRAINT df_file_usages_created_at DEFAULT GETDATE(),
            CONSTRAINT fk_fu_file FOREIGN KEY (file_id) REFERENCES dbo.files(id),
            CONSTRAINT uq_file_usages UNIQUE (file_id, entity_name, entity_id, field_name),
            CONSTRAINT ck_file_usages_sort_order CHECK (sort_order >= 0)
        );

        CREATE INDEX ix_file_usages_entity ON dbo.file_usages(entity_name, entity_id);
        CREATE INDEX ix_file_usages_file_id ON dbo.file_usages(file_id);
    END;

    ------------------------------------------------------------
    -- 4) BACKFILL ANH CHAN DUNG -> FILES/FILE_USAGES (best effort)
    ------------------------------------------------------------
    ;WITH cte_avatar AS
    (
        SELECT
            hv.id AS hoc_vien_id,
            hv.nguoi_dung_id,
            hv.anh_chan_dung
        FROM dbo.hoc_vien hv
        WHERE hv.anh_chan_dung IS NOT NULL
          AND LTRIM(RTRIM(hv.anh_chan_dung)) <> ''
    )
    INSERT INTO dbo.files
    (
        storage_provider,
        bucket_name,
        object_key,
        public_url,
        file_name,
        mime_type,
        size_bytes,
        checksum_sha256,
        width,
        height,
        duration_seconds,
        trang_thai,
        created_by,
        created_at,
        updated_at
    )
    SELECT
        'local',
        NULL,
        a.anh_chan_dung,
        a.anh_chan_dung,
        RIGHT(a.anh_chan_dung, CHARINDEX('/', REVERSE(REPLACE(a.anh_chan_dung, '\\', '/'))) - 1),
        'image/jpeg',
        0,
        NULL,
        NULL,
        NULL,
        NULL,
        'active',
        a.nguoi_dung_id,
        GETDATE(),
        GETDATE()
    FROM cte_avatar a
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.file_usages fu
        WHERE fu.entity_name = 'hoc_vien'
          AND fu.entity_id = a.hoc_vien_id
          AND fu.field_name = 'avatar'
    );

    ;WITH map_avatar AS
    (
        SELECT
            hv.id AS hoc_vien_id,
            f.id AS file_id
        FROM dbo.hoc_vien hv
        INNER JOIN dbo.files f ON f.public_url = hv.anh_chan_dung
        WHERE hv.anh_chan_dung IS NOT NULL
          AND LTRIM(RTRIM(hv.anh_chan_dung)) <> ''
    )
    INSERT INTO dbo.file_usages
    (
        file_id,
        entity_name,
        entity_id,
        field_name,
        is_primary,
        sort_order,
        created_at
    )
    SELECT
        m.file_id,
        'hoc_vien',
        m.hoc_vien_id,
        'avatar',
        1,
        0,
        GETDATE()
    FROM map_avatar m
    WHERE NOT EXISTS
    (
        SELECT 1
        FROM dbo.file_usages fu
        WHERE fu.file_id = m.file_id
          AND fu.entity_name = 'hoc_vien'
          AND fu.entity_id = m.hoc_vien_id
          AND fu.field_name = 'avatar'
    );

    COMMIT TRANSACTION;
    PRINT N'OK: 03_add_rbac_entitlement_and_files.sql completed.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrLine INT = ERROR_LINE();
    DECLARE @ErrNum INT = ERROR_NUMBER();

    RAISERROR(N'[03_add_rbac_entitlement_and_files.sql] Error %d at line %d: %s', 16, 1, @ErrNum, @ErrLine, @ErrMsg);
END CATCH;
GO
