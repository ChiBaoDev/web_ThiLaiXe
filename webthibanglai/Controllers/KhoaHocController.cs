using Microsoft.AspNetCore.Mvc;
using webthibanglai.Models;
using webthibanglai.Services;

namespace webthibanglai.Controllers
{
    public class KhoaHocController : Controller
    {
        private const string AccessTokenSessionKey = "AccessToken";
        private readonly ICourseApiService _courseApiService;

        public KhoaHocController(ICourseApiService courseApiService)
        {
            _courseApiService = courseApiService;
        }

        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var model = await _courseApiService.GetCoursesAsync(cancellationToken);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id, CancellationToken cancellationToken)
        {
            var model = await _courseApiService.GetCourseDetailAsync(id, cancellationToken);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int courseId, int classId, string? ghiChu, CancellationToken cancellationToken)
        {
            var accessToken = HttpContext.Session.GetString(AccessTokenSessionKey);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                TempData["LoginSuccess"] = "Bạn cần đăng nhập để đăng ký khóa học.";
                return RedirectToAction("Index", "Login", new { returnUrl = Url.Action(nameof(Detail), "KhoaHoc", new { id = courseId }) });
            }

            var model = await _courseApiService.GetCourseDetailAsync(courseId, cancellationToken);
            model.SelectedClassId = classId;

            if (model.Course is null)
            {
                model.ErrorMessage ??= "Không tìm thấy khóa học để đăng ký.";
                return View("Detail", model);
            }

            var result = await _courseApiService.RegisterCourseAsync(accessToken, courseId, classId, ghiChu, cancellationToken);

            if (result.RequiresLogin)
            {
                TempData["LoginSuccess"] = result.Message;
                return RedirectToAction("Index", "Login", new { returnUrl = Url.Action(nameof(Detail), "KhoaHoc", new { id = courseId }) });
            }

            if (result.RequiresStudentProfile)
            {
                TempData["StudentProfileNotice"] = result.Message;
                return RedirectToAction("Index", "LichHoc");
            }

            if (result.IsSuccess)
            {
                TempData["CourseRegistrationStatusMessage"] = "Đăng ký thành công. Hồ sơ của bạn đang ở trạng thái chờ duyệt.";
                TempData["CourseRegistrationStatusState"] = "success";
                return RedirectToAction(nameof(MyRegistrations));
            }

            model.RegistrationErrorMessage = result.Message;

            return View("Detail", model);
        }

        [HttpGet]
        public async Task<IActionResult> MyRegistrations(string? receiptId, CancellationToken cancellationToken)
        {
            var accessToken = HttpContext.Session.GetString(AccessTokenSessionKey);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                TempData["LoginSuccess"] = "Bạn cần đăng nhập để xem đăng ký khóa học của mình.";
                return RedirectToAction("Index", "Login", new { returnUrl = BuildCurrentReturnUrl() });
            }

            var model = await _courseApiService.GetMyCourseRegistrationsAsync(accessToken, receiptId, cancellationToken);
            model.StatusMessage ??= TempData["CourseRegistrationStatusMessage"]?.ToString();
            model.StatusState ??= TempData["CourseRegistrationStatusState"]?.ToString();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayWithZaloPay(int registrationId, string? paymentMethod, string? returnAction, CancellationToken cancellationToken)
        {
            var accessToken = HttpContext.Session.GetString(AccessTokenSessionKey);
            var result = await _courseApiService.CreateVnPayOrderAsync(accessToken, registrationId, cancellationToken);
            var targetAction = string.Equals(returnAction, "LichHocIndex", StringComparison.OrdinalIgnoreCase)
                ? (action: "Index", controller: "LichHoc")
                : (action: nameof(MyRegistrations), controller: "KhoaHoc");

            if (result.RequiresLogin)
            {
                TempData["LoginSuccess"] = result.Message;
                return RedirectToAction("Index", "Login", new { returnUrl = BuildCurrentReturnUrl() });
            }

            if (!result.IsSuccess || string.IsNullOrWhiteSpace(result.OrderUrl))
            {
                if (HttpContext.Request.Headers.XRequestedWith == "XMLHttpRequest")
                {
                    Response.StatusCode = StatusCodes.Status400BadRequest;
                    return Json(new
                    {
                        success = false,
                        message = "Không thể tạo thanh toán VNPAY, vui lòng thử lại."
                    });
                }

                TempData["CourseRegistrationStatusMessage"] = result.Message;
                TempData["CourseRegistrationStatusState"] = "danger";
                return RedirectToAction(targetAction.action, targetAction.controller);
            }

            if (result.ReceiptId.HasValue)
            {
                TempData["LatestPaymentReceiptId"] = result.ReceiptId.Value.ToString();
            }

            if (HttpContext.Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                return Json(new
                {
                    success = true,
                    orderUrl = result.OrderUrl,
                    receiptId = result.ReceiptId
                });
            }

            return Redirect(result.OrderUrl);
        }

        [HttpGet]
        public async Task<IActionResult> VnPayReturn(string? vnp_ResponseCode, string? vnp_TransactionStatus, string? vnp_TxnRef, long? receiptId, CancellationToken cancellationToken)
        {
            var confirmResult = await _courseApiService.ConfirmVnPayReturnAsync(Request.Query, cancellationToken);
            if (confirmResult.IsSuccess)
            {
                TempData["CourseRegistrationStatusMessage"] = confirmResult.Message;
                TempData["CourseRegistrationStatusState"] = "success";
            }
            else
            {
                TempData["CourseRegistrationStatusMessage"] = string.IsNullOrWhiteSpace(confirmResult.Message)
                    ? string.IsNullOrWhiteSpace(vnp_TxnRef)
                    ? "Thanh toán VNPAY chưa hoàn tất hoặc đã bị hủy."
                    : $"Thanh toán VNPAY chưa hoàn tất hoặc đã bị hủy. Mã giao dịch: {vnp_TxnRef}"
                    : confirmResult.Message;
                TempData["CourseRegistrationStatusState"] = "warning";
            }

            return RedirectToAction(nameof(MyRegistrations), new
            {
                receiptId = confirmResult.ReceiptId?.ToString() ?? receiptId?.ToString() ?? TempData["LatestPaymentReceiptId"]?.ToString()
            });
        }

        private string BuildCurrentReturnUrl()
        {
            return Request.PathBase + Request.Path + Request.QueryString;
        }
    }
}
