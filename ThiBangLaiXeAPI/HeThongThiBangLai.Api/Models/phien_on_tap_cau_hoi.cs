using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class phien_on_tap_cau_hoi
{
    public long id { get; set; }

    public long phien_on_tap_id { get; set; }

    public long cau_hoi_id { get; set; }

    public long? dap_an_chon_id { get; set; }

    public bool? la_dung { get; set; }

    public int thu_tu_cau { get; set; }

    public virtual cau_hoi cau_hoi { get; set; } = null!;

    public virtual dap_an? dap_an_chon { get; set; }

    public virtual phien_on_tap phien_on_tap { get; set; } = null!;
}
