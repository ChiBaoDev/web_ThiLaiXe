-- ================================
-- 1. TAO DATABASE
-- ================================
CREATE DATABASE he_thong_thi_bang_lai;
GO

USE he_thong_thi_bang_lai;
GO

-- ================================
-- 2. NHOM TAI KHOAN VA PHAN QUYEN
-- ================================

CREATE TABLE nguoi_dung (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    ten_dang_nhap VARCHAR(50) NOT NULL,
    mat_khau_hash VARCHAR(255) NOT NULL,
    email VARCHAR(100) NOT NULL,
    so_dien_thoai VARCHAR(20) NULL,
    trang_thai VARCHAR(30) NOT NULL DEFAULT 'hoat_dong',
    lan_dang_nhap_cuoi DATETIME2 NULL,
    created_at DATETIME2 NOT NULL DEFAULT GETDATE(),
    updated_at DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT uq_nguoi_dung_ten_dang_nhap UNIQUE (ten_dang_nhap),
    CONSTRAINT uq_nguoi_dung_email UNIQUE (email)
);
GO

CREATE TABLE vai_tro (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    ma_vai_tro VARCHAR(30) NOT NULL,
    ten_vai_tro NVARCHAR(100) NOT NULL,
    mo_ta NVARCHAR(255) NULL,

    CONSTRAINT uq_vai_tro_ma_vai_tro UNIQUE (ma_vai_tro)
);
GO

CREATE TABLE quyen_han (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    ma_quyen VARCHAR(50) NOT NULL,
    ten_quyen NVARCHAR(100) NOT NULL,
    mo_ta NVARCHAR(255) NULL,

    CONSTRAINT uq_quyen_han_ma_quyen UNIQUE (ma_quyen)
);
GO

CREATE TABLE nguoi_dung_vai_tro (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    nguoi_dung_id BIGINT NOT NULL,
    vai_tro_id BIGINT NOT NULL,

    CONSTRAINT fk_ndvt_nguoi_dung FOREIGN KEY (nguoi_dung_id) REFERENCES nguoi_dung(id),
    CONSTRAINT fk_ndvt_vai_tro FOREIGN KEY (vai_tro_id) REFERENCES vai_tro(id),
    CONSTRAINT uq_ndvt UNIQUE (nguoi_dung_id, vai_tro_id)
);
GO

CREATE TABLE vai_tro_quyen_han (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    vai_tro_id BIGINT NOT NULL,
    quyen_han_id BIGINT NOT NULL,

    CONSTRAINT fk_vtqh_vai_tro FOREIGN KEY (vai_tro_id) REFERENCES vai_tro(id),
    CONSTRAINT fk_vtqh_quyen_han FOREIGN KEY (quyen_han_id) REFERENCES quyen_han(id),
    CONSTRAINT uq_vtqh UNIQUE (vai_tro_id, quyen_han_id)
);
GO

CREATE TABLE nhat_ky_he_thong (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    nguoi_dung_id BIGINT NULL,
    hanh_dong NVARCHAR(100) NOT NULL,
    bang_tac_dong VARCHAR(100) NULL,
    khoa_chinh_du_lieu BIGINT NULL,
    noi_dung NVARCHAR(MAX) NULL,
    ip_address VARCHAR(45) NULL,
    created_at DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT fk_nhat_ky_nguoi_dung FOREIGN KEY (nguoi_dung_id) REFERENCES nguoi_dung(id)
);
GO

-- ================================
-- 3. NHOM HO SO HOC VIEN
-- ================================

CREATE TABLE hoc_vien (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    nguoi_dung_id BIGINT NOT NULL,
    ho_ten NVARCHAR(150) NOT NULL,
    ngay_sinh DATE NULL,
    gioi_tinh NVARCHAR(10) NULL,
    cccd VARCHAR(20) NULL,
    dia_chi NVARCHAR(255) NULL,
    anh_chan_dung VARCHAR(255) NULL,
    created_at DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT fk_hoc_vien_nguoi_dung FOREIGN KEY (nguoi_dung_id) REFERENCES nguoi_dung(id),
    CONSTRAINT uq_hoc_vien_nguoi_dung UNIQUE (nguoi_dung_id),
    CONSTRAINT uq_hoc_vien_cccd UNIQUE (cccd)
);
GO

