using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class vai_tro
{
    public long id { get; set; }

    public string ma_vai_tro { get; set; } = null!;

    public string ten_vai_tro { get; set; } = null!;

    public string? mo_ta { get; set; }

    public virtual ICollection<nguoi_dung_vai_tro> nguoi_dung_vai_tros { get; set; } = new List<nguoi_dung_vai_tro>();

    public virtual ICollection<vai_tro_quyen_han> vai_tro_quyen_hans { get; set; } = new List<vai_tro_quyen_han>();
}
