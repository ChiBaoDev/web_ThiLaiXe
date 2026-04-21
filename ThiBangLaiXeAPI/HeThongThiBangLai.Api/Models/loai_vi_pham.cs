using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class loai_vi_pham
{
    public long id { get; set; }

    public string ma_loai { get; set; } = null!;

    public string ten_loai { get; set; } = null!;

    public string? mo_ta { get; set; }

    public string? muc_xu_ly_mac_dinh { get; set; }

    public virtual ICollection<vi_pham_quy_che> vi_pham_quy_ches { get; set; } = new List<vi_pham_quy_che>();
}