CREATE TABLE ho_so_dang_ky (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    hoc_vien_id BIGINT NOT NULL,
    ma_ho_so VARCHAR(30) NOT NULL,
    ngay_nop DATETIME2 NULL,
    trang_thai VARCHAR(30) NOT NULL DEFAULT 'cho_nop',
    ghi_chu NVARCHAR(500) NULL,
    nguoi_duyet_id BIGINT NULL,
    ngay_duyet DATETIME2 NULL,

    CONSTRAINT fk_ho_so_hoc_vien FOREIGN KEY (hoc_vien_id) REFERENCES hoc_vien(id),
    CONSTRAINT fk_ho_so_nguoi_duyet FOREIGN KEY (nguoi_duyet_id) REFERENCES nguoi_dung(id),
    CONSTRAINT uq_ho_so_ma_ho_so UNIQUE (ma_ho_so)
);
GO

CREATE TABLE giay_to_dinh_kem (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    ho_so_id BIGINT NOT NULL,
    ten_giay_to NVARCHAR(150) NOT NULL,
    duong_dan_file VARCHAR(255) NOT NULL,
    loai_file VARCHAR(20) NULL,
    ngay_tai_len DATETIME2 NOT NULL DEFAULT GETDATE(),
    trang_thai VARCHAR(30) NOT NULL DEFAULT 'hop_le',

    CONSTRAINT fk_giay_to_ho_so FOREIGN KEY (ho_so_id) REFERENCES ho_so_dang_ky(id)
);
GO

-- ================================
-- 4. NHOM DAO TAO
-- ================================

CREATE TABLE khoa_hoc (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    ma_khoa_hoc VARCHAR(30) NOT NULL,
    ten_khoa_hoc NVARCHAR(150) NOT NULL,
    mo_ta NVARCHAR(500) NULL,
    hoc_phi DECIMAL(18,2) NOT NULL DEFAULT 0,
    thoi_luong INT NULL,
    trang_thai VARCHAR(30) NOT NULL DEFAULT 'dang_mo',

    CONSTRAINT uq_khoa_hoc_ma UNIQUE (ma_khoa_hoc),
    CONSTRAINT ck_khoa_hoc_hoc_phi CHECK (hoc_phi >= 0),
    CONSTRAINT ck_khoa_hoc_thoi_luong CHECK (thoi_luong IS NULL OR thoi_luong > 0)
);
GO

CREATE TABLE lop_hoc (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    khoa_hoc_id BIGINT NOT NULL,
    ma_lop VARCHAR(30) NOT NULL,
    ten_lop NVARCHAR(150) NOT NULL,
    giao_vien_id BIGINT NULL,
    ngay_bat_dau DATE NULL,
    ngay_ket_thuc DATE NULL,
    si_so_toi_da INT NOT NULL DEFAULT 0,
    trang_thai VARCHAR(30) NOT NULL DEFAULT 'dang_mo',

    CONSTRAINT fk_lop_hoc_khoa_hoc FOREIGN KEY (khoa_hoc_id) REFERENCES khoa_hoc(id),
    CONSTRAINT fk_lop_hoc_giao_vien FOREIGN KEY (giao_vien_id) REFERENCES nguoi_dung(id),
    CONSTRAINT uq_lop_hoc_ma UNIQUE (ma_lop),
    CONSTRAINT ck_lop_hoc_si_so CHECK (si_so_toi_da >= 0),
    CONSTRAINT ck_lop_hoc_ngay CHECK (ngay_ket_thuc IS NULL OR ngay_bat_dau IS NULL OR ngay_ket_thuc >= ngay_bat_dau)
);
GO

