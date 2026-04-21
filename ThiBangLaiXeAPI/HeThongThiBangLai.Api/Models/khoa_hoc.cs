using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class khoa_hoc
{
    public long id { get; set; }

    public string ma_khoa_hoc { get; set; } = null!;

    public string ten_khoa_hoc { get; set; } = null!;

    public string? mo_ta { get; set; }

    public decimal hoc_phi { get; set; }

    public int? thoi_luong { get; set; }

    public string trang_thai { get; set; } = null!;

    public virtual ICollection<dang_ky_khoa_hoc> dang_ky_khoa_hocs { get; set; } = new List<dang_ky_khoa_hoc>();

    public virtual ICollection<lop_hoc> lop_hocs { get; set; } = new List<lop_hoc>();
}
