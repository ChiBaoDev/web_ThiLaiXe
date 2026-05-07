using webthibanglai.Models;

namespace webthibanglai.Models;

public class LichHocViewModel
{
    public StudentDashboardProfile Profile { get; set; } = new();
    public StudentProfileRegistrationModel Registration { get; set; } = new();
    public StudentDashboardStats Stats { get; set; } = new();
    public List<StudentRegisteredCourseItem> RegisteredCourses { get; set; } = new();
    public List<StudentCourseRegistrationItem> CourseRegistrations { get; set; } = new();
    public List<StudentScheduleItem> Schedule { get; set; } = new();
    public List<StudentPaidCourseScheduleViewModel> PaidCourseSchedules { get; set; } = new();
    public WeeklyScheduleTableViewModel CombinedPaidCourseScheduleTable { get; set; } = new();
    public int TotalPaidCourseSessions { get; set; }
    public bool HasStudentProfile { get; set; }
    public string? RegistrationErrorMessage { get; set; }
    public string? RegistrationSuccessMessage { get; set; }
    public string? CourseRegistrationStatusMessage { get; set; }
    public string? CourseRegistrationStatusState { get; set; }

    public void ApplyPaidCourseSchedules(List<MyCourseRegistrationItem> registrations, Dictionary<int, List<KhoaHocClassItem>> courseClassesByCourseId)
    {
        PaidCourseSchedules = registrations
            .Where(item => IsApprovedRegistrationStatus(item.TrangThai) && IsPaymentSuccessStatus(item.PaymentStatus))
            .Select(item =>
            {
                var matchedClass = FindMatchedClass(item, courseClassesByCourseId);
                var classSchedule = matchedClass?.LichHoc ?? item.LichHoc;
                var startDate = matchedClass?.NgayBatDau ?? item.NgayBatDau;
                var endDate = matchedClass?.NgayKetThuc ?? item.NgayKetThuc;
                var teacherName = matchedClass?.GiaoVien ?? item.GiaoVien;
                var className = matchedClass?.TenLop ?? item.TenLop;

                var scheduleDetails = classSchedule
                    .Select(schedule => new StudentCourseScheduleItem
                    {
                        DayOfWeek = schedule.ThuTrongTuan,
                        DayLabel = FormatDayOfWeek(schedule.ThuTrongTuan),
                        StartTime = schedule.GioBatDau,
                        EndTime = schedule.GioKetThuc,
                        Location = string.IsNullOrWhiteSpace(schedule.DiaDiem) ? "Theo phân công trung tâm" : schedule.DiaDiem,
                        CourseName = item.TenKhoaHoc,
                        ClassName = className
                    })
                    .ToList();

                var scheduleTable = BuildWeeklyScheduleTable(scheduleDetails, startDate, endDate);
                var totalSessions = scheduleTable.WeekTables
                    .SelectMany(week => week.Rows)
                    .SelectMany(row => row.Cells)
                    .Sum(cell => cell.Items.Count);

                return new StudentPaidCourseScheduleViewModel
                {
                    RegistrationId = item.RegistrationId,
                    CourseName = item.TenKhoaHoc,
                    ClassName = className,
                    TeacherName = string.IsNullOrWhiteSpace(teacherName) ? "Theo phân công trung tâm" : teacherName,
                    StudyTimeText = BuildStudyDateText(startDate, endDate),
                    Status = item.TrangThai,
                    PaymentStatus = item.PaymentStatus,
                    ScheduleDetails = scheduleDetails,
                    ScheduleTable = scheduleTable,
                    TotalSessions = totalSessions
                };
            })
            .ToList();

        CombinedPaidCourseScheduleTable = BuildCombinedWeeklyScheduleTable(PaidCourseSchedules);
        TotalPaidCourseSessions = CombinedPaidCourseScheduleTable.WeekTables
            .SelectMany(week => week.Rows)
            .SelectMany(row => row.Cells)
            .Sum(cell => cell.Items.Count);
    }

