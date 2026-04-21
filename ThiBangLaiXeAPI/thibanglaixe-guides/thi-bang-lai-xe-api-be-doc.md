# TÀI LIỆU ENDPOINT API - Backend ↔ Frontend - Hệ thống Quản lý Trường Dạy Lái Xe

## Mục lục
- Quy ước chung
- 1. Xác thực (Authentication)
- 2. Quản lý người dùng & phân quyền
- 3. Quản lý học viên
- 4. Hồ sơ đăng ký & giấy tờ
- 5. Quản lý khóa học
- 6. Quản lý lớp học
- 7. Đăng ký khóa học
- 8. Buổi học & điểm danh
- 9. Ngân hàng câu hỏi
- 10. Ôn tập (Practice)
- 11. Thi cử (Exam)
- 12. Đăng ký dự thi
- 13. Bài thi & kết quả
- 14. Tài chính (Phiếu thu)
- 15. Vi phạm quy chế
- 16. Dashboard & Báo cáo

## Quy ước chung
- Tất cả request/response đều dùng Content-Type: application/json
- Các endpoint có 🔐 yêu cầu Header: Authorization: Bearer <access_token>
- Lỗi chung: 401 Unauthorized | 403 Forbidden | 404 Not Found | 500 Internal Server Error
- Danh sách (GET list) hỗ trợ: ?page=0&size=10&sort=createdAt,desc&search=...

## 1. Xác thực (Authentication)

#### `POST /api/auth/register` - Đăng ký tài khoản mới

##### Request Body:
| Trường | Kiểu | Bắt buộc | Mô tả |
| --- | --- | --- | --- |
| ten_dang_nhap | String | ✔ Có | Tên đăng nhập, không dấu, 6–30 ký tự |
| mat_khau | String | ✔ Có | Mật khẩu, tối thiểu 6 ký tự |
| email | String | ✔ Có | Email hợp lệ |
| so_dien_thoai | String | ✔ Có | Số điện thoại Việt Nam |
| ho_ten | String | ✔ Có | Họ và tên đầy đủ |
| ngay_sinh | String | ✔ Có | Định dạng YYYY-MM-DD |
| gioi_tinh | String | ✔ Có | Nam / Nữ / Khác |
| cccd | String | ✔ Có | Căn cước công dân 12 số |
| dia_chi | String | Không | Địa chỉ thường trú |
| anh_chan_dung | String | Không | URL ảnh đại diện |

##### Response (thành công):
```
// HTTP 201 Created
{
  "user_id": 1,
  "ten_dang_nhap": "admin01",
  "email": "admin@gmail.com",
  "role_mac_dinh": "HOC_VIEN",
  "created_at": "2026-04-01T08:00:00Z"
}
```

#### `POST /api/auth/login` - Đăng nhập, nhận access_token

##### Request Body (JSON):
```
{
  "ten_dang_nhap": "admin01",
  "mat_khau": "123456"
}
```

##### Response (thành công):
```
// HTTP 200 OK
{
  "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refresh_token": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4...",
  "token_type": "Bearer",
  "expires_in": 3600,
  "user": {
    "user_id": 1,
    "ten_dang_nhap": "admin01",
    "ho_ten": "Nguyễn Văn A",
    "email": "admin@gmail.com",
    "anh_chan_dung": "https://example.com/avatar.jpg",
    "role": "HOC_VIEN"
  }
}
```

#### `POST /api/auth/logout` - Đăng xuất, thu hồi token
**Auth:** Bearer JWT Token

##### Request Body (JSON):
```
{ "refresh_token": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4..." }
```

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Đăng xuất thành công" }
```

#### `GET /api/auth/me` - Lấy thông tin người dùng hiện tại
**Auth:** Bearer JWT Token

##### Response (thành công):
```
// HTTP 200 OK
{
  "user_id": 1,
  "ten_dang_nhap": "admin01",
  "ho_ten": "Nguyễn Văn A",
  "email": "admin@gmail.com",
  "so_dien_thoai": "0901234567",
  "ngay_sinh": "2000-01-15",
  "gioi_tinh": "Nam",
  "dia_chi": "TP.HCM",
  "anh_chan_dung": "https://example.com/avatar.jpg",
  "role": "HOC_VIEN",
  "trang_thai": "HOAT_DONG",
  "created_at": "2026-04-01T08:00:00Z"
}
```

#### `POST /api/auth/refresh-token` - Làm mới access token

##### Request Body (JSON):
```
{ "refresh_token": "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4..." }
```

##### Response (thành công):
```
// HTTP 200 OK
{
  "access_token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expires_in": 3600
}
```

#### `PUT /api/auth/change-password` - Đổi mật khẩu
**Auth:** Bearer JWT Token

##### Request Body (JSON):
```
{
  "mat_khau_cu": "123456",
  "mat_khau_moi": "newPass@2026",
  "xac_nhan_mat_khau_moi": "newPass@2026"
}
```

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Đổi mật khẩu thành công" }
```

#### `POST /api/auth/forgot-password` - Gửi email đặt lại mật khẩu

##### Request Body (JSON):
```
{ "email": "admin@gmail.com" }
```

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Email đặt lại mật khẩu đã được gửi" }
```

#### `POST /api/auth/reset-password` - Đặt lại mật khẩu bằng token email

##### Request Body (JSON):
```
{
  "token": "abc123resettoken",
  "mat_khau_moi": "newPass@2026",
  "xac_nhan_mat_khau_moi": "newPass@2026"
}
```

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Đặt lại mật khẩu thành công" }
```

## 2. Quản lý người dùng & phân quyền

#### `GET /api/users` - Lấy danh sách người dùng
**Auth:** Admin

##### Query Params
| Param | Kiểu | Mặc định | Mô tả |
| --- | --- | --- | --- |
| page | int | 0 | Số trang (bắt đầu từ 0) |
| size | int | 10 | Số bản ghi mỗi trang |
| search | String |  | Tìm theo tên, email, SĐT |
| role | String |  | Lọc theo vai trò |
| status | String |  | HOAT_DONG / BI_KHOA |

##### Response (thành công):
```
// HTTP 200 OK
{
  "content": [
    {
      "user_id": 1,
      "ten_dang_nhap": "admin01",
      "ho_ten": "Nguyễn Văn A",
      "email": "admin@gmail.com",
      "so_dien_thoai": "0901234567",
      "role": "HOC_VIEN",
      "trang_thai": "HOAT_DONG",
      "created_at": "2026-04-01T08:00:00Z"
    }
  ],
  "totalElements": 50,
  "totalPages": 5,
  "currentPage": 0,
  "pageSize": 10
}
```

