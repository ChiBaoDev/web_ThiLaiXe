# Backend Context cho Frontend (FE) — Chuẩn hóa theo implementation thật

> Nguồn sự thật (source of truth) cho FE ở giai đoạn hiện tại là API đang chạy trong backend code.
> Tài liệu này ưu tiên tính tích hợp nhanh, rõ contract request/response, rõ luồng màn hình.

---

## 1) Scope & nguyên tắc tích hợp

- Base URL: `/api/v1`
- Auth: `Bearer JWT` (header `Authorization: Bearer <token>`)
- Content-Type: `application/json`
- Mọi response đi theo envelope chung:

```json
{
  "success": true,
  "message": "Success",
  "data": {},
  "errors": null,
  "meta": null,
  "timestamp": "2026-01-01T00:00:00Z",
  "traceId": "..."
}
```

### 1.1 Cấu trúc response chung

- `success`: trạng thái nghiệp vụ.
- `message`: message hiển thị nhanh.
- `data`: payload chính.
- `errors`: danh sách lỗi chi tiết, mỗi phần tử thường có:
  - `code`
  - `field`
  - `detail`
- `meta`: có khi trả list phân trang.
- `timestamp`, `traceId`: hữu ích để debug.

### 1.2 Cấu trúc phân trang

Khi API trả list phân trang, `data` thường là object có:
- `items`
- `totalCount`
- `page`
- `pageSize`
- `totalPages`
- `hasPrevious`
- `hasNext`

Đồng thời `meta` cũng có thể chứa:
- `page`
- `pageSize`
- `totalItems`
- `totalPages`

> FE nên code linh hoạt để support cả thông tin pagination trong `data` và/hoặc `meta`.

### 1.3 Nguồn contract ưu tiên (khi có xung đột)

Thứ tự ưu tiên để FE quyết định contract:
1. Swagger/OpenAPI runtime của backend đang chạy.
2. Source code implementation thật (`Controller` -> `Service` -> `DTO`).
3. Tài liệu chuẩn nội bộ trong `thibanglaixe-guides/`.
4. Tài liệu mock cũ.

> Vì vậy FE **không** bám cứng tài liệu mock `thi-bang-lai-xe-api-be-doc.md` cho scope chưa được implement.

### 1.4 FE API Layer Rules (áp dụng ngay)

- Tạo 1 API client core dùng chung với:
  - baseURL `/api/v1`
  - inject bearer token tự động
  - parse envelope `ApiResponse<T>`
  - chuẩn hóa lỗi theo `errors[]`
- Không gọi API trực tiếp trong UI component; mọi call đi qua service/repository layer phía FE.
- Không hard-code pass/fail message theo suy đoán; hiển thị theo `message` + `data` backend trả về.
- Không hard-code role permission tuyệt đối ở FE; luôn fallback theo HTTP `401/403`.
- Mọi list page dùng 1 pagination adapter thống nhất (ưu tiên đọc từ `data`, fallback `meta`).

---

## 2) Auth + phiên người dùng

## 2.1 Đăng ký
- `POST /api/v1/auth/register`
- Public: có

Request body:
```json
{
  "ten_dang_nhap": "string",
  "mat_khau": "string",
  "email": "string",
  "so_dien_thoai": "string?",
  "ho_ten": "string",
  "ngay_sinh": "yyyy-mm-dd?",
  "gioi_tinh": "string?",
  "cccd": "string?",
  "dia_chi": "string?",
  "anh_chan_dung": "string?"
}
```

Response `data` chính:
```json
{
  "user_id": 0,
  "ten_dang_nhap": "string",
  "email": "string",
  "role_mac_dinh": "string",
  "created_at": "datetime"
}
```

## 2.2 Đăng nhập
- `POST /api/v1/auth/login`
- Public: có

Request body:
```json
{
  "ten_dang_nhap_hoac_email": "string",
  "mat_khau": "string"
}
```

Response `data` chính:
```json
{
  "user_id": 0,
  "ten_dang_nhap": "string",
  "email": "string",
  "access_token": "jwt",
  "expires_at_utc": "datetime",
  "roles": ["ADMIN"]
}
```

