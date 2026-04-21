using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class quyen_han
{
    public long id { get; set; }

    public string ma_quyen { get; set; } = null!;

    public string ten_quyen { get; set; } = null!;

    public string? mo_ta { get; set; }

    public virtual ICollection<vai_tro_quyen_han> vai_tro_quyen_hans { get; set; } = new List<vai_tro_quyen_han>();
}
