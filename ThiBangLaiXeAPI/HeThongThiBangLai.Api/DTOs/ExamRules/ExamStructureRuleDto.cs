namespace HeThongThiBangLai.Api.DTOs.ExamRules;

public class ExamStructureRuleDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TotalQuestions { get; set; }
    public int DurationMinutes { get; set; }
    public int PassingCorrectAnswers { get; set; }
    public int RequiredCriticalQuestions { get; set; }
    public bool AutoSubmitEnabled { get; set; }
    public bool CriticalFailEnabled { get; set; }
    public bool IsActive { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<ExamRuleTopicAllocationDto> TopicAllocations { get; set; } = [];
    public List<ExamRuleDifficultyAllocationDto> DifficultyAllocations { get; set; } = [];
}
