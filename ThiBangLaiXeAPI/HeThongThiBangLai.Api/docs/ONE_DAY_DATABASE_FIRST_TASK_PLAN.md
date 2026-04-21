# KẾ HOẠCH TASK 1 NGÀY (DATABASE-FIRST) – HOÀN THIỆN NỀN TẢNG BUSINESS CHO WEB THI BẰNG LÁI XE MÔ TÔ

## 1) Mục tiêu trong 1 ngày

Hoàn thành **phiên bản nền tảng chạy được** theo hướng **Database First** để mở rộng kinh doanh:

1. Chuẩn hóa RBAC + business entitlement cho User (user mới / học viên / giáo viên / admin / sale).
2. Bổ sung cụm bảng `files` để quản trị media tập trung.
3. Bổ sung cụm bảng CMS tối thiểu cho SEO (`categories`, `posts`, liên kết danh mục).
4. Bổ sung bảng kết quả/chứng chỉ tối thiểu (`exam_results`, `certificates`) để xác nhận đậu/rớt.
5. Sinh model/scaffold theo database-first và mở API nền tảng cho FE.

> Phạm vi 1 ngày: ưu tiên “xương sống” có thể chạy và tích hợp FE ngay, chưa đi sâu tối ưu UI/UX.

---

## 2) Nguyên tắc bắt buộc

- **Database First tuyệt đối**: Thiết kế SQL trước → chạy migration SQL → verify DB → scaffold EF models/context → code Repository/Service/Controller.
- Tất cả thay đổi schema phải là SQL script version hóa trong thư mục `db/`.
- Mọi API phải trả về chuẩn `ApiResponse<T>` hiện có của dự án.
- Ưu tiên idempotent script (chạy lại không hỏng dữ liệu seed nền).

---

## 3) Danh sách file cần tạo/cập nhật trong ngày

## 3.1. SQL (ưu tiên cao nhất)

Tạo mới trong `HeThongThiBangLai.Api/db/`:

1. `03_add_rbac_entitlement_and_files.sql`
2. `04_add_cms_and_certificate.sql`
3. `05_seed_roles_permissions_entitlements.sql`
4. `06_verify_new_modules.sql`

Cập nhật orchestrator team:

5. `01_full_setup_database_team.sql` (thêm `:r .\03...`, `:r .\04...`, `:r .\05...`, `:r .\06...`)

## 3.2. Backend API (.NET)

Tạo mới các nhóm chính:

- `Controllers/FilesController.cs`
- `Controllers/CmsController.cs`
- `Controllers/EntitlementsController.cs`
- `Controllers/CertificatesController.cs`

- `DTOs/Files/*`
- `DTOs/Cms/*`
- `DTOs/Entitlements/*`
- `DTOs/Certificates/*`

- `Repositories/Files/*`, `Repositories/Cms/*`, `Repositories/Entitlements/*`, `Repositories/Certificates/*`
- `Services/Files/*`, `Services/Cms/*`, `Services/Entitlements/*`, `Services/Certificates/*`
- `Validators/...` tương ứng

Cập nhật:

- `Program.cs` (đăng ký DI + policy auth)
- `seed_admin.sql` (đồng bộ role code với logic runtime)

---

## 4) Thiết kế Database chi tiết (Database First)

## 4.1. RBAC + Entitlement

### Bảng mới

1. `loai_nguoi_dung`
- `id` BIGINT IDENTITY PK
- `ma_loai` VARCHAR(30) UNIQUE (GUEST, LEAD, HOC_VIEN, GIAO_VIEN, STAFF_SALE, ADMIN)
- `ten_loai` NVARCHAR(100)
- `mo_ta` NVARCHAR(255) NULL

2. `nguoi_dung_loai`
- `id` BIGINT IDENTITY PK
- `nguoi_dung_id` BIGINT FK -> `nguoi_dung(id)`
- `loai_nguoi_dung_id` BIGINT FK -> `loai_nguoi_dung(id)`
- unique (`nguoi_dung_id`, `loai_nguoi_dung_id`)

3. `goi_quyen`
- `id` BIGINT IDENTITY PK
- `ma_goi` VARCHAR(50) UNIQUE
- `ten_goi` NVARCHAR(150)
- `mo_ta` NVARCHAR(500) NULL
- `is_active` BIT DEFAULT 1

