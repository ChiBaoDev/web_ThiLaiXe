namespace webthibanglai.Models;

public class HomeViewModel
{
    public HomeDashboardOverview? Overview { get; set; }
    public HomeExamStats? ExamStats { get; set; }
    public List<HomeWeakTopicItem> WeakTopics { get; set; } = new();
    public HomeCriticalQuestionStats? CriticalQuestionStats { get; set; }
}

public class HomeDashboardOverview
{
    public int TotalCandidates { get; set; }
    public int TotalSessions { get; set; }
    public decimal PassRate { get; set; }
    public decimal AverageScore { get; set; }
    public decimal CriticalFailRate { get; set; }
}

public class HomeExamStats
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public int TotalSessions { get; set; }
    public int PassedSessions { get; set; }
    public int FailedSessions { get; set; }
    public decimal PassRate { get; set; }
    public decimal AverageScore { get; set; }
    public List<HomeTrendPointItem> DailyTrend { get; set; } = new();
}

public class HomeTrendPointItem
{
    public DateTime Date { get; set; }
    public int SessionCount { get; set; }
}

public class HomeWeakTopicItem
{
    public long TopicId { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public int TotalAnswered { get; set; }
    public int WrongCount { get; set; }
    public decimal AccuracyRate { get; set; }
}

public class HomeCriticalQuestionStats
{
    public int TotalCriticalAttempts { get; set; }
    public int WrongCriticalAttempts { get; set; }
    public decimal CriticalErrorRate { get; set; }
    public List<HomeQuestionErrorItem> TopCriticalWrongQuestions { get; set; } = new();
}

public class HomeQuestionErrorItem
{
    public long QuestionId { get; set; }
    public string QuestionContent { get; set; } = string.Empty;
    public int WrongCount { get; set; }
}

