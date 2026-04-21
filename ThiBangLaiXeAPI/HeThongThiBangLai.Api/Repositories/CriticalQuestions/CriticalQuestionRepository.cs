using HeThongThiBangLai.Api.Data;
using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeThongThiBangLai.Api.Repositories.CriticalQuestions;

public class CriticalQuestionRepository : ICriticalQuestionRepository
{
    private readonly ApplicationDbContext _context;

    public CriticalQuestionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<cau_hoi>> GetCriticalQuestionsAsync()
    {
        return await _context.cau_hois
            .Where(x => x.la_cau_diem_liet && x.trang_thai == "approved")
            .ToListAsync();
    }

    public async Task<hoc_vien?> GetStudentByUserIdAsync(long userId)
    {
        return await _context.hoc_viens.FirstOrDefaultAsync(x => x.nguoi_dung_id == userId);
    }

    public async Task<int> GetCriticalPracticeSessionCountAsync(long hocVienId)
    {
        return await _context.phien_on_taps
            .CountAsync(x => x.hoc_vien_id == hocVienId && x.trang_thai == "critical_practice_started");
    }

    public async Task<DateTime?> GetLatestCriticalPracticeAtAsync(long hocVienId)
    {
        return await _context.phien_on_taps
            .Where(x => x.hoc_vien_id == hocVienId && x.trang_thai == "critical_practice_started")
            .OrderByDescending(x => x.ngay_tao)
            .Select(x => (DateTime?)x.ngay_tao)
            .FirstOrDefaultAsync();
    }

    public async Task AddPracticeSessionAsync(phien_on_tap session)
    {
        await _context.phien_on_taps.AddAsync(session);
    }

    public async Task AddPracticeSessionQuestionsAsync(IEnumerable<phien_on_tap_cau_hoi> questions)
    {
        await _context.phien_on_tap_cau_hois.AddRangeAsync(questions);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
