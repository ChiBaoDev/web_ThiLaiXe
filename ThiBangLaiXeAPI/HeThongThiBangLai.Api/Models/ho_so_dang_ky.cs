using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class ho_so_dang_ky
{
    public long id { get; set; }

    public long hoc_vien_id { get; set; }

    public string ma_ho_so { get; set; } = null!;

    public DateTime? ngay_nop { get; set; }

    public string trang_thai { get; set; } = null!;

    public string? ghi_chu { get; set; }

    public long? nguoi_duyet_id { get; set; }

    public DateTime? ngay_duyet { get; set; }

    public virtual ICollection<giay_to_dinh_kem> giay_to_dinh_kems { get; set; } = new List<giay_to_dinh_kem>();

    public virtual hoc_vien hoc_vien { get; set; } = null!;

    public virtual nguoi_dung? nguoi_duyet { get; set; }
}
