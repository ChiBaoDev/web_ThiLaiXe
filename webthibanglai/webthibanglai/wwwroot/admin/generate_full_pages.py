from pathlib import Path
import re

# Đọc file template đầy đủ
template_path = Path('driving-school-dotnet.html')
if not template_path.exists():
    raise FileNotFoundError('driving-school-dotnet.html not found')

full_template = template_path.read_text(encoding='utf-8')

# Danh sách các module/trang cần tạo
nav_pages = [
    ('dashboard', 'Dashboard', 'Bảng điều khiển', 'Hệ thống đào tạo &amp; cấp giấy phép lái xe — Cập nhật: Thứ Ba, 10/03/2026 02:24', '📊', '＋ Thêm học viên'),
    ('lich-hoc-lich-thi', 'Lịch học &amp; Lịch thi', 'Lịch học và lịch thi', 'Quản lý lịch học và lịch thi đồng bộ với khoá học', '📅', '＋ Thêm lịch'),
    ('thong-bao', 'Thông báo', 'Thông báo', 'Trung tâm thông báo toàn bộ hệ thống', '🔔', '＋ Gửi thông báo'),
    ('dao-tao', 'Đào tạo', 'Đào tạo', 'Tổng quan mô-đun đào tạo', '🎓', '＋ Thêm khoá học'),
    ('hoc-vien', 'Học viên', 'Học viên', 'Quản lý học viên', '👥', '＋ Thêm học viên'),
    ('khoa-hoc', 'Khóa học', 'Khóa học', 'Quản lý khóa học', '📚', '＋ Thêm khoá'),
    ('giao-vien', 'Giáo viên', 'Giáo viên', 'Quản lý giáo viên', '👨‍🏫', '＋ Thêm giáo viên'),
    ('giao-trinh', 'Giáo trình', 'Giáo trình', 'Quản lý giáo trình', '📘', '＋ Thêm giáo trình'),
    ('thi-cu', 'Thi cử', 'Thi cử', 'Tổng quan thi cử', '📝', '＋ Thêm kỳ thi'),
    ('de-thi-sat-hach', 'Đề thi &amp; Sát hạch', 'Đề thi &amp; Sát hạch', 'Quản lý đề thi và sát hạch', '🧾', '＋ Thêm đề thi'),
    ('ket-qua-thi', 'Kết quả thi', 'Kết quả thi', 'Kết quả thi gần nhất', '📈', '＋ Nhập điểm'),
    ('cap-gplx', 'Cấp GPLX', 'Cấp GPLX', 'Quản lý cấp giấy phép lái xe', '🏆', '＋ Cấp GPLX'),
    ('quan-ly', 'Quản lý', 'Quản lý', 'Các công cụ quản lý chung', '⚙️', '＋ Cấu hình'),
    ('phuong-tien', 'Phương tiện', 'Phương tiện', 'Quản lý đội xe', '🚙', '＋ Thêm xe'),
    ('hoc-phi-thanh-toan', 'Học phí &amp; Thanh toán', 'Học phí &amp; Thanh toán', 'Quản lý học phí và thanh toán', '💰', '＋ Thêm ghi chú'),
]

