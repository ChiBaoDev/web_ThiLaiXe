using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.Data;
using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeThongThiBangLai.Api.Repositories.Topics;

public class TopicRepository : ITopicRepository
{
    private readonly ApplicationDbContext _context;

    public TopicRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<chu_de_cau_hoi?> GetByIdAsync(long id)
    {
        return await _context.chu_de_cau_hois
            .Include(x => x.cau_hois)
            .FirstOrDefaultAsync(x => x.id == id);
    }

    public async Task<chu_de_cau_hoi?> GetByCodeAsync(string code)
    {
        return await _context.chu_de_cau_hois.FirstOrDefaultAsync(x => x.ma_chu_de == code);
    }

    public async Task<PagedList<chu_de_cau_hoi>> GetPagedAsync(int page, int pageSize, string? search = null)
    {
        var query = _context.chu_de_cau_hois.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(t => t.ten_chu_de.Contains(search) || t.ma_chu_de.Contains(search));
        }

        return await PagedList<chu_de_cau_hoi>.CreateAsync(query, page, pageSize);
    }

    public async Task AddAsync(chu_de_cau_hoi topic)
    {
        await _context.chu_de_cau_hois.AddAsync(topic);
    }

    public void Update(chu_de_cau_hoi topic)
    {
        _context.chu_de_cau_hois.Update(topic);
    }

    public void Remove(chu_de_cau_hoi topic)
    {
        _context.chu_de_cau_hois.Remove(topic);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}