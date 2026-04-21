using AutoMapper;
using HeThongThiBangLai.Api.DTOs.Auth;
using HeThongThiBangLai.Api.DTOs.CriticalQuestions;
using HeThongThiBangLai.Api.DTOs.Cms;
using HeThongThiBangLai.Api.DTOs.Exams;
using HeThongThiBangLai.Api.DTOs.Files;
using HeThongThiBangLai.Api.DTOs.Entitlements;
using HeThongThiBangLai.Api.DTOs.Certificates;
using HeThongThiBangLai.Api.DTOs.Questions;
using HeThongThiBangLai.Api.DTOs.Topics;
using HeThongThiBangLai.Api.Models;

namespace HeThongThiBangLai.Api.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Auth mappings
        CreateMap<RegisterRequestDto, nguoi_dung>()
            .ForMember(dest => dest.mat_khau_hash, opt => opt.Ignore())
            .ForMember(dest => dest.trang_thai, opt => opt.MapFrom(src => "Active"));

        CreateMap<nguoi_dung, MeResponseDto>();

        // Question mappings
        CreateMap<cau_hoi, QuestionDto>()
            .ForMember(dest => dest.TopicId, opt => opt.MapFrom(src => src.chu_de_id))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.noi_dung))
            .ForMember(dest => dest.QuestionType, opt => opt.MapFrom(src => src.loai_cau_hoi))
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.muc_do))
            .ForMember(dest => dest.IsCritical, opt => opt.MapFrom(src => src.la_cau_diem_liet))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.trang_thai));

        CreateMap<CreateQuestionRequestDto, cau_hoi>()
            .ForMember(dest => dest.chu_de_id, opt => opt.MapFrom(src => src.TopicId))
            .ForMember(dest => dest.noi_dung, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.loai_cau_hoi, opt => opt.MapFrom(src => src.QuestionType))
            .ForMember(dest => dest.muc_do, opt => opt.MapFrom(src => src.Level))
            .ForMember(dest => dest.la_cau_diem_liet, opt => opt.MapFrom(src => src.IsCritical))
            .ForMember(dest => dest.trang_thai, opt => opt.MapFrom(src => "draft"));

        CreateMap<UpdateQuestionRequestDto, cau_hoi>()
            .ForMember(dest => dest.chu_de_id, opt => opt.MapFrom(src => src.TopicId))
            .ForMember(dest => dest.noi_dung, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.loai_cau_hoi, opt => opt.MapFrom(src => src.QuestionType))
            .ForMember(dest => dest.muc_do, opt => opt.MapFrom(src => src.Level))
            .ForMember(dest => dest.la_cau_diem_liet, opt => opt.MapFrom(src => src.IsCritical));

        CreateMap<cau_hoi, QuestionListResponseDto>()
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.noi_dung))
            .ForMember(dest => dest.QuestionType, opt => opt.MapFrom(src => src.loai_cau_hoi))
            .ForMember(dest => dest.IsCritical, opt => opt.MapFrom(src => src.la_cau_diem_liet))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.trang_thai));

        // Critical question mappings
        CreateMap<cau_hoi, CriticalQuestionDto>()
            .ForMember(dest => dest.TopicId, opt => opt.MapFrom(src => src.chu_de_id))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.noi_dung))
            .ForMember(dest => dest.QuestionType, opt => opt.MapFrom(src => src.loai_cau_hoi))
            .ForMember(dest => dest.Level, opt => opt.MapFrom(src => src.muc_do));

        // Topic mappings
        CreateMap<chu_de_cau_hoi, TopicDto>()
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.ma_chu_de))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ten_chu_de))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.mo_ta))
            .ForMember(dest => dest.QuestionCount, opt => opt.MapFrom(src => src.cau_hois.Count));

        CreateMap<CreateTopicRequestDto, chu_de_cau_hoi>()
            .ForMember(dest => dest.ma_chu_de, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.ten_chu_de, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.mo_ta, opt => opt.MapFrom(src => src.Description));

        CreateMap<UpdateTopicRequestDto, chu_de_cau_hoi>()
            .ForMember(dest => dest.ma_chu_de, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.ten_chu_de, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.mo_ta, opt => opt.MapFrom(src => src.Description));

        // File mappings
        CreateMap<files, FileDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.StorageProvider, opt => opt.MapFrom(src => src.storage_provider))
            .ForMember(dest => dest.BucketName, opt => opt.MapFrom(src => src.bucket_name))
            .ForMember(dest => dest.ObjectKey, opt => opt.MapFrom(src => src.object_key))
            .ForMember(dest => dest.PublicUrl, opt => opt.MapFrom(src => src.public_url))
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.file_name))
            .ForMember(dest => dest.MimeType, opt => opt.MapFrom(src => src.mime_type))
            .ForMember(dest => dest.SizeBytes, opt => opt.MapFrom(src => src.size_bytes))
            .ForMember(dest => dest.ChecksumSha256, opt => opt.MapFrom(src => src.checksum_sha256))
            .ForMember(dest => dest.Width, opt => opt.MapFrom(src => src.width))
            .ForMember(dest => dest.Height, opt => opt.MapFrom(src => src.height))
            .ForMember(dest => dest.DurationSeconds, opt => opt.MapFrom(src => src.duration_seconds))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.trang_thai))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.created_by))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.created_at))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.updated_at));

        CreateMap<files, FileListResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.file_name))
            .ForMember(dest => dest.MimeType, opt => opt.MapFrom(src => src.mime_type))
            .ForMember(dest => dest.SizeBytes, opt => opt.MapFrom(src => src.size_bytes))
            .ForMember(dest => dest.StorageProvider, opt => opt.MapFrom(src => src.storage_provider))
            .ForMember(dest => dest.PublicUrl, opt => opt.MapFrom(src => src.public_url))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.trang_thai))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.created_at));

        CreateMap<CreateFileRequestDto, files>()
            .ForMember(dest => dest.storage_provider, opt => opt.MapFrom(src => src.StorageProvider))
            .ForMember(dest => dest.bucket_name, opt => opt.MapFrom(src => src.BucketName))
            .ForMember(dest => dest.object_key, opt => opt.MapFrom(src => src.ObjectKey))
            .ForMember(dest => dest.public_url, opt => opt.MapFrom(src => src.PublicUrl))
            .ForMember(dest => dest.file_name, opt => opt.MapFrom(src => src.FileName))
            .ForMember(dest => dest.mime_type, opt => opt.MapFrom(src => src.MimeType))
            .ForMember(dest => dest.size_bytes, opt => opt.MapFrom(src => src.SizeBytes))
            .ForMember(dest => dest.checksum_sha256, opt => opt.MapFrom(src => src.ChecksumSha256))
            .ForMember(dest => dest.width, opt => opt.MapFrom(src => src.Width))
            .ForMember(dest => dest.height, opt => opt.MapFrom(src => src.Height))
            .ForMember(dest => dest.duration_seconds, opt => opt.MapFrom(src => src.DurationSeconds));

        CreateMap<file_usages, FileUsageDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.FileId, opt => opt.MapFrom(src => src.file_id))
            .ForMember(dest => dest.EntityName, opt => opt.MapFrom(src => src.entity_name))
            .ForMember(dest => dest.EntityId, opt => opt.MapFrom(src => src.entity_id))
            .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.field_name))
            .ForMember(dest => dest.IsPrimary, opt => opt.MapFrom(src => src.is_primary))
            .ForMember(dest => dest.SortOrder, opt => opt.MapFrom(src => src.sort_order))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.created_at));

        // CMS mappings
        CreateMap<categories, CategoryDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.parent_id))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.ma_danh_muc))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ten_danh_muc))
            .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.slug))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.mo_ta))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.is_active))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.created_by))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.created_at))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.updated_at));

        CreateMap<CreateCategoryRequestDto, categories>()
            .ForMember(dest => dest.parent_id, opt => opt.MapFrom(src => src.ParentId))
            .ForMember(dest => dest.ma_danh_muc, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.ten_danh_muc, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.slug, opt => opt.MapFrom(src => src.Slug))
            .ForMember(dest => dest.mo_ta, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.is_active, opt => opt.MapFrom(src => src.IsActive));

        CreateMap<UpdateCategoryRequestDto, categories>()
            .ForMember(dest => dest.parent_id, opt => opt.MapFrom(src => src.ParentId))
            .ForMember(dest => dest.ma_danh_muc, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.ten_danh_muc, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.slug, opt => opt.MapFrom(src => src.Slug))
            .ForMember(dest => dest.mo_ta, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.is_active, opt => opt.MapFrom(src => src.IsActive));

        CreateMap<posts, PostListResponseDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.ma_bai_viet))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.title))
            .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.slug))
            .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.summary))
            .ForMember(dest => dest.PostType, opt => opt.MapFrom(src => src.post_type))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.trang_thai))
            .ForMember(dest => dest.PublishedAt, opt => opt.MapFrom(src => src.published_at))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.created_at));

        CreateMap<posts, PostDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.ma_bai_viet))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.title))
            .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.slug))
            .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.summary))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.content))
            .ForMember(dest => dest.PostType, opt => opt.MapFrom(src => src.post_type))
            .ForMember(dest => dest.ThumbnailFileId, opt => opt.MapFrom(src => src.thumbnail_file_id))
            .ForMember(dest => dest.MetaTitle, opt => opt.MapFrom(src => src.meta_title))
            .ForMember(dest => dest.MetaDescription, opt => opt.MapFrom(src => src.meta_description))
            .ForMember(dest => dest.CanonicalUrl, opt => opt.MapFrom(src => src.canonical_url))
            .ForMember(dest => dest.PublishedAt, opt => opt.MapFrom(src => src.published_at))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.trang_thai))
            .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.author_id))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.created_at))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.updated_at))
            .ForMember(dest => dest.CategoryIds, opt => opt.MapFrom(src => src.post_categories.Select(pc => pc.category_id).ToList()));

        CreateMap<CreatePostRequestDto, posts>()
            .ForMember(dest => dest.ma_bai_viet, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.slug, opt => opt.MapFrom(src => src.Slug))
            .ForMember(dest => dest.summary, opt => opt.MapFrom(src => src.Summary))
            .ForMember(dest => dest.content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.post_type, opt => opt.MapFrom(src => src.PostType))
            .ForMember(dest => dest.thumbnail_file_id, opt => opt.MapFrom(src => src.ThumbnailFileId))
            .ForMember(dest => dest.meta_title, opt => opt.MapFrom(src => src.MetaTitle))
            .ForMember(dest => dest.meta_description, opt => opt.MapFrom(src => src.MetaDescription))
            .ForMember(dest => dest.canonical_url, opt => opt.MapFrom(src => src.CanonicalUrl))
            .ForMember(dest => dest.published_at, opt => opt.MapFrom(src => src.PublishedAt))
            .ForMember(dest => dest.trang_thai, opt => opt.MapFrom(src => src.Status));

        CreateMap<UpdatePostRequestDto, posts>()
            .ForMember(dest => dest.ma_bai_viet, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.slug, opt => opt.MapFrom(src => src.Slug))
            .ForMember(dest => dest.summary, opt => opt.MapFrom(src => src.Summary))
            .ForMember(dest => dest.content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.post_type, opt => opt.MapFrom(src => src.PostType))
            .ForMember(dest => dest.thumbnail_file_id, opt => opt.MapFrom(src => src.ThumbnailFileId))
            .ForMember(dest => dest.meta_title, opt => opt.MapFrom(src => src.MetaTitle))
            .ForMember(dest => dest.meta_description, opt => opt.MapFrom(src => src.MetaDescription))
            .ForMember(dest => dest.canonical_url, opt => opt.MapFrom(src => src.CanonicalUrl))
            .ForMember(dest => dest.published_at, opt => opt.MapFrom(src => src.PublishedAt))
            .ForMember(dest => dest.trang_thai, opt => opt.MapFrom(src => src.Status));

        // Entitlement mappings
        CreateMap<goi_quyen, EntitlementPackageDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.ma_goi))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ten_goi))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.mo_ta))
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.is_active))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.created_at))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.updated_at));

        CreateMap<CreateEntitlementPackageRequestDto, goi_quyen>()
            .ForMember(dest => dest.ma_goi, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.ten_goi, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.mo_ta, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.is_active, opt => opt.MapFrom(src => src.IsActive));

        CreateMap<UpdateEntitlementPackageRequestDto, goi_quyen>()
            .ForMember(dest => dest.ma_goi, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.ten_goi, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.mo_ta, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.is_active, opt => opt.MapFrom(src => src.IsActive));

        CreateMap<quyen_su_dung, UserEntitlementDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.nguoi_dung_id))
            .ForMember(dest => dest.PackageId, opt => opt.MapFrom(src => src.goi_quyen_id))
            .ForMember(dest => dest.EffectiveFrom, opt => opt.MapFrom(src => src.ngay_hieu_luc))
            .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => src.ngay_het_han))
            .ForMember(dest => dest.Source, opt => opt.MapFrom(src => src.nguon_cap))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.trang_thai))
            .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.ghi_chu))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.created_by))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.created_at))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.updated_at));

        CreateMap<GrantUserEntitlementRequestDto, quyen_su_dung>()
            .ForMember(dest => dest.nguoi_dung_id, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.goi_quyen_id, opt => opt.MapFrom(src => src.PackageId))
            .ForMember(dest => dest.ngay_hieu_luc, opt => opt.MapFrom(src => src.EffectiveFrom))
            .ForMember(dest => dest.ngay_het_han, opt => opt.MapFrom(src => src.ExpiresAt))
            .ForMember(dest => dest.nguon_cap, opt => opt.MapFrom(src => src.Source))
            .ForMember(dest => dest.ghi_chu, opt => opt.MapFrom(src => src.Note));

        // Certificate mappings
        CreateMap<certificates, CertificateDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.id))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.ma_chung_chi))
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.hoc_vien_id))
            .ForMember(dest => dest.ExamResultId, opt => opt.MapFrom(src => src.exam_result_id))
            .ForMember(dest => dest.IssuedAt, opt => opt.MapFrom(src => src.ngay_cap))
            .ForMember(dest => dest.ExpiresAt, opt => opt.MapFrom(src => src.ngay_het_han))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.trang_thai))
            .ForMember(dest => dest.CertificateFileId, opt => opt.MapFrom(src => src.certificate_file_id))
            .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.created_by))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.created_at))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.updated_at));

        CreateMap<IssueCertificateRequestDto, certificates>()
            .ForMember(dest => dest.ma_chung_chi, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.hoc_vien_id, opt => opt.MapFrom(src => src.StudentId))
            .ForMember(dest => dest.exam_result_id, opt => opt.MapFrom(src => src.ExamResultId))
            .ForMember(dest => dest.ngay_cap, opt => opt.MapFrom(src => src.IssuedAt))
            .ForMember(dest => dest.ngay_het_han, opt => opt.MapFrom(src => src.ExpiresAt))
            .ForMember(dest => dest.certificate_file_id, opt => opt.MapFrom(src => src.CertificateFileId));

        // Sample exam mappings
        CreateMap<de_thi, SampleExamDto>()
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.ma_de_thi))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ten_de_thi))
            .ForMember(dest => dest.ExamPeriodId, opt => opt.MapFrom(src => src.ky_thi_id))
            .ForMember(dest => dest.TotalQuestions, opt => opt.MapFrom(src => src.tong_so_cau))
            .ForMember(dest => dest.DurationMinutes, opt => opt.MapFrom(src => src.thoi_gian_lam_bai))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.trang_thai))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.ngay_tao))
            .ForMember(dest => dest.LinkedQuestionCount, opt => opt.MapFrom(src => src.de_thi_cau_hois.Count))
            .ForMember(dest => dest.QuestionIds, opt => opt.MapFrom(src => src.de_thi_cau_hois.Select(x => x.cau_hoi_id).ToList()));

        CreateMap<CreateSampleExamRequestDto, de_thi>()
            .ForMember(dest => dest.ma_de_thi, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.ten_de_thi, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.ky_thi_id, opt => opt.MapFrom(src => src.ExamPeriodId))
            .ForMember(dest => dest.tong_so_cau, opt => opt.MapFrom(src => src.TotalQuestions))
            .ForMember(dest => dest.thoi_gian_lam_bai, opt => opt.MapFrom(src => src.DurationMinutes));

        CreateMap<UpdateSampleExamRequestDto, de_thi>()
            .ForMember(dest => dest.ma_de_thi, opt => opt.MapFrom(src => src.Code))
            .ForMember(dest => dest.ten_de_thi, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.ky_thi_id, opt => opt.MapFrom(src => src.ExamPeriodId))
            .ForMember(dest => dest.tong_so_cau, opt => opt.MapFrom(src => src.TotalQuestions))
            .ForMember(dest => dest.thoi_gian_lam_bai, opt => opt.MapFrom(src => src.DurationMinutes));
    }
}
