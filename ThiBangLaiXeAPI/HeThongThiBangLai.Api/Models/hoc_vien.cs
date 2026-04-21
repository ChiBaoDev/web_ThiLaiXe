using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class hoc_vien
{
    public long id { get; set; }

    public long nguoi_dung_id { get; set; }

    public string ho_ten { get; set; } = null!;

    public DateOnly? ngay_sinh { get; set; }

    public string? gioi_tinh { get; set; }

    public string? cccd { get; set; }

    public string? dia_chi { get; set; }

    public string? anh_chan_dung { get; set; }

    public DateTime created_at { get; set; }

    public virtual ICollection<bai_thi> bai_this { get; set; } = new List<bai_thi>();

    public virtual ICollection<dang_ky_du_thi> dang_ky_du_this { get; set; } = new List<dang_ky_du_thi>();

    public virtual ICollection<dang_ky_khoa_hoc> dang_ky_khoa_hocs { get; set; } = new List<dang_ky_khoa_hoc>();

    public virtual ICollection<diem_danh> diem_danhs { get; set; } = new List<diem_danh>();

    public virtual ICollection<ho_so_dang_ky> ho_so_dang_kies { get; set; } = new List<ho_so_dang_ky>();

    public virtual ICollection<lop_hoc_hoc_vien> lop_hoc_hoc_viens { get; set; } = new List<lop_hoc_hoc_vien>();

    public virtual nguoi_dung nguoi_dung { get; set; } = null!;

    public virtual ICollection<phien_on_tap> phien_on_taps { get; set; } = new List<phien_on_tap>();

    public virtual ICollection<phieu_thu> phieu_thus { get; set; } = new List<phieu_thu>();

    public virtual ICollection<exam_results> exam_results { get; set; } = new List<exam_results>();

    public virtual ICollection<certificates> certificates { get; set; } = new List<certificates>();

    public virtual ICollection<vi_pham_quy_che> vi_pham_quy_ches { get; set; } = new List<vi_pham_quy_che>();
}
