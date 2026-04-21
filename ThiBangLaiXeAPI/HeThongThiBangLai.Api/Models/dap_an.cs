using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class dap_an
{
    public long id { get; set; }

    public long cau_hoi_id { get; set; }

    public string noi_dung { get; set; } = null!;

    public bool la_dap_an_dung { get; set; }

    public int thu_tu { get; set; }

    public virtual cau_hoi cau_hoi { get; set; } = null!;

    public virtual ICollection<chi_tiet_bai_thi> chi_tiet_bai_this { get; set; } = new List<chi_tiet_bai_thi>();

    public virtual ICollection<phien_on_tap_cau_hoi> phien_on_tap_cau_hois { get; set; } = new List<phien_on_tap_cau_hoi>();
}
