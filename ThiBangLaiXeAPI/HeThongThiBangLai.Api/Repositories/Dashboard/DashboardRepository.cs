using HeThongThiBangLai.Api.Data;
using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeThongThiBangLai.Api.Repositories.Dashboard;

public class DashboardRepository : IDashboardRepository
{
    private readonly ApplicationDbContext _context;

    public DashboardRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetTotalCandidatesAsync()
    {
        return await _context.hoc_viens.CountAsync();
    }

    public async Task<List<bai_thi>> GetExamSessionsAsync(DateTime? from = null, DateTime? to = null)
    {
        var query = _context.bai_this.AsQueryable();
        query = ApplyDateFilter(query, from, to);

        return await query.ToListAsync();
    }

    public async Task<List<chi_tiet_bai_thi>> GetExamDetailsAsync(DateTime? from = null, DateTime? to = null)
    {
        var query = _context.chi_tiet_bai_this
            .Include(x => x.bai_thi)
            .Include(x => x.cau_hoi)
            .AsQueryable();

        if (from.HasValue)
        {
            query = query.Where(x => x.bai_thi.thoi_gian_nop.HasValue && x.bai_thi.thoi_gian_nop.Value >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.bai_thi.thoi_gian_nop.HasValue && x.bai_thi.thoi_gian_nop.Value <= to.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<List<TopicWeakStat>> GetWeakTopicStatsAsync(DateTime? from = null, DateTime? to = null)
    {
        var query = _context.chi_tiet_bai_this
            .Include(x => x.bai_thi)
            .Include(x => x.cau_hoi)
                .ThenInclude(q => q.chu_de)
            .AsQueryable();

        if (from.HasValue)
        {
            query = query.Where(x => x.bai_thi.thoi_gian_nop.HasValue && x.bai_thi.thoi_gian_nop.Value >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.bai_thi.thoi_gian_nop.HasValue && x.bai_thi.thoi_gian_nop.Value <= to.Value);
        }

        return await query
            .GroupBy(x => new { x.cau_hoi.chu_de_id, x.cau_hoi.chu_de.ten_chu_de })
            .Select(g => new TopicWeakStat
            {
                TopicId = g.Key.chu_de_id,
                TopicName = g.Key.ten_chu_de,
                TotalAnswered = g.Count(x => x.la_dung.HasValue),
                WrongCount = g.Count(x => x.la_dung == false)
            })
            .ToListAsync();
    }

    public async Task<List<QuestionWrongStat>> GetTopWrongQuestionsAsync(DateTime? from = null, DateTime? to = null, int take = 10, bool criticalOnly = false)
    {
        var query = _context.chi_tiet_bai_this
            .Include(x => x.bai_thi)
            .Include(x => x.cau_hoi)
            .AsQueryable();

        if (from.HasValue)
        {
            query = query.Where(x => x.bai_thi.thoi_gian_nop.HasValue && x.bai_thi.thoi_gian_nop.Value >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.bai_thi.thoi_gian_nop.HasValue && x.bai_thi.thoi_gian_nop.Value <= to.Value);
        }

        if (criticalOnly)
        {
            query = query.Where(x => x.cau_hoi.la_cau_diem_liet);
        }

        return await query
            .Where(x => x.la_dung == false)
            .GroupBy(x => new { x.cau_hoi_id, x.cau_hoi.noi_dung })
            .Select(g => new QuestionWrongStat
            {
                QuestionId = g.Key.cau_hoi_id,
                QuestionContent = g.Key.noi_dung,
                WrongCount = g.Count()
            })
            .OrderByDescending(x => x.WrongCount)
            .Take(take)
            .ToListAsync();
    }

    private static IQueryable<bai_thi> ApplyDateFilter(IQueryable<bai_thi> query, DateTime? from, DateTime? to)
    {
        if (from.HasValue)
        {
            query = query.Where(x => x.thoi_gian_nop.HasValue && x.thoi_gian_nop.Value >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.thoi_gian_nop.HasValue && x.thoi_gian_nop.Value <= to.Value);
        }

        return query;
    }
}
