$ErrorActionPreference = 'Stop'
$files = Get-ChildItem -Path . -Filter *.html

foreach ($f in $files) {
    $c = Get-Content -Path $f.FullName -Raw

    $c = $c.Replace('Contact Us', 'Liên hệ')
    $c = $c.Replace('Your Site Name', 'ThiXeMay')
    $c = $c.Replace('Designed By', 'Thiết kế bởi')
    $c = $c.Replace('Distributed By:', 'Tùy biến bởi:')
    $c = $c.Replace('Page Not Found', 'Không tìm thấy trang')
    $c = $c.Replace('Subject', 'Chủ đề')
    $c = $c.Replace('Send Lời nhắn', 'Gửi lời nhắn')
    $c = $c.Replace('Make An Appointment To Pass Test & Get A License On The First Try', 'Đặt lịch tư vấn ôn thi bằng lái xe máy ngay hôm nay')

    if ($c -notmatch 'href="exam\.html" class="nav-item nav-link') {
        $c = [regex]::Replace(
            $c,
            '<a href="contact.html" class="nav-item nav-link( active)?">Liên hệ</a>',
            '<a href="exam.html" class="nav-item nav-link">Thi thử</a>`r`n                <a href="contact.html" class="nav-item nav-link$1">Liên hệ</a>',
            1
        )
    }

    $c = $c.Replace('href="" class="btn btn-primary py-4 px-lg-5 d-none d-lg-block">Bắt đầu thi thử', 'href="exam.html" class="btn btn-primary py-4 px-lg-5 d-none d-lg-block">Bắt đầu thi thử')

    Set-Content -Path $f.FullName -Value $c -Encoding UTF8
}

Write-Host "done"