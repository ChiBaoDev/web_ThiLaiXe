$ErrorActionPreference = 'Stop'

$files = Get-ChildItem -Path . -Filter *.html

$replacements = @{
    'Contact Us' = 'Liên hệ'
    'Page Not Found' = 'Không tìm thấy trang'
    'Learn To Drive With Confidence' = 'Luyện thi bằng lái xe máy tự tin hơn'
    'Safe Driving Is Our Top Priority' = 'Ôn tập an toàn giao thông là ưu tiên hàng đầu'
    'Learn More' = 'Xem chi tiết'
    'Our Courses' = 'Bộ đề thi'
    'Easy Driving Learn' = 'Học nhanh, nhớ lâu'
    'National Instructor' = 'Giảng viên toàn quốc'
    'Get licence' = 'Đạt bằng lái'
    'About Us' = 'Về chúng tôi'
    'We Help Students To Pass Test & Get A License On The First Try' = 'Giúp bạn ôn thi bằng lái xe máy và tăng tỷ lệ đậu ngay lần đầu'
    'Fully Licensed' = 'Nội dung chuẩn quy định'
    'Online Tracking' = 'Theo dõi kết quả trực tuyến'
    'Afordable Fee' = 'Học phí hợp lý'
    'Best Trainers' = 'Đội ngũ kinh nghiệm'
    'Tranding Courses' = 'Bộ đề nổi bật'
    'Our Courses Upskill You With Driving Training' = 'Bộ đề thi thử cho hạng A1 và A'
    'Automatic Car Lessons' = 'Đề thi thử hạng A1'
    'Highway Driving Lesson' = 'Đề thi thử hạng A'
    'International Driving' = 'Mẹo làm bài lý thuyết'
    'Beginner' = 'Cơ bản'
    '3 Week' = '3 tuần'
    'Read More' = 'Xem thêm'
    'Make An Appointment To Pass Test & Get A License On The First Try' = 'Đặt lịch tư vấn ôn thi bằng lái xe máy ngay hôm nay'
    'Designed By <a href="https://htmlcodex.com">HTML Codex</a>' = 'Thiết kế giao diện gốc bởi <a href="https://htmlcodex.com">HTML Codex</a>'
    '<br>Distributed By: <a href="https://themewagon.com" target="_blank">ThemeWagon</a>' = '<br>Tùy biến cho hệ thống thi bằng lái xe máy'
    'Weâ€™re sorry, the page you have looked for does not exist in our website! Maybe go to our home page or try to use a search?' = 'Xin lỗi, trang bạn tìm không tồn tại. Vui lòng quay lại trang chủ để tiếp tục sử dụng hệ thống thi thử.'
    'The contact form is currently inactive. Get a functional and working contact form with Ajax & PHP in a few minutes. Just copy and paste the files, add a little code and you''re done. <a href="https://htmlcodex.com/contact-form">Download Now</a>.' = 'Biểu mẫu liên hệ hiện đang ở chế độ giao diện mẫu. Ở giai đoạn ASP.NET MVC, dữ liệu sẽ được gửi qua API và lưu trên SQL Server.'
}

foreach ($f in $files) {
    $c = Get-Content -Path $f.FullName -Raw

    foreach ($k in $replacements.Keys) {
        $c = $c.Replace($k, $replacements[$k])
    }

    if ($c -notmatch 'href="exam.html" class="nav-item nav-link') {
        $c = [regex]::Replace(
            $c,
            '(<a href="contact.html" class="nav-item nav-link(?: active)?">Liên hệ</a>)',
            '<a href="exam.html" class="nav-item nav-link">Thi thử</a>`r`n                $1',
            1
        )
    }

    $c = $c.Replace(
        '<a href="" class="btn btn-primary py-4 px-lg-5 d-none d-lg-block">Bắt đầu thi thử<i class="fa fa-arrow-right ms-3"></i></a>',
        '<a href="exam.html" class="btn btn-primary py-4 px-lg-5 d-none d-lg-block">Bắt đầu thi thử<i class="fa fa-arrow-right ms-3"></i></a>'
    )

    $c = $c.Replace(
        '<a href="" class="btn btn-outline-primary border-2"',
        '<a href="exam.html" class="btn btn-outline-primary border-2"'
    )

    Set-Content -Path $f.FullName -Value $c -Encoding UTF8
}

Write-Host "Updated $($files.Count) HTML files."