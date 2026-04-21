using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.Entitlements;

namespace HeThongThiBangLai.Api.Services.Interfaces;

public interface IEntitlementService
{
    Task<ApiResponse<PagedList<EntitlementPackageDto>>> GetPackagesAsync(int page = 1, int pageSize = 20, string? search = null, bool? isActive = null);
    Task<ApiResponse<EntitlementPackageDto>> GetPackageByIdAsync(long id);
    Task<ApiResponse<EntitlementPackageDto>> CreatePackageAsync(CreateEntitlementPackageRequestDto request);
    Task<ApiResponse<EntitlementPackageDto>> UpdatePackageAsync(long id, UpdateEntitlementPackageRequestDto request);
    Task DeletePackageAsync(long id);

    Task<ApiResponse<PagedList<UserEntitlementDto>>> GetUserEntitlementsAsync(int page = 1, int pageSize = 20, long? userId = null, string? status = null);
    Task<ApiResponse<UserEntitlementDto>> GetUserEntitlementByIdAsync(long id);
    Task<ApiResponse<UserEntitlementDto>> GrantUserEntitlementAsync(GrantUserEntitlementRequestDto request, long? createdBy = null);
    Task<ApiResponse<UserEntitlementDto>> UpdateUserEntitlementStatusAsync(long id, UpdateUserEntitlementStatusRequestDto request);
}
