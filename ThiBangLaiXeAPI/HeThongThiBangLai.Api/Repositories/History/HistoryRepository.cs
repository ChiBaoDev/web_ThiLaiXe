using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.Data;
using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeThongThiBangLai.Api.Repositories.History;

public class HistoryRepository : IHistoryRepository
{
    private readonly ApplicationDbContext _context;

    public HistoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<hoc_vien?> GetStudentByUserIdAsync(long userId)
    {
        return await _context.hoc_viens.FirstOrDefaultAsync(x => x.nguoi_dung_id == userId);
    }

    public async Task<bai_thi?> GetExamByIdAsync(long sessionId)
    {
        return await _context.bai_this
            .Include(x => x.de_thi)
            .FirstOrDefaultAsync(x => x.id == sessionId);
    }

    public async Task<bai_thi?> GetExamByIdForStudentAsync(long sessionId, long hocVienId)
    {
        return await _context.bai_this
            .Include(x => x.de_thi)
            .FirstOrDefaultAsync(x => x.id == sessionId && x.hoc_vien_id == hocVienId);
    }

    public async Task<PagedList<bai_thi>> GetExamListForStudentAsync(long hocVienId, int page, int pageSize, DateTime? from = null, DateTime? to = null, string? result = null)
    {
        var query = _context.bai_this
            .Include(x => x.de_thi)
            .Where(x => x.hoc_vien_id == hocVienId)
            .AsQueryable();

        query = ApplyFilter(query, from, to, result);

        return await PagedList<bai_thi>.CreateAsync(query.OrderByDescending(x => x.id), page, pageSize);
    }

    public async Task<PagedList<bai_thi>> GetExamListForAdminAsync(int page, int pageSize, DateTime? from = null, DateTime? to = null, string? result = null)
    {
        var query = _context.bai_this
            .Include(x => x.de_thi)
            .AsQueryable();

        query = ApplyFilter(query, from, to, result);

        return await PagedList<bai_thi>.CreateAsync(query.OrderByDescending(x => x.id), page, pageSize);
    }

    public async Task<PagedList<bai_thi>> GetExamListByStudentIdForAdminAsync(long hocVienId, int page, int pageSize, DateTime? from = null, DateTime? to = null, string? result = null)
    {
        var query = _context.bai_this
            .Include(x => x.de_thi)
            .Where(x => x.hoc_vien_id == hocVienId)
            .AsQueryable();

        query = ApplyFilter(query, from, to, result);

        return await PagedList<bai_thi>.CreateAsync(query.OrderByDescending(x => x.id), page, pageSize);
    }

    private static IQueryable<bai_thi> ApplyFilter(IQueryable<bai_thi> query, DateTime? from, DateTime? to, string? result)
    {
        if (from.HasValue)
        {
            query = query.Where(x => x.thoi_gian_bat_dau.HasValue && x.thoi_gian_bat_dau.Value >= from.Value);
        }

        if (to.HasValue)
        {
            query = query.Where(x => x.thoi_gian_bat_dau.HasValue && x.thoi_gian_bat_dau.Value <= to.Value);
        }

        if (!string.IsNullOrWhiteSpace(result))
        {
            query = query.Where(x => x.ket_qua != null && x.ket_qua == result);
        }

        return query;
    }
}
