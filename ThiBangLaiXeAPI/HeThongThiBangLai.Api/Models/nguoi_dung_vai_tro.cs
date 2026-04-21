using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class nguoi_dung_vai_tro
{
    public long id { get; set; }

    public long nguoi_dung_id { get; set; }

    public long vai_tro_id { get; set; }

    public virtual nguoi_dung nguoi_dung { get; set; } = null!;

    public virtual vai_tro vai_tro { get; set; } = null!;
}
