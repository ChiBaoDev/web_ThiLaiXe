using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.Data;
using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeThongThiBangLai.Api.Repositories.Exams;

public class SampleExamRepository : ISampleExamRepository
{
    private readonly ApplicationDbContext _context;

    public SampleExamRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<de_thi?> GetByIdAsync(long id)
    {
        return await _context.de_this
            .Include(x => x.de_thi_cau_hois)
            .Include(x => x.bai_this)
            .FirstOrDefaultAsync(x => x.id == id);
    }

    public async Task<de_thi?> GetByCodeAsync(string code)
    {
        return await _context.de_this.FirstOrDefaultAsync(x => x.ma_de_thi == code);
    }

    public async Task<PagedList<de_thi>> GetPagedAsync(int page, int pageSize, string? search = null)
    {
        var query = _context.de_this
            .Include(x => x.de_thi_cau_hois)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.ma_de_thi.Contains(search) || x.ten_de_thi.Contains(search));
        }

        return await PagedList<de_thi>.CreateAsync(query, page, pageSize);
    }

    public async Task<ky_thi?> GetExamPeriodByIdAsync(long id)
    {
        return await _context.ky_this.FirstOrDefaultAsync(x => x.id == id);
    }

    public async Task<List<cau_hoi>> GetQuestionsByIdsAsync(List<long> ids)
    {
        return await _context.cau_hois
            .Where(x => ids.Contains(x.id))
            .ToListAsync();
    }

    public async Task<de_thi_cau_hoi?> GetAssignmentAsync(long sampleExamId, long questionId)
    {
        return await _context.de_thi_cau_hois
            .FirstOrDefaultAsync(x => x.de_thi_id == sampleExamId && x.cau_hoi_id == questionId);
    }

    public async Task<int> GetMaxQuestionOrderAsync(long sampleExamId)
    {
        var max = await _context.de_thi_cau_hois
            .Where(x => x.de_thi_id == sampleExamId)
            .Select(x => (int?)x.thu_tu_cau)
            .MaxAsync();

        return max ?? 0;
    }

    public async Task AddAsync(de_thi sampleExam)
    {
        await _context.de_this.AddAsync(sampleExam);
    }

    public void Update(de_thi sampleExam)
    {
        _context.de_this.Update(sampleExam);
    }

    public void Remove(de_thi sampleExam)
    {
        _context.de_this.Remove(sampleExam);
    }

    public async Task AddAssignmentAsync(de_thi_cau_hoi assignment)
    {
        await _context.de_thi_cau_hois.AddAsync(assignment);
    }

    public void RemoveAssignment(de_thi_cau_hoi assignment)
    {
        _context.de_thi_cau_hois.Remove(assignment);
    }

    public void RemoveAssignments(IEnumerable<de_thi_cau_hoi> assignments)
    {
        _context.de_thi_cau_hois.RemoveRange(assignments);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
