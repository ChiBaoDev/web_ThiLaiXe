using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class loai_khoan_thu
{
    public long id { get; set; }

    public string ma_loai { get; set; } = null!;

    public string ten_loai { get; set; } = null!;

    public decimal so_tien_mac_dinh { get; set; }

    public string? mo_ta { get; set; }

    public string trang_thai { get; set; } = null!;

    public virtual ICollection<chi_tiet_phieu_thu> chi_tiet_phieu_thus { get; set; } = new List<chi_tiet_phieu_thu>();
}
