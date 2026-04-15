from pathlib import Path

# Danh sách tất cả trang
pages = [
    'dashboard', 'lich-hoc-lich-thi', 'thong-bao', 'dao-tao', 'hoc-vien', 
    'khoa-hoc', 'giao-vien', 'giao-trinh', 'thi-cu', 'de-thi-sat-hach',
    'ket-qua-thi', 'cap-gplx', 'quan-ly', 'phuong-tien', 'hoc-phi-thanh-toan'
]

# Script chuyển hướng đúng
correct_script = '''<script>
  function setActive(el) {
    document.querySelectorAll('.nav-item').forEach(i => i.classList.remove('active'));
    el.classList.add('active');
    const page = el.dataset.page;
    if (page && page !== document.body.dataset.page) {
      location.href = page + '.html';
    }
  }
  function openAddModal() {
    const ov = document.getElementById('modal-overlay');
    if (ov) ov.style.display = 'flex';
  }
  function closeModal() {
    const ov = document.getElementById('modal-overlay');
    if (ov) ov.style.display = 'none';
  }
  const ov = document.getElementById('modal-overlay');
  if (ov) {
    ov.addEventListener('click', function(e) {
      if (e.target === this) closeModal();
    });
  }
  window.addEventListener('DOMContentLoaded', function() {
    const current = document.body.dataset.page;
    if (current) {
      const active = document.querySelector('.nav-item[data-page="' + current + '"]');
      if (active) active.classList.add('active');
    }
  });
</script>'''

for page in pages:
    path = Path(f'{page}.html')
    if not path.exists():
        print(f'⚠️  {page}.html không tìm thấy')
        continue
    
    content = path.read_text(encoding='utf-8')
    
    # Tìm và thay thế script block
    import re
    script_pattern = r'<script>[\s\S]*?</script>'
    
    if re.search(script_pattern, content):
        content = re.sub(script_pattern, correct_script, content)
        path.write_text(content, encoding='utf-8')
        print(f'✅ Cập nhật {page}.html')
    else:
        print(f'⚠️  {page}.html không có script block')

print('\n✅ Hoàn thành cập nhật tất cả trang!')
