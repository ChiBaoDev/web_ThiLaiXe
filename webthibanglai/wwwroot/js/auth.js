(function () {
    const AUTH_KEY = 'thiXeMayAuth';
    const USERS_KEY = 'thiXeMayUsers';
    const RESULT_KEY = 'thiXeMayLastResult';

    const DEFAULT_USERS = {
        admin: {
            username: 'admin',
            password: 'admin123',
            role: 'admin',
            fullName: 'Quản trị viên',
            avatar: 'Q',
            studyCount: 12
        },
        user: {
            username: 'user',
            password: 'user123',
            role: 'user',
            fullName: 'Học viên Nguyễn Văn A',
            avatar: 'U',
            studyCount: 0,
            registeredCourse: {
                name: 'Khóa học A1 cơ bản',
                description: 'Lộ trình dành cho học viên mới bắt đầu, học lý thuyết, biển báo và thực hành sa hình theo từng buổi.',
                startDate: '15/04/2026',
                schedule: 'Thứ 2 - Thứ 4 - Thứ 6, 18:30',
                teacher: 'Thầy Trần Minh Khang',
                nextLesson: 'Thứ 4, 18:30 - Luyện đề và giải đáp án',
                status: 'Đang học'
            }
        }
    };

    function getPageType() {
        return window.location.pathname.toLowerCase().endsWith('.html') ? 'static' : 'mvc';
    }

    function getUrl(name) {
        const urls = {
            login: '/Login',
            profile: '/Login/Profile',
            admin: '/admin/dashboard.html',
            home: '/',
            about: '/About',
            courses: '/KhoaHoc',
            lichHoc: '/LichHoc',
            contact: '/Contact',
            exam: '/Exam'
        };

        return urls[name] || urls.home;
    }

    function navigateTo(url) {
        if (window.top && window.top !== window) {
            window.top.location.href = url;
            return;
        }

        window.location.href = url;
    }

    function normalizeStaticNavigation() {
        const mappings = {
            'index.html': getUrl('home'),
            'about.html': getUrl('about'),
            'courses.html': getUrl('courses'),
            'contact.html': getUrl('contact'),
            'exam.html': getUrl('exam'),
            'login.html': getUrl('login'),
            'profile.html': getUrl('profile')
        };

        Object.entries(mappings).forEach(([from, to]) => {
            document.querySelectorAll(`a[href="${from}"]`).forEach(link => {
                link.setAttribute('href', to);
                if (window.top && window.top !== window) {
                    link.setAttribute('target', '_top');
                }
            });
        });
    }

    function parseJson(value, fallback) {
        try {
            return JSON.parse(value);
        } catch {
            return fallback;
        }
    }

    function getUsers() {
        const stored = parseJson(localStorage.getItem(USERS_KEY), null);
        if (!stored || typeof stored !== 'object') {
            localStorage.setItem(USERS_KEY, JSON.stringify(DEFAULT_USERS));
            return JSON.parse(JSON.stringify(DEFAULT_USERS));
        }

        const merged = JSON.parse(JSON.stringify(DEFAULT_USERS));
        Object.keys(stored).forEach(key => {
            merged[key] = { ...merged[key], ...stored[key] };
        });
        return merged;
    }

    function saveUsers(users) {
        localStorage.setItem(USERS_KEY, JSON.stringify(users));
    }

    function getAuth() {
        return parseJson(localStorage.getItem(AUTH_KEY), null);
    }

    function setAuth(data) {
        localStorage.setItem(AUTH_KEY, JSON.stringify(data));
    }

    function clearAuth() {
        localStorage.removeItem(AUTH_KEY);
    }

    function syncAuthWithUsers() {
        const auth = getAuth();
        if (!auth) return null;

        const users = getUsers();
        const latest = Object.values(users).find(user => user.username === auth.username);
        if (!latest) return auth;

        const nextAuth = {
            ...auth,
            fullName: latest.fullName,
            avatar: latest.avatar,
            studyCount: latest.studyCount || 0,
            registeredCourse: latest.registeredCourse || null
        };

        setAuth(nextAuth);
        return nextAuth;
    }

    function getLastResult() {
        return parseJson(localStorage.getItem(RESULT_KEY), null);
    }

    function updateStudyStats() {
        const auth = getAuth();
        const result = getLastResult();
        if (!auth || !result) return;

        const countedKey = `thiXeMayResultCounted:${result.generatedAt}:${auth.username}`;
        if (sessionStorage.getItem(countedKey) === '1') return;

        const users = getUsers();
        const targetEntry = Object.keys(users).find(key => users[key].username === auth.username);
        if (!targetEntry) return;

        users[targetEntry].studyCount = (users[targetEntry].studyCount || 0) + 1;
        saveUsers(users);
        sessionStorage.setItem(countedKey, '1');
        syncAuthWithUsers();
    }

    function login(username, password) {
        const u = (username || '').trim().toLowerCase();
        const p = (password || '').trim();
        const users = getUsers();
        const found = Object.values(users).find(x => x.username === u && x.password === p);
        if (!found) return { ok: false, message: 'Sai tài khoản hoặc mật khẩu.' };

        const auth = {
            username: found.username,
            fullName: found.fullName,
            role: found.role,
            avatar: found.avatar || found.fullName?.charAt(0)?.toUpperCase() || found.username.charAt(0).toUpperCase(),
            studyCount: found.studyCount || 0,
            registeredCourse: found.registeredCourse || null,
            loginAt: new Date().toISOString()
        };
        setAuth(auth);
        return { ok: true, auth };
    }

    function logout() {
        clearAuth();
        navigateTo(getUrl('login'));
    }

    function createNavLink(href, text, key, className) {
        const a = document.createElement('a');
        const currentPath = window.location.pathname.toLowerCase().replace(/\/$/, '') || '/';
        const targetPath = (href || '').toLowerCase().replace(/\/$/, '') || '/';
        const isActive = currentPath === targetPath;

        a.href = href;
        a.className = `${className || 'nav-item nav-link'}${isActive ? ' active' : ''}`;
        a.textContent = text;
        a.setAttribute('data-auth', key);
        return a;
    }

    function ensureNavAuth() {
        const nav = document.querySelector('.navbar-nav');
        if (!nav) return;

        const auth = syncAuthWithUsers();
        nav.querySelectorAll('[data-auth]').forEach(item => item.remove());

        if (!auth) {
            nav.appendChild(createNavLink(getUrl('login'), 'Đăng nhập', 'login'));
            return;
        }

        if (auth.role === 'admin' && !nav.querySelector('[data-auth="admin"]')) {
            nav.appendChild(createNavLink(getUrl('admin'), 'Quản trị', 'admin'));
        }

        if (auth.role !== 'admin' && auth.registeredCourse && !nav.querySelector('[data-auth="lich-hoc"]')) {
            nav.appendChild(createNavLink(getUrl('lichHoc'), 'Lịch học', 'lich-hoc'));
        }
 
        const wrapper = document.createElement('div');
        wrapper.className = 'nav-item dropdown';
        wrapper.setAttribute('data-auth', 'profile-menu');

        const quickLink = auth.role === 'admin'
            ? `<a class="dropdown-item" href="${getUrl('admin')}"><i class="fas fa-tools me-2 text-primary"></i>Vào trang quản trị</a>`
            : `<a class="dropdown-item" href="${getUrl('profile')}"><i class="fas fa-id-card me-2 text-primary"></i>Thông tin cá nhân</a>`;

        wrapper.innerHTML = `
            <a href="#" class="nav-link dropdown-toggle d-flex align-items-center user-nav-trigger" data-bs-toggle="dropdown" aria-expanded="false">
                <span>${auth.fullName}</span>
            </a>
            <div class="dropdown-menu dropdown-menu-end bg-light border-0 shadow-sm mt-2 user-nav-dropdown">
                <div class="px-3 py-2 border-bottom">
                    <div class="fw-bold">${auth.fullName}</div>
                    <small class="text-muted">${auth.username} • ${auth.role === 'admin' ? 'Quản trị viên' : 'Học viên'}</small>
                </div>
                ${quickLink}
                <button type="button" class="dropdown-item" data-auth="logout-action"><i class="fas fa-sign-out-alt me-2 text-primary"></i>Đăng xuất</button>
            </div>`;

        const logoutBtn = wrapper.querySelector('[data-auth="logout-action"]');
        logoutBtn.addEventListener('click', function () {
            logout();
        });

        nav.appendChild(wrapper);
    }

    function protectAdminPage() {
        const isAdminPage = /admin(\/dashboard)?\.html$/i.test(window.location.pathname);
        if (!isAdminPage) return;
        const auth = getAuth();
        if (!auth || auth.role !== 'admin') {
            navigateTo(getUrl('login'));
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
                    navigateTo(getUrl('admin'));
                } else {
                    navigateTo(getUrl('profile'));
                }
            }, 500);
        });
    }

    function bindAdminInfo() {
        const box = document.getElementById('adminInfo');
        if (!box) return;
        const auth = syncAuthWithUsers();
        if (!auth) return;
        box.innerHTML = `
            <p class="mb-1"><strong>Tài khoản:</strong> ${auth.username}</p>
            <p class="mb-1"><strong>Vai trò:</strong> ${auth.role}</p>
            <p class="mb-0"><strong>Đăng nhập lúc:</strong> ${new Date(auth.loginAt).toLocaleString('vi-VN')}</p>
        `;
    }

    function fillProfilePage() {
        const page = document.getElementById('profilePage');
        if (!page) return;

        const auth = syncAuthWithUsers();
        if (!auth) {
            navigateTo(getUrl('login'));
            return;
        }

        if (auth.role === 'admin') {
            navigateTo(getUrl('admin'));
            return;
        }

        const lastResult = getLastResult();
        const avatar = document.getElementById('profileAvatar');
        const fullName = document.getElementById('profileFullName');
        const username = document.getElementById('profileUsername');
        const role = document.getElementById('profileRole');
        const loginAt = document.getElementById('profileLoginAt');
        const studyCount = document.getElementById('profileStudyCount');
        const lastScore = document.getElementById('profileLastScore');
        const lastStatus = document.getElementById('profileLastStatus');

        const courseName = document.getElementById('profileCourseName');
        const courseDescription = document.getElementById('profileCourseDescription');
        const courseStartDate = document.getElementById('profileCourseStartDate');
        const courseSchedule = document.getElementById('profileCourseSchedule');
        const courseTeacher = document.getElementById('profileCourseTeacher');
        const scheduleLink = document.getElementById('profileScheduleLink');
        const registeredCourseCount = document.getElementById('profileRegisteredCourseCount');
        const nextLesson = document.getElementById('profileNextLesson');
        const courseStatus = document.getElementById('profileCourseStatus');

        if (avatar) avatar.textContent = auth.avatar || auth.username.charAt(0).toUpperCase();
        if (fullName) fullName.textContent = auth.fullName;
        if (username) username.textContent = auth.username;
        if (role) role.textContent = auth.role === 'admin' ? 'Quản trị viên' : 'Học viên';
        if (loginAt) loginAt.textContent = new Date(auth.loginAt).toLocaleString('vi-VN');
        if (studyCount) studyCount.textContent = String(auth.studyCount || 0);

        if (auth.registeredCourse) {
            if (courseName) courseName.textContent = auth.registeredCourse.name || 'Đã đăng ký khóa học';
            if (courseDescription) courseDescription.textContent = auth.registeredCourse.description || 'Khóa học hiện tại của bạn đang được cập nhật.';
            if (courseStartDate) courseStartDate.textContent = auth.registeredCourse.startDate || 'Chưa cập nhật';
            if (courseSchedule) courseSchedule.textContent = auth.registeredCourse.schedule || 'Chưa cập nhật';
            if (courseTeacher) courseTeacher.textContent = auth.registeredCourse.teacher || 'Chưa cập nhật';
            if (registeredCourseCount) registeredCourseCount.textContent = '1';
            if (nextLesson) nextLesson.textContent = auth.registeredCourse.nextLesson || 'Đang cập nhật';
            if (courseStatus) {
                courseStatus.textContent = auth.registeredCourse.status || 'Đang học';
                courseStatus.className = 'badge bg-success';
            }
            if (scheduleLink) {
                scheduleLink.classList.remove('disabled');
                scheduleLink.removeAttribute('aria-disabled');
                scheduleLink.setAttribute('href', getUrl('lichHoc'));
            }
        } else {
            if (registeredCourseCount) registeredCourseCount.textContent = '0';
            if (nextLesson) nextLesson.textContent = 'Chưa có';
            if (courseStatus) {
                courseStatus.textContent = 'Chưa đăng ký';
                courseStatus.className = 'badge bg-secondary';
            }
        }

        if (lastResult) {
            if (lastScore) lastScore.textContent = `${lastResult.correct}/${lastResult.total}`;
            if (lastStatus) {
                lastStatus.textContent = lastResult.pass ? 'Đạt' : 'Chưa đạt';
                lastStatus.className = lastResult.pass ? 'badge bg-success' : 'badge bg-danger';
            }
        } else {
            if (lastScore) lastScore.textContent = 'Chưa có dữ liệu';
            if (lastStatus) {
                lastStatus.textContent = 'Chưa thi';
                lastStatus.className = 'badge bg-secondary';
            }
        }
    }

    function bindPasswordChangeForm() {
        const form = document.getElementById('changePasswordForm');
        if (!form) return;

        const auth = getAuth();
        if (!auth) return;

        const msg = document.getElementById('changePasswordMsg');
        form.addEventListener('submit', function (e) {
            e.preventDefault();

            const currentPassword = document.getElementById('currentPassword').value.trim();
            const newPassword = document.getElementById('newPassword').value.trim();
            const confirmPassword = document.getElementById('confirmPassword').value.trim();
            const users = getUsers();
            const entryKey = Object.keys(users).find(key => users[key].username === auth.username);

            if (!entryKey) {
                msg.className = 'alert alert-danger';
                msg.textContent = 'Không tìm thấy tài khoản người dùng.';
                return;
            }

            if (users[entryKey].password !== currentPassword) {
                msg.className = 'alert alert-danger';
                msg.textContent = 'Mật khẩu hiện tại không đúng.';
                return;
            }

            if (newPassword.length < 6) {
                msg.className = 'alert alert-danger';
                msg.textContent = 'Mật khẩu mới phải có ít nhất 6 ký tự.';
                return;
            }

            if (newPassword !== confirmPassword) {
                msg.className = 'alert alert-danger';
                msg.textContent = 'Mật khẩu xác nhận không khớp.';
                return;
            }

            users[entryKey].password = newPassword;
            saveUsers(users);
            syncAuthWithUsers();
            form.reset();
            msg.className = 'alert alert-success';
            msg.textContent = 'Đổi mật khẩu thành công.';
        });
    }

    function fillSchedulePage() {
        const page = document.getElementById('lichHocPage');
        if (!page) return;

        const auth = syncAuthWithUsers();
        if (!auth) {
            navigateTo(getUrl('login'));
            return;
        }

        if (auth.role === 'admin') {
            navigateTo(getUrl('admin'));
            return;
        }

        if (!auth.registeredCourse) {
            navigateTo(getUrl('courses'));
            return;
        }

        const course = auth.registeredCourse;
        const courseName = document.getElementById('scheduleCourseName');
        const courseBadge = document.getElementById('scheduleCourseBadge');
        const time = document.getElementById('scheduleTime');
        const teacher = document.getElementById('scheduleTeacher');
        const status = document.getElementById('scheduleStatus');

        if (courseName) courseName.textContent = course.name || 'đang đăng ký';
        if (courseBadge) courseBadge.textContent = course.name || 'Chưa cập nhật';
        if (time) time.textContent = course.schedule || 'Chưa cập nhật';
        if (teacher) teacher.textContent = course.teacher || 'Chưa cập nhật';
        if (status) status.textContent = course.status || 'Đang học';
    }

    function bindProfileActions() {
        const logoutButtons = document.querySelectorAll('[data-profile-logout]');
        logoutButtons.forEach(button => {
            button.addEventListener('click', function () {
                logout();
            });
        });
    }

    document.addEventListener('DOMContentLoaded', function () {
        normalizeStaticNavigation();
        updateStudyStats();
        protectAdminPage();
        ensureNavAuth();
        bindLoginForm();
        bindAdminInfo();
        fillProfilePage();
        fillSchedulePage();
        bindPasswordChangeForm();
        bindProfileActions();
    });
})();