CREATE TABLE dang_ky_khoa_hoc (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    hoc_vien_id BIGINT NOT NULL,
    khoa_hoc_id BIGINT NOT NULL,
    ngay_dang_ky DATETIME2 NOT NULL DEFAULT GETDATE(),
    trang_thai VARCHAR(30) NOT NULL DEFAULT 'cho_duyet',
    nguoi_duyet_id BIGINT NULL,
    ngay_duyet DATETIME2 NULL,

    CONSTRAINT fk_dk_khoa_hoc_hoc_vien FOREIGN KEY (hoc_vien_id) REFERENCES hoc_vien(id),
    CONSTRAINT fk_dk_khoa_hoc_khoa_hoc FOREIGN KEY (khoa_hoc_id) REFERENCES khoa_hoc(id),
    CONSTRAINT fk_dk_khoa_hoc_nguoi_duyet FOREIGN KEY (nguoi_duyet_id) REFERENCES nguoi_dung(id),
    CONSTRAINT uq_dang_ky_khoa_hoc UNIQUE (hoc_vien_id, khoa_hoc_id)
);
GO

CREATE TABLE buoi_hoc (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    lop_hoc_id BIGINT NOT NULL,
    ten_buoi NVARCHAR(150) NOT NULL,
    ngay_hoc DATE NOT NULL,
    gio_bat_dau TIME NOT NULL,
    gio_ket_thuc TIME NOT NULL,
    noi_dung NVARCHAR(500) NULL,
    phong_hoc NVARCHAR(100) NULL,

    CONSTRAINT fk_buoi_hoc_lop_hoc FOREIGN KEY (lop_hoc_id) REFERENCES lop_hoc(id),
    CONSTRAINT ck_buoi_hoc_gio CHECK (gio_ket_thuc > gio_bat_dau)
);
GO

CREATE TABLE lop_hoc_hoc_vien (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    lop_hoc_id BIGINT NOT NULL,
    hoc_vien_id BIGINT NOT NULL,
    ngay_vao_lop DATE NULL,
    trang_thai VARCHAR(30) NOT NULL DEFAULT 'dang_hoc',

    CONSTRAINT fk_lhhv_lop_hoc FOREIGN KEY (lop_hoc_id) REFERENCES lop_hoc(id),
    CONSTRAINT fk_lhhv_hoc_vien FOREIGN KEY (hoc_vien_id) REFERENCES hoc_vien(id),
    CONSTRAINT uq_lop_hoc_hoc_vien UNIQUE (lop_hoc_id, hoc_vien_id)
);
GO

CREATE TABLE diem_danh (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    buoi_hoc_id BIGINT NOT NULL,
    hoc_vien_id BIGINT NOT NULL,
    trang_thai VARCHAR(30) NOT NULL,
    ghi_chu NVARCHAR(255) NULL,
    giao_vien_id BIGINT NULL,
    thoi_gian_diem_danh DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT fk_diem_danh_buoi_hoc FOREIGN KEY (buoi_hoc_id) REFERENCES buoi_hoc(id),
    CONSTRAINT fk_diem_danh_hoc_vien FOREIGN KEY (hoc_vien_id) REFERENCES hoc_vien(id),
    CONSTRAINT fk_diem_danh_giao_vien FOREIGN KEY (giao_vien_id) REFERENCES nguoi_dung(id),
    CONSTRAINT uq_diem_danh UNIQUE (buoi_hoc_id, hoc_vien_id)
);
GO

-- ================================
-- 5. NHOM NGAN HANG CAU HOI
-- ================================

CREATE TABLE chu_de_cau_hoi (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    ma_chu_de VARCHAR(30) NOT NULL,
    ten_chu_de NVARCHAR(150) NOT NULL,
    mo_ta NVARCHAR(255) NULL,

    CONSTRAINT uq_chu_de_cau_hoi_ma UNIQUE (ma_chu_de)
);
GO

