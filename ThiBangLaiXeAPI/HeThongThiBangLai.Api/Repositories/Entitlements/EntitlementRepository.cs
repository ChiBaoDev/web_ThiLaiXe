using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.Data;
using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeThongThiBangLai.Api.Repositories.Entitlements;

public class EntitlementRepository : IEntitlementRepository
{
    private readonly ApplicationDbContext _context;

    public EntitlementRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<goi_quyen?> GetPackageByIdAsync(long id)
    {
        return await _context.goi_quyens.FindAsync(id);
    }

    public async Task<goi_quyen?> GetPackageByCodeAsync(string code)
    {
        return await _context.goi_quyens.FirstOrDefaultAsync(x => x.ma_goi == code);
    }

    public async Task<PagedList<goi_quyen>> GetPackagesPagedAsync(int page, int pageSize, string? search = null, bool? isActive = null)
    {
        var query = _context.goi_quyens.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.ma_goi.Contains(search) || x.ten_goi.Contains(search));
        }

        if (isActive.HasValue)
        {
            query = query.Where(x => x.is_active == isActive.Value);
        }

        query = query.OrderByDescending(x => x.created_at);
        return await PagedList<goi_quyen>.CreateAsync(query, page, pageSize);
    }

    public async Task AddPackageAsync(goi_quyen package)
    {
        await _context.goi_quyens.AddAsync(package);
    }

    public void UpdatePackage(goi_quyen package)
    {
        _context.goi_quyens.Update(package);
    }

    public void RemovePackage(goi_quyen package)
    {
        _context.goi_quyens.Remove(package);
    }

    public async Task<bool> HasAnyUserEntitlementByPackageIdAsync(long packageId)
    {
        return await _context.quyen_su_dungs.AnyAsync(x => x.goi_quyen_id == packageId);
    }

    public async Task<quyen_su_dung?> GetUserEntitlementByIdAsync(long id)
    {
        return await _context.quyen_su_dungs.FirstOrDefaultAsync(x => x.id == id);
    }

    public async Task<PagedList<quyen_su_dung>> GetUserEntitlementsPagedAsync(int page, int pageSize, long? userId = null, string? status = null)
    {
        var query = _context.quyen_su_dungs.AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(x => x.nguoi_dung_id == userId.Value);
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.trang_thai == status);
        }

        query = query.OrderByDescending(x => x.created_at);
        return await PagedList<quyen_su_dung>.CreateAsync(query, page, pageSize);
    }

    public async Task AddUserEntitlementAsync(quyen_su_dung entitlement)
    {
        await _context.quyen_su_dungs.AddAsync(entitlement);
    }

    public void UpdateUserEntitlement(quyen_su_dung entitlement)
    {
        _context.quyen_su_dungs.Update(entitlement);
    }

    public async Task<bool> UserExistsAsync(long userId)
    {
        return await _context.nguoi_dungs.AnyAsync(x => x.id == userId);
    }

    public async Task<bool> PackageExistsAsync(long packageId)
    {
        return await _context.goi_quyens.AnyAsync(x => x.id == packageId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