## 2.3 Đăng xuất
- `POST /api/v1/auth/logout`
- Auth: bắt buộc

## 2.4 Quên mật khẩu
- `POST /api/v1/auth/forgot-password`
- Public: có

## 2.5 Đặt lại mật khẩu
- `POST /api/v1/auth/reset-password`
- Public: có

## 2.6 Đổi mật khẩu
- `POST /api/v1/auth/change-password`
- Auth: bắt buộc
- Lưu ý: endpoint dùng `POST` (không phải `PUT`).

## 2.7 Hồ sơ hiện tại
- `GET /api/v1/auth/me`
- `PUT /api/v1/auth/me`
- Auth: bắt buộc

`GET /me` trả `data` dạng:
```json
{
  "user_id": 0,
  "ten_dang_nhap": "string",
  "email": "string",
  "so_dien_thoai": "string?",
  "trang_thai": "string",
  "hoc_vien_id": 0,
  "ho_ten": "string",
  "ngay_sinh": "yyyy-mm-dd?",
  "gioi_tinh": "string?",
  "cccd": "string?",
  "dia_chi": "string?",
  "anh_chan_dung": "string?",
  "roles": ["string"]
}
```

`PUT /me` request body (field optional):
```json
{
  "email": "string?",
  "so_dien_thoai": "string?",
  "ho_ten": "string?",
  "ngay_sinh": "yyyy-mm-dd?",
  "gioi_tinh": "string?",
  "cccd": "string?",
  "dia_chi": "string?",
  "anh_chan_dung": "string?"
}
```

---

## 3) Question Topics

- `GET /api/v1/question-topics`
- `GET /api/v1/question-topics/{id}`
- `POST /api/v1/question-topics`
- `PUT /api/v1/question-topics/{id}`
- `DELETE /api/v1/question-topics/{id}`

Create/Update body:
```json
{
  "code": "string",
  "name": "string",
  "description": "string?"
}
```

Topic item:
```json
{
  "id": 0,
  "code": "string",
  "name": "string",
  "description": "string?",
  "questionCount": 0
}
```

---

## 4) Questions

- `GET /api/v1/questions`
- `GET /api/v1/questions/{id}`
- `POST /api/v1/questions`
- `PUT /api/v1/questions/{id}`
- `PATCH /api/v1/questions/{id}/approve`
- `PATCH /api/v1/questions/{id}/archive`
- `DELETE /api/v1/questions/{id}`

Create body:
```json
{
  "topicId": 0,
  "content": "string",
  "questionType": "string",
  "level": "string?",
  "isCritical": false
}
```

Question item:
```json
{
  "id": 0,
  "topicId": 0,
  "content": "string",
  "questionType": "string",
  "level": "string?",
  "isCritical": false,
  "status": "string"
}
```

---

## 5) Sample Exams

- `GET /api/v1/sample-exams`
- `GET /api/v1/sample-exams/{id}`
- `POST /api/v1/sample-exams`
- `PUT /api/v1/sample-exams/{id}`
- `POST /api/v1/sample-exams/{id}/questions`
- `DELETE /api/v1/sample-exams/{id}/questions/{questionId}`
- `PATCH /api/v1/sample-exams/{id}/publish`
- `DELETE /api/v1/sample-exams/{id}`

Create/Update body:
```json
{
  "code": "string",
  "name": "string",
  "examPeriodId": 0,
  "totalQuestions": 25,
  "durationMinutes": 19
}
```

Assign questions body:
```json
{
  "questionIds": [1, 2, 3]
}
```

Sample exam item:
```json
{
  "id": 0,
  "code": "string",
  "name": "string",
  "examPeriodId": 0,
  "totalQuestions": 0,
  "durationMinutes": 0,
  "status": "string",
  "createdAt": "datetime",
  "linkedQuestionCount": 0,
  "questionIds": [1, 2]
}
```

---

## 6) Exam Sessions (flow thi chính FE cần bám)

