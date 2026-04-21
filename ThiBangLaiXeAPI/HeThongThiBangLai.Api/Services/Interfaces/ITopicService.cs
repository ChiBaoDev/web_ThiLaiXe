using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.Topics;

namespace HeThongThiBangLai.Api.Services.Interfaces;

public interface ITopicService
{
    Task<ApiResponse<TopicDto>> GetByIdAsync(long id);
    Task<ApiResponse<PagedList<TopicDto>>> GetListAsync(int page = 1, int pageSize = 20, string? search = null);
    Task<ApiResponse<TopicDto>> CreateAsync(CreateTopicRequestDto request);
    Task<ApiResponse<TopicDto>> UpdateAsync(long id, UpdateTopicRequestDto request);
    Task DeleteAsync(long id);
}