#### `GET /api/users/{id}` - Lấy chi tiết người dùng
**Auth:** Admin

##### Response (thành công):
```
// HTTP 200 OK
{
  "user_id": 1,
  "ten_dang_nhap": "admin01",
  "ho_ten": "Nguyễn Văn A",
  "email": "admin@gmail.com",
  "so_dien_thoai": "0901234567",
  "ngay_sinh": "2000-01-15",
  "gioi_tinh": "Nam",
  "cccd": "079123456789",
  "dia_chi": "TP.HCM",
  "anh_chan_dung": "https://example.com/avatar.jpg",
  "role": "HOC_VIEN",
  "trang_thai": "HOAT_DONG",
  "created_at": "2026-04-01T08:00:00Z"
}
```

#### `POST /api/users` - Tạo người dùng (Admin tạo thủ công)
**Auth:** Admin

##### Request Body:
| Trường | Kiểu | Bắt buộc | Mô tả |
| --- | --- | --- | --- |
| ten_dang_nhap | String | ✔ Có | Tên đăng nhập |
| mat_khau | String | ✔ Có | Mật khẩu khởi tạo |
| email | String | ✔ Có | Email |
| ho_ten | String | ✔ Có | Họ và tên |
| so_dien_thoai | String | Không | SĐT |
| role_id | int | ✔ Có | ID vai trò |

##### Response (thành công):
```
// HTTP 201 Created
{ "user_id": 2, "message": "Tạo người dùng thành công" }
```

#### `PUT /api/users/{id}` - Cập nhật thông tin người dùng
**Auth:** Admin

##### Request Body (JSON):
```
{
  "ho_ten": "Trần Thị B",
  "email": "b@gmail.com",
  "so_dien_thoai": "0912345678",
  "dia_chi": "Hà Nội",
  "anh_chan_dung": "https://example.com/b.jpg"
}
```

##### Response (thành công):
```
// HTTP 200 OK
{ "user_id": 2, "message": "Cập nhật thành công" }
```

#### `PUT /api/users/{id}/status` - Cập nhật trạng thái người dùng (khoá/mở)
**Auth:** Admin

##### Request Body (JSON):
```
{ "trang_thai": "BI_KHOA" }
```

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Cập nhật trạng thái thành công" }
```

#### `DELETE /api/users/{id}` - Xoá / ngưng hoạt động người dùng
**Auth:** Admin

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Người dùng đã bị xoá khỏi hệ thống" }
```

### Vai trò (Roles)

#### `GET /api/roles` - Lấy danh sách vai trò
**Auth:** Admin

##### Response (thành công):
```
// HTTP 200 OK
[
  { "role_id": 1, "ten_vai_tro": "ADMIN",    "mo_ta": "Quản trị hệ thống" },
  { "role_id": 2, "ten_vai_tro": "GIAO_VIEN","mo_ta": "Giáo viên hướng dẫn" },
  { "role_id": 3, "ten_vai_tro": "HOC_VIEN", "mo_ta": "Học viên đăng ký học" }
]
```

#### `POST /api/roles` - Tạo vai trò mới
**Auth:** Admin

##### Request Body (JSON):
```
{ "ten_vai_tro": "KE_TOAN", "mo_ta": "Kế toán thu phí" }
```

##### Response (thành công):
```
// HTTP 201 Created
{ "role_id": 4, "ten_vai_tro": "KE_TOAN", "mo_ta": "Kế toán thu phí" }
```

#### `PUT /api/roles/{id}` - Cập nhật vai trò
**Auth:** Admin

##### Request Body (JSON):
```
{ "ten_vai_tro": "KE_TOAN_TRUONG", "mo_ta": "Kế toán trưởng" }
```

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Cập nhật vai trò thành công" }
```

#### `DELETE /api/roles/{id}` - Xoá vai trò
**Auth:** Admin

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Xoá vai trò thành công" }
```

#### `GET /api/users/{id}/roles` - Lấy danh sách vai trò của user
**Auth:** Admin

##### Response (thành công):
```
// HTTP 200 OK
[{ "role_id": 3, "ten_vai_tro": "HOC_VIEN" }]
```

#### `POST /api/users/{id}/roles` - Gán vai trò cho user
**Auth:** Admin

##### Request Body (JSON):
```
{ "role_id": 2 }
```

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Gán vai trò thành công" }
```

#### `DELETE /api/users/{id}/roles/{roleId}` - Gỡ vai trò khỏi user
**Auth:** Admin

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Gỡ vai trò thành công" }
```

### Nhật ký hệ thống

#### `GET /api/system-logs` - Lấy danh sách nhật ký
**Auth:** Admin

##### Query Params
| Param | Kiểu | Mặc định | Mô tả |
| --- | --- | --- | --- |
| page | int | 0 | Trang |
| size | int | 20 | Kích thước |
| user_id | int |  | Lọc theo user |
| action | String |  | Loại hành động |
| from_date | String |  | Từ ngày (YYYY-MM-DD) |
| to_date | String |  | Đến ngày (YYYY-MM-DD) |

##### Response (thành công):
```
// HTTP 200 OK
{
  "content": [
    {
      "log_id": 1,
      "user_id": 1,
      "ho_ten": "Nguyễn Văn A",
      "action": "LOGIN",
      "mo_ta": "Đăng nhập thành công",
      "ip_address": "127.0.0.1",
      "created_at": "2026-04-01T08:00:00Z"
    }
  ],
  "totalElements": 200,
  "totalPages": 10
}
```

## 3. Quản lý học viên

#### `GET /api/students` - Lấy danh sách học viên
**Auth:** Admin / Giáo viên

##### Query Params
| Param | Kiểu | Mặc định | Mô tả |
| --- | --- | --- | --- |
| page | int | 0 | Trang |
| size | int | 10 | Kích thước |
| search | String |  | Tìm theo tên, CCCD, SĐT |
| status | String |  | HOAT_DONG / BI_KHOA |

##### Response (thành công):
```
// HTTP 200 OK
{
  "content": [
    {
      "student_id": 1,
      "user_id": 5,
      "ho_ten": "Lê Văn C",
      "email": "c@gmail.com",
      "so_dien_thoai": "0901111111",
      "ngay_sinh": "2000-05-10",
      "cccd": "079000000001",
      "trang_thai": "HOAT_DONG"
    }
  ],
  "totalElements": 120,
  "totalPages": 12
}
```

#### `GET /api/students/{id}` - Lấy chi tiết học viên
**Auth:** Admin / Giáo viên

