using webthibanglai.Models;

namespace webthibanglai.Models;

public class LichHocViewModel
{
    public StudentDashboardProfile Profile { get; set; } = new();
    public StudentDashboardStats Stats { get; set; } = new();
    public List<StudentRegisteredCourseItem> RegisteredCourses { get; set; } = new();
    public List<StudentScheduleItem> Schedule { get; set; } = new();
}

public class StudentDashboardProfile
{
    public long UserId { get; set; }
    public long HocVienId { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public string TenDangNhap { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SoDienThoai { get; set; } = string.Empty;
    public string GioiTinh { get; set; } = string.Empty;
    public string NgaySinhText { get; set; } = string.Empty;
    public string Cccd { get; set; } = string.Empty;
    public string DiaChi { get; set; } = string.Empty;
    public string AnhChanDung { get; set; } = string.Empty;
    public string TrangThai { get; set; } = string.Empty;
    public string Initials { get; set; } = "HV";
    public string RoleLabel { get; set; } = "Học viên";
}

public class StudentDashboardStats
{
    public int TotalExams { get; set; }
    public int PassedExams { get; set; }
    public int FailedExams { get; set; }
    public decimal PassRate { get; set; }
    public decimal AverageScore { get; set; }
    public int CorrectAnswers { get; set; }
    public int WrongAnswers { get; set; }
    public int CriticalWrongCount { get; set; }
    public int PracticeCount { get; set; }
    public int WrongQuestionCount { get; set; }
    public int TotalCriticalAttempts { get; set; }
    public decimal CriticalErrorRate { get; set; }
}

public class StudentRegisteredCourseItem
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ScheduleText { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

public class StudentScheduleItem
{
    public string DayLabel { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string TimeText { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string AccentClass { get; set; } = "primary";
}
