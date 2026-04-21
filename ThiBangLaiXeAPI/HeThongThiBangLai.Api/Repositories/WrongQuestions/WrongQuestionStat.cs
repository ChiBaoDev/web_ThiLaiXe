namespace HeThongThiBangLai.Api.Repositories.WrongQuestions;

public class WrongQuestionStat
{
    public long QuestionId { get; set; }
    public int WrongCount { get; set; }
    public DateTime? LastWrongAt { get; set; }
}
