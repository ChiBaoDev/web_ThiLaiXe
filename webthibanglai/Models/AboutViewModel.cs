namespace webthibanglai.Models;

public class AboutViewModel
{
    public AboutSummaryInfo? SummaryInfo { get; set; }
    public List<AboutTeacherItem> Teachers { get; set; } = new();
}

public class AboutSummaryInfo
{
    public int TongHocVien { get; set; }
    public int HocVienMoiThangNay { get; set; }
    public decimal TyLeDatThi { get; set; }
    public int TongKyThiSapDienRa { get; set; }
}

public class AboutTeacherItem
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