CREATE TABLE cau_hoi (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    chu_de_id BIGINT NOT NULL,
    noi_dung NVARCHAR(MAX) NOT NULL,
    loai_cau_hoi VARCHAR(30) NOT NULL DEFAULT 'trac_nghiem',
    muc_do VARCHAR(30) NULL,
    la_cau_diem_liet BIT NOT NULL DEFAULT 0,
    trang_thai VARCHAR(30) NOT NULL DEFAULT 'hoat_dong',

    CONSTRAINT fk_cau_hoi_chu_de FOREIGN KEY (chu_de_id) REFERENCES chu_de_cau_hoi(id)
);
GO

CREATE TABLE dap_an (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    cau_hoi_id BIGINT NOT NULL,
    noi_dung NVARCHAR(1000) NOT NULL,
    la_dap_an_dung BIT NOT NULL DEFAULT 0,
    thu_tu INT NOT NULL,

    CONSTRAINT fk_dap_an_cau_hoi FOREIGN KEY (cau_hoi_id) REFERENCES cau_hoi(id),
    CONSTRAINT uq_dap_an_thu_tu UNIQUE (cau_hoi_id, thu_tu)
);
GO

-- ================================
-- 6. NHOM ON TAP
-- ================================

CREATE TABLE phien_on_tap (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    hoc_vien_id BIGINT NOT NULL,
    ngay_tao DATETIME2 NOT NULL DEFAULT GETDATE(),
    thoi_gian_bat_dau DATETIME2 NULL,
    thoi_gian_nop DATETIME2 NULL,
    tong_so_cau INT NOT NULL DEFAULT 0,
    so_cau_dung INT NOT NULL DEFAULT 0,
    diem DECIMAL(5,2) NOT NULL DEFAULT 0,
    trang_thai VARCHAR(30) NOT NULL DEFAULT 'moi_tao',

    CONSTRAINT fk_phien_on_tap_hoc_vien FOREIGN KEY (hoc_vien_id) REFERENCES hoc_vien(id),
    CONSTRAINT ck_phien_on_tap_tong_so_cau CHECK (tong_so_cau >= 0),
    CONSTRAINT ck_phien_on_tap_so_cau_dung CHECK (so_cau_dung >= 0),
    CONSTRAINT ck_phien_on_tap_diem CHECK (diem >= 0)
);
GO

CREATE TABLE phien_on_tap_cau_hoi (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    phien_on_tap_id BIGINT NOT NULL,
    cau_hoi_id BIGINT NOT NULL,
    dap_an_chon_id BIGINT NULL,
    la_dung BIT NULL,
    thu_tu_cau INT NOT NULL,

    CONSTRAINT fk_pot_ch_phien_on_tap FOREIGN KEY (phien_on_tap_id) REFERENCES phien_on_tap(id),
    CONSTRAINT fk_pot_ch_cau_hoi FOREIGN KEY (cau_hoi_id) REFERENCES cau_hoi(id),
    CONSTRAINT fk_pot_ch_dap_an FOREIGN KEY (dap_an_chon_id) REFERENCES dap_an(id),
    CONSTRAINT uq_phien_on_tap_cau_hoi UNIQUE (phien_on_tap_id, cau_hoi_id),
    CONSTRAINT uq_phien_on_tap_thu_tu UNIQUE (phien_on_tap_id, thu_tu_cau)
);
GO

-- ================================
-- 7. NHOM THI CU
-- ================================

CREATE TABLE ky_thi (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    ma_ky_thi VARCHAR(30) NOT NULL,
    ten_ky_thi NVARCHAR(150) NOT NULL,
    ngay_thi DATE NOT NULL,
    mo_ta NVARCHAR(255) NULL,
    trang_thai VARCHAR(30) NOT NULL DEFAULT 'sap_dien_ra',

    CONSTRAINT uq_ky_thi_ma UNIQUE (ma_ky_thi)
);
GO

