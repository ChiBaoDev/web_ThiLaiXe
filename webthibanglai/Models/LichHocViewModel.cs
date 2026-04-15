namespace webthibanglai.Models;

public class LichHocViewModel
{
    public LichHocOverview? Overview { get; set; }
    public List<LichHocSessionItem> Sessions { get; set; } = new();
    public List<LichHocMilestoneItem> Milestones { get; set; } = new();
}

public class LichHocOverview
{
    public string TenKhoaHoc { get; set; } = string.Empty;
    public string LoaiBangLai { get; set; } = string.Empty;
    public string TenGiaoVien { get; set; } = string.Empty;
    public string TrangThaiLop { get; set; } = string.Empty;
    public int SiSo { get; set; }
}

public class LichHocSessionItem
{
    public int SessionId { get; set; }
    public int ClassId { get; set; }
    public DateOnly NgayHoc { get; set; }
    public string GioBatDau { get; set; } = string.Empty;
    public string GioKetThuc { get; set; } = string.Empty;
    public string DiaDiem { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;
    public string TrangThai { get; set; } = string.Empty;
}

public class LichHocMilestoneItem
{
    public int ExamId { get; set; }
    public string TenKyThi { get; set; } = string.Empty;
    public DateOnly NgayThi { get; set; }
    public string DiaDiem { get; set; } = string.Empty;
    public string TrangThai { get; set; } = string.Empty;
}
