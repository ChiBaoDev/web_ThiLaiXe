using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using webthibanglai.Models;

namespace webthibanglai.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<LoginController> _logger;
        private const string AccessTokenSessionKey = "AccessToken";

        public LoginController(IHttpClientFactory httpClientFactory, ILogger<LoginController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.LoginRequest.TenDangNhapHoacEmail) || string.IsNullOrWhiteSpace(model.LoginRequest.MatKhau))
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập tên đăng nhập/email và mật khẩu.");
                return View(model);
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            var payload = new
            {
                ten_dang_nhap_hoac_email = model.LoginRequest.TenDangNhapHoacEmail,
                mat_khau = model.LoginRequest.MatKhau
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/v1/auth/login", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Login API raw response: {ResponseBody}", responseBody);

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError(string.Empty, ExtractErrorMessage(responseBody) ?? "Đăng nhập thất bại.");
                return View(model);
            }

            var apiResponse = JsonSerializer.Deserialize<ApiEnvelope<AuthTokenResponse>>(responseBody, JsonOptions());
            if (apiResponse?.Data == null)
            {
                _logger.LogWarning("Login API response has no data. Body: {ResponseBody}", responseBody);
                ModelState.AddModelError(string.Empty, "Không đọc được dữ liệu đăng nhập từ API.");
                return View(model);
            }

            var auth = NormalizeAuthToken(apiResponse.Data, responseBody);
            _logger.LogInformation("Normalized login auth data: User={User}, Email={Email}, HasToken={HasToken}, Roles={Roles}", auth.TenDangNhap, auth.Email, !string.IsNullOrWhiteSpace(auth.AccessToken), string.Join(",", auth.Roles));

            if (string.IsNullOrWhiteSpace(auth.AccessToken))
            {
                _logger.LogWarning("Login API response missing access token after normalization. Body: {ResponseBody}", responseBody);
                ModelState.AddModelError(string.Empty, "API đăng nhập không trả về access token hợp lệ.");
                return View(model);
            }

            HttpContext.Session.SetString(AccessTokenSessionKey, auth.AccessToken);
            _logger.LogInformation("Saved access token to session for user {Username}. ExpiresAtUtc={ExpiresAtUtc}", auth.TenDangNhap, auth.ExpiresAtUtc);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
            var meResponse = await client.GetAsync("/api/v1/auth/me");
            var meResponseBody = await meResponse.Content.ReadAsStringAsync();
            _logger.LogInformation("Auth/me raw response: {ResponseBody}", meResponseBody);

            if (meResponse.IsSuccessStatusCode)
            {
                var meApiResponse = JsonSerializer.Deserialize<ApiEnvelope<CurrentUserInfo>>(meResponseBody, JsonOptions());
                var currentUser = meApiResponse?.Data;

                TempData["AuthUsername"] = !string.IsNullOrWhiteSpace(currentUser?.HoTen)
                    ? currentUser.HoTen
                    : auth.TenDangNhap;
                TempData["AuthEmail"] = currentUser?.Email ?? auth.Email;
                TempData["AuthRoles"] = string.Join(",", currentUser?.Roles ?? auth.Roles);
                TempData["ProfileUserId"] = currentUser?.UserId.ToString();
                TempData["ProfileHocVienId"] = currentUser?.HocVienId.ToString();
                TempData["ProfileHoTen"] = currentUser?.HoTen;
                TempData["ProfileTenDangNhap"] = currentUser?.TenDangNhap;
                TempData["ProfileEmail"] = currentUser?.Email;
                TempData["ProfileSoDienThoai"] = currentUser?.SoDienThoai;
                TempData["ProfileTrangThai"] = currentUser?.TrangThai;
                TempData["ProfileNgaySinh"] = currentUser?.NgaySinh?.ToString("dd/MM/yyyy");
                TempData["ProfileGioiTinh"] = currentUser?.GioiTinh;
                TempData["ProfileCccd"] = currentUser?.Cccd;
                TempData["ProfileDiaChi"] = currentUser?.DiaChi;
                TempData["ProfileAnhChanDung"] = currentUser?.AnhChanDung;
            }
            else
            {
                TempData["AuthUsername"] = auth.TenDangNhap;
                TempData["AuthEmail"] = auth.Email;
                TempData["AuthRoles"] = string.Join(",", auth.Roles);
            }

            TempData["LoginSuccess"] = $"Đăng nhập thành công: {auth.TenDangNhap}";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Profile(bool debug = false)
        {
            if (debug)
            {
                return View(new LoginViewModel());
            }

            return View(new LoginViewModel());
        }

        [HttpGet]
        public async Task<IActionResult> CurrentUserSummary()
        {
            var token = HttpContext.Session.GetString(AccessTokenSessionKey);
            if (string.IsNullOrWhiteSpace(token))
            {
                return Json(new { isAuthenticated = false });
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("/api/v1/auth/me");
            if (!response.IsSuccessStatusCode)
            {
                HttpContext.Session.Remove(AccessTokenSessionKey);
                return Json(new { isAuthenticated = false });
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonSerializer.Deserialize<ApiEnvelope<CurrentUserInfo>>(responseBody, JsonOptions());
            var user = apiResponse?.Data;

            if (user == null)
            {
                return Json(new { isAuthenticated = false });
            }

            var displayName = !string.IsNullOrWhiteSpace(user.HoTen) ? user.HoTen : user.TenDangNhap;
            var initials = BuildInitials(displayName);
            var isAdmin = user.Roles?.Any(x => string.Equals(x, "ADMIN", StringComparison.OrdinalIgnoreCase)) ?? false;

            return Json(new
            {
                isAuthenticated = true,
                fullName = displayName,
                email = user.Email,
                initials,
                profileUrl = Url.Action(nameof(Profile), "Login"),
                logoutUrl = Url.Action(nameof(Logout), "Login"),
                roleLabel = isAdmin ? "Quản trị viên" : "Học viên"
            });
        }

        [HttpGet]
        public async Task<IActionResult> DebugAuthState()
        {
            var cookieToken = HttpContext.Session.GetString(AccessTokenSessionKey);
            var result = new Dictionary<string, object?>
            {
                ["hasSessionToken"] = !string.IsNullOrWhiteSpace(cookieToken),
                ["sessionTokenPreview"] = string.IsNullOrWhiteSpace(cookieToken)
                    ? null
                    : $"{cookieToken[..Math.Min(24, cookieToken.Length)]}...",
                ["requestIsHttps"] = Request.IsHttps
            };

            if (string.IsNullOrWhiteSpace(cookieToken))
            {
                result["apiMeStatus"] = "SKIPPED_NO_SESSION_TOKEN";
                return Json(result);
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", cookieToken);

            var response = await client.GetAsync("/api/v1/auth/me");
            var responseBody = await response.Content.ReadAsStringAsync();

            result["apiMeStatusCode"] = (int)response.StatusCode;
            result["apiMeReasonPhrase"] = response.ReasonPhrase;
            result["apiMeBody"] = responseBody;

            return Json(result);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove(AccessTokenSessionKey);
            return RedirectToAction(nameof(Index));
        }

        private static string? ExtractErrorMessage(string responseBody)
        {
            var apiResponse = JsonSerializer.Deserialize<ApiEnvelope<object>>(responseBody, JsonOptions());
            return apiResponse?.Message;
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

        private static AuthTokenResponse NormalizeAuthToken(AuthTokenResponse auth, string responseBody)
        {
            if (!string.IsNullOrWhiteSpace(auth.AccessToken))
            {
                return auth;
            }

            using var document = JsonDocument.Parse(responseBody);
            if (!document.RootElement.TryGetProperty("data", out var dataElement) || dataElement.ValueKind != JsonValueKind.Object)
            {
                return auth;
            }

            if (dataElement.TryGetProperty("access_token", out var accessTokenElement))
            {
                auth.AccessToken = accessTokenElement.GetString() ?? string.Empty;
            }

            if (dataElement.TryGetProperty("user_id", out var userIdElement) && userIdElement.TryGetInt64(out var userId))
            {
                auth.UserId = userId;
            }

            if (dataElement.TryGetProperty("ten_dang_nhap", out var usernameElement))
            {
                auth.TenDangNhap = usernameElement.GetString() ?? string.Empty;
            }

            if (dataElement.TryGetProperty("email", out var emailElement))
            {
                auth.Email = emailElement.GetString() ?? string.Empty;
            }

            if (dataElement.TryGetProperty("roles", out var rolesElement) && rolesElement.ValueKind == JsonValueKind.Array)
            {
                auth.Roles = rolesElement.EnumerateArray()
                    .Select(x => x.GetString())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Cast<string>()
                    .ToList();
            }

            if (dataElement.TryGetProperty("expires_at_utc", out var expiresElement))
            {
                var expiresRaw = expiresElement.GetString();
                if (!string.IsNullOrWhiteSpace(expiresRaw)
                    && DateTime.TryParse(expiresRaw, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var parsedExpires))
                {
                    auth.ExpiresAtUtc = parsedExpires;
                }
            }

            return auth;
        }

        private static string BuildInitials(string? fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return "U";
            }

            var parts = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Take(2)
                .Select(x => char.ToUpperInvariant(x[0]));

            var initials = string.Concat(parts);
            return string.IsNullOrWhiteSpace(initials) ? "U" : initials;
        }
    }

    public class ApiEnvelope<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
    }
}
