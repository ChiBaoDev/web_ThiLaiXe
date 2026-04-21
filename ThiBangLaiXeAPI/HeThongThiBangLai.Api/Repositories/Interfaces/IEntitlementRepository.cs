using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.Models;

namespace HeThongThiBangLai.Api.Repositories.Interfaces;

public interface IEntitlementRepository
{
    Task<goi_quyen?> GetPackageByIdAsync(long id);
    Task<goi_quyen?> GetPackageByCodeAsync(string code);
    Task<PagedList<goi_quyen>> GetPackagesPagedAsync(int page, int pageSize, string? search = null, bool? isActive = null);
    Task AddPackageAsync(goi_quyen package);
    void UpdatePackage(goi_quyen package);
    void RemovePackage(goi_quyen package);
    Task<bool> HasAnyUserEntitlementByPackageIdAsync(long packageId);

    Task<quyen_su_dung?> GetUserEntitlementByIdAsync(long id);
    Task<PagedList<quyen_su_dung>> GetUserEntitlementsPagedAsync(int page, int pageSize, long? userId = null, string? status = null);
    Task AddUserEntitlementAsync(quyen_su_dung entitlement);
    void UpdateUserEntitlement(quyen_su_dung entitlement);

    Task<bool> UserExistsAsync(long userId);
    Task<bool> PackageExistsAsync(long packageId);

    Task SaveChangesAsync();
}
