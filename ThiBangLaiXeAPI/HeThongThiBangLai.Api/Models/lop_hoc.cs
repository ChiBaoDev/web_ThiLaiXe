using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class lop_hoc
{
    public long id { get; set; }

    public long khoa_hoc_id { get; set; }

    public string ma_lop { get; set; } = null!;

    public string ten_lop { get; set; } = null!;

    public long? giao_vien_id { get; set; }

    public DateOnly? ngay_bat_dau { get; set; }

    public DateOnly? ngay_ket_thuc { get; set; }

    public int si_so_toi_da { get; set; }

    public string trang_thai { get; set; } = null!;

    public virtual ICollection<buoi_hoc> buoi_hocs { get; set; } = new List<buoi_hoc>();

    public virtual nguoi_dung? giao_vien { get; set; }

    public virtual khoa_hoc khoa_hoc { get; set; } = null!;

    public virtual ICollection<lop_hoc_hoc_vien> lop_hoc_hoc_viens { get; set; } = new List<lop_hoc_hoc_vien>();
}
