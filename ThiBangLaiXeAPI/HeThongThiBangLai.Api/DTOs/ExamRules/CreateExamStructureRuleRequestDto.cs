namespace HeThongThiBangLai.Api.DTOs.ExamRules;

public class CreateExamStructureRuleRequestDto
{
    public string Name { get; set; } = string.Empty;
    public int TotalQuestions { get; set; }
    public int DurationMinutes { get; set; }
    public int PassingCorrectAnswers { get; set; }
    public int RequiredCriticalQuestions { get; set; }
    public bool AutoSubmitEnabled { get; set; } = true;
    public bool CriticalFailEnabled { get; set; } = true;
    public List<ExamRuleTopicAllocationDto> TopicAllocations { get; set; } = [];
    public List<ExamRuleDifficultyAllocationDto> DifficultyAllocations { get; set; } = [];
}
