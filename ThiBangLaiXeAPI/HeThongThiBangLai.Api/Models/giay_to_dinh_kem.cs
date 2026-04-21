using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class giay_to_dinh_kem
{
    public long id { get; set; }

    public long ho_so_id { get; set; }

    public string ten_giay_to { get; set; } = null!;

    public string duong_dan_file { get; set; } = null!;

    public string? loai_file { get; set; }

    public DateTime ngay_tai_len { get; set; }

    public string trang_thai { get; set; } = null!;

    public virtual ho_so_dang_ky ho_so { get; set; } = null!;
}
