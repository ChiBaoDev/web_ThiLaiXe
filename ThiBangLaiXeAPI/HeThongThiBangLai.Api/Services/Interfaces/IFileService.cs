using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.Files;

namespace HeThongThiBangLai.Api.Services.Interfaces;

public interface IFileService
{
    Task<ApiResponse<FileDto>> GetByIdAsync(long id);
    Task<ApiResponse<PagedList<FileListResponseDto>>> GetListAsync(int page = 1, int pageSize = 20, string? search = null, string? status = null);
    Task<ApiResponse<FileDto>> CreateAsync(CreateFileRequestDto request);
    Task<ApiResponse<FileDto>> UpdateAsync(long id, UpdateFileRequestDto request);
    Task DeleteAsync(long id);
    Task<ApiResponse<List<FileUsageDto>>> GetUsagesAsync(long fileId);
    Task<ApiResponse<FileUsageDto>> AddUsageAsync(long fileId, CreateFileUsageRequestDto request);
}
