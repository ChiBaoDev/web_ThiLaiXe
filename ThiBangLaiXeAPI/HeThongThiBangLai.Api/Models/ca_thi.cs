using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class ca_thi
{
    public long id { get; set; }

    public long ky_thi_id { get; set; }

    public string ma_ca_thi { get; set; } = null!;

    public string ten_ca_thi { get; set; } = null!;

    public TimeOnly gio_bat_dau { get; set; }

    public TimeOnly gio_ket_thuc { get; set; }

    public string? phong_thi { get; set; }

    public int so_luong_toi_da { get; set; }

    public virtual ICollection<bai_thi> bai_this { get; set; } = new List<bai_thi>();

    public virtual ICollection<dang_ky_du_thi> dang_ky_du_this { get; set; } = new List<dang_ky_du_thi>();

    public virtual ky_thi ky_thi { get; set; } = null!;
}
