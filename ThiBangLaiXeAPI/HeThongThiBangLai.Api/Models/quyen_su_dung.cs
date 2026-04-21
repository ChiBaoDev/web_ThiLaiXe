using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class quyen_su_dung
{
    public long id { get; set; }

    public long nguoi_dung_id { get; set; }

    public long goi_quyen_id { get; set; }

    public DateTime ngay_hieu_luc { get; set; }

    public DateTime? ngay_het_han { get; set; }

    public string nguon_cap { get; set; } = null!;

    public string trang_thai { get; set; } = null!;

    public string? ghi_chu { get; set; }

    public long? created_by { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual nguoi_dung? created_by_nguoi_dung { get; set; }

    public virtual goi_quyen goi_quyen { get; set; } = null!;

    public virtual nguoi_dung nguoi_dung { get; set; } = null!;
}
