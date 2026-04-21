using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class post_categories
{
    public long id { get; set; }

    public long post_id { get; set; }

    public long category_id { get; set; }

    public DateTime created_at { get; set; }

    public virtual categories category { get; set; } = null!;

    public virtual posts post { get; set; } = null!;
}
