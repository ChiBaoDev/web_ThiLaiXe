import os

ROOT = os.path.dirname(os.path.dirname(__file__))


def inject(path: str, tag: str):
    with open(path, 'r', encoding='utf-8') as f:
        c = f.read()
    if tag in c:
        return False
    if '</body>' not in c:
        return False
    c = c.replace('</body>', tag + '\n</body>')
    with open(path, 'w', encoding='utf-8') as f:
        f.write(c)
    return True

changed = []

# admin.html
p = os.path.join(ROOT, 'admin.html')
if inject(p, '<script src="js/admin-ui-sync.js"></script>'):
    changed.append('admin.html')

# admin/*.html
admin_dir = os.path.join(ROOT, 'admin')
for fn in sorted(os.listdir(admin_dir)):
    if not fn.endswith('.html'):
        continue
    fp = os.path.join(admin_dir, fn)
    if inject(fp, '<script src="../js/admin-ui-sync.js"></script>'):
        changed.append('admin/' + fn)

print('changed:', len(changed))
for x in changed:
    print(x)