    private static KhoaHocClassItem? FindMatchedClass(MyCourseRegistrationItem registration, Dictionary<int, List<KhoaHocClassItem>> courseClassesByCourseId)
    {
        if (!courseClassesByCourseId.TryGetValue(registration.CourseId, out var classes) || classes.Count == 0)
        {
            return null;
        }

        if (registration.ClassId.HasValue)
        {
            var byId = classes.FirstOrDefault(item => item.ClassId == registration.ClassId.Value);
            if (byId is not null)
            {
                return byId;
            }
        }

        return classes.FirstOrDefault(item => string.Equals(item.TenLop, registration.TenLop, StringComparison.OrdinalIgnoreCase));
    }

    private static string BuildStudyDateText(DateOnly? startDate, DateOnly? endDate)
    {
        if (startDate.HasValue && endDate.HasValue)
        {
            return $"{startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}";
        }

        if (startDate.HasValue)
        {
            return startDate.Value.ToString("dd/MM/yyyy");
        }

        return "Đang cập nhật";
    }

    private static WeeklyScheduleTableViewModel BuildWeeklyScheduleTable(List<StudentCourseScheduleItem> schedules, DateOnly? startDate, DateOnly? endDate)
    {
        var expandedSchedule = new List<(int DayOfWeek, string SessionKey, WeeklyScheduleOccurrenceItem Item)>();

        if (startDate.HasValue && endDate.HasValue && startDate <= endDate)
        {
            var nextSearchDate = startDate.Value;

            foreach (var schedule in schedules)
            {
                var occurrenceDate = FindNextDateByDayOfWeek(nextSearchDate, schedule.DayOfWeek);
                if (occurrenceDate > endDate.Value)
                {
                    break;
                }

                expandedSchedule.Add((
                    schedule.DayOfWeek,
                    DetectSession(schedule.StartTime),
                    new WeeklyScheduleOccurrenceItem
                        {
                            NgayHoc = occurrenceDate,
                            CourseName = schedule.CourseName,
                            ClassName = schedule.ClassName,
                            GioBatDau = schedule.StartTime,
                            GioKetThuc = schedule.EndTime,
                            DiaDiem = schedule.Location
                    }));

                nextSearchDate = occurrenceDate.AddDays(1);
            }
        }

        var effectiveStartDate = startDate ?? DateOnly.FromDateTime(DateTime.Today);
        var effectiveEndDate = endDate ?? effectiveStartDate;
        var firstWeekStart = GetStartOfWeek(effectiveStartDate);
        var lastWeekStart = GetStartOfWeek(effectiveEndDate);
        var weekOptions = new List<WeeklyScheduleWeekOption>();

        for (var weekStart = firstWeekStart; weekStart <= lastWeekStart; weekStart = weekStart.AddDays(7))
        {
            var weekEnd = weekStart.AddDays(6);
            weekOptions.Add(new WeeklyScheduleWeekOption
            {
                Index = weekOptions.Count,
                StartDate = weekStart,
                EndDate = weekEnd,
                Label = $"Tuần {weekOptions.Count + 1}: {weekStart:dd/MM} - {weekEnd:dd/MM}"
            });
        }

        if (weekOptions.Count == 0)
        {
            weekOptions.Add(new WeeklyScheduleWeekOption
            {
                Index = 0,
                StartDate = firstWeekStart,
                EndDate = firstWeekStart.AddDays(6),
                Label = $"Tuần 1: {firstWeekStart:dd/MM} - {firstWeekStart.AddDays(6):dd/MM}"
            });
        }

        var scheduleTable = new WeeklyScheduleTableViewModel
        {
            Weeks = weekOptions,
            SelectedWeekIndex = 0,
            WeekTables = new List<WeeklyScheduleWeekTable>()
        };

        var sessionDefinitions = new[]
        {
            new { Key = "Sang", Label = "Sáng" },
            new { Key = "Chieu", Label = "Chiều" },
            new { Key = "Toi", Label = "Tối" }
        };

        foreach (var week in weekOptions)
        {
            var weekTable = new WeeklyScheduleWeekTable
            {
                WeekIndex = week.Index,
                Label = week.Label,
                Days = Enumerable.Range(0, 7)
                    .Select(offset => week.StartDate.AddDays(offset))
                    .Select(date => new WeeklyScheduleDayColumn
                    {
                        DayOfWeek = NormalizeDayOfWeek(date.DayOfWeek),
                        Label = FormatDayOfWeek(NormalizeDayOfWeek(date.DayOfWeek)),
                        Date = date
                    })
                    .ToList(),
                Rows = new List<WeeklyScheduleRow>()
            };

            foreach (var session in sessionDefinitions)
            {
                weekTable.Rows.Add(new WeeklyScheduleRow
                {
                    SessionKey = session.Key,
                    SessionLabel = session.Label,
                    Cells = weekTable.Days.Select(day => new WeeklyScheduleCell
                    {
                        DayOfWeek = day.DayOfWeek,
                        SessionKey = session.Key,
                        Items = expandedSchedule
                            .Where(item => item.DayOfWeek == day.DayOfWeek
                                && item.SessionKey == session.Key
                                && item.Item.NgayHoc == day.Date)
                            .Select(item => item.Item)
                            .OrderBy(item => item.NgayHoc)
                            .ThenBy(item => item.GioBatDau)
                            .ToList()
                    }).ToList()
                });
            }

            scheduleTable.WeekTables.Add(weekTable);
        }

        return scheduleTable;
    }

