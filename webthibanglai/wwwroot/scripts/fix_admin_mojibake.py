from pathlib import Path
import html
import re

ADMIN_DIR = Path('admin')
HTML_FILES = sorted(ADMIN_DIR.glob('*.html'))

BROKEN_MARKERS = (
    'Ã', 'Â', 'Ä', 'Å', 'Æ', 'Ç', 'È', 'É', 'Ê', 'Ë', 'Ì', 'Í', 'Î', 'Ï',
    'Ð', 'Ñ', 'Ò', 'Ó', 'Ô', 'Õ', 'Ö', 'Ø', 'Ù', 'Ú', 'Û', 'Ü', 'Ý', 'Þ', 'ß',
    'à', 'áº', 'á»', 'â', 'ã', 'ä', 'å', 'æ', 'ç', 'è', 'é', 'ê', 'ë', 'ì', 'í',
    'î', 'ï', 'ð', 'ñ', 'ò', 'ó', 'ô', 'õ', 'ö'
)

COMMON_REPLACEMENTS = {
    'ðŸ‘¨â€\x8dðŸ\x8f«': '👨‍🏫',
    'ðŸ“\x9d': '📝',
    'ðŸ“\x8b': '📋',
    'ðŸ“\x8a': '📊',
    'ðŸ“\x85': '📅',
    'ðŸ“\x98': '📘',
    'ðŸ“\x9a': '📚',
    'ðŸ“\x88': '📈',
    'ðŸ§¾': '🧾',
    'ðŸ†': '🏆',
    'ðŸ\x81': '🏁',
    'ðŸ ': '🏠',
    'ðŸ”\x8d': '🔍',
    'âš™ï¸\x8f': '⚙️',
    'âš ï¸': '⚠️',
    'âš ï¸\x8f': '⚠️',
    'âŒ': '❌',
    'â“': '❓',
    'â³': '⏳',
    'â±ï¸\x8f': '⏱️',
    'â†‘': '↑',
    'â†’': '→',
    'Â·': '·',
    'â€”': '—',
    'â€“': '–',
    'â€œ': '“',
    'â€\x9d': '”',
    'â€\x98': '‘',
    'â€\x99': '’',
    'ï¼‹': '＋',
    'â•': '═',
}

CANONICAL_PHRASES = [
    'Hệ thống đào tạo & cấp giấy phép lái xe — Cập nhật: Thứ Ba, 10/03/2026 02:24',
    'Hệ thống đào tạo & cấp giấy phép lái xe — Cập nhật: Thứ Ba, 10/03/2026 02:24',
    'Bảng điều khiển',
    'Lịch học & Lịch thi',
    'Lịch học & Lịch thi',
    'Lịch học và lịch thi',
    'Thông báo',
    'Đào tạo',
    'Tổng quan',
    'Học viên',
    'Khóa học',
    'Giáo viên',
    'Giáo trình',
    'Thi cử',
    'Đề thi & Sát hạch',
    'Đề thi & Sát hạch',
    'Kết quả thi',
    'Cấp GPLX',
    'Quản lý',
    'Phương tiện',
    'Học phí & Thanh toán',
    'Học phí & Thanh toán',
    'Cài đặt hệ thống',
    'Đang hoạt động',
    'Tìm kiếm nhanh...',
    '＋ Thêm học viên',
    'Tính năng sẽ được phát triển',
    'Nhắc nhở hệ thống',
    'Còn 5 học viên chưa nộp đủ hồ sơ. Kỳ thi sát hạch lý thuyết diễn ra vào ',
    'Xem chi tiết',
    'Học viên đang học',
    'Thi sát hạch tháng này',
    'Tỷ lệ đậu bình quân',
    'Hồ sơ chờ xử lý',
    'Học viên đăng ký gần đây',
    'Lọc nhanh...',
    'Họ tên',
    'Hạng bằng',
    'Trạng thái',
    'Học phí',
    'Đang học',
    'Chờ lý thuyết',
    'Chuẩn bị thi',
    'Đã đậu',
    'Chưa khai giảng',
    'Chưa nộp',
    'Xem tất cả →',
    'Về trang chủ',
    'Đăng xuất',
    'Toàn màn hình',
    'Quản trị viên',
    'Hệ thống / ',
]

