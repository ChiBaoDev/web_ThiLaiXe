namespace webthibanglai.Models;

public class KhoaHocViewModel
{
    public List<KhoaHocCourseItem> Courses { get; set; } = new();
}

public class KhoaHocCourseItem
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
