# Tổng quan dự án `webthibanglai`

## 1. Giới thiệu
`webthibanglai` là một dự án website ôn luyện và thi thử bằng lái xe máy, đang được triển khai theo hướng **ASP.NET Core MVC** nhưng hiện vẫn tận dụng đáng kể phần giao diện tĩnh trong thư mục `wwwroot`.

Mục tiêu hiện tại của dự án là:
- Xây dựng website giới thiệu nền tảng ôn thi GPLX xe máy.
- Cung cấp luồng thi thử cho hạng `A1` và `A`.
- Hỗ trợ đăng nhập người dùng/quản trị ở mức frontend.
- Chuẩn bị nền tảng để mở rộng sang backend/API + cơ sở dữ liệu ở giai đoạn sau.

## 2. Công nghệ đang sử dụng
- **.NET 9 / ASP.NET Core MVC** trong [`webthibanglai/webthibanglai.csproj`](webthibanglai/webthibanglai.csproj)
- Cấu hình khởi động MVC trong [`webthibanglai/Program.cs`](webthibanglai/Program.cs:1)
- Razor Views và Partial Views trong thư mục [`webthibanglai/Views`](webthibanglai/Views)
- Tài nguyên tĩnh HTML/CSS/JS trong [`webthibanglai/wwwroot`](webthibanglai/wwwroot)
- Bootstrap, Font Awesome, jQuery, WOW.js, Owl Carousel trong [`webthibanglai/Views/Shared/_Layout.cshtml`](webthibanglai/Views/Shared/_Layout.cshtml:1)

## 3. Cấu trúc chính của dự án
- [`webthibanglai/Program.cs`](webthibanglai/Program.cs:1): cấu hình pipeline ứng dụng MVC.
- [`webthibanglai/Controllers`](webthibanglai/Controllers): các controller điều hướng trang.
- [`webthibanglai/Views`](webthibanglai/Views): giao diện Razor.
- [`webthibanglai/Views/Shared/_Layout.cshtml`](webthibanglai/Views/Shared/_Layout.cshtml:1): layout dùng chung.
- [`webthibanglai/Views/Shared/_Header.cshtml`](webthibanglai/Views/Shared/_Header.cshtml:1): menu điều hướng chính.
- [`webthibanglai/wwwroot/js/exam.js`](webthibanglai/wwwroot/js/exam.js:1): logic thi thử phía frontend.
- [`webthibanglai/wwwroot/js/auth.js`](webthibanglai/wwwroot/js/auth.js:1): logic đăng nhập frontend bằng `localStorage`.
- [`webthibanglai/wwwroot/plans/ke-hoach-he-thong-thi-bang-lai-xe-may.md`](webthibanglai/wwwroot/plans/ke-hoach-he-thong-thi-bang-lai-xe-may.md:1): tài liệu kế hoạch triển khai.

## 4. Chức năng hiện có
### 4.1. Phần website MVC
Các controller hiện tại chủ yếu phục vụ điều hướng tới trang giao diện:
- [`HomeController`](webthibanglai/Controllers/HomeController.cs:7)
- [`AboutController`](webthibanglai/Controllers/AboutController.cs:5)
- [`CoursesController`](webthibanglai/Controllers/CoursesController.cs:5)
- [`AppointmentController`](webthibanglai/Controllers/AppointmentController.cs:5)
- [`ContactController`](webthibanglai/Controllers/ContactController.cs:5)
- [`ExamController`](webthibanglai/Controllers/ExamController.cs:5)
- [`LoginController`](webthibanglai/Controllers/LoginController.cs:5)

Menu chính đã có các mục:
- Trang chủ
- Giới thiệu
- Bộ đề
- Tư vấn
- Liên hệ

Xem tại [`webthibanglai/Views/Shared/_Header.cshtml`](webthibanglai/Views/Shared/_Header.cshtml:28).

### 4.2. Trang chủ
Trang chủ đã được Việt hóa và định hướng đúng chủ đề ôn thi bằng lái xe máy, bao gồm:
- Banner giới thiệu
- Khối lợi ích/hệ thống
- Phần bộ đề nổi bật
- Khối đăng ký tư vấn
- Nội dung marketing cho nền tảng luyện thi

Tham chiếu: [`webthibanglai/Views/Home/Index.cshtml`](webthibanglai/Views/Home/Index.cshtml:1).

### 4.3. Thi thử frontend
Luồng thi thử hiện đã có ở mức frontend:
- Chọn hạng bằng `A1` hoặc `A`
- Cấu hình số câu, thời gian, điểm đạt
- Sinh bộ câu hỏi mẫu
- Chọn đáp án theo từng câu
- Theo dõi tiến độ làm bài
- Đếm ngược thời gian
- Nộp bài thủ công hoặc tự động khi hết giờ
- Chấm điểm và hiển thị câu sai

Tham chiếu:
- [`webthibanglai/wwwroot/js/exam.js`](webthibanglai/wwwroot/js/exam.js:1)
- [`webthibanglai/Views/Exam/Index.cshtml`](webthibanglai/Views/Exam/Index.cshtml:1)