    private static WeeklyScheduleTableViewModel BuildCombinedWeeklyScheduleTable(List<StudentPaidCourseScheduleViewModel> paidCourseSchedules)
    {
        var allOccurrences = paidCourseSchedules
            .SelectMany(course => course.ScheduleTable.WeekTables)
            .SelectMany(week => week.Rows)
            .SelectMany(row => row.Cells)
            .SelectMany(cell => cell.Items)
            .Where(item => item.NgayHoc.HasValue)
            .OrderBy(item => item.NgayHoc)
            .ThenBy(item => item.GioBatDau)
            .ToList();

        if (allOccurrences.Count == 0)
        {
            return new WeeklyScheduleTableViewModel();
        }

        var effectiveStartDate = allOccurrences.Min(item => item.NgayHoc!.Value);
        var effectiveEndDate = allOccurrences.Max(item => item.NgayHoc!.Value);
        var firstWeekStart = GetStartOfWeek(effectiveStartDate);
        var lastWeekStart = GetStartOfWeek(effectiveEndDate);
        var weekOptions = new List<WeeklyScheduleWeekOption>();

        for (var weekStart = firstWeekStart; weekStart <= lastWeekStart; weekStart = weekStart.AddDays(7))
        {
            var weekEnd = weekStart.AddDays(6);
            weekOptions.Add(new WeeklyScheduleWeekOption
            {
                Index = weekOptions.Count,
                StartDate = weekStart,
                EndDate = weekEnd,
                Label = $"Tuần {weekOptions.Count + 1}: {weekStart:dd/MM} - {weekEnd:dd/MM}"
            });
        }

        var scheduleTable = new WeeklyScheduleTableViewModel
        {
            Weeks = weekOptions,
            SelectedWeekIndex = 0,
            WeekTables = new List<WeeklyScheduleWeekTable>()
        };

        var sessionDefinitions = new[]
        {
            new { Key = "Sang", Label = "Sáng" },
            new { Key = "Chieu", Label = "Chiều" },
            new { Key = "Toi", Label = "Tối" }
        };

        foreach (var week in weekOptions)
        {
            var weekTable = new WeeklyScheduleWeekTable
            {
                WeekIndex = week.Index,
                Label = week.Label,
                Days = Enumerable.Range(0, 7)
                    .Select(offset => week.StartDate.AddDays(offset))
                    .Select(date => new WeeklyScheduleDayColumn
                    {
                        DayOfWeek = NormalizeDayOfWeek(date.DayOfWeek),
                        Label = FormatDayOfWeek(NormalizeDayOfWeek(date.DayOfWeek)),
                        Date = date
                    })
                    .ToList(),
                Rows = new List<WeeklyScheduleRow>()
            };

            foreach (var session in sessionDefinitions)
            {
                weekTable.Rows.Add(new WeeklyScheduleRow
                {
                    SessionKey = session.Key,
                    SessionLabel = session.Label,
                    Cells = weekTable.Days.Select(day => new WeeklyScheduleCell
                    {
                        DayOfWeek = day.DayOfWeek,
                        SessionKey = session.Key,
                        Items = allOccurrences
                            .Where(item => item.NgayHoc == day.Date && DetectSession(item.GioBatDau) == session.Key)
                            .OrderBy(item => item.GioBatDau)
                            .ThenBy(item => item.CourseName)
                            .ToList()
                    }).ToList()
                });
            }

            scheduleTable.WeekTables.Add(weekTable);
        }

        return scheduleTable;
    }

