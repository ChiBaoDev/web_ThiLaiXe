using AutoMapper;
using HeThongThiBangLai.Api.Common.Exceptions;
using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.Files;
using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using HeThongThiBangLai.Api.Services.Interfaces;

namespace HeThongThiBangLai.Api.Services.Files;

public class FileService : IFileService
{
    private readonly IFileRepository _repository;
    private readonly IMapper _mapper;

    public FileService(IFileRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<FileDto>> GetByIdAsync(long id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return ApiResponseFactory.Fail<FileDto>("File not found");

        var dto = _mapper.Map<FileDto>(entity);
        return ApiResponseFactory.Success(dto, "File retrieved successfully");
    }

    public async Task<ApiResponse<PagedList<FileListResponseDto>>> GetListAsync(int page = 1, int pageSize = 20, string? search = null, string? status = null)
    {
        var pagedFiles = await _repository.GetPagedAsync(page, pageSize, search, status);
        var dtos = _mapper.Map<List<FileListResponseDto>>(pagedFiles.Items);
        var pagedDtos = new PagedList<FileListResponseDto>(dtos, pagedFiles.TotalCount, page, pageSize);

        return ApiResponseFactory.SuccessPaged(pagedDtos, "Files retrieved successfully");
    }

    public async Task<ApiResponse<FileDto>> CreateAsync(CreateFileRequestDto request)
    {
        var entity = _mapper.Map<files>(request);
        entity.trang_thai = "active";

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        var dto = _mapper.Map<FileDto>(entity);
        return ApiResponseFactory.Created(dto, "File created successfully");
    }

    public async Task<ApiResponse<FileDto>> UpdateAsync(long id, UpdateFileRequestDto request)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            throw new NotFoundAppException("File not found");
        }

        if (request.PublicUrl != null) entity.public_url = request.PublicUrl;
        if (request.FileName != null) entity.file_name = request.FileName;
        if (request.MimeType != null) entity.mime_type = request.MimeType;
        if (request.SizeBytes.HasValue) entity.size_bytes = request.SizeBytes.Value;
        if (request.ChecksumSha256 != null) entity.checksum_sha256 = request.ChecksumSha256;
        if (request.Width.HasValue) entity.width = request.Width;
        if (request.Height.HasValue) entity.height = request.Height;
        if (request.DurationSeconds.HasValue) entity.duration_seconds = request.DurationSeconds;
        if (request.Status != null) entity.trang_thai = request.Status;
        entity.updated_at = DateTime.UtcNow;

        _repository.Update(entity);
        await _repository.SaveChangesAsync();

        var dto = _mapper.Map<FileDto>(entity);
        return ApiResponseFactory.Success(dto, "File updated successfully");
    }

    public async Task DeleteAsync(long id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            throw new NotFoundAppException("File not found");
        }

        var usages = await _repository.GetUsagesByFileIdAsync(id);
        if (usages.Count > 0)
        {
            throw new ConflictAppException("Cannot delete file that is currently in use", "FILE_IN_USE");
        }

        _repository.Remove(entity);
        await _repository.SaveChangesAsync();
    }

    public async Task<ApiResponse<List<FileUsageDto>>> GetUsagesAsync(long fileId)
    {
        var entity = await _repository.GetByIdAsync(fileId);
        if (entity == null)
        {
            throw new NotFoundAppException("File not found");
        }

        var usages = await _repository.GetUsagesByFileIdAsync(fileId);
        var dtos = _mapper.Map<List<FileUsageDto>>(usages);

        return ApiResponseFactory.Success(dtos, "File usages retrieved successfully");
    }

    public async Task<ApiResponse<FileUsageDto>> AddUsageAsync(long fileId, CreateFileUsageRequestDto request)
    {
        var entity = await _repository.GetByIdAsync(fileId);
        if (entity == null)
        {
            throw new NotFoundAppException("File not found");
        }

        var exists = await _repository.ExistsUsageAsync(fileId, request.EntityName, request.EntityId, request.FieldName);
        if (exists)
        {
            throw new ConflictAppException("File usage already exists", "FILE_USAGE_EXISTS");
        }

        var usage = new file_usages
        {
            file_id = fileId,
            entity_name = request.EntityName,
            entity_id = request.EntityId,
            field_name = request.FieldName,
            is_primary = request.IsPrimary,
            sort_order = request.SortOrder
        };

        await _repository.AddUsageAsync(usage);
        await _repository.SaveChangesAsync();

        var dto = _mapper.Map<FileUsageDto>(usage);
        return ApiResponseFactory.Created(dto, "File usage created successfully");
    }
}
