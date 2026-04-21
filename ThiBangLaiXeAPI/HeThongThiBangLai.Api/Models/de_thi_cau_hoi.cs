using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class de_thi_cau_hoi
{
    public long id { get; set; }

    public long de_thi_id { get; set; }

    public long cau_hoi_id { get; set; }

    public int thu_tu_cau { get; set; }

    public virtual cau_hoi cau_hoi { get; set; } = null!;

    public virtual de_thi de_thi { get; set; } = null!;
}
