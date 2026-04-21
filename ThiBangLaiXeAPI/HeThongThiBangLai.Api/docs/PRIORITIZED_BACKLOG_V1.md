# Prioritized Backlog v1 - HeThongThiBangLai.Api

## 1. Nguyên tắc khóa phạm vi v1

### In-scope v1 (bắt buộc)
1. Auth & Profile
2. Question Topics
3. Question Bank
4. Exam Regulations
5. Traffic Signs
6. Exam Structure Rules
7. Sample Exams
8. Exam Sessions (start/answer/submit/result/review)
9. Wrong Questions
10. Critical Practice
11. History & Analytics (candidate)
12. Dashboard overview (admin/editor)
13. Audit Logs API

### Out-of-scope v1 (để phase sau)
- Đào tạo vận hành trung tâm: khóa học/lớp học/buổi học/điểm danh
- Tài chính vận hành: phiếu thu/chi tiết phiếu thu
- Hồ sơ hành chính mở rộng

## 2. Sprint backlog đề xuất (ưu tiên cao -> thấp)

## Sprint A - Foundation hardening (đang triển khai)
- [x] Chuẩn hóa route versioning `/api/v1/*` cho controller hiện có (Auth, Topics, Questions).
- [x] Chuẩn hóa exception model có `StatusCode` và mapping qua middleware.
- [x] Tách secret cứng khỏi `appsettings.json` (JWT key để trống, bắt buộc nạp từ cấu hình môi trường).
- [ ] Chuẩn hóa swagger conventions theo module + response/error examples.
- [ ] Bổ sung health endpoint cơ bản.

**DoD Sprint A**
- Build pass
- Tất cả API hiện có dùng prefix `/api/v1`
- Lỗi nghiệp vụ trả đúng status code (400/401/403/404/409/422/500)

## Sprint B - Content core
- Topics: thêm `PUT`, `PATCH status`, `DELETE`, duplicate check, lifecycle rule.
- Questions: thêm `PUT`, `PATCH approve/archive`, filters nâng cao, duplicate-check API.
- Exam Regulations: CRUD + publish/version endpoints.
- Traffic Signs: CRUD + category APIs.

**DoD Sprint B**
- Module content chạy đủ theo API blueprint
- Validation rule rõ ràng bằng FluentValidation

## Sprint C - Exam definition
- Sample Exams: CRUD + assign questions + publish.
- Exam Structure Rules: CRUD + activate + validate pool.
- Critical question config rule.

## Sprint D - Exam runtime
- Start sample/random/wrong/critical sessions.
- Save answer từng câu, flag/unflag, submit/auto-submit.
- Grading + critical-fail rule + result snapshot.

## Sprint E - Candidate support + admin
- Wrong questions APIs + resolved lifecycle.
- Critical practice APIs.
- History APIs cho candidate/admin.
- Dashboard overview + exam stats nền tảng.
- Audit logs listing APIs.

## Sprint F - Hardening & quality gates
- Integration tests cho auth/content/exam runtime.
- Seed dữ liệu chuẩn cho swagger smoke test.
- Security hardening (rate-limit auth, policy rõ theo role).

## 3. Risk matrix rút gọn
1. **Domain trộn (exam + training operations)**
   - Risk: roadmap trượt, API phình to.
   - Mitigation: khóa phạm vi in-scope/out-of-scope như mục 1.
2. **Thiếu test layer**
   - Risk: regression cao khi thêm exam runtime.
   - Mitigation: thêm integration tests từ Sprint D.
3. **Thiếu seed đủ lớn**
   - Risk: swagger test không phản ánh thực tế nghiệp vụ.
   - Mitigation: tạo seed 50+ câu hỏi, 10+ critical.

## 4. Kế hoạch thực thi ngay sau tài liệu này
1. Hoàn tất phần còn lại của Sprint A (swagger conventions + health endpoint).
2. Bắt đầu Sprint B theo thứ tự: Topics -> Questions -> Regulations -> Traffic Signs.
