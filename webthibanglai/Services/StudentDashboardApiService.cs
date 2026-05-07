using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using webthibanglai.Models;

namespace webthibanglai.Services;

public interface IStudentDashboardApiService
{
    Task<LichHocViewModel> GetDashboardAsync(string? accessToken, CancellationToken cancellationToken = default);
    Task<RegisterStudentProfileResult> RegisterStudentProfileAsync(string? accessToken, StudentProfileRegistrationModel request, CancellationToken cancellationToken = default);
}

public record RegisterStudentProfileResult(bool IsSuccess, string? ErrorMessage, LichHocViewModel? Dashboard);

public class StudentDashboardApiService : IStudentDashboardApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<StudentDashboardApiService> _logger;

    public StudentDashboardApiService(IHttpClientFactory httpClientFactory, ILogger<StudentDashboardApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<LichHocViewModel> GetDashboardAsync(string? accessToken, CancellationToken cancellationToken = default)
    {
        var model = BuildStaticModel();
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return model;
        }

        var client = CreateAuthorizedClient(accessToken);

        try
        {
            var meTask = client.GetAsync("/api/v1/auth/me", cancellationToken);
            var studentProfileTask = client.GetAsync("/api/v1/auth/me/student-profile", cancellationToken);
            var historyTask = client.GetAsync("/api/v1/history/analytics", cancellationToken);
            var wrongSummaryTask = client.GetAsync("/api/v1/wrong-questions/summary", cancellationToken);
            var criticalStatsTask = client.GetAsync("/api/v1/dashboard/critical-question-stats", cancellationToken);
            var courseRegistrationsTask = client.GetAsync("/api/v1/my/course-registrations?page=1&pageSize=10", cancellationToken);

            await Task.WhenAll(meTask, studentProfileTask, historyTask, wrongSummaryTask, criticalStatsTask, courseRegistrationsTask);

            await PopulateProfileAsync(model, meTask.Result, cancellationToken);
            await PopulateStudentProfileAsync(model, studentProfileTask.Result, cancellationToken);
            await PopulateHistoryStatsAsync(model, historyTask.Result, cancellationToken);
            await PopulateWrongQuestionStatsAsync(model, wrongSummaryTask.Result, cancellationToken);
            await PopulateCriticalStatsAsync(model, criticalStatsTask.Result, cancellationToken);
            await PopulateCourseRegistrationsAsync(model, courseRegistrationsTask.Result, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Không thể tải đầy đủ dữ liệu dashboard học viên từ API.");
        }

        model.Stats.PracticeCount = model.Stats.TotalExams;
        return model;
    }

    public async Task<RegisterStudentProfileResult> RegisterStudentProfileAsync(string? accessToken, StudentProfileRegistrationModel request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return new RegisterStudentProfileResult(false, "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.", null);
        }

        var client = CreateAuthorizedClient(accessToken);
        var payload = new
        {
            ho_ten = request.HoTen.Trim(),
            ngay_sinh = string.IsNullOrWhiteSpace(request.NgaySinh) ? null : request.NgaySinh.Trim(),
            gioi_tinh = string.IsNullOrWhiteSpace(request.GioiTinh) ? null : request.GioiTinh.Trim(),
            cccd = string.IsNullOrWhiteSpace(request.Cccd) ? null : request.Cccd.Trim(),
            dia_chi = string.IsNullOrWhiteSpace(request.DiaChi) ? null : request.DiaChi.Trim(),
            anh_chan_dung = string.IsNullOrWhiteSpace(request.AnhChanDung) ? null : request.AnhChanDung.Trim()
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
        var response = await client.PostAsync("/api/v1/auth/register-student-profile", content, cancellationToken);
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorMessage = ExtractErrorMessage(responseBody);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                errorMessage ??= "Không tìm thấy API đăng ký hồ sơ học viên.";
            }

            return new RegisterStudentProfileResult(false, errorMessage ?? "Đăng ký hồ sơ học viên thất bại.", null);
        }

        var dashboard = await GetDashboardAsync(accessToken, cancellationToken);
        return new RegisterStudentProfileResult(true, null, dashboard);
    }

    private async Task PopulateProfileAsync(LichHocViewModel model, HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Get auth/me failed. StatusCode={StatusCode}, Response={Response}", response.StatusCode, responseBody);
            return;
        }

        var apiResponse = Deserialize<ApiEnvelope<CurrentUserInfo>>(responseBody);
        var user = apiResponse?.Data;
        if (user == null)
        {
            return;
        }

        var displayName = !string.IsNullOrWhiteSpace(user.HoTen) ? user.HoTen : user.TenDangNhap;
        var isAdmin = user.Roles.Any(x => string.Equals(x, "ADMIN", StringComparison.OrdinalIgnoreCase));

        model.Profile = new StudentDashboardProfile
        {
            UserId = user.UserId,
            HocVienId = user.HocVienId,
            HoTen = displayName,
            TenDangNhap = user.TenDangNhap,
            Email = user.Email,
            SoDienThoai = user.SoDienThoai,
            GioiTinh = user.GioiTinh,
            NgaySinhText = user.NgaySinh?.ToString("dd/MM/yyyy") ?? "Chưa cập nhật",
            Cccd = user.Cccd,
            DiaChi = user.DiaChi,
            AnhChanDung = user.AnhChanDung,
            TrangThai = string.IsNullOrWhiteSpace(user.TrangThai) ? "Đang hoạt động" : user.TrangThai,
            Initials = BuildInitials(displayName),
            RoleLabel = isAdmin ? "Quản trị viên" : "Học viên"
        };

        model.Registration.HoTen = displayName;
    }

    private async Task PopulateStudentProfileAsync(LichHocViewModel model, HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            model.HasStudentProfile = false;
            return;
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Get auth/me/student-profile failed. StatusCode={StatusCode}, Response={Response}", response.StatusCode, responseBody);
            model.HasStudentProfile = false;
            return;
        }

        var apiResponse = Deserialize<ApiEnvelope<StudentProfileApiResponse>>(responseBody);
        var profile = apiResponse?.Data;
        if (profile == null)
        {
            model.HasStudentProfile = false;
            return;
        }

        model.HasStudentProfile = true;
        model.Profile.HocVienId = profile.HocVienId;
        model.Profile.HoTen = string.IsNullOrWhiteSpace(profile.HoTen) ? model.Profile.HoTen : profile.HoTen;
        model.Profile.GioiTinh = profile.GioiTinh ?? string.Empty;
        model.Profile.NgaySinhText = profile.NgaySinh?.ToString("dd/MM/yyyy") ?? "Chưa cập nhật";
        model.Profile.Cccd = profile.Cccd ?? string.Empty;
        model.Profile.DiaChi = profile.DiaChi ?? string.Empty;
        model.Profile.AnhChanDung = profile.AnhChanDung ?? string.Empty;
        model.Profile.Initials = BuildInitials(model.Profile.HoTen);

        model.Registration = new StudentProfileRegistrationModel
        {
            HoTen = model.Profile.HoTen,
            NgaySinh = profile.NgaySinh?.ToString("yyyy-MM-dd") ?? string.Empty,
            GioiTinh = profile.GioiTinh ?? string.Empty,
            Cccd = profile.Cccd ?? string.Empty,
            DiaChi = profile.DiaChi ?? string.Empty,
            AnhChanDung = profile.AnhChanDung ?? string.Empty
        };
    }

    private async Task PopulateHistoryStatsAsync(LichHocViewModel model, HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Get history analytics failed. StatusCode={StatusCode}, Response={Response}", response.StatusCode, responseBody);
            return;
        }

        using var document = JsonDocument.Parse(responseBody);
        var data = TryGetDataElement(document.RootElement);
        if (data == null)
        {
            return;
        }

        var totalExams = ReadInt(data.Value, "total_sessions", "totalSessions", "session_count", "exam_count");
        var passedExams = ReadInt(data.Value, "passed_sessions", "passedSessions", "passed_count");
        var failedExams = ReadInt(data.Value, "failed_sessions", "failedSessions", "failed_count");

        if (failedExams == 0 && totalExams > 0 && passedExams <= totalExams)
        {
            failedExams = totalExams - passedExams;
        }

        var passRate = ReadDecimal(data.Value, "pass_rate", "passRate");
        if (passRate <= 0 && totalExams > 0)
        {
            passRate = Math.Round((decimal)passedExams * 100m / totalExams, 2);
        }

        model.Stats.TotalExams = totalExams;
        model.Stats.PassedExams = passedExams;
        model.Stats.FailedExams = failedExams;
        model.Stats.PassRate = passRate;
        model.Stats.AverageScore = ReadDecimal(data.Value, "average_score", "averageScore", "avg_score");
        model.Stats.CorrectAnswers = ReadInt(data.Value, "total_correct_answers", "correct_answers", "correctAnswers");
        model.Stats.WrongAnswers = ReadInt(data.Value, "total_wrong_answers", "wrong_answers", "wrongAnswers");

        if (model.Stats.CorrectAnswers == 0 && model.Stats.WrongAnswers == 0)
        {
            model.Stats.CorrectAnswers = passedExams * 21;
            model.Stats.WrongAnswers = Math.Max(totalExams * 25 - model.Stats.CorrectAnswers, 0);
        }
    }

    private async Task PopulateWrongQuestionStatsAsync(LichHocViewModel model, HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Get wrong-questions summary failed. StatusCode={StatusCode}, Response={Response}", response.StatusCode, responseBody);
            return;
        }

        using var document = JsonDocument.Parse(responseBody);
        var data = TryGetDataElement(document.RootElement);
        if (data == null)
        {
            return;
        }

        model.Stats.WrongQuestionCount = ReadInt(data.Value, "total_wrong_questions", "totalWrongQuestions", "wrong_count", "wrongQuestions");
        if (model.Stats.WrongAnswers == 0)
        {
            model.Stats.WrongAnswers = model.Stats.WrongQuestionCount;
        }
    }

    private async Task PopulateCriticalStatsAsync(LichHocViewModel model, HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Get critical-question-stats failed. StatusCode={StatusCode}, Response={Response}", response.StatusCode, responseBody);
            return;
        }

        var apiResponse = Deserialize<ApiEnvelope<HomeCriticalQuestionStats>>(responseBody);
        var data = apiResponse?.Data;
        if (data == null)
        {
            using var document = JsonDocument.Parse(responseBody);
            var fallback = TryGetDataElement(document.RootElement);
            if (fallback != null)
            {
                model.Stats.TotalCriticalAttempts = ReadInt(fallback.Value, "total_critical_attempts", "totalCriticalAttempts");
                model.Stats.CriticalWrongCount = ReadInt(fallback.Value, "wrong_critical_attempts", "wrongCriticalAttempts", "critical_wrong_count");
                model.Stats.CriticalErrorRate = ReadDecimal(fallback.Value, "critical_error_rate", "criticalErrorRate");
            }

            return;
        }

        model.Stats.TotalCriticalAttempts = data.TotalCriticalAttempts;
        model.Stats.CriticalWrongCount = data.WrongCriticalAttempts;
        model.Stats.CriticalErrorRate = data.CriticalErrorRate;
    }

    private HttpClient CreateAuthorizedClient(string accessToken)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }

    private static LichHocViewModel BuildStaticModel()
    {
        return new LichHocViewModel
        {
            Profile = new StudentDashboardProfile
            {
                HoTen = "Học viên",
                TrangThai = "Đang học",
                Initials = "HV",
                RoleLabel = "Học viên",
                NgaySinhText = "Chưa cập nhật"
            },
            RegisteredCourses = new List<StudentRegisteredCourseItem>
            {
                new()
                {
                    Name = "Khóa học A1 cơ bản",
                    Description = "Lộ trình học lý thuyết, luyện đề và thực hành sát hạch dành cho học viên mới.",
                    ScheduleText = "Thứ 2 - Thứ 4 - Thứ 6, 18:30 - 20:00",
                    TeacherName = "GV. Nguyễn Hoàng Minh",
                    Status = "Đang học"
                },
                new()
                {
                    Name = "Khóa ôn tập cuối tuần",
                    Description = "Tăng cường luyện đề và sửa lỗi sai trước kỳ thi chính thức.",
                    ScheduleText = "Thứ 7 - Chủ nhật, 08:00 - 10:00",
                    TeacherName = "GV. Trần Gia Huy",
                    Status = "Sắp khai giảng"
                }
            },
            CourseRegistrations = new List<StudentCourseRegistrationItem>(),
            Schedule = new List<StudentScheduleItem>
            {
                new() { DayLabel = "Thứ 2", Title = "Lý thuyết biển báo và sa hình", TimeText = "18:30 - 20:00", Location = "Phòng học A1", AccentClass = "primary" },
                new() { DayLabel = "Thứ 4", Title = "Luyện đề và giải thích đáp án", TimeText = "18:30 - 20:30", Location = "Phòng máy số 2", AccentClass = "warning" },
                new() { DayLabel = "Thứ 6", Title = "Thực hành sa hình", TimeText = "17:30 - 19:00", Location = "Sân tập thực hành", AccentClass = "success" },
                new() { DayLabel = "Chủ nhật", Title = "Thi thử tổng hợp", TimeText = "08:00 - 10:00", Location = "Phòng thi mô phỏng", AccentClass = "danger" }
            }
        };
    }

    private async Task PopulateCourseRegistrationsAsync(LichHocViewModel model, HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("Get my course registrations failed. StatusCode={StatusCode}, Response={Response}", response.StatusCode, responseBody);
            return;
        }

        var apiResponse = Deserialize<StudentDashboardApiEnvelope<StudentDashboardPagedResult<CourseRegistrationApiItem>>>(responseBody);
        var registrations = apiResponse?.Data?.Items ?? new List<CourseRegistrationApiItem>();

        model.CourseRegistrations = registrations.Select(item => new StudentCourseRegistrationItem
        {
            RegistrationId = item.RegistrationId,
            CourseName = item.TenKhoaHoc ?? "Khóa học",
            ClassName = item.TenLop ?? "Chưa phân lớp",
            ScheduleText = BuildScheduleText(item.NgayBatDau, item.NgayKetThuc),
            Status = NormalizeRegistrationStatus(item.TrangThai),
            PaymentStatus = NormalizePaymentStatus(item.PaymentStatus),
            ScheduleDetails = item.LichHoc
                .OrderBy(schedule => schedule.ThuTrongTuan)
                .ThenBy(schedule => schedule.GioBatDau)
                .Select(schedule => new StudentCourseScheduleItem
                {
                    DayOfWeek = schedule.ThuTrongTuan,
                    DayLabel = FormatDayOfWeek(schedule.ThuTrongTuan),
                    StartTime = schedule.GioBatDau ?? string.Empty,
                    EndTime = schedule.GioKetThuc ?? string.Empty,
                    Location = string.IsNullOrWhiteSpace(schedule.DiaDiem) ? "Theo phân công trung tâm" : schedule.DiaDiem!
                }).ToList(),
            CanPayWithZaloPay = CanShowPaymentButton(item.TrangThai, item.PaymentStatus),
            PaymentDisabledReason = BuildPaymentDisabledReason(item.TrangThai, item.PaymentStatus),
            ReceiptId = item.ReceiptId?.ToString(),
            TuitionFee = item.HocPhi,
            TeacherName = string.IsNullOrWhiteSpace(item.GiaoVien) ? "Theo phân công trung tâm" : item.GiaoVien!
        }).ToList();

        if (model.CourseRegistrations.Count > 0)
        {
            model.RegisteredCourses = model.CourseRegistrations.Select(item => new StudentRegisteredCourseItem
            {
                Name = item.CourseName,
                Description = $"Lớp: {item.ClassName}",
                ScheduleText = item.ScheduleText,
                TeacherName = item.TeacherName,
                Status = item.Status,
                PaymentStatus = item.PaymentStatus,
                StudyTimeText = item.ScheduleText,
                ScheduleDetails = item.ScheduleDetails
            }).ToList();
        }
    }

    private static string? BuildPaymentDisabledReason(string? registrationStatus, string? paymentStatus)
    {
        if (CanShowPaymentButton(registrationStatus, paymentStatus))
        {
            return null;
        }

        if (IsPaymentSuccessStatus(paymentStatus))
        {
            return "Đăng ký này đã hoàn tất thanh toán.";
        }

        if (!IsApprovedRegistrationStatus(registrationStatus))
        {
            return "Bạn chỉ có thể thanh toán sau khi đăng ký được duyệt.";
        }

        return "Hiện chưa thể thanh toán cho đăng ký này.";
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

    private static bool IsPendingPaymentStatus(string? status)
    {
        return string.IsNullOrWhiteSpace(status)
            || string.Equals(status, "chua_thanh_toan", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "ChuaThanhToan", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "chưa thanh toán", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "cho_xac_nhan", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "ChoXacNhan", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "chờ xác nhận", StringComparison.OrdinalIgnoreCase);
    }

    private static string NormalizeRegistrationStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return "Chờ duyệt";
        }

        if (IsApprovedRegistrationStatus(status))
        {
            return "Đã duyệt";
        }

        if (string.Equals(status, "cho_duyet", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "ChoDuyet", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "chờ duyệt", StringComparison.OrdinalIgnoreCase))
        {
            return "Chờ duyệt";
        }

        return status;
    }

    private static string NormalizePaymentStatus(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return "Chưa thanh toán";
        }

        if (IsPaymentSuccessStatus(status))
        {
            return "Đã thanh toán";
        }

        if (string.Equals(status, "cho_xac_nhan", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "ChoXacNhan", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "chờ xác nhận", StringComparison.OrdinalIgnoreCase))
        {
            return "Chờ xác nhận";
        }

        if (string.Equals(status, "chua_thanh_toan", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "ChuaThanhToan", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "chưa thanh toán", StringComparison.OrdinalIgnoreCase))
        {
            return "Chưa thanh toán";
        }

        return status;
    }

    private static bool CanShowPaymentButton(string? registrationStatus, string? paymentStatus)
    {
        if (IsPaymentSuccessStatus(paymentStatus))
        {
            return false;
        }

        if (IsPendingPaymentStatus(paymentStatus))
        {
            return true;
        }

        return IsApprovedRegistrationStatus(registrationStatus) && !IsPaymentSuccessStatus(paymentStatus);
    }

    private static string BuildScheduleText(DateOnly? startDate, DateOnly? endDate)
    {
        if (startDate.HasValue && endDate.HasValue)
        {
            return $"{startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy}";
        }

        if (startDate.HasValue)
        {
            return startDate.Value.ToString("dd/MM/yyyy");
        }

        return "Đang cập nhật lịch học";
    }

    private static JsonElement? TryGetDataElement(JsonElement root)
    {
        if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("data", out var data))
        {
            return data;
        }

        return root.ValueKind == JsonValueKind.Object ? root : null;
    }

    private static int ReadInt(JsonElement element, params string[] names)
    {
        foreach (var name in names)
        {
            if (!TryGetPropertyIgnoreCase(element, name, out var value))
            {
                continue;
            }

            if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var intValue))
            {
                return intValue;
            }

            if (value.ValueKind == JsonValueKind.String && int.TryParse(value.GetString(), out intValue))
            {
                return intValue;
            }
        }

        return 0;
    }

    private static decimal ReadDecimal(JsonElement element, params string[] names)
    {
        foreach (var name in names)
        {
            if (!TryGetPropertyIgnoreCase(element, name, out var value))
            {
                continue;
            }

            if (value.ValueKind == JsonValueKind.Number && value.TryGetDecimal(out var decimalValue))
            {
                return decimalValue;
            }

            if (value.ValueKind == JsonValueKind.String && decimal.TryParse(value.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimalValue))
            {
                return decimalValue;
            }
        }

        return 0;
    }

    private static bool TryGetPropertyIgnoreCase(JsonElement element, string propertyName, out JsonElement value)
    {
        foreach (var property in element.EnumerateObject())
        {
            if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                value = property.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    private static T? Deserialize<T>(string responseBody)
    {
        return JsonSerializer.Deserialize<T>(responseBody, JsonOptions());
    }

    private static string? ExtractErrorMessage(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
        {
            return null;
        }

        try
        {
            using var document = JsonDocument.Parse(responseBody);
            var root = document.RootElement;

            if (TryGetPropertyIgnoreCase(root, "message", out var messageElement) && messageElement.ValueKind == JsonValueKind.String)
            {
                return messageElement.GetString();
            }

            if (TryGetPropertyIgnoreCase(root, "errors", out var errorsElement) && errorsElement.ValueKind == JsonValueKind.Array)
            {
                var details = errorsElement.EnumerateArray()
                    .Select(item =>
                    {
                        if (item.ValueKind == JsonValueKind.String)
                        {
                            return item.GetString();
                        }

                        if (item.ValueKind == JsonValueKind.Object && TryGetPropertyIgnoreCase(item, "detail", out var detailElement) && detailElement.ValueKind == JsonValueKind.String)
                        {
                            return detailElement.GetString();
                        }

                        return null;
                    })
                    .Where(item => !string.IsNullOrWhiteSpace(item));

                var combined = string.Join(" ", details!);
                return string.IsNullOrWhiteSpace(combined) ? null : combined;
            }
        }
        catch (JsonException)
        {
            return null;
        }

        return null;
    }

    private static JsonSerializerOptions JsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
    }

    private static string BuildInitials(string? fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
        {
            return "HV";
        }

        var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Take(2)
            .Select(x => char.ToUpperInvariant(x[0]));

        var initials = string.Concat(parts);
        return string.IsNullOrWhiteSpace(initials) ? "HV" : initials;
    }
}

