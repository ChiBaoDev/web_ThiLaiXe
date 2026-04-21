using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class vi_pham_quy_che
{
    public long id { get; set; }

    public long hoc_vien_id { get; set; }

    public long? bai_thi_id { get; set; }

    public long loai_vi_pham_id { get; set; }

    public long? nguoi_ghi_nhan_id { get; set; }

    public DateTime thoi_gian_vi_pham { get; set; }

    public string? mo_ta { get; set; }

    public string? hinh_thuc_xu_ly { get; set; }

    public virtual bai_thi? bai_thi { get; set; }

    public virtual hoc_vien hoc_vien { get; set; } = null!;

    public virtual loai_vi_pham loai_vi_pham { get; set; } = null!;

    public virtual nguoi_dung? nguoi_ghi_nhan { get; set; }
}
