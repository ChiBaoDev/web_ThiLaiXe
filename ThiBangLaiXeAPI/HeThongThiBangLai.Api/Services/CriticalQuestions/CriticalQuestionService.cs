using AutoMapper;
using HeThongThiBangLai.Api.Common.Exceptions;
using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.CriticalQuestions;
using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using HeThongThiBangLai.Api.Services.Interfaces;

namespace HeThongThiBangLai.Api.Services.CriticalQuestions;

public class CriticalQuestionService : ICriticalQuestionService
{
    private readonly ICriticalQuestionRepository _repository;
    private readonly IMapper _mapper;

    public CriticalQuestionService(ICriticalQuestionRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<List<CriticalQuestionDto>>> GetListAsync()
    {
        var questions = await _repository.GetCriticalQuestionsAsync();
        var dto = _mapper.Map<List<CriticalQuestionDto>>(questions);
        return ApiResponseFactory.Success(dto, "Critical questions retrieved successfully");
    }

    public async Task<ApiResponse<CriticalQuestionSummaryDto>> GetSummaryAsync(long userId)
    {
        var student = await _repository.GetStudentByUserIdAsync(userId);
        if (student == null)
        {
            throw new NotFoundAppException("Candidate profile not found");
        }

        var criticalQuestions = await _repository.GetCriticalQuestionsAsync();
        var sessionCount = await _repository.GetCriticalPracticeSessionCountAsync(student.id);
        var latestAt = await _repository.GetLatestCriticalPracticeAtAsync(student.id);

        var summary = new CriticalQuestionSummaryDto
        {
            TotalCriticalQuestions = criticalQuestions.Count,
            TotalPracticeSessions = sessionCount,
            LatestPracticeAt = latestAt
        };

        return ApiResponseFactory.Success(summary, "Critical question summary retrieved successfully");
    }

    public async Task<ApiResponse<CriticalPracticeSessionDto>> StartPracticeAsync(long userId, StartCriticalPracticeRequestDto request)
    {
        var student = await _repository.GetStudentByUserIdAsync(userId);
        if (student == null)
        {
            throw new NotFoundAppException("Candidate profile not found");
        }

        var criticalQuestions = await _repository.GetCriticalQuestionsAsync();
        if (criticalQuestions.Count < request.Size)
        {
            throw new BusinessRuleAppException("Not enough critical questions for practice", "NOT_ENOUGH_CRITICAL_QUESTIONS");
        }

        var selectedQuestions = criticalQuestions
            .OrderBy(_ => Guid.NewGuid())
            .Take(request.Size)
            .ToList();

        var session = new phien_on_tap
        {
            hoc_vien_id = student.id,
            ngay_tao = DateTime.UtcNow,
            thoi_gian_bat_dau = DateTime.UtcNow,
            tong_so_cau = selectedQuestions.Count,
            so_cau_dung = 0,
            diem = 0,
            trang_thai = "critical_practice_started"
        };

        await _repository.AddPracticeSessionAsync(session);
        await _repository.SaveChangesAsync();

        var sessionQuestions = selectedQuestions
            .Select((question, index) => new phien_on_tap_cau_hoi
            {
                phien_on_tap_id = session.id,
                cau_hoi_id = question.id,
                thu_tu_cau = index + 1
            })
            .ToList();

        await _repository.AddPracticeSessionQuestionsAsync(sessionQuestions);
        await _repository.SaveChangesAsync();

        var result = new CriticalPracticeSessionDto
        {
            SessionId = session.id,
            TotalQuestions = selectedQuestions.Count,
            StartedAt = session.thoi_gian_bat_dau ?? DateTime.UtcNow,
            Status = session.trang_thai,
            QuestionIds = selectedQuestions.Select(x => x.id).ToList()
        };

        return ApiResponseFactory.Created(result, "Critical practice started successfully");
    }
}
