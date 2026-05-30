// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

$(document).ready(function () {
    // Dynamic Sleek Modern Confirm Modal
    $(document).on('submit', 'form[data-confirm]', function (e) {
        var form = this;
        if (form.dataset.confirmed === 'true') {
            return true;
        }

        e.preventDefault();
        var message = form.getAttribute('data-confirm') || 'Bạn có chắc chắn muốn thực hiện thao tác này?';
        
        // Custom look based on action or theme
        var action = form.getAttribute('action') || '';
        var theme = 'primary';
        var icon = 'fa-question-circle';
        
        if (action.includes('Complete') || message.includes('hoàn thành')) {
            theme = 'success';
            icon = 'fa-check-circle';
        } else if (action.includes('Cancel') || message.includes('hủy')) {
            theme = 'warning';
            icon = 'fa-exclamation-triangle';
        } else if (action.includes('Delete') || message.includes('xóa')) {
            theme = 'danger';
            icon = 'fa-trash-alt';
        }

        showModernConfirm(message, theme, icon, function () {
            form.dataset.confirmed = 'true';
            form.submit();
        });
    });
});

/**
 * Dynamically builds and displays a premium customized confirmation modal
 * @param {string} message The message to show
 * @param {string} theme The theme color (primary, success, warning, danger)
 * @param {string} icon The FontAwesome icon name
 * @param {function} onConfirm Callback when user clicks confirm
 */
function showModernConfirm(message, theme, icon, onConfirm) {
    // Remove existing modal if any
    var existingModal = document.getElementById('modernConfirmModal');
    if (existingModal) {
        existingModal.remove();
    }

    var btnClass = 'btn-' + theme;
    var iconHtml = '<i class="fas ' + icon + ' fa-3x text-' + theme + ' mb-3 animate__animated animate__zoomIn"></i>';

    var modalHtml = 
        '<div class="modal fade" id="modernConfirmModal" tabindex="-1" aria-hidden="true">' +
        '  <div class="modal-dialog modal-dialog-centered" style="max-width: 400px;">' +
        '    <div class="modal-content border-0 shadow-lg" style="border-radius: 16px; overflow: hidden;">' +
        '      <div class="modal-body text-center p-4 bg-white">' +
        '        ' + iconHtml +
        '        <h5 class="fw-bold mb-2 text-dark">Xác nhận thao tác</h5>' +
        '        <p class="text-muted mb-4 px-2" style="font-size: 0.95rem; line-height: 1.5;">' + message + '</p>' +
        '        <div class="d-flex gap-2 justify-content-center">' +
        '          <button type="button" class="btn btn-light px-4 py-2" data-bs-dismiss="modal" style="border-radius: 8px;">Hủy bỏ</button>' +
        '          <button type="button" id="btnConfirmSubmit" class="btn ' + btnClass + ' btn-confirm px-4 py-2 text-white" style="border-radius: 8px;">Đồng ý</button>' +
        '        </div>' +
        '      </div>' +
        '    </div>' +
        '  </div>' +
        '</div>';

    document.body.insertAdjacentHTML('beforeend', modalHtml);

    var modalEl = document.getElementById('modernConfirmModal');
    var modal = new bootstrap.Modal(modalEl);
    
    document.getElementById('btnConfirmSubmit').addEventListener('click', function () {
        modal.hide();
        setTimeout(onConfirm, 250); // Allow modal hide animation
    });

    modal.show();
}
