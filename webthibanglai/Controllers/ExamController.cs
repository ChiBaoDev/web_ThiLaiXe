using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using webthibanglai.Models;
using webthibanglai.Services;

namespace webthibanglai.Controllers
{
    public class ExamController : Controller
    {
        private const string AccessTokenSessionKey = "AccessToken";
        private readonly IExamApiService _examApiService;

        public ExamController(IExamApiService examApiService)
        {
            _examApiService = examApiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var accessToken = HttpContext.Session.GetString(AccessTokenSessionKey);
            var model = await _examApiService.GetSampleExamsAsync(accessToken, cancellationToken);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Start(long sampleExamId, CancellationToken cancellationToken)
        {
            var sampleExam = await _examApiService.GetSampleExamAsync(sampleExamId, cancellationToken);
            if (sampleExam == null)
            {
                TempData["ExamError"] = "Không tìm thấy đề thi thử đã chọn.";
                return RedirectToAction(nameof(Index));
            }

            var accessToken = HttpContext.Session.GetString(AccessTokenSessionKey);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                TempData["ExamError"] = "Vui lòng đăng nhập để bắt đầu phiên thi thử thật.";
                return RedirectToAction("Index", "Login", new { returnUrl = Url.Action(nameof(Index), "Exam") });
            }

            var startedSession = await _examApiService.StartSampleExamAsync(sampleExamId, accessToken, cancellationToken);
            if (startedSession == null)
            {
                TempData["ExamError"] = "Không thể khởi tạo phiên thi cho đề mẫu đã chọn.";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Launch), new { sessionId = startedSession.SessionId, number = 1 });
        }

        [HttpGet]
        [ActionName("Start")]
        public async Task<IActionResult> StartPreview(long id, CancellationToken cancellationToken)
        {
            var sampleExam = await _examApiService.GetSampleExamAsync(id, cancellationToken);
            if (sampleExam == null)
            {
                TempData["ExamError"] = "Không tìm thấy đề thi thử đã chọn.";
                return RedirectToAction(nameof(Index));
            }

            var model = new ExamViewModel
            {
                IsAuthenticated = !string.IsNullOrWhiteSpace(HttpContext.Session.GetString(AccessTokenSessionKey)),
                SelectedSampleExam = sampleExam,
                ErrorMessage = TempData["ExamError"]?.ToString()
            };

            return View("Start", model);
        }

