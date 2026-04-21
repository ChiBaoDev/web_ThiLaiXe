using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class goi_quyen
{
    public long id { get; set; }

    public string ma_goi { get; set; } = null!;

    public string ten_goi { get; set; } = null!;

    public string? mo_ta { get; set; }

    public bool is_active { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual ICollection<quyen_su_dung> quyen_su_dungs { get; set; } = new List<quyen_su_dung>();
}
