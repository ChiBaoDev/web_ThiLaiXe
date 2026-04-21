using AutoMapper;
using HeThongThiBangLai.Api.Common.Exceptions;
using HeThongThiBangLai.Api.Common.Responses;
using HeThongThiBangLai.Api.DTOs.Certificates;
using HeThongThiBangLai.Api.Models;
using HeThongThiBangLai.Api.Repositories.Interfaces;
using HeThongThiBangLai.Api.Services.Interfaces;

namespace HeThongThiBangLai.Api.Services.Certificates;

public class CertificateService : ICertificateService
{
    private readonly ICertificateRepository _repository;
    private readonly IMapper _mapper;

    public CertificateService(ICertificateRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PagedList<CertificateDto>>> GetListAsync(int page = 1, int pageSize = 20, string? search = null, string? status = null)
    {
        var paged = await _repository.GetPagedAsync(page, pageSize, search, status);
        var dtos = _mapper.Map<List<CertificateDto>>(paged.Items);
        var result = new PagedList<CertificateDto>(dtos, paged.TotalCount, page, pageSize);

        return ApiResponseFactory.SuccessPaged(result, "Certificates retrieved successfully");
    }

    public async Task<ApiResponse<CertificateDto>> GetByIdAsync(long id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return ApiResponseFactory.Fail<CertificateDto>("Certificate not found");

        var dto = _mapper.Map<CertificateDto>(entity);
        return ApiResponseFactory.Success(dto, "Certificate retrieved successfully");
    }

    public async Task<ApiResponse<CertificateDto>> VerifyByCodeAsync(string code)
    {
        var entity = await _repository.GetByCodeAsync(code);
        if (entity == null)
            return ApiResponseFactory.Fail<CertificateDto>("Certificate not found");

        var dto = _mapper.Map<CertificateDto>(entity);
        return ApiResponseFactory.Success(dto, "Certificate verified successfully");
    }

    public async Task<ApiResponse<CertificateDto>> IssueAsync(IssueCertificateRequestDto request, long? createdBy = null)
    {
        var existingCode = await _repository.GetByCodeAsync(request.Code);
        if (existingCode != null)
        {
            throw new ConflictAppException("Certificate code already exists", "CERTIFICATE_CODE_EXISTS");
        }

        var studentExists = await _repository.StudentExistsAsync(request.StudentId);
        if (!studentExists)
        {
            throw new NotFoundAppException("Student not found");
        }

        var examResult = await _repository.GetExamResultByIdAsync(request.ExamResultId);
        if (examResult == null)
        {
            throw new NotFoundAppException("Exam result not found");
        }

        if (examResult.hoc_vien_id != request.StudentId)
        {
            throw new BusinessRuleAppException("Exam result does not belong to the provided student", "EXAM_RESULT_STUDENT_MISMATCH");
        }

        if (examResult.ket_qua != "dat")
        {
            throw new BusinessRuleAppException("Certificate can only be issued for passed exam result", "EXAM_RESULT_NOT_PASSED");
        }

        var existingByExamResult = await _repository.GetByExamResultIdAsync(request.ExamResultId);
        if (existingByExamResult != null)
        {
            throw new ConflictAppException("Certificate already issued for this exam result", "CERTIFICATE_ALREADY_ISSUED");
        }

        if (request.CertificateFileId.HasValue)
        {
            var fileExists = await _repository.FileExistsAsync(request.CertificateFileId.Value);
            if (!fileExists)
            {
                throw new NotFoundAppException("Certificate file not found");
            }
        }

        var entity = _mapper.Map<certificates>(request);
        entity.created_by = createdBy;
        entity.trang_thai = "valid";

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        var dto = _mapper.Map<CertificateDto>(entity);
        return ApiResponseFactory.Created(dto, "Certificate issued successfully");
    }

    public async Task<ApiResponse<CertificateDto>> UpdateStatusAsync(long id, UpdateCertificateStatusRequestDto request)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
        {
            throw new NotFoundAppException("Certificate not found");
        }

        entity.trang_thai = request.Status;
        entity.updated_at = DateTime.UtcNow;

        _repository.Update(entity);
        await _repository.SaveChangesAsync();

        var dto = _mapper.Map<CertificateDto>(entity);
        return ApiResponseFactory.Success(dto, "Certificate status updated successfully");
    }

    public async Task<ApiResponse<object>> ConfirmExamResultAsync(long examResultId, ConfirmExamResultRequestDto request, long? confirmedBy = null)
    {
        var examResult = await _repository.GetExamResultByIdAsync(examResultId);
        if (examResult == null)
        {
            throw new NotFoundAppException("Exam result not found");
        }

        examResult.ket_qua = request.Result;
        examResult.xac_nhan_boi = confirmedBy;
        examResult.xac_nhan_luc = DateTime.UtcNow;
        examResult.updated_at = DateTime.UtcNow;

        await _repository.SaveChangesAsync();

        return ApiResponseFactory.Success<object>(new { examResultId = examResult.id, result = examResult.ket_qua }, "Exam result confirmed successfully");
    }
}