4. `quyen_su_dung`
- `id` BIGINT IDENTITY PK
- `nguoi_dung_id` BIGINT FK -> `nguoi_dung(id)`
- `goi_quyen_id` BIGINT FK -> `goi_quyen(id)`
- `ngay_hieu_luc` DATETIME2 NOT NULL
- `ngay_het_han` DATETIME2 NULL
- `nguon_cap` VARCHAR(30) NOT NULL (`payment/manual/promo`)
- `trang_thai` VARCHAR(30) NOT NULL (`active/expired/revoked`)
- index: (`nguoi_dung_id`, `trang_thai`), (`ngay_het_han`)

### Quy tắc business chính

- User mới (`LEAD`) chỉ xem khóa học/public post, chưa được thi.
- User có entitlement `EXAM_PRACTICE` mới được start exam session.
- User có entitlement `LEARNING_MATERIAL` mới truy cập tài liệu ôn tập chuyên sâu.

---

## 4.2. Files storage trung tâm

### Bảng mới

1. `files`
- `id` BIGINT IDENTITY PK
- `storage_provider` VARCHAR(30) NOT NULL (`local/s3/cloudinary/...`)
- `bucket_name` VARCHAR(100) NULL
- `object_key` VARCHAR(500) NOT NULL
- `public_url` VARCHAR(1000) NOT NULL
- `file_name` NVARCHAR(255) NOT NULL
- `mime_type` VARCHAR(100) NOT NULL
- `size_bytes` BIGINT NOT NULL CHECK >= 0
- `checksum_sha256` VARCHAR(128) NULL
- `width` INT NULL
- `height` INT NULL
- `duration_seconds` INT NULL
- `trang_thai` VARCHAR(30) NOT NULL DEFAULT 'active'
- `created_by` BIGINT NULL FK -> `nguoi_dung(id)`
- `created_at` DATETIME2 DEFAULT GETDATE()

2. `file_usages`
- `id` BIGINT IDENTITY PK
- `file_id` BIGINT FK -> `files(id)`
- `entity_name` VARCHAR(50) NOT NULL (hoc_vien, post, khoa_hoc, ...)
- `entity_id` BIGINT NOT NULL
- `field_name` VARCHAR(50) NOT NULL (thumbnail, cover, avatar, ...)
- `is_primary` BIT DEFAULT 0
- `sort_order` INT DEFAULT 0
- unique (`file_id`, `entity_name`, `entity_id`, `field_name`)
- index (`entity_name`, `entity_id`)

### Migration dữ liệu cũ

- Nếu `hoc_vien.anh_chan_dung` có dữ liệu:
  - insert sang `files` + `file_usages` (`entity_name='hoc_vien'`, `field_name='avatar'`).
  - tạm thời giữ cột cũ để tương thích, đánh dấu deprecate ở docs.

---

## 4.3. CMS + SEO cơ bản

### Bảng mới

1. `categories`
- `id`, `parent_id` nullable self FK
- `ma_danh_muc` VARCHAR(50) UNIQUE
- `ten_danh_muc` NVARCHAR(150)
- `slug` VARCHAR(200) UNIQUE
- `mo_ta` NVARCHAR(500) NULL
- `is_active` BIT

2. `posts`
- `id`
- `ma_bai_viet` VARCHAR(50) UNIQUE
- `title` NVARCHAR(255)
- `slug` VARCHAR(255) UNIQUE
- `summary` NVARCHAR(1000) NULL
- `content` NVARCHAR(MAX)
- `post_type` VARCHAR(30) (`gioi_thieu`, `tin_tuc`, `khoa_hoc`, `huong_dan`)
- `thumbnail_file_id` BIGINT NULL FK -> `files(id)`
- SEO fields: `meta_title`, `meta_description`, `canonical_url`
- `published_at` DATETIME2 NULL
- `trang_thai` VARCHAR(30) (`draft/published/archived`)
- `author_id` BIGINT NULL FK -> `nguoi_dung(id)`

3. `post_categories`
- `id`
- `post_id` FK -> `posts(id)`
- `category_id` FK -> `categories(id)`
- unique (`post_id`, `category_id`)

---

## 4.4. Kết quả thi + chứng chỉ

### Bảng mới

