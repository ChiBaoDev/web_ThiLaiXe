# Enterprise Backend Audit — ThiBangLaiXe API

## 1) Phạm vi audit
- Tài liệu đối chiếu: `thibanglaixe-guides/thi-bang-lai-xe-api-be-doc.md`.
- Mã nguồn đối chiếu: toàn bộ backend trong `HeThongThiBangLai.Api` (controllers/services/repositories/DTOs/validators/bootstrap).

## 2) Kết luận tổng quan
Hệ thống backend hiện tại **không triển khai theo scope của tài liệu mock**. Thay vào đó, code đang chạy theo một scope khác, tập trung vào:
- Auth + hồ sơ người dùng hiện tại.
- Ngân hàng câu hỏi/chủ đề.
- Đề mẫu + phiên thi + chấm điểm.
- Wrong/Critical question practice.
- CMS, Files, Entitlements, Certificates, Dashboard, History.

=> Nếu coi tài liệu mock là chuẩn chính thức, mức độ lệch hiện tại là **cao** (khác domain, khác endpoint, khác flow nghiệp vụ).

## 3) Ma trận khớp/lệch/thiếu (rút gọn theo domain)

### A. Nhóm có triển khai (khớp một phần)
1. **Auth**
   - Có: register/login/logout/me/change-password/forgot/reset.
   - Lệch:
     - Prefix route dùng `/api/v1/*` thay vì `/api/*`.
     - `change-password` dùng `POST` (code) thay vì `PUT` (doc).
     - Thiếu `refresh-token`.

2. **Question topics & questions**
   - Có: CRUD chủ đề câu hỏi; CRUD câu hỏi + approve/archive.
   - Lệch:
     - Doc có nhánh `answers` tách riêng; code đang quản trị đáp án theo flow nội bộ service.

3. **Exam-related**
   - Có: đề mẫu (`/sample-exams`), quy tắc cấu trúc đề (`/exam-structure-rules`), phiên thi (`/exams/sessions/...`).
   - Lệch:
     - Doc mô tả `exam-shifts`, `exam-papers`, `test-attempts`, `exam-registrations` theo mô hình khác.
     - Code dùng mô hình đề mẫu + session state machine riêng.

4. **Dashboard/History**
   - Có endpoint dashboard + lịch sử thi user/admin.
   - Lệch với doc dashboard/reports theo role sâu hơn.

### B. Nhóm doc có nhưng code **chưa có**
- Users/Roles/System-logs API management đầy đủ.
- Students/Registration profiles/Documents.
- Courses/Classes/Course registrations/Sessions/Attendance.
- Fee types/Receipts/Violations/Reports (revenue, attendance, exam result report theo template doc).

### C. Nhóm code có nhưng doc mock **không phản ánh đầy đủ**
- CMS public/internal.
- Files + file usages.
- Entitlements.
- Certificates (issue/confirm/verify).
- Wrong questions/Critical questions workflows.
- Exam structure rule validate/activate lifecycle.

## 4) Điểm tốt nên giữ
1. **Layering rõ ràng**: Controller → Service → Repository.
2. **Chuẩn hóa response** với `ApiResponse<T>` + `ApiResponseFactory`.
3. **Global exception middleware** xử lý tập trung AppException và 500 fallback.
4. **Validation coverage tốt** qua FluentValidation (nhiều validator theo DTO).
5. **DI/Bootstrap rõ** trong Program, có policy auth định nghĩa sẵn.
6. **Tách public/private API** ở CMS/Certificates tương đối sạch.

## 5) Vấn đề cần sửa/refactor

### 5.1 Mức Critical
1. **Chốt lại source of truth** giữa doc và code.
   - Hiện trạng lệch lớn sẽ gây sai tích hợp FE/QA/UAT.
2. **Đồng nhất contract REST**
   - Một số endpoint trả `NotFound(result)` theo `result.Success`, số khác throw exception.
   - Cần thống nhất triết lý: service throw exception business hay service trả fail object.

### 5.2 Mức High
3. **Chuẩn hóa authorization**
   - Nhiều controller mới chỉ `[Authorize]` chung; policy/role chưa bọc đầy đủ thao tác nhạy cảm (tạo/sửa/xóa).
4. **Chuẩn hóa naming trạng thái/domain constants**
   - Tránh trộn status string rời rạc, chuyển sang constants/enums mapping rõ.
5. **Chuẩn hóa API versioning strategy**
   - Đã dùng `/api/v1`; cần khóa guideline và đồng bộ toàn tài liệu.

### 5.3 Mức Medium
6. **Giảm logic nghiệp vụ nặng trong service dài**
   - Tách domain services hoặc policy classes cho các flow lớn (exam session, auth reset password).
7. **Repository query optimization**
   - Rà soát `AsNoTracking` cho read-only queries, giảm tracking overhead.
8. **Audit log chuẩn hóa**
   - Một số module lưu snapshot JSON trong log; cần schema/audit-event convention rõ.

## 6) Kế hoạch cải tiến theo ưu tiên

### Quick wins (1–2 sprint)
1. Freeze API contract thực tế và phát hành OpenAPI/Swagger snapshot.
2. Cập nhật tài liệu chính thức theo `/api/v1` và endpoint hiện có.
3. Bổ sung policy cho các route ghi dữ liệu quan trọng.
4. Chuẩn hóa mapping mã lỗi business (errorCode) trong toàn hệ thống.

### Medium (2–4 sprint)
5. Refactor thống nhất error handling pattern service/repository.
6. Tách reusable component lấy userId từ token (tránh lặp trong nhiều controller).
7. Bổ sung integration tests cho auth/exam/certificates/entitlements.

### Strategic (4+ sprint)
8. Nếu cần theo đúng doc mock: lập chương trình gap implementation theo domain (students/courses/finance/reports...).
9. Nếu không theo doc mock: viết lại tài liệu nghiệp vụ/API chính thức và deprecate tài liệu cũ.

## 7) Quyết định kiến trúc khuyến nghị
Khuyến nghị chọn **một trong hai hướng** (không làm song song mơ hồ):
- **Hướng A (Theo code hiện tại):** Chuẩn hóa tài liệu theo implementation thật, đẩy nhanh tích hợp FE.
- **Hướng B (Theo mock doc):** Giữ mock doc làm target product và mở roadmap xây thêm các domain còn thiếu.

## 8) Đánh giá readiness hiện tại
- Cho các chức năng thi lý thuyết hiện có: mức **khá tốt để tiếp tục phát triển**.
- Cho scope ERP/trung tâm đào tạo đầy đủ như mock doc: mức **chưa sẵn sàng**, cần thêm nhiều module mới.
