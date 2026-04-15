/**
 * admin-auth.js
 * Bảo vệ tất cả trang trong thư mục admin/
 * Redirect về ../login.html nếu chưa đăng nhập hoặc không phải admin
 */
(function () {
    'use strict';

    var AUTH_KEY = 'thiXeMayAuth';

    function getAuth() {
        try {
            return JSON.parse(localStorage.getItem(AUTH_KEY) || 'null');
        } catch (e) {
            return null;
        }
    }

    function logout() {
        localStorage.removeItem(AUTH_KEY);
        location.href = '../login.html';
    }

    // Kiểm tra quyền truy cập
    var auth = getAuth();
    if (!auth || auth.role !== 'admin') {
        location.href = '../login.html';
        return;
    }

    // Hiển thị thông tin user trong sidebar footer
    window.addEventListener('DOMContentLoaded', function () {
        // Cập nhật user card trong sidebar
        var nameEl = document.getElementById('admin-user-name');
        var roleEl = document.getElementById('admin-user-role');
        var avatarEl = document.getElementById('admin-user-avatar');

        if (nameEl) nameEl.textContent = auth.username || 'Admin';
        if (roleEl) roleEl.textContent = 'Quản trị viên hệ thống';
        if (avatarEl) {
            var initials = (auth.username || 'A').substring(0, 2).toUpperCase();
            avatarEl.textContent = initials;
        }

        // Gắn sự kiện đăng xuất
        var logoutBtns = document.querySelectorAll('[data-action="logout"]');
        logoutBtns.forEach(function (btn) {
            btn.addEventListener('click', function (e) {
                e.preventDefault();
                if (confirm('Bạn có chắc muốn đăng xuất?')) {
                    logout();
                }
            });
        });
    });

    // Expose logout globally
    window.adminLogout = logout;
})();
