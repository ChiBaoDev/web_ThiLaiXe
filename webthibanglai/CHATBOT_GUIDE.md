# Hướng dẫn Chatbot AI - Hệ thống Thi Bằng Lái Xe

## 📋 Tổng quan

Chatbot AI đã được tích hợp vào website với các tính năng:
- ✅ Floating button góc phải dưới màn hình
- ✅ Popup chat với UI hiện đại
- ✅ Tích hợp OpenAI API (có fallback responses)
- ✅ Context-aware (nhận biết trang hiện tại)
- ✅ Responsive trên mobile
- ✅ Có thể ẩn/hiện theo trang

## 🚀 Cấu hình

### 1. Cấu hình OpenAI API Key

Mở file `appsettings.json` và thêm API key của bạn:

```json
{
  "OpenAI": {
    "ApiKey": "sk-your-openai-api-key-here",
    "ApiUrl": "https://api.openai.com/v1/chat/completions"
  }
}
```

**Lưu ý:** 
- Nếu không có API key, chatbot vẫn hoạt động với fallback responses (câu trả lời có sẵn)
- Để lấy API key: https://platform.openai.com/api-keys

### 2. Cấu hình cho Production

Với production, nên dùng User Secrets hoặc Environment Variables:

```bash
# Sử dụng User Secrets (khuyên dùng cho development)
dotnet user-secrets set "OpenAI:ApiKey" "sk-your-key-here"

# Hoặc Environment Variable (cho production)
export OpenAI__ApiKey="sk-your-key-here"
```

## 📁 Cấu trúc Files

```
webthibanglai/
├── Controllers/
│   └── ChatbotController.cs          # API endpoint cho chatbot
├── Services/
│   ├── IAIService.cs                 # Interface
│   └── OpenAIService.cs              # Implementation với OpenAI API
├── Views/
│   └── Shared/
│       ├── _Chatbot.cshtml           # UI chatbot (partial view)
│       └── _Layout.cshtml            # Đã tích hợp chatbot
├── appsettings.json                  # Cấu hình OpenAI
└── Program.cs                        # Đăng ký services
```

## 🎯 Cách sử dụng

### Chatbot hiển thị mặc định trên tất cả trang

Chatbot sẽ tự động xuất hiện ở góc phải dưới màn hình.

### Ẩn chatbot trên một trang cụ thể

Trong Controller, thêm:

```csharp
public IActionResult Index()
{
    ViewBag.HideChatbot = true;  // Ẩn chatbot
    return View();
}
```

Ví dụ ẩn chatbot ở trang Login:

```csharp
// LoginController.cs
public IActionResult Index()
{
    ViewBag.HideChatbot = true;
    return View();
}
```

### Ẩn chatbot bằng JavaScript (theo URL)

Thêm vào `_Chatbot.cshtml` (đã có sẵn trong code):

```javascript
// Ẩn chatbot ở một số trang cụ thể
if (window.location.pathname.includes("/login") || 
    window.location.pathname.includes("/admin")) {
    document.getElementById("chatbot-container").style.display = "none";
}
```

## 🎨 Tùy chỉnh giao diện

### Thay đổi màu sắc

Trong `_Chatbot.cshtml`, tìm và sửa:

```css
/* Màu gradient chính */
background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);

/* Thay bằng màu khác, ví dụ: */
background: linear-gradient(135deg, #06b6d4 0%, #3b82f6 100%);
```

### Thay đổi vị trí

```css
#chatbot-button {
    bottom: 20px;   /* Khoảng cách từ dưới */
    right: 20px;    /* Khoảng cách từ phải */
    
    /* Hoặc đặt bên trái: */
    /* left: 20px; */
}
```

### Thay đổi kích thước popup

```css
#chatbot-box {
    width: 380px;    /* Chiều rộng */
    height: 550px;   /* Chiều cao */
}
```

## 💡 Tính năng nâng cao

### 1. Context-Aware Responses

Chatbot tự động nhận biết trang hiện tại và đưa ra câu trả lời phù hợp:

- **Trang Exam**: Tập trung vào mẹo thi, chiến lược làm bài
- **Trang Khóa học**: Tư vấn về khóa học, học phí
- **Trang Lịch học**: Hỗ trợ về lịch học, lịch thi
- **Trang Login**: Hỗ trợ đăng nhập, quên mật khẩu

### 2. Lưu lịch sử chat (Optional)

Trong `_Chatbot.cshtml`, bỏ comment dòng này:

```javascript
// loadChatHistory(); // Uncomment to enable chat history
```

Lịch sử chat sẽ được lưu trong `localStorage` của trình duyệt.

### 3. Thêm gợi ý nhanh (Quick Replies)

Thêm vào phần welcome message trong `_Chatbot.cshtml`:

```html
<div class="quick-replies">
    <button class="quick-reply-btn" onclick="sendQuickReply('Các loại bằng lái có gì?')">
        Loại bằng lái
    </button>
    <button class="quick-reply-btn" onclick="sendQuickReply('Mẹo thi hiệu quả')">
        Mẹo thi
    </button>
</div>
```

## 🔧 Troubleshooting

### Chatbot không hiển thị

1. Kiểm tra `_Layout.cshtml` đã có `<partial name="_Chatbot" />`
2. Kiểm tra `ViewBag.HideChatbot` có được set = true không
3. Xem Console (F12) có lỗi JavaScript không

### API không hoạt động

1. Kiểm tra API key trong `appsettings.json`
2. Kiểm tra logs trong Console
3. Chatbot vẫn hoạt động với fallback responses

### Lỗi CORS khi gọi API

Nếu dùng API khác OpenAI, cần cấu hình CORS trong `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
```

## 📊 Monitoring & Analytics

### Theo dõi số lượng câu hỏi

Thêm vào `ChatbotController.cs`:

```csharp
[HttpPost]
public async Task<IActionResult> AskAI([FromBody] ChatRequest request)
{
    // Log câu hỏi
    _logger.LogInformation($"Chatbot question: {request.Message}");
    
    // ... existing code
}
```

### Lưu câu hỏi vào database (Optional)

Tạo bảng `ChatLogs` và lưu mỗi câu hỏi để phân tích sau.

## 🚀 Nâng cấp trong tương lai

### 1. Tích hợp với dữ liệu khóa học

```csharp
// Trong OpenAIService.cs
var courses = await _courseService.GetAllCoursesAsync();
var courseInfo = string.Join(", ", courses.Select(c => c.Name));
systemPrompt += $"\n\nCác khóa học hiện có: {courseInfo}";
```

### 2. Chatbot đa ngôn ngữ

```csharp
var language = Request.Headers["Accept-Language"].ToString();
var systemPrompt = language.Contains("en") 
    ? "You are an AI assistant..." 
    : "Bạn là trợ lý AI...";
```

### 3. Voice input/output

Tích hợp Web Speech API để chatbot có thể nghe và nói.

## 📝 Best Practices

1. **Bảo mật API Key**: Không commit API key vào Git
2. **Rate Limiting**: Giới hạn số request từ mỗi user
3. **Caching**: Cache các câu hỏi phổ biến
4. **Error Handling**: Luôn có fallback response
5. **User Experience**: Hiển thị loading state khi đang xử lý

## 📞 Hỗ trợ

Nếu cần hỗ trợ thêm, tham khảo:
- OpenAI API Docs: https://platform.openai.com/docs
- ASP.NET Core Docs: https://docs.microsoft.com/aspnet/core

---

**Phiên bản**: 1.0  
**Ngày cập nhật**: 29/04/2026
