using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class dang_ky_du_thi
{
    public long id { get; set; }

    public long hoc_vien_id { get; set; }

    public long ca_thi_id { get; set; }

    public DateTime ngay_dang_ky { get; set; }

    public string trang_thai { get; set; } = null!;

    public long? nguoi_duyet_id { get; set; }

    public DateTime? ngay_duyet { get; set; }

    public virtual ca_thi ca_thi { get; set; } = null!;

    public virtual hoc_vien hoc_vien { get; set; } = null!;

    public virtual nguoi_dung? nguoi_duyet { get; set; }
}
