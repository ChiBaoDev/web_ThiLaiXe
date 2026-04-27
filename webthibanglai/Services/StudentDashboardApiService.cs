using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using webthibanglai.Models;

namespace webthibanglai.Services;

public interface IStudentDashboardApiService
{
    Task<LichHocViewModel> GetDashboardAsync(string? accessToken, CancellationToken cancellationToken = default);
}

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
            var historyTask = client.GetAsync("/api/v1/history/analytics", cancellationToken);
            var wrongSummaryTask = client.GetAsync("/api/v1/wrong-questions/summary", cancellationToken);
            var criticalStatsTask = client.GetAsync("/api/v1/dashboard/critical-question-stats", cancellationToken);

            await Task.WhenAll(meTask, historyTask, wrongSummaryTask, criticalStatsTask);

            await PopulateProfileAsync(model, meTask.Result, cancellationToken);
            await PopulateHistoryStatsAsync(model, historyTask.Result, cancellationToken);
            await PopulateWrongQuestionStatsAsync(model, wrongSummaryTask.Result, cancellationToken);
            await PopulateCriticalStatsAsync(model, criticalStatsTask.Result, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Không thể tải đầy đủ dữ liệu dashboard học viên từ API.");
        }

        model.Stats.PracticeCount = model.Stats.TotalExams;
        return model;
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
            Schedule = new List<StudentScheduleItem>
            {
                new() { DayLabel = "Thứ 2", Title = "Lý thuyết biển báo và sa hình", TimeText = "18:30 - 20:00", Location = "Phòng học A1", AccentClass = "primary" },
                new() { DayLabel = "Thứ 4", Title = "Luyện đề và giải thích đáp án", TimeText = "18:30 - 20:30", Location = "Phòng máy số 2", AccentClass = "warning" },
                new() { DayLabel = "Thứ 6", Title = "Thực hành sa hình", TimeText = "17:30 - 19:00", Location = "Sân tập thực hành", AccentClass = "success" },
                new() { DayLabel = "Chủ nhật", Title = "Thi thử tổng hợp", TimeText = "08:00 - 10:00", Location = "Phòng thi mô phỏng", AccentClass = "danger" }
            }
        };
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
