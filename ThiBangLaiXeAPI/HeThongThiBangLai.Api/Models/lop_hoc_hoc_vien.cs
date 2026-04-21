using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class lop_hoc_hoc_vien
{
    public long id { get; set; }

    public long lop_hoc_id { get; set; }

    public long hoc_vien_id { get; set; }

    public DateOnly? ngay_vao_lop { get; set; }

    public string trang_thai { get; set; } = null!;

    public virtual hoc_vien hoc_vien { get; set; } = null!;

    public virtual lop_hoc lop_hoc { get; set; } = null!;
}
