using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class posts
{
    public long id { get; set; }

    public string ma_bai_viet { get; set; } = null!;

    public string title { get; set; } = null!;

    public string slug { get; set; } = null!;

    public string? summary { get; set; }

    public string content { get; set; } = null!;

    public string post_type { get; set; } = null!;

    public long? thumbnail_file_id { get; set; }

    public string? meta_title { get; set; }

    public string? meta_description { get; set; }

    public string? canonical_url { get; set; }

    public DateTime? published_at { get; set; }

    public string trang_thai { get; set; } = null!;

    public long? author_id { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual nguoi_dung? author { get; set; }

    public virtual ICollection<post_categories> post_categories { get; set; } = new List<post_categories>();

    public virtual files? thumbnail_file { get; set; }
}
