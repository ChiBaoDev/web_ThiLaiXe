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
    Task<CourseRegistrationResult> RegisterCourseAsync(string? accessToken, int courseId, int classId, string? ghiChu, CancellationToken cancellationToken = default);
    Task<MyCourseRegistrationsViewModel> GetMyCourseRegistrationsAsync(string? accessToken, string? paymentReceiptId = null, CancellationToken cancellationToken = default);
    Task<VnPayOrderResult> CreateVnPayOrderAsync(string? accessToken, int registrationId, CancellationToken cancellationToken = default);
    Task<VnPayReturnResult> ConfirmVnPayReturnAsync(IQueryCollection query, CancellationToken cancellationToken = default);
}

public record CourseRegistrationResult(bool IsSuccess, bool RequiresLogin, bool RequiresStudentProfile, string Message);
public record VnPayOrderResult(bool IsSuccess, bool RequiresLogin, string Message, string? OrderUrl, long? ReceiptId);
public record VnPayReturnResult(bool IsSuccess, string Message, long? ReceiptId, string PaymentStatus);

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
            model.Classes = await GetCourseClassesAsync(courseId, cancellationToken);
            return model;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting course detail. CourseId={CourseId}", courseId);
            model.ErrorMessage = "Đã xảy ra lỗi khi tải chi tiết khóa học.";
            return model;
        }
    }

    public async Task<CourseRegistrationResult> RegisterCourseAsync(string? accessToken, int courseId, int classId, string? ghiChu, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return new CourseRegistrationResult(false, true, false, "Bạn cần đăng nhập trước khi đăng ký khóa học.");
        }

        if (courseId <= 0)
        {
            return new CourseRegistrationResult(false, false, false, "Mã khóa học không hợp lệ.");
        }

        if (classId <= 0)
        {
            return new CourseRegistrationResult(false, false, false, "Vui lòng chọn lớp học muốn đăng ký.");
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
                ClassId = classId,
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

    public async Task<MyCourseRegistrationsViewModel> GetMyCourseRegistrationsAsync(string? accessToken, string? paymentReceiptId = null, CancellationToken cancellationToken = default)
    {
        var model = new MyCourseRegistrationsViewModel { IsLoading = true };

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            model.ErrorMessage = "Bạn cần đăng nhập để xem danh sách đăng ký.";
            model.IsLoading = false;
            return model;
        }

        try
        {
            var client = CreateAuthorizedClient(accessToken);
            var response = await client.GetAsync("/api/v1/my/course-registrations?page=1&pageSize=10", cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            {
                model.ErrorMessage = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
                model.IsLoading = false;
                return model;
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Get my course registrations failed. StatusCode={StatusCode}, Response={Response}", response.StatusCode, responseBody);
                model.ErrorMessage = ExtractErrorMessage(responseBody) ?? "Không tải được danh sách đăng ký của bạn.";
                model.IsLoading = false;
                return model;
            }

            var apiResponse = JsonSerializer.Deserialize<ApiEnvelope<PagedResult<MyCourseRegistrationApiItem>>>(responseBody, JsonOptions());
            var registrations = apiResponse?.Data?.Items ?? new List<MyCourseRegistrationApiItem>();
            model.Registrations = registrations.Select(MapMyCourseRegistration).ToList();

            if (!string.IsNullOrWhiteSpace(paymentReceiptId))
            {
                var paymentStatus = await GetPaymentReceiptStatusAsync(client, paymentReceiptId, cancellationToken);
                if (!string.IsNullOrWhiteSpace(paymentStatus.Message))
                {
                    model.StatusMessage = paymentStatus.Message;
                    model.StatusState = paymentStatus.State;
                }
            }

            model.IsLoading = false;
            return model;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting my course registrations.");
            model.ErrorMessage = "Đã xảy ra lỗi khi tải danh sách đăng ký của bạn.";
            model.IsLoading = false;
            return model;
        }
    }

    public async Task<VnPayOrderResult> CreateVnPayOrderAsync(string? accessToken, int registrationId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return new VnPayOrderResult(false, true, "Bạn cần đăng nhập để thanh toán VNPAY.", null, null);
        }

        if (registrationId <= 0)
        {
            return new VnPayOrderResult(false, false, "Mã đăng ký không hợp lệ.", null, null);
        }

        try
        {
            var client = CreateAuthorizedClient(accessToken);
            var payload = new { registrationId };
            var content = new StringContent(JsonSerializer.Serialize(payload, JsonOptions()), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/v1/payments/vnpay/create-order", content, cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden)
            {
                return new VnPayOrderResult(false, true, "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.", null, null);
            }

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Create VnPay order failed. RegistrationId={RegistrationId}, StatusCode={StatusCode}, Response={Response}", registrationId, response.StatusCode, responseBody);
                var apiErrorMessage = ExtractErrorMessage(responseBody) ?? "Không thể tạo đơn thanh toán VNPAY.";
                if (apiErrorMessage.Contains("Chưa cấu hình loại khoản thu học phí", StringComparison.OrdinalIgnoreCase))
                {
                    apiErrorMessage = "Hệ thống backend chưa cấu hình khoản thu học phí cho đăng ký này, nên hiện chưa thể tạo đơn VNPAY.";
                }

                return new VnPayOrderResult(false, false, apiErrorMessage, null, null);
            }

            var apiResponse = JsonSerializer.Deserialize<ApiEnvelope<VnPayOrderApiItem>>(responseBody, JsonOptions());
            var data = apiResponse?.Data;
            if (data == null || string.IsNullOrWhiteSpace(data.OrderUrl))
            {
                return new VnPayOrderResult(false, false, "API không trả về liên kết thanh toán hợp lệ.", null, null);
            }

            return new VnPayOrderResult(true, false, apiResponse?.Message ?? "Tạo đơn thanh toán thành công.", data.OrderUrl, data.ReceiptId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating VnPay order. RegistrationId={RegistrationId}", registrationId);
            return new VnPayOrderResult(false, false, "Đã xảy ra lỗi khi tạo đơn thanh toán VNPAY.", null, null);
        }
    }

    public async Task<VnPayReturnResult> ConfirmVnPayReturnAsync(IQueryCollection query, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var queryString = QueryString.Create(query.SelectMany(item =>
                item.Value.Select(value => new KeyValuePair<string, string?>(item.Key, value))));
            var response = await client.GetAsync($"/api/v1/payments/vnpay/return{queryString}", cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Confirm VnPay return failed. StatusCode={StatusCode}, Response={Response}", response.StatusCode, responseBody);
                return new VnPayReturnResult(false, "Không xác nhận được trạng thái thanh toán VNPAY.", null, string.Empty);
            }

            var apiResponse = JsonSerializer.Deserialize<ApiEnvelope<VnPayReturnApiItem>>(responseBody, JsonOptions());
            var data = apiResponse?.Data;
            if (data is null)
            {
                return new VnPayReturnResult(false, "API không trả về kết quả xác nhận thanh toán hợp lệ.", null, string.Empty);
            }

            var success = IsPaymentSuccessStatus(data.PaymentStatus ?? string.Empty);
            return new VnPayReturnResult(
                success,
                success ? "Thanh toán VNPAY thành công." : "Thanh toán VNPAY chưa hoàn tất hoặc đã bị hủy.",
                data.ReceiptId,
                data.PaymentStatus ?? string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while confirming VnPay return.");
            return new VnPayReturnResult(false, "Không xác nhận được trạng thái thanh toán VNPAY.", null, string.Empty);
        }
    }

    private async Task<List<KhoaHocClassItem>> GetCourseClassesAsync(int courseId, CancellationToken cancellationToken)
    {
        var classes = new List<KhoaHocClassItem>();

        try
        {
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.GetAsync($"/api/v1/courses/{courseId}/classes", cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Get course classes failed. CourseId={CourseId}, StatusCode={StatusCode}, Response={Response}", courseId, response.StatusCode, responseBody);
                return classes;
            }

            var apiResponse = JsonSerializer.Deserialize<ApiEnvelope<List<CourseClassApiItem>>>(responseBody, JsonOptions());
            classes = apiResponse?.Data?.Select(MapCourseClass).ToList() ?? new List<KhoaHocClassItem>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while getting course classes. CourseId={CourseId}", courseId);
        }

        return classes;
    }

    private async Task<(string? Message, string? State)> GetPaymentReceiptStatusAsync(HttpClient client, string receiptId, CancellationToken cancellationToken)
    {
        try
        {
            var response = await client.GetAsync($"/api/v1/payments/vnpay/receipts/{Uri.EscapeDataString(receiptId)}/status", cancellationToken);
            var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Get payment receipt status failed. ReceiptId={ReceiptId}, StatusCode={StatusCode}, Response={Response}", receiptId, response.StatusCode, responseBody);
                return (null, null);
            }

            var apiResponse = JsonSerializer.Deserialize<ApiEnvelope<PaymentReceiptStatusApiItem>>(responseBody, JsonOptions());
            var paymentStatus = apiResponse?.Data?.PaymentStatus ?? string.Empty;
            if (IsPaymentSuccessStatus(paymentStatus))
            {
                return ("Thanh toán thành công.", "success");
            }

            if (IsPaymentPendingStatus(paymentStatus))
            {
                return ("Đang chờ xác nhận thanh toán.", "warning");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while checking payment receipt status. ReceiptId={ReceiptId}", receiptId);
        }

        return (null, null);
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

    private static KhoaHocClassItem MapCourseClass(CourseClassApiItem item)
    {
        return new KhoaHocClassItem
        {
            ClassId = item.ClassId,
            TenLop = item.TenLop ?? $"Lớp {item.ClassId}",
            SiSoHienTai = item.SiSoHienTai,
            SiSoToiDa = item.SiSoToiDa,
            NgayBatDau = item.NgayBatDau,
            NgayKetThuc = item.NgayKetThuc,
            GiaoVien = item.GiaoVien?.HoTen,
            TrangThai = item.TrangThai ?? string.Empty,
            IsOpenForRegistration = item.IsOpenForRegistration,
            LichHoc = item.LichHoc.Select(schedule => new KhoaHocScheduleItem
            {
                ThuTrongTuan = schedule.ThuTrongTuan,
                GioBatDau = schedule.GioBatDau ?? string.Empty,
                GioKetThuc = schedule.GioKetThuc ?? string.Empty,
                DiaDiem = schedule.DiaDiem ?? string.Empty
            }).ToList()
        };
    }

    private static MyCourseRegistrationItem MapMyCourseRegistration(MyCourseRegistrationApiItem item)
    {
        var paymentStatus = item.PaymentStatus ?? string.Empty;
        var registrationStatus = item.TrangThai ?? string.Empty;

        var disabledReason = string.Empty;
        if (IsApprovedRegistrationStatus(registrationStatus) && !IsPaymentSuccessStatus(paymentStatus))
        {
            disabledReason = "Có thể thanh toán qua VNPAY khi backend đã cấu hình khoản thu học phí đầy đủ.";
        }

        return new MyCourseRegistrationItem
        {
            RegistrationId = item.RegistrationId,
            CourseId = item.CourseId,
            ClassId = item.ClassId,
            MaKhoaHoc = item.MaKhoaHoc ?? string.Empty,
            TenKhoaHoc = item.TenKhoaHoc ?? string.Empty,
            LoaiBangLai = item.LoaiBangLai ?? string.Empty,
            TenLop = item.TenLop ?? "Chưa phân lớp",
            NgayHocText = BuildStudyDateText(item.NgayBatDau, item.NgayKetThuc),
            NgayBatDau = item.NgayBatDau,
            NgayKetThuc = item.NgayKetThuc,
            HocPhi = item.HocPhi,
            SoBuoiHoc = item.SoBuoiHoc,
            SiSoHienTai = item.SiSoHienTai,
            SiSoToiDa = item.SiSoToiDa,
            GiaoVien = item.GiaoVien ?? string.Empty,
            SoDienThoaiGiaoVien = item.SoDienThoaiGiaoVien ?? string.Empty,
            MoTa = item.MoTa ?? string.Empty,
            LichHoc = item.LichHoc.Select(schedule => new KhoaHocScheduleItem
            {
                ThuTrongTuan = schedule.ThuTrongTuan,
                GioBatDau = schedule.GioBatDau ?? string.Empty,
                GioKetThuc = schedule.GioKetThuc ?? string.Empty,
                DiaDiem = schedule.DiaDiem ?? string.Empty
            }).ToList(),
            TrangThai = registrationStatus,
            PaymentStatus = paymentStatus,
            ReceiptId = item.ReceiptId?.ToString(),
            CanPayWithZaloPay = IsApprovedRegistrationStatus(registrationStatus) && !IsPaymentSuccessStatus(paymentStatus),
            PaymentDisabledReason = disabledReason
        };
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

    private static bool IsApprovedRegistrationStatus(string? status)
    {
        return string.Equals(status, "da_duyet", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "DaDuyet", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsPaymentSuccessStatus(string? status)
    {
        return string.Equals(status, "da_xac_nhan", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "DaXacNhan", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "da_thanh_toan", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "DaThanhToan", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsPaymentPendingStatus(string? status)
    {
        return string.Equals(status, "cho_xac_nhan", StringComparison.OrdinalIgnoreCase)
            || string.Equals(status, "ChoXacNhan", StringComparison.OrdinalIgnoreCase);
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
        public int ClassId { get; set; }
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

    private sealed class MyCourseRegistrationApiItem
    {
        public int RegistrationId { get; set; }
        public int CourseId { get; set; }
        public int? ClassId { get; set; }
        public string? MaKhoaHoc { get; set; }
        public string? TenKhoaHoc { get; set; }
        public string? LoaiBangLai { get; set; }
        public string? TenLop { get; set; }
        public DateOnly? NgayBatDau { get; set; }
        public DateOnly? NgayKetThuc { get; set; }
        public long HocPhi { get; set; }
        public int SoBuoiHoc { get; set; }
        public int SiSoHienTai { get; set; }
        public int SiSoToiDa { get; set; }
        public string? GiaoVien { get; set; }
        public string? SoDienThoaiGiaoVien { get; set; }
        public string? MoTa { get; set; }
        public List<CourseScheduleApiItem> LichHoc { get; set; } = new();
        public string? TrangThai { get; set; }
        public string? PaymentStatus { get; set; }
        public long? ReceiptId { get; set; }
    }

    private sealed class VnPayOrderApiItem
    {
        public string? OrderUrl { get; set; }
        public long? ReceiptId { get; set; }
        public string? TransactionRef { get; set; }
        public long Amount { get; set; }
        public string? PaymentStatus { get; set; }
    }

    private sealed class VnPayReturnApiItem
    {
        public long ReceiptId { get; set; }
        public string? TransactionRef { get; set; }
        public long RegistrationId { get; set; }
        public string? PaymentStatus { get; set; }
    }

    private sealed class PaymentReceiptStatusApiItem
    {
        public string? PaymentStatus { get; set; }
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

    private sealed class CourseClassApiItem
    {
        public int ClassId { get; set; }
        public string? TenLop { get; set; }
        public int SiSoHienTai { get; set; }
        public int SiSoToiDa { get; set; }
        public DateOnly? NgayBatDau { get; set; }
        public DateOnly? NgayKetThuc { get; set; }
        public string? TrangThai { get; set; }
        public bool IsOpenForRegistration { get; set; }
        public CourseTeacherApiItem? GiaoVien { get; set; }
        public List<CourseScheduleApiItem> LichHoc { get; set; } = new();
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
