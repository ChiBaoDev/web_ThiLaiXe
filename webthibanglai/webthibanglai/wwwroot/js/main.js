(function ($) {
    "use strict";

    // Spinner
    var spinner = function () {
        setTimeout(function () {
            if ($('#spinner').length > 0) {
                $('#spinner').removeClass('show');
            }
        }, 1);
    };
    spinner();

    // Initiate wowjs
    new WOW().init();

    // Sticky Navbar
    $(window).scroll(function () {
        if ($(this).scrollTop() > 20) {
            $('.sticky-top').addClass('shadow-sm').css('top', '0px');
        } else {
            $('.sticky-top').removeClass('shadow-sm').css('top', '0px');
        }
    });

    // Back to top button
    $(window).scroll(function () {
        if ($(this).scrollTop() > 300) {
            $('.back-to-top').fadeIn('slow');
        } else {
            $('.back-to-top').fadeOut('slow');
        }
    });
    $('.back-to-top').click(function () {
        $('html, body').animate({ scrollTop: 0 }, 1500, 'easeInOutExpo');
        return false;
    });

    // Testimonials carousel
    $(".testimonial-carousel").owlCarousel({
        autoplay: true,
        smartSpeed: 1000,
        items: 1,
        dots: true,
        loop: true,
    });

    // Việt hóa fallback + thêm menu Thi thử cho toàn site
    const textMap = {
        'Contact Us': 'Liên hệ',
        'Page Not Found': 'Không tìm thấy trang',
        'Designed By': 'Thiết kế bởi',
        'Distributed By:': 'Tùy biến bởi:',
        'Your Site Name': 'ThiXeMay',
        'Email Address': 'Địa chỉ email',
        'Subject': 'Chủ đề',
        'Send Message': 'Gửi lời nhắn',
        'Send Lời nhắn': 'Gửi lời nhắn',
        'Learn More': 'Xem chi tiết',
        'Our Courses': 'Bộ đề thi',
        'Previous': 'Trước',
        'Next': 'Sau',
        'Download Now': 'Tải ngay',
        'Leave a message here': 'Nhập lời nhắn tại đây',
        'Weâ€™re sorry, the page you have looked for does not exist in our website! Maybe go to our home page or try to use a search?': 'Xin lỗi, trang bạn tìm không tồn tại. Vui lòng quay lại trang chủ để tiếp tục thi thử.',
        "The contact form is currently inactive. Get a functional and working contact form with Ajax & PHP in a few minutes. Just copy and paste the files, add a little code and you're done.": 'Biểu mẫu liên hệ hiện ở bản giao diện mẫu. Giai đoạn ASP.NET MVC sẽ gửi dữ liệu qua API và lưu trên SQL Server.',
        'Make An Appointment To Pass Test & Get A License On The First Try': 'Đặt lịch tư vấn ôn thi bằng lái xe máy ngay hôm nay'
    };

    function replaceTextNodes(root) {
        const walker = document.createTreeWalker(root, NodeFilter.SHOW_TEXT, null);
        let node;
        while ((node = walker.nextNode())) {
            const value = node.nodeValue && node.nodeValue.trim();
            if (value && textMap[value]) {
                node.nodeValue = node.nodeValue.replace(value, textMap[value]);
            }
        }
    }

    function replaceAttributes() {
        document.querySelectorAll('[placeholder], [value]').forEach((el) => {
            if (el.placeholder && textMap[el.placeholder]) el.placeholder = textMap[el.placeholder];
            if (el.value && textMap[el.value]) el.value = textMap[el.value];
        });
    }

    function ensureExamNavLink() {
        const navList = document.querySelector('.navbar-nav');
        if (!navList) return;
        if (!navList.querySelector('a[href="exam.html"]')) {
            const contactLink = navList.querySelector('a[href="contact.html"]');
            const examLink = document.createElement('a');
            examLink.href = 'exam.html';
            examLink.className = 'nav-item nav-link';
            examLink.textContent = 'Thi thử';
            if (contactLink) {
                navList.insertBefore(examLink, contactLink);
            } else {
                navList.appendChild(examLink);
            }
        }

        document.querySelectorAll('a.btn.btn-primary').forEach((btn) => {
            if (btn.textContent.includes('Bắt đầu thi thử')) btn.href = 'exam.html';
        });
    }

    document.addEventListener('DOMContentLoaded', function () {
        replaceTextNodes(document.body);
        replaceAttributes();
        ensureExamNavLink();
    });

})(jQuery);
