using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.Models;

namespace HeThongThiBangLai.Api.Repositories.Interfaces;

public interface IQuestionRepository
{
    Task<cau_hoi?> GetByIdAsync(long id);
    Task<List<cau_hoi>> GetAllAsync();
    Task<PagedList<cau_hoi>> GetPagedAsync(int page, int pageSize, string? search = null);
    Task AddAsync(cau_hoi question);
    void Update(cau_hoi question);
    void Remove(cau_hoi question);
    Task SaveChangesAsync();
}
