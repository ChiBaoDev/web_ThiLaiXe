using AutoMapper;
using HeThongThiBangLai.Api.Common.Exceptions;
using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.Questions;
using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using HeThongThiBangLai.Api.Services.Interfaces;

namespace HeThongThiBangLai.Api.Services.Questions;

public class QuestionService : IQuestionService
{
    private readonly IQuestionRepository _repository;
    private readonly IMapper _mapper;

    public QuestionService(IQuestionRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<QuestionDto>> GetByIdAsync(long id)
    {
        var question = await _repository.GetByIdAsync(id);
        if (question == null)
            return ApiResponseFactory.Fail<QuestionDto>("Question not found");

        var dto = _mapper.Map<QuestionDto>(question);
        return ApiResponseFactory.Success(dto, "Question retrieved successfully");
    }

    public async Task<ApiResponse<PagedList<QuestionListResponseDto>>> GetListAsync(int page = 1, int pageSize = 20, string? search = null)
    {
        var pagedQuestions = await _repository.GetPagedAsync(page, pageSize, search);
        var dtos = _mapper.Map<List<QuestionListResponseDto>>(pagedQuestions.Items);

        var pagedDtos = new PagedList<QuestionListResponseDto>(dtos, pagedQuestions.TotalCount, page, pageSize);

        return ApiResponseFactory.SuccessPaged(pagedDtos, "Questions retrieved successfully");
    }

    public async Task<ApiResponse<QuestionDto>> CreateAsync(CreateQuestionRequestDto request)
    {
        var question = _mapper.Map<cau_hoi>(request);
        question.trang_thai = "draft";

        await _repository.AddAsync(question);
        await _repository.SaveChangesAsync();

        var dto = _mapper.Map<QuestionDto>(question);
        return ApiResponseFactory.Created(dto, "Question created successfully");
    }

    public async Task<ApiResponse<QuestionDto>> UpdateAsync(long id, UpdateQuestionRequestDto request)
    {
        var question = await _repository.GetByIdAsync(id);
        if (question == null)
        {
            throw new NotFoundAppException("Question not found");
        }

        _mapper.Map(request, question);
        _repository.Update(question);
        await _repository.SaveChangesAsync();

        var dto = _mapper.Map<QuestionDto>(question);
        return ApiResponseFactory.Success(dto, "Question updated successfully");
    }

    public async Task<ApiResponse<QuestionDto>> ApproveAsync(long id)
    {
        var question = await _repository.GetByIdAsync(id);
        if (question == null)
        {
            throw new NotFoundAppException("Question not found");
        }

        question.trang_thai = "approved";
        _repository.Update(question);
        await _repository.SaveChangesAsync();

        var dto = _mapper.Map<QuestionDto>(question);
        return ApiResponseFactory.Success(dto, "Question approved successfully");
    }

    public async Task<ApiResponse<QuestionDto>> ArchiveAsync(long id)
    {
        var question = await _repository.GetByIdAsync(id);
        if (question == null)
        {
            throw new NotFoundAppException("Question not found");
        }

        question.trang_thai = "archived";
        _repository.Update(question);
        await _repository.SaveChangesAsync();

        var dto = _mapper.Map<QuestionDto>(question);
        return ApiResponseFactory.Success(dto, "Question archived successfully");
    }

    public async Task DeleteAsync(long id)
    {
        var question = await _repository.GetByIdAsync(id);
        if (question == null)
        {
            throw new NotFoundAppException("Question not found");
        }

        _repository.Remove(question);
        await _repository.SaveChangesAsync();
    }
}
