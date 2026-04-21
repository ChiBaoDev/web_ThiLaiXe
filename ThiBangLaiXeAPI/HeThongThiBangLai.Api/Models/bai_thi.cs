using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class bai_thi
{
    public long id { get; set; }

    public long hoc_vien_id { get; set; }

    public long de_thi_id { get; set; }

    public long ca_thi_id { get; set; }

    public DateTime? thoi_gian_bat_dau { get; set; }

    public DateTime? thoi_gian_nop { get; set; }

    public int tong_so_cau { get; set; }

    public int so_cau_dung { get; set; }

    public decimal diem { get; set; }

    public string? ket_qua { get; set; }

    public string trang_thai { get; set; } = null!;

    public virtual ca_thi ca_thi { get; set; } = null!;

    public virtual ICollection<chi_tiet_bai_thi> chi_tiet_bai_this { get; set; } = new List<chi_tiet_bai_thi>();

    public virtual de_thi de_thi { get; set; } = null!;

    public virtual hoc_vien hoc_vien { get; set; } = null!;

    public virtual exam_results? exam_result { get; set; }

    public virtual ICollection<vi_pham_quy_che> vi_pham_quy_ches { get; set; } = new List<vi_pham_quy_che>();
}
