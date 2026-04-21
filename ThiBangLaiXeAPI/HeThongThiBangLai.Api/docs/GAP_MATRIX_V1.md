# GAP Matrix v1 - HeThongThiBangLai.Api

## 1) Nguồn đối chiếu
- Master plan: `thibanglaixe-guides/THIBANGLAIXE_MASTER_PLAN.md`
- API conventions: `thibanglaixe-guides/API_CREATION_RULES.md`, `thibanglaixe-guides/API_RESPONSE_GUIDE.md`
- Domain rules: `thibanglaixe-guides/EXAM_DOMAIN_RULES.md`

## 2) Hiện trạng codebase (tóm tắt)
- Có nền tảng `.NET 8 + EF Core + JWT + Swagger + FluentValidation + AutoMapper`.
- API hiện có thực chất: `Auth`, `Topics`, `Questions`.
- Có `GlobalExceptionMiddleware`, `ApiResponse<T>`, `ApiResponseFactory`.
- DB-first map đầy đủ nhiều nhóm bảng (bao gồm cả nhóm ngoài phạm vi exam v1).

## 3) DB-first alignment (SQL ↔ DbContext)
### 3.1 Điểm đạt
- `ApplicationDbContext` map tương đối đầy đủ các bảng trong `new_database_moto_lise.sql`.
- Ràng buộc chính (PK/FK/unique/default) được phản ánh ở mức cấu hình EF.

### 3.2 Lệch phạm vi domain v1
Theo master plan, v1 tập trung exam-learning domain. Tuy nhiên schema hiện có trộn thêm nhóm vận hành trung tâm:
- Nhóm ngoài v1: `khoa_hoc`, `lop_hoc`, `buoi_hoc`, `diem_danh`, `phieu_thu`, `chi_tiet_phieu_thu`, `giay_to_dinh_kem`, `ho_so_dang_ky`, `dang_ky_khoa_hoc`...
- Rủi ro: tăng độ phức tạp API surface, tăng coupling, khó hoàn thành đúng tiến độ v1.

## 4) GAP theo tiêu chuẩn kỹ thuật

| Hạng mục | Chuẩn mục tiêu | Hiện trạng | GAP | Mức độ |
|---|---|---|---|---|
| API versioning | `/api/v1/...` | Route đang là `/api/...` | Thiếu version prefix toàn cục | High |
| Auth endpoints | Bộ Auth đầy đủ + nhất quán chuẩn route v1 | Có đủ flow chính, nhưng route chưa v1 | Chuẩn hoá route + swagger tag | Medium |
| Unified response | Tất cả endpoint trả `ApiResponse<T>` nhất quán | Có dùng `ApiResponse`, nhưng cách dùng chưa thống nhất tuyệt đối | Cần chuẩn hoá controller trả về + status code mapping | High |
| Exception model | Exception có status code/business code rõ | `AppException` chưa có status code; middleware hardcode 400 cho `AppException` | Cần mở rộng exception hierarchy (`NotFound`, `Forbidden`, `Conflict`, `Business`) | High |
| Security config hygiene | Không commit secret DB/JWT | `appsettings.json` đang chứa DB password + JWT secret | Bắt buộc tách secret khỏi tracked config | Critical |
| Swagger conventions | Tag/module, response mẫu, mã lỗi chuẩn | Mới có cấu hình JWT cơ bản | Thiếu tài liệu hoá response lỗi + grouping theo module | Medium |
| Domain scope control | Khóa phạm vi exam-v1 | DB model trộn domain | Cần quyết định: out-of-scope/để phase sau | High |
| Core modules v1 | Topics/Questions/Regulations/Traffic Signs/Exam Rules/Exam Sessions/... | Mới có Auth + Topics + Questions (một phần CRUD) | Thiếu nhiều module trọng yếu | Critical |
| Audit/API logs | Truy vấn/khai thác log quản trị | Có bảng `nhat_ky_he_thong`, chưa có API audit riêng | Thiếu module audit | Medium |
| Tests | Unit + integration + smoke | Chưa thấy test project | Thiếu lớp test nền tảng | High |

## 5) GAP theo module API blueprint (master plan)

| Module | Trạng thái |
|---|---|
| Auth & Profile | Partial (đã có major flows, chưa v1 route, chưa refresh token) |
| Roles/Users | Missing |
| Exam Regulations | Missing |
| Question Topics | Partial (GET/GET by id/POST) |
| Question Bank | Partial (GET/GET by id/POST) |
| Traffic Signs | Missing |
| Sample Exams | Missing |
| Exam Structure Rules | Missing |
| Exam Sessions/Runner | Missing |
| Wrong Questions | Missing |
| Critical Practice | Missing |
| History/Analytics (candidate/admin) | Missing |
| Dashboard/Statistics | Missing |
| Settings | Missing |
| Audit Logs API | Missing |

## 6) Backlog ưu tiên triển khai (đề xuất)
1. **Foundation hardening (bắt buộc trước):**
   - Chuẩn hóa route `/api/v1`.
   - Chuẩn hóa exception model + middleware status code.
   - Dọn secret khỏi `appsettings.json`.
   - Swagger conventions + response docs cơ bản.
2. **Content core:**
   - Hoàn thiện Topics CRUD + status lifecycle.
   - Hoàn thiện Questions CRUD + approve/archive + filter.
3. **Exam core:**
   - Exam structure rules.
   - Sample exams.
   - Exam session start/answer/submit/result.
4. **Learning support:**
   - Wrong questions, critical practice, history.
5. **Admin:**
   - Dashboard, settings, audit log APIs.

## 7) Phạm vi v1 khóa cứng (khuyến nghị)
- In-scope: Auth, Topics, Questions, Regulations, Traffic Signs, Exam Rules, Sample Exams, Exam Sessions, Wrong/Critical Practice, History, Dashboard cơ bản, Audit.
- Out-of-scope v1: đào tạo lớp học/điểm danh/tài chính/hồ sơ hành chính.

## 8) Hành động kế tiếp ngay
- Refactor nền tảng theo mục 1 (route versioning + exception/status + config hygiene + swagger).
- Sau đó mới mở rộng module theo delivery order của master plan.
