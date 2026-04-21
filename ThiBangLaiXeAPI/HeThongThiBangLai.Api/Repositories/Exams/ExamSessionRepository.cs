using HeThongThiBangLai.Api.Data;
using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeThongThiBangLai.Api.Repositories.Exams;

public class ExamSessionRepository : IExamSessionRepository
{
    private readonly ApplicationDbContext _context;

    public ExamSessionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<hoc_vien?> GetStudentByUserIdAsync(long userId)
    {
        return await _context.hoc_viens.FirstOrDefaultAsync(x => x.nguoi_dung_id == userId);
    }

    public async Task<de_thi?> GetPublishedSampleExamByIdAsync(long sampleExamId)
    {
        return await _context.de_this
            .Include(x => x.de_thi_cau_hois)
                .ThenInclude(x => x.cau_hoi)
                    .ThenInclude(x => x.dap_ans)
            .FirstOrDefaultAsync(x => x.id == sampleExamId && x.trang_thai == "published");
    }

    public async Task AddExamSessionAsync(bai_thi session)
    {
        await _context.bai_this.AddAsync(session);
    }

    public async Task AddExamSessionDetailsAsync(IEnumerable<chi_tiet_bai_thi> details)
    {
        await _context.chi_tiet_bai_this.AddRangeAsync(details);
    }

    public void UpdateExamSession(bai_thi session)
    {
        _context.bai_this.Update(session);
    }

    public void UpdateExamSessionDetail(chi_tiet_bai_thi detail)
    {
        _context.chi_tiet_bai_this.Update(detail);
    }

    public async Task<bai_thi?> GetSessionByIdForStudentAsync(long sessionId, long hocVienId)
    {
        return await _context.bai_this
            .Include(x => x.de_thi)
            .Include(x => x.chi_tiet_bai_this)
                .ThenInclude(x => x.cau_hoi)
                    .ThenInclude(x => x.dap_ans)
            .Include(x => x.chi_tiet_bai_this)
                .ThenInclude(x => x.dap_an_chon)
            .FirstOrDefaultAsync(x => x.id == sessionId && x.hoc_vien_id == hocVienId);
    }

    public async Task<List<chi_tiet_bai_thi>> GetSessionDetailsAsync(long sessionId)
    {
        return await _context.chi_tiet_bai_this
            .Include(x => x.cau_hoi)
                .ThenInclude(x => x.dap_ans)
            .Include(x => x.dap_an_chon)
            .Where(x => x.bai_thi_id == sessionId)
            .OrderBy(x => x.id)
            .ToListAsync();
    }

    public async Task AddSystemLogAsync(nhat_ky_he_thong log)
    {
        await _context.nhat_ky_he_thongs.AddAsync(log);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