1. `exam_results`
- `id`
- `bai_thi_id` BIGINT UNIQUE FK -> `bai_thi(id)`
- `hoc_vien_id` FK -> `hoc_vien(id)`
- `tong_so_cau`, `so_cau_dung`, `diem`
- `ket_qua` VARCHAR(20) (`dat/khong_dat`)
- `xac_nhan_boi` BIGINT NULL FK -> `nguoi_dung(id)`
- `xac_nhan_luc` DATETIME2 NULL

2. `certificates`
- `id`
- `ma_chung_chi` VARCHAR(50) UNIQUE
- `hoc_vien_id` FK -> `hoc_vien(id)`
- `exam_result_id` FK -> `exam_results(id)`
- `ngay_cap` DATETIME2
- `ngay_het_han` DATETIME2 NULL
- `trang_thai` VARCHAR(30) (`valid/revoked/expired`)
- `certificate_file_id` BIGINT NULL FK -> `files(id)`

---

## 5) Kế hoạch theo giờ trong 1 ngày

## Khung giờ 08:00 - 09:00

- Chốt schema mới (RBAC/Entitlement/Files/CMS/Certificate).
- Viết skeleton 2 script DDL:
  - `03_add_rbac_entitlement_and_files.sql`
  - `04_add_cms_and_certificate.sql`
- Chuẩn hóa tên cột/constraint/index theo convention hiện tại.

## Khung giờ 09:00 - 10:00

- Viết seed + verify script:
  - `05_seed_roles_permissions_entitlements.sql`
  - `06_verify_new_modules.sql`
- Seed role bắt buộc: `ADMIN`, `HOC_VIEN`, `GIAO_VIEN`, `STAFF_SALE`, `LEAD`.
- Seed permission cốt lõi: `CMS_POST_MANAGE`, `FILE_MANAGE`, `EXAM_ACCESS`, `CERTIFICATE_ISSUE`, `ENTITLEMENT_GRANT`.

## Khung giờ 10:00 - 11:00

- Cập nhật `01_full_setup_database_team.sql` để chạy full pipeline.
- Chạy full setup trên SQLCMD Mode.
- Chạy `06_verify_new_modules.sql`, chụp kết quả verify.

## Khung giờ 11:00 - 12:00

- Database-first scaffold:
  - Cập nhật `ApplicationDbContext` + model mới (theo quy trình hiện tại dự án).
- Build solution kiểm tra compile.

## Khung giờ 13:30 - 15:00

- Tạo API module `Files`:
  - upload metadata (không bắt buộc upload binary trong ngày 1)
  - link file vào entity
  - list file theo entity

## Khung giờ 15:00 - 16:00

- Tạo API module `CMS`:
  - categories CRUD
  - posts CRUD + publish/unpublish
  - API public list posts theo slug/category

## Khung giờ 16:00 - 17:00

- Tạo API module `Entitlements`:
  - grant/revoke entitlement
  - check quyền dùng theo user
- Tạo API module `Certificates`:
  - xác nhận kết quả thi
  - cấp chứng chỉ
  - tra cứu chứng chỉ theo mã

## Khung giờ 17:00 - 18:00

- Gắn authorization policy theo role/permission.
- Cập nhật Swagger summary cho API mới.
- Smoke test nhanh toàn bộ endpoint chính.

## Khung giờ 20:00 - 21:00 (buffer)

- Sửa bug phát sinh.
- Ghi tài liệu API contract cho FE.
- Chốt checklist bàn giao.

---

## 6) API contract cần có trong ngày

## 6.1. Files API

1. `POST /api/v1/files`
- Mục tiêu: tạo metadata file
- Request: `file_name, mime_type, size_bytes, public_url, object_key, storage_provider`
- Response: `file_id, public_url`

2. `POST /api/v1/files/{fileId}/usages`
- Mục tiêu: gán file cho entity
- Request: `entity_name, entity_id, field_name, is_primary`

3. `GET /api/v1/files/usages`
- Query: `entity_name, entity_id`
- Response: danh sách file đã gán

## 6.2. CMS API

1. `POST /api/v1/cms/categories`
2. `GET /api/v1/cms/categories`
3. `POST /api/v1/cms/posts`
4. `PUT /api/v1/cms/posts/{id}`
5. `POST /api/v1/cms/posts/{id}/publish`
6. `GET /api/v1/public/posts?type=tin_tuc&categorySlug=...`
7. `GET /api/v1/public/posts/{slug}`

