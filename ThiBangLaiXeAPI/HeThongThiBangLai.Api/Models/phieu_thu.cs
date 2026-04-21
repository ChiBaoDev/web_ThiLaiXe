using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class phieu_thu
{
    public long id { get; set; }

    public string ma_phieu_thu { get; set; } = null!;

    public long hoc_vien_id { get; set; }

    public DateTime ngay_thu { get; set; }

    public decimal tong_tien { get; set; }

    public string trang_thai { get; set; } = null!;

    public long? nguoi_lap_id { get; set; }

    public long? nguoi_xac_nhan_id { get; set; }

    public virtual ICollection<chi_tiet_phieu_thu> chi_tiet_phieu_thus { get; set; } = new List<chi_tiet_phieu_thu>();

    public virtual hoc_vien hoc_vien { get; set; } = null!;

    public virtual nguoi_dung? nguoi_lap { get; set; }

    public virtual nguoi_dung? nguoi_xac_nhan { get; set; }
}
