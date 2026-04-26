# Tổng quan dự án `webthibanglai`

## 1. Giới thiệu
`webthibanglai` là website ôn luyện và thi thử bằng lái xe máy, được phát triển theo hướng **ASP.NET Core MVC** và hiện đã chuyển đáng kể từ giao diện tĩnh sang Razor View có tích hợp API thật cho luồng thi đề mẫu.

Mục tiêu hiện tại của dự án là:
- Xây dựng website giới thiệu nền tảng ôn thi GPLX xe máy.
- Cung cấp luồng thi thử/làm đề mẫu theo phiên thi thật từ API.
- Hỗ trợ đăng nhập, lấy token và dùng token đó để gọi API protected.
- Từng bước thay thế phần giao diện HTML tĩnh bằng MVC + Razor hoàn chỉnh.
- Giữ frontend đồng bộ với backend hiện có mà **không sửa API** cho các yêu cầu phía web.

## 2. Công nghệ đang sử dụng
- **.NET 9 / ASP.NET Core MVC** trong `webthibanglai.csproj`
- Cấu hình khởi động MVC trong `Program.cs`
- Razor Views và Partial Views trong thư mục `Views`
- `HttpClient` + service layer để gọi API backend trong `Services/ExamApiService.cs`
- Session để lưu access token dùng cho các request tới API
- Bootstrap, Font Awesome, jQuery, WOW.js, Owl Carousel trong `Views/Shared/_Layout.cshtml`
- Tài nguyên tĩnh HTML/CSS/JS trong `wwwroot`

## 3. Cấu trúc chính của dự án
- `Program.cs`: cấu hình DI, session, pipeline MVC.
- `Controllers`: các controller điều hướng và xử lý logic trang.
- `Views`: giao diện Razor cho từng màn hình.
- `Models`: view models cho giao diện MVC.
- `Services/ExamApiService.cs`: lớp gọi API đề mẫu, phiên thi, lưu đáp án, nộp bài, xem kết quả.
- `Views/Shared/_Layout.cshtml`: layout dùng chung.
- `Views/Shared/_Header.cshtml`: header/menu điều hướng chính.
- `Views/Shared/_Footer.cshtml`: footer giao diện.
- `Views/Exam/Index.cshtml`: danh sách đề thi mẫu.
- `Views/Exam/Launch.cshtml`: trang trung gian để vào fullscreen trước khi thi.
- `Views/Exam/Session.cshtml`: màn hình làm bài thi theo session thật.
- `Views/Exam/Result.cshtml`: màn hình kết quả và review bài thi.
- `wwwroot/js/exam.js`: logic thi thử frontend cũ/dạng demo.
- `wwwroot/js/auth.js`: logic đồng bộ trạng thái đăng nhập phía frontend.
- `wwwroot/plans/ke-hoach-he-thong-thi-bang-lai-xe-may.md`: tài liệu kế hoạch triển khai.

## 4. Chức năng hiện có

### 4.1. Phần website MVC
Các controller hiện tại phục vụ điều hướng và xử lý cho các khu vực chính của website:
- `HomeController`
- `AboutController`
- `ContactController`
- `KhoaHocController`
- `LichHocController`
- `LoginController`
- `ExamController`

Menu chính đã có các mục như:
- Trang chủ
- Giới thiệu
- Bộ đề
- Khóa học / lịch học
- Liên hệ
- Đăng nhập / hồ sơ

### 4.2. Trang chủ và các trang nội dung
Website đã có giao diện giới thiệu tương đối đầy đủ, gồm:
- Banner giới thiệu
- Nội dung marketing cho nền tảng luyện thi GPLX
- Khối lợi ích/hệ thống
- Nội dung về khóa học, liên hệ, tư vấn
- Các trang thông tin cơ bản được chuyển sang MVC

### 4.3. Đăng nhập và hồ sơ người dùng
Dự án hiện đã có phần đăng nhập/profile theo hướng kết nối API:
- Form đăng nhập trong `Views/Login/Index.cshtml`
- Controller xử lý login/logout/profile trong `Controllers/LoginController.cs`
- Lưu token trong session server-side để dùng cho các API cần xác thực
- Có màn hình hồ sơ người dùng trong `Views/Login/Profile.cshtml`

Lưu ý: ngoài phần login MVC, dự án vẫn còn file `wwwroot/js/auth.js` từ luồng frontend cũ để phục vụ một số trang tĩnh.

### 4.4. Danh sách đề thi mẫu từ API
Luồng đề thi mẫu đã được tích hợp vào MVC:
- Gọi API `GET /api/v1/sample-exams` qua service
- Hiển thị danh sách đề thi trong `Views/Exam/Index.cshtml`
- Có nút **Bắt đầu thi** cho từng đề
- Đã tinh chỉnh giao diện header và nội dung hiển thị cho phù hợp web hiện tại
- Đã bỏ các thông tin không cần thiết như mã đề kỹ thuật ở phần UI

