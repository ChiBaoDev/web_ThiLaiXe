using HeThongThiBangLai.Api.Data;
using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeThongThiBangLai.Api.Repositories.WrongQuestions;

public class WrongQuestionRepository : IWrongQuestionRepository
{
    private readonly ApplicationDbContext _context;

    public WrongQuestionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<hoc_vien?> GetStudentByUserIdAsync(long userId)
    {
        return await _context.hoc_viens.FirstOrDefaultAsync(x => x.nguoi_dung_id == userId);
    }

    public async Task<List<WrongQuestionStat>> GetWrongQuestionStatsAsync(long hocVienId)
    {
        return await _context.chi_tiet_bai_this
            .Where(x => x.bai_thi.hoc_vien_id == hocVienId && x.la_dung == false && x.bai_thi.thoi_gian_nop.HasValue)
            .GroupBy(x => x.cau_hoi_id)
            .Select(g => new WrongQuestionStat
            {
                QuestionId = g.Key,
                WrongCount = g.Count(),
                LastWrongAt = g.Max(x => x.bai_thi.thoi_gian_nop ?? x.bai_thi.thoi_gian_bat_dau)
            })
            .ToListAsync();
    }

    public async Task<List<cau_hoi>> GetQuestionsByIdsAsync(IEnumerable<long> questionIds)
    {
        var ids = questionIds.Distinct().ToList();
        if (ids.Count == 0)
        {
            return new List<cau_hoi>();
        }

        return await _context.cau_hois
            .Where(x => ids.Contains(x.id) && x.trang_thai == "approved")
            .ToListAsync();
    }

    public async Task<HashSet<long>> GetHandledQuestionIdsAsync(long userId)
    {
        var handledIds = await _context.nhat_ky_he_thongs
            .Where(x => x.nguoi_dung_id == userId
                && x.bang_tac_dong == "wrong_question"
                && (x.hanh_dong == "wrong_question_resolved" || x.hanh_dong == "wrong_question_removed")
                && x.khoa_chinh_du_lieu.HasValue)
            .Select(x => x.khoa_chinh_du_lieu!.Value)
            .ToListAsync();

        return handledIds.ToHashSet();
    }

    public async Task<int> GetWrongPracticeSessionCountAsync(long hocVienId)
    {
        return await _context.phien_on_taps
            .CountAsync(x => x.hoc_vien_id == hocVienId && x.trang_thai == "wrong_practice_started");
    }

    public async Task<DateTime?> GetLatestWrongPracticeAtAsync(long hocVienId)
    {
        return await _context.phien_on_taps
            .Where(x => x.hoc_vien_id == hocVienId && x.trang_thai == "wrong_practice_started")
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

    public async Task AddSystemLogAsync(nhat_ky_he_thong log)
    {
        await _context.nhat_ky_he_thongs.AddAsync(log);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
