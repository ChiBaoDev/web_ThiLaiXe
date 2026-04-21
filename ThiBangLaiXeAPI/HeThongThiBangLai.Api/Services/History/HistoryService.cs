using HeThongThiBangLai.Api.Common.Exceptions;
using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.History;
using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using HeThongThiBangLai.Api.Services.Interfaces;

namespace HeThongThiBangLai.Api.Services.History;

public class HistoryService : IHistoryService
{
    private readonly IHistoryRepository _repository;

    public HistoryService(IHistoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<PagedList<ExamHistoryItemDto>>> GetCandidateExamHistoryAsync(long userId, int page = 1, int pageSize = 20, DateTime? from = null, DateTime? to = null, string? result = null)
    {
        var student = await _repository.GetStudentByUserIdAsync(userId);
        if (student == null)
        {
            throw new NotFoundAppException("Candidate profile not found");
        }

        var paged = await _repository.GetExamListForStudentAsync(student.id, page, pageSize, from, to, result);
        var items = paged.Items.Select(MapItem).ToList();

        var mapped = new PagedList<ExamHistoryItemDto>(items, paged.TotalCount, page, pageSize);
        return ApiResponseFactory.SuccessPaged(mapped, "Exam history retrieved successfully");
    }

    public async Task<ApiResponse<ExamHistoryDetailDto>> GetCandidateExamHistoryDetailAsync(long userId, long sessionId)
    {
        var student = await _repository.GetStudentByUserIdAsync(userId);
        if (student == null)
        {
            throw new NotFoundAppException("Candidate profile not found");
        }

        var exam = await _repository.GetExamByIdForStudentAsync(sessionId, student.id);
        if (exam == null)
        {
            throw new NotFoundAppException("Exam session not found");
        }

        return ApiResponseFactory.Success(MapDetail(exam), "Exam session detail retrieved successfully");
    }

    public async Task<ApiResponse<ExamHistoryAnalyticsDto>> GetCandidateAnalyticsAsync(long userId, DateTime? from = null, DateTime? to = null)
    {
        var student = await _repository.GetStudentByUserIdAsync(userId);
        if (student == null)
        {
            throw new NotFoundAppException("Candidate profile not found");
        }

        var paged = await _repository.GetExamListForStudentAsync(student.id, 1, 1000, from, to);
        var analytics = BuildAnalytics(paged.Items);

        return ApiResponseFactory.Success(analytics, "Exam history analytics retrieved successfully");
    }

    public async Task<ApiResponse<PagedList<ExamHistoryItemDto>>> GetAdminExamHistoryAsync(int page = 1, int pageSize = 20, DateTime? from = null, DateTime? to = null, string? result = null)
    {
        var paged = await _repository.GetExamListForAdminAsync(page, pageSize, from, to, result);
        var items = paged.Items.Select(MapItem).ToList();

        var mapped = new PagedList<ExamHistoryItemDto>(items, paged.TotalCount, page, pageSize);
        return ApiResponseFactory.SuccessPaged(mapped, "Admin exam history retrieved successfully");
    }

    public async Task<ApiResponse<PagedList<ExamHistoryItemDto>>> GetAdminUserExamHistoryAsync(long userId, int page = 1, int pageSize = 20, DateTime? from = null, DateTime? to = null, string? result = null)
    {
        var student = await _repository.GetStudentByUserIdAsync(userId);
        if (student == null)
        {
            throw new NotFoundAppException("Candidate profile not found");
        }

        var paged = await _repository.GetExamListByStudentIdForAdminAsync(student.id, page, pageSize, from, to, result);
        var items = paged.Items.Select(MapItem).ToList();

        var mapped = new PagedList<ExamHistoryItemDto>(items, paged.TotalCount, page, pageSize);
        return ApiResponseFactory.SuccessPaged(mapped, "Admin user exam history retrieved successfully");
    }

    private static ExamHistoryItemDto MapItem(bai_thi exam)
    {
        return new ExamHistoryItemDto
        {
            SessionId = exam.id,
            SampleExamId = exam.de_thi_id,
            SampleExamName = exam.de_thi?.ten_de_thi ?? string.Empty,
            StartedAt = exam.thoi_gian_bat_dau,
            SubmittedAt = exam.thoi_gian_nop,
            TotalQuestions = exam.tong_so_cau,
            CorrectAnswers = exam.so_cau_dung,
            Score = exam.diem,
            Result = exam.ket_qua,
            Status = exam.trang_thai
        };
    }

    private static ExamHistoryDetailDto MapDetail(bai_thi exam)
    {
        return new ExamHistoryDetailDto
        {
            SessionId = exam.id,
            StudentId = exam.hoc_vien_id,
            SampleExamId = exam.de_thi_id,
            SampleExamName = exam.de_thi?.ten_de_thi ?? string.Empty,
            StartedAt = exam.thoi_gian_bat_dau,
            SubmittedAt = exam.thoi_gian_nop,
            TotalQuestions = exam.tong_so_cau,
            CorrectAnswers = exam.so_cau_dung,
            Score = exam.diem,
            Result = exam.ket_qua,
            Status = exam.trang_thai
        };
    }

    private static ExamHistoryAnalyticsDto BuildAnalytics(List<bai_thi> exams)
    {
        var total = exams.Count;
        var passed = exams.Count(x => x.ket_qua == "pass");
        var failed = exams.Count(x => x.ket_qua == "fail");
        var avgScore = total == 0 ? 0 : exams.Average(x => x.diem);
        var passRate = total == 0 ? 0 : (decimal)passed * 100 / total;

        return new ExamHistoryAnalyticsDto
        {
            TotalSessions = total,
            PassedSessions = passed,
            FailedSessions = failed,
            AverageScore = decimal.Round(avgScore, 2),
            PassRate = decimal.Round(passRate, 2)
        };
    }
}