Service liên quan:
- `ExamApiService.GetSampleExamsAsync()`

Controller liên quan:
- `ExamController.Index()`
- `ExamController.Start()`

### 4.5. Luồng thi theo session thật từ API
Đây là phần đã được cập nhật mạnh nhất ở tiến độ hiện tại.

Luồng hiện tại:
1. Người dùng vào danh sách đề mẫu.
2. Bấm **Bắt đầu thi**.
3. Web gọi API `POST /api/v1/exams/sample/{sampleExamId}/start`.
4. Nhận `sessionId`.
5. Chuyển sang trang `Launch` để chuẩn bị fullscreen.
6. Từ đó render bài thi thật trong `Session` theo dữ liệu API session.

Các API đang được web sử dụng qua `ExamApiService`:
- `GET /api/v1/sample-exams`
- `POST /api/v1/exams/sample/{sampleExamId}/start`
- `GET /api/v1/exams/sessions/{sessionId}`
- `GET /api/v1/exams/sessions/{sessionId}/questions/{number}`
- `POST /api/v1/exams/sessions/{sessionId}/answers`
- `POST /api/v1/exams/sessions/{sessionId}/submit`
- `POST /api/v1/exams/sessions/{sessionId}/auto-submit`
- `GET /api/v1/exams/sessions/{sessionId}/result`
- `GET /api/v1/exams/sessions/{sessionId}/review`

### 4.6. Màn hình làm bài thi MVC thực tế
Màn hình thi trong `Views/Exam/Session.cshtml` hiện đã hỗ trợ:
- Render câu hỏi thật từ API session.
- Chỉ hiển thị đúng số đáp án thực tế từ dữ liệu API.
- Bấm chuyển câu ở phần **Tổng quan**.
- Highlight các câu đã làm.
- Hiển thị **Tiến độ làm bài** theo số câu đã trả lời.
- Đếm ngược thời gian còn lại.
- Ẩn các phần giao diện thừa khi đang ở chế độ thi fullscreen/embedded.
- Dùng giao diện gần với layout thi cũ nhưng đã chạy bằng Razor.

### 4.7. Lưu đáp án realtime không reload
Phần này đã được bổ sung mới trong tiến độ hiện tại.

Khi người dùng chọn đáp án trong `Views/Exam/Session.cshtml`:
- Frontend gọi AJAX tới action MVC trung gian.
- MVC gọi tiếp API `POST /api/v1/exams/sessions/{sessionId}/answers`.
- Không reload trang khi lưu đáp án.
- Có trạng thái giao diện: đang lưu / lưu thành công / lỗi.
- Sau khi bấm chuyển câu, hệ thống điều hướng sang đúng câu tiếp theo hoặc câu trước.

Các action MVC phục vụ cho luồng này:
- `ExamController.SaveAnswerAjax()`
- `ExamController.SubmitAjax()`

Lưu ý quan trọng:
- Luồng mới được làm ở phía `webthibanglai`.
- Backend API **không bị sửa**.

### 4.8. Nộp bài và xem kết quả
Luồng nộp bài/kết quả hiện đã đầy đủ ở mức web MVC:
- Nộp bài thủ công bằng AJAX.
- Tự động nộp bài khi hết giờ.
- Chuyển sang trang kết quả sau khi submit thành công.
- Hiển thị:
  - tổng số câu
  - số câu đúng
  - số câu sai
  - số câu chưa trả lời
  - điểm
  - trạng thái đạt / không đạt
  - cảnh báo câu điểm liệt nếu có
- Có danh sách review các câu với trạng thái đúng/sai/chưa trả lời.

Các view liên quan:
- `Views/Exam/Result.cshtml`
- `Views/Exam/Session.cshtml`

### 4.9. Thi thử frontend cũ vẫn còn tồn tại
Ngoài luồng session thật nói trên, dự án vẫn còn phần thi thử frontend cũ:
- Dữ liệu mẫu hard-code trong `wwwroot/js/exam.js`
- Trang HTML demo trong `wwwroot/exam.html`

Phần này vẫn hữu ích như tài nguyên tham chiếu giao diện, nhưng **không còn là luồng thi chính mới nhất**.

## 5. Hiện trạng triển khai