##### Response (thành công):
```
// HTTP 200 OK
{
  "student_id": 1,
  "user_id": 5,
  "ho_ten": "Lê Văn C",
  "email": "c@gmail.com",
  "so_dien_thoai": "0901111111",
  "ngay_sinh": "2000-05-10",
  "gioi_tinh": "Nam",
  "cccd": "079000000001",
  "dia_chi": "Bình Dương",
  "anh_chan_dung": "https://example.com/c.jpg",
  "trang_thai": "HOAT_DONG",
  "created_at": "2026-03-01T00:00:00Z"
}
```

#### `GET /api/students/me` - Học viên xem hồ sơ của mình
**Auth:** Học viên (Bearer JWT)

##### Response (thành công):
```
// HTTP 200 OK
{
  "student_id": 1,
  "ho_ten": "Lê Văn C",
  "email": "c@gmail.com",
  "so_dien_thoai": "0901111111",
  "ngay_sinh": "2000-05-10",
  "dia_chi": "Bình Dương",
  "anh_chan_dung": "https://example.com/c.jpg"
}
```

#### `PUT /api/students/me` - Học viên tự cập nhật hồ sơ
**Auth:** Học viên

##### Request Body (JSON):
```
{
  "so_dien_thoai": "0909999999",
  "dia_chi": "TP.HCM",
  "anh_chan_dung": "https://example.com/new.jpg"
}
```

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Cập nhật hồ sơ thành công" }
```

#### `POST /api/students` - Tạo học viên (Admin)
**Auth:** Admin
> Tạo đồng thời user + student. Gửi thông tin đầy đủ giống /register.

##### Request Body (JSON):
```
{
  "ten_dang_nhap": "hocvien01",
  "mat_khau": "123456",
  "email": "hv01@gmail.com",
  "ho_ten": "Phạm Văn D",
  "so_dien_thoai": "0903456789",
  "ngay_sinh": "2001-08-20",
  "gioi_tinh": "Nam",
  "cccd": "079000000099",
  "dia_chi": "Đồng Nai"
}
```

##### Response (thành công):
```
// HTTP 201 Created
{ "student_id": 10, "user_id": 15, "message": "Tạo học viên thành công" }
```

#### `PUT /api/students/{id}` - Cập nhật học viên (Admin)
**Auth:** Admin

##### Request Body (JSON):
```
{ "so_dien_thoai": "0908888888", "dia_chi": "Long An" }
```

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Cập nhật học viên thành công" }
```

#### `DELETE /api/students/{id}` - Xoá học viên
**Auth:** Admin

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Xoá học viên thành công" }
```

## 4. Hồ sơ đăng ký & giấy tờ

#### `GET /api/registration-profiles` - Lấy danh sách hồ sơ đăng ký
**Auth:** Admin

##### Query Params
| Param | Kiểu | Mặc định | Mô tả |
| --- | --- | --- | --- |
| page | int | 0 | Trang |
| size | int | 10 | Kích thước |
| trang_thai | String |  | CHO_DUYET / DA_DUYET / TU_CHOI |
| student_id | int |  | Lọc theo học viên |

##### Response (thành công):
```
// HTTP 200 OK
{
  "content": [
    {
      "profile_id": 1,
      "student_id": 1,
      "ho_ten": "Lê Văn C",
      "loai_bang_lai": "B2",
      "trang_thai": "CHO_DUYET",
      "ngay_nop": "2026-03-15T00:00:00Z"
    }
  ],
  "totalElements": 30
}
```

#### `GET /api/registration-profiles/{id}` - Lấy chi tiết hồ sơ
**Auth:** Admin / Học viên sở hữu

##### Response (thành công):
```
// HTTP 200 OK
{
  "profile_id": 1,
  "student_id": 1,
  "loai_bang_lai": "B2",
  "trang_thai": "CHO_DUYET",
  "ghi_chu": "",
  "ngay_nop": "2026-03-15T00:00:00Z",
  "ngay_duyet": null,
  "nguoi_duyet_id": null,
  "documents": [
    { "doc_id": 1, "loai_giay_to": "CCCD", "url": "https://..." }
  ]
}
```

#### `POST /api/registration-profiles` - Nộp hồ sơ đăng ký
**Auth:** Học viên

##### Request Body (JSON):
```
{ "student_id": 1, "loai_bang_lai": "B2", "ghi_chu": "Học lần 2" }
```

##### Response (thành công):
```
// HTTP 201 Created
{ "profile_id": 5, "trang_thai": "CHO_DUYET", "message": "Nộp hồ sơ thành công" }
```

#### `PUT /api/registration-profiles/{id}/approve` - Duyệt hồ sơ
**Auth:** Admin

##### Request Body (JSON):
```
{ "ghi_chu": "Hồ sơ hợp lệ" }
```

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Duyệt hồ sơ thành công", "trang_thai": "DA_DUYET" }
```

#### `PUT /api/registration-profiles/{id}/reject` - Từ chối hồ sơ
**Auth:** Admin

##### Request Body (JSON):
```
{ "ly_do": "Thiếu ảnh CCCD mặt sau" }
```

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Đã từ chối hồ sơ", "trang_thai": "TU_CHOI" }
```

#### `GET /api/registration-profiles/{id}/documents` - Lấy danh sách giấy tờ của hồ sơ
**Auth:** Admin / Học viên sở hữu

##### Response (thành công):
```
// HTTP 200 OK
[
  { "doc_id": 1, "loai_giay_to": "CCCD_MAT_TRUOC", "url": "https://...", "uploaded_at": "2026-03-15T00:00:00Z" },
  { "doc_id": 2, "loai_giay_to": "CCCD_MAT_SAU",   "url": "https://...", "uploaded_at": "2026-03-15T00:00:00Z" }
]
```

#### `POST /api/registration-profiles/{id}/documents` - Upload giấy tờ
**Auth:** Học viên sở hữu
> Content-Type: multipart/form-data. Gửi file dạng form-data với key "file" và field "loai_giay_to".

##### Request Body (JSON):
```
// multipart/form-data
{
  "loai_giay_to": "CCCD_MAT_TRUOC",
  "file": <binary>
}
```

##### Response (thành công):
```
// HTTP 201 Created
{
  "doc_id": 3,
  "loai_giay_to": "CCCD_MAT_TRUOC",
  "url": "https://storage.example.com/docs/abc.jpg",
  "uploaded_at": "2026-04-01T09:00:00Z"
}
```

#### `DELETE /api/documents/{id}` - Xoá giấy tờ
**Auth:** Admin / Học viên sở hữu

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Xoá giấy tờ thành công" }
```

