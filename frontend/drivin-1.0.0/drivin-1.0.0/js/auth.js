(function () {
    const KEY = 'thiXeMayAuth';

    const USERS = {
        admin: { username: 'admin', password: 'admin123', role: 'admin', fullName: 'Quản trị viên' },
        user: { username: 'user', password: 'user123', role: 'user', fullName: 'Học viên' }
    };

    function getAuth() {
        try {
            return JSON.parse(localStorage.getItem(KEY) || 'null');
        } catch {
            return null;
        }
    }

    function setAuth(data) {
        localStorage.setItem(KEY, JSON.stringify(data));
    }

    function clearAuth() {
        localStorage.removeItem(KEY);
    }

    function login(username, password) {
        const u = (username || '').trim().toLowerCase();
        const p = (password || '').trim();
        const found = Object.values(USERS).find(x => x.username === u && x.password === p);
        if (!found) return { ok: false, message: 'Sai tài khoản hoặc mật khẩu.' };

        const auth = {
            username: found.username,
            fullName: found.fullName,
            role: found.role,
            loginAt: new Date().toISOString()
        };
        setAuth(auth);
        return { ok: true, auth };
    }

    function ensureNavAuth() {
        const nav = document.querySelector('.navbar-nav');
        if (!nav) return;

        const auth = getAuth();

        const existedLogin = nav.querySelector('a[data-auth="login"]');
        const existedAdmin = nav.querySelector('a[data-auth="admin"]');
        const existedLogout = nav.querySelector('a[data-auth="logout"]');
        if (existedLogin) existedLogin.remove();
        if (existedAdmin) existedAdmin.remove();
        if (existedLogout) existedLogout.remove();

        const makeLink = (href, text, key) => {
            const a = document.createElement('a');
            a.href = href;
            a.className = 'nav-item nav-link';
            a.textContent = text;
            a.setAttribute('data-auth', key);
            return a;
        };

        if (!auth) {
            nav.appendChild(makeLink('login.html', 'Đăng nhập', 'login'));
            return;
        }

        if (auth.role === 'admin' && !nav.querySelector('a[href="admin.html"]')) {
            nav.appendChild(makeLink('admin.html', 'Quản trị', 'admin'));
        }

        const logout = makeLink('#', `Đăng xuất (${auth.username})`, 'logout');
        logout.addEventListener('click', function (e) {
            e.preventDefault();
            clearAuth();
            window.location.href = 'login.html';
        });
        nav.appendChild(logout);
    }

    function protectAdminPage() {
        const isAdminPage = /admin\.html$/i.test(window.location.pathname);
        if (!isAdminPage) return;
        const auth = getAuth();
        if (!auth || auth.role !== 'admin') {
            window.location.href = 'login.html';
        }
    }

    function bindLoginForm() {
        const form = document.getElementById('loginForm');
        if (!form) return;

        const msg = document.getElementById('loginMsg');
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            const username = document.getElementById('username').value;
            const password = document.getElementById('password').value;
            const result = login(username, password);

            if (!result.ok) {
                msg.textContent = result.message;
                msg.className = 'alert alert-danger mt-3';
                return;
            }

            msg.textContent = 'Đăng nhập thành công, đang chuyển trang...';
            msg.className = 'alert alert-success mt-3';
            setTimeout(function () {
                if (result.auth.role === 'admin') {
                    window.location.href = 'admin.html';
                } else {
                    window.location.href = 'exam.html';
                }
            }, 500);
        });
    }

    function bindAdminInfo() {
        const box = document.getElementById('adminInfo');
        if (!box) return;
        const auth = getAuth();
        if (!auth) return;
        box.innerHTML = `
            <p class="mb-1"><strong>Tài khoản:</strong> ${auth.username}</p>
            <p class="mb-1"><strong>Vai trò:</strong> ${auth.role}</p>
            <p class="mb-0"><strong>Đăng nhập lúc:</strong> ${new Date(auth.loginAt).toLocaleString('vi-VN')}</p>
        `;
    }

    document.addEventListener('DOMContentLoaded', function () {
        protectAdminPage();
        ensureNavAuth();
        bindLoginForm();
        bindAdminInfo();
    });
})();