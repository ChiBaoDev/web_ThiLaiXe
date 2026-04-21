/*
    File: 04_add_cms_and_certificate.sql
    Muc tieu:
    - Bo sung CMS co ban (categories, posts, post_categories)
    - Bo sung ket qua thi + chung chi (exam_results, certificates)

    Luu y:
    - Script idempotent: co the chay lap lai
*/

USE he_thong_thi_bang_lai;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    ------------------------------------------------------------
    -- 1) CMS TABLES
    ------------------------------------------------------------
    IF OBJECT_ID('dbo.categories', 'U') IS NULL
    BEGIN
        CREATE TABLE dbo.categories
        (
            id BIGINT IDENTITY(1,1) PRIMARY KEY,
            parent_id BIGINT NULL,
            ma_danh_muc VARCHAR(50) NOT NULL,
            ten_danh_muc NVARCHAR(150) NOT NULL,
            slug VARCHAR(200) NOT NULL,
            mo_ta NVARCHAR(500) NULL,
            is_active BIT NOT NULL CONSTRAINT df_categories_is_active DEFAULT 1,
            created_by BIGINT NULL,
            created_at DATETIME2 NOT NULL CONSTRAINT df_categories_created_at DEFAULT GETDATE(),
            updated_at DATETIME2 NOT NULL CONSTRAINT df_categories_updated_at DEFAULT GETDATE(),
            CONSTRAINT uq_categories_ma_danh_muc UNIQUE (ma_danh_muc),
            CONSTRAINT uq_categories_slug UNIQUE (slug),
            CONSTRAINT fk_categories_parent FOREIGN KEY (parent_id) REFERENCES dbo.categories(id),
            CONSTRAINT fk_categories_created_by FOREIGN KEY (created_by) REFERENCES dbo.nguoi_dung(id)
        );

        CREATE INDEX ix_categories_parent_id ON dbo.categories(parent_id);
        CREATE INDEX ix_categories_is_active ON dbo.categories(is_active);
    END;

    IF OBJECT_ID('dbo.posts', 'U') IS NULL
    BEGIN
        CREATE TABLE dbo.posts
        (
            id BIGINT IDENTITY(1,1) PRIMARY KEY,
            ma_bai_viet VARCHAR(50) NOT NULL,
            title NVARCHAR(255) NOT NULL,
            slug VARCHAR(255) NOT NULL,
            summary NVARCHAR(1000) NULL,
            content NVARCHAR(MAX) NOT NULL,
            post_type VARCHAR(30) NOT NULL,
            thumbnail_file_id BIGINT NULL,
            meta_title NVARCHAR(255) NULL,
            meta_description NVARCHAR(500) NULL,
            canonical_url VARCHAR(500) NULL,
            published_at DATETIME2 NULL,
            trang_thai VARCHAR(30) NOT NULL CONSTRAINT df_posts_trang_thai DEFAULT 'draft',
            author_id BIGINT NULL,
            created_at DATETIME2 NOT NULL CONSTRAINT df_posts_created_at DEFAULT GETDATE(),
            updated_at DATETIME2 NOT NULL CONSTRAINT df_posts_updated_at DEFAULT GETDATE(),
            CONSTRAINT uq_posts_ma_bai_viet UNIQUE (ma_bai_viet),
            CONSTRAINT uq_posts_slug UNIQUE (slug),
            CONSTRAINT fk_posts_thumbnail_file FOREIGN KEY (thumbnail_file_id) REFERENCES dbo.files(id),
            CONSTRAINT fk_posts_author FOREIGN KEY (author_id) REFERENCES dbo.nguoi_dung(id),
            CONSTRAINT ck_posts_post_type CHECK (post_type IN ('gioi_thieu','tin_tuc','khoa_hoc','huong_dan')),
            CONSTRAINT ck_posts_trang_thai CHECK (trang_thai IN ('draft','published','archived'))
        );

        CREATE INDEX ix_posts_post_type ON dbo.posts(post_type);
        CREATE INDEX ix_posts_trang_thai ON dbo.posts(trang_thai);
        CREATE INDEX ix_posts_published_at ON dbo.posts(published_at);
        CREATE INDEX ix_posts_author_id ON dbo.posts(author_id);
    END;

    IF OBJECT_ID('dbo.post_categories', 'U') IS NULL
    BEGIN
        CREATE TABLE dbo.post_categories
        (
            id BIGINT IDENTITY(1,1) PRIMARY KEY,
            post_id BIGINT NOT NULL,
            category_id BIGINT NOT NULL,
            created_at DATETIME2 NOT NULL CONSTRAINT df_post_categories_created_at DEFAULT GETDATE(),
            CONSTRAINT fk_post_categories_post FOREIGN KEY (post_id) REFERENCES dbo.posts(id),
            CONSTRAINT fk_post_categories_category FOREIGN KEY (category_id) REFERENCES dbo.categories(id),
            CONSTRAINT uq_post_categories UNIQUE (post_id, category_id)
        );

        CREATE INDEX ix_post_categories_post_id ON dbo.post_categories(post_id);
        CREATE INDEX ix_post_categories_category_id ON dbo.post_categories(category_id);
    END;

    ------------------------------------------------------------
    -- 2) EXAM RESULT + CERTIFICATE TABLES
    ------------------------------------------------------------
    IF OBJECT_ID('dbo.exam_results', 'U') IS NULL
    BEGIN
        CREATE TABLE dbo.exam_results
        (
            id BIGINT IDENTITY(1,1) PRIMARY KEY,
            bai_thi_id BIGINT NOT NULL,
            hoc_vien_id BIGINT NOT NULL,
            tong_so_cau INT NOT NULL,
            so_cau_dung INT NOT NULL,
            diem DECIMAL(5,2) NOT NULL,
            ket_qua VARCHAR(20) NOT NULL,
            xac_nhan_boi BIGINT NULL,
            xac_nhan_luc DATETIME2 NULL,
            created_at DATETIME2 NOT NULL CONSTRAINT df_exam_results_created_at DEFAULT GETDATE(),
            updated_at DATETIME2 NOT NULL CONSTRAINT df_exam_results_updated_at DEFAULT GETDATE(),
            CONSTRAINT uq_exam_results_bai_thi_id UNIQUE (bai_thi_id),
            CONSTRAINT fk_exam_results_bai_thi FOREIGN KEY (bai_thi_id) REFERENCES dbo.bai_thi(id),
            CONSTRAINT fk_exam_results_hoc_vien FOREIGN KEY (hoc_vien_id) REFERENCES dbo.hoc_vien(id),
            CONSTRAINT fk_exam_results_xac_nhan_boi FOREIGN KEY (xac_nhan_boi) REFERENCES dbo.nguoi_dung(id),
            CONSTRAINT ck_exam_results_tong_so_cau CHECK (tong_so_cau >= 0),
            CONSTRAINT ck_exam_results_so_cau_dung CHECK (so_cau_dung >= 0),
            CONSTRAINT ck_exam_results_diem CHECK (diem >= 0),
            CONSTRAINT ck_exam_results_ket_qua CHECK (ket_qua IN ('dat','khong_dat')),
            CONSTRAINT ck_exam_results_so_cau CHECK (so_cau_dung <= tong_so_cau)
        );

        CREATE INDEX ix_exam_results_hoc_vien_id ON dbo.exam_results(hoc_vien_id);
        CREATE INDEX ix_exam_results_ket_qua ON dbo.exam_results(ket_qua);
        CREATE INDEX ix_exam_results_xac_nhan_luc ON dbo.exam_results(xac_nhan_luc);
    END;

    IF OBJECT_ID('dbo.certificates', 'U') IS NULL
    BEGIN
        CREATE TABLE dbo.certificates
        (
            id BIGINT IDENTITY(1,1) PRIMARY KEY,
            ma_chung_chi VARCHAR(50) NOT NULL,
            hoc_vien_id BIGINT NOT NULL,
            exam_result_id BIGINT NOT NULL,
            ngay_cap DATETIME2 NOT NULL,
            ngay_het_han DATETIME2 NULL,
            trang_thai VARCHAR(30) NOT NULL,
            certificate_file_id BIGINT NULL,
            created_by BIGINT NULL,
            created_at DATETIME2 NOT NULL CONSTRAINT df_certificates_created_at DEFAULT GETDATE(),
            updated_at DATETIME2 NOT NULL CONSTRAINT df_certificates_updated_at DEFAULT GETDATE(),
            CONSTRAINT uq_certificates_ma_chung_chi UNIQUE (ma_chung_chi),
            CONSTRAINT uq_certificates_exam_result_id UNIQUE (exam_result_id),
            CONSTRAINT fk_certificates_hoc_vien FOREIGN KEY (hoc_vien_id) REFERENCES dbo.hoc_vien(id),
            CONSTRAINT fk_certificates_exam_result FOREIGN KEY (exam_result_id) REFERENCES dbo.exam_results(id),
            CONSTRAINT fk_certificates_file FOREIGN KEY (certificate_file_id) REFERENCES dbo.files(id),
            CONSTRAINT fk_certificates_created_by FOREIGN KEY (created_by) REFERENCES dbo.nguoi_dung(id),
            CONSTRAINT ck_certificates_trang_thai CHECK (trang_thai IN ('valid','revoked','expired')),
            CONSTRAINT ck_certificates_ngay CHECK (ngay_het_han IS NULL OR ngay_het_han >= ngay_cap)
        );

        CREATE INDEX ix_certificates_hoc_vien_id ON dbo.certificates(hoc_vien_id);
        CREATE INDEX ix_certificates_trang_thai ON dbo.certificates(trang_thai);
        CREATE INDEX ix_certificates_ngay_cap ON dbo.certificates(ngay_cap);
    END;

    ------------------------------------------------------------
    -- 3) BACKFILL exam_results tu bai_thi da nop
    ------------------------------------------------------------
    INSERT INTO dbo.exam_results
    (
        bai_thi_id,
        hoc_vien_id,
        tong_so_cau,
        so_cau_dung,
        diem,
        ket_qua,
        xac_nhan_boi,
        xac_nhan_luc,
        created_at,
        updated_at
    )
    SELECT
        bt.id,
        bt.hoc_vien_id,
        bt.tong_so_cau,
        bt.so_cau_dung,
        bt.diem,
        CASE
            WHEN bt.ket_qua = 'dat' THEN 'dat'
            ELSE 'khong_dat'
        END,
        NULL,
        NULL,
        GETDATE(),
        GETDATE()
    FROM dbo.bai_thi bt
    WHERE bt.trang_thai IN ('da_nop','da_cham','hoan_thanh')
      AND NOT EXISTS
      (
          SELECT 1
          FROM dbo.exam_results er
          WHERE er.bai_thi_id = bt.id
      );

    COMMIT TRANSACTION;
    PRINT N'OK: 04_add_cms_and_certificate.sql completed.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    DECLARE @ErrMsg NVARCHAR(4000) = ERROR_MESSAGE();
    DECLARE @ErrLine INT = ERROR_LINE();
    DECLARE @ErrNum INT = ERROR_NUMBER();

    RAISERROR(N'[04_add_cms_and_certificate.sql] Error %d at line %d: %s', 16, 1, @ErrNum, @ErrLine, @ErrMsg);
END CATCH;
GO
