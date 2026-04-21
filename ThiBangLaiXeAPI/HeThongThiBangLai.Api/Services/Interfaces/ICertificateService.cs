using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.Certificates;

namespace HeThongThiBangLai.Api.Services.Interfaces;

public interface ICertificateService
{
    Task<ApiResponse<PagedList<CertificateDto>>> GetListAsync(int page = 1, int pageSize = 20, string? search = null, string? status = null);
    Task<ApiResponse<CertificateDto>> GetByIdAsync(long id);
    Task<ApiResponse<CertificateDto>> VerifyByCodeAsync(string code);
    Task<ApiResponse<CertificateDto>> IssueAsync(IssueCertificateRequestDto request, long? createdBy = null);
    Task<ApiResponse<CertificateDto>> UpdateStatusAsync(long id, UpdateCertificateStatusRequestDto request);
    Task<ApiResponse<object>> ConfirmExamResultAsync(long examResultId, ConfirmExamResultRequestDto request, long? confirmedBy = null);
}