- `POST /api/v1/exams/sample/{sampleExamId}/start`
- `GET /api/v1/exams/sessions/{sessionId}`
- `GET /api/v1/exams/sessions/{sessionId}/questions/{number}`
- `POST /api/v1/exams/sessions/{sessionId}/answers`
- `POST /api/v1/exams/sessions/{sessionId}/submit`
- `POST /api/v1/exams/sessions/{sessionId}/auto-submit`
- `GET /api/v1/exams/sessions/{sessionId}/result`
- `GET /api/v1/exams/sessions/{sessionId}/review`

`start` trả:
```json
{
  "sessionId": 0,
  "sampleExamId": 0,
  "sampleExamName": "string",
  "totalQuestions": 0,
  "durationMinutes": 0,
  "startedAt": "datetime",
  "status": "string"
}
```

`submit answer` body:
```json
{
  "questionId": 0,
  "answerId": 0
}
```

`result` trả:
```json
{
  "sessionId": 0,
  "totalQuestions": 0,
  "correctAnswers": 0,
  "wrongAnswers": 0,
  "unansweredAnswers": 0,
  "score": 0,
  "result": "string",
  "failedByCriticalQuestion": false,
  "submittedAt": "datetime?",
  "status": "string"
}
```

### 6.1 Trình tự màn hình FE đề xuất
1. User chọn đề mẫu → gọi `start`.
2. Điều hướng vào màn thi với `sessionId`.
3. Mỗi câu: gọi API lấy câu theo `number`.
4. Chọn đáp án: gọi `answers`.
5. Hết giờ: FE gọi `auto-submit` hoặc backend tự enforce qua rule.
6. Nộp bài: gọi `submit`.
7. Xem kết quả: gọi `result`.
8. Xem giải chi tiết: gọi `review`.

---

## 7) Exam Structure Rules

- `GET /api/v1/exam-structure-rules`
- `GET /api/v1/exam-structure-rules/{id}`
- `POST /api/v1/exam-structure-rules`
- `PUT /api/v1/exam-structure-rules/{id}`
- `PATCH /api/v1/exam-structure-rules/{id}/activate`
- `POST /api/v1/exam-structure-rules/{id}/validate`
- `DELETE /api/v1/exam-structure-rules/{id}`

> Module này thiên về vận hành/admin để đảm bảo rule sinh đề hợp lệ.

---

## 8) Wrong Questions

- `GET /api/v1/wrong-questions`
- `GET /api/v1/wrong-questions/summary`
- `POST /api/v1/wrong-questions/start-practice`
- `PATCH /api/v1/wrong-questions/{questionId}/resolved`
- `DELETE /api/v1/wrong-questions/{questionId}`

Start practice body:
```json
{
  "size": 10
}
```

---

## 9) Critical Questions

- `GET /api/v1/critical-questions`
- `GET /api/v1/critical-questions/summary`
- `POST /api/v1/critical-questions/start-practice`

Start practice body:
```json
{
  "size": 10
}
```

---

## 10) History

### 10.1 Candidate
- `GET /api/v1/history/exams`
- `GET /api/v1/history/exams/{sessionId}`
- `GET /api/v1/history/analytics`

### 10.2 Admin
- `GET /api/v1/admin/history/exams`
- `GET /api/v1/admin/history/users/{userId}`

Query thường dùng:
- `page`, `pageSize`
- `from`, `to`
- `result`

---

## 11) Dashboard

- `GET /api/v1/dashboard/overview`
- `GET /api/v1/dashboard/exam-stats`
- `GET /api/v1/dashboard/question-stats`
- `GET /api/v1/dashboard/weak-topics`
- `GET /api/v1/dashboard/critical-question-stats`

Query thường dùng:
- `from`, `to`

---

## 12) CMS

### 12.1 Internal
- `GET /api/v1/cms/categories`
- `GET /api/v1/cms/categories/{id}`
- `POST /api/v1/cms/categories`
- `PUT /api/v1/cms/categories/{id}`
- `DELETE /api/v1/cms/categories/{id}`
- `GET /api/v1/cms/posts`
- `GET /api/v1/cms/posts/{id}`
- `POST /api/v1/cms/posts`
- `PUT /api/v1/cms/posts/{id}`
- `DELETE /api/v1/cms/posts/{id}`

