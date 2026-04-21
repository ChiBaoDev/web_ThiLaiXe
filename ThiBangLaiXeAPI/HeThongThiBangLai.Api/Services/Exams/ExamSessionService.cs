using HeThongThiBangLai.Api.Common.Exceptions;
using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.ExamSessions;
using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using HeThongThiBangLai.Api.Services.Interfaces;

namespace HeThongThiBangLai.Api.Services.Exams;

public class ExamSessionService : IExamSessionService
{
    private readonly IExamSessionRepository _repository;

    public ExamSessionService(IExamSessionRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<StartExamSessionResponseDto>> StartSampleExamAsync(long userId, long sampleExamId)
    {
        var student = await _repository.GetStudentByUserIdAsync(userId)
            ?? throw new NotFoundAppException("Candidate profile not found");

        var sampleExam = await _repository.GetPublishedSampleExamByIdAsync(sampleExamId)
            ?? throw new NotFoundAppException("Published sample exam not found");

        if (sampleExam.de_thi_cau_hois.Count != sampleExam.tong_so_cau)
        {
            throw new BusinessRuleAppException("Sample exam structure is invalid", "INVALID_SAMPLE_EXAM_STRUCTURE");
        }

        var startedAt = DateTime.UtcNow;
        var session = new bai_thi
        {
            hoc_vien_id = student.id,
            de_thi_id = sampleExam.id,
            ca_thi_id = sampleExam.ky_thi_id,
            thoi_gian_bat_dau = startedAt,
            tong_so_cau = sampleExam.tong_so_cau,
            so_cau_dung = 0,
            diem = 0,
            ket_qua = null,
            trang_thai = "dang_lam"
        };

        await _repository.AddExamSessionAsync(session);
        await _repository.SaveChangesAsync();

        var orderedQuestions = sampleExam.de_thi_cau_hois
            .OrderBy(x => x.thu_tu_cau)
            .ToList();

        var details = orderedQuestions.Select(x => new chi_tiet_bai_thi
        {
            bai_thi_id = session.id,
            cau_hoi_id = x.cau_hoi_id,
            dap_an_chon_id = null,
            la_dung = null
        });

        await _repository.AddExamSessionDetailsAsync(details);

        await _repository.AddSystemLogAsync(new nhat_ky_he_thong
        {
            nguoi_dung_id = userId,
            hanh_dong = "exam_started",
            bang_tac_dong = "bai_thi",
            khoa_chinh_du_lieu = session.id,
            noi_dung = $"Started sample exam {sampleExam.id}",
            created_at = startedAt
        });

        await _repository.SaveChangesAsync();

        var result = new StartExamSessionResponseDto
        {
            SessionId = session.id,
            SampleExamId = sampleExam.id,
            SampleExamName = sampleExam.ten_de_thi,
            TotalQuestions = sampleExam.tong_so_cau,
            DurationMinutes = sampleExam.thoi_gian_lam_bai,
            StartedAt = startedAt,
            Status = session.trang_thai
        };

        return ApiResponseFactory.Created(result, "Exam session started successfully");
    }

    public async Task<ApiResponse<ExamSessionDto>> GetSessionAsync(long userId, long sessionId)
    {
        var session = await GetSessionOrThrowAsync(userId, sessionId);

        var remaining = CalculateRemainingSeconds(session);

        var dto = new ExamSessionDto
        {
            SessionId = session.id,
            SampleExamId = session.de_thi_id,
            SampleExamName = session.de_thi.ten_de_thi,
            TotalQuestions = session.tong_so_cau,
            CorrectAnswers = session.so_cau_dung,
            Score = session.diem,
            Result = session.ket_qua,
            Status = session.trang_thai,
            StartedAt = session.thoi_gian_bat_dau,
            SubmittedAt = session.thoi_gian_nop,
            DurationMinutes = session.de_thi.thoi_gian_lam_bai,
            RemainingSeconds = remaining
        };

        return ApiResponseFactory.Success(dto, "Exam session retrieved successfully");
    }

    public async Task<ApiResponse<ExamSessionQuestionDto>> GetQuestionAsync(long userId, long sessionId, int number)
    {
        var session = await GetSessionOrThrowAsync(userId, sessionId);
        var details = session.chi_tiet_bai_this.OrderBy(x => x.id).ToList();

        if (number < 1 || number > details.Count)
        {
            throw new NotFoundAppException("Question number not found");
        }

        var detail = details[number - 1];
        var dto = MapQuestion(detail, number);
        return ApiResponseFactory.Success(dto, "Exam question retrieved successfully");
    }

    public async Task<ApiResponse<object>> SubmitAnswerAsync(long userId, long sessionId, SubmitExamAnswerRequestDto request)
    {
        var session = await GetSessionOrThrowAsync(userId, sessionId);
        EnsureSessionCanEdit(session);

        var detail = session.chi_tiet_bai_this.FirstOrDefault(x => x.cau_hoi_id == request.QuestionId)
            ?? throw new NotFoundAppException("Question not found in this exam session");

        var selectedAnswer = detail.cau_hoi.dap_ans.FirstOrDefault(x => x.id == request.AnswerId)
            ?? throw new NotFoundAppException("Answer not found for this question");

        detail.dap_an_chon_id = selectedAnswer.id;
        detail.la_dung = selectedAnswer.la_dap_an_dung;

        _repository.UpdateExamSessionDetail(detail);
        await _repository.SaveChangesAsync();

        return ApiResponseFactory.Success<object>(new
        {
            sessionId,
            questionId = request.QuestionId,
            answerId = request.AnswerId,
            isCorrect = detail.la_dung
        }, "Answer submitted successfully");
    }

    public async Task<ApiResponse<ExamSessionResultDto>> SubmitAsync(long userId, long sessionId, bool isAutoSubmit = false)
    {
        var session = await GetSessionOrThrowAsync(userId, sessionId);
        EnsureSessionCanEdit(session);

        var details = session.chi_tiet_bai_this.ToList();
        var total = details.Count;
        var correct = details.Count(x => x.la_dung == true);
        var wrong = details.Count(x => x.la_dung == false);
        var unanswered = details.Count(x => !x.dap_an_chon_id.HasValue);

        var failedByCriticalQuestion = details.Any(x => x.la_dung == false && x.cau_hoi.la_cau_diem_liet);
        var passingCorrectAnswers = GetPassingCorrectAnswers(total);
        var passed = correct >= passingCorrectAnswers && !failedByCriticalQuestion;

        var now = DateTime.UtcNow;
        session.so_cau_dung = correct;
        session.tong_so_cau = total;
        session.diem = total == 0 ? 0 : decimal.Round((decimal)correct * 10 / total, 2);
        session.ket_qua = passed ? "pass" : "fail";
        session.thoi_gian_nop = now;
        session.trang_thai = isAutoSubmit ? "tu_dong_nop" : "da_nop";

        _repository.UpdateExamSession(session);

        await _repository.AddSystemLogAsync(new nhat_ky_he_thong
        {
            nguoi_dung_id = userId,
            hanh_dong = isAutoSubmit ? "exam_auto_submitted" : "exam_submitted",
            bang_tac_dong = "bai_thi",
            khoa_chinh_du_lieu = session.id,
            noi_dung = $"Submitted exam session {session.id}",
            created_at = now
        });

        await _repository.SaveChangesAsync();

        return ApiResponseFactory.Success(BuildResult(session, wrong, unanswered, failedByCriticalQuestion), "Exam submitted successfully");
    }

    public async Task<ApiResponse<ExamSessionResultDto>> GetResultAsync(long userId, long sessionId)
    {
        var session = await GetSessionOrThrowAsync(userId, sessionId);
        EnsureSessionSubmitted(session);

        var details = session.chi_tiet_bai_this.ToList();
        var wrong = details.Count(x => x.la_dung == false);
        var unanswered = details.Count(x => !x.dap_an_chon_id.HasValue);
        var failedByCriticalQuestion = details.Any(x => x.la_dung == false && x.cau_hoi.la_cau_diem_liet);

        return ApiResponseFactory.Success(BuildResult(session, wrong, unanswered, failedByCriticalQuestion), "Exam result retrieved successfully");
    }

    public async Task<ApiResponse<ExamSessionReviewDto>> GetReviewAsync(long userId, long sessionId)
    {
        var session = await GetSessionOrThrowAsync(userId, sessionId);
        EnsureSessionSubmitted(session);

        var items = session.chi_tiet_bai_this
            .OrderBy(x => x.id)
            .Select((x, index) => new ExamSessionReviewItemDto
            {
                Number = index + 1,
                QuestionId = x.cau_hoi_id,
                QuestionContent = x.cau_hoi.noi_dung,
                IsCritical = x.cau_hoi.la_cau_diem_liet,
                SelectedAnswerId = x.dap_an_chon_id,
                CorrectAnswerId = x.cau_hoi.dap_ans.FirstOrDefault(a => a.la_dap_an_dung)?.id,
                IsCorrect = x.la_dung
            })
            .ToList();

        return ApiResponseFactory.Success(new ExamSessionReviewDto
        {
            SessionId = session.id,
            Items = items
        }, "Exam review retrieved successfully");
    }

    private async Task<bai_thi> GetSessionOrThrowAsync(long userId, long sessionId)
    {
        var student = await _repository.GetStudentByUserIdAsync(userId)
            ?? throw new NotFoundAppException("Candidate profile not found");

        var session = await _repository.GetSessionByIdForStudentAsync(sessionId, student.id)
            ?? throw new NotFoundAppException("Exam session not found");

        return session;
    }

    private static void EnsureSessionCanEdit(bai_thi session)
    {
        if (session.thoi_gian_nop.HasValue)
        {
            throw new BusinessRuleAppException("Exam session already submitted", "EXAM_SESSION_ALREADY_SUBMITTED");
        }
    }

    private static void EnsureSessionSubmitted(bai_thi session)
    {
        if (!session.thoi_gian_nop.HasValue)
        {
            throw new BusinessRuleAppException("Exam session has not been submitted", "EXAM_SESSION_NOT_SUBMITTED");
        }
    }

    private static int CalculateRemainingSeconds(bai_thi session)
    {
        if (!session.thoi_gian_bat_dau.HasValue || session.thoi_gian_nop.HasValue)
        {
            return 0;
        }

        var duration = TimeSpan.FromMinutes(session.de_thi.thoi_gian_lam_bai);
        var endAt = session.thoi_gian_bat_dau.Value.Add(duration);
        var remain = endAt - DateTime.UtcNow;
        return remain.TotalSeconds <= 0 ? 0 : (int)remain.TotalSeconds;
    }

    private static int GetPassingCorrectAnswers(int totalQuestions)
    {
        if (totalQuestions == 25)
        {
            return 21;
        }

        return (int)Math.Ceiling(totalQuestions * 0.84m);
    }

    private static ExamSessionResultDto BuildResult(bai_thi session, int wrong, int unanswered, bool failedByCriticalQuestion)
    {
        return new ExamSessionResultDto
        {
            SessionId = session.id,
            TotalQuestions = session.tong_so_cau,
            CorrectAnswers = session.so_cau_dung,
            WrongAnswers = wrong,
            UnansweredAnswers = unanswered,
            Score = session.diem,
            Result = session.ket_qua ?? string.Empty,
            FailedByCriticalQuestion = failedByCriticalQuestion,
            SubmittedAt = session.thoi_gian_nop,
            Status = session.trang_thai
        };
    }

    private static ExamSessionQuestionDto MapQuestion(chi_tiet_bai_thi detail, int number)
    {
        return new ExamSessionQuestionDto
        {
            Number = number,
            QuestionId = detail.cau_hoi_id,
            Content = detail.cau_hoi.noi_dung,
            TopicId = detail.cau_hoi.chu_de_id,
            IsCritical = detail.cau_hoi.la_cau_diem_liet,
            SelectedAnswerId = detail.dap_an_chon_id,
            Answers = detail.cau_hoi.dap_ans
                .OrderBy(x => x.thu_tu)
                .Select(x => new ExamSessionAnswerOptionDto
                {
                    AnswerId = x.id,
                    Content = x.noi_dung,
                    Order = x.thu_tu
                })
                .ToList()
        };
    }
}
