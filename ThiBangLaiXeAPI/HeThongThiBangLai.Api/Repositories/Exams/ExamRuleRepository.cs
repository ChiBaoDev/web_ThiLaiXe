using HeThongThiBangLai.Api.Data;
using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeThongThiBangLai.Api.Repositories.Exams;

public class ExamRuleRepository : IExamRuleRepository
{
    private readonly ApplicationDbContext _context;

    public ExamRuleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<nhat_ky_he_thong>> GetRuleLogsAsync()
    {
        return await _context.nhat_ky_he_thongs
            .Where(x => x.bang_tac_dong == "exam_structure_rule")
            .OrderBy(x => x.created_at)
            .ThenBy(x => x.id)
            .ToListAsync();
    }

    public async Task<int> CountApprovedQuestionsByTopicAsync(long topicId)
    {
        return await _context.cau_hois.CountAsync(x => x.chu_de_id == topicId && x.trang_thai == "approved");
    }

    public async Task<int> CountApprovedQuestionsByDifficultyAsync(string difficulty)
    {
        return await _context.cau_hois.CountAsync(x => x.muc_do == difficulty && x.trang_thai == "approved");
    }

    public async Task<int> CountApprovedCriticalQuestionsAsync()
    {
        return await _context.cau_hois.CountAsync(x => x.la_cau_diem_liet && x.trang_thai == "approved");
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
