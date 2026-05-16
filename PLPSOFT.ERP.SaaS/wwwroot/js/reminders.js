/**
 * Hệ thống nhắc hẹn lịch chăm sóc khách hàng
 * - Tự động polling API mỗi 60 giây
 * - Hiển thị toast notification khi có nhắc hẹn mới
 * - Cập nhật badge trên chuông và dropdown danh sách
 */
(function () {
    'use strict';

    const POLL_INTERVAL = 60000; // 60 giây
    const API_URL = '/CRM/Schedules/AllReminders';

    // Lưu trữ ID đã hiển thị toast để tránh lặp
    let notifiedIds = new Set();
    let isFirstLoad = true;

    /**
     * Gọi API lấy danh sách nhắc hẹn
     */
    function fetchReminders() {
        $.ajax({
            url: API_URL,
            type: 'GET',
            dataType: 'json',
            success: function (data) {
                updateBellBadge(data);
                updateDropdownList(data);
                showNewToasts(data);
                isFirstLoad = false;
            },
            error: function (xhr, status, error) {
                console.warn('[Reminders] Không thể tải nhắc hẹn:', error);
            }
        });
    }

    /**
     * Cập nhật badge số lượng trên chuông
     */
    function updateBellBadge(reminders) {
        var badge = document.getElementById('reminderBadge');
        if (!badge) return;

        if (reminders && reminders.length > 0) {
            badge.textContent = reminders.length > 9 ? '9+' : reminders.length;
            badge.classList.remove('d-none');

            // Thêm animation nhấp nháy khi có reminder mới
            var bell = document.getElementById('btnReminderBell');
            if (bell) {
                bell.classList.add('bell-ringing');
                setTimeout(function () { bell.classList.remove('bell-ringing'); }, 2000);
            }
        } else {
            badge.classList.add('d-none');
        }
    }

    /**
     * Cập nhật danh sách trong dropdown
     */
    function updateDropdownList(reminders) {
        var container = document.getElementById('reminderList');
        if (!container) return;

        if (!reminders || reminders.length === 0) {
            container.innerHTML =
                '<div class="text-center text-muted py-3">' +
                '  <div style="font-size: 2rem;">✅</div>' +
                '  <small>Không có nhắc hẹn nào</small>' +
                '</div>';
            return;
        }

        var html = '';
        for (var i = 0; i < reminders.length; i++) {
            var r = reminders[i];
            var typeClass = r.reminderType === 'STARTING' ? 'reminder-starting' : 'reminder-ending';
            var typeIcon = r.reminderType === 'STARTING' ? '🟢' : '🟠';
            var typeLabel = r.reminderType === 'STARTING' ? 'Sắp bắt đầu' : 'Sắp kết thúc';
            var timeStr = r.reminderType === 'STARTING'
                ? formatDateTime(r.startTime)
                : formatDateTime(r.endTime);

            html +=
                '<a href="/CRM/Schedules/Details/' + r.scheduleID + '" class="dropdown-item reminder-item ' + typeClass + '" style="white-space: normal;">' +
                '  <div class="d-flex align-items-start">' +
                '    <span class="me-2" style="font-size: 1.2rem;">' + typeIcon + '</span>' +
                '    <div class="flex-grow-1">' +
                '      <div class="fw-bold text-truncate" style="max-width: 280px;">' + escapeHtml(r.title) + '</div>' +
                '      <div class="small text-muted">👤 ' + escapeHtml(r.customerName) + '</div>' +
                '      <div class="small">' +
                '        <span class="badge ' + (r.reminderType === 'STARTING' ? 'bg-success' : 'bg-warning text-dark') + '">' + typeLabel + '</span>' +
                '        <span class="ms-1 text-muted">⏱️ ' + r.minutesRemaining + ' phút</span>' +
                '      </div>' +
                '      <div class="small text-muted mt-1">🕐 ' + timeStr + '</div>' +
                '    </div>' +
                '  </div>' +
                '</a>' +
                (i < reminders.length - 1 ? '<hr class="dropdown-divider my-1">' : '');
        }

        container.innerHTML = html;
    }

    /**
     * Hiển thị toast cho nhắc hẹn mới
     */
    function showNewToasts(reminders) {
        if (!reminders || reminders.length === 0) return;
        // Không hiển thị toast lần tải đầu tiên (tránh spam khi mới mở trang)
        if (isFirstLoad && reminders.length > 3) {
            // Nếu có quá nhiều, chỉ hiện 1 toast tổng hợp
            showToast(
                '🔔 Bạn có ' + reminders.length + ' nhắc hẹn',
                'Nhấn vào chuông 🔔 trên thanh menu để xem chi tiết.',
                'info',
                null
            );
            reminders.forEach(function (r) {
                notifiedIds.add(r.scheduleID + '_' + r.reminderType);
            });
            return;
        }

        for (var i = 0; i < reminders.length; i++) {
            var r = reminders[i];
            var key = r.scheduleID + '_' + r.reminderType;

            if (!notifiedIds.has(key)) {
                notifiedIds.add(key);

                var bgClass = r.reminderType === 'STARTING' ? 'success' : 'warning';
                showToast(
                    r.reminderType === 'STARTING' ? '⏰ Sắp đến lịch!' : '⚠️ Sắp hết lịch!',
                    r.message + '\n👤 KH: ' + r.customerName,
                    bgClass,
                    r.scheduleID
                );
            }
        }
    }

    /**
     * Tạo và hiển thị toast notification
     */
    function showToast(title, message, bgClass, scheduleId) {
        var container = document.getElementById('toastContainer');
        if (!container) return;

        var toastId = 'toast_' + Date.now() + '_' + Math.random().toString(36).substr(2, 5);
        var clickAction = scheduleId ? ' onclick="window.location.href=\'/CRM/Schedules/Details/' + scheduleId + '\'" style="cursor: pointer;"' : '';

        var headerBg = bgClass === 'success' ? 'bg-success text-white'
            : bgClass === 'warning' ? 'bg-warning text-dark'
            : 'bg-info text-white';

        var toastHtml =
            '<div id="' + toastId + '" class="toast toast-reminder" role="alert" aria-live="assertive" aria-atomic="true" data-bs-autohide="true" data-bs-delay="10000"' + clickAction + '>' +
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
        var toast = new bootstrap.Toast(toastEl);
        toast.show();

        // Phát âm thanh thông báo nhẹ
        playNotificationSound();

        // Xóa khỏi DOM khi ẩn
        toastEl.addEventListener('hidden.bs.toast', function () {
            toastEl.remove();
        });
    }

    /**
     * Phát âm thanh thông báo nhẹ bằng Web Audio API
     */
    function playNotificationSound() {
        try {
            var audioCtx = new (window.AudioContext || window.webkitAudioContext)();
            var oscillator = audioCtx.createOscillator();
            var gainNode = audioCtx.createGain();

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
        } catch (e) {
            // Web Audio API không khả dụng - bỏ qua
        }
    }

    /**
     * Format DateTime từ JSON
     */
    function formatDateTime(dateStr) {
        if (!dateStr) return '';
        var d = new Date(dateStr);
        var day = String(d.getDate()).padStart(2, '0');
        var month = String(d.getMonth() + 1).padStart(2, '0');
        var year = d.getFullYear();
        var hours = String(d.getHours()).padStart(2, '0');
        var minutes = String(d.getMinutes()).padStart(2, '0');
        return day + '/' + month + '/' + year + ' ' + hours + ':' + minutes;
    }

    /**
     * Escape HTML để tránh XSS
     */
    function escapeHtml(text) {
        if (!text) return '';
        var div = document.createElement('div');
        div.appendChild(document.createTextNode(text));
        return div.innerHTML;
    }

    // Khởi chạy
    $(document).ready(function () {
        // Tải nhắc hẹn lần đầu
        fetchReminders();

        // Polling mỗi 60 giây
        setInterval(fetchReminders, POLL_INTERVAL);

        // Refresh khi click vào chuông
        $('#btnReminderBell').on('click', function () {
            fetchReminders();
        });
    });
})();
