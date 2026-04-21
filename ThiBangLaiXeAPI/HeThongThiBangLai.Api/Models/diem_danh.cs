using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class diem_danh
{
    public long id { get; set; }

    public long buoi_hoc_id { get; set; }

    public long hoc_vien_id { get; set; }

    public string trang_thai { get; set; } = null!;

    public string? ghi_chu { get; set; }

    public long? giao_vien_id { get; set; }

    public DateTime thoi_gian_diem_danh { get; set; }

    public virtual buoi_hoc buoi_hoc { get; set; } = null!;

    public virtual nguoi_dung? giao_vien { get; set; }

    public virtual hoc_vien hoc_vien { get; set; } = null!;
}
