using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class de_thi
{
    public long id { get; set; }

    public string ma_de_thi { get; set; } = null!;

    public string ten_de_thi { get; set; } = null!;

    public long ky_thi_id { get; set; }

    public int tong_so_cau { get; set; }

    public int thoi_gian_lam_bai { get; set; }

    public string trang_thai { get; set; } = null!;

    public long? nguoi_tao_id { get; set; }

    public DateTime ngay_tao { get; set; }

    public virtual ICollection<bai_thi> bai_this { get; set; } = new List<bai_thi>();

    public virtual ICollection<de_thi_cau_hoi> de_thi_cau_hois { get; set; } = new List<de_thi_cau_hoi>();

    public virtual ky_thi ky_thi { get; set; } = null!;

    public virtual nguoi_dung? nguoi_tao { get; set; }
}
