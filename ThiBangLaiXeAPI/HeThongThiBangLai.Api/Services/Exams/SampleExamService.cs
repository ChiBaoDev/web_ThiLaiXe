using AutoMapper;
using HeThongThiBangLai.Api.Common.Exceptions;
using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.Exams;
using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using HeThongThiBangLai.Api.Services.Interfaces;

namespace HeThongThiBangLai.Api.Services.Exams;

public class SampleExamService : ISampleExamService
{
    private readonly ISampleExamRepository _repository;
    private readonly IMapper _mapper;

    public SampleExamService(ISampleExamRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedList<SampleExamDto>>> GetListAsync(int page = 1, int pageSize = 20, string? search = null)
    {
        var pagedExams = await _repository.GetPagedAsync(page, pageSize, search);
        var dtos = _mapper.Map<List<SampleExamDto>>(pagedExams.Items);

        var pagedDtos = new PagedList<SampleExamDto>(dtos, pagedExams.TotalCount, page, pageSize);
        return ApiResponseFactory.SuccessPaged(pagedDtos, "Sample exams retrieved successfully");
    }

    public async Task<ApiResponse<SampleExamDto>> GetByIdAsync(long id)
    {
        var exam = await _repository.GetByIdAsync(id);
        if (exam == null)
        {
            throw new NotFoundAppException("Sample exam not found");
        }

        var dto = _mapper.Map<SampleExamDto>(exam);
        return ApiResponseFactory.Success(dto, "Sample exam retrieved successfully");
    }

    public async Task<ApiResponse<SampleExamDto>> CreateAsync(CreateSampleExamRequestDto request)
    {
        var existingByCode = await _repository.GetByCodeAsync(request.Code);
        if (existingByCode != null)
        {
            throw new ConflictAppException("Sample exam code already exists", "SAMPLE_EXAM_CODE_EXISTS");
        }

        var examPeriod = await _repository.GetExamPeriodByIdAsync(request.ExamPeriodId);
        if (examPeriod == null)
        {
            throw new NotFoundAppException("Exam period not found");
        }

        var sampleExam = _mapper.Map<de_thi>(request);
        sampleExam.trang_thai = "nhap";
        sampleExam.ngay_tao = DateTime.UtcNow;

        await _repository.AddAsync(sampleExam);
        await _repository.SaveChangesAsync();

        var dto = _mapper.Map<SampleExamDto>(sampleExam);
        return ApiResponseFactory.Created(dto, "Sample exam created successfully");
    }

    public async Task<ApiResponse<SampleExamDto>> UpdateAsync(long id, UpdateSampleExamRequestDto request)
    {
        var sampleExam = await _repository.GetByIdAsync(id);
        if (sampleExam == null)
        {
            throw new NotFoundAppException("Sample exam not found");
        }

        var existingByCode = await _repository.GetByCodeAsync(request.Code);
        if (existingByCode != null && existingByCode.id != id)
        {
            throw new ConflictAppException("Sample exam code already exists", "SAMPLE_EXAM_CODE_EXISTS");
        }

        var examPeriod = await _repository.GetExamPeriodByIdAsync(request.ExamPeriodId);
        if (examPeriod == null)
        {
            throw new NotFoundAppException("Exam period not found");
        }

        _mapper.Map(request, sampleExam);
        _repository.Update(sampleExam);
        await _repository.SaveChangesAsync();

        var dto = _mapper.Map<SampleExamDto>(sampleExam);
        return ApiResponseFactory.Success(dto, "Sample exam updated successfully");
    }

    public async Task<ApiResponse<SampleExamDto>> AssignQuestionsAsync(long id, AssignSampleExamQuestionsRequestDto request)
    {
        var sampleExam = await _repository.GetByIdAsync(id);
        if (sampleExam == null)
        {
            throw new NotFoundAppException("Sample exam not found");
        }

        var requestQuestionIds = request.QuestionIds.Distinct().ToList();
        if (requestQuestionIds.Count != request.QuestionIds.Count)
        {
            throw new ConflictAppException("Duplicate question ids in request", "DUPLICATE_QUESTION_IDS");
        }

        var questions = await _repository.GetQuestionsByIdsAsync(requestQuestionIds);
        if (questions.Count != requestQuestionIds.Count)
        {
            throw new NotFoundAppException("One or more questions not found");
        }

        var invalidStatusQuestion = questions.FirstOrDefault(q => !string.Equals(q.trang_thai, "approved", StringComparison.OrdinalIgnoreCase));
        if (invalidStatusQuestion != null)
        {
            throw new BusinessRuleAppException("All linked questions must be approved", "QUESTION_NOT_APPROVED");
        }

        var existedQuestionIds = sampleExam.de_thi_cau_hois.Select(x => x.cau_hoi_id).ToHashSet();
        var duplicatedWithExam = requestQuestionIds.FirstOrDefault(existedQuestionIds.Contains);
        if (duplicatedWithExam > 0)
        {
            throw new ConflictAppException("Question already assigned to sample exam", "QUESTION_ALREADY_ASSIGNED");
        }

        var totalAfterAssign = sampleExam.de_thi_cau_hois.Count + requestQuestionIds.Count;
        if (totalAfterAssign > sampleExam.tong_so_cau)
        {
            throw new BusinessRuleAppException("Total assigned questions exceed sample exam structure", "TOTAL_QUESTIONS_EXCEEDED");
        }

        var order = await _repository.GetMaxQuestionOrderAsync(id);
        foreach (var questionId in requestQuestionIds)
        {
            order++;
            await _repository.AddAssignmentAsync(new de_thi_cau_hoi
            {
                de_thi_id = id,
                cau_hoi_id = questionId,
                thu_tu_cau = order
            });
        }

        await _repository.SaveChangesAsync();

        var latestExam = await _repository.GetByIdAsync(id);
        var dto = _mapper.Map<SampleExamDto>(latestExam);
        return ApiResponseFactory.Success(dto, "Questions assigned successfully");
    }

    public async Task DeleteQuestionAsync(long id, long questionId)
    {
        var sampleExam = await _repository.GetByIdAsync(id);
        if (sampleExam == null)
        {
            throw new NotFoundAppException("Sample exam not found");
        }

        var assignment = await _repository.GetAssignmentAsync(id, questionId);
        if (assignment == null)
        {
            throw new NotFoundAppException("Question assignment not found");
        }

        _repository.RemoveAssignment(assignment);
        await _repository.SaveChangesAsync();
    }

    public async Task<ApiResponse<SampleExamDto>> PublishAsync(long id)
    {
        var sampleExam = await _repository.GetByIdAsync(id);
        if (sampleExam == null)
        {
            throw new NotFoundAppException("Sample exam not found");
        }

        if (sampleExam.de_thi_cau_hois.Count != sampleExam.tong_so_cau)
        {
            throw new BusinessRuleAppException("Total question count must match structure", "TOTAL_QUESTIONS_MISMATCH");
        }

        var questionIds = sampleExam.de_thi_cau_hois.Select(x => x.cau_hoi_id).ToList();
        var questions = await _repository.GetQuestionsByIdsAsync(questionIds);
        var invalidStatusQuestion = questions.FirstOrDefault(q => !string.Equals(q.trang_thai, "approved", StringComparison.OrdinalIgnoreCase));
        if (invalidStatusQuestion != null)
        {
            throw new BusinessRuleAppException("All linked questions must be approved", "QUESTION_NOT_APPROVED");
        }

        sampleExam.trang_thai = "published";
        _repository.Update(sampleExam);
        await _repository.SaveChangesAsync();

        var dto = _mapper.Map<SampleExamDto>(sampleExam);
        return ApiResponseFactory.Success(dto, "Sample exam published successfully");
    }

    public async Task DeleteAsync(long id)
    {
        var sampleExam = await _repository.GetByIdAsync(id);
        if (sampleExam == null)
        {
            throw new NotFoundAppException("Sample exam not found");
        }

        if (sampleExam.bai_this.Count > 0)
        {
            throw new ConflictAppException("Cannot delete sample exam with exam sessions", "SAMPLE_EXAM_HAS_SESSIONS");
        }

        if (sampleExam.de_thi_cau_hois.Count > 0)
        {
            _repository.RemoveAssignments(sampleExam.de_thi_cau_hois);
        }

        _repository.Remove(sampleExam);
        await _repository.SaveChangesAsync();
    }
}