## 6.3. Entitlement API

1. `POST /api/v1/entitlements/grant`
- Request: `user_id, package_code, start_at, end_at, source`

2. `POST /api/v1/entitlements/revoke`
- Request: `user_id, entitlement_id, reason`

3. `GET /api/v1/entitlements/users/{userId}`
- Response: entitlement đang active

## 6.4. Certificates API

1. `POST /api/v1/certificates/exam-results/confirm`
- Request: `bai_thi_id, confirmed_by`
- Tác vụ: upsert `exam_results`

2. `POST /api/v1/certificates/issue`
- Request: `exam_result_id, hoc_vien_id`
- Tác vụ: tạo `certificates`

3. `GET /api/v1/public/certificates/{ma_chung_chi}`
- Public verify trạng thái chứng chỉ

---

## 7) Logic phân quyền áp dụng ngay

- `ADMIN`: full access.
- `STAFF_SALE`: quản lý lead/user cơ bản, cấp entitlement theo thanh toán.
- `GIAO_VIEN`: xem lớp, xác nhận thi, không có quyền cấu hình hệ thống.
- `HOC_VIEN`: thi thử/ôn tập nếu có entitlement.
- `LEAD`: chỉ xem public CMS + khóa học giới thiệu.

### Policy đề xuất

- `Policy:CanManageCms`
- `Policy:CanManageFiles`
- `Policy:CanGrantEntitlement`
- `Policy:CanIssueCertificate`
- `Policy:CanTakeExam`

---

## 8) Checklist nghiệm thu cuối ngày

## 8.1. Database

- [ ] Chạy full setup thành công từ `01_full_setup_database_team.sql`.
- [ ] Có đủ bảng mới: `files`, `file_usages`, `loai_nguoi_dung`, `nguoi_dung_loai`, `goi_quyen`, `quyen_su_dung`, `categories`, `posts`, `post_categories`, `exam_results`, `certificates`.
- [ ] Có index/unique/check cơ bản đúng.
- [ ] Verify script pass 100%.

## 8.2. Backend

- [ ] Build API thành công.
- [ ] Swagger hiển thị endpoint mới.
- [ ] Test được ít nhất 1 luồng end-to-end:
  - user LEAD -> mua gói (grant entitlement) -> mở quyền thi -> nộp bài -> xác nhận kết quả -> cấp chứng chỉ -> verify chứng chỉ.

## 8.3. Frontend readiness

- [ ] Có public API cho category/post/news.
- [ ] Có API entitlement để FE khóa/mở màn hình chức năng.
- [ ] Có API file để FE gán ảnh bài viết/khóa học.

---

## 9) Rủi ro & phương án xử lý nhanh

1. **Lệch role code giữa seed và service**
- Cách xử lý: sửa seed + hằng role trong service theo 1 nguồn chuẩn duy nhất.

2. **Scaffold model lỗi do schema đổi nhiều**
- Cách xử lý: hoàn tất DB trước, scaffold 1 lần, không chỉnh tay model trước khi scaffold.

3. **Không kịp upload binary file service**
- Cách xử lý: ngày 1 chỉ làm metadata + mapping, binary upload xử lý ngày 2.

4. **Không kịp full CRUD CMS nâng cao**
- Cách xử lý: chốt CRUD cơ bản + publish + public listing.

---

## 10) Đầu ra bắt buộc bàn giao cuối ngày

1. SQL scripts mới trong `db/` + chạy được.
2. API modules mới (`Files`, `CMS`, `Entitlements`, `Certificates`) chạy được qua Swagger.
3. Tài liệu endpoint ngắn cho FE (request/response mẫu).
4. Báo cáo verify DB + danh sách test case đã pass.

---

## 11) Định nghĩa “hoàn thành trong 1 ngày”

Được xem là hoàn thành khi:

- Hệ thống đã có nền tảng business để bán khóa học/đóng tiền/mở quyền.
- FE đã có đủ API để làm trang public SEO + trang user có điều kiện quyền.
- Dữ liệu ảnh/file được quản lý tập trung.
- Có luồng chứng chỉ/xác nhận kết quả tối thiểu.
- Mọi thứ tuân thủ quy trình **Database First**.
