using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class chi_tiet_bai_thi
{
    public long id { get; set; }

    public long bai_thi_id { get; set; }

    public long cau_hoi_id { get; set; }

    public long? dap_an_chon_id { get; set; }

    public bool? la_dung { get; set; }

    public virtual bai_thi bai_thi { get; set; } = null!;

    public virtual cau_hoi cau_hoi { get; set; } = null!;

    public virtual dap_an? dap_an_chon { get; set; }
}
