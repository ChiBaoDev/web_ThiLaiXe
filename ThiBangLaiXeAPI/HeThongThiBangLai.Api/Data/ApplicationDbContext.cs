using System;
using System.Collections.Generic;
using HeThongThiBangLai.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace HeThongThiBangLai.Api.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<bai_thi> bai_this { get; set; }

    public virtual DbSet<buoi_hoc> buoi_hocs { get; set; }

    public virtual DbSet<ca_thi> ca_this { get; set; }

    public virtual DbSet<cau_hoi> cau_hois { get; set; }

    public virtual DbSet<chi_tiet_bai_thi> chi_tiet_bai_this { get; set; }

    public virtual DbSet<chi_tiet_phieu_thu> chi_tiet_phieu_thus { get; set; }

    public virtual DbSet<chu_de_cau_hoi> chu_de_cau_hois { get; set; }

    public virtual DbSet<dang_ky_du_thi> dang_ky_du_this { get; set; }

    public virtual DbSet<dang_ky_khoa_hoc> dang_ky_khoa_hocs { get; set; }

    public virtual DbSet<dap_an> dap_ans { get; set; }

    public virtual DbSet<de_thi> de_this { get; set; }

    public virtual DbSet<de_thi_cau_hoi> de_thi_cau_hois { get; set; }

    public virtual DbSet<diem_danh> diem_danhs { get; set; }

    public virtual DbSet<giay_to_dinh_kem> giay_to_dinh_kems { get; set; }

    public virtual DbSet<ho_so_dang_ky> ho_so_dang_kies { get; set; }

    public virtual DbSet<hoc_vien> hoc_viens { get; set; }

    public virtual DbSet<khoa_hoc> khoa_hocs { get; set; }

    public virtual DbSet<ky_thi> ky_this { get; set; }

    public virtual DbSet<loai_nguoi_dung> loai_nguoi_dungs { get; set; }

    public virtual DbSet<loai_khoan_thu> loai_khoan_thus { get; set; }

    public virtual DbSet<loai_vi_pham> loai_vi_phams { get; set; }

    public virtual DbSet<lop_hoc> lop_hocs { get; set; }

    public virtual DbSet<lop_hoc_hoc_vien> lop_hoc_hoc_viens { get; set; }

    public virtual DbSet<nguoi_dung> nguoi_dungs { get; set; }

    public virtual DbSet<nguoi_dung_loai> nguoi_dung_loais { get; set; }

    public virtual DbSet<nguoi_dung_vai_tro> nguoi_dung_vai_tros { get; set; }

    public virtual DbSet<nhat_ky_he_thong> nhat_ky_he_thongs { get; set; }

    public virtual DbSet<goi_quyen> goi_quyens { get; set; }

    public virtual DbSet<quyen_su_dung> quyen_su_dungs { get; set; }

    public virtual DbSet<files> files { get; set; }

    public virtual DbSet<file_usages> file_usages { get; set; }

    public virtual DbSet<categories> categories { get; set; }

    public virtual DbSet<posts> posts { get; set; }

    public virtual DbSet<post_categories> post_categories { get; set; }

    public virtual DbSet<exam_results> exam_results { get; set; }

    public virtual DbSet<certificates> certificates { get; set; }

    public virtual DbSet<phien_on_tap> phien_on_taps { get; set; }

    public virtual DbSet<phien_on_tap_cau_hoi> phien_on_tap_cau_hois { get; set; }

    public virtual DbSet<phieu_thu> phieu_thus { get; set; }

    public virtual DbSet<quyen_han> quyen_hans { get; set; }

    public virtual DbSet<vai_tro> vai_tros { get; set; }

    public virtual DbSet<vai_tro_quyen_han> vai_tro_quyen_hans { get; set; }

    public virtual DbSet<vi_pham_quy_che> vi_pham_quy_ches { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<bai_thi>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__bai_thi__3213E83FFC7C9EE0");

            entity.ToTable("bai_thi");

            entity.HasIndex(e => e.de_thi_id, "ix_bai_thi_de_thi_id");

            entity.HasIndex(e => e.hoc_vien_id, "ix_bai_thi_hoc_vien_id");

            entity.Property(e => e.diem).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ket_qua)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.trang_thai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("chua_lam");

            entity.HasOne(d => d.ca_thi).WithMany(p => p.bai_this)
                .HasForeignKey(d => d.ca_thi_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bai_thi_ca_thi");

            entity.HasOne(d => d.de_thi).WithMany(p => p.bai_this)
                .HasForeignKey(d => d.de_thi_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bai_thi_de_thi");

            entity.HasOne(d => d.hoc_vien).WithMany(p => p.bai_this)
                .HasForeignKey(d => d.hoc_vien_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_bai_thi_hoc_vien");
        });

        modelBuilder.Entity<buoi_hoc>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__buoi_hoc__3213E83F6B1FF810");

            entity.ToTable("buoi_hoc");

            entity.HasIndex(e => e.lop_hoc_id, "ix_buoi_hoc_lop_hoc_id");

            entity.Property(e => e.noi_dung).HasMaxLength(500);
            entity.Property(e => e.phong_hoc).HasMaxLength(100);
            entity.Property(e => e.ten_buoi).HasMaxLength(150);

            entity.HasOne(d => d.lop_hoc).WithMany(p => p.buoi_hocs)
                .HasForeignKey(d => d.lop_hoc_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_buoi_hoc_lop_hoc");
        });

        modelBuilder.Entity<ca_thi>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__ca_thi__3213E83F9E612AC3");

            entity.ToTable("ca_thi");

            entity.HasIndex(e => e.ky_thi_id, "ix_ca_thi_ky_thi_id");

            entity.HasIndex(e => e.ma_ca_thi, "uq_ca_thi_ma").IsUnique();

            entity.Property(e => e.ma_ca_thi)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.phong_thi).HasMaxLength(100);
            entity.Property(e => e.ten_ca_thi).HasMaxLength(150);

            entity.HasOne(d => d.ky_thi).WithMany(p => p.ca_this)
                .HasForeignKey(d => d.ky_thi_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ca_thi_ky_thi");
        });

        modelBuilder.Entity<cau_hoi>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__cau_hoi__3213E83F29EA0D9D");

            entity.ToTable("cau_hoi");

            entity.HasIndex(e => e.chu_de_id, "ix_cau_hoi_chu_de_id");

            entity.Property(e => e.loai_cau_hoi)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("trac_nghiem");
            entity.Property(e => e.muc_do)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.trang_thai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("hoat_dong");

            entity.HasOne(d => d.chu_de).WithMany(p => p.cau_hois)
                .HasForeignKey(d => d.chu_de_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_cau_hoi_chu_de");
        });

        modelBuilder.Entity<chi_tiet_bai_thi>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__chi_tiet__3213E83FD804AFAD");

            entity.ToTable("chi_tiet_bai_thi");

            entity.HasIndex(e => new { e.bai_thi_id, e.cau_hoi_id }, "uq_ctbt").IsUnique();

            entity.HasOne(d => d.bai_thi).WithMany(p => p.chi_tiet_bai_this)
                .HasForeignKey(d => d.bai_thi_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ctbt_bai_thi");

            entity.HasOne(d => d.cau_hoi).WithMany(p => p.chi_tiet_bai_this)
                .HasForeignKey(d => d.cau_hoi_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ctbt_cau_hoi");

            entity.HasOne(d => d.dap_an_chon).WithMany(p => p.chi_tiet_bai_this)
                .HasForeignKey(d => d.dap_an_chon_id)
                .HasConstraintName("fk_ctbt_dap_an");
        });

        modelBuilder.Entity<chi_tiet_phieu_thu>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__chi_tiet__3213E83F519732CA");

            entity.ToTable("chi_tiet_phieu_thu");

            entity.Property(e => e.ghi_chu).HasMaxLength(255);
            entity.Property(e => e.so_tien).HasColumnType("decimal(18, 2)");

            entity.HasOne(d => d.loai_khoan_thu).WithMany(p => p.chi_tiet_phieu_thus)
                .HasForeignKey(d => d.loai_khoan_thu_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ctpt_loai_khoan_thu");

            entity.HasOne(d => d.phieu_thu).WithMany(p => p.chi_tiet_phieu_thus)
                .HasForeignKey(d => d.phieu_thu_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ctpt_phieu_thu");
        });

        modelBuilder.Entity<chu_de_cau_hoi>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__chu_de_c__3213E83F07F331E7");

            entity.ToTable("chu_de_cau_hoi");

            entity.HasIndex(e => e.ma_chu_de, "uq_chu_de_cau_hoi_ma").IsUnique();

            entity.Property(e => e.ma_chu_de)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.mo_ta).HasMaxLength(255);
            entity.Property(e => e.ten_chu_de).HasMaxLength(150);
        });

        modelBuilder.Entity<dang_ky_du_thi>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__dang_ky___3213E83F64B35EAB");

            entity.ToTable("dang_ky_du_thi");

            entity.HasIndex(e => new { e.hoc_vien_id, e.ca_thi_id }, "uq_dang_ky_du_thi").IsUnique();

            entity.Property(e => e.ngay_dang_ky).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.trang_thai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("cho_duyet");

            entity.HasOne(d => d.ca_thi).WithMany(p => p.dang_ky_du_this)
                .HasForeignKey(d => d.ca_thi_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_dkdt_ca_thi");

            entity.HasOne(d => d.hoc_vien).WithMany(p => p.dang_ky_du_this)
                .HasForeignKey(d => d.hoc_vien_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_dkdt_hoc_vien");

            entity.HasOne(d => d.nguoi_duyet).WithMany(p => p.dang_ky_du_this)
                .HasForeignKey(d => d.nguoi_duyet_id)
                .HasConstraintName("fk_dkdt_nguoi_duyet");
        });

        modelBuilder.Entity<dang_ky_khoa_hoc>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__dang_ky___3213E83F6D436961");

            entity.ToTable("dang_ky_khoa_hoc");

            entity.HasIndex(e => new { e.hoc_vien_id, e.khoa_hoc_id }, "uq_dang_ky_khoa_hoc").IsUnique();

            entity.Property(e => e.ngay_dang_ky).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.trang_thai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("cho_duyet");

            entity.HasOne(d => d.hoc_vien).WithMany(p => p.dang_ky_khoa_hocs)
                .HasForeignKey(d => d.hoc_vien_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_dk_khoa_hoc_hoc_vien");

            entity.HasOne(d => d.khoa_hoc).WithMany(p => p.dang_ky_khoa_hocs)
                .HasForeignKey(d => d.khoa_hoc_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_dk_khoa_hoc_khoa_hoc");

            entity.HasOne(d => d.nguoi_duyet).WithMany(p => p.dang_ky_khoa_hocs)
                .HasForeignKey(d => d.nguoi_duyet_id)
                .HasConstraintName("fk_dk_khoa_hoc_nguoi_duyet");
        });

        modelBuilder.Entity<dap_an>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__dap_an__3213E83F4B332C61");

            entity.ToTable("dap_an");

            entity.HasIndex(e => e.cau_hoi_id, "ix_dap_an_cau_hoi_id");

            entity.HasIndex(e => new { e.cau_hoi_id, e.thu_tu }, "uq_dap_an_thu_tu").IsUnique();

            entity.Property(e => e.noi_dung).HasMaxLength(1000);

            entity.HasOne(d => d.cau_hoi).WithMany(p => p.dap_ans)
                .HasForeignKey(d => d.cau_hoi_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_dap_an_cau_hoi");
        });

        modelBuilder.Entity<de_thi>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__de_thi__3213E83F22362081");

            entity.ToTable("de_thi");

            entity.HasIndex(e => e.ma_de_thi, "uq_de_thi_ma").IsUnique();

            entity.Property(e => e.ma_de_thi)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.ngay_tao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ten_de_thi).HasMaxLength(150);
            entity.Property(e => e.trang_thai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("nhap");

            entity.HasOne(d => d.ky_thi).WithMany(p => p.de_this)
                .HasForeignKey(d => d.ky_thi_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_de_thi_ky_thi");

            entity.HasOne(d => d.nguoi_tao).WithMany(p => p.de_this)
                .HasForeignKey(d => d.nguoi_tao_id)
                .HasConstraintName("fk_de_thi_nguoi_tao");
        });

        modelBuilder.Entity<de_thi_cau_hoi>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__de_thi_c__3213E83FFB05EAD9");

            entity.ToTable("de_thi_cau_hoi");

            entity.HasIndex(e => new { e.de_thi_id, e.cau_hoi_id }, "uq_de_thi_cau_hoi").IsUnique();

            entity.HasIndex(e => new { e.de_thi_id, e.thu_tu_cau }, "uq_de_thi_thu_tu").IsUnique();

            entity.HasOne(d => d.cau_hoi).WithMany(p => p.de_thi_cau_hois)
                .HasForeignKey(d => d.cau_hoi_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_dtch_cau_hoi");

            entity.HasOne(d => d.de_thi).WithMany(p => p.de_thi_cau_hois)
                .HasForeignKey(d => d.de_thi_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_dtch_de_thi");
        });

        modelBuilder.Entity<diem_danh>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__diem_dan__3213E83F623DEDEF");

            entity.ToTable("diem_danh");

            entity.HasIndex(e => e.buoi_hoc_id, "ix_diem_danh_buoi_hoc_id");

            entity.HasIndex(e => new { e.buoi_hoc_id, e.hoc_vien_id }, "uq_diem_danh").IsUnique();

            entity.Property(e => e.ghi_chu).HasMaxLength(255);
            entity.Property(e => e.thoi_gian_diem_danh).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.trang_thai)
                .HasMaxLength(30)
                .IsUnicode(false);

            entity.HasOne(d => d.buoi_hoc).WithMany(p => p.diem_danhs)
                .HasForeignKey(d => d.buoi_hoc_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_diem_danh_buoi_hoc");

            entity.HasOne(d => d.giao_vien).WithMany(p => p.diem_danhs)
                .HasForeignKey(d => d.giao_vien_id)
                .HasConstraintName("fk_diem_danh_giao_vien");

            entity.HasOne(d => d.hoc_vien).WithMany(p => p.diem_danhs)
                .HasForeignKey(d => d.hoc_vien_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_diem_danh_hoc_vien");
        });

        modelBuilder.Entity<giay_to_dinh_kem>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__giay_to___3213E83F360E6ECA");

            entity.ToTable("giay_to_dinh_kem");

            entity.Property(e => e.duong_dan_file)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.loai_file)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ngay_tai_len).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ten_giay_to).HasMaxLength(150);
            entity.Property(e => e.trang_thai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("hop_le");

            entity.HasOne(d => d.ho_so).WithMany(p => p.giay_to_dinh_kems)
                .HasForeignKey(d => d.ho_so_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_giay_to_ho_so");
        });

        modelBuilder.Entity<ho_so_dang_ky>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__ho_so_da__3213E83F8CFFB716");

            entity.ToTable("ho_so_dang_ky");

            entity.HasIndex(e => e.hoc_vien_id, "ix_ho_so_dang_ky_hoc_vien_id");

            entity.HasIndex(e => e.ma_ho_so, "uq_ho_so_ma_ho_so").IsUnique();

            entity.Property(e => e.ghi_chu).HasMaxLength(500);
            entity.Property(e => e.ma_ho_so)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.trang_thai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("cho_nop");

            entity.HasOne(d => d.hoc_vien).WithMany(p => p.ho_so_dang_kies)
                .HasForeignKey(d => d.hoc_vien_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ho_so_hoc_vien");

            entity.HasOne(d => d.nguoi_duyet).WithMany(p => p.ho_so_dang_kies)
                .HasForeignKey(d => d.nguoi_duyet_id)
                .HasConstraintName("fk_ho_so_nguoi_duyet");
        });

        modelBuilder.Entity<hoc_vien>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__hoc_vien__3213E83FD9A9A150");

            entity.ToTable("hoc_vien");

            entity.HasIndex(e => e.nguoi_dung_id, "ix_hoc_vien_nguoi_dung_id");

            entity.HasIndex(e => e.cccd, "uq_hoc_vien_cccd").IsUnique();

            entity.HasIndex(e => e.nguoi_dung_id, "uq_hoc_vien_nguoi_dung").IsUnique();

            entity.Property(e => e.anh_chan_dung)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.cccd)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.dia_chi).HasMaxLength(255);
            entity.Property(e => e.gioi_tinh).HasMaxLength(10);
            entity.Property(e => e.ho_ten).HasMaxLength(150);

            entity.HasOne(d => d.nguoi_dung).WithOne(p => p.hoc_vien)
                .HasForeignKey<hoc_vien>(d => d.nguoi_dung_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_hoc_vien_nguoi_dung");
        });

        modelBuilder.Entity<khoa_hoc>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__khoa_hoc__3213E83F9A6B857F");

            entity.ToTable("khoa_hoc");

            entity.HasIndex(e => e.ma_khoa_hoc, "uq_khoa_hoc_ma").IsUnique();

            entity.Property(e => e.hoc_phi).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ma_khoa_hoc)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.mo_ta).HasMaxLength(500);
            entity.Property(e => e.ten_khoa_hoc).HasMaxLength(150);
            entity.Property(e => e.trang_thai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("dang_mo");
        });

        modelBuilder.Entity<ky_thi>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__ky_thi__3213E83FF2AD1C11");

            entity.ToTable("ky_thi");

            entity.HasIndex(e => e.ngay_thi, "ix_ky_thi_ngay_thi");

            entity.HasIndex(e => e.ma_ky_thi, "uq_ky_thi_ma").IsUnique();

            entity.Property(e => e.ma_ky_thi)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.mo_ta).HasMaxLength(255);
            entity.Property(e => e.ten_ky_thi).HasMaxLength(150);
            entity.Property(e => e.trang_thai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("sap_dien_ra");
        });

        modelBuilder.Entity<loai_khoan_thu>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__loai_kho__3213E83F53F1DCAA");

            entity.ToTable("loai_khoan_thu");

            entity.HasIndex(e => e.ma_loai, "uq_loai_khoan_thu_ma").IsUnique();

            entity.Property(e => e.ma_loai)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.mo_ta).HasMaxLength(255);
            entity.Property(e => e.so_tien_mac_dinh).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ten_loai).HasMaxLength(150);
            entity.Property(e => e.trang_thai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("hoat_dong");
        });

        modelBuilder.Entity<loai_vi_pham>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__loai_vi___3213E83FCE816368");

            entity.ToTable("loai_vi_pham");

            entity.HasIndex(e => e.ma_loai, "uq_loai_vi_pham_ma").IsUnique();

            entity.Property(e => e.ma_loai)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.mo_ta).HasMaxLength(255);
            entity.Property(e => e.muc_xu_ly_mac_dinh).HasMaxLength(255);
            entity.Property(e => e.ten_loai).HasMaxLength(150);
        });

        modelBuilder.Entity<lop_hoc>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__lop_hoc__3213E83F5B2E35FA");

            entity.ToTable("lop_hoc");

            entity.HasIndex(e => e.khoa_hoc_id, "ix_lop_hoc_khoa_hoc_id");

            entity.HasIndex(e => e.ma_lop, "uq_lop_hoc_ma").IsUnique();

            entity.Property(e => e.ma_lop)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.ten_lop).HasMaxLength(150);
            entity.Property(e => e.trang_thai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("dang_mo");

            entity.HasOne(d => d.giao_vien).WithMany(p => p.lop_hocs)
                .HasForeignKey(d => d.giao_vien_id)
                .HasConstraintName("fk_lop_hoc_giao_vien");

            entity.HasOne(d => d.khoa_hoc).WithMany(p => p.lop_hocs)
                .HasForeignKey(d => d.khoa_hoc_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_lop_hoc_khoa_hoc");
        });

        modelBuilder.Entity<lop_hoc_hoc_vien>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__lop_hoc___3213E83FE4598538");

            entity.ToTable("lop_hoc_hoc_vien");

            entity.HasIndex(e => new { e.lop_hoc_id, e.hoc_vien_id }, "uq_lop_hoc_hoc_vien").IsUnique();

            entity.Property(e => e.trang_thai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("dang_hoc");

            entity.HasOne(d => d.hoc_vien).WithMany(p => p.lop_hoc_hoc_viens)
                .HasForeignKey(d => d.hoc_vien_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_lhhv_hoc_vien");

            entity.HasOne(d => d.lop_hoc).WithMany(p => p.lop_hoc_hoc_viens)
                .HasForeignKey(d => d.lop_hoc_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_lhhv_lop_hoc");
        });

        modelBuilder.Entity<nguoi_dung>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__nguoi_du__3213E83F218C9DD2");

            entity.ToTable("nguoi_dung");

            entity.HasIndex(e => e.email, "uq_nguoi_dung_email").IsUnique();

            entity.HasIndex(e => e.ten_dang_nhap, "uq_nguoi_dung_ten_dang_nhap").IsUnique();

            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.mat_khau_hash)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.so_dien_thoai)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.ten_dang_nhap)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.trang_thai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("hoat_dong");
            entity.Property(e => e.updated_at).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<nguoi_dung_vai_tro>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__nguoi_du__3213E83F0696B63A");

            entity.ToTable("nguoi_dung_vai_tro");

            entity.HasIndex(e => new { e.nguoi_dung_id, e.vai_tro_id }, "uq_ndvt").IsUnique();

            entity.HasOne(d => d.nguoi_dung).WithMany(p => p.nguoi_dung_vai_tros)
                .HasForeignKey(d => d.nguoi_dung_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ndvt_nguoi_dung");

            entity.HasOne(d => d.vai_tro).WithMany(p => p.nguoi_dung_vai_tros)
                .HasForeignKey(d => d.vai_tro_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ndvt_vai_tro");
        });

        modelBuilder.Entity<nhat_ky_he_thong>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__nhat_ky___3213E83F7EF1BC17");

            entity.ToTable("nhat_ky_he_thong");

            entity.Property(e => e.bang_tac_dong)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.hanh_dong).HasMaxLength(100);
            entity.Property(e => e.ip_address)
                .HasMaxLength(45)
                .IsUnicode(false);

            entity.HasOne(d => d.nguoi_dung).WithMany(p => p.nhat_ky_he_thongs)
                .HasForeignKey(d => d.nguoi_dung_id)
                .HasConstraintName("fk_nhat_ky_nguoi_dung");
        });

        modelBuilder.Entity<phien_on_tap>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__phien_on__3213E83F9B30F30C");

            entity.ToTable("phien_on_tap");

            entity.HasIndex(e => e.hoc_vien_id, "ix_phien_on_tap_hoc_vien_id");

            entity.Property(e => e.diem).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ngay_tao).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.trang_thai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("moi_tao");

            entity.HasOne(d => d.hoc_vien).WithMany(p => p.phien_on_taps)
                .HasForeignKey(d => d.hoc_vien_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_phien_on_tap_hoc_vien");
        });

        modelBuilder.Entity<phien_on_tap_cau_hoi>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__phien_on__3213E83F2C6DE98B");

            entity.ToTable("phien_on_tap_cau_hoi");

            entity.HasIndex(e => new { e.phien_on_tap_id, e.cau_hoi_id }, "uq_phien_on_tap_cau_hoi").IsUnique();

            entity.HasIndex(e => new { e.phien_on_tap_id, e.thu_tu_cau }, "uq_phien_on_tap_thu_tu").IsUnique();

            entity.HasOne(d => d.cau_hoi).WithMany(p => p.phien_on_tap_cau_hois)
                .HasForeignKey(d => d.cau_hoi_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_pot_ch_cau_hoi");

            entity.HasOne(d => d.dap_an_chon).WithMany(p => p.phien_on_tap_cau_hois)
                .HasForeignKey(d => d.dap_an_chon_id)
                .HasConstraintName("fk_pot_ch_dap_an");

            entity.HasOne(d => d.phien_on_tap).WithMany(p => p.phien_on_tap_cau_hois)
                .HasForeignKey(d => d.phien_on_tap_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_pot_ch_phien_on_tap");
        });

        modelBuilder.Entity<phieu_thu>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__phieu_th__3213E83F3C705F4E");

            entity.ToTable("phieu_thu");

            entity.HasIndex(e => e.hoc_vien_id, "ix_phieu_thu_hoc_vien_id");

            entity.HasIndex(e => e.ma_phieu_thu, "uq_phieu_thu_ma").IsUnique();

            entity.Property(e => e.ma_phieu_thu)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.ngay_thu).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.tong_tien).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.trang_thai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("cho_xac_nhan");

            entity.HasOne(d => d.hoc_vien).WithMany(p => p.phieu_thus)
                .HasForeignKey(d => d.hoc_vien_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_phieu_thu_hoc_vien");

            entity.HasOne(d => d.nguoi_lap).WithMany(p => p.phieu_thunguoi_laps)
                .HasForeignKey(d => d.nguoi_lap_id)
                .HasConstraintName("fk_phieu_thu_nguoi_lap");

            entity.HasOne(d => d.nguoi_xac_nhan).WithMany(p => p.phieu_thunguoi_xac_nhans)
                .HasForeignKey(d => d.nguoi_xac_nhan_id)
                .HasConstraintName("fk_phieu_thu_nguoi_xac_nhan");
        });

        modelBuilder.Entity<loai_nguoi_dung>(entity =>
        {
            entity.HasKey(e => e.id);

            entity.ToTable("loai_nguoi_dung");

            entity.HasIndex(e => e.ma_loai, "uq_loai_nguoi_dung_ma_loai").IsUnique();

            entity.Property(e => e.ma_loai)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.mo_ta).HasMaxLength(255);
            entity.Property(e => e.ten_loai).HasMaxLength(100);
            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.updated_at).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<nguoi_dung_loai>(entity =>
        {
            entity.HasKey(e => e.id);

            entity.ToTable("nguoi_dung_loai");

            entity.HasIndex(e => new { e.nguoi_dung_id, e.loai_nguoi_dung_id }, "uq_nguoi_dung_loai").IsUnique();

            entity.HasIndex(e => e.nguoi_dung_id, "ix_nguoi_dung_loai_nguoi_dung_id");

            entity.HasIndex(e => e.loai_nguoi_dung_id, "ix_nguoi_dung_loai_loai_id");

            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.nguoi_dung).WithMany()
                .HasForeignKey(d => d.nguoi_dung_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ndl_nguoi_dung");

            entity.HasOne(d => d.loai_nguoi_dung).WithMany(p => p.nguoi_dung_loais)
                .HasForeignKey(d => d.loai_nguoi_dung_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ndl_loai_nguoi_dung");
        });

        modelBuilder.Entity<goi_quyen>(entity =>
        {
            entity.HasKey(e => e.id);

            entity.ToTable("goi_quyen");

            entity.HasIndex(e => e.ma_goi, "uq_goi_quyen_ma_goi").IsUnique();

            entity.Property(e => e.ma_goi)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ten_goi).HasMaxLength(150);
            entity.Property(e => e.mo_ta).HasMaxLength(500);
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.updated_at).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<quyen_su_dung>(entity =>
        {
            entity.HasKey(e => e.id);

            entity.ToTable("quyen_su_dung");

            entity.HasIndex(e => e.goi_quyen_id, "ix_qsd_goi_quyen_id");

            entity.HasIndex(e => e.ngay_het_han, "ix_qsd_ngay_het_han");

            entity.HasIndex(e => new { e.nguoi_dung_id, e.trang_thai }, "ix_qsd_nguoi_dung_trang_thai");

            entity.Property(e => e.nguon_cap)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.trang_thai)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.ghi_chu).HasMaxLength(500);
            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.updated_at).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.created_by_nguoi_dung).WithMany()
                .HasForeignKey(d => d.created_by)
                .HasConstraintName("fk_qsd_created_by");

            entity.HasOne(d => d.goi_quyen).WithMany(p => p.quyen_su_dungs)
                .HasForeignKey(d => d.goi_quyen_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_qsd_goi_quyen");

            entity.HasOne(d => d.nguoi_dung).WithMany()
                .HasForeignKey(d => d.nguoi_dung_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_qsd_nguoi_dung");
        });

        modelBuilder.Entity<files>(entity =>
        {
            entity.HasKey(e => e.id);

            entity.ToTable("files");

            entity.HasIndex(e => e.storage_provider, "ix_files_storage_provider");

            entity.HasIndex(e => e.created_at, "ix_files_created_at");

            entity.HasIndex(e => e.created_by, "ix_files_created_by");

            entity.Property(e => e.storage_provider)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.bucket_name)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.object_key)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.public_url)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.file_name).HasMaxLength(255);
            entity.Property(e => e.mime_type)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.checksum_sha256)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.trang_thai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("active");
            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.updated_at).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.created_by_nguoi_dung).WithMany()
                .HasForeignKey(d => d.created_by)
                .HasConstraintName("fk_files_created_by");
        });

        modelBuilder.Entity<file_usages>(entity =>
        {
            entity.HasKey(e => e.id);

            entity.ToTable("file_usages");

            entity.HasIndex(e => new { e.file_id, e.entity_name, e.entity_id, e.field_name }, "uq_file_usages").IsUnique();

            entity.HasIndex(e => new { e.entity_name, e.entity_id }, "ix_file_usages_entity");

            entity.HasIndex(e => e.file_id, "ix_file_usages_file_id");

            entity.Property(e => e.entity_name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.field_name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.is_primary).HasDefaultValue(false);
            entity.Property(e => e.sort_order).HasDefaultValue(0);
            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.file).WithMany(p => p.file_usages)
                .HasForeignKey(d => d.file_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_fu_file");
        });

        modelBuilder.Entity<categories>(entity =>
        {
            entity.HasKey(e => e.id);

            entity.ToTable("categories");

            entity.HasIndex(e => e.ma_danh_muc, "uq_categories_ma_danh_muc").IsUnique();

            entity.HasIndex(e => e.slug, "uq_categories_slug").IsUnique();

            entity.HasIndex(e => e.parent_id, "ix_categories_parent_id");

            entity.HasIndex(e => e.is_active, "ix_categories_is_active");

            entity.Property(e => e.ma_danh_muc)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.ten_danh_muc).HasMaxLength(150);
            entity.Property(e => e.slug)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.mo_ta).HasMaxLength(500);
            entity.Property(e => e.is_active).HasDefaultValue(true);
            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.updated_at).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.created_by_nguoi_dung).WithMany()
                .HasForeignKey(d => d.created_by)
                .HasConstraintName("fk_categories_created_by");

            entity.HasOne(d => d.parent).WithMany(p => p.inverse_parent)
                .HasForeignKey(d => d.parent_id)
                .HasConstraintName("fk_categories_parent");
        });

        modelBuilder.Entity<posts>(entity =>
        {
            entity.HasKey(e => e.id);

            entity.ToTable("posts");

            entity.HasIndex(e => e.ma_bai_viet, "uq_posts_ma_bai_viet").IsUnique();

            entity.HasIndex(e => e.slug, "uq_posts_slug").IsUnique();

            entity.HasIndex(e => e.post_type, "ix_posts_post_type");

            entity.HasIndex(e => e.trang_thai, "ix_posts_trang_thai");

            entity.HasIndex(e => e.published_at, "ix_posts_published_at");

            entity.HasIndex(e => e.author_id, "ix_posts_author_id");

            entity.Property(e => e.ma_bai_viet)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.title).HasMaxLength(255);
            entity.Property(e => e.slug)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.summary).HasMaxLength(1000);
            entity.Property(e => e.post_type)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.meta_title).HasMaxLength(255);
            entity.Property(e => e.meta_description).HasMaxLength(500);
            entity.Property(e => e.canonical_url)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.trang_thai)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasDefaultValue("draft");
            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.updated_at).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.author).WithMany()
                .HasForeignKey(d => d.author_id)
                .HasConstraintName("fk_posts_author");

            entity.HasOne(d => d.thumbnail_file).WithMany(p => p.posts)
                .HasForeignKey(d => d.thumbnail_file_id)
                .HasConstraintName("fk_posts_thumbnail_file");
        });

        modelBuilder.Entity<post_categories>(entity =>
        {
            entity.HasKey(e => e.id);

            entity.ToTable("post_categories");

            entity.HasIndex(e => new { e.post_id, e.category_id }, "uq_post_categories").IsUnique();

            entity.HasIndex(e => e.post_id, "ix_post_categories_post_id");

            entity.HasIndex(e => e.category_id, "ix_post_categories_category_id");

            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.category).WithMany(p => p.post_categories)
                .HasForeignKey(d => d.category_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_post_categories_category");

            entity.HasOne(d => d.post).WithMany(p => p.post_categories)
                .HasForeignKey(d => d.post_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_post_categories_post");
        });

        modelBuilder.Entity<exam_results>(entity =>
        {
            entity.HasKey(e => e.id);

            entity.ToTable("exam_results");

            entity.HasIndex(e => e.bai_thi_id, "uq_exam_results_bai_thi_id").IsUnique();

            entity.HasIndex(e => e.hoc_vien_id, "ix_exam_results_hoc_vien_id");

            entity.HasIndex(e => e.ket_qua, "ix_exam_results_ket_qua");

            entity.HasIndex(e => e.xac_nhan_luc, "ix_exam_results_xac_nhan_luc");

            entity.Property(e => e.diem).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.ket_qua)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.updated_at).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.bai_thi).WithOne(p => p.exam_result)
                .HasForeignKey<exam_results>(d => d.bai_thi_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_exam_results_bai_thi");

            entity.HasOne(d => d.hoc_vien).WithMany(p => p.exam_results)
                .HasForeignKey(d => d.hoc_vien_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_exam_results_hoc_vien");

            entity.HasOne(d => d.xac_nhan_boi_nguoi_dung).WithMany()
                .HasForeignKey(d => d.xac_nhan_boi)
                .HasConstraintName("fk_exam_results_xac_nhan_boi");
        });

        modelBuilder.Entity<certificates>(entity =>
        {
            entity.HasKey(e => e.id);

            entity.ToTable("certificates");

            entity.HasIndex(e => e.ma_chung_chi, "uq_certificates_ma_chung_chi").IsUnique();

            entity.HasIndex(e => e.exam_result_id, "uq_certificates_exam_result_id").IsUnique();

            entity.HasIndex(e => e.hoc_vien_id, "ix_certificates_hoc_vien_id");

            entity.HasIndex(e => e.trang_thai, "ix_certificates_trang_thai");

            entity.HasIndex(e => e.ngay_cap, "ix_certificates_ngay_cap");

            entity.Property(e => e.ma_chung_chi)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.trang_thai)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.created_at).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.updated_at).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.certificate_file).WithMany(p => p.certificates)
                .HasForeignKey(d => d.certificate_file_id)
                .HasConstraintName("fk_certificates_file");

            entity.HasOne(d => d.created_by_nguoi_dung).WithMany()
                .HasForeignKey(d => d.created_by)
                .HasConstraintName("fk_certificates_created_by");

            entity.HasOne(d => d.exam_result).WithMany(p => p.certificates)
                .HasForeignKey(d => d.exam_result_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_certificates_exam_result");

            entity.HasOne(d => d.hoc_vien).WithMany(p => p.certificates)
                .HasForeignKey(d => d.hoc_vien_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_certificates_hoc_vien");
        });

        modelBuilder.Entity<quyen_han>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__quyen_ha__3213E83F99FB91A4");

            entity.ToTable("quyen_han");

            entity.HasIndex(e => e.ma_quyen, "uq_quyen_han_ma_quyen").IsUnique();

            entity.Property(e => e.ma_quyen)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.mo_ta).HasMaxLength(255);
            entity.Property(e => e.ten_quyen).HasMaxLength(100);
        });

        modelBuilder.Entity<vai_tro>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__vai_tro__3213E83F6D6E8B4C");

            entity.ToTable("vai_tro");

            entity.HasIndex(e => e.ma_vai_tro, "uq_vai_tro_ma_vai_tro").IsUnique();

            entity.Property(e => e.ma_vai_tro)
                .HasMaxLength(30)
                .IsUnicode(false);
            entity.Property(e => e.mo_ta).HasMaxLength(255);
            entity.Property(e => e.ten_vai_tro).HasMaxLength(100);
        });

        modelBuilder.Entity<vai_tro_quyen_han>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__vai_tro___3213E83F1F80C3DC");

            entity.ToTable("vai_tro_quyen_han");

            entity.HasIndex(e => new { e.vai_tro_id, e.quyen_han_id }, "uq_vtqh").IsUnique();

            entity.HasOne(d => d.quyen_han).WithMany(p => p.vai_tro_quyen_hans)
                .HasForeignKey(d => d.quyen_han_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vtqh_quyen_han");

            entity.HasOne(d => d.vai_tro).WithMany(p => p.vai_tro_quyen_hans)
                .HasForeignKey(d => d.vai_tro_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vtqh_vai_tro");
        });

        modelBuilder.Entity<vi_pham_quy_che>(entity =>
        {
            entity.HasKey(e => e.id).HasName("PK__vi_pham___3213E83F554E65D5");

            entity.ToTable("vi_pham_quy_che");

            entity.HasIndex(e => e.hoc_vien_id, "ix_vi_pham_hoc_vien_id");

            entity.Property(e => e.hinh_thuc_xu_ly).HasMaxLength(255);
            entity.Property(e => e.mo_ta).HasMaxLength(500);
            entity.Property(e => e.thoi_gian_vi_pham).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.bai_thi).WithMany(p => p.vi_pham_quy_ches)
                .HasForeignKey(d => d.bai_thi_id)
                .HasConstraintName("fk_vpqc_bai_thi");

            entity.HasOne(d => d.hoc_vien).WithMany(p => p.vi_pham_quy_ches)
                .HasForeignKey(d => d.hoc_vien_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vpqc_hoc_vien");

            entity.HasOne(d => d.loai_vi_pham).WithMany(p => p.vi_pham_quy_ches)
                .HasForeignKey(d => d.loai_vi_pham_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_vpqc_loai_vi_pham");

            entity.HasOne(d => d.nguoi_ghi_nhan).WithMany(p => p.vi_pham_quy_ches)
                .HasForeignKey(d => d.nguoi_ghi_nhan_id)
                .HasConstraintName("fk_vpqc_nguoi_ghi_nhan");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
