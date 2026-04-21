using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class nhat_ky_he_thong
{
    public long id { get; set; }

    public long? nguoi_dung_id { get; set; }

    public string hanh_dong { get; set; } = null!;

    public string? bang_tac_dong { get; set; }

    public long? khoa_chinh_du_lieu { get; set; }

    public string? noi_dung { get; set; }

    public string? ip_address { get; set; }

    public DateTime created_at { get; set; }

    public virtual nguoi_dung? nguoi_dung { get; set; }
}
