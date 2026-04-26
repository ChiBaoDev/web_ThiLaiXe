namespace webthibanglai.Models;

public class ProfileViewModel
{
    public StudentProfileInfo? Profile { get; set; }
    public List<CourseRegistrationItem> CourseRegistrations { get; set; } = new();
    public List<PracticeHistoryItem> PracticeHistory { get; set; } = new();
    public ProfileChangePasswordModel ChangePassword { get; set; } = new();
}

public class StudentProfileInfo
{
    public int StudentId { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SoDienThoai { get; set; } = string.Empty;
    public DateOnly NgaySinh { get; set; }
    public string DiaChi { get; set; } = string.Empty;
    public string AnhChanDung { get; set; } = string.Empty;
}

public class CourseRegistrationItem
{
    public int RegistrationId { get; set; }
    public int StudentId { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public int CourseId { get; set; }
    public string TenKhoaHoc { get; set; } = string.Empty;
    public string TrangThai { get; set; } = string.Empty;
    public DateTime NgayDangKy { get; set; }
}

public class ProfileChangePasswordModel
{
    public string MatKhauCu { get; set; } = string.Empty;
    public string MatKhauMoi { get; set; } = string.Empty;
    public string XacNhanMatKhauMoi { get; set; } = string.Empty;
}