internal sealed class StudentProfileApiResponse
{
    public long HocVienId { get; set; }
    public long UserId { get; set; }
    public string HoTen { get; set; } = string.Empty;
    public DateOnly? NgaySinh { get; set; }
    public string? GioiTinh { get; set; }
    public string? Cccd { get; set; }
    public string? DiaChi { get; set; }
    public string? AnhChanDung { get; set; }
}

internal sealed class StudentDashboardApiEnvelope<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
}

internal sealed class StudentDashboardPagedResult<T>
{
    public List<T> Items { get; set; } = new();
}

internal sealed class CourseRegistrationApiItem
{
    public int RegistrationId { get; set; }
    public string? TenKhoaHoc { get; set; }
    public string? TenLop { get; set; }
    public DateOnly? NgayBatDau { get; set; }
    public DateOnly? NgayKetThuc { get; set; }
    public string? TrangThai { get; set; }
    public string? PaymentStatus { get; set; }
    public long HocPhi { get; set; }
    public long? ReceiptId { get; set; }
    public string? GiaoVien { get; set; }
    public List<CourseRegistrationScheduleApiItem> LichHoc { get; set; } = new();
}

internal sealed class CourseRegistrationScheduleApiItem
{
    public int ThuTrongTuan { get; set; }
    public string? GioBatDau { get; set; }
    public string? GioKetThuc { get; set; }
    public string? DiaDiem { get; set; }
}
