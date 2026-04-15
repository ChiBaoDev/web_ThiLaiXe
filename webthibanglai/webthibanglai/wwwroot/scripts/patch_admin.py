#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
patch_admin.py
Thêm admin-auth.js và cập nhật sidebar footer cho tất cả trang admin/
"""
import os, re

ADMIN_DIR = os.path.join(os.path.dirname(__file__), '..', 'admin')

# Sidebar footer mới với id cho JS và nút đăng xuất
NEW_FOOTER = '''  <div class="sidebar-footer">
    <a href="../index.html" style="display:flex;align-items:center;gap:8px;padding:8px 10px;border-radius:8px;color:var(--text-secondary);font-size:12px;font-weight:500;text-decoration:none;margin-bottom:6px;transition:all .18s" onmouseover="this.style.background='var(--bg-hover)';this.style.color='var(--text-primary)'" onmouseout="this.style.background='transparent';this.style.color='var(--text-secondary)'">
      <span style="font-size:15px">🏠</span> Về trang chủ
    </a>
    <div class="user-card" style="cursor:default">
      <div class="user-avatar" id="admin-user-avatar">AD</div>
      <div class="user-info">
        <div class="user-name" id="admin-user-name">Admin</div>
        <div class="user-role" id="admin-user-role">Quản trị viên</div>
      </div>
      <span data-action="logout" title="Đăng xuất" style="color:var(--text-muted);font-size:15px;cursor:pointer" onmouseover="this.style.color='#ef4444'" onmouseout="this.style.color='var(--text-muted)'">🚪</span>
    </div>
  </div>'''

AUTH_SCRIPT = '<script src="../js/admin-auth.js"></script>'

# Pattern để tìm sidebar-footer cũ (bất kỳ nội dung nào)
FOOTER_PATTERN = re.compile(
    r'<div class="sidebar-footer">.*?</div>\s*</div>\s*(?=</aside>)',
    re.DOTALL
)

skip = {'driving-school-dotnet.html', 'generate_full_pages.py', 'generate_pages.py',
        'temp_gen.py', 'update_scripts.py'}

updated = []
skipped = []

for fname in sorted(os.listdir(ADMIN_DIR)):
    if fname in skip or not fname.endswith('.html'):
        skipped.append(fname)
        continue

    fpath = os.path.join(ADMIN_DIR, fname)
    with open(fpath, 'r', encoding='utf-8') as f:
        content = f.read()

    changed = False

    # 1. Thay sidebar-footer
    if 'sidebar-footer' in content:
        new_content = FOOTER_PATTERN.sub(NEW_FOOTER + '\n</aside>', content)
        if new_content != content:
            content = new_content
            changed = True
            print(f'  [footer] {fname}')
        else:
            # Fallback: tìm pattern đơn giản hơn
            old_pattern = re.compile(
                r'<div class="sidebar-footer">[\s\S]*?</div>\s*\n</aside>',
                re.DOTALL
            )
            new_content2 = old_pattern.sub(NEW_FOOTER + '\n</aside>', content)
            if new_content2 != content:
                content = new_content2
                changed = True
                print(f'  [footer-fallback] {fname}')
            else:
                print(f'  [footer-SKIP no match] {fname}')

    # 2. Thêm admin-auth.js trước </body> nếu chưa có
    if 'admin-auth.js' not in content:
        content = content.replace('</body>', AUTH_SCRIPT + '\n</body>')
        changed = True
        print(f'  [auth-script] {fname}')

    if changed:
        with open(fpath, 'w', encoding='utf-8') as f:
            f.write(content)
        updated.append(fname)
    else:
        print(f'  [no change] {fname}')

print(f'\nDone: {len(updated)} updated, {len(skipped)} skipped')
print('Updated:', updated)
