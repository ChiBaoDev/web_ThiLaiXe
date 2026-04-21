using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class cau_hoi
{
    public long id { get; set; }

    public long chu_de_id { get; set; }

    public string noi_dung { get; set; } = null!;

    public string loai_cau_hoi { get; set; } = null!;

    public string? muc_do { get; set; }

    public bool la_cau_diem_liet { get; set; }

    public string trang_thai { get; set; } = null!;

    public virtual ICollection<chi_tiet_bai_thi> chi_tiet_bai_this { get; set; } = new List<chi_tiet_bai_thi>();

    public virtual chu_de_cau_hoi chu_de { get; set; } = null!;

    public virtual ICollection<dap_an> dap_ans { get; set; } = new List<dap_an>();

    public virtual ICollection<de_thi_cau_hoi> de_thi_cau_hois { get; set; } = new List<de_thi_cau_hoi>();

    public virtual ICollection<phien_on_tap_cau_hoi> phien_on_tap_cau_hois { get; set; } = new List<phien_on_tap_cau_hoi>();
}
