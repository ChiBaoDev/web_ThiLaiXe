using AutoMapper;
using HeThongThiBangLai.Api.Common.Exceptions;
using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.Entitlements;
using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using HeThongThiBangLai.Api.Services.Interfaces;

namespace HeThongThiBangLai.Api.Services.Entitlements;

public class EntitlementService : IEntitlementService
{
    private readonly IEntitlementRepository _repository;
    private readonly IMapper _mapper;

    public EntitlementService(IEntitlementRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedList<EntitlementPackageDto>>> GetPackagesAsync(int page = 1, int pageSize = 20, string? search = null, bool? isActive = null)
    {
        var paged = await _repository.GetPackagesPagedAsync(page, pageSize, search, isActive);
        var dtos = _mapper.Map<List<EntitlementPackageDto>>(paged.Items);
        var result = new PagedList<EntitlementPackageDto>(dtos, paged.TotalCount, page, pageSize);

        return ApiResponseFactory.SuccessPaged(result, "Entitlement packages retrieved successfully");
    }

    public async Task<ApiResponse<EntitlementPackageDto>> GetPackageByIdAsync(long id)
    {
        var entity = await _repository.GetPackageByIdAsync(id);
        if (entity == null)
            return ApiResponseFactory.Fail<EntitlementPackageDto>("Entitlement package not found");

        var dto = _mapper.Map<EntitlementPackageDto>(entity);
        return ApiResponseFactory.Success(dto, "Entitlement package retrieved successfully");
    }

    public async Task<ApiResponse<EntitlementPackageDto>> CreatePackageAsync(CreateEntitlementPackageRequestDto request)
    {
        var existingCode = await _repository.GetPackageByCodeAsync(request.Code);
        if (existingCode != null)
        {
            throw new ConflictAppException("Package code already exists", "PACKAGE_CODE_EXISTS");
        }

        var entity = _mapper.Map<goi_quyen>(request);
        await _repository.AddPackageAsync(entity);
        await _repository.SaveChangesAsync();

        var dto = _mapper.Map<EntitlementPackageDto>(entity);
        return ApiResponseFactory.Created(dto, "Entitlement package created successfully");
    }

    public async Task<ApiResponse<EntitlementPackageDto>> UpdatePackageAsync(long id, UpdateEntitlementPackageRequestDto request)
    {
        var entity = await _repository.GetPackageByIdAsync(id);
        if (entity == null)
        {
            throw new NotFoundAppException("Entitlement package not found");
        }

        var existingCode = await _repository.GetPackageByCodeAsync(request.Code);
        if (existingCode != null && existingCode.id != id)
        {
            throw new ConflictAppException("Package code already exists", "PACKAGE_CODE_EXISTS");
        }

        _mapper.Map(request, entity);
        entity.updated_at = DateTime.UtcNow;

        _repository.UpdatePackage(entity);
        await _repository.SaveChangesAsync();

        var dto = _mapper.Map<EntitlementPackageDto>(entity);
        return ApiResponseFactory.Success(dto, "Entitlement package updated successfully");
    }

    public async Task DeletePackageAsync(long id)
    {
        var entity = await _repository.GetPackageByIdAsync(id);
        if (entity == null)
        {
            throw new NotFoundAppException("Entitlement package not found");
        }

        var hasUsage = await _repository.HasAnyUserEntitlementByPackageIdAsync(id);
        if (hasUsage)
        {
            throw new ConflictAppException("Cannot delete package that has granted entitlements", "PACKAGE_IN_USE");
        }

        _repository.RemovePackage(entity);
        await _repository.SaveChangesAsync();
    }

    public async Task<ApiResponse<PagedList<UserEntitlementDto>>> GetUserEntitlementsAsync(int page = 1, int pageSize = 20, long? userId = null, string? status = null)
    {
        var paged = await _repository.GetUserEntitlementsPagedAsync(page, pageSize, userId, status);
        var dtos = _mapper.Map<List<UserEntitlementDto>>(paged.Items);
        var result = new PagedList<UserEntitlementDto>(dtos, paged.TotalCount, page, pageSize);

        return ApiResponseFactory.SuccessPaged(result, "User entitlements retrieved successfully");
    }

    public async Task<ApiResponse<UserEntitlementDto>> GetUserEntitlementByIdAsync(long id)
    {
        var entity = await _repository.GetUserEntitlementByIdAsync(id);
        if (entity == null)
            return ApiResponseFactory.Fail<UserEntitlementDto>("User entitlement not found");

        var dto = _mapper.Map<UserEntitlementDto>(entity);
        return ApiResponseFactory.Success(dto, "User entitlement retrieved successfully");
    }

    public async Task<ApiResponse<UserEntitlementDto>> GrantUserEntitlementAsync(GrantUserEntitlementRequestDto request, long? createdBy = null)
    {
        var userExists = await _repository.UserExistsAsync(request.UserId);
        if (!userExists)
        {
            throw new NotFoundAppException("User not found");
        }

        var packageExists = await _repository.PackageExistsAsync(request.PackageId);
        if (!packageExists)
        {
            throw new NotFoundAppException("Entitlement package not found");
        }

        var entity = _mapper.Map<quyen_su_dung>(request);
        entity.created_by = createdBy;
        entity.trang_thai = "active";

        await _repository.AddUserEntitlementAsync(entity);
        await _repository.SaveChangesAsync();

        var dto = _mapper.Map<UserEntitlementDto>(entity);
        return ApiResponseFactory.Created(dto, "User entitlement granted successfully");
    }

    public async Task<ApiResponse<UserEntitlementDto>> UpdateUserEntitlementStatusAsync(long id, UpdateUserEntitlementStatusRequestDto request)
    {
        var entity = await _repository.GetUserEntitlementByIdAsync(id);
        if (entity == null)
        {
            throw new NotFoundAppException("User entitlement not found");
        }

        entity.trang_thai = request.Status;
        entity.ghi_chu = request.Note;
        entity.updated_at = DateTime.UtcNow;

        _repository.UpdateUserEntitlement(entity);
        await _repository.SaveChangesAsync();

        var dto = _mapper.Map<UserEntitlementDto>(entity);
        return ApiResponseFactory.Success(dto, "User entitlement status updated successfully");
    }
}
