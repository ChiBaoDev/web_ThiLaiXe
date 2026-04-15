namespace webthibanglai.Models;

public class ExamViewModel
{
    public List<ExamPaperSummaryItem> ExamPapers { get; set; } = new();
    public PracticeSessionConfig? PracticeConfig { get; set; }
    public List<PracticeQuestionItem> Questions { get; set; } = new();
    public PracticeSubmitResult? Result { get; set; }
    public List<PracticeHistoryItem> History { get; set; } = new();
}

public class ExamPaperSummaryItem
{
    public int PaperId { get; set; }
    public string TenDeThi { get; set; } = string.Empty;
    public string LoaiBangLai { get; set; } = string.Empty;
    public int SoCau { get; set; }
    public int ThoiGianThiPhut { get; set; }
    public int DiemDat { get; set; }
    public string TrangThai { get; set; } = string.Empty;
}

public class PracticeSessionConfig
{
    public int PracticeSessionId { get; set; }
    public string TrangThai { get; set; } = string.Empty;
    public DateTime ThoiGianBatDau { get; set; }
    public int SoCauHoi { get; set; }
}

public class PracticeQuestionItem
{
    public int Stt { get; set; }
    public int QuestionId { get; set; }
    public string NoiDung { get; set; } = string.Empty;
    public bool LaCauDiemLiet { get; set; }
    public List<PracticeAnswerItem> Answers { get; set; } = new();
}

public class PracticeAnswerItem
{
    public int AnswerId { get; set; }
    public string NoiDung { get; set; } = string.Empty;
}

public class PracticeSubmitResult
{
    public int SoCauDung { get; set; }
    public int SoCauSai { get; set; }
    public int Diem { get; set; }
    public string ThoiGianLamBai { get; set; } = string.Empty;
    public string KetQua { get; set; } = string.Empty;
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
