using System.Text.Json;
using HeThongThiBangLai.Api.Common.Exceptions;
using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.ExamRules;
using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using HeThongThiBangLai.Api.Services.Interfaces;

namespace HeThongThiBangLai.Api.Services.Exams;

public class ExamRuleService : IExamRuleService
{
    private readonly IExamRuleRepository _repository;

    public ExamRuleService(IExamRuleRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<List<ExamStructureRuleDto>>> GetListAsync()
    {
        var rules = await LoadRulesAsync();
        return ApiResponseFactory.Success(rules.OrderByDescending(x => x.UpdatedAt).ToList(), "Exam structure rules retrieved successfully");
    }

    public async Task<ApiResponse<ExamStructureRuleDto>> GetByIdAsync(long id)
    {
        var rule = await FindRuleOrThrowAsync(id);
        return ApiResponseFactory.Success(rule, "Exam structure rule retrieved successfully");
    }

    public async Task<ApiResponse<ExamStructureRuleDto>> CreateAsync(CreateExamStructureRuleRequestDto request)
    {
        await ValidateRequestAsync(request);

        var rules = await LoadRulesAsync();
        var now = DateTime.UtcNow;
        var id = rules.Count == 0 ? 1 : rules.Max(x => x.Id) + 1;

        var dto = new ExamStructureRuleDto
        {
            Id = id,
            Name = request.Name,
            TotalQuestions = request.TotalQuestions,
            DurationMinutes = request.DurationMinutes,
            PassingCorrectAnswers = request.PassingCorrectAnswers,
            RequiredCriticalQuestions = request.RequiredCriticalQuestions,
            AutoSubmitEnabled = request.AutoSubmitEnabled,
            CriticalFailEnabled = request.CriticalFailEnabled,
            IsActive = rules.Count == 0,
            UpdatedAt = now,
            TopicAllocations = request.TopicAllocations,
            DifficultyAllocations = request.DifficultyAllocations
        };

        await SaveRuleAsync("exam_rule_created", dto.Id, dto, $"Created exam rule: {dto.Name}");

        return ApiResponseFactory.Created(dto, "Exam structure rule created successfully");
    }

    public async Task<ApiResponse<ExamStructureRuleDto>> UpdateAsync(long id, UpdateExamStructureRuleRequestDto request)
    {
        await ValidateRequestAsync(request);

        var rules = await LoadRulesAsync();
        var existing = rules.FirstOrDefault(x => x.Id == id)
            ?? throw new NotFoundAppException("Exam structure rule not found");

        var updated = new ExamStructureRuleDto
        {
            Id = existing.Id,
            Name = request.Name,
            TotalQuestions = request.TotalQuestions,
            DurationMinutes = request.DurationMinutes,
            PassingCorrectAnswers = request.PassingCorrectAnswers,
            RequiredCriticalQuestions = request.RequiredCriticalQuestions,
            AutoSubmitEnabled = request.AutoSubmitEnabled,
            CriticalFailEnabled = request.CriticalFailEnabled,
            IsActive = existing.IsActive,
            UpdatedAt = DateTime.UtcNow,
            TopicAllocations = request.TopicAllocations,
            DifficultyAllocations = request.DifficultyAllocations
        };

        await SaveRuleAsync("exam_rule_updated", updated.Id, updated, $"Updated exam rule: {updated.Name}");

        return ApiResponseFactory.Success(updated, "Exam structure rule updated successfully");
    }

    public async Task<ApiResponse<ExamStructureRuleDto>> ActivateAsync(long id)
    {
        var rules = await LoadRulesAsync();
        var target = rules.FirstOrDefault(x => x.Id == id)
            ?? throw new NotFoundAppException("Exam structure rule not found");

        foreach (var rule in rules)
        {
            rule.IsActive = rule.Id == id;
            rule.UpdatedAt = DateTime.UtcNow;
        }

        await SaveSnapshotAsync(rules);

        return ApiResponseFactory.Success(target, "Exam structure rule activated successfully");
    }

    public async Task<ApiResponse<ExamRuleValidationResultDto>> ValidateAsync(long id)
    {
        var rule = await FindRuleOrThrowAsync(id);
        var errors = new List<string>();

        foreach (var allocation in rule.TopicAllocations)
        {
            var available = await _repository.CountApprovedQuestionsByTopicAsync(allocation.TopicId);
            if (available < allocation.QuestionCount)
            {
                errors.Add($"Topic {allocation.TopicId} requires {allocation.QuestionCount}, available {available}");
            }
        }

        foreach (var allocation in rule.DifficultyAllocations)
        {
            var available = await _repository.CountApprovedQuestionsByDifficultyAsync(allocation.Difficulty);
            if (available < allocation.QuestionCount)
            {
                errors.Add($"Difficulty '{allocation.Difficulty}' requires {allocation.QuestionCount}, available {available}");
            }
        }

        var availableCritical = await _repository.CountApprovedCriticalQuestionsAsync();
        if (availableCritical < rule.RequiredCriticalQuestions)
        {
            errors.Add($"Critical questions require {rule.RequiredCriticalQuestions}, available {availableCritical}");
        }

        var result = new ExamRuleValidationResultDto
        {
            IsValid = errors.Count == 0,
            Errors = errors
        };

        return ApiResponseFactory.Success(result, "Exam structure rule validation completed");
    }

    public async Task DeleteAsync(long id)
    {
        var rules = await LoadRulesAsync();
        var target = rules.FirstOrDefault(x => x.Id == id)
            ?? throw new NotFoundAppException("Exam structure rule not found");

        if (target.IsActive)
        {
            throw new ConflictAppException("Cannot delete active exam structure rule", "EXAM_RULE_IS_ACTIVE");
        }

        await SaveRuleAsync("exam_rule_deleted", id, null, $"Deleted exam rule id={id}");
    }

    private async Task<ExamStructureRuleDto> FindRuleOrThrowAsync(long id)
    {
        var rules = await LoadRulesAsync();
        return rules.FirstOrDefault(x => x.Id == id)
            ?? throw new NotFoundAppException("Exam structure rule not found");
    }

    private async Task<List<ExamStructureRuleDto>> LoadRulesAsync()
    {
        var logs = await _repository.GetRuleLogsAsync();
        var rules = new Dictionary<long, ExamStructureRuleDto>();

        foreach (var log in logs)
        {
            if (!log.khoa_chinh_du_lieu.HasValue)
            {
                continue;
            }

            var ruleId = log.khoa_chinh_du_lieu.Value;

            if (log.hanh_dong == "exam_rule_deleted")
            {
                rules.Remove(ruleId);
                continue;
            }

            if (string.IsNullOrWhiteSpace(log.noi_dung))
            {
                continue;
            }

            var payload = TryExtractPayload(log.noi_dung!);
            if (payload == null)
            {
                continue;
            }

            var dto = JsonSerializer.Deserialize<ExamStructureRuleDto>(payload, JsonOptions());
            if (dto == null)
            {
                continue;
            }

            dto.Id = ruleId;
            dto.UpdatedAt = log.created_at;
            rules[ruleId] = dto;
        }

        if (rules.Count > 0 && !rules.Values.Any(x => x.IsActive))
        {
            var latest = rules.Values.OrderByDescending(x => x.UpdatedAt).First();
            latest.IsActive = true;
        }

        return rules.Values.ToList();
    }

    private async Task SaveRuleAsync(string action, long ruleId, ExamStructureRuleDto? rule, string message)
    {
        string content;
        if (rule == null)
        {
            content = message;
        }
        else
        {
            var payload = JsonSerializer.Serialize(rule, JsonOptions());
            content = $"{message} | payload={payload}";
        }

        await _repository.AddSystemLogAsync(new nhat_ky_he_thong
        {
            nguoi_dung_id = null,
            hanh_dong = action,
            bang_tac_dong = "exam_structure_rule",
            khoa_chinh_du_lieu = ruleId,
            noi_dung = content,
            created_at = DateTime.UtcNow
        });

        await _repository.SaveChangesAsync();
    }

    private async Task SaveSnapshotAsync(List<ExamStructureRuleDto> rules)
    {
        foreach (var rule in rules)
        {
            await SaveRuleAsync("exam_rule_updated", rule.Id, rule, $"Activated rule snapshot: {rule.Name}");
        }
    }

    private static string? TryExtractPayload(string content)
    {
        const string marker = "| payload=";
        var index = content.IndexOf(marker, StringComparison.Ordinal);
        if (index < 0)
        {
            return null;
        }

        return content[(index + marker.Length)..];
    }

    private static JsonSerializerOptions JsonOptions()
    {
        return new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    private async Task ValidateRequestAsync(CreateExamStructureRuleRequestDto request)
    {
        if (request.TotalQuestions <= 0)
        {
            throw new BusinessRuleAppException("TotalQuestions must be greater than 0", "INVALID_TOTAL_QUESTIONS");
        }

        if (request.DurationMinutes <= 0)
        {
            throw new BusinessRuleAppException("DurationMinutes must be greater than 0", "INVALID_DURATION");
        }

        if (request.PassingCorrectAnswers <= 0 || request.PassingCorrectAnswers > request.TotalQuestions)
        {
            throw new BusinessRuleAppException("PassingCorrectAnswers must be between 1 and TotalQuestions", "INVALID_PASSING_CORRECT_ANSWERS");
        }

        var topicTotal = request.TopicAllocations.Sum(x => x.QuestionCount);
        if (topicTotal > 0 && topicTotal != request.TotalQuestions)
        {
            throw new BusinessRuleAppException("Sum of topic allocations must equal TotalQuestions", "INVALID_TOPIC_ALLOCATION");
        }

        var diffTotal = request.DifficultyAllocations.Sum(x => x.QuestionCount);
        if (diffTotal > 0 && diffTotal != request.TotalQuestions)
        {
            throw new BusinessRuleAppException("Sum of difficulty allocations must equal TotalQuestions", "INVALID_DIFFICULTY_ALLOCATION");
        }
    }

    private async Task ValidateRequestAsync(UpdateExamStructureRuleRequestDto request)
    {
        await ValidateRequestAsync(new CreateExamStructureRuleRequestDto
        {
            Name = request.Name,
            TotalQuestions = request.TotalQuestions,
            DurationMinutes = request.DurationMinutes,
            PassingCorrectAnswers = request.PassingCorrectAnswers,
            RequiredCriticalQuestions = request.RequiredCriticalQuestions,
            AutoSubmitEnabled = request.AutoSubmitEnabled,
            CriticalFailEnabled = request.CriticalFailEnabled,
            TopicAllocations = request.TopicAllocations,
            DifficultyAllocations = request.DifficultyAllocations
        });
    }
}
