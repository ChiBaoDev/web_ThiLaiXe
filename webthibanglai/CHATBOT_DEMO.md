# Demo Chatbot AI

## Cách test Chatbot

### 1. Chạy ứng dụng

```bash
cd webthibanglai
dotnet run
```

Hoặc nhấn F5 trong Visual Studio.

### 2. Mở trình duyệt

Truy cập: `https://localhost:5001` hoặc `http://localhost:5000`

### 3. Kiểm tra Chatbot

Bạn sẽ thấy:
- ✅ Nút floating màu tím gradient ở góc phải dưới màn hình
- ✅ Click vào nút để mở popup chat
- ✅ Chatbot hiển thị lời chào và gợi ý

### 4. Test các tính năng

#### Test câu hỏi cơ bản (không cần API key):

1. **"Các loại bằng lái có gì?"**
   - Chatbot sẽ liệt kê A1, A2, B1, B2, C, D, E

2. **"Mẹo thi hiệu quả"**
   - Chatbot đưa ra các mẹo ôn thi

3. **"Thông tin khóa học"**
   - Chatbot tư vấn về khóa học

4. **"Lịch học"**
   - Chatbot hỗ trợ về lịch học, lịch thi

#### Test với OpenAI API (nếu có API key):

1. Thêm API key vào `appsettings.json`:
```json
{
  "OpenAI": {
    "ApiKey": "sk-your-key-here"
  }
}
```

2. Hỏi các câu phức tạp hơn:
   - "Tôi muốn học bằng B2, cần chuẩn bị gì?"
   - "Phân biệt biển báo cấm và biển báo nguy hiểm"
   - "Kỹ thuật đỗ xe song song"

### 5. Test Context-Aware

Chatbot sẽ trả lời khác nhau tùy theo trang:

- **Trang Exam** (`/Exam`): Tập trung vào mẹo thi
- **Trang Khóa học** (`/KhoaHoc`): Tư vấn khóa học
- **Trang Lịch học** (`/LichHoc`): Hỗ trợ lịch học

### 6. Test ẩn Chatbot

Để test tính năng ẩn chatbot, thêm vào Controller:

```csharp
// Ví dụ: LoginController.cs
public IActionResult Index()
{
    ViewBag.HideChatbot = true;  // Ẩn chatbot ở trang login
    return View();
}
```

### 7. Test trên Mobile

- Mở DevTools (F12)
- Chuyển sang chế độ mobile (Ctrl+Shift+M)
- Kiểm tra responsive của chatbot

## Screenshots mong đợi

### Desktop
```
┌─────────────────────────────────────┐
│         Website Header              │
├─────────────────────────────────────┤
│                                     │
│         Content Area                │
│                                     │
│                                     │
│                              ┌────┐ │
│                              │ 💬 │ │ <- Floating button
│                              └────┘ │
└─────────────────────────────────────┘
```

### Khi mở Chatbot
```
┌─────────────────────────────────────┐
│         Website Header              │
├─────────────────────────────────────┤
│                                     │
│         Content Area         ┌────┐ │
│                              │Chat│ │
│                              │Box │ │
│                              │    │ │
│                              │    │ │
│                              └────┘ │
│                              ┌────┐ │
│                              │ 💬 │ │
│                              └────┘ │
└─────────────────────────────────────┘
```

## Troubleshooting

### Chatbot không hiển thị?
1. Kiểm tra Console (F12) có lỗi không
2. Xem `_Layout.cshtml` đã include `_Chatbot.cshtml` chưa
3. Clear cache và reload (Ctrl+Shift+R)

### API không hoạt động?
1. Kiểm tra API key trong `appsettings.json`
2. Xem Network tab (F12) có request đến `/Chatbot/AskAI` không
3. Chatbot vẫn hoạt động với fallback responses

### Lỗi CORS?
- Không có vấn đề CORS vì API được gọi từ backend (server-side)

## Video Demo (Tự quay)

1. Mở website
2. Click vào nút chatbot
3. Gõ câu hỏi: "Các loại bằng lái có gì?"
4. Xem phản hồi
5. Thử thêm vài câu hỏi khác
6. Đóng chatbot
7. Chuyển sang trang khác (Exam, Khóa học)
8. Mở chatbot lại và hỏi câu tương tự
9. So sánh câu trả lời có khác nhau không

## Checklist Test

- [ ] Chatbot hiển thị ở góc phải dưới
- [ ] Click mở/đóng hoạt động
- [ ] Gửi tin nhắn thành công
- [ ] Nhận được phản hồi từ bot
- [ ] Loading indicator hiển thị khi đang xử lý
- [ ] Scroll chat messages hoạt động
- [ ] Enter để gửi tin nhắn
- [ ] Responsive trên mobile
- [ ] Context-aware (câu trả lời khác nhau theo trang)
- [ ] Có thể ẩn chatbot bằng ViewBag

## Kết quả mong đợi

✅ Chatbot hoạt động mượt mà  
✅ UI đẹp, hiện đại  
✅ Phản hồi nhanh (với hoặc không có API)  
✅ Responsive tốt trên mọi thiết bị  
✅ Không có lỗi JavaScript  
✅ Build thành công (0 errors, 0 warnings)
