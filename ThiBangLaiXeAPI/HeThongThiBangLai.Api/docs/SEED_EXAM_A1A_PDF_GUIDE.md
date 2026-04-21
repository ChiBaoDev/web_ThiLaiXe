# Seed dữ liệu đề thi A1/A theo PDF 250 câu

Tài liệu này mô tả cách dùng script [`02_seed_exam_a1a_from_pdf.sql`](../db/02_seed_exam_a1a_from_pdf.sql) để tạo dữ liệu test đầy đủ cho flow thi thử.

## 1) Những gì đã được seed

Script [`02_seed_exam_a1a_from_pdf.sql`](../db/02_seed_exam_a1a_from_pdf.sql) tạo các nhóm dữ liệu sau:

- **Topic ngân hàng câu hỏi** (`chu_de_cau_hoi`)
  - `CD_QTGT`: Quy tắc giao thông
  - `CD_LIET`: Câu điểm liệt
  - `CD_VH`: Văn hóa/đạo đức
  - `CD_KT`: Kỹ thuật/cấu tạo
  - `CD_BH`: Báo hiệu đường bộ
  - `CD_SH`: Sa hình/tình huống
- **37 câu hỏi mẫu** (`cau_hoi`) với prefix nội dung `[PDF-A1A-xxx]`
  - có gắn `la_cau_diem_liet = 1` cho nhóm câu liệt
  - trạng thái `trang_thai = 'approved'` để thỏa điều kiện publish đề
- **Đáp án trắc nghiệm** (`dap_an`)
  - mỗi câu 4 đáp án, đúng 1 đáp án
- **Kỳ thi / ca thi / đề thi mẫu**
  - kỳ thi id cố định `260401`
  - ca thi id cố định `260401` (để tương thích luồng hiện tại)
  - đề `DE_A1A_PDF_25_01`, `tong_so_cau = 25`, `thoi_gian_lam_bai = 19`, `trang_thai = 'published'`
- **Cấu trúc đề 25 câu** (`de_thi_cau_hoi`) theo phân bổ logic:
  - 08 quy tắc + 01 liệt + 01 văn hóa + 01 kỹ thuật + 08 báo hiệu + 06 sa hình
- **Exam structure rule** dạng log (`nhat_ky_he_thong`) cho API quản lý rule:
  - `totalQuestions=25`
  - `durationMinutes=19`
  - `passingCorrectAnswers=21`
  - `requiredCriticalQuestions=1`

## 2) Mapping rule từ PDF sang seed

Từ nội dung trong [`250-cau-hoi-thi-ly-thuyet-lai-xe-moto-tt.pdf`](../Example/250-cau-hoi-thi-ly-thuyet-lai-xe-moto-tt.pdf), script map theo hướng test backend:

- Bộ câu hỏi có nhóm `CÂU LIỆT` → map vào `la_cau_diem_liet`.
- Đề chuẩn 25 câu, thời gian 19 phút → map vào `de_thi.tong_so_cau`, `de_thi.thoi_gian_lam_bai`.
- Cấu trúc đề theo nhóm chủ đề → map vào `de_thi_cau_hoi` + payload rule.
- Điều kiện fail khi sai câu liệt đã được backend áp dụng trong [`GetPassingCorrectAnswers()`](../Services/Exams/ExamSessionService.cs:274) và logic submit trong [`SubmitAsync()`](../Services/Exams/ExamSessionService.cs:155).

## 3) Cách chạy trong SSMS

1. Đảm bảo DB đã dựng bằng [`01_full_setup_database_team.sql`](../db/01_full_setup_database_team.sql).
2. Mở và chạy [`02_seed_exam_a1a_from_pdf.sql`](../db/02_seed_exam_a1a_from_pdf.sql) trong database `he_thong_thi_bang_lai`.
3. Script có transaction + rollback khi lỗi.

## 4) Checklist verify bằng SQL

Chạy các truy vấn sau sau khi seed:

```sql
USE [he_thong_thi_bang_lai];
GO

-- 1) Topic
SELECT ma_chu_de, ten_chu_de
FROM chu_de_cau_hoi
WHERE ma_chu_de LIKE 'CD_%'
ORDER BY ma_chu_de;

-- 2) Tổng câu hỏi PDF-A1A
SELECT COUNT(*) AS tong_cau_pdf
FROM cau_hoi
WHERE noi_dung LIKE N'[PDF-A1A-%]%';

-- 3) Kiểm tra câu điểm liệt
SELECT COUNT(*) AS so_cau_liet
FROM cau_hoi
WHERE noi_dung LIKE N'[PDF-A1A-%]%'
  AND la_cau_diem_liet = 1;

-- 4) Mỗi câu có 4 đáp án
SELECT TOP 10 c.id, c.noi_dung, COUNT(a.id) AS so_dap_an
FROM cau_hoi c
JOIN dap_an a ON a.cau_hoi_id = c.id
WHERE c.noi_dung LIKE N'[PDF-A1A-%]%'
GROUP BY c.id, c.noi_dung
ORDER BY c.id;

-- 5) Đề thi mẫu
SELECT d.id, d.ma_de_thi, d.tong_so_cau, d.thoi_gian_lam_bai, d.trang_thai,
       COUNT(dtch.id) AS so_cau_da_gan
FROM de_thi d
LEFT JOIN de_thi_cau_hoi dtch ON dtch.de_thi_id = d.id
WHERE d.ma_de_thi = 'DE_A1A_PDF_25_01'
GROUP BY d.id, d.ma_de_thi, d.tong_so_cau, d.thoi_gian_lam_bai, d.trang_thai;

-- 6) Rule log
SELECT TOP 5 id, hanh_dong, bang_tac_dong, khoa_chinh_du_lieu, created_at
FROM nhat_ky_he_thong
WHERE bang_tac_dong = 'exam_structure_rule'
ORDER BY created_at DESC;
```

## 5) Checklist verify bằng Swagger

1. Đăng nhập tài khoản học viên (đã có `hoc_vien`).
2. Gọi API danh sách đề thi mẫu và lấy `id` của `DE_A1A_PDF_25_01`.
3. Start exam session.
4. Trả lời thử:
   - case A: làm đúng >= 21, không sai câu liệt → kết quả pass.
   - case B: sai 1 câu liệt → kết quả fail.
5. Gọi API kết quả/review để kiểm tra:
   - `FailedByCriticalQuestion = true` với case B.

## 6) Lưu ý giới hạn hiện tại

- Script này seed **dữ liệu mô phỏng logic** theo cấu trúc PDF, không nhập full nguyên văn đủ 250 câu.
- Trong code hiện tại, ngưỡng pass cho đề 25 câu đang hard-code về 21 tại [`GetPassingCorrectAnswers()`](../Services/Exams/ExamSessionService.cs:274).
- Vì vậy khác biệt A1 (21/25) vs A (23/25) **chưa được tách theo hạng** trong flow chấm điểm hiện tại.
- Script đã tạo `ky_thi.id = ca_thi.id` để tương thích với gán `ca_thi_id = sampleExam.ky_thi_id` trong [`StartSampleExamAsync()`](../Services/Exams/ExamSessionService.cs:19).
