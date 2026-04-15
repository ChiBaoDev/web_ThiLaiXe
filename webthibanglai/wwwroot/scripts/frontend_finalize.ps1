$ErrorActionPreference = 'Stop'

$files = Get-ChildItem -Path . -Filter *.html

foreach ($f in $files) {
    $c = Get-Content -Path $f.FullName -Raw

    # Chuẩn hóa một số chuỗi còn sót tiếng Anh
    $c = $c.Replace('Contact Us', 'Liên hệ')
    $c = $c.Replace('Page Not Found', 'Khong tim thay trang')
    $c = $c.Replace('Make An Appointment To Pass Test & Get A License On The First Try', 'Dat lich tu van on thi bang lai xe may ngay hom nay')
    $c = $c.Replace('Designed By', 'Thiet ke boi')
    $c = $c.Replace('Distributed By:', 'Tuy bien boi:')
    $c = $c.Replace('Email Address', 'Dia chi email')
    $c = $c.Replace('Your Site Name', 'ThiXeMay')
    $c = $c.Replace('Send Lời nhắn', 'Gui loi nhan')
    $c = $c.Replace('Subject', 'Chu de')
    $c = $c.Replace('The contact form is currently inactive. Get a functional and working contact form with Ajax & PHP in a few minutes. Just copy and paste the files, add a little code and you''re done. <a href="https://htmlcodex.com/contact-form">Download Now</a>.', 'Bieu mau lien he dang o ban giao dien mau. O giai doan ASP.NET MVC, du lieu se duoc gui qua API va luu tren SQL Server.')
    $c = $c.Replace('Weâ€™re sorry, the page you have looked for does not exist in our website! Maybe go to our home page or try to use a search?', 'Xin loi, trang ban tim khong ton tai. Vui long quay lai trang chu de tiep tuc thi thu.')

    # Chèn menu Thi thử nếu chưa có
    if ($c -notmatch 'href="exam\.html"\s+class="nav-item nav-link') {
        $c = [regex]::Replace(
            $c,
            '<a href="contact\.html" class="nav-item nav-link( active)?">Liên hệ</a>',
            '<a href="exam.html" class="nav-item nav-link">Thi thử</a>`r`n                <a href="contact.html" class="nav-item nav-link$1">Liên hệ</a>',
            1
        )
    }

    # CTA về trang thi thử
    $c = $c.Replace(
        '<a href="" class="btn btn-primary py-4 px-lg-5 d-none d-lg-block">Bắt đầu thi thử<i class="fa fa-arrow-right ms-3"></i></a>',
        '<a href="exam.html" class="btn btn-primary py-4 px-lg-5 d-none d-lg-block">Bắt đầu thi thử<i class="fa fa-arrow-right ms-3"></i></a>'
    )

    # Nút xem thêm ở phần bộ đề về trang thi
    $c = $c.Replace('<a class="btn btn-outline-primary border-2" href="">Xem thêm</a>', '<a class="btn btn-outline-primary border-2" href="exam.html">Xem thêm</a>')

    Set-Content -Path $f.FullName -Value $c -Encoding UTF8
}

Write-Host "Updated $($files.Count) HTML files."