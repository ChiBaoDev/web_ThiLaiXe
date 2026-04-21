using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.Data;
using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HeThongThiBangLai.Api.Repositories.Certificates;

public class CertificateRepository : ICertificateRepository
{
    private readonly ApplicationDbContext _context;

    public CertificateRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<certificates?> GetByIdAsync(long id)
    {
        return await _context.certificates.FirstOrDefaultAsync(x => x.id == id);
    }

    public async Task<certificates?> GetByCodeAsync(string code)
    {
        return await _context.certificates.FirstOrDefaultAsync(x => x.ma_chung_chi == code);
    }

    public async Task<certificates?> GetByExamResultIdAsync(long examResultId)
    {
        return await _context.certificates.FirstOrDefaultAsync(x => x.exam_result_id == examResultId);
    }

    public async Task<PagedList<certificates>> GetPagedAsync(int page, int pageSize, string? search = null, string? status = null)
    {
        var query = _context.certificates.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x => x.ma_chung_chi.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.trang_thai == status);
        }

        query = query.OrderByDescending(x => x.created_at);
        return await PagedList<certificates>.CreateAsync(query, page, pageSize);
    }

    public async Task AddAsync(certificates certificate)
    {
        await _context.certificates.AddAsync(certificate);
    }

    public void Update(certificates certificate)
    {
        _context.certificates.Update(certificate);
    }

    public async Task<exam_results?> GetExamResultByIdAsync(long id)
    {
        return await _context.exam_results.FirstOrDefaultAsync(x => x.id == id);
    }

    public async Task<bool> StudentExistsAsync(long studentId)
    {
        return await _context.hoc_viens.AnyAsync(x => x.id == studentId);
    }

    public async Task<bool> FileExistsAsync(long fileId)
    {
        return await _context.files.AnyAsync(x => x.id == fileId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