## 5. Quản lý khóa học

#### `GET /api/courses` - Lấy danh sách khóa học
**Auth:** Tất cả

##### Query Params
| Param | Kiểu | Mặc định | Mô tả |
| --- | --- | --- | --- |
| page | int | 0 | Trang |
| size | int | 10 | Kích thước |
| trang_thai | String |  | DANG_MO / DONG / SAP_MO |

##### Response (thành công):
```
// HTTP 200 OK
{
  "content": [
    {
      "course_id": 1,
      "ten_khoa_hoc": "Khóa B2 - Tháng 4/2026",
      "loai_bang_lai": "B2",
      "hoc_phi": 8500000,
      "so_buoi_hoc": 30,
      "thoi_gian_bat_dau": "2026-04-10",
      "thoi_gian_ket_thuc": "2026-06-10",
      "trang_thai": "DANG_MO",
      "so_luong_toi_da": 30,
      "so_luong_hien_tai": 15
    }
  ],
  "totalElements": 10
}
```

#### `POST /api/courses` - Tạo khóa học mới
**Auth:** Admin

##### Request Body:
| Trường | Kiểu | Bắt buộc | Mô tả |
| --- | --- | --- | --- |
| ten_khoa_hoc | String | ✔ Có | Tên khóa học |
| loai_bang_lai | String | ✔ Có | A1 / A2 / B1 / B2 / C |
| hoc_phi | long | ✔ Có | Học phí (VND) |
| so_buoi_hoc | int | ✔ Có | Tổng số buổi học |
| thoi_gian_bat_dau | String | ✔ Có | Ngày khai giảng YYYY-MM-DD |
| thoi_gian_ket_thuc | String | ✔ Có | Ngày kết thúc YYYY-MM-DD |
| so_luong_toi_da | int | ✔ Có | Số lượng học viên tối đa |

##### Response (thành công):
```
// HTTP 201 Created
{ "course_id": 5, "message": "Tạo khóa học thành công" }
```

#### `PUT /api/courses/{id}` - Cập nhật khóa học
**Auth:** Admin

##### Request Body (JSON):
```
{ "hoc_phi": 9000000, "so_luong_toi_da": 35 }
```

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Cập nhật khóa học thành công" }
```

#### `PUT /api/courses/{id}/status` - Cập nhật trạng thái khóa học
**Auth:** Admin

##### Request Body (JSON):
```
{ "trang_thai": "DONG" }
```

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Cập nhật trạng thái thành công" }
```

## 6. Quản lý lớp học

#### `GET /api/classes` - Lấy danh sách lớp học
**Auth:** Admin / Giáo viên

##### Response (thành công):
```
// HTTP 200 OK
{
  "content": [
    {
      "class_id": 1,
      "ten_lop": "B2-04-2026",
      "course_id": 1,
      "ten_khoa_hoc": "Khóa B2 - Tháng 4/2026",
      "giao_vien_id": 2,
      "ten_giao_vien": "Nguyễn Văn Thầy",
      "si_so": 15,
      "trang_thai": "DANG_HOC"
    }
  ]
}
```

#### `POST /api/classes` - Tạo lớp học
**Auth:** Admin

##### Request Body (JSON):
```
{
  "ten_lop": "B2-04-2026",
  "course_id": 1,
  "giao_vien_id": 2,
  "phong_hoc": "P.101",
  "si_so_toi_da": 30
}
```

##### Response (thành công):
```
// HTTP 201 Created
{ "class_id": 3, "message": "Tạo lớp học thành công" }
```

#### `GET /api/classes/{id}/students` - Lấy danh sách học viên trong lớp
**Auth:** Admin / Giáo viên

##### Response (thành công):
```
// HTTP 200 OK
[
  { "student_id": 1, "ho_ten": "Lê Văn C", "email": "c@gmail.com", "trang_thai_lop": "DANG_HOC" }
]
```

#### `POST /api/classes/{id}/students` - Thêm học viên vào lớp
**Auth:** Admin

##### Request Body (JSON):
```
{ "student_id": 5 }
```

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Thêm học viên vào lớp thành công" }
```

#### `DELETE /api/classes/{id}/students/{studentId}` - Xoá học viên khỏi lớp
**Auth:** Admin

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Xoá học viên khỏi lớp thành công" }
```

## 7. Đăng ký khóa học

#### `GET /api/course-registrations` - Lấy danh sách đăng ký khóa học
**Auth:** Admin

##### Query Params
| Param | Kiểu | Mặc định | Mô tả |
| --- | --- | --- | --- |
| student_id | int |  | Lọc theo học viên |
| course_id | int |  | Lọc theo khóa học |
| trang_thai | String |  | CHO_DUYET / DA_DUYET / TU_CHOI |

##### Response (thành công):
```
// HTTP 200 OK
{
  "content": [
    {
      "registration_id": 1,
      "student_id": 1,
      "ho_ten": "Lê Văn C",
      "course_id": 1,
      "ten_khoa_hoc": "Khóa B2 - Tháng 4/2026",
      "trang_thai": "CHO_DUYET",
      "ngay_dang_ky": "2026-04-01T00:00:00Z"
    }
  ]
}
```

#### `POST /api/course-registrations` - Đăng ký khóa học
**Auth:** Học viên

##### Request Body (JSON):
```
{ "student_id": 1, "course_id": 1, "ghi_chu": "Muốn học buổi chiều" }
```

##### Response (thành công):
```
// HTTP 201 Created
{ "registration_id": 10, "trang_thai": "CHO_DUYET", "message": "Đăng ký thành công, chờ duyệt" }
```

#### `PUT /api/course-registrations/{id}/approve` - Duyệt đăng ký khóa học
**Auth:** Admin

##### Request Body (JSON):
```
{ "class_id": 1, "ghi_chu": "Xếp vào lớp B2-04-2026" }
```

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Duyệt đăng ký thành công", "trang_thai": "DA_DUYET" }
```

#### `PUT /api/course-registrations/{id}/reject` - Từ chối đăng ký khóa học
**Auth:** Admin

##### Request Body (JSON):
```
{ "ly_do": "Khóa học đã đủ chỗ" }
```

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Đã từ chối đăng ký", "trang_thai": "TU_CHOI" }
```

## 8. Buổi học & điểm danh

#### `GET /api/classes/{id}/sessions` - Lấy danh sách buổi học của lớp
**Auth:** Admin / Giáo viên / Học viên trong lớp

