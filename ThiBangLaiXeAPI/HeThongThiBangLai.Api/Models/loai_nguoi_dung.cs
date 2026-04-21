using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class loai_nguoi_dung
{
    public long id { get; set; }

    public string ma_loai { get; set; } = null!;

    public string ten_loai { get; set; } = null!;

    public string? mo_ta { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual ICollection<nguoi_dung_loai> nguoi_dung_loais { get; set; } = new List<nguoi_dung_loai>();
}
