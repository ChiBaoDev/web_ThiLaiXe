namespace HeThongThiBangLai.Api.Repositories.Dashboard;

public class QuestionWrongStat
{
    public long QuestionId { get; set; }
    public string QuestionContent { get; set; } = string.Empty;
    public int WrongCount { get; set; }
}