##### Response (thành công):
```
// HTTP 200 OK
[
  {
    "session_id": 1,
    "class_id": 1,
    "ngay_hoc": "2026-04-10",
    "gio_bat_dau": "08:00",
    "gio_ket_thuc": "11:00",
    "dia_diem": "Sân tập số 1",
    "noi_dung": "Bài 1: Làm quen xe",
    "trang_thai": "CHUA_HOC"
  }
]
```

#### `POST /api/classes/{id}/sessions` - Tạo buổi học
**Auth:** Admin / Giáo viên

##### Request Body (JSON):
```
{
  "ngay_hoc": "2026-04-10",
  "gio_bat_dau": "08:00",
  "gio_ket_thuc": "11:00",
  "dia_diem": "Sân tập số 1",
  "noi_dung": "Bài 1: Làm quen xe"
}
```

##### Response (thành công):
```
// HTTP 201 Created
{ "session_id": 5, "message": "Tạo buổi học thành công" }
```

#### `GET /api/sessions/{id}/attendance` - Lấy danh sách điểm danh buổi học
**Auth:** Admin / Giáo viên

##### Response (thành công):
```
// HTTP 200 OK
[
  { "student_id": 1, "ho_ten": "Lê Văn C", "trang_thai": "CO_MAT", "ghi_chu": "" },
  { "student_id": 2, "ho_ten": "Phạm Văn D", "trang_thai": "VANG", "ghi_chu": "Nghỉ bệnh" }
]
```

#### `POST /api/sessions/{id}/attendance` - Tạo điểm danh cho cả buổi học
**Auth:** Giáo viên
> Gọi 1 lần để tạo record điểm danh cho tất cả học viên trong lớp (mặc định: CHUA_DIEM_DANH).

##### Response (thành công):
```
// HTTP 201 Created
{ "message": "Khởi tạo điểm danh thành công", "so_hoc_vien": 15 }
```

#### `PUT /api/sessions/{id}/attendance/{studentId}` - Cập nhật trạng thái điểm danh
**Auth:** Giáo viên
> trang_thai: CO_MAT | VANG | VANG_PHEP | TRE

