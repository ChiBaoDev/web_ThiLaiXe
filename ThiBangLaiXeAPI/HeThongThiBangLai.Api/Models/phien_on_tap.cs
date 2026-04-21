using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class phien_on_tap
{
    public long id { get; set; }

    public long hoc_vien_id { get; set; }

    public DateTime ngay_tao { get; set; }

    public DateTime? thoi_gian_bat_dau { get; set; }

    public DateTime? thoi_gian_nop { get; set; }

    public int tong_so_cau { get; set; }

    public int so_cau_dung { get; set; }

    public decimal diem { get; set; }

    public string trang_thai { get; set; } = null!;

    public virtual hoc_vien hoc_vien { get; set; } = null!;

    public virtual ICollection<phien_on_tap_cau_hoi> phien_on_tap_cau_hois { get; set; } = new List<phien_on_tap_cau_hoi>();
}