CREATE TABLE ca_thi (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    ky_thi_id BIGINT NOT NULL,
    ma_ca_thi VARCHAR(30) NOT NULL,
    ten_ca_thi NVARCHAR(150) NOT NULL,
    gio_bat_dau TIME NOT NULL,
    gio_ket_thuc TIME NOT NULL,
    phong_thi NVARCHAR(100) NULL,
    so_luong_toi_da INT NOT NULL DEFAULT 0,

    CONSTRAINT fk_ca_thi_ky_thi FOREIGN KEY (ky_thi_id) REFERENCES ky_thi(id),
    CONSTRAINT uq_ca_thi_ma UNIQUE (ma_ca_thi),
    CONSTRAINT ck_ca_thi_gio CHECK (gio_ket_thuc > gio_bat_dau),
    CONSTRAINT ck_ca_thi_so_luong CHECK (so_luong_toi_da >= 0)
);
GO

CREATE TABLE dang_ky_du_thi (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    hoc_vien_id BIGINT NOT NULL,
    ca_thi_id BIGINT NOT NULL,
    ngay_dang_ky DATETIME2 NOT NULL DEFAULT GETDATE(),
    trang_thai VARCHAR(30) NOT NULL DEFAULT 'cho_duyet',
    nguoi_duyet_id BIGINT NULL,
    ngay_duyet DATETIME2 NULL,

    CONSTRAINT fk_dkdt_hoc_vien FOREIGN KEY (hoc_vien_id) REFERENCES hoc_vien(id),
    CONSTRAINT fk_dkdt_ca_thi FOREIGN KEY (ca_thi_id) REFERENCES ca_thi(id),
    CONSTRAINT fk_dkdt_nguoi_duyet FOREIGN KEY (nguoi_duyet_id) REFERENCES nguoi_dung(id),
    CONSTRAINT uq_dang_ky_du_thi UNIQUE (hoc_vien_id, ca_thi_id)
);
GO

CREATE TABLE de_thi (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    ma_de_thi VARCHAR(30) NOT NULL,
    ten_de_thi NVARCHAR(150) NOT NULL,
    ky_thi_id BIGINT NOT NULL,
    tong_so_cau INT NOT NULL DEFAULT 0,
    thoi_gian_lam_bai INT NOT NULL,
    trang_thai VARCHAR(30) NOT NULL DEFAULT 'nhap',
    nguoi_tao_id BIGINT NULL,
    ngay_tao DATETIME2 NOT NULL DEFAULT GETDATE(),

    CONSTRAINT fk_de_thi_ky_thi FOREIGN KEY (ky_thi_id) REFERENCES ky_thi(id),
    CONSTRAINT fk_de_thi_nguoi_tao FOREIGN KEY (nguoi_tao_id) REFERENCES nguoi_dung(id),
    CONSTRAINT uq_de_thi_ma UNIQUE (ma_de_thi),
    CONSTRAINT ck_de_thi_tong_so_cau CHECK (tong_so_cau >= 0),
    CONSTRAINT ck_de_thi_thoi_gian CHECK (thoi_gian_lam_bai > 0)
);
GO

CREATE TABLE de_thi_cau_hoi (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    de_thi_id BIGINT NOT NULL,
    cau_hoi_id BIGINT NOT NULL,
    thu_tu_cau INT NOT NULL,

    CONSTRAINT fk_dtch_de_thi FOREIGN KEY (de_thi_id) REFERENCES de_thi(id),
    CONSTRAINT fk_dtch_cau_hoi FOREIGN KEY (cau_hoi_id) REFERENCES cau_hoi(id),
    CONSTRAINT uq_de_thi_cau_hoi UNIQUE (de_thi_id, cau_hoi_id),
    CONSTRAINT uq_de_thi_thu_tu UNIQUE (de_thi_id, thu_tu_cau)
);
GO

