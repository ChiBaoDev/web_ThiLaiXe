(function () {
    'use strict';

    var THEME = {
        primary: '#F3BD00', // css/style.css --primary
        dark: '#0C2B4B',    // css/style.css --dark
        light: '#F3F6F8',
        secondary: '#757575'
    };

    function injectSyncStyle() {
        if (document.getElementById('admin-ui-sync-style')) return;

        var style = document.createElement('style');
        style.id = 'admin-ui-sync-style';
        style.textContent = [
            ':root {',
            '  --primary: ' + THEME.primary + ';',
            '  --dark: ' + THEME.dark + ';',
            '  --light: ' + THEME.light + ';',
            '  --secondary: ' + THEME.secondary + ';',
            '  --accent: ' + THEME.primary + ' !important;',
            '  --accent2: ' + THEME.dark + ' !important;',
            '  --warning: ' + THEME.primary + ' !important;',
            '  --bg-panel: #ffffff;',
            '  --bg-card: #ffffff;',
            '  --bg-hover: #f3f6f8;',
            '  --border: #e5edf5;',
            '  --text-primary: #0C2B4B;',
            '  --text-secondary: #385a7d;',
            '  --text-muted: #6f8aa6;',
            '}',
            '',
            'body {',
            '  background: #ffffff !important;',
            '}',
            '.main, .content { background: #ffffff !important; }',
            '',
            '.sidebar, .header, .card, .stat-card, .user-card, .icon-btn, .search-bar, .gateway-card, .gateway-header, .gateway-body {',
            '  transition: all .5s ease !important;',
            '}',
            '',
            '.nav-item:hover, .card:hover, .stat-card:hover, .module-card:hover {',
            '  box-shadow: 0 8px 20px rgba(12,43,75,.08);',
            '}',
            '',
            '.btn-primary, .btn-dashboard {',
            '  background: var(--primary) !important;',
            '  border-color: var(--primary) !important;',
            '  color: #0C2B4B !important;',
            '  transition: .5s !important;',
            '}',
            '.btn-primary:hover, .btn-dashboard:hover {',
            '  filter: brightness(1.05);',
            '}',
            '',
            '.topbar-tag {',
            '  background: rgba(243,189,0,.15) !important;',
            '  color: #8a6a00 !important;',
            '}',
            '.sidebar { box-shadow: 4px 0 18px rgba(12,43,75,.06); }',
            '.header { box-shadow: 0 2px 12px rgba(12,43,75,.05); }',
            '.nav-item.active { color: var(--dark) !important; background: rgba(243,189,0,.18) !important; }',
            '.nav-item .nav-icon { color: var(--dark) !important; }',
            '.card, .stat-card, .user-card, .search-bar, .icon-btn { border-color: var(--border) !important; }',
            '.gateway-card, .gateway-header, .gateway-body, .session-info, .stat-mini, .module-card { background: #fff !important; border-color: var(--border) !important; }',
            '.module-name, .card-title, .stat-label, .stat-mini-lbl { color: var(--text-secondary) !important; }',
            '',
            '.admin-home-fab {',
            '  position: fixed;',
            '  right: 18px;',
            '  bottom: 18px;',
            '  z-index: 1200;',
            '  display: inline-flex;',
            '  align-items: center;',
            '  gap: 8px;',
            '  padding: 10px 14px;',
            '  border-radius: 999px;',
            '  background: var(--primary);',
            '  color: var(--dark) !important;',
            '  font-weight: 700;',
            '  font-size: 13px;',
            '  text-decoration: none !important;',
            '  box-shadow: 0 8px 22px rgba(243,189,0,.35);',
            '  transition: .3s ease;',
            '}',
            '.admin-home-fab:hover { transform: translateY(-2px); filter: brightness(1.02); }',
            '',
            '@keyframes adminFadeUpSync {',
            '  from { opacity: 0; transform: translateY(14px); }',
            '  to { opacity: 1; transform: translateY(0); }',
            '}',
            '.sync-fade-up {',
            '  animation: adminFadeUpSync .5s ease both;',
            '}',
            '.sync-delay-1 { animation-delay: .05s; }',
            '.sync-delay-2 { animation-delay: .1s; }',
            '.sync-delay-3 { animation-delay: .15s; }',
            '.sync-delay-4 { animation-delay: .2s; }'
        ].join('\n');

        document.head.appendChild(style);
    }

    function ensureHomeFab() {
        if (document.querySelector('.admin-home-fab')) return;

        var a = document.createElement('a');
        a.className = 'admin-home-fab';
        a.href = (location.pathname.indexOf('/admin/') !== -1) ? '../index.html' : 'index.html';
        a.innerHTML = '🏠 <span>Trang chủ</span>';
        document.body.appendChild(a);
    }

    function applyEntryAnimation() {
        var groups = [
            '.stats-grid .stat-card',
            '.stat-row .stat-mini',
            '.content .card',
            '.module-grid .module-card',
            '.row .card',
            '.sidebar .nav-item'
        ];

        groups.forEach(function (selector) {
            var nodes = document.querySelectorAll(selector);
            nodes.forEach(function (el, idx) {
                el.classList.add('sync-fade-up');
                if (idx === 0) el.classList.add('sync-delay-1');
                else if (idx === 1) el.classList.add('sync-delay-2');
                else if (idx === 2) el.classList.add('sync-delay-3');
                else if (idx >= 3) el.classList.add('sync-delay-4');
            });
        });
    }

    document.addEventListener('DOMContentLoaded', function () {
        injectSyncStyle();
        ensureHomeFab();
        applyEntryAnimation();
    });
})();