### 12.2 Public
- `GET /api/v1/public/cms/categories`
- `GET /api/v1/public/cms/posts`
- `GET /api/v1/public/cms/posts/{id}`

Create post body:
```json
{
  "code": "string",
  "title": "string",
  "slug": "string",
  "summary": "string?",
  "content": "string",
  "postType": "string",
  "thumbnailFileId": 0,
  "metaTitle": "string?",
  "metaDescription": "string?",
  "canonicalUrl": "string?",
  "publishedAt": "datetime?",
  "status": "draft",
  "categoryIds": [1, 2]
}
```

Post item:
```json
{
  "id": 0,
  "code": "string",
  "title": "string",
  "slug": "string",
  "summary": "string?",
  "content": "string",
  "postType": "string",
  "thumbnailFileId": 0,
  "metaTitle": "string?",
  "metaDescription": "string?",
  "canonicalUrl": "string?",
  "publishedAt": "datetime?",
  "status": "string",
  "authorId": 0,
  "createdAt": "datetime",
  "updatedAt": "datetime",
  "categoryIds": [1, 2]
}
```

---

## 13) Files

- `GET /api/v1/files`
- `GET /api/v1/files/{id}`
- `POST /api/v1/files`
- `PUT /api/v1/files/{id}`
- `DELETE /api/v1/files/{id}`
- `GET /api/v1/files/{id}/usages`
- `POST /api/v1/files/{id}/usages`

Create file body:
```json
{
  "storageProvider": "string",
  "bucketName": "string?",
  "objectKey": "string",
  "publicUrl": "string",
  "fileName": "string",
  "mimeType": "string",
  "sizeBytes": 0,
  "checksumSha256": "string?",
  "width": 0,
  "height": 0,
  "durationSeconds": 0
}
```

File item:
```json
{
  "id": 0,
  "storageProvider": "string",
  "bucketName": "string?",
  "objectKey": "string",
  "publicUrl": "string",
  "fileName": "string",
  "mimeType": "string",
  "sizeBytes": 0,
  "checksumSha256": "string?",
  "width": 0,
  "height": 0,
  "durationSeconds": 0,
  "status": "string",
  "createdBy": 0,
  "createdAt": "datetime",
  "updatedAt": "datetime"
}
```

---

## 14) Entitlements

- `GET /api/v1/entitlements/packages`
- `GET /api/v1/entitlements/packages/{id}`
- `POST /api/v1/entitlements/packages`
- `PUT /api/v1/entitlements/packages/{id}`
- `DELETE /api/v1/entitlements/packages/{id}`
- `GET /api/v1/entitlements/user-entitlements`
- `GET /api/v1/entitlements/user-entitlements/{id}`
- `POST /api/v1/entitlements/user-entitlements/grant`
- `PATCH /api/v1/entitlements/user-entitlements/{id}/status`

Grant body:
```json
{
  "userId": 0,
  "packageId": 0,
  "effectiveFrom": "datetime",
  "expiresAt": "datetime?",
  "source": "string",
  "note": "string?"
}
```

---

## 15) Certificates

### 15.1 Internal
- `GET /api/v1/certificates`
- `GET /api/v1/certificates/{id}`
- `POST /api/v1/certificates/issue`
- `PATCH /api/v1/certificates/{id}/status`
- `POST /api/v1/certificates/exam-results/{examResultId}/confirm`

Issue body:
```json
{
  "code": "string",
  "studentId": 0,
  "examResultId": 0,
  "issuedAt": "datetime",
  "expiresAt": "datetime?",
  "certificateFileId": 0
}
```

### 15.2 Public
- `GET /api/v1/public/certificates/verify/{code}`

---

## 16) Health

- `GET /api/v1/health`
- Public: có

---

## 17) Authorization notes (quan trọng cho FE phân quyền UI)

- Nhiều endpoint dùng `[Authorize]` chung (cần token hợp lệ).
- Một số endpoint có policy cụ thể (đặc biệt nhóm certificates).
- FE nên tách:
  - **UI visibility** theo role từ payload login/me.
  - **API guard** theo response thực tế (401/403), không hard-code tuyệt đối ở client.