        [HttpGet]
        public async Task<IActionResult> Launch(long sessionId, int number = 1, CancellationToken cancellationToken = default)
        {
            var accessToken = HttpContext.Session.GetString(AccessTokenSessionKey);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                TempData["ExamError"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại để bắt đầu thi.";
                return RedirectToAction("Index", "Login", new { returnUrl = Url.Action(nameof(Index), "Exam") });
            }

            var session = await _examApiService.GetSessionAsync(sessionId, accessToken, cancellationToken);
            if (session == null)
            {
                TempData["ExamError"] = "Không tìm thấy phiên thi vừa tạo.";
                return RedirectToAction(nameof(Index));
            }

            if (IsFinished(session))
            {
                return RedirectToAction(nameof(Result), new { sessionId });
            }

            var model = new ExamViewModel
            {
                IsAuthenticated = true,
                LaunchSessionId = sessionId,
                LaunchQuestionNumber = Math.Max(number, 1),
                LaunchExamName = session.SampleExamName,
                ErrorMessage = TempData["ExamError"]?.ToString()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Session(long sessionId, int number = 1, bool review = false, bool embedded = false, CancellationToken cancellationToken = default)
        {
            var accessToken = HttpContext.Session.GetString(AccessTokenSessionKey);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                TempData["ExamError"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại để tiếp tục làm bài.";
                return RedirectToAction("Index", "Login", new { returnUrl = Url.Action(nameof(Index), "Exam") });
            }

            var session = await _examApiService.GetSessionAsync(sessionId, accessToken, cancellationToken);
            if (session == null)
            {
                TempData["ExamError"] = "Không tìm thấy phiên thi.";
                return RedirectToAction(nameof(Index));
            }

            if (IsFinished(session) && !review)
            {
                return RedirectToAction(nameof(Result), new { sessionId });
            }

            var safeNumber = Math.Min(Math.Max(number, 1), Math.Max(session.TotalQuestions, 1));
            var question = await _examApiService.GetQuestionAsync(sessionId, safeNumber, accessToken, cancellationToken);
            if (question == null)
            {
                TempData["ExamError"] = "Không tải được câu hỏi của phiên thi.";
                return RedirectToAction(nameof(Index));
            }

            session.CurrentQuestionNumber = safeNumber;
            session.CurrentQuestion = question;
            session.IsReviewMode = review;
            session.IsEmbeddedMode = embedded;

            var model = new ExamViewModel
            {
                IsAuthenticated = true,
                SessionPage = session,
                ErrorMessage = TempData["ExamError"]?.ToString()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAnswer(long sessionId, int currentNumber, long questionId, long? answerId, string actionType, CancellationToken cancellationToken)
        {
            var accessToken = HttpContext.Session.GetString(AccessTokenSessionKey);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                TempData["ExamError"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại để tiếp tục làm bài.";
                return RedirectToAction(nameof(Index));
            }

            if (answerId.HasValue)
            {
                var saved = await _examApiService.SubmitAnswerAsync(sessionId, questionId, answerId.Value, accessToken, cancellationToken);
                if (!saved)
                {
                    TempData["ExamError"] = "Không thể lưu đáp án. Vui lòng thử lại.";
                    return RedirectToAction(nameof(Session), new { sessionId, number = currentNumber });
                }
            }
            else if (string.Equals(actionType, "submit", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ExamError"] = "Vui lòng chọn đáp án trước khi nộp bài ở câu hiện tại.";
                return RedirectToAction(nameof(Session), new { sessionId, number = currentNumber });
            }

            if (string.Equals(actionType, "submit", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction(nameof(Submit), new { sessionId });
            }

            var nextNumber = string.Equals(actionType, "previous", StringComparison.OrdinalIgnoreCase)
                ? Math.Max(currentNumber - 1, 1)
                : currentNumber + 1;

            return RedirectToAction(nameof(Session), new { sessionId, number = nextNumber });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(long sessionId, bool autoSubmit = false, CancellationToken cancellationToken = default)
        {
            var accessToken = HttpContext.Session.GetString(AccessTokenSessionKey);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                TempData["ExamError"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại để xem kết quả.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _examApiService.SubmitSessionAsync(sessionId, autoSubmit, accessToken, cancellationToken);
            if (result == null)
            {
                TempData["ExamError"] = autoSubmit
                    ? "Hết thời gian nhưng hệ thống chưa thể tự nộp bài. Vui lòng thử nộp lại thủ công."
                    : "Không thể nộp bài thi.";
                return RedirectToAction(nameof(Session), new { sessionId });
            }

            return RedirectToAction(nameof(Result), new { sessionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveAnswerAjax([FromBody] SaveAnswerAjaxRequest request, CancellationToken cancellationToken = default)
        {
            var accessToken = HttpContext.Session.GetString(AccessTokenSessionKey);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { success = false, message = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại." });
            }

            if (request.SessionId <= 0 || request.QuestionId <= 0 || request.AnswerId <= 0 || request.CurrentNumber <= 0)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json(new { success = false, message = "Dữ liệu lưu đáp án không hợp lệ." });
            }

            var saved = await _examApiService.SubmitAnswerAsync(request.SessionId, request.QuestionId, request.AnswerId, accessToken, cancellationToken);
            if (!saved)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json(new { success = false, message = "Không thể lưu đáp án. Vui lòng thử lại." });
            }

            return Json(new
            {
                success = true,
                message = "Đã lưu đáp án.",
                questionNumber = request.CurrentNumber,
                nextUrl = Url.Action(nameof(Session), new { sessionId = request.SessionId, number = request.CurrentNumber + 1 }),
                previousUrl = Url.Action(nameof(Session), new { sessionId = request.SessionId, number = Math.Max(request.CurrentNumber - 1, 1) })
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitAjax([FromBody] SubmitAjaxRequest request, CancellationToken cancellationToken = default)
        {
            var accessToken = HttpContext.Session.GetString(AccessTokenSessionKey);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Json(new { success = false, message = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại." });
            }

            if (request.SessionId <= 0)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json(new { success = false, message = "Phiên thi không hợp lệ." });
            }

            var result = await _examApiService.SubmitSessionAsync(request.SessionId, request.AutoSubmit, accessToken, cancellationToken);
            if (result == null)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                return Json(new
                {
                    success = false,
                    message = request.AutoSubmit
                        ? "Hết thời gian nhưng hệ thống chưa thể tự nộp bài. Vui lòng thử nộp lại thủ công."
                        : "Không thể nộp bài thi."
                });
            }

            return Json(new
            {
                success = true,
                redirectUrl = Url.Action(nameof(Result), new { sessionId = request.SessionId })
            });
        }

        [HttpGet]
        public async Task<IActionResult> Result(long sessionId, CancellationToken cancellationToken)
        {
            var accessToken = HttpContext.Session.GetString(AccessTokenSessionKey);
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                TempData["ExamError"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại để xem kết quả.";
                return RedirectToAction(nameof(Index));
            }

            var result = await _examApiService.GetResultAsync(sessionId, accessToken, cancellationToken);
            if (result == null)
            {
                TempData["ExamError"] = "Không tải được kết quả bài thi.";
                return RedirectToAction(nameof(Index));
            }

            result.ReviewItems = await _examApiService.GetReviewAsync(sessionId, accessToken, cancellationToken);

            var model = new ExamViewModel
            {
                IsAuthenticated = true,
                SessionResult = result,
                ErrorMessage = TempData["ExamError"]?.ToString()
            };

            return View(model);
        }

        private static bool IsFinished(ExamSessionPageViewModel session)
        {
            if (session.SubmittedAt.HasValue)
            {
                return true;
            }

            if (string.IsNullOrWhiteSpace(session.Status))
            {
                return false;
            }

            return session.Status.Contains("submit", StringComparison.OrdinalIgnoreCase)
                || session.Status.Contains("complete", StringComparison.OrdinalIgnoreCase)
                || session.Status.Contains("finish", StringComparison.OrdinalIgnoreCase);
        }

        public class SaveAnswerAjaxRequest
        {
            public long SessionId { get; set; }
            public int CurrentNumber { get; set; }
            public long QuestionId { get; set; }

            [JsonPropertyName("answerId")]
            public long AnswerId { get; set; }
        }

        public class SubmitAjaxRequest
        {
            public long SessionId { get; set; }
            public bool AutoSubmit { get; set; }
        }
    }
}