### Đã hoàn thành
- Khởi tạo project MVC trên .NET 9.
- Tổ chức cấu trúc `Controllers` / `Views` / `Models` / `Services` / `wwwroot`.
- Có layout, header, footer dùng chung.
- Việt hóa đáng kể giao diện theo chủ đề thi bằng lái xe máy.
- Có trang đăng nhập, hồ sơ người dùng và xử lý token/session.
- Tích hợp danh sách đề thi mẫu từ API thật.
- Tích hợp luồng bắt đầu thi đề mẫu bằng API session.
- Có trang trung gian `Launch` để xử lý trải nghiệm fullscreen trước khi vào thi.
- Có màn hình thi MVC dùng dữ liệu thật từ API.
- Chỉ render đúng đáp án thực tế theo dữ liệu backend.
- Có palette tổng quan, highlight câu đã làm và tiến độ làm bài.
- Có lưu đáp án realtime bằng AJAX, không reload trang.
- Có submit bài và xem kết quả/review bằng session API thật.
- Đã sửa các lỗi UI/luồng gần đây như:
  - lỗi `@media` trong Razor ở `Launch.cshtml`
  - lỗi footer còn hiện khi đang thi
  - lỗi chuyển câu trong phần tổng quan
  - lỗi điều hướng câu sau khi chọn đáp án

### Đang ở mức chuyển tiếp / cần tiếp tục hoàn thiện
- Một số phần của website vẫn còn chịu ảnh hưởng từ template frontend cũ.
- Một phần JS/demo trong `wwwroot` vẫn tồn tại song song với luồng MVC mới.
- README và tài liệu nội bộ cần tiếp tục cập nhật khi luồng MVC thay đổi thêm.
- Chưa có test tự động cho các luồng thi hiện tại.

### Chưa thấy hoàn thiện
- Chưa thấy `DbContext`, migration hay tích hợp SQL Server ở riêng project web.
- Chưa có test tự động UI/integration cho luồng thi.
- Một số khu vực admin phía web vẫn thiên về trang tĩnh.
- Chưa có tài liệu vận hành/deploy chi tiết cho production.

## 6. Đánh giá tiến độ tổng quan
Tiến độ hiện tại có thể đánh giá sơ bộ như sau:

| Hạng mục | Mức độ | Ghi chú |
|---|---|---|
| Khởi tạo nền tảng MVC | ~95% | Cấu trúc ứng dụng đã rõ ràng |
| Giao diện người dùng | ~85% | Phần lớn trang chính đã hoạt động trên MVC |
| Đăng nhập / hồ sơ | ~75% | Đã có luồng MVC + session token |
| Danh sách đề mẫu | ~90% | Đã lấy dữ liệu thật từ API |
| Thi theo session API thật | ~88% | Đã có start, load câu hỏi, save answer, submit, result |
| Realtime answer saving | ~85% | Đã có AJAX, cần tiếp tục kiểm thử thêm |
| Kết quả / review bài thi | ~85% | Đã hiển thị điểm và đúng/sai |
| Backend nghiệp vụ | phụ thuộc project API | Web đã tích hợp khá sâu với API hiện có |
| Test tự động | ~5% | Hầu như chưa thấy |
| Quản trị thực tế | ~25% | Vẫn còn nhiều phần tĩnh/chưa hoàn thiện |

**Kết luận ngắn:** dự án đã đi qua giai đoạn “demo frontend đơn thuần”, và hiện đang ở giai đoạn **MVC + tích hợp API thật cho luồng thi đề mẫu**, trong đó phần làm bài theo session đã tiến triển rõ rệt.

## 7. Hướng phát triển tiếp theo nên ưu tiên
1. Tiếp tục kiểm thử toàn bộ luồng thi session trên dữ liệu thật.
2. Hoàn thiện nốt các lỗi UI/UX nhỏ phát sinh khi điều hướng câu, fullscreen và auto-submit.
3. Cải thiện trang kết quả để hiển thị review trực quan hơn (tên đáp án thay vì chỉ id, nếu API hiện có cho phép).
4. Tiếp tục chuyển các phần còn phụ thuộc file HTML tĩnh sang Razor View.
5. Rà soát lại mã JavaScript cũ trong `wwwroot/js/exam.js` để tách rõ phần demo và phần production.
6. Bổ sung tài liệu kỹ thuật cho luồng `ExamController` + `ExamApiService`.
7. Viết test cho các luồng quan trọng của phần thi.
8. Hoàn thiện khu vực quản trị khi có yêu cầu nghiệp vụ tiếp theo.

## 8. Cách chạy dự án
Từ thư mục workspace hiện tại:

```bash
dotnet run --project webthibanglai/webthibanglai.csproj
```

Hoặc mở solution `webthibanglai.sln` bằng Visual Studio rồi chạy project web.

### Lưu ý môi trường build
Trong môi trường máy hiện tại, đã từng gặp lỗi build từ SDK/Windows liên quan CET, ví dụ:
- `Your Windows doesn't fully support CET. Please install all available Windows updates.`

Điều này là lỗi môi trường chạy/build của máy, không phải mô tả chức năng của source code.

## 9. Tóm tắt một dòng
Dự án hiện là **website ôn thi bằng lái xe máy theo hướng ASP.NET Core MVC, đã tích hợp API thật cho danh sách đề mẫu và luồng thi theo session, đồng thời đã có lưu đáp án realtime, nộp bài và xem kết quả trên giao diện web**.
