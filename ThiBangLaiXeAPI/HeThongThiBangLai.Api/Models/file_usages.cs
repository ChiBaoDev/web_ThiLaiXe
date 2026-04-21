using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class file_usages
{
    public long id { get; set; }

    public long file_id { get; set; }

    public string entity_name { get; set; } = null!;

    public long entity_id { get; set; }

    public string field_name { get; set; } = null!;

    public bool is_primary { get; set; }

    public int sort_order { get; set; }

    public DateTime created_at { get; set; }

    public virtual files file { get; set; } = null!;
}
