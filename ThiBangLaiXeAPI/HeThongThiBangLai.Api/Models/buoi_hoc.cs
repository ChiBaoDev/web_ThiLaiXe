using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class buoi_hoc
{
    public long id { get; set; }

    public long lop_hoc_id { get; set; }

    public string ten_buoi { get; set; } = null!;

    public DateOnly ngay_hoc { get; set; }

    public TimeOnly gio_bat_dau { get; set; }

    public TimeOnly gio_ket_thuc { get; set; }

    public string? noi_dung { get; set; }

    public string? phong_hoc { get; set; }

    public virtual ICollection<diem_danh> diem_danhs { get; set; } = new List<diem_danh>();

    public virtual lop_hoc lop_hoc { get; set; } = null!;
}