CREATE TABLE bai_thi (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    hoc_vien_id BIGINT NOT NULL,
    de_thi_id BIGINT NOT NULL,
    ca_thi_id BIGINT NOT NULL,
    thoi_gian_bat_dau DATETIME2 NULL,
    thoi_gian_nop DATETIME2 NULL,
    tong_so_cau INT NOT NULL DEFAULT 0,
    so_cau_dung INT NOT NULL DEFAULT 0,
    diem DECIMAL(5,2) NOT NULL DEFAULT 0,
    ket_qua VARCHAR(20) NULL,
    trang_thai VARCHAR(30) NOT NULL DEFAULT 'chua_lam',

    CONSTRAINT fk_bai_thi_hoc_vien FOREIGN KEY (hoc_vien_id) REFERENCES hoc_vien(id),
    CONSTRAINT fk_bai_thi_de_thi FOREIGN KEY (de_thi_id) REFERENCES de_thi(id),
    CONSTRAINT fk_bai_thi_ca_thi FOREIGN KEY (ca_thi_id) REFERENCES ca_thi(id),
    CONSTRAINT ck_bai_thi_tong_so_cau CHECK (tong_so_cau >= 0),
    CONSTRAINT ck_bai_thi_so_cau_dung CHECK (so_cau_dung >= 0),
    CONSTRAINT ck_bai_thi_diem CHECK (diem >= 0)
);
GO

CREATE TABLE chi_tiet_bai_thi (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    bai_thi_id BIGINT NOT NULL,
    cau_hoi_id BIGINT NOT NULL,
    dap_an_chon_id BIGINT NULL,
    la_dung BIT NULL,

    CONSTRAINT fk_ctbt_bai_thi FOREIGN KEY (bai_thi_id) REFERENCES bai_thi(id),
    CONSTRAINT fk_ctbt_cau_hoi FOREIGN KEY (cau_hoi_id) REFERENCES cau_hoi(id),
    CONSTRAINT fk_ctbt_dap_an FOREIGN KEY (dap_an_chon_id) REFERENCES dap_an(id),
    CONSTRAINT uq_ctbt UNIQUE (bai_thi_id, cau_hoi_id)
);
GO

-- ================================
-- 8. NHOM TAI CHINH
-- ================================

CREATE TABLE loai_khoan_thu (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    ma_loai VARCHAR(30) NOT NULL,
    ten_loai NVARCHAR(150) NOT NULL,
    so_tien_mac_dinh DECIMAL(18,2) NOT NULL DEFAULT 0,
    mo_ta NVARCHAR(255) NULL,
    trang_thai VARCHAR(30) NOT NULL DEFAULT 'hoat_dong',

    CONSTRAINT uq_loai_khoan_thu_ma UNIQUE (ma_loai),
    CONSTRAINT ck_loai_khoan_thu_so_tien CHECK (so_tien_mac_dinh >= 0)
);
GO

CREATE TABLE phieu_thu (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    ma_phieu_thu VARCHAR(30) NOT NULL,
    hoc_vien_id BIGINT NOT NULL,
    ngay_thu DATETIME2 NOT NULL DEFAULT GETDATE(),
    tong_tien DECIMAL(18,2) NOT NULL DEFAULT 0,
    trang_thai VARCHAR(30) NOT NULL DEFAULT 'cho_xac_nhan',
    nguoi_lap_id BIGINT NULL,
    nguoi_xac_nhan_id BIGINT NULL,

    CONSTRAINT fk_phieu_thu_hoc_vien FOREIGN KEY (hoc_vien_id) REFERENCES hoc_vien(id),
    CONSTRAINT fk_phieu_thu_nguoi_lap FOREIGN KEY (nguoi_lap_id) REFERENCES nguoi_dung(id),
    CONSTRAINT fk_phieu_thu_nguoi_xac_nhan FOREIGN KEY (nguoi_xac_nhan_id) REFERENCES nguoi_dung(id),
    CONSTRAINT uq_phieu_thu_ma UNIQUE (ma_phieu_thu),
    CONSTRAINT ck_phieu_thu_tong_tien CHECK (tong_tien >= 0)
);
GO

