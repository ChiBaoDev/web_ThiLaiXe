using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class categories
{
    public long id { get; set; }

    public long? parent_id { get; set; }

    public string ma_danh_muc { get; set; } = null!;

    public string ten_danh_muc { get; set; } = null!;

    public string slug { get; set; } = null!;

    public string? mo_ta { get; set; }

    public bool is_active { get; set; }

    public long? created_by { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual nguoi_dung? created_by_nguoi_dung { get; set; }

    public virtual categories? parent { get; set; }

    public virtual ICollection<categories> inverse_parent { get; set; } = new List<categories>();

    public virtual ICollection<post_categories> post_categories { get; set; } = new List<post_categories>();
}
