/**
 * Hệ thống thông báo CRM:
 *   1. Nhắc hẹn lịch chăm sóc khách hàng (polling /CRM/Schedules/AllReminders)
 *   2. Thông báo khiếu nại mới — COMPLAINT (polling /CRM/Feedbacks/ComplaintAlerts)
 *
 * Cả hai kênh dùng chung một chuông (bell) trên navbar:
 *   - Badge hiển thị tổng số chưa đọc
 *   - Dropdown phân thành 2 nhóm rõ ràng
 *   - Toast góc dưới phải cho thông báo mới
 */
(function () {
    'use strict';

    const POLL_INTERVAL         = 60000;                           // 60 giây
    const REMINDER_API_URL      = '/CRM/Schedules/AllReminders';
    const COMPLAINT_API_URL     = '/CRM/Feedbacks/ComplaintAlerts';
    const DISMISS_API_BASE      = '/CRM/Feedbacks/DismissAlert';

    // Lưu trữ ID đã hiển thị toast để tránh lặp
    let notifiedReminderIds  = new Set();
    let notifiedComplaintIds = new Set();
    let isFirstLoad          = true;

    // Cache kết quả mới nhất của 2 kênh
    let latestReminders  = [];
    let latestComplaints = [];

    // =========================================================
    // FETCH — Nhắc hẹn lịch
    // =========================================================
    function fetchReminders() {
        $.ajax({
            url: REMINDER_API_URL,
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                latestReminders = data || [];
                refreshBell();
                showNewReminderToasts(latestReminders);
                if (isFirstLoad) {
                    isFirstLoad = false;
                }
            },
            error: function (xhr, status, error) {
                console.warn('[Reminders] Không thể tải nhắc hẹn:', error);
            }
        });
    }

    // =========================================================
    // FETCH — Thông báo khiếu nại
    // =========================================================
    function fetchComplaintAlerts() {
        $.ajax({
            url: COMPLAINT_API_URL,
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                latestComplaints = data || [];
                refreshBell();
                showNewComplaintToasts(latestComplaints);
            },
            error: function (xhr, status, error) {
                console.warn('[Complaints] Không thể tải thông báo khiếu nại:', error);
            }
        });
    }

    // =========================================================
    // CẬP NHẬT BADGE + DROPDOWN (gọi sau mỗi lần fetch)
    // =========================================================
    function refreshBell() {
        var total = latestReminders.length + latestComplaints.length;
        updateBellBadge(total);
        updateDropdownList(latestReminders, latestComplaints);
    }

    // =========================================================
    // BADGE — Hiển thị tổng số thông báo
    // =========================================================
    function updateBellBadge(total) {
        var badge = document.getElementById('reminderBadge');
        if (!badge) return;

        if (total > 0) {
            badge.textContent = total > 9 ? '9+' : total;
            badge.classList.remove('d-none');

            // Animation nhấp nháy khi có thông báo mới
            var bell = document.getElementById('btnReminderBell');
            if (bell) {
                bell.classList.add('bell-ringing');
                setTimeout(function () { bell.classList.remove('bell-ringing'); }, 2000);
            }
        } else {
            badge.classList.add('d-none');
        }
    }

    // =========================================================
    // DROPDOWN — Hiển thị 2 nhóm: Khiếu nại + Nhắc hẹn
    // =========================================================
    function updateDropdownList(reminders, complaints) {
        var container = document.getElementById('reminderList');
        if (!container) return;

        var html = '';

        // --- NHÓM 1: KHIẾU NẠI MỚI (ưu tiên hiển thị trên cùng) ---
        if (complaints && complaints.length > 0) {
            html += '<div class="px-3 py-1 bg-danger bg-opacity-10 border-bottom">' +
                    '  <small class="fw-bold text-danger">🚨 Khiếu nại mới (' + complaints.length + ')</small>' +
                    '</div>';

            for (var i = 0; i < complaints.length; i++) {
                var c = complaints[i];
                html +=
                    '<div class="dropdown-item complaint-item px-3 py-2" style="white-space: normal; cursor: pointer;" ' +
                    '     onclick="handleComplaintClick(\'' + escapeHtml(c.alertId) + '\', ' + c.feedbackID + ')">' +
                    '  <div class="d-flex align-items-start">' +
                    '    <span class="me-2" style="font-size:1.2rem;">🚨</span>' +
                    '    <div class="flex-grow-1">' +
                    '      <div class="fw-bold text-danger text-truncate" style="max-width:260px;">' + escapeHtml(c.title) + '</div>' +
                    '      <div class="small text-muted">👤 ' + escapeHtml(c.customerName) + '</div>' +
                    '      <div class="small text-muted">🏢 ' + escapeHtml(c.branchName) + '</div>' +
                    '      <div class="mt-1"><span class="badge bg-danger">Khiếu nại</span>' +
                    '      <span class="ms-1 small text-muted">⏱ ' + escapeHtml(c.timeAgo) + '</span></div>' +
                    '    </div>' +
                    '  </div>' +
                    '</div>' +
                    (i < complaints.length - 1 ? '<hr class="dropdown-divider my-1">' : '');
            }
        }

        // --- NHÓM 2: NHẮC HẸN LỊCH ---
        if (reminders && reminders.length > 0) {
            if (complaints && complaints.length > 0) {
                html += '<hr class="dropdown-divider my-1">';
            }
            html += '<div class="px-3 py-1 bg-light border-bottom">' +
                    '  <small class="fw-bold text-secondary">📅 Nhắc hẹn (' + reminders.length + ')</small>' +
                    '</div>';

            for (var j = 0; j < reminders.length; j++) {
                var r = reminders[j];
                var typeClass = r.reminderType === 'STARTING' ? 'reminder-starting' : 'reminder-ending';
                var typeIcon  = r.reminderType === 'STARTING' ? '🟢' : '🟠';
                var typeLabel = r.reminderType === 'STARTING' ? 'Sắp bắt đầu' : 'Sắp kết thúc';
                var timeStr   = r.reminderType === 'STARTING' ? formatDateTime(r.startTime) : formatDateTime(r.endTime);

                html +=
                    '<a href="/CRM/Schedules/Details/' + r.scheduleID + '" class="dropdown-item reminder-item ' + typeClass + '" style="white-space:normal;">' +
                    '  <div class="d-flex align-items-start">' +
                    '    <span class="me-2" style="font-size:1.2rem;">' + typeIcon + '</span>' +
                    '    <div class="flex-grow-1">' +
                    '      <div class="fw-bold text-truncate" style="max-width:260px;">' + escapeHtml(r.title) + '</div>' +
                    '      <div class="small text-muted">👤 ' + escapeHtml(r.customerName) + '</div>' +
                    '      <div class="small">' +
                    '        <span class="badge ' + (r.reminderType === 'STARTING' ? 'bg-success' : 'bg-warning text-dark') + '">' + typeLabel + '</span>' +
                    '        <span class="ms-1 text-muted">⏱ ' + r.minutesRemaining + ' phút</span>' +
                    '      </div>' +
                    '      <div class="small text-muted mt-1">🕐 ' + timeStr + '</div>' +
                    '    </div>' +
                    '  </div>' +
                    '</a>' +
                    (j < reminders.length - 1 ? '<hr class="dropdown-divider my-1">' : '');
            }
        }

        // --- KHÔNG CÓ GÌ ---
        if ((!complaints || complaints.length === 0) && (!reminders || reminders.length === 0)) {
            html =
                '<div class="text-center text-muted py-3">' +
                '  <div style="font-size:2rem;">✅</div>' +
                '  <small>Không có thông báo nào</small>' +
                '</div>';
        }

        container.innerHTML = html;
    }

    // =========================================================
    // XỬ LÝ CLICK VÀO THÔNG BÁO KHIẾU NẠI
    // Dismiss khỏi cache → chuyển đến trang Details
    // =========================================================
    window.handleComplaintClick = function (alertId, feedbackId) {
        // Gọi API dismiss (không cần đợi kết quả)
        $.post(DISMISS_API_BASE + '/' + alertId, function () {
            // Cập nhật lại UI ngay lập tức (xóa khỏi local list)
            latestComplaints = latestComplaints.filter(function (c) { return c.alertId !== alertId; });
            refreshBell();
        });
        // Chuyển đến trang Details ngay
        window.location.href = '/CRM/Feedbacks/Details/' + feedbackId;
    };

    // =========================================================
    // TOAST — Nhắc hẹn lịch (giữ nguyên logic cũ)
    // =========================================================
    function showNewReminderToasts(reminders) {
        if (!reminders || reminders.length === 0) return;
        if (isFirstLoad && reminders.length > 3) {
            showToast(
                '🔔 Bạn có ' + reminders.length + ' nhắc hẹn',
                'Nhấn vào chuông 🔔 để xem chi tiết.',
                'info',
                null,
                null
            );
            reminders.forEach(function (r) { notifiedReminderIds.add(r.scheduleID + '_' + r.reminderType); });
            return;
        }

        for (var i = 0; i < reminders.length; i++) {
            var r = reminders[i];
            var key = r.scheduleID + '_' + r.reminderType;
            if (!notifiedReminderIds.has(key)) {
                notifiedReminderIds.add(key);
                var bgClass = r.reminderType === 'STARTING' ? 'success' : 'warning';
                showToast(
                    r.reminderType === 'STARTING' ? '⏰ Sắp đến lịch!' : '⚠️ Sắp hết lịch!',
                    r.message + '\n👤 KH: ' + r.customerName,
                    bgClass,
                    r.scheduleID,
                    'schedule'
                );
            }
        }
    }

    // =========================================================
    // TOAST — Thông báo khiếu nại mới (màu đỏ)
    // =========================================================
    function showNewComplaintToasts(complaints) {
        if (!complaints || complaints.length === 0) return;

        for (var i = 0; i < complaints.length; i++) {
            var c = complaints[i];
            if (!notifiedComplaintIds.has(c.alertId)) {
                notifiedComplaintIds.add(c.alertId);
                showToast(
                    '🚨 Khiếu nại mới!',
                    '👤 ' + c.customerName + '\n📋 ' + c.title,
                    'danger',
                    c.feedbackID,
                    'feedback'
                );
            }
        }
    }

    // =========================================================
    // TẠO TOAST NOTIFICATION
    // =========================================================
    function showToast(title, message, bgClass, itemId, itemType) {
        var container = document.getElementById('toastContainer');
        if (!container) return;

        var toastId     = 'toast_' + Date.now() + '_' + Math.random().toString(36).substr(2, 5);
        var clickHref   = itemId && itemType === 'schedule'  ? '/CRM/Schedules/Details/' + itemId
                        : itemId && itemType === 'feedback'  ? '/CRM/Feedbacks/Details/' + itemId
                        : null;
        var clickAction = clickHref ? ' onclick="window.location.href=\'' + clickHref + '\'" style="cursor:pointer;"' : '';

        var headerBg = bgClass === 'success' ? 'bg-success text-white'
                     : bgClass === 'warning' ? 'bg-warning text-dark'
                     : bgClass === 'danger'  ? 'bg-danger text-white'
                     : 'bg-info text-white';

        var toastHtml =
            '<div id="' + toastId + '" class="toast toast-reminder" role="alert" aria-live="assertive" aria-atomic="true" data-bs-autohide="true" data-bs-delay="2000"' + clickAction + '>' +
            '  <div class="toast-header ' + headerBg + '">' +
            '    <strong class="me-auto">' + escapeHtml(title) + '</strong>' +
            '    <small>Vừa xong</small>' +
            '    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" aria-label="Close" onclick="event.stopPropagation();"></button>' +
            '  </div>' +
            '  <div class="toast-body">' +
            '    ' + escapeHtml(message).replace(/\n/g, '<br>') +
            '  </div>' +
            '</div>';

        container.insertAdjacentHTML('beforeend', toastHtml);

        var toastEl = document.getElementById(toastId);
        new bootstrap.Toast(toastEl).show();

        playNotificationSound();

        toastEl.addEventListener('hidden.bs.toast', function () { toastEl.remove(); });
    }

    // =========================================================
    // ÂM THANH THÔNG BÁO
    // =========================================================
    function playNotificationSound() {
        try {
            var audioCtx    = new (window.AudioContext || window.webkitAudioContext)();
            var oscillator  = audioCtx.createOscillator();
            var gainNode    = audioCtx.createGain();

            oscillator.connect(gainNode);
            gainNode.connect(audioCtx.destination);

            oscillator.type = 'sine';
            oscillator.frequency.setValueAtTime(800, audioCtx.currentTime);
            oscillator.frequency.setValueAtTime(600, audioCtx.currentTime + 0.1);
            oscillator.frequency.setValueAtTime(800, audioCtx.currentTime + 0.2);

            gainNode.gain.setValueAtTime(0.1, audioCtx.currentTime);
            gainNode.gain.exponentialRampToValueAtTime(0.01, audioCtx.currentTime + 0.4);

            oscillator.start(audioCtx.currentTime);
            oscillator.stop(audioCtx.currentTime + 0.4);
        } catch (e) { /* Web Audio API không khả dụng */ }
    }

    // =========================================================
    // FORMAT NGÀY GIỜ
    // =========================================================
    function formatDateTime(dateStr) {
        if (!dateStr) return '';
        var d       = new Date(dateStr);
        var day     = String(d.getDate()).padStart(2, '0');
        var month   = String(d.getMonth() + 1).padStart(2, '0');
        var year    = d.getFullYear();
        var hours   = String(d.getHours()).padStart(2, '0');
        var minutes = String(d.getMinutes()).padStart(2, '0');
        return day + '/' + month + '/' + year + ' ' + hours + ':' + minutes;
    }

    // =========================================================
    // ESCAPE HTML (chống XSS)
    // =========================================================
    function escapeHtml(text) {
        if (!text) return '';
        var div = document.createElement('div');
        div.appendChild(document.createTextNode(String(text)));
        return div.innerHTML;
    }

    // =========================================================
    // KHỞI CHẠY
    // =========================================================
    $(document).ready(function () {
        // Tải lần đầu: cả 2 kênh
        fetchReminders();
        fetchComplaintAlerts();

        // Polling mỗi 60 giây
        setInterval(function () {
            fetchReminders();
            fetchComplaintAlerts();
        }, POLL_INTERVAL);

        // Refresh ngay khi nhấn chuông
        $('#btnReminderBell').on('click', function () {
            fetchReminders();
            fetchComplaintAlerts();
        });
    });

})();