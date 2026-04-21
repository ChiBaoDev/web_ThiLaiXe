using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class exam_results
{
    public long id { get; set; }

    public long bai_thi_id { get; set; }

    public long hoc_vien_id { get; set; }

    public int tong_so_cau { get; set; }

    public int so_cau_dung { get; set; }

    public decimal diem { get; set; }

    public string ket_qua { get; set; } = null!;

    public long? xac_nhan_boi { get; set; }

    public DateTime? xac_nhan_luc { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual bai_thi bai_thi { get; set; } = null!;

    public virtual ICollection<certificates> certificates { get; set; } = new List<certificates>();

    public virtual hoc_vien hoc_vien { get; set; } = null!;

    public virtual nguoi_dung? xac_nhan_boi_nguoi_dung { get; set; }
}
