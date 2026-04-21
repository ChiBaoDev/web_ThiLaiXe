namespace HeThongThiBangLai.Api.DTOs.Dashboard;

public class DashboardWeakTopicDto
{
    public long TopicId { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public int TotalAnswered { get; set; }
    public int WrongCount { get; set; }
    public decimal AccuracyRate { get; set; }
}
