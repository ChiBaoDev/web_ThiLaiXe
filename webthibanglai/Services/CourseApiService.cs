using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using webthibanglai.Models;

namespace webthibanglai.Services;

public interface ICourseApiService
{
    Task<KhoaHocViewModel> GetCoursesAsync(CancellationToken cancellationToken = default);
    Task<KhoaHocDetailViewModel> GetCourseDetailAsync(int courseId, CancellationToken cancellationToken = default);
    Task<CourseRegistrationResult> RegisterCourseAsync(string? accessToken, int courseId, string? ghiChu, CancellationToken cancellationToken = default);
}

public record CourseRegistrationResult(bool IsSuccess, bool RequiresLogin, bool RequiresStudentProfile, string Message);

public sealed class CourseApiService : ICourseApiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<CourseApiService> _logger;

    public CourseApiService(IHttpClientFactory httpClientFactory, ILogger<CourseApiService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<KhoaHocViewModel> GetCoursesAsync(CancellationToken cancellationToken = default)
    {
        var model = new KhoaHocViewModel();

        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync("/api/v1/courses?page=1&pageSize=50", cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Get courses failed. StatusCode={StatusCode}, Response={Response}", response.StatusCode, responseBody);
                model.ErrorMessage = "Không tải được danh sách khóa học từ hệ thống.";
                return model;
            }

            var apiResponse = JsonSerializer.Deserialize<ApiEnvelope<PagedResult<CourseApiItem>>>(responseBody, JsonOptions());
            var courses = apiResponse?.Data?.Items ?? new List<CourseApiItem>();

            model.Courses = courses.Select(MapCourse).ToList();
            model.TotalCourses = model.Courses.Count;
            model.OpenCourses = model.Courses.Count(item => item.IsOpenForRegistration);

            if (model.Courses.Count > 0)
            {
                model.LowestPrice = model.Courses.Min(item => item.HocPhi);
            }

            if (model.Courses.Count == 0)
            {
                model.ErrorMessage = apiResponse?.Message ?? "Hiện chưa có khóa học nào để hiển thị.";
            }

            return model;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting courses.");
            model.ErrorMessage = "Đã xảy ra lỗi khi tải danh sách khóa học.";
            return model;
        }
    }

    public async Task<KhoaHocDetailViewModel> GetCourseDetailAsync(int courseId, CancellationToken cancellationToken = default)
    {
        var model = new KhoaHocDetailViewModel();

        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"/api/v1/courses/{courseId}", cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Get course detail failed. CourseId={CourseId}, StatusCode={StatusCode}, Response={Response}", courseId, response.StatusCode, responseBody);
                model.ErrorMessage = response.StatusCode == System.Net.HttpStatusCode.NotFound
                    ? "Không tìm thấy khóa học bạn đang xem."
                    : "Không tải được chi tiết khóa học từ hệ thống.";
                return model;
            }

            var apiResponse = JsonSerializer.Deserialize<ApiEnvelope<CourseDetailApiItem>>(responseBody, JsonOptions());
            if (apiResponse?.Data is null)
            {
                model.ErrorMessage = "Dữ liệu chi tiết khóa học không hợp lệ.";
                return model;
            }

            model.Course = MapCourseDetail(apiResponse.Data);
            return model;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting course detail. CourseId={CourseId}", courseId);
            model.ErrorMessage = "Đã xảy ra lỗi khi tải chi tiết khóa học.";
            return model;
        }
    }

    public async Task<CourseRegistrationResult> RegisterCourseAsync(string? accessToken, int courseId, string? ghiChu, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return new CourseRegistrationResult(false, true, false, "Bạn cần đăng nhập trước khi đăng ký khóa học.");
        }

        if (courseId <= 0)
        {
            return new CourseRegistrationResult(false, false, false, "Mã khóa học không hợp lệ.");
        }

        var client = CreateAuthorizedClient(accessToken);

        try
        {
            var profileResponse = await client.GetAsync("/api/v1/auth/me/student-profile", cancellationToken);
            var profileResponseBody = await profileResponse.Content.ReadAsStringAsync(cancellationToken);

            if (profileResponse.StatusCode == HttpStatusCode.NotFound)
            {
                return new CourseRegistrationResult(false, false, true, "Bạn cần đăng ký hồ sơ học viên trước khi đăng ký khóa học.");
            }

            if (profileResponse.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            {
                return new CourseRegistrationResult(false, true, false, "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.");
            }

            if (!profileResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning("Check student profile failed before course registration. StatusCode={StatusCode}, Response={Response}", profileResponse.StatusCode, profileResponseBody);
                var profileErrorMessage = ExtractErrorMessage(profileResponseBody);
                if (profileResponse.StatusCode == HttpStatusCode.InternalServerError && IsUnexpectedServerErrorMessage(profileErrorMessage))
                {
                    return new CourseRegistrationResult(false, false, true, "Bạn cần đăng ký hồ sơ học viên trước khi đăng ký khóa học.");
                }

                return new CourseRegistrationResult(false, false, false, profileErrorMessage ?? "Không kiểm tra được hồ sơ học viên. Vui lòng thử lại sau.");
            }

            var profileApiResponse = JsonSerializer.Deserialize<ApiEnvelope<StudentProfileApiItem>>(profileResponseBody, JsonOptions());
            if (profileApiResponse?.Data is null)
            {
                return new CourseRegistrationResult(false, false, true, "Bạn cần đăng ký hồ sơ học viên trước khi đăng ký khóa học.");
            }

            var payload = new CourseRegistrationRequest
            {
                CourseId = courseId,
                GhiChu = string.IsNullOrWhiteSpace(ghiChu) ? null : ghiChu.Trim()
            };

            var content = new StringContent(JsonSerializer.Serialize(payload, JsonOptions()), Encoding.UTF8, "application/json");
            var registrationResponse = await client.PostAsync("/api/v1/course-registrations", content, cancellationToken);
            var registrationResponseBody = await registrationResponse.Content.ReadAsStringAsync(cancellationToken);

            if (registrationResponse.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            {
                return new CourseRegistrationResult(false, true, false, "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.");
            }

            if (!registrationResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning("Register course failed. CourseId={CourseId}, StatusCode={StatusCode}, Response={Response}", courseId, registrationResponse.StatusCode, registrationResponseBody);
                var errorMessage = ExtractErrorMessage(registrationResponseBody) ?? "Đăng ký khóa học thất bại. Vui lòng thử lại sau.";
                if (IsMissingStudentProfileMessage(errorMessage))
                {
                    return new CourseRegistrationResult(false, false, true, "Bạn cần đăng ký hồ sơ học viên trước khi đăng ký khóa học.");
                }

                return new CourseRegistrationResult(false, false, false, errorMessage);
            }

            var apiResponse = JsonSerializer.Deserialize<ApiEnvelope<CourseRegistrationApiItem>>(registrationResponseBody, JsonOptions());
            var registeredCourseName = apiResponse?.Data?.TenKhoaHoc;
            var message = !string.IsNullOrWhiteSpace(apiResponse?.Message)
                ? apiResponse.Message!
                : !string.IsNullOrWhiteSpace(registeredCourseName)
                    ? $"Đăng ký khóa học {registeredCourseName} thành công."
                    : "Đăng ký khóa học thành công.";

            return new CourseRegistrationResult(true, false, false, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while registering course. CourseId={CourseId}", courseId);
            return new CourseRegistrationResult(false, false, false, "Đã xảy ra lỗi khi đăng ký khóa học.");
        }
    }

    private HttpClient CreateAuthorizedClient(string accessToken)
    {
        var client = _httpClientFactory.CreateClient("ApiClient");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        return client;
    }

    private static KhoaHocCourseItem MapCourse(CourseApiItem course)
    {
        var occupancyRate = course.SoLuongToiDa <= 0
            ? 0
            : (int)Math.Round((double)course.SoLuongHienTai / course.SoLuongToiDa * 100, MidpointRounding.AwayFromZero);

        return new KhoaHocCourseItem
        {
            CourseId = course.CourseId,
            MaKhoaHoc = course.MaKhoaHoc ?? string.Empty,
            TenKhoaHoc = course.TenKhoaHoc ?? string.Empty,
            LoaiBangLai = course.LoaiBangLai ?? "Chưa xác định",
            HocPhi = course.HocPhi,
            SoBuoiHoc = course.SoBuoiHoc,
            ThoiGianBatDau = course.NgayBatDau,
            ThoiGianKetThuc = course.NgayKetThuc,
            TrangThai = course.TrangThai ?? "SapKhaiGiang",
            SoLuongToiDa = course.SoLuongToiDa,
            SoLuongHienTai = course.SoLuongHienTai,
            MoTaNgan = course.MoTaNgan ?? "Khóa học đang được cập nhật mô tả chi tiết.",
            LichHocTomTat = course.LichHocTomTat,
            HinhAnh = string.IsNullOrWhiteSpace(course.HinhAnh) ? "~/img/courses-1.jpg" : course.HinhAnh,
            IsOpenForRegistration = course.IsOpenForRegistration,
            OccupancyRate = Math.Clamp(occupancyRate, 0, 100)
        };
    }

    private static KhoaHocCourseDetail MapCourseDetail(CourseDetailApiItem course)
    {
        var occupancyRate = course.SoLuongToiDa <= 0
            ? 0
            : (int)Math.Round((double)course.SoLuongHienTai / course.SoLuongToiDa * 100, MidpointRounding.AwayFromZero);

        return new KhoaHocCourseDetail
        {
            CourseId = course.CourseId,
            MaKhoaHoc = course.MaKhoaHoc ?? string.Empty,
            TenKhoaHoc = course.TenKhoaHoc ?? string.Empty,
            LoaiBangLai = course.LoaiBangLai ?? "Chưa xác định",
            MoTa = course.MoTa ?? "Khóa học đang được cập nhật nội dung chi tiết.",
            HocPhi = course.HocPhi,
            SoBuoiHoc = course.SoBuoiHoc,
            SoLuongToiDa = course.SoLuongToiDa,
            SoLuongHienTai = course.SoLuongHienTai,
            NgayBatDau = course.NgayBatDau,
            NgayKetThuc = course.NgayKetThuc,
            TrangThai = course.TrangThai ?? "SapKhaiGiang",
            GiaoVienChinh = course.GiaoVienChinh?.HoTen,
            SoDienThoaiGiaoVien = course.GiaoVienChinh?.SoDienThoai,
            LichHocMau = course.LichHocMau.Select(item => new KhoaHocScheduleItem
            {
                ThuTrongTuan = item.ThuTrongTuan,
                GioBatDau = item.GioBatDau ?? string.Empty,
                GioKetThuc = item.GioKetThuc ?? string.Empty,
                DiaDiem = item.DiaDiem ?? string.Empty
            }).ToList(),
            HinhAnh = string.IsNullOrWhiteSpace(course.HinhAnh) ? "~/img/courses-1.jpg" : course.HinhAnh,
            IsOpenForRegistration = IsOpenForRegistration(course.TrangThai, course.SoLuongHienTai, course.SoLuongToiDa),
            OccupancyRate = Math.Clamp(occupancyRate, 0, 100)
        };
    }

    private static bool IsOpenForRegistration(string? status, int soLuongHienTai, int soLuongToiDa)
    {
        var normalizedStatus = status?.Trim();
        var isOpenStatus = string.Equals(normalizedStatus, "DangMo", StringComparison.OrdinalIgnoreCase)
            || string.Equals(normalizedStatus, "DangMoDangKy", StringComparison.OrdinalIgnoreCase);

        if (!isOpenStatus)
        {
            return false;
        }

        if (soLuongToiDa <= 0)
        {
            return true;
        }

        return soLuongHienTai < soLuongToiDa;
    }

    private static JsonSerializerOptions JsonOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        options.Converters.Add(new FlexibleLongJsonConverter());
        return options;
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

    private static bool TryGetPropertyIgnoreCase(JsonElement element, string propertyName, out JsonElement value)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in element.EnumerateObject())
            {
                if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    value = property.Value;
                    return true;
                }
            }
        }

        value = default;
        return false;
    }

    private static bool IsMissingStudentProfileMessage(string message)
    {
        return message.Contains("học viên", StringComparison.OrdinalIgnoreCase)
            || message.Contains("hoc vien", StringComparison.OrdinalIgnoreCase)
            || message.Contains("student", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsUnexpectedServerErrorMessage(string? message)
    {
        return string.IsNullOrWhiteSpace(message)
            || string.Equals(message.Trim(), "An unexpected error occurred", StringComparison.OrdinalIgnoreCase);
    }

    private sealed class FlexibleLongJsonConverter : JsonConverter<long>
    {
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.TryGetInt64(out var longValue))
                {
                    return longValue;
                }

                if (reader.TryGetDecimal(out var decimalValue))
                {
                    return decimal.ToInt64(decimalValue);
                }
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                var rawValue = reader.GetString();
                if (string.IsNullOrWhiteSpace(rawValue))
                {
                    return 0;
                }

                if (long.TryParse(rawValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedLong))
                {
                    return parsedLong;
                }

                if (decimal.TryParse(rawValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedDecimal))
                {
                    return decimal.ToInt64(parsedDecimal);
                }
            }

            throw new JsonException("Không thể chuyển giá trị JSON sang kiểu long.");
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }

    private sealed class ApiEnvelope<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
    }

    private sealed class CourseRegistrationRequest
    {
        public int CourseId { get; set; }
        public string? GhiChu { get; set; }
    }

    private sealed class CourseRegistrationApiItem
    {
        public int RegistrationId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public string? TenKhoaHoc { get; set; }
        public DateTime? NgayDangKy { get; set; }
        public string? TrangThai { get; set; }
        public string? GhiChu { get; set; }
    }

    private sealed class StudentProfileApiItem
    {
        public long HocVienId { get; set; }
    }

    private sealed class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
    }

    private sealed class CourseApiItem
    {
        public int CourseId { get; set; }
        public string? MaKhoaHoc { get; set; }
        public string? TenKhoaHoc { get; set; }
        public string? LoaiBangLai { get; set; }
        public string? MoTaNgan { get; set; }
        public long HocPhi { get; set; }
        public int SoBuoiHoc { get; set; }
        public DateOnly? NgayBatDau { get; set; }
        public DateOnly? NgayKetThuc { get; set; }
        public string? LichHocTomTat { get; set; }
        public string? TrangThai { get; set; }
        public string? HinhAnh { get; set; }
        public bool IsOpenForRegistration { get; set; }
        public int SoLuongToiDa { get; set; }
        public int SoLuongHienTai { get; set; }
    }

    private sealed class CourseDetailApiItem
    {
        public int CourseId { get; set; }
        public string? MaKhoaHoc { get; set; }
        public string? TenKhoaHoc { get; set; }
        public string? LoaiBangLai { get; set; }
        public string? MoTa { get; set; }
        public long HocPhi { get; set; }
        public int SoBuoiHoc { get; set; }
        public int SoLuongToiDa { get; set; }
        public int SoLuongHienTai { get; set; }
        public DateOnly? NgayBatDau { get; set; }
        public DateOnly? NgayKetThuc { get; set; }
        public string? TrangThai { get; set; }
        public CourseTeacherApiItem? GiaoVienChinh { get; set; }
        public List<CourseScheduleApiItem> LichHocMau { get; set; } = new();
        public string? HinhAnh { get; set; }
    }

    private sealed class CourseTeacherApiItem
    {
        public int TeacherId { get; set; }
        public string? HoTen { get; set; }
        public string? SoDienThoai { get; set; }
    }

    private sealed class CourseScheduleApiItem
    {
        public int ThuTrongTuan { get; set; }
        public string? GioBatDau { get; set; }
        public string? GioKetThuc { get; set; }
        public string? DiaDiem { get; set; }
    }
}