DIRECT_REPLACEMENTS = {
    'Lá»‹ch há»c & Lá»‹ch thi': 'Lịch học & Lịch thi',
    'ÄÃ o táº¡o': 'Đào tạo',
    'Äá» thi & SÃ¡t háº¡ch': 'Đề thi & Sát hạch',
    'Vá» trang chá»§': 'Về trang chủ',
    'ÄÄƒng xuáº¥t': 'Đăng xuất',
    'Äang hoáº¡t Ä‘á»™ng': 'Đang hoạt động',
    '＋ ThÃªm há»c viÃªn': '＋ Thêm học viên',
    'ThÃªm há»c viÃªn': 'Thêm học viên',
    'CÃ²n 5 há»c viÃªn chÆ°a ná»™p Ä‘á»§ há»“ sÆ¡. Ká»³ thi sÃ¡t háº¡ch lÃ½ thuyáº¿t diá»…n ra vÃ o ': 'Còn 5 học viên chưa nộp đủ hồ sơ. Kỳ thi sát hạch lý thuyết diễn ra vào ',
    'Báº£ng Ä‘iá»u khiá»ƒn': 'Bảng điều khiển',
    'Há»c viÃªn Ä‘ang há»c': 'Học viên đang học',
    'Học viên Ä‘ang há»c': 'Học viên đang học',
    'Há»“ sÆ¡ chá» xá»­ lÃ½': 'Hồ sơ chờ xử lý',
    'Há»c viÃªn Ä‘Äƒng kÃ½ gáº§n Ä‘Ã¢y': 'Học viên đăng ký gần đây',
    'Học viên Ä‘Äƒng kÃ½ gáº§n Ä‘Ã¢y': 'Học viên đăng ký gần đây',
    'Há» tÃªn': 'Họ tên',
    'KhoÃ¡ há»c': 'Khoá học',
    'Äang há»c': 'Đang học',
    'Chá» lÃ½ thuyáº¿t': 'Chờ lý thuyết',
    'LÃª VÄƒn Äá»©c HÃ¹ng': 'Lê Văn Đức Hùng',
    'ÄÃ£ Ä‘áº­u': 'Đã đậu',
    'Lá»c nhanh...': 'Lọc nhanh...',
}

TEXT_RE = re.compile(r'>([^<>]+)<')
ATTR_RE = re.compile(r'((?:title|placeholder|aria-label|alt|value)\s*=\s*["\'])(.*?)(["\'])', re.IGNORECASE)
COMMENT_RE = re.compile(r'<!--(.*?)-->', re.DOTALL)


def contains_broken(text: str) -> bool:
    return any(marker in text for marker in BROKEN_MARKERS)


def misencode_once(text: str) -> str:
    return text.encode('utf-8').decode('latin1')


def variant_spellings(text: str) -> set[str]:
    variants = {text}
    escaped = html.escape(text, quote=False)
    variants.add(escaped)

    for base in list(variants):
        try:
            variants.add(misencode_once(base))
        except UnicodeDecodeError:
            pass
        try:
            first = misencode_once(base)
            variants.add(misencode_once(first))
        except UnicodeDecodeError:
            pass
    return {item for item in variants if item and item != text}


def build_phrase_replacements() -> dict[str, str]:
    replacements: dict[str, str] = {}
    for phrase in CANONICAL_PHRASES:
        for broken in variant_spellings(phrase):
            replacements[broken] = phrase
    return dict(sorted(replacements.items(), key=lambda item: len(item[0]), reverse=True))


PHRASE_REPLACEMENTS = build_phrase_replacements()


def fix_once(text: str) -> str:
    for encoding in ('latin1', 'cp1252'):
        try:
            return text.encode(encoding, errors='strict').decode('utf-8', errors='strict')
        except (UnicodeEncodeError, UnicodeDecodeError):
            continue
    return text


def normalize_entities(text: str) -> str:
    decoded = html.unescape(text)
    return decoded.replace('\xa0', ' ')


def apply_common_replacements(text: str) -> str:
    value = text
    for broken, fixed in COMMON_REPLACEMENTS.items():
        value = value.replace(broken, fixed)
    for broken, fixed in DIRECT_REPLACEMENTS.items():
        value = value.replace(broken, fixed)
    for broken, fixed in PHRASE_REPLACEMENTS.items():
        value = value.replace(broken, fixed)
    return value


def repair_text_piece(text: str) -> str:
    if not text:
        return text

    value = apply_common_replacements(text)

    for _ in range(3):
        candidate = normalize_entities(fix_once(value))
        candidate = apply_common_replacements(candidate)
        if candidate == value:
            break
        value = candidate
        if not contains_broken(value):
            break

    return value


def repair_html_document(source: str) -> str:
    repaired = apply_common_replacements(source)

    repaired = ATTR_RE.sub(
        lambda m: f"{m.group(1)}{repair_text_piece(m.group(2))}{m.group(3)}",
        repaired,
    )
    repaired = TEXT_RE.sub(
        lambda m: f">{repair_text_piece(m.group(1))}<",
        repaired,
    )
    repaired = COMMENT_RE.sub(
        lambda m: f"<!--{repair_text_piece(m.group(1))}-->",
        repaired,
    )

    repaired = apply_common_replacements(repaired)
    return repaired


def main() -> None:
    changed = []
    for path in HTML_FILES:
        original = path.read_text(encoding='utf-8-sig')
        repaired = repair_html_document(original)
        if repaired != original:
            path.write_text(repaired, encoding='utf-8-sig')
            changed.append(path.as_posix())

    print(f'Changed {len(changed)} file(s).')
    for item in changed:
        print(item)


if __name__ == '__main__':
    main()
