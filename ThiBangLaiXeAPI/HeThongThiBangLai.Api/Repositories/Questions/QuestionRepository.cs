using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.Data;
using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeThongThiBangLai.Api.Repositories.Questions;

public class QuestionRepository : IQuestionRepository
{
    private readonly ApplicationDbContext _context;

    public QuestionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<cau_hoi?> GetByIdAsync(long id)
    {
        return await _context.cau_hois.FindAsync(id);
    }

    public async Task<List<cau_hoi>> GetAllAsync()
    {
        return await _context.cau_hois.ToListAsync();
    }

    public async Task<PagedList<cau_hoi>> GetPagedAsync(int page, int pageSize, string? search = null)
    {
        var query = _context.cau_hois.AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(q => q.noi_dung.Contains(search));
        }

        return await PagedList<cau_hoi>.CreateAsync(query, page, pageSize);
    }

    public async Task AddAsync(cau_hoi question)
    {
        await _context.cau_hois.AddAsync(question);
    }

    public void Update(cau_hoi question)
    {
        _context.cau_hois.Update(question);
    }

    public void Remove(cau_hoi question)
    {
        _context.cau_hois.Remove(question);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
