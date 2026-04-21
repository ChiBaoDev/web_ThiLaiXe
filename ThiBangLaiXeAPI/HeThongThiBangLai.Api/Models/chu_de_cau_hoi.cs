using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class chu_de_cau_hoi
{
    public long id { get; set; }

    public string ma_chu_de { get; set; } = null!;

    public string ten_chu_de { get; set; } = null!;

    public string? mo_ta { get; set; }

    public virtual ICollection<cau_hoi> cau_hois { get; set; } = new List<cau_hoi>();
}