##### Request Body (JSON):
```
{ "trang_thai": "CO_MAT", "ghi_chu": "" }
```

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Cập nhật điểm danh thành công" }
```

#### `GET /api/students/{id}/attendance` - Lịch sử điểm danh của học viên
**Auth:** Admin / Giáo viên / Học viên sở hữu

##### Response (thành công):
```
// HTTP 200 OK
{
  "student_id": 1,
  "tong_buoi": 30,
  "co_mat": 25,
  "vang": 3,
  "vang_phep": 2,
  "chi_tiet": [
    { "session_id": 1, "ngay_hoc": "2026-04-10", "trang_thai": "CO_MAT" }
  ]
}
```

## 9. Ngân hàng câu hỏi

#### `GET /api/question-topics` - Lấy danh sách chủ đề câu hỏi
**Auth:** Admin / Giáo viên

##### Response (thành công):
```
// HTTP 200 OK
[
  { "topic_id": 1, "ten_chu_de": "Luật Giao thông đường bộ", "so_cau_hoi": 50 },
  { "topic_id": 2, "ten_chu_de": "Biển báo giao thông",      "so_cau_hoi": 40 }
]
```

#### `POST /api/question-topics` - Tạo chủ đề câu hỏi
**Auth:** Admin

##### Request Body (JSON):
```
{ "ten_chu_de": "Sa hình lái xe", "mo_ta": "Các bài thi sa hình thực hành" }
```

##### Response (thành công):
```
// HTTP 201 Created
{ "topic_id": 5, "ten_chu_de": "Sa hình lái xe" }
```

#### `GET /api/questions` - Lấy danh sách câu hỏi
**Auth:** Admin / Giáo viên

##### Query Params
| Param | Kiểu | Mặc định | Mô tả |
| --- | --- | --- | --- |
| page | int | 0 | Trang |
| size | int | 20 | Kích thước |
| topic_id | int |  | Lọc theo chủ đề |
| do_kho | String |  | DE / TRUNG_BINH / KHO |
| trang_thai | String |  | HOAT_DONG / TAM_KHOA |

##### Response (thành công):
```
// HTTP 200 OK
{
  "content": [
    {
      "question_id": 1,
      "noi_dung": "Tốc độ tối đa trong khu dân cư là bao nhiêu?",
      "topic_id": 1,
      "ten_chu_de": "Luật GTĐB",
      "do_kho": "DE",
      "la_cau_diem_liet": false,
      "trang_thai": "HOAT_DONG"
    }
  ],
  "totalElements": 500
}
```

#### `GET /api/questions/{id}` - Lấy chi tiết câu hỏi kèm đáp án
**Auth:** Admin / Giáo viên

##### Response (thành công):
```
// HTTP 200 OK
{
  "question_id": 1,
  "noi_dung": "Tốc độ tối đa trong khu dân cư là bao nhiêu?",
  "hinh_anh": null,
  "do_kho": "DE",
  "la_cau_diem_liet": false,
  "answers": [
    { "answer_id": 1, "noi_dung": "30 km/h", "la_dap_an_dung": false },
    { "answer_id": 2, "noi_dung": "50 km/h", "la_dap_an_dung": true },
    { "answer_id": 3, "noi_dung": "70 km/h", "la_dap_an_dung": false },
    { "answer_id": 4, "noi_dung": "80 km/h", "la_dap_an_dung": false }
  ]
}
```

#### `POST /api/questions` - Tạo câu hỏi mới
**Auth:** Admin / Giáo viên

##### Request Body (JSON):
```
{
  "noi_dung": "Biển báo P.102 có ý nghĩa gì?",
  "topic_id": 2,
  "do_kho": "TRUNG_BINH",
  "la_cau_diem_liet": false,
  "answers": [
    { "noi_dung": "Cấm đỗ xe",     "la_dap_an_dung": false },
    { "noi_dung": "Cấm dừng xe",   "la_dap_an_dung": false },
    { "noi_dung": "Cấm đi ngược chiều", "la_dap_an_dung": true },
    { "noi_dung": "Đường cấm",     "la_dap_an_dung": false }
  ]
}
```

##### Response (thành công):
```
// HTTP 201 Created
{ "question_id": 100, "message": "Tạo câu hỏi thành công" }
```

#### `POST /api/questions/{id}/answers` - Thêm đáp án vào câu hỏi
**Auth:** Admin / Giáo viên

##### Request Body (JSON):
```
{ "noi_dung": "Cấm rẽ trái", "la_dap_an_dung": false }
```

##### Response (thành công):
```
// HTTP 201 Created
{ "answer_id": 10, "message": "Thêm đáp án thành công" }
```

## 10. Ôn tập (Practice)

#### `POST /api/practice-sessions/start` - Bắt đầu phiên ôn tập
**Auth:** Học viên

##### Request Body (JSON):
```
{
  "topic_id": 1,
  "so_cau_hoi": 25,
  "ghi_chu": "Ôn tập buổi tối"
}
```

##### Response (thành công):
```
// HTTP 201 Created
{
  "practice_session_id": 10,
  "trang_thai": "DANG_LAM",
  "thoi_gian_bat_dau": "2026-04-01T20:00:00Z",
  "so_cau_hoi": 25
}
```

#### `GET /api/practice-sessions/{id}/questions` - Lấy danh sách câu hỏi trong phiên ôn tập
**Auth:** Học viên sở hữu
> Trả về câu hỏi đã được xáo trộn ngẫu nhiên. Không trả về trường la_dap_an_dung.

##### Response (thành công):
```
// HTTP 200 OK
[
  {
    "stt": 1,
    "question_id": 5,
    "noi_dung": "Tốc độ tối đa trên cao tốc là bao nhiêu?",
    "la_cau_diem_liet": false,
    "answers": [
      { "answer_id": 10, "noi_dung": "100 km/h" },
      { "answer_id": 11, "noi_dung": "120 km/h" },
      { "answer_id": 12, "noi_dung": "90 km/h"  },
      { "answer_id": 13, "noi_dung": "80 km/h"  }
    ]
  }
]
```

#### `POST /api/practice-sessions/{id}/answers` - Nộp đáp án từng câu
**Auth:** Học viên sở hữu

##### Request Body (JSON):
```
{ "question_id": 5, "answer_id": 11 }
```

##### Response (thành công):
```
// HTTP 200 OK
{
  "la_dung": true,
  "dap_an_dung_id": 11,
  "giai_thich": "120 km/h là tốc độ tối đa trên đường cao tốc."
}
```

#### `POST /api/practice-sessions/{id}/submit` - Nộp phiên ôn tập
**Auth:** Học viên sở hữu

##### Response (thành công):
```
// HTTP 200 OK
{
  "so_cau_dung": 22,
  "so_cau_sai": 3,
  "diem": 88,
  "thoi_gian_lam_bai": "00:18:45",
  "ket_qua": "DAT"
}
```

#### `GET /api/practice-sessions/my-history` - Lịch sử ôn tập của học viên
**Auth:** Học viên

##### Response (thành công):
```
// HTTP 200 OK
[
  {
    "practice_session_id": 10,
    "topic": "Luật GTĐB",
    "so_cau": 25,
    "so_dung": 22,
    "diem": 88,
    "ngay_on_tap": "2026-04-01T20:00:00Z"
  }
]
```

## 11. Thi cử (Exam)

#### `GET /api/exams` - Lấy danh sách kỳ thi
**Auth:** Tất cả

##### Response (thành công):
```
// HTTP 200 OK
{
  "content": [
    {
      "exam_id": 1,
      "ten_ky_thi": "Kỳ thi B2 - Tháng 5/2026",
      "loai_bang_lai": "B2",
      "ngay_thi": "2026-05-15",
      "dia_diem": "Trung tâm sát hạch Đồng Nai",
      "trang_thai": "SAP_DIEN_RA"
    }
  ]
}
```

#### `POST /api/exams` - Tạo kỳ thi
**Auth:** Admin

##### Request Body (JSON):
```
{
  "ten_ky_thi": "Kỳ thi B2 - Tháng 5/2026",
  "loai_bang_lai": "B2",
  "ngay_thi": "2026-05-15",
  "dia_diem": "Trung tâm sát hạch Đồng Nai",
  "ghi_chu": "Thi lý thuyết + sa hình"
}
```

##### Response (thành công):
```
// HTTP 201 Created
{ "exam_id": 3, "message": "Tạo kỳ thi thành công" }
```

#### `GET /api/exam-shifts` - Lấy danh sách ca thi
**Auth:** Admin

##### Query Params
| Param | Kiểu | Mặc định | Mô tả |
| --- | --- | --- | --- |
| exam_id | int |  | Lọc theo kỳ thi |

##### Response (thành công):
```
// HTTP 200 OK
[
  {
    "shift_id": 1,
    "exam_id": 1,
    "ten_ca": "Ca sáng",
    "gio_bat_dau": "08:00",
    "gio_ket_thuc": "10:00",
    "so_thi_sinh_toi_da": 30
  }
]
```

#### `POST /api/exam-shifts` - Tạo ca thi
**Auth:** Admin

##### Request Body (JSON):
```
{
  "exam_id": 1,
  "ten_ca": "Ca chiều",
  "gio_bat_dau": "13:00",
  "gio_ket_thuc": "15:00",
  "so_thi_sinh_toi_da": 25,
  "exam_paper_id": 2
}
```

##### Response (thành công):
```
// HTTP 201 Created
{ "shift_id": 3, "message": "Tạo ca thi thành công" }
```

#### `GET /api/exam-papers` - Lấy danh sách đề thi
**Auth:** Admin

##### Response (thành công):
```
// HTTP 200 OK
[
  {
    "paper_id": 1,
    "ten_de_thi": "Đề B2 - Số 01",
    "loai_bang_lai": "B2",
    "so_cau": 35,
    "thoi_gian_thi_phut": 19,
    "diem_dat": 32,
    "trang_thai": "HOAT_DONG"
  }
]
```

#### `POST /api/exam-papers` - Tạo đề thi
**Auth:** Admin

##### Request Body (JSON):
```
{
  "ten_de_thi": "Đề B2 - Số 02",
  "loai_bang_lai": "B2",
  "thoi_gian_thi_phut": 19,
  "diem_dat": 32,
  "ghi_chu": "Đề thi chính thức"
}
```

##### Response (thành công):
```
// HTTP 201 Created
{ "paper_id": 5, "message": "Tạo đề thi thành công" }
```

#### `POST /api/exam-papers/{id}/questions` - Thêm câu hỏi vào đề thi
**Auth:** Admin

##### Request Body (JSON):
```
{ "question_id": 42, "thu_tu": 10 }
```

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Thêm câu hỏi vào đề thi thành công" }
```

## 12. Đăng ký dự thi

#### `POST /api/exam-registrations` - Đăng ký dự thi
**Auth:** Học viên

##### Request Body (JSON):
```
{
  "student_id": 1,
  "exam_id": 1,
  "shift_id": 1,
  "ghi_chu": "Đăng ký ca sáng"
}
```