    private static bool IsApprovedRegistrationStatus(string? status)
    {
        return string.Equals(status, "da_duyet", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "DaDuyet", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "đã duyệt", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsPaymentSuccessStatus(string? status)
    {
        return string.Equals(status, "da_xac_nhan", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "DaXacNhan", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "da_thanh_toan", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "DaThanhToan", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "đã thanh toán", StringComparison.OrdinalIgnoreCase);
    }

    private static string FormatDayOfWeek(int day) => day switch
    {
        2 => "Thứ 2",
        3 => "Thứ 3",
        4 => "Thứ 4",
        5 => "Thứ 5",
        6 => "Thứ 6",
        7 => "Thứ 7",
        8 => "Chủ nhật",
        _ => $"Thứ {day}"
    };

    private static int NormalizeDayOfWeek(DayOfWeek dayOfWeek) => dayOfWeek switch
    {
        DayOfWeek.Monday => 2,
        DayOfWeek.Tuesday => 3,
        DayOfWeek.Wednesday => 4,
        DayOfWeek.Thursday => 5,
        DayOfWeek.Friday => 6,
        DayOfWeek.Saturday => 7,
        DayOfWeek.Sunday => 8,
        _ => 0
    };

    private static string DetectSession(string startTime)
    {
        if (!TimeOnly.TryParse(startTime, out var time))
        {
            return "Khac";
        }

        if (time < new TimeOnly(12, 0))
        {
            return "Sang";
        }

        if (time < new TimeOnly(18, 0))
        {
            return "Chieu";
        }

        return "Toi";
    }

    private static DateOnly GetStartOfWeek(DateOnly date)
    {
        var normalizedDay = NormalizeDayOfWeek(date.DayOfWeek);
        var offset = normalizedDay - 2;
        return date.AddDays(-offset);
    }

    private static DateOnly FindNextDateByDayOfWeek(DateOnly fromDate, int targetDayOfWeek)
    {
        var normalizedCurrentDay = NormalizeDayOfWeek(fromDate.DayOfWeek);
        var offset = targetDayOfWeek - normalizedCurrentDay;
        if (offset < 0)
        {
            offset += 7;
        }

        return fromDate.AddDays(offset);
    }
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

public class StudentProfileRegistrationModel
{
    public string HoTen { get; set; } = string.Empty;
    public string NgaySinh { get; set; } = string.Empty;
    public string GioiTinh { get; set; } = string.Empty;
    public string Cccd { get; set; } = string.Empty;
    public string DiaChi { get; set; } = string.Empty;
    public string AnhChanDung { get; set; } = string.Empty;
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
    public string PaymentStatus { get; set; } = string.Empty;
    public string StudyTimeText { get; set; } = string.Empty;
    public List<StudentCourseScheduleItem> ScheduleDetails { get; set; } = new();
}

public class StudentCourseRegistrationItem
{
    public int RegistrationId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string ScheduleText { get; set; } = string.Empty;
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public string RawStatus { get; set; } = string.Empty;
    public string RawPaymentStatus { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public List<StudentCourseScheduleItem> ScheduleDetails { get; set; } = new();
    public bool CanPayWithZaloPay { get; set; }
    public string? PaymentDisabledReason { get; set; }
    public string? ReceiptId { get; set; }
    public long TuitionFee { get; set; }
    public string TeacherName { get; set; } = string.Empty;
}

public class StudentCourseScheduleItem
{
    public int DayOfWeek { get; set; }
    public string DayLabel { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string CourseName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
}

public class StudentScheduleItem
{
    public string DayLabel { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string TimeText { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string AccentClass { get; set; } = "primary";
}

public class StudentPaidCourseScheduleViewModel
{
    public int RegistrationId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public string StudyTimeText { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public List<StudentCourseScheduleItem> ScheduleDetails { get; set; } = new();
    public WeeklyScheduleTableViewModel ScheduleTable { get; set; } = new();
    public int TotalSessions { get; set; }
}
