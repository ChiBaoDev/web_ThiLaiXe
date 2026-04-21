using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class certificates
{
    public long id { get; set; }

    public string ma_chung_chi { get; set; } = null!;

    public long hoc_vien_id { get; set; }

    public long exam_result_id { get; set; }

    public DateTime ngay_cap { get; set; }

    public DateTime? ngay_het_han { get; set; }

    public string trang_thai { get; set; } = null!;

    public long? certificate_file_id { get; set; }

    public long? created_by { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual files? certificate_file { get; set; }

    public virtual nguoi_dung? created_by_nguoi_dung { get; set; }

    public virtual exam_results exam_result { get; set; } = null!;

    public virtual hoc_vien hoc_vien { get; set; } = null!;
}
