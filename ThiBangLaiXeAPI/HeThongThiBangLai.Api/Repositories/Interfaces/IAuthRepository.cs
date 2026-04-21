using HeThongThiBangLai.Api.Models;

namespace HeThongThiBangLai.Api.Repositories.Interfaces;

public interface IAuthRepository
{
    Task<nguoi_dung?> FindUserByUsernameAsync(string username);
    Task<nguoi_dung?> FindUserByEmailAsync(string email);
    Task<nguoi_dung?> FindUserByIdAsync(long userId);
    Task<List<vai_tro>> GetRolesByUserIdAsync(long userId);
    Task<vai_tro?> FindRoleByCodeAsync(string roleCode);
    Task<hoc_vien?> FindHocVienByUserIdAsync(long userId);

    Task AddUserAsync(nguoi_dung user);
    Task AddUserRoleAsync(nguoi_dung_vai_tro userRole);
    Task AddHocVienProfileAsync(hoc_vien hocVienProfile);
    Task UpdateUserAsync(nguoi_dung user);
    Task UpdateHocVienProfileAsync(hoc_vien hocVienProfile);

    Task AddSystemLogAsync(nhat_ky_he_thong systemLog);
    Task<List<nhat_ky_he_thong>> GetSystemLogsByUserAndActionAsync(long userId, string action);
    Task UpdateSystemLogAsync(nhat_ky_he_thong systemLog);
    Task<int> SaveChangesAsync();
}