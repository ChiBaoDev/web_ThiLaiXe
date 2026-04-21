using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class vai_tro_quyen_han
{
    public long id { get; set; }

    public long vai_tro_id { get; set; }

    public long quyen_han_id { get; set; }

    public virtual quyen_han quyen_han { get; set; } = null!;

    public virtual vai_tro vai_tro { get; set; } = null!;
}
