from pathlib import Path
import re

base_path = Path('dashboard.html')
if not base_path.exists():
    raise FileNotFoundError('dashboard.html not found')
base = base_path.read_text(encoding='utf-8')

style_match = re.search(r'(<style>[\s\S]*?</style>)', base)
if not style_match:
    raise SystemExit('Style not found')
style = style_match.group(1)

main_start = base.index('<main class="content">')
main_end = base.index('</main>', main_start) + len('</main>')
content_block = base[main_start:main_end]
modal_block = ''

nav_pages = [
    ('dashboard', 'Dashboard', 'Tổng quan hệ thống', '📊'),
    ('lich-hoc-lich-thi', 'Lịch học & Lịch thi', 'Lịch học và lịch thi', '📅'),
    ('thong-bao', 'Thông báo', 'Danh sách thông báo', '🔔'),
    ('dao-tao', 'Đào tạo', 'Quản lý đào tạo', '🎓'),
    ('hoc-vien', 'Học viên', 'Quản lý học viên', '👥'),
    ('khoa-hoc', 'Khóa học', 'Quản lý khóa học', '📚'),
    ('giao-vien', 'Giáo viên', 'Quản lý giáo viên', '👨‍🏫'),
    ('giao-trinh', 'Giáo trình', 'Quản lý giáo trình', '📘'),
    ('thi-cu', 'Thi cử', 'Tổng quan thi cử', '📝'),
    ('de-thi-sat-hach', 'Đề thi & Sát hạch', 'Đề thi và sát hạch', '🧾'),
    ('ket-qua-thi', 'Kết quả thi', 'Kết quả thi', '📈'),
    ('cap-gplx', 'Cấp GPLX', 'Cấp GPLX', '🏆'),
    ('quan-ly', 'Quản lý', 'Công cụ quản lý', '⚙️'),
    ('phuong-tien', 'Phương tiện', 'Quản lý phương tiện', '🚙'),
    ('hoc-phi-thanh-toan', 'Học phí & Thanh toán', 'Học phí và thanh toán', '💰'),
]

page_map_entries = ',\n'.join([f"      '{p[0]}': '{p[0]}.html'" for p in nav_pages])

base_template = '''<!DOCTYPE html>
<html lang="vi">
<head>
<meta charset="UTF-8" />
<meta name="viewport" content="width=device-width, initial-scale=1.0" />
<title>{title} — GPLX Portal</title>
{style}
</head>
<body data-page="{page}">
<aside class="sidebar">
  <div class="sidebar-logo">
    <div class="logo-icon">🚗</div>
    <div class="logo-text">
      <strong>GPLX Portal</strong>
      <span>v2.5.1 — .NET 8.0</span>
    </div>
  </div>
{nav}
  <div class="sidebar-footer">
    <div class="user-card">
      <div class="user-avatar">TN</div>
      <div class="user-info">
        <div class="user-name">Trần Nguyễn Admin</div>
        <div class="user-role">Quản trị viên hệ thống</div>
      </div>
      <span style="color:var(--text-muted);font-size:13px">⚙️</span>
    </div>
  </div>
</aside>
<div class="main">
  <header class="header">
    <div class="header-breadcrumb">
      <span style="color:var(--text-muted)">Hệ thống / </span>
      <strong>{title}</strong>
    </div>
    <div class="topbar-tag"><span class="live-dot"></span>Đang hoạt động</div>
    <div class="header-actions">
      <div class="search-bar" style="width:200px">
        <span>🔍</span>
        <input type="text" placeholder="Tìm kiếm nhanh..." />
      </div>
      <div class="icon-btn" title="Thông báo">🔔<span class="notif-dot"></span></div>
      <div class="icon-btn" title="Toàn màn hình">⛶</div>
      <button class="btn btn-primary" onclick="alert('Thêm mới');">＋ Thêm mới</button>
    </div>
  </header>

  {content}
</div>

{modal}

<script>
  function setActive(el) {
    const target = el.dataset.page;
    document.querySelectorAll('.nav-item').forEach(i => i.classList.remove('active'));
    el.classList.add('active');
    const route = {
{page_map}
    };
    if (route[target] && location.pathname.indexOf(route[target]) === -1) {
      location.href = route[target];
    }
  }

  window.addEventListener('DOMContentLoaded', function() {
    const current = document.body.dataset.page;
    if (current) {
      const active = document.querySelector('.nav-item[data-page="' + current + '"]');
      if (active) active.classList.add('active');
    }
  });
</script>
</body>
</html>'''

for slug, label, subtitle, icon in nav_pages:
    page_content = content_block.replace('<h1>Dashboard</h1>', f'<h1>{label}</h1>')
    page_content = page_content.replace('<p> Tổng quan hệ thống</p>', f'<p> {subtitle}</p>')
    page_content = page_content.replace('Trang nội dung mẫu cho <strong>Dashboard</strong>', f'Trang nội dung mẫu cho <strong>{label}</strong>')

    nav_active = '  <div class="nav-section">\n    <div class="nav-label">Tổng quan</div>\n'
    for s, l, _, i in nav_pages:
        active_cls = ' active' if s == slug else ''
        nav_active += f'    <div class="nav-item{active_cls}" data-page="{s}" onclick="setActive(this)">\n      <span class="nav-icon">{i}</span> {l}\n    </div>\n'
    nav_active += '  </div>\n'

    html = (base_template.replace('{title}', label)
                         .replace('{style}', style)
                         .replace('{page}', slug)
                         .replace('{nav}', nav_active)
                         .replace('{content}', page_content)
                         .replace('{page_map}', page_map_entries))
    Path(f'{slug}.html').write_text(html, encoding='utf-8')

print('Created pages with full dashboard-like content')