##### Response (thành công):
```
// HTTP 201 Created
{
  "registration_id": 8,
  "trang_thai": "CHO_DUYET",
  "message": "Đăng ký dự thi thành công, chờ xét duyệt"
}
```

#### `PUT /api/exam-registrations/{id}/approve` - Duyệt đăng ký dự thi
**Auth:** Admin

##### Request Body (JSON):
```
{ "so_bao_danh": "B2-2026-001", "ghi_chu": "" }
```

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Duyệt thành công", "so_bao_danh": "B2-2026-001" }
```

#### `PUT /api/exam-registrations/{id}/reject` - Từ chối đăng ký dự thi
**Auth:** Admin

##### Request Body (JSON):
```
{ "ly_do": "Chưa hoàn thành học phí" }
```

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Đã từ chối đăng ký", "trang_thai": "TU_CHOI" }
```

## 13. Bài thi & kết quả

#### `POST /api/test-attempts/start` - Bắt đầu bài thi chính thức
**Auth:** Học viên (đã đăng ký ca thi)

##### Request Body (JSON):
```
{ "exam_registration_id": 8 }
```

##### Response (thành công):
```
// HTTP 201 Created
{
  "attempt_id": 5,
  "thoi_gian_bat_dau": "2026-05-15T08:00:00Z",
  "thoi_gian_ket_thuc": "2026-05-15T08:19:00Z",
  "so_cau": 35,
  "trang_thai": "DANG_THI"
}
```

#### `GET /api/test-attempts/{id}/questions` - Lấy đề thi (câu hỏi trong bài thi)
**Auth:** Học viên sở hữu bài thi
> Không trả về la_dap_an_dung. Câu hỏi điểm liệt được đánh dấu la_cau_diem_liet: true.

##### Response (thành công):
```
// HTTP 200 OK
{
  "attempt_id": 5,
  "thoi_gian_con_lai_giay": 847,
  "questions": [
    {
      "stt": 1,
      "question_id": 10,
      "noi_dung": "...",
      "la_cau_diem_liet": true,
      "answers": [
        { "answer_id": 40, "noi_dung": "Phương án A" },
        { "answer_id": 41, "noi_dung": "Phương án B" }
      ]
    }
  ]
}
```

#### `POST /api/test-attempts/{id}/answers` - Nộp đáp án từng câu (trong lúc thi)
**Auth:** Học viên sở hữu
> Frontend gọi API này mỗi khi học viên chọn đáp án. Không trả về đúng/sai trong lúc thi.

