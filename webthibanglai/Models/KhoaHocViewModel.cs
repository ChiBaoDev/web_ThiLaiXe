namespace webthibanglai.Models;

public class KhoaHocViewModel
{
    public List<KhoaHocCourseItem> Courses { get; set; } = new();
    public int TotalCourses { get; set; }
    public int OpenCourses { get; set; }
    public long LowestPrice { get; set; }
    public string? ErrorMessage { get; set; }
}

public class KhoaHocDetailViewModel
{
    public KhoaHocCourseDetail? Course { get; set; }
    public List<KhoaHocClassItem> Classes { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public string? RegistrationMessage { get; set; }
    public string? RegistrationErrorMessage { get; set; }
    public int? SelectedClassId { get; set; }
    public string? PaymentStatusMessage { get; set; }
    public string? PaymentStatusState { get; set; }
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

public class KhoaHocCourseDetail
{
    public int CourseId { get; set; }
    public string MaKhoaHoc { get; set; } = string.Empty;
    public string TenKhoaHoc { get; set; } = string.Empty;
    public string LoaiBangLai { get; set; } = string.Empty;
    public string MoTa { get; set; } = string.Empty;
    public long HocPhi { get; set; }
    public int SoBuoiHoc { get; set; }
    public int SoLuongToiDa { get; set; }
    public int SoLuongHienTai { get; set; }
    public DateOnly? NgayBatDau { get; set; }
    public DateOnly? NgayKetThuc { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public string? GiaoVienChinh { get; set; }
    public string? SoDienThoaiGiaoVien { get; set; }
    public List<KhoaHocScheduleItem> LichHocMau { get; set; } = new();
    public string HinhAnh { get; set; } = string.Empty;
    public bool IsOpenForRegistration { get; set; }
    public int OccupancyRate { get; set; }
    public int RemainingSlots => Math.Max(SoLuongToiDa - SoLuongHienTai, 0);
}

public class KhoaHocClassItem
{
    public int ClassId { get; set; }
    public string TenLop { get; set; } = string.Empty;
    public int SiSoHienTai { get; set; }
    public int SiSoToiDa { get; set; }
    public DateOnly? NgayBatDau { get; set; }
    public DateOnly? NgayKetThuc { get; set; }
    public string? GiaoVien { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public bool IsOpenForRegistration { get; set; }
    public List<KhoaHocScheduleItem> LichHoc { get; set; } = new();
    public int RemainingSlots => Math.Max(SiSoToiDa - SiSoHienTai, 0);
}

public class MyCourseRegistrationsViewModel
{
    public List<MyCourseRegistrationItem> Registrations { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public string? StatusMessage { get; set; }
    public string? StatusState { get; set; }
    public bool IsLoading { get; set; }
}

public class MyCourseRegistrationItem
{
    public int RegistrationId { get; set; }
    public int CourseId { get; set; }
    public int? ClassId { get; set; }
    public string MaKhoaHoc { get; set; } = string.Empty;
    public string TenKhoaHoc { get; set; } = string.Empty;
    public string LoaiBangLai { get; set; } = string.Empty;
    public string TenLop { get; set; } = string.Empty;
    public string NgayHocText { get; set; } = string.Empty;
    public DateOnly? NgayBatDau { get; set; }
    public DateOnly? NgayKetThuc { get; set; }
    public long HocPhi { get; set; }
    public int SoBuoiHoc { get; set; }
    public int SiSoHienTai { get; set; }
    public int SiSoToiDa { get; set; }
    public string GiaoVien { get; set; } = string.Empty;
    public string SoDienThoaiGiaoVien { get; set; } = string.Empty;
    public string MoTa { get; set; } = string.Empty;
    public List<KhoaHocScheduleItem> LichHoc { get; set; } = new();
    public string TrangThai { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public string? ReceiptId { get; set; }
    public bool CanPayWithZaloPay { get; set; }
    public string? PaymentDisabledReason { get; set; }
    public int RemainingSlots => Math.Max(SiSoToiDa - SiSoHienTai, 0);
}

public class KhoaHocScheduleItem
{
    public int ThuTrongTuan { get; set; }
    public string GioBatDau { get; set; } = string.Empty;
    public string GioKetThuc { get; set; } = string.Empty;
    public string DiaDiem { get; set; } = string.Empty;
}

public class WeeklyScheduleTableViewModel
{
    public List<WeeklyScheduleWeekOption> Weeks { get; set; } = new();
    public List<WeeklyScheduleWeekTable> WeekTables { get; set; } = new();
    public int SelectedWeekIndex { get; set; }
}

public class WeeklyScheduleWeekOption
{
    public int Index { get; set; }
    public string Label { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
}

public class WeeklyScheduleWeekTable
{
    public int WeekIndex { get; set; }
    public string Label { get; set; } = string.Empty;
    public List<WeeklyScheduleDayColumn> Days { get; set; } = new();
    public List<WeeklyScheduleRow> Rows { get; set; } = new();
}

public class WeeklyScheduleDayColumn
{
    public int DayOfWeek { get; set; }
    public string Label { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
}

public class WeeklyScheduleRow
{
    public string SessionKey { get; set; } = string.Empty;
    public string SessionLabel { get; set; } = string.Empty;
    public List<WeeklyScheduleCell> Cells { get; set; } = new();
}

public class WeeklyScheduleCell
{
    public int DayOfWeek { get; set; }
    public string SessionKey { get; set; } = string.Empty;
    public List<WeeklyScheduleOccurrenceItem> Items { get; set; } = new();
}

public class WeeklyScheduleOccurrenceItem
{
    public DateOnly? NgayHoc { get; set; }
    public string GioBatDau { get; set; } = string.Empty;
    public string GioKetThuc { get; set; } = string.Empty;
    public string DiaDiem { get; set; } = string.Empty;
}
