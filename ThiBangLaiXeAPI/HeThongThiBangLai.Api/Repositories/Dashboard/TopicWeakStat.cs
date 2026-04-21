namespace HeThongThiBangLai.Api.Repositories.Dashboard;

public class TopicWeakStat
{
    public long TopicId { get; set; }
    public string TopicName { get; set; } = string.Empty;
    public int TotalAnswered { get; set; }
    public int WrongCount { get; set; }
}
