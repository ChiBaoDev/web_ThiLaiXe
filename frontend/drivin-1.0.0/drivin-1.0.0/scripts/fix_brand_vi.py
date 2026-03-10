from pathlib import Path

ROOT = Path('.')

HTML_FILES = [
    p for p in ROOT.glob('*.html')
    if p.is_file()
]

# 1) Đồng bộ icon logo xe máy trên toàn site
for path in HTML_FILES:
    text = path.read_text(encoding='utf-8-sig')
    new_text = text.replace('fa fa-car', 'fa fa-motorcycle')
    if new_text != text:
        path.write_text(new_text, encoding='utf-8-sig')

# 2) Sửa navbar exam về chuẩn collapse để tránh lệch layout
exam_path = ROOT / 'exam.html'
exam_text = exam_path.read_text(encoding='utf-8-sig')
exam_text_new = exam_text.replace(
    'class="collapse navbar-collapse show" id="navbarCollapse"',
    'class="collapse navbar-collapse" id="navbarCollapse"'
)
if exam_text_new != exam_text:
    exam_path.write_text(exam_text_new, encoding='utf-8-sig')

# 3) Việt hóa tiếng Anh còn sót ở trang chủ
index_path = ROOT / 'index.html'
index_text = index_path.read_text(encoding='utf-8-sig')

replacements = {
    'Bộ đề thi Upskill You With Driving Training': 'Bộ đề thi mô phỏng sát hạch GPLX xe máy',
    'Tempor erat elitr rebum at clita dolor diam ipsum sit diam amet diam et eos': 'Bộ đề bám sát cấu trúc đề thật, có giải thích đáp án rõ ràng giúp bạn nhớ lâu và làm bài tự tin hơn.',
    'placeholder="Gurdian Name"': 'placeholder="Họ và tên"',
    'placeholder="Gurdian Email"': 'placeholder="Email"',
    'placeholder="Child Name"': 'placeholder="Hạng bằng quan tâm"',
    'placeholder="Child Age"': 'placeholder="Hình thức học"',
    'placeholder="Leave a message here"': 'placeholder="Nhập lời nhắn"',
    'Magna sea eos sit dolor, ipsum amet ipsum lorem diam eos': 'Nội dung học được biên soạn dễ hiểu, bám sát quy định hiện hành và cập nhật thường xuyên.',
    'Dolores sed duo clita tempor justo dolor et stet lorem kasd labore dolore lorem ipsum. At lorem lorem magna ut et, nonumy et labore et tempor diam tempor erat.': 'Nền tảng dễ sử dụng, bộ đề chuẩn và phần thống kê chi tiết giúp tôi cải thiện điểm số rõ rệt chỉ sau vài ngày ôn tập.',
    'Contact Us': 'Liên hệ',
    'Your Site Name': 'ThiXeMay',
    'Designed By <a href="https://htmlcodex.com">HTML Codex</a>': 'Thiết kế giao diện: ThiXeMay',
    '<br>Distributed By: <a href="https://themewagon.com" target="_blank">ThemeWagon</a>': '<br>Phát triển & Việt hóa: ThiXeMay'
}

for src, dst in replacements.items():
    index_text = index_text.replace(src, dst)

index_path.write_text(index_text, encoding='utf-8-sig')

print('OK: applied branding + vi localization + exam navbar normalize')
