namespace webthibanglai.Models;

public class HomeViewModel
{
    public List<CourseSummaryItem> FeaturedCourses { get; set; } = new();
    public List<ExamSummaryItem> UpcomingExams { get; set; } = new();
    public List<HomeTeacherItem> Teachers { get; set; } = new();
    public HomeAboutInfo? AboutInfo { get; set; }
}

public class CourseSummaryItem
{
    public int CourseId { get; set; }
    public string TenKhoaHoc { get; set; } = string.Empty;
    public string LoaiBangLai { get; set; } = string.Empty;
    public long HocPhi { get; set; }
    public int SoBuoiHoc { get; set; }
    public DateOnly ThoiGianBatDau { get; set; }
    public DateOnly ThoiGianKetThuc { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public int SoLuongToiDa { get; set; }
    public int SoLuongHienTai { get; set; }
}

public class ExamSummaryItem
{
    public int ExamId { get; set; }
    public string TenKyThi { get; set; } = string.Empty;
    public string LoaiBangLai { get; set; } = string.Empty;
    public DateOnly NgayThi { get; set; }
    public string DiaDiem { get; set; } = string.Empty;
    public string TrangThai { get; set; } = string.Empty;
}

public class HomeTeacherItem
{
    public int GiaoVienId { get; set; }
    public string TenGiaoVien { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public string TenLop { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string TenKhoaHoc { get; set; } = string.Empty;
    public int SiSo { get; set; }
    public string TrangThai { get; set; } = string.Empty;
}

public class HomeAboutInfo
{
    public int TongHocVien { get; set; }
    public int HocVienMoiThangNay { get; set; }
    public decimal TyLeDatThi { get; set; }
    public int TongKyThiSapDienRa { get; set; }
}

