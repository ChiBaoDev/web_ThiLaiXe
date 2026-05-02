using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using webthibanglai.Models;

namespace webthibanglai.Services;

public interface ICourseApiService
{
    Task<KhoaHocViewModel> GetCoursesAsync(CancellationToken cancellationToken = default);
}

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

    private static JsonSerializerOptions JsonOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        options.Converters.Add(new FlexibleLongJsonConverter());
        return options;
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
        public T? Data { get; set; }
        public string? Message { get; set; }
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
}
