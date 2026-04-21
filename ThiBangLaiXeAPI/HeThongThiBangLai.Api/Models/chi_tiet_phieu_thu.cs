using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class chi_tiet_phieu_thu
{
    public long id { get; set; }

    public long phieu_thu_id { get; set; }

    public long loai_khoan_thu_id { get; set; }

    public decimal so_tien { get; set; }

    public string? ghi_chu { get; set; }

    public virtual loai_khoan_thu loai_khoan_thu { get; set; } = null!;

    public virtual phieu_thu phieu_thu { get; set; } = null!;
}
