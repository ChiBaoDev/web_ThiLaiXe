namespace webthibanglai.Models;

public class ExamViewModel
{
    public List<SampleExamItem> SampleExams { get; set; } = new();
    public SampleExamItem? SelectedSampleExam { get; set; }
    public ExamSessionPageViewModel? SessionPage { get; set; }
    public ExamSessionResultViewModel? SessionResult { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsAuthenticated { get; set; }
    public long LaunchSessionId { get; set; }
    public int LaunchQuestionNumber { get; set; } = 1;
    public string? LaunchExamName { get; set; }
}

public class SampleExamItem
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public long ExamPeriodId { get; set; }
    public int TotalQuestions { get; set; }
    public int DurationMinutes { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public int LinkedQuestionCount { get; set; }
}

public class PracticeHistoryItem
{
    public int PracticeSessionId { get; set; }
    public string Topic { get; set; } = string.Empty;
    public int SoCau { get; set; }
    public int SoDung { get; set; }
    public int Diem { get; set; }
    public DateTime NgayOnTap { get; set; }
}

public class ExamSessionPageViewModel
{
    public long SessionId { get; set; }
    public long SampleExamId { get; set; }
    public string SampleExamName { get; set; } = string.Empty;
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public decimal Score { get; set; }
    public string? Result { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? StartedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public int DurationMinutes { get; set; }
    public int RemainingSeconds { get; set; }
    public int CurrentQuestionNumber { get; set; }
    public bool IsReviewMode { get; set; }
    public bool IsEmbeddedMode { get; set; }
    public ExamSessionQuestionViewModel? CurrentQuestion { get; set; }
}

public class ExamSessionQuestionViewModel
{
    public int Number { get; set; }
    public long QuestionId { get; set; }
    public string Content { get; set; } = string.Empty;
    public long TopicId { get; set; }
    public bool IsCritical { get; set; }
    public long? SelectedAnswerId { get; set; }
    public string? ImageUrl { get; set; }
    public List<ExamSessionAnswerOptionViewModel> Answers { get; set; } = new();
}

public class ExamSessionAnswerOptionViewModel
{
    public long AnswerId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Order { get; set; }
}

public class ExamSessionResultViewModel
{
    public long SessionId { get; set; }
    public int TotalQuestions { get; set; }
    public int CorrectAnswers { get; set; }
    public int WrongAnswers { get; set; }
    public int UnansweredAnswers { get; set; }
    public decimal Score { get; set; }
    public string Result { get; set; } = string.Empty;
    public bool FailedByCriticalQuestion { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<ExamSessionReviewItemViewModel> ReviewItems { get; set; } = new();
}

public class ExamSessionReviewItemViewModel
{
    public int Number { get; set; }
    public long QuestionId { get; set; }
    public string QuestionContent { get; set; } = string.Empty;
    public bool IsCritical { get; set; }
    public long? SelectedAnswerId { get; set; }
    public long? CorrectAnswerId { get; set; }
    public bool? IsCorrect { get; set; }
}

public class StartExamSessionResponseViewModel
{
    public long SessionId { get; set; }
    public long SampleExamId { get; set; }
    public string SampleExamName { get; set; } = string.Empty;
    public int TotalQuestions { get; set; }
    public int DurationMinutes { get; set; }
    public DateTime StartedAt { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class ApiEnvelope<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<ApiErrorItem> Errors { get; set; } = new();
}

public class ApiErrorItem
{
    public string Code { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string Detail { get; set; } = string.Empty;
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}