##### Request Body (JSON):
```
{ "question_id": 10, "answer_id": 41 }
```

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Đã lưu đáp án" }
```

#### `POST /api/test-attempts/{id}/submit` - Nộp bài thi
**Auth:** Học viên sở hữu

##### Response (thành công):
```
// HTTP 200 OK
{
  "so_cau_dung": 33,
  "so_cau_sai": 2,
  "diem": 33,
  "diem_dat": 32,
  "co_cau_diem_liet_sai": false,
  "ket_qua": "DAT",
  "thoi_gian_lam_bai": "00:15:32"
}
```

#### `GET /api/test-attempts/{id}/result` - Xem kết quả chi tiết bài thi
**Auth:** Admin / Học viên sở hữu

##### Response (thành công):
```
// HTTP 200 OK
{
  "attempt_id": 5,
  "ho_ten": "Lê Văn C",
  "ten_ky_thi": "Kỳ thi B2 - Tháng 5/2026",
  "so_cau_dung": 33,
  "diem": 33,
  "ket_qua": "DAT",
  "chi_tiet": [
    {
      "question_id": 10,
      "noi_dung": "...",
      "dap_an_chon_id": 41,
      "dap_an_dung_id": 41,
      "la_dung": true,
      "la_cau_diem_liet": true
    }
  ]
}
```

## 14. Tài chính (Phiếu thu)

#### `GET /api/fee-types` - Lấy danh sách loại khoản thu
**Auth:** Admin

##### Response (thành công):
```
// HTTP 200 OK
[
  { "fee_type_id": 1, "ten_khoang_thu": "Học phí",     "mo_ta": "Phí học lái xe" },
  { "fee_type_id": 2, "ten_khoang_thu": "Lệ phí thi",  "mo_ta": "Phí đăng ký dự thi" },
  { "fee_type_id": 3, "ten_khoang_thu": "Phí hồ sơ",   "mo_ta": "Phí làm hồ sơ" }
]
```

#### `GET /api/receipts` - Lấy danh sách phiếu thu
**Auth:** Admin

##### Query Params
| Param | Kiểu | Mặc định | Mô tả |
| --- | --- | --- | --- |
| student_id | int |  | Lọc theo học viên |
| trang_thai | String |  | CHO_XAC_NHAN / DA_XAC_NHAN |
| from_date | String |  | Từ ngày YYYY-MM-DD |
| to_date | String |  | Đến ngày YYYY-MM-DD |

##### Response (thành công):
```
// HTTP 200 OK
{
  "content": [
    {
      "receipt_id": 1,
      "student_id": 1,
      "ho_ten": "Lê Văn C",
      "tong_tien": 8500000,
      "trang_thai": "DA_XAC_NHAN",
      "ngay_thu": "2026-04-01",
      "nguoi_thu": "Nguyễn Admin"
    }
  ]
}
```

#### `POST /api/receipts` - Tạo phiếu thu mới
**Auth:** Admin

##### Request Body (JSON):
```
{
  "student_id": 1,
  "ghi_chu": "Thu học phí khóa B2",
  "items": [
    { "fee_type_id": 1, "so_tien": 8500000, "mo_ta": "Học phí khóa B2 tháng 4/2026" }
  ]
}
```

##### Response (thành công):
```
// HTTP 201 Created
{ "receipt_id": 10, "tong_tien": 8500000, "message": "Tạo phiếu thu thành công" }
```

#### `PUT /api/receipts/{id}/confirm` - Xác nhận phiếu thu (đã thu tiền)
**Auth:** Admin

##### Request Body (JSON):
```
{ "phuong_thuc": "TIEN_MAT", "ghi_chu": "Đã nhận đủ tiền mặt" }
```

##### Response (thành công):
```
// HTTP 200 OK
{ "message": "Xác nhận phiếu thu thành công", "trang_thai": "DA_XAC_NHAN" }
```

## 15. Vi phạm quy chế

#### `GET /api/violations` - Lấy danh sách vi phạm quy chế
**Auth:** Admin / Giáo viên

##### Query Params
| Param | Kiểu | Mặc định | Mô tả |
| --- | --- | --- | --- |
| student_id | int |  | Lọc theo học viên |
| violation_type_id | int |  | Lọc theo loại vi phạm |

##### Response (thành công):
```
// HTTP 200 OK
[
  {
    "violation_id": 1,
    "student_id": 1,
    "ho_ten": "Lê Văn C",
    "loai_vi_pham": "Sử dụng điện thoại khi thi",
    "hinh_thuc_xu_ly": "Truất quyền dự thi",
    "ngay_vi_pham": "2026-05-15",
    "nguoi_lap_bien_ban": "Nguyễn Giám Thị"
  }
]
```

#### `POST /api/violations` - Tạo biên bản vi phạm
**Auth:** Admin / Giáo viên

##### Request Body (JSON):
```
{
  "student_id": 1,
  "violation_type_id": 2,
  "mo_ta_chi_tiet": "Thí sinh sử dụng điện thoại trong phòng thi",
  "hinh_thuc_xu_ly": "Truất quyền dự thi",
  "ngay_vi_pham": "2026-05-15"
}
```

##### Response (thành công):
```
// HTTP 201 Created
{ "violation_id": 5, "message": "Lập biên bản vi phạm thành công" }
```

## 16. Dashboard & Báo cáo

#### `GET /api/dashboard/admin` - Dashboard quản trị viên
**Auth:** Admin

##### Response (thành công):
```
// HTTP 200 OK
{
  "tong_hoc_vien": 350,
  "hoc_vien_moi_thang_nay": 28,
  "doanh_thu_thang_nay": 238000000,
  "doanh_thu_thang_truoc": 195000000,
  "ty_le_dat_thi": 78.5,
  "tong_ky_thi_sap_dien_ra": 3,
  "ho_so_cho_duyet": 12,
  "dang_ky_khoa_hoc_cho_duyet": 8
}
```

#### `GET /api/dashboard/teacher` - Dashboard giáo viên
**Auth:** Giáo viên

##### Response (thành công):
```
// HTTP 200 OK
{
  "so_lop_dang_day": 2,
  "tong_hoc_vien": 35,
  "buoi_hoc_hom_nay": 1,
  "ty_le_diem_danh_trung_binh": 92.5,
  "danh_sach_lop": [
    { "class_id": 1, "ten_lop": "B2-04-2026", "si_so": 20, "buoi_hoc_tiep_theo": "2026-04-10 08:00" }
  ]
}
```

#### `GET /api/dashboard/student` - Dashboard học viên
**Auth:** Học viên

##### Response (thành công):
```
// HTTP 200 OK
{
  "ten_khoa_hoc": "Khóa B2 - Tháng 4/2026",
  "tien_do_hoc": 40,
  "so_buoi_da_hoc": 12,
  "tong_so_buoi": 30,
  "ty_le_diem_danh": 91.7,
  "buoi_hoc_tiep_theo": { "ngay": "2026-04-10", "gio": "08:00", "dia_diem": "Sân tập số 1" },
  "diem_on_tap_trung_binh": 82.4,
  "ky_thi_sap_toi": { "ten_ky_thi": "Kỳ thi B2 - Tháng 5/2026", "ngay_thi": "2026-05-15" }
}
```

### Báo cáo

#### `GET /api/reports/students` - Báo cáo học viên
**Auth:** Admin

##### Query Params
| Param | Kiểu | Mặc định | Mô tả |
| --- | --- | --- | --- |
| from_date | String |  | Từ ngày |
| to_date | String |  | Đến ngày |
| loai_bang | String |  | A1/B2/... |

##### Response (thành công):
```
// HTTP 200 OK
{
  "tong_hoc_vien": 350,
  "hoc_vien_moi": 28,
  "hoc_vien_bi_khoa": 5,
  "phan_bo_bang_lai": { "A1": 80, "B1": 120, "B2": 150 },
  "theo_thang": [
    { "thang": "2026-03", "so_dang_ky": 25 },
    { "thang": "2026-04", "so_dang_ky": 28 }
  ]
}
```

#### `GET /api/reports/revenue` - Báo cáo doanh thu
**Auth:** Admin

##### Query Params
| Param | Kiểu | Mặc định | Mô tả |
| --- | --- | --- | --- |
| from_date | String |  | Từ ngày |
| to_date | String |  | Đến ngày |
| nhom_theo | String | month | day / month / quarter |

##### Response (thành công):
```
// HTTP 200 OK
{
  "tong_doanh_thu": 975000000,
  "da_xac_nhan": 850000000,
  "cho_xac_nhan": 125000000,
  "theo_thang": [
    { "period": "2026-02", "doanh_thu": 195000000 },
    { "period": "2026-03", "doanh_thu": 238000000 }
  ],
  "theo_loai": [
    { "fee_type": "Học phí",    "tong": 750000000 },
    { "fee_type": "Lệ phí thi", "tong": 225000000 }
  ]
}
```

#### `GET /api/reports/exam-results` - Báo cáo kết quả thi
**Auth:** Admin

##### Query Params
| Param | Kiểu | Mặc định | Mô tả |
| --- | --- | --- | --- |
| exam_id | int |  | Lọc theo kỳ thi |
| from_date | String |  | Từ ngày |
| to_date | String |  | Đến ngày |

##### Response (thành công):
```
// HTTP 200 OK
{
  "tong_thi_sinh": 120,
  "dat": 94,
  "khong_dat": 26,
  "ty_le_dat": 78.3,
  "diem_trung_binh": 31.2,
  "theo_ky_thi": [
    { "exam_id": 1, "ten_ky_thi": "KT B2 T5/2026", "dat": 50, "khong_dat": 10, "ty_le": 83.3 }
  ]
}
```

#### `GET /api/reports/attendance` - Báo cáo điểm danh
**Auth:** Admin / Giáo viên

##### Query Params
| Param | Kiểu | Mặc định | Mô tả |
| --- | --- | --- | --- |
| class_id | int |  | Lọc theo lớp |
| from_date | String |  | Từ ngày |
| to_date | String |  | Đến ngày |

##### Response (thành công):
```
// HTTP 200 OK
{
  "tong_buoi_hoc": 30,
  "ty_le_co_mat_trung_binh": 91.2,
  "chi_tiet_hoc_vien": [
    { "student_id": 1, "ho_ten": "Lê Văn C", "co_mat": 28, "vang": 2, "ty_le": 93.3 }
  ]
}
```

> Hết tài liệu — Cập nhật: 2026-04-01
