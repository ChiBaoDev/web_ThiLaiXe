using System;
using System.Collections.Generic;

namespace HeThongThiBangLai.Api.Models;

public partial class files
{
    public long id { get; set; }

    public string storage_provider { get; set; } = null!;

    public string? bucket_name { get; set; }

    public string object_key { get; set; } = null!;

    public string public_url { get; set; } = null!;

    public string file_name { get; set; } = null!;

    public string mime_type { get; set; } = null!;

    public long size_bytes { get; set; }

    public string? checksum_sha256 { get; set; }

    public int? width { get; set; }

    public int? height { get; set; }

    public int? duration_seconds { get; set; }

    public string trang_thai { get; set; } = null!;

    public long? created_by { get; set; }

    public DateTime created_at { get; set; }

    public DateTime updated_at { get; set; }

    public virtual nguoi_dung? created_by_nguoi_dung { get; set; }

    public virtual ICollection<certificates> certificates { get; set; } = new List<certificates>();

    public virtual ICollection<file_usages> file_usages { get; set; } = new List<file_usages>();

    public virtual ICollection<posts> posts { get; set; } = new List<posts>();
}
