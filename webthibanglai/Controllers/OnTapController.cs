using Microsoft.AspNetCore.Mvc;
using webthibanglai.Services;

namespace webthibanglai.Controllers;

public class OnTapController : Controller
{
    private const string AccessTokenSessionKey = "AccessToken";
    private readonly IPracticeApiService _practiceApiService;
    private readonly ILogger<OnTapController> _logger;

    public OnTapController(IPracticeApiService practiceApiService, ILogger<OnTapController> logger)
    {
        _practiceApiService = practiceApiService;
        _logger = logger;
    }

    public IActionResult Index()
    {
        // Kiểm tra xem người dùng đã đăng nhập chưa
        var authUsername = TempData.Peek("AuthUsername")?.ToString();
        if (string.IsNullOrWhiteSpace(authUsername))
        {
            return RedirectToAction("Index", "Login");
        }

        return View();
    }

    [HttpGet]
    public IActionResult Launch(string topicCode)
    {
        var accessToken = HttpContext.Session.GetString(AccessTokenSessionKey);
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            TempData["PracticeError"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
            return RedirectToAction("Index", "Login");
        }

        var topicName = topicCode switch
        {
            "CD_QTGT" => "Khái niệm và quy tắc giao thông",
            "CD_LIET" => "Câu hỏi điểm liệt",
            "CD_VH" => "Văn hóa và đạo đức người lái xe",
            "CD_KT" => "Kỹ thuật lái xe",
            "CD_BH" => "Biển báo đường bộ",
            "CD_SH" => "Sa hình",
            "" => "Tất cả 250 câu hỏi",
            _ => "Ôn tập"
        };

        ViewBag.TopicCode = topicCode;
        ViewBag.TopicName = topicName;
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> Practice(string topicCode, bool embedded = false, CancellationToken cancellationToken = default)
    {
        var accessToken = HttpContext.Session.GetString(AccessTokenSessionKey);
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            TempData["PracticeError"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
            return RedirectToAction("Index", "Login");
        }

        var questions = await _practiceApiService.GetQuestionsByTopicAsync(topicCode, accessToken, cancellationToken);
        if (questions == null || questions.Items.Count == 0)
        {
            TempData["PracticeError"] = "Không tìm thấy câu hỏi cho chủ đề này.";
            return RedirectToAction(nameof(Index));
        }

        // Lưu questions vào session để sử dụng trong practice
        var jsonOptions = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        HttpContext.Session.SetString($"Practice_{topicCode}",
            System.Text.Json.JsonSerializer.Serialize(questions, jsonOptions));

        return RedirectToAction(nameof(Session), new { topicCode, number = 1, embedded });
    }

    [HttpGet]
    public async Task<IActionResult> Session(string topicCode, int number = 1, bool embedded = false, CancellationToken cancellationToken = default)
    {
        var accessToken = HttpContext.Session.GetString(AccessTokenSessionKey);
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            TempData["PracticeError"] = "Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.";
            return RedirectToAction("Index", "Login");
        }

        var questionsJson = HttpContext.Session.GetString($"Practice_{topicCode}");
        
        // Nếu Session hết hạn, tự động reload từ API
        if (string.IsNullOrWhiteSpace(questionsJson))
        {
            Console.WriteLine($"[Session] Session expired for {topicCode}, reloading from API...");
            var questionsResponse = await _practiceApiService.GetQuestionsByTopicAsync(topicCode, accessToken, cancellationToken);
            
            if (questionsResponse == null || questionsResponse.Items == null || !questionsResponse.Items.Any())
            {
                TempData["PracticeError"] = "Không thể tải dữ liệu câu hỏi từ API.";
                return RedirectToAction(nameof(Index));
            }

            questionsJson = System.Text.Json.JsonSerializer.Serialize(questionsResponse);
            HttpContext.Session.SetString($"Practice_{topicCode}", questionsJson);
            Console.WriteLine($"[Session] Reloaded {questionsResponse.Items.Count} questions for {topicCode}");
        }

        var jsonOptions = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var questions = System.Text.Json.JsonSerializer.Deserialize<PracticeQuestionsResponse>(questionsJson, jsonOptions);
        if (questions == null || questions.Items.Count == 0)
        {
            TempData["PracticeError"] = "Không tìm thấy câu hỏi.";
            return RedirectToAction(nameof(Index));
        }

        var safeNumber = Math.Min(Math.Max(number, 1), questions.Items.Count);
        var currentQuestion = questions.Items[safeNumber - 1];

        ViewBag.TopicCode = topicCode;
        ViewBag.TopicName = currentQuestion.TopicName;
        ViewBag.CurrentNumber = safeNumber;
        ViewBag.Questions = questions.Items;
        ViewBag.ErrorMessage = TempData["PracticeError"]?.ToString();
        ViewBag.IsEmbedded = embedded;

        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetQuestionJson(string topicCode, int number, CancellationToken cancellationToken = default)
    {
        var accessToken = HttpContext.Session.GetString(AccessTokenSessionKey);
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return Json(new { success = false, message = "Phiên đăng nhập đã hết hạn." });
        }

        var questionsJson = HttpContext.Session.GetString($"Practice_{topicCode}");
        
        // Nếu Session hết hạn, tự động reload từ API
        if (string.IsNullOrWhiteSpace(questionsJson))
        {
            var questionsResponse = await _practiceApiService.GetQuestionsByTopicAsync(topicCode, accessToken, cancellationToken);
            
            if (questionsResponse == null || questionsResponse.Items == null || !questionsResponse.Items.Any())
            {
                return Json(new { success = false, message = "Không thể tải dữ liệu câu hỏi từ API." });
            }

            questionsJson = System.Text.Json.JsonSerializer.Serialize(questionsResponse);
            HttpContext.Session.SetString($"Practice_{topicCode}", questionsJson);
        }

        var jsonOptions = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var questions = System.Text.Json.JsonSerializer.Deserialize<PracticeQuestionsResponse>(questionsJson, jsonOptions);
        if (questions == null || questions.Items.Count == 0)
        {
            return Json(new { success = false, message = "Không tìm thấy câu hỏi." });
        }

        var safeNumber = Math.Min(Math.Max(number, 1), questions.Items.Count);
        var currentQuestion = questions.Items[safeNumber - 1];
        var orderedAnswers = currentQuestion.Answers.OrderBy(x => x.Order).ToList();

        return Json(new
        {
            success = true,
            question = new
            {
                id = currentQuestion.Id,
                content = currentQuestion.Content,
                imageUrl = currentQuestion.ImageUrl,
                isCritical = currentQuestion.IsCritical,
                topicName = currentQuestion.TopicName,
                number = safeNumber,
                total = questions.Items.Count,
                answers = orderedAnswers.Select(a => new
                {
                    answerId = a.AnswerId,
                    content = a.Content,
                    order = a.Order,
                    isCorrect = a.IsCorrect
                }).ToList()
            }
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckAnswer(string topicCode, long questionId, long answerId, CancellationToken cancellationToken)
    {
        var accessToken = HttpContext.Session.GetString(AccessTokenSessionKey);
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            return Json(new { success = false, message = "Phiên đăng nhập đã hết hạn." });
        }

        var questionsJson = HttpContext.Session.GetString($"Practice_{topicCode}");
        
        // Nếu Session hết hạn, tự động reload từ API
        if (string.IsNullOrWhiteSpace(questionsJson))
        {
            Console.WriteLine($"Session expired for {topicCode}, reloading from API...");
            var questionsResponse = await _practiceApiService.GetQuestionsByTopicAsync(topicCode, accessToken, cancellationToken);
            
            if (questionsResponse == null || questionsResponse.Items == null || !questionsResponse.Items.Any())
            {
                return Json(new { success = false, message = "Không thể tải dữ liệu câu hỏi." });
            }

            questionsJson = System.Text.Json.JsonSerializer.Serialize(questionsResponse);
            HttpContext.Session.SetString($"Practice_{topicCode}", questionsJson);
            Console.WriteLine($"Reloaded {questionsResponse.Items.Count} questions for {topicCode}");
        }

        var jsonOptions = new System.Text.Json.JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var questions = System.Text.Json.JsonSerializer.Deserialize<PracticeQuestionsResponse>(questionsJson, jsonOptions);
        var question = questions?.Items.FirstOrDefault(q => q.Id == questionId);
        
        if (question == null)
        {
            return Json(new { success = false, message = "Không tìm thấy câu hỏi." });
        }

        var selectedAnswer = question.Answers.FirstOrDefault(a => a.AnswerId == answerId);
        var correctAnswer = question.Answers.FirstOrDefault(a => a.IsCorrect == true);

        if (selectedAnswer == null || correctAnswer == null)
        {
            return Json(new { success = false, message = "Đáp án không hợp lệ." });
        }

        var isCorrect = selectedAnswer.IsCorrect == true;

        // Nếu sai, lưu vào wrong questions (không chờ kết quả)
        if (!isCorrect)
        {
            _ = _practiceApiService.RecordWrongAnswerAsync(questionId, answerId, accessToken, cancellationToken);
        }

        var response = new
        {
            success = true,
            isCorrect,
            selectedAnswerId = answerId,
            correctAnswerId = correctAnswer.AnswerId
        };

        // Debug log
        Console.WriteLine($"=== CheckAnswer Response ===");
        Console.WriteLine($"QuestionId: {questionId}");
        Console.WriteLine($"SelectedAnswerId: {answerId}");
        Console.WriteLine($"CorrectAnswerId: {correctAnswer.AnswerId}");
        Console.WriteLine($"IsCorrect: {isCorrect}");
        Console.WriteLine($"Response JSON: {System.Text.Json.JsonSerializer.Serialize(response)}");
        Console.WriteLine($"============================");

        return Json(response);
    }
}
