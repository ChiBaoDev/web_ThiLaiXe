using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class ky_thi
{
    public long id { get; set; }

    public string ma_ky_thi { get; set; } = null!;

    public string ten_ky_thi { get; set; } = null!;

    public DateOnly ngay_thi { get; set; }

    public string? mo_ta { get; set; }

    public string trang_thai { get; set; } = null!;

    public virtual ICollection<ca_thi> ca_this { get; set; } = new List<ca_thi>();

    public virtual ICollection<de_thi> de_this { get; set; } = new List<de_thi>();
}