CREATE TABLE chi_tiet_phieu_thu (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    phieu_thu_id BIGINT NOT NULL,
    loai_khoan_thu_id BIGINT NOT NULL,
    so_tien DECIMAL(18,2) NOT NULL,
    ghi_chu NVARCHAR(255) NULL,

    CONSTRAINT fk_ctpt_phieu_thu FOREIGN KEY (phieu_thu_id) REFERENCES phieu_thu(id),
    CONSTRAINT fk_ctpt_loai_khoan_thu FOREIGN KEY (loai_khoan_thu_id) REFERENCES loai_khoan_thu(id),
    CONSTRAINT ck_ctpt_so_tien CHECK (so_tien >= 0)
);
GO

-- ================================
-- 9. NHOM VI PHAM QUY CHE
-- ================================

CREATE TABLE loai_vi_pham (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    ma_loai VARCHAR(30) NOT NULL,
    ten_loai NVARCHAR(150) NOT NULL,
    mo_ta NVARCHAR(255) NULL,
    muc_xu_ly_mac_dinh NVARCHAR(255) NULL,

    CONSTRAINT uq_loai_vi_pham_ma UNIQUE (ma_loai)
);
GO

CREATE TABLE vi_pham_quy_che (
    id BIGINT IDENTITY(1,1) PRIMARY KEY,
    hoc_vien_id BIGINT NOT NULL,
    bai_thi_id BIGINT NULL,
    loai_vi_pham_id BIGINT NOT NULL,
    nguoi_ghi_nhan_id BIGINT NULL,
    thoi_gian_vi_pham DATETIME2 NOT NULL DEFAULT GETDATE(),
    mo_ta NVARCHAR(500) NULL,
    hinh_thuc_xu_ly NVARCHAR(255) NULL,

    CONSTRAINT fk_vpqc_hoc_vien FOREIGN KEY (hoc_vien_id) REFERENCES hoc_vien(id),
    CONSTRAINT fk_vpqc_bai_thi FOREIGN KEY (bai_thi_id) REFERENCES bai_thi(id),
    CONSTRAINT fk_vpqc_loai_vi_pham FOREIGN KEY (loai_vi_pham_id) REFERENCES loai_vi_pham(id),
    CONSTRAINT fk_vpqc_nguoi_ghi_nhan FOREIGN KEY (nguoi_ghi_nhan_id) REFERENCES nguoi_dung(id)
);
GO

-- ================================
-- 10. INDEX GOI Y
-- ================================

CREATE INDEX ix_hoc_vien_nguoi_dung_id ON hoc_vien(nguoi_dung_id);
CREATE INDEX ix_ho_so_dang_ky_hoc_vien_id ON ho_so_dang_ky(hoc_vien_id);
CREATE INDEX ix_lop_hoc_khoa_hoc_id ON lop_hoc(khoa_hoc_id);
CREATE INDEX ix_buoi_hoc_lop_hoc_id ON buoi_hoc(lop_hoc_id);
CREATE INDEX ix_diem_danh_buoi_hoc_id ON diem_danh(buoi_hoc_id);
CREATE INDEX ix_cau_hoi_chu_de_id ON cau_hoi(chu_de_id);
CREATE INDEX ix_dap_an_cau_hoi_id ON dap_an(cau_hoi_id);
CREATE INDEX ix_phien_on_tap_hoc_vien_id ON phien_on_tap(hoc_vien_id);
CREATE INDEX ix_ky_thi_ngay_thi ON ky_thi(ngay_thi);
CREATE INDEX ix_ca_thi_ky_thi_id ON ca_thi(ky_thi_id);
CREATE INDEX ix_bai_thi_hoc_vien_id ON bai_thi(hoc_vien_id);
CREATE INDEX ix_bai_thi_de_thi_id ON bai_thi(de_thi_id);
CREATE INDEX ix_phieu_thu_hoc_vien_id ON phieu_thu(hoc_vien_id);
CREATE INDEX ix_vi_pham_hoc_vien_id ON vi_pham_quy_che(hoc_vien_id);
GO