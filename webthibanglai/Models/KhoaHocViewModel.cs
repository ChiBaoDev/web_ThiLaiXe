namespace webthibanglai.Models;

public class KhoaHocViewModel
{
    public List<KhoaHocCourseItem> Courses { get; set; } = new();
    public int TotalCourses { get; set; }
    public int OpenCourses { get; set; }
    public long LowestPrice { get; set; }
    public string? ErrorMessage { get; set; }
}

public class KhoaHocCourseItem
{
    public int CourseId { get; set; }
    public string MaKhoaHoc { get; set; } = string.Empty;
    public string TenKhoaHoc { get; set; } = string.Empty;
    public string LoaiBangLai { get; set; } = string.Empty;
    public long HocPhi { get; set; }
    public int SoBuoiHoc { get; set; }
    public DateOnly? ThoiGianBatDau { get; set; }
    public DateOnly? ThoiGianKetThuc { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public int SoLuongToiDa { get; set; }
    public int SoLuongHienTai { get; set; }
    public string MoTaNgan { get; set; } = string.Empty;
    public string? LichHocTomTat { get; set; }
    public string HinhAnh { get; set; } = string.Empty;
    public bool IsOpenForRegistration { get; set; }
    public int OccupancyRate { get; set; }
}
