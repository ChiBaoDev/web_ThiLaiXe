using AutoMapper;
using HeThongThiBangLai.Api.Common.Exceptions;
using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.Topics;
using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using HeThongThiBangLai.Api.Services.Interfaces;

namespace HeThongThiBangLai.Api.Services.Topics;

public class TopicService : ITopicService
{
    private readonly ITopicRepository _repository;
    private readonly IMapper _mapper;

    public TopicService(ITopicRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<TopicDto>> GetByIdAsync(long id)
    {
        var topic = await _repository.GetByIdAsync(id);
        if (topic == null)
            return ApiResponseFactory.Fail<TopicDto>("Topic not found");

        var dto = _mapper.Map<TopicDto>(topic);
        return ApiResponseFactory.Success(dto, "Topic retrieved successfully");
    }

    public async Task<ApiResponse<PagedList<TopicDto>>> GetListAsync(int page = 1, int pageSize = 20, string? search = null)
    {
        var pagedTopics = await _repository.GetPagedAsync(page, pageSize, search);
        var dtos = _mapper.Map<List<TopicDto>>(pagedTopics.Items);

        var pagedDtos = new PagedList<TopicDto>(dtos, pagedTopics.TotalCount, page, pageSize);

        return ApiResponseFactory.SuccessPaged(pagedDtos, "Topics retrieved successfully");
    }

    public async Task<ApiResponse<TopicDto>> CreateAsync(CreateTopicRequestDto request)
    {
        var existingByCode = await _repository.GetByCodeAsync(request.Code);
        if (existingByCode != null)
        {
            throw new ConflictAppException("Topic code already exists", "TOPIC_CODE_EXISTS");
        }

        var topic = _mapper.Map<chu_de_cau_hoi>(request);
        await _repository.AddAsync(topic);
        await _repository.SaveChangesAsync();

        var dto = _mapper.Map<TopicDto>(topic);
        return ApiResponseFactory.Created(dto, "Topic created successfully");
    }

    public async Task<ApiResponse<TopicDto>> UpdateAsync(long id, UpdateTopicRequestDto request)
    {
        var topic = await _repository.GetByIdAsync(id);
        if (topic == null)
        {
            throw new NotFoundAppException("Topic not found");
        }

        var existingByCode = await _repository.GetByCodeAsync(request.Code);
        if (existingByCode != null && existingByCode.id != id)
        {
            throw new ConflictAppException("Topic code already exists", "TOPIC_CODE_EXISTS");
        }

        _mapper.Map(request, topic);
        _repository.Update(topic);
        await _repository.SaveChangesAsync();

        var dto = _mapper.Map<TopicDto>(topic);
        return ApiResponseFactory.Success(dto, "Topic updated successfully");
    }

    public async Task DeleteAsync(long id)
    {
        var topic = await _repository.GetByIdAsync(id);
        if (topic == null)
        {
            throw new NotFoundAppException("Topic not found");
        }

        if (topic.cau_hois.Count > 0)
        {
            throw new ConflictAppException("Cannot delete topic with linked questions", "TOPIC_HAS_QUESTIONS");
        }

        _repository.Remove(topic);
        await _repository.SaveChangesAsync();
    }
}