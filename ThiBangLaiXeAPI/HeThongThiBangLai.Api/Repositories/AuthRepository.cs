using HeThongThiBangLai.Api.Data;
using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeThongThiBangLai.Api.Repositories;

public class AuthRepository : IAuthRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AuthRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<nguoi_dung?> FindUserByUsernameAsync(string username)
    {
        return await _dbContext.nguoi_dungs
            .FirstOrDefaultAsync(x => x.ten_dang_nhap == username);
    }

    public async Task<nguoi_dung?> FindUserByEmailAsync(string email)
    {
        return await _dbContext.nguoi_dungs
            .FirstOrDefaultAsync(x => x.email == email);
    }

    public async Task<nguoi_dung?> FindUserByIdAsync(long userId)
    {
        return await _dbContext.nguoi_dungs
            .FirstOrDefaultAsync(x => x.id == userId);
    }

    public async Task<List<vai_tro>> GetRolesByUserIdAsync(long userId)
    {
        return await _dbContext.nguoi_dung_vai_tros
            .Where(x => x.nguoi_dung_id == userId)
            .Select(x => x.vai_tro)
            .ToListAsync();
    }

    public async Task<vai_tro?> FindRoleByCodeAsync(string roleCode)
    {
        return await _dbContext.vai_tros
            .FirstOrDefaultAsync(x => x.ma_vai_tro == roleCode);
    }

    public async Task<hoc_vien?> FindHocVienByUserIdAsync(long userId)
    {
        return await _dbContext.hoc_viens
            .FirstOrDefaultAsync(x => x.nguoi_dung_id == userId);
    }

    public async Task AddUserAsync(nguoi_dung user)
    {
        await _dbContext.nguoi_dungs.AddAsync(user);
    }

    public async Task AddUserRoleAsync(nguoi_dung_vai_tro userRole)
    {
        await _dbContext.nguoi_dung_vai_tros.AddAsync(userRole);
    }

    public async Task AddHocVienProfileAsync(hoc_vien hocVienProfile)
    {
        await _dbContext.hoc_viens.AddAsync(hocVienProfile);
    }

    public Task UpdateUserAsync(nguoi_dung user)
    {
        _dbContext.nguoi_dungs.Update(user);
        return Task.CompletedTask;
    }

    public Task UpdateHocVienProfileAsync(hoc_vien hocVienProfile)
    {
        _dbContext.hoc_viens.Update(hocVienProfile);
        return Task.CompletedTask;
    }

    public async Task AddSystemLogAsync(nhat_ky_he_thong systemLog)
    {
        await _dbContext.nhat_ky_he_thongs.AddAsync(systemLog);
    }

    public async Task<List<nhat_ky_he_thong>> GetSystemLogsByUserAndActionAsync(long userId, string action)
    {
        return await _dbContext.nhat_ky_he_thongs
            .Where(x => x.nguoi_dung_id == userId && x.hanh_dong == action)
            .ToListAsync();
    }

    public Task UpdateSystemLogAsync(nhat_ky_he_thong systemLog)
    {
        _dbContext.nhat_ky_he_thongs.Update(systemLog);
        return Task.CompletedTask;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
}