import os

nav_pages = [
    ('dashboard', 'Dashboard', ' Tổng quan hệ thống'),
    ('lich-hoc-lich-thi', 'Lịch học & Lịch thi', ' Quản lý lịch học và lịch thi'),
    ('thong-bao', 'Thông báo', ' Trung tâm thông báo'),
    ('dao-tao', 'Đào tạo', ' Tổng quan đào tạo'),
    ('hoc-vien', 'Học viên', ' Quản lý học viên'),
    ('khoa-hoc', 'Khóa học', ' Quản lý khóa học'),
    ('giao-vien', 'Giáo viên', ' Quản lý giáo viên'),
    ('giao-trinh', 'Giáo trình', ' Quản lý giáo trình'),
    ('thi-cu', 'Thi cử', ' Tổng quan thi cử'),
    ('de-thi-sat-hach', 'Đề thi & Sát hạch', ' Quản lý đề thi và sát hạch'),
    ('ket-qua-thi', 'Kết quả thi', ' Kết quả thi gần nhất'),
    ('cap-gplx', 'Cấp GPLX', ' Quản lý cấp giấy phép lái xe'),
    ('quan-ly', 'Quản lý', ' Các công cụ quản lý chung'),
    ('phuong-tien', 'Phương tiện', ' Quản lý đội xe'),
    ('hoc-phi-thanh-toan', 'Học phí & Thanh toán', ' Quản lý học phí và thanh toán'),
]

with open('dashboard.html', 'r', encoding='utf-8') as f:
    text = f.read()

start = text.index('<style>')
end = text.index('</style>') + len('</style>')
base_style = text[start:end]

nav_html = ''
for key, label, _ in nav_pages:
    icon = ''
    if label == 'Dashboard': icon = ''
    elif 'Lịch' in label: icon = ''
    elif label == 'Thông báo': icon = ''
    elif label == 'Đào tạo': icon = ''
    elif label == 'Học viên': icon = ''
    elif label == 'Khóa học': icon = ''
    elif label == 'Giáo viên': icon = ''
    elif label == 'Giáo trình': icon = ''
    elif label == 'Thi cử': icon = ''
    elif label == 'Đề thi & Sát hạch': icon = ''
    elif label == 'Kết quả thi': icon = ''
    elif label == 'Cấp GPLX': icon = ''
    elif label == 'Quản lý': icon = ''
    elif label == 'Phương tiện': icon = ''
    elif label == 'Học phí & Thanh toán': icon = ''
    nav_html += f"    <div class=\"nav-item\" data-page=\"{key}\" onclick=\"setActive(this)\">\n      <span class=\"nav-icon\">{icon}</span> {label}\n    </div>\n"

page_map_entries = ',\n'.join([f"      '{p[0]}': '{p[0]}.html'" for p in nav_pages])

base_html_template = '''<!DOCTYPE html>
<html lang="vi">
<head>
<meta charset="UTF-8" />
<meta name="viewport" content="width=device-width, initial-scale=1.0" />
<title>__TITLE__  GPLX Portal</title>
__STYLE__
</head>
<body data-page="__PAGE__">
<aside class="sidebar">
  <div class="sidebar-logo">
    <div class="logo-icon"></div>
    <div class="logo-text">
      <strong>GPLX Portal</strong>
      <span>v2.5.1  .NET 8.0</span>
    </div>
  </div>
  <div class="nav-section">
    <div class="nav-label">Chuyển trang</div>
__NAV__  </div>
</aside>
<div class="main">
  <header class="header">
    <div class="header-breadcrumb">
      <span style="color:var(--text-muted)">Hệ thống / </span>
      <strong>__LABEL__</strong>
    </div>
  </header>
  <main class="content">
    <div class="page-header">
      <h1>__LABEL__</h1>
      <p>__SUBTITLE__</p>
    </div>
    <div class="card">
      <div class="card-body">
        <p>Trang nội dung mẫu cho <strong>__LABEL__</strong>. Đây là khu vực bạn thay bằng bảng điều khiển chi tiết.</p>
        <p>Điều hướng giữa các tab thực hiện bằng cách nhấn vào thanh điều hướng bên trái.</p>
      </div>
    </div>
  </main>
</div>
<script>
  function setActive(el) {
    const target = el.dataset.page;
    if (!target) return;
    document.querySelectorAll('.nav-item').forEach(i => i.classList.remove('active'));
    el.classList.add('active');
    const pageMap = {
__PAGE_MAP__
    };
    if (pageMap[target] && location.pathname.indexOf(pageMap[target]) === -1) {
      location.href = pageMap[target];
    }
  }
  window.addEventListener('DOMContentLoaded', function(){
    const current = document.body.dataset.page;
    if(current){
      const active = document.querySelector('.nav-item[data-page="'+current+'"]');
      if(active) active.classList.add('active');
    }
  });
</script>
</body>
</html>'''

for slug, label, subtitle in nav_pages:
    filename = f'{slug}.html'
    html = base_html_template.replace('__TITLE__', label)
    html = html.replace('__STYLE__', base_style)
    html = html.replace('__PAGE__', slug)
    html = html.replace('__NAV__', nav_html)
    html = html.replace('__LABEL__', label)
    html = html.replace('__SUBTITLE__', subtitle)
    html = html.replace('__PAGE_MAP__', page_map_entries)
    with open(filename, 'w', encoding='utf-8') as f:
        f.write(html)

print('Created', len(nav_pages), 'pages')
