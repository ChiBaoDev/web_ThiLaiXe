using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class nguoi_dung
{
    public long id { get; set; }

    public string ten_dang_nhap { get; set; } = null!;

    public string mat_khau_hash { get; set; } = null!;

    public string email { get; set; } = null!;

    public string? so_dien_thoai { get; set; }

    public string trang_thai { get; set; } = null!;

    public DateTime? lan_dang_nhap_cuoi { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual ICollection<dang_ky_du_thi> dang_ky_du_this { get; set; } = new List<dang_ky_du_thi>();

    public virtual ICollection<dang_ky_khoa_hoc> dang_ky_khoa_hocs { get; set; } = new List<dang_ky_khoa_hoc>();

    public virtual ICollection<de_thi> de_this { get; set; } = new List<de_thi>();

    public virtual ICollection<diem_danh> diem_danhs { get; set; } = new List<diem_danh>();

    public virtual ICollection<ho_so_dang_ky> ho_so_dang_kies { get; set; } = new List<ho_so_dang_ky>();

    public virtual hoc_vien? hoc_vien { get; set; }

    public virtual ICollection<lop_hoc> lop_hocs { get; set; } = new List<lop_hoc>();

    public virtual ICollection<nguoi_dung_vai_tro> nguoi_dung_vai_tros { get; set; } = new List<nguoi_dung_vai_tro>();

    public virtual ICollection<nhat_ky_he_thong> nhat_ky_he_thongs { get; set; } = new List<nhat_ky_he_thong>();

    public virtual ICollection<phieu_thu> phieu_thunguoi_laps { get; set; } = new List<phieu_thu>();

    public virtual ICollection<phieu_thu> phieu_thunguoi_xac_nhans { get; set; } = new List<phieu_thu>();

    public virtual ICollection<vi_pham_quy_che> vi_pham_quy_ches { get; set; } = new List<vi_pham_quy_che>();
}
