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

        public IActionResult Index(string? returnUrl = null)
        {
            var model = BuildLoginViewModel();
            ViewBag.ReturnUrl = GetSafeReturnUrl(returnUrl);

            if (TempData.TryGetValue("RegisterSuccess", out var registerSuccessMessage))
            {
                ViewBag.RegisterSuccess = registerSuccessMessage?.ToString();
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = GetSafeReturnUrl(returnUrl);

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
            HttpResponseMessage response;
            string responseBody;
            try
            {
                response = await client.PostAsync("/api/v1/auth/login", content);
                responseBody = await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Cannot connect to API while logging in. BaseAddress={BaseAddress}", client.BaseAddress);
                ModelState.AddModelError(string.Empty, "Không thể kết nối tới API đăng nhập. Vui lòng kiểm tra backend API đang chạy tại cấu hình ApiSettings:BaseUrl.");
                return View(model);
            }
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
            return RedirectAfterAuth(returnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> Register(LoginViewModel model, string? returnUrl = null)
        {
            ViewBag.ReturnUrl = GetSafeReturnUrl(returnUrl);
            var request = model.RegisterRequest;

            // Validate các trường bắt buộc
            if (string.IsNullOrWhiteSpace(request.TenDangNhap))
                ModelState.AddModelError("RegisterRequest.TenDangNhap", "Tên đăng nhập không được để trống.");
            if (string.IsNullOrWhiteSpace(request.MatKhau))
                ModelState.AddModelError("RegisterRequest.MatKhau", "Mật khẩu không được để trống.");
            if (request.MatKhau.Length < 8)
                ModelState.AddModelError("RegisterRequest.MatKhau", "Mật khẩu phải có ít nhất 8 ký tự.");
            if (string.IsNullOrWhiteSpace(request.Email))
                ModelState.AddModelError("RegisterRequest.Email", "Email không được để trống.");
            if (!IsValidEmail(request.Email))
                ModelState.AddModelError("RegisterRequest.Email", "Email không hợp lệ.");

            if (!ModelState.IsValid)
            {
                TempData["ActiveLoginTab"] = "register";
                return View("Index", model);
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            var payload = new
            {
                ten_dang_nhap = request.TenDangNhap.Trim(),
                mat_khau = request.MatKhau,
                email = request.Email.Trim(),
                so_dien_thoai = string.IsNullOrWhiteSpace(request.SoDienThoai) ? null : request.SoDienThoai.Trim()
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/v1/auth/register", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Register API raw response: {ResponseBody}", responseBody);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Register API failed. Status: {StatusCode}, Body: {ResponseBody}", response.StatusCode, responseBody);
                var errorMessage = ExtractDetailedErrorMessage(responseBody) ?? ExtractErrorMessage(responseBody);
                
                // Nếu là lỗi server 500 và message chung chung, đưa ra thông báo thân thiện hơn
                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError &&
                    (string.IsNullOrEmpty(errorMessage) || errorMessage.Contains("unexpected error", StringComparison.OrdinalIgnoreCase)))
                {
                    errorMessage = "Đăng ký thất bại. Có thể email hoặc tên đăng nhập đã tồn tại. Vui lòng thử lại với thông tin khác.";
                }
                else if (string.IsNullOrEmpty(errorMessage))
                {
                    errorMessage = "Đăng ký thất bại.";
                }
                
                _logger.LogInformation("Extracted error message: {ErrorMessage}", errorMessage);
                ModelState.AddModelError(string.Empty, errorMessage);
                TempData["ActiveLoginTab"] = "register";
                return View("Index", model);
            }

            var apiResponse = JsonSerializer.Deserialize<ApiEnvelope<RegisterResponseData>>(responseBody, JsonOptions());
            var registeredUser = apiResponse?.Data;

            if (registeredUser == null)
            {
                TempData["RegisterSuccess"] = "Đăng ký thành công. Vui lòng đăng nhập bằng tài khoản vừa tạo.";
                return RedirectToAction(nameof(Index));
            }

            // Tự động đăng nhập sau khi đăng ký thành công
            _logger.LogInformation("Auto-login after registration for user: {Username}", registeredUser.TenDangNhap);
            
            var loginPayload = new
            {
                ten_dang_nhap_hoac_email = request.TenDangNhap,
                mat_khau = request.MatKhau
            };

            var loginContent = new StringContent(JsonSerializer.Serialize(loginPayload), Encoding.UTF8, "application/json");
            var loginResponse = await client.PostAsync("/api/v1/auth/login", loginContent);
            var loginResponseBody = await loginResponse.Content.ReadAsStringAsync();

            if (loginResponse.IsSuccessStatusCode)
            {
                var loginApiResponse = JsonSerializer.Deserialize<ApiEnvelope<AuthTokenResponse>>(loginResponseBody, JsonOptions());
                if (loginApiResponse?.Data != null)
                {
                    var auth = NormalizeAuthToken(loginApiResponse.Data, loginResponseBody);
                    if (!string.IsNullOrWhiteSpace(auth.AccessToken))
                    {
                        HttpContext.Session.SetString(AccessTokenSessionKey, auth.AccessToken);
                        _logger.LogInformation("Saved access token to session for user {Username}. ExpiresAtUtc={ExpiresAtUtc}", auth.TenDangNhap, auth.ExpiresAtUtc);

                        // Gọi API /auth/me để lấy thông tin user đầy đủ
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
                        var meResponse = await client.GetAsync("/api/v1/auth/me");
                        var meResponseBody = await meResponse.Content.ReadAsStringAsync();
                        _logger.LogInformation("Auth/me raw response after registration: {ResponseBody}", meResponseBody);

                        if (meResponse.IsSuccessStatusCode)
                        {
                            var meApiResponse = JsonSerializer.Deserialize<ApiEnvelope<CurrentUserInfo>>(meResponseBody, JsonOptions());
                            var currentUser = meApiResponse?.Data;

                            if (currentUser != null)
                            {
                                TempData["AuthUsername"] = !string.IsNullOrWhiteSpace(currentUser.HoTen) ? currentUser.HoTen : auth.TenDangNhap;
                                TempData["AuthEmail"] = currentUser.Email ?? auth.Email;
                                TempData["AuthRoles"] = string.Join(",", currentUser.Roles ?? auth.Roles);
                                TempData["ProfileUserId"] = currentUser.UserId.ToString();
                                TempData["ProfileHocVienId"] = currentUser.HocVienId.ToString();
                                TempData["ProfileHoTen"] = currentUser.HoTen;
                                TempData["ProfileTenDangNhap"] = currentUser.TenDangNhap;
                                TempData["ProfileEmail"] = currentUser.Email;
                                TempData["ProfileSoDienThoai"] = currentUser.SoDienThoai;
                                TempData["ProfileTrangThai"] = currentUser.TrangThai;
                                TempData["ProfileNgaySinh"] = currentUser.NgaySinh?.ToString("dd/MM/yyyy");
                                TempData["ProfileGioiTinh"] = currentUser.GioiTinh;
                                TempData["ProfileCccd"] = currentUser.Cccd;
                                TempData["ProfileDiaChi"] = currentUser.DiaChi;
                                TempData["ProfileAnhChanDung"] = currentUser.AnhChanDung;
                            }
                        }
                        else
                        {
                            TempData["AuthUsername"] = auth.TenDangNhap;
                            TempData["AuthEmail"] = auth.Email;
                            TempData["AuthRoles"] = string.Join(",", auth.Roles);
                        }

                        TempData["LoginSuccess"] = $"Đăng ký và đăng nhập thành công! Chào mừng {registeredUser.TenDangNhap}";
                        return RedirectAfterAuth(returnUrl, defaultAction: "Index", defaultController: "Onboarding");
                    }
                }
            }

            // Nếu auto-login thất bại, vẫn thông báo đăng ký thành công
            TempData["RegisterSuccess"] = $"Đăng ký thành công cho tài khoản {registeredUser.TenDangNhap}. Vui lòng đăng nhập.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Profile(bool debug = false)
        {
            var token = HttpContext.Session.GetString(AccessTokenSessionKey);
            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["LoginError"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var client = _httpClientFactory.CreateClient("ApiClient");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync("/api/v1/auth/me");
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to get user profile. Status: {StatusCode}, Body: {Body}", response.StatusCode, responseBody);
                    TempData["ProfileError"] = "Không thể tải thông tin hồ sơ. Vui lòng thử lại.";
                    return RedirectToAction("Index", "Home");
                }

                var apiResponse = JsonSerializer.Deserialize<ApiEnvelope<CurrentUserInfo>>(responseBody, JsonOptions());
                if (apiResponse?.Data == null)
                {
                    TempData["ProfileError"] = "Không đọc được dữ liệu hồ sơ.";
                    return RedirectToAction("Index", "Home");
                }

                await PopulateStudentProfileForCurrentUserAsync(client, apiResponse.Data);

                var model = new LoginViewModel
                {
                    CurrentUser = apiResponse.Data,
                    UpdateProfileRequest = new UpdateProfileRequestModel
                    {
                        HoTen = apiResponse.Data.HoTen,
                        Email = apiResponse.Data.Email,
                        SoDienThoai = apiResponse.Data.SoDienThoai
                    },
                    ChangePasswordRequest = new ChangePasswordRequestModel()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading profile");
                TempData["ProfileError"] = "Đã xảy ra lỗi khi tải hồ sơ.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(LoginViewModel model)
        {
            var token = HttpContext.Session.GetString(AccessTokenSessionKey);
            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["ProfileUpdateError"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToAction(nameof(Index));
            }

            var request = model.UpdateProfileRequest;
            
            // Validate thông tin cơ bản (chỉ email vì bảng nguoi_dung không có ho_ten)
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                TempData["ProfileUpdateError"] = "Email không được để trống.";
                TempData["ActiveProfileTab"] = "update";
                return RedirectToAction(nameof(Profile));
            }

            if (!IsValidEmail(request.Email))
            {
                TempData["ProfileUpdateError"] = "Email không hợp lệ.";
                TempData["ActiveProfileTab"] = "update";
                return RedirectToAction(nameof(Profile));
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Chỉ cập nhật email và số điện thoại vì bảng nguoi_dung không có trường ho_ten
            var payload = new
            {
                email = request.Email.Trim(),
                so_dien_thoai = string.IsNullOrWhiteSpace(request.SoDienThoai) ? null : request.SoDienThoai.Trim()
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            _logger.LogInformation("Update profile API request payload: {Payload}", JsonSerializer.Serialize(payload));
            
            var response = await client.PutAsync("/api/v1/auth/me", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Update profile API response - Status: {StatusCode}, Body: {ResponseBody}", response.StatusCode, responseBody);

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = ExtractDetailedErrorMessage(responseBody) ?? ExtractErrorMessage(responseBody);
                
                // Nếu là lỗi 500 và message chung chung, cung cấp thông tin chi tiết hơn
                if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    _logger.LogError("Update profile failed with 500 error. Response: {ResponseBody}", responseBody);
                    if (string.IsNullOrEmpty(errorMessage) || errorMessage.Contains("unexpected error", StringComparison.OrdinalIgnoreCase))
                    {
                        errorMessage = $"Lỗi server khi cập nhật hồ sơ. Vui lòng kiểm tra logs hoặc thử lại sau. (Status: {response.StatusCode})";
                    }
                }
                
                TempData["ProfileUpdateError"] = errorMessage ?? "Cập nhật hồ sơ thất bại.";
                TempData["ActiveProfileTab"] = "update";
                PreserveProfileTempDataFromRequest(request, model);
                return RedirectToAction(nameof(Profile));
            }

            var apiResponse = JsonSerializer.Deserialize<ApiEnvelope<CurrentUserInfo>>(responseBody, JsonOptions());
            if (apiResponse?.Data == null)
            {
                TempData["ProfileUpdateError"] = "Không đọc được dữ liệu hồ sơ sau khi cập nhật.";
                TempData["ActiveProfileTab"] = "update";
                PreserveProfileTempDataFromRequest(request, model);
                return RedirectToAction(nameof(Profile));
            }

            PopulateProfileTempData(apiResponse.Data);
            TempData["ProfileUpdateSuccess"] = "Cập nhật hồ sơ thành công.";
            TempData["ActiveProfileTab"] = "update";
            return RedirectToAction(nameof(Profile));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(LoginViewModel model)
        {
            var token = HttpContext.Session.GetString(AccessTokenSessionKey);
            if (string.IsNullOrWhiteSpace(token))
            {
                TempData["ChangePasswordError"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
                return RedirectToAction(nameof(Index));
            }

            var request = model.ChangePasswordRequest;
            PreserveChangePasswordTempData(request);

            if (string.IsNullOrWhiteSpace(request.OldPassword)
                || string.IsNullOrWhiteSpace(request.NewPassword)
                || string.IsNullOrWhiteSpace(request.ConfirmPassword))
            {
                TempData["ChangePasswordError"] = "Vui lòng nhập đầy đủ thông tin đổi mật khẩu.";
                TempData["ActiveProfileTab"] = "password";
                return RedirectToAction(nameof(Profile));
            }

            if (request.NewPassword.Length < 8)
            {
                TempData["ChangePasswordError"] = "Mật khẩu mới phải có ít nhất 8 ký tự.";
                TempData["ActiveProfileTab"] = "password";
                return RedirectToAction(nameof(Profile));
            }

            if (!string.Equals(request.NewPassword, request.ConfirmPassword, StringComparison.Ordinal))
            {
                TempData["ChangePasswordError"] = "Xác nhận mật khẩu không khớp.";
                TempData["ActiveProfileTab"] = "password";
                return RedirectToAction(nameof(Profile));
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var payload = new
            {
                mat_khau_cu = request.OldPassword,
                mat_khau_moi = request.NewPassword
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/v1/auth/change-password", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Change password API raw response: {ResponseBody}", responseBody);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ChangePasswordError"] = ExtractDetailedErrorMessage(responseBody) ?? ExtractErrorMessage(responseBody) ?? "Đổi mật khẩu thất bại.";
                TempData["ActiveProfileTab"] = "password";
                return RedirectToAction(nameof(Profile));
            }

            ClearChangePasswordTempData();
            TempData["ChangePasswordSuccess"] = ExtractErrorMessage(responseBody) ?? "Đổi mật khẩu thành công.";
            TempData["ActiveProfileTab"] = "password";
            return RedirectToAction(nameof(Profile));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(LoginViewModel model)
        {
            var request = model.ForgotPasswordRequest;
            TempData["ForgotPasswordEmail"] = request.Email;

            if (string.IsNullOrWhiteSpace(request.Email) || !IsValidEmail(request.Email))
            {
                TempData["ForgotPasswordError"] = "Vui lòng nhập email hợp lệ.";
                TempData["ActiveLoginTab"] = "forgot";
                return RedirectToAction(nameof(Index));
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            var payload = new
            {
                email = request.Email.Trim()
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/v1/auth/forgot-password", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Forgot password API raw response: {ResponseBody}", responseBody);

            TempData["ActiveLoginTab"] = "forgot";

            if (!response.IsSuccessStatusCode)
            {
                TempData["ForgotPasswordError"] = ExtractDetailedErrorMessage(responseBody) ?? ExtractErrorMessage(responseBody) ?? "Gửi yêu cầu quên mật khẩu thất bại.";
                return RedirectToAction(nameof(Index));
            }

            TempData["ForgotPasswordSuccess"] = ExtractErrorMessage(responseBody) ?? "Yêu cầu quên mật khẩu đã được gửi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(LoginViewModel model)
        {
            var request = model.ResetPasswordRequest;
            PreserveResetPasswordTempData(request);
            TempData["ActiveLoginTab"] = "reset";

            if (string.IsNullOrWhiteSpace(request.Email) || !IsValidEmail(request.Email))
            {
                TempData["ResetPasswordError"] = "Email không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(request.Token))
            {
                TempData["ResetPasswordError"] = "Token không được để trống.";
                return RedirectToAction(nameof(Index));
            }

            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                TempData["ResetPasswordError"] = "Mật khẩu mới không được để trống.";
                return RedirectToAction(nameof(Index));
            }

            if (request.NewPassword.Length < 8)
            {
                TempData["ResetPasswordError"] = "Mật khẩu mới phải có ít nhất 8 ký tự.";
                return RedirectToAction(nameof(Index));
            }

            if (!string.Equals(request.NewPassword, request.ConfirmPassword, StringComparison.Ordinal))
            {
                TempData["ResetPasswordError"] = "Xác nhận mật khẩu không khớp.";
                return RedirectToAction(nameof(Index));
            }

            var client = _httpClientFactory.CreateClient("ApiClient");
            var payload = new
            {
                email = request.Email.Trim(),
                reset_token = request.Token.Trim(),
                mat_khau_moi = request.NewPassword
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/v1/auth/reset-password", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Reset password API raw response: {ResponseBody}", responseBody);

            if (!response.IsSuccessStatusCode)
            {
                TempData["ResetPasswordError"] = ExtractDetailedErrorMessage(responseBody) ?? ExtractErrorMessage(responseBody) ?? "Đặt lại mật khẩu thất bại.";
                return RedirectToAction(nameof(Index));
            }

            ClearResetPasswordTempData();
            TempData["ResetPasswordSuccess"] = ExtractErrorMessage(responseBody) ?? "Đặt lại mật khẩu thành công.";
            return RedirectToAction(nameof(Index));
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

            TempData.Remove("AuthUsername");
            TempData.Remove("AuthEmail");
            TempData.Remove("AuthRoles");
            TempData.Remove("ProfileUserId");
            TempData.Remove("ProfileHocVienId");
            TempData.Remove("ProfileHoTen");
            TempData.Remove("ProfileTenDangNhap");
            TempData.Remove("ProfileEmail");
            TempData.Remove("ProfileSoDienThoai");
            TempData.Remove("ProfileTrangThai");
            TempData.Remove("ProfileNgaySinh");
            TempData.Remove("ProfileGioiTinh");
            TempData.Remove("ProfileCccd");
            TempData.Remove("ProfileDiaChi");
            TempData.Remove("ProfileAnhChanDung");
            TempData.Remove("LoginSuccess");

            return RedirectToAction(nameof(Index));
        }

        private LoginViewModel BuildProfileViewModel()
        {
            return new LoginViewModel
            {
                UpdateProfileRequest = new UpdateProfileRequestModel
                {
                    HoTen = TempData.Peek("ProfileHoTen")?.ToString() ?? string.Empty,
                    Email = TempData.Peek("ProfileEmail")?.ToString() ?? string.Empty,
                    SoDienThoai = TempData.Peek("ProfileSoDienThoai")?.ToString() ?? string.Empty
                },
                ChangePasswordRequest = new ChangePasswordRequestModel
                {
                    OldPassword = TempData.Peek("ChangePasswordOldPassword")?.ToString() ?? string.Empty,
                    NewPassword = TempData.Peek("ChangePasswordNewPassword")?.ToString() ?? string.Empty,
                    ConfirmPassword = TempData.Peek("ChangePasswordConfirmPassword")?.ToString() ?? string.Empty
                }
            };
        }

        private LoginViewModel BuildLoginViewModel()
        {
            return new LoginViewModel
            {
                ForgotPasswordRequest = new ForgotPasswordRequestModel
                {
                    Email = TempData.Peek("ForgotPasswordEmail")?.ToString() ?? string.Empty
                },
                ResetPasswordRequest = new ResetPasswordRequestModel
                {
                    Email = TempData.Peek("ResetPasswordEmail")?.ToString() ?? string.Empty,
                    Token = TempData.Peek("ResetPasswordToken")?.ToString() ?? string.Empty,
                    NewPassword = TempData.Peek("ResetPasswordNewPassword")?.ToString() ?? string.Empty,
                    ConfirmPassword = TempData.Peek("ResetPasswordConfirmPassword")?.ToString() ?? string.Empty
                }
            };
        }

        private string? GetSafeReturnUrl(string? returnUrl)
        {
            if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return null;
            }

            return returnUrl;
        }

        private IActionResult RedirectAfterAuth(string? returnUrl, string defaultAction = "Index", string defaultController = "Home")
        {
            var safeReturnUrl = GetSafeReturnUrl(returnUrl);
            if (!string.IsNullOrWhiteSpace(safeReturnUrl))
            {
                return LocalRedirect(safeReturnUrl);
            }

            return RedirectToAction(defaultAction, defaultController);
        }

        private void PopulateProfileTempData(CurrentUserInfo currentUser)
        {
            TempData["AuthUsername"] = !string.IsNullOrWhiteSpace(currentUser.HoTen)
                ? currentUser.HoTen
                : currentUser.TenDangNhap;
            TempData["AuthEmail"] = currentUser.Email;
            TempData["AuthRoles"] = string.Join(",", currentUser.Roles ?? new List<string>());
            TempData["ProfileUserId"] = currentUser.UserId.ToString();
            TempData["ProfileHocVienId"] = currentUser.HocVienId.ToString();
            TempData["ProfileHoTen"] = currentUser.HoTen;
            TempData["ProfileTenDangNhap"] = currentUser.TenDangNhap;
            TempData["ProfileEmail"] = currentUser.Email;
            TempData["ProfileSoDienThoai"] = currentUser.SoDienThoai;
            TempData["ProfileTrangThai"] = currentUser.TrangThai;
            TempData["ProfileNgaySinh"] = currentUser.NgaySinh?.ToString("dd/MM/yyyy");
            TempData["ProfileGioiTinh"] = currentUser.GioiTinh;
            TempData["ProfileCccd"] = currentUser.Cccd;
            TempData["ProfileDiaChi"] = currentUser.DiaChi;
            TempData["ProfileAnhChanDung"] = currentUser.AnhChanDung;
        }

        private async Task PopulateStudentProfileForCurrentUserAsync(HttpClient client, CurrentUserInfo currentUser)
        {
            try
            {
                var response = await client.GetAsync("/api/v1/auth/me/student-profile");
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Get student-profile for profile page failed. Status: {StatusCode}, Body: {Body}", response.StatusCode, responseBody);
                    return;
                }

                var apiResponse = JsonSerializer.Deserialize<ApiEnvelope<StudentProfileForProfilePage>>(responseBody, JsonOptions());
                var studentProfile = apiResponse?.Data;
                if (studentProfile is null || studentProfile.HocVienId <= 0)
                {
                    return;
                }

                currentUser.HocVienId = studentProfile.HocVienId;
                if (!string.IsNullOrWhiteSpace(studentProfile.HoTen))
                {
                    currentUser.HoTen = studentProfile.HoTen;
                }

                currentUser.NgaySinh = studentProfile.NgaySinh ?? currentUser.NgaySinh;
                currentUser.GioiTinh = studentProfile.GioiTinh ?? currentUser.GioiTinh;
                currentUser.Cccd = studentProfile.Cccd ?? currentUser.Cccd;
                currentUser.DiaChi = studentProfile.DiaChi ?? currentUser.DiaChi;
                currentUser.AnhChanDung = studentProfile.AnhChanDung ?? currentUser.AnhChanDung;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Không thể tải hồ sơ học viên từ /api/v1/auth/me/student-profile cho trang profile.");
            }
        }

        private void PreserveProfileTempDataFromRequest(UpdateProfileRequestModel request, LoginViewModel model)
        {
            TempData["ProfileHoTen"] = request.HoTen;
            TempData["ProfileEmail"] = request.Email;
            TempData["ProfileSoDienThoai"] = request.SoDienThoai;
            model.UpdateProfileRequest = request;
        }

        private void PreserveChangePasswordTempData(ChangePasswordRequestModel request)
        {
            TempData["ChangePasswordOldPassword"] = request.OldPassword;
            TempData["ChangePasswordNewPassword"] = request.NewPassword;
            TempData["ChangePasswordConfirmPassword"] = request.ConfirmPassword;
        }

        private void ClearChangePasswordTempData()
        {
            TempData.Remove("ChangePasswordOldPassword");
            TempData.Remove("ChangePasswordNewPassword");
            TempData.Remove("ChangePasswordConfirmPassword");
        }

        private void PreserveResetPasswordTempData(ResetPasswordRequestModel request)
        {
            TempData["ResetPasswordEmail"] = request.Email;
            TempData["ResetPasswordToken"] = request.Token;
            TempData["ResetPasswordNewPassword"] = request.NewPassword;
            TempData["ResetPasswordConfirmPassword"] = request.ConfirmPassword;
        }

        private void ClearResetPasswordTempData()
        {
            TempData.Remove("ResetPasswordEmail");
            TempData.Remove("ResetPasswordToken");
            TempData.Remove("ResetPasswordNewPassword");
            TempData.Remove("ResetPasswordConfirmPassword");
        }

        private static DateOnly? ParseDateOnly(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            if (DateOnly.TryParseExact(value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedVietnameseDate))
            {
                return parsedVietnameseDate;
            }

            if (DateOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
            {
                return parsedDate;
            }

            return null;
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                _ = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static string? ExtractErrorMessage(string responseBody)
        {
            try
            {
                var apiResponse = JsonSerializer.Deserialize<ApiEnvelope<object>>(responseBody, JsonOptions());
                return apiResponse?.Message;
            }
            catch
            {
                return null;
            }
        }

        private static string? ExtractDetailedErrorMessage(string responseBody)
        {
            try
            {
                // Thử parse với JsonSerializerOptions có IgnoreNullValues
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };
                
                var apiResponse = JsonSerializer.Deserialize<ApiEnvelope<object>>(responseBody, options);
                
                // Nếu có errors và là array
                if (apiResponse?.Errors != null && apiResponse.Errors.Count > 0)
                {
                    return string.Join(" ", apiResponse.Errors
                        .Select(x => !string.IsNullOrWhiteSpace(x.Detail)
                            ? x.Detail
                            : !string.IsNullOrWhiteSpace(x.Field)
                                ? $"{x.Field}: dữ liệu không hợp lệ."
                                : x.Code)
                        .Where(x => !string.IsNullOrWhiteSpace(x)));
                }
                
                // Nếu không có errors, trả về message
                return apiResponse?.Message;
            }
            catch
            {
                // Nếu không parse được JSON, thử tìm message trong raw response
                if (responseBody.Contains("Email đã tồn tại", StringComparison.OrdinalIgnoreCase))
                    return "Email đã tồn tại.";
                    
                if (responseBody.Contains("Tên đăng nhập đã tồn tại", StringComparison.OrdinalIgnoreCase))
                    return "Tên đăng nhập đã tồn tại.";
                    
                if (responseBody.Contains("uq_hoc_vien_cccd", StringComparison.OrdinalIgnoreCase) ||
                    responseBody.Contains("duplicate key", StringComparison.OrdinalIgnoreCase))
                    return "CCCD đã tồn tại trong hệ thống. Vui lòng kiểm tra lại.";
                    
                if (responseBody.Contains("UNIQUE KEY constraint", StringComparison.OrdinalIgnoreCase))
                    return "Thông tin đã tồn tại trong hệ thống (email, tên đăng nhập hoặc CCCD). Vui lòng kiểm tra lại.";
                
                return null;
            }
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

        private sealed class StudentProfileForProfilePage
        {
            public long HocVienId { get; set; }
            public long UserId { get; set; }
            public string? HoTen { get; set; }
            public DateOnly? NgaySinh { get; set; }
            public string? GioiTinh { get; set; }
            public string? Cccd { get; set; }
            public string? DiaChi { get; set; }
            public string? AnhChanDung { get; set; }
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
        public List<ApiErrorDetail>? Errors { get; set; }
    }

    public class ApiErrorDetail
    {
        public string Code { get; set; } = string.Empty;
        public string Field { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
    }

    public class RegisterResponseData
    {
        public long UserId { get; set; }
        public string TenDangNhap { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RoleMacDinh { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
