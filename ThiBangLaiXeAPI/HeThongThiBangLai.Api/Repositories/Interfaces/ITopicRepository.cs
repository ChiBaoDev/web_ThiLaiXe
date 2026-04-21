using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.Models;

namespace HeThongThiBangLai.Api.Repositories.Interfaces;

public interface ITopicRepository
{
    Task<chu_de_cau_hoi?> GetByIdAsync(long id);
    Task<chu_de_cau_hoi?> GetByCodeAsync(string code);
    Task<PagedList<chu_de_cau_hoi>> GetPagedAsync(int page, int pageSize, string? search = null);
    Task AddAsync(chu_de_cau_hoi topic);
    void Update(chu_de_cau_hoi topic);
    void Remove(chu_de_cau_hoi topic);
    Task SaveChangesAsync();
}