Lưu ý: trang thi thử MVC hiện đang nhúng file tĩnh `exam.html` qua `iframe` trong [`webthibanglai/Views/Exam/Index.cshtml`](webthibanglai/Views/Exam/Index.cshtml:5).

### 4.4. Đăng nhập frontend
Dự án đã có xử lý đăng nhập ở mức frontend:
- Tài khoản mẫu `admin/admin123`
- Tài khoản mẫu `user/user123`
- Lưu trạng thái đăng nhập bằng `localStorage`
- Điều chỉnh menu theo trạng thái đăng nhập
- Chặn truy cập trang quản trị tĩnh nếu không phải admin

Tham chiếu: [`webthibanglai/wwwroot/js/auth.js`](webthibanglai/wwwroot/js/auth.js:1).

## 5. Hiện trạng triển khai
### Đã hoàn thành
- Khởi tạo project MVC trên .NET 9.
- Tổ chức cấu trúc `Controllers` / `Views` / `wwwroot`.
- Tạo layout và header dùng chung.
- Việt hóa đáng kể giao diện theo chủ đề thi bằng lái xe máy.
- Tạo nhiều controller và view cơ bản cho các trang chính.
- Xây dựng luồng thi thử ở phía frontend bằng JavaScript.
- Bổ sung đăng nhập frontend và một số trang quản trị tĩnh trong [`webthibanglai/wwwroot/admin`](webthibanglai/wwwroot/admin).
- Có tài liệu kế hoạch tổng thể cho giai đoạn frontend-first.

### Đang ở mức cơ bản / tạm thời
- Phần lớn controller mới chỉ `return View()` và chưa có xử lý nghiệp vụ backend.
- Dữ liệu đề thi hiện là dữ liệu mẫu hard-code trong JavaScript tại [`webthibanglai/wwwroot/js/exam.js`](webthibanglai/wwwroot/js/exam.js:7).
- Đăng nhập hiện chưa kết nối backend, chưa có phân quyền thật.
- Một phần nội dung vẫn đang dựa trên template frontend cũ và file HTML tĩnh.
- Trang thi thử MVC chưa render native bằng Razor mà đang dùng `iframe` nhúng trang tĩnh.

### Chưa thấy hoàn thiện
- Chưa có `Models` nghiệp vụ chính như câu hỏi, đề thi, kết quả, học viên.
- Chưa có `DbContext`, migration hay tích hợp SQL Server.
- Chưa có API/backend để chấm điểm, lưu lịch sử, quản lý ngân hàng đề.
- Chưa thấy test tự động.
- Chưa có quy trình xác thực/ủy quyền thật bằng ASP.NET Core Identity hoặc JWT/cookie auth.
- Chưa có README tổng thể trước đó ở gốc dự án.

## 6. Đánh giá tiến độ tổng quan
Tiến độ hiện tại có thể đánh giá sơ bộ như sau:

| Hạng mục | Mức độ | Ghi chú |
|---|---|---|
| Khởi tạo nền tảng MVC | ~90% | Đã chạy được cấu trúc cơ bản |
| Giao diện người dùng | ~75% | Nhiều trang đã có nội dung và điều hướng |
| Thi thử frontend | ~70% | Đã có luồng làm bài, nhưng dữ liệu còn giả lập |
| Tích hợp MVC thuần cho toàn bộ giao diện | ~45% | Vẫn còn phụ thuộc các file HTML tĩnh |
| Backend nghiệp vụ | ~10% | Gần như chưa triển khai |
| Cơ sở dữ liệu | 0% | Chưa thấy tích hợp |
| Quản trị thực tế | ~20% | Mới ở mức trang tĩnh + auth giả lập |

**Kết luận ngắn:** dự án đang ở giai đoạn **frontend/migration sang MVC**, đã có hình hài sản phẩm demo khá rõ, nhưng **chưa bước vào giai đoạn backend hoàn chỉnh**.

## 7. Hướng phát triển tiếp theo nên ưu tiên
1. Chuyển toàn bộ các trang tĩnh quan trọng sang Razor View thống nhất.
2. Bỏ phụ thuộc `iframe` ở trang thi thử, render trực tiếp trong MVC.
3. Tách ngân hàng câu hỏi khỏi JavaScript hard-code.
4. Thiết kế `Models` nghiệp vụ: câu hỏi, đề thi, đáp án, kết quả, người dùng.
5. Tích hợp SQL Server.
6. Xây dựng API hoặc service backend cho chấm điểm và lưu lịch sử làm bài.
7. Thay đăng nhập giả lập bằng xác thực thật.
8. Hoàn thiện khu vực quản trị để CRUD đề thi và câu hỏi.

## 8. Cách chạy dự án
Từ thư mục workspace hiện tại:

```bash
dotnet run --project webthibanglai/webthibanglai.csproj
```

Hoặc mở solution [`webthibanglai.sln`](webthibanglai.sln) bằng Visual Studio rồi chạy project web.

## 9. Tóm tắt một dòng
Dự án hiện là **website ôn thi bằng lái xe máy theo hướng ASP.NET Core MVC, đã khá rõ ở phần giao diện và thi thử frontend, nhưng backend, dữ liệu và quản trị thực tế vẫn đang trong giai đoạn đầu**.