def generate_nav_html(current_page):
    """Tạo HTML navigation sidebar với active state hiện tại"""
    nav_html = '''  <div class="nav-section">
    <div class="nav-label">Tổng quan</div>
    <div class="nav-item{}" data-page="dashboard" onclick="setActive(this)">
      <span class="nav-icon">📊</span> Dashboard
    </div>
    <div class="nav-item{}" data-page="lich-hoc-lich-thi" onclick="setActive(this)">
      <span class="nav-icon">📅</span> Lịch học &amp; Lịch thi
    </div>
    <div class="nav-item{}" data-page="thong-bao" onclick="setActive(this)">
      <span class="nav-icon">🔔</span> Thông báo
      <span class="nav-badge">3</span>
    </div>
  </div>

  <div class="nav-section">
    <div class="nav-label">Đào tạo</div>
    <div class="nav-item{}" data-page="hoc-vien" onclick="setActive(this)">
      <span class="nav-icon">👥</span> Học viên
    </div>
    <div class="nav-item{}" data-page="khoa-hoc" onclick="setActive(this)">
      <span class="nav-icon">📚</span> Khóa học
    </div>
    <div class="nav-item{}" data-page="giao-vien" onclick="setActive(this)">
      <span class="nav-icon">👨‍🏫</span> Giáo viên
    </div>
    <div class="nav-item{}" data-page="giao-trinh" onclick="setActive(this)">
      <span class="nav-icon">📘</span> Giáo trình
    </div>
  </div>

  <div class="nav-section">
    <div class="nav-label">Thi cử</div>
    <div class="nav-item{}" data-page="thi-cu" onclick="setActive(this)">
      <span class="nav-icon">📝</span> Thi cử
    </div>
    <div class="nav-item{}" data-page="de-thi-sat-hach" onclick="setActive(this)">
      <span class="nav-icon">🧾</span> Đề thi &amp; Sát hạch
    </div>
    <div class="nav-item{}" data-page="ket-qua-thi" onclick="setActive(this)">
      <span class="nav-icon">📈</span> Kết quả thi
    </div>
  </div>

  <div class="nav-section">
    <div class="nav-label">Quản lý</div>
    <div class="nav-item{}" data-page="phuong-tien" onclick="setActive(this)">
      <span class="nav-icon">🚙</span> Phương tiện
    </div>
    <div class="nav-item{}" data-page="hoc-phi-thanh-toan" onclick="setActive(this)">
      <span class="nav-icon">💰</span> Học phí &amp; Thanh toán
    </div>
    <div class="nav-item{}" data-page="quan-ly" onclick="setActive(this)">
      <span class="nav-icon">⚙️</span> Cài đặt hệ thống
    </div>
  </div>
'''.format(
        ' active' if current_page == 'dashboard' else '',
        ' active' if current_page == 'lich-hoc-lich-thi' else '',
        ' active' if current_page == 'thong-bao' else '',
        ' active' if current_page == 'hoc-vien' else '',
        ' active' if current_page == 'khoa-hoc' else '',
        ' active' if current_page == 'giao-vien' else '',
        ' active' if current_page == 'giao-trinh' else '',
        ' active' if current_page == 'thi-cu' else '',
        ' active' if current_page == 'de-thi-sat-hach' else '',
        ' active' if current_page == 'ket-qua-thi' else '',
        ' active' if current_page == 'phuong-tien' else '',
        ' active' if current_page == 'hoc-phi-thanh-toan' else '',
        ' active' if current_page == 'quan-ly' else '',
    )
    return nav_html

# Để đơn giản, tạo trang cho mỗi module
for slug, nav_label, title, subtitle_desc, icon, btn_label in nav_pages:
    # Start with the full template
    html = full_template
    
    # Replace title in browser tab
    html = html.replace(
        '<title>HệThống Đào Tạo Lái Xe — GPLX Portal</title>',
        f'<title>{title} — GPLX Portal</title>'
    )
    
    # Replace breadcrumb
    html = html.replace(
        '<strong>Dashboard</strong>',
        f'<strong>{nav_label}</strong>'
    )
    
    # Replace page header h1
    html = html.replace(
        '<h1>Bảng điều khiển</h1>',
        f'<h1>{title}</h1>'
    )
    
    # Replace page subtitle
    html = html.replace(
        '<p>Hệ thống đào tạo &amp; cấp giấy phép lái xe — Cập nhật: Thứ Ba, 10/03/2026 02:24</p>',
        f'<p>{subtitle_desc}</p>'
    )
    
    # Replace primary button label
    html = html.replace(
        '<button class="btn btn-primary" onclick="openAddModal()">',
        f'<button class="btn btn-primary" onclick="alert(\'{btn_label} - Tính năng sẽ được phát triển\')">'
    )
    html = html.replace(
        '＋ Thêm học viên',
        btn_label
    )
    
    # Replace data-page attribute in body
    html = html.replace(
        '<body>',
        f'<body data-page="{slug}">'
    )
    
    # Replace sidebar navigation
    nav_start = html.find('  <div class="nav-section">')
    if nav_start != -1:
        # Find the end of all nav-sections (before sidebar-footer)
        nav_end = html.find('  <div class="sidebar-footer">', nav_start)
        if nav_end != -1:
            old_nav = html[nav_start:nav_end]
            new_nav = generate_nav_html(slug)
            html = html[:nav_start] + new_nav + html[nav_end:]
    
    # Write the file
    output_path = Path(f'{slug}.html')
    output_path.write_text(html, encoding='utf-8')
    print(f'✓ Tạo {output_path} ({len(html)} bytes)')

print(f'\n✅ Đã tạo xong {len(nav_pages)} trang với nội dung đầy đủ!')