### 17.1 Role map FE nên chuẩn hóa

- Candidate/User:
  - thi thử, lịch sử cá nhân, profile cá nhân.
- Editor/ContentEditor:
  - quản lý nội dung ngân hàng câu hỏi/CMS.
- Admin:
  - dashboard, history admin, entitlement/certificate operation nhạy cảm.

> Tên role thực tế phải lấy từ payload `login`/`me`; FE chỉ dùng map này để dựng UX, không xem là source of truth tuyệt đối.

### 17.2 Auth flow rules cho FE

- Nếu `401`: clear session local + redirect login (trừ endpoint public).
- Nếu `403`: giữ session, hiển thị màn hình "không có quyền".
- Không refresh token tự chế nếu backend chưa có refresh-token endpoint chính thức.
- Khi app khởi động:
  1. đọc token local,
  2. gọi `GET /auth/me`,
  3. nếu fail thì logout mềm.

---

## 18) HTTP status thường gặp

- `200`: thành công.
- `201`: tạo mới thành công.
- `204`: xóa thành công, không body dữ liệu nghiệp vụ.
- `400`: input không hợp lệ.
- `401`: thiếu/invalid token.
- `403`: không đủ quyền.
- `404`: không tìm thấy.
- `409`: conflict dữ liệu.
- `422`: nghiệp vụ không cho phép tại trạng thái hiện tại.
- `500`: lỗi hệ thống.

---

## 19) Validation & Query Rules FE cần tuân thủ

Theo guideline validation nội bộ + implementation hiện tại:

- FE validate sớm để UX tốt hơn, nhưng **không thay thế** backend validation.
- Với list APIs, chuẩn query param nên dùng nhất quán:
  - `page`, `pageSize`, `search`
  - module đặc thù: `status`, `from`, `to`, `result`, ...
- Rule FE đề xuất:
  - `page >= 1`
  - `1 <= pageSize <= 100` (nếu chưa có spec riêng)
- Khi backend trả lỗi validation:
  - map `errors[].field` vào form field tương ứng,
  - hiển thị `errors[].detail` ưu tiên cao hơn message chung.

---

## 20) Exam Domain Rules FE bắt buộc bám

Để tránh sai logic thi:

- Mốc tham chiếu hiện tại:
  - tổng câu hỏi thường là `25`
  - thời gian `19` phút
  - đạt khi đúng từ `21` câu
- Có rule liệt (critical question):
  - sai >= 1 câu liệt => rớt, kể cả điểm tổng đủ.
- FE không tự tính kết quả cuối cùng để chốt pass/fail; backend là nguồn quyết định.
- FE phải hiển thị rõ trạng thái thất bại do câu liệt khi `failedByCriticalQuestion = true`.

---

## 21) Checklist tích hợp FE (khuyến nghị triển khai ngay)

1. Tạo API client typed theo từng module.
2. Tạo interceptor cho `Authorization` + chuẩn hóa parse envelope.
3. Chuẩn hóa error adapter từ `errors[]` -> message hiển thị form.
4. Build auth store gồm:
   - token
   - expiresAt
   - me profile
   - roles
5. Build exam session state store:
   - `sessionId`
   - current question number
   - selected answers
   - countdown timer
   - submit lock flag (chặn double submit)
6. Build common pagination adapter cho list pages.
7. Tạo contract test tối thiểu cho các flow chính:
   - auth login/me
   - exam session start/answer/submit/result
   - wrong/critical practice
8. Kết nối Swagger/OpenAPI vào pipeline FE để gen typed client (nếu team dùng gen-api).

---

## 22) Gaps đã chốt để FE không nhầm scope

- Không bám tài liệu mock cũ cho các domain chưa có implementation (students/courses/receipts/reports...).
- FE sprint hiện tại chỉ tích hợp theo endpoint trong tài liệu này.
- Nếu backend mở rộng thêm scope, tài liệu này sẽ được version hóa bổ sung theo mốc release.
- Khi có mâu thuẫn giữa guide và behavior runtime, ưu tiên behavior runtime + mở issue cập nhật guide.
