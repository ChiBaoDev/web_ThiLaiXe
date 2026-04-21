using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class nguoi_dung_loai
{
    public long id { get; set; }

    public long nguoi_dung_id { get; set; }

    public long loai_nguoi_dung_id { get; set; }

    public DateTime created_at { get; set; }

    public virtual loai_nguoi_dung loai_nguoi_dung { get; set; } = null!;

    public virtual nguoi_dung nguoi_dung { get; set; } = null!;
}
