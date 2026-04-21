using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class dang_ky_khoa_hoc
{
    public long id { get; set; }

    public long hoc_vien_id { get; set; }

    public long khoa_hoc_id { get; set; }

    public DateTime ngay_dang_ky { get; set; }

    public string trang_thai { get; set; } = null!;

    public long? nguoi_duyet_id { get; set; }

    public DateTime? ngay_duyet { get; set; }

    public virtual hoc_vien hoc_vien { get; set; } = null!;

    public virtual khoa_hoc khoa_hoc { get; set; } = null!;

    public virtual nguoi_dung? nguoi_duyet { get; set; }
}
