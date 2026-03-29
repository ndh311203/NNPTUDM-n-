// Notification system với SignalR
(function () {
    let connection = null;
    let isAdmin = false;
    let userId = null;
    
    // Expose connection globally for debugging
    window.getNotificationConnection = function() {
        return connection;
    };

    // Khởi tạo SignalR connection
    function initSignalR() {
        const isAdminUser = document.body.getAttribute('data-is-admin') === 'true';
        const userIdValue = document.body.getAttribute('data-user-id');
        
        isAdmin = isAdminUser;
        userId = userIdValue;

        if (!isAdmin && !userId) {
            console.log("⚠️ Không kết nối SignalR: không phải admin và không có userId");
            return; // Không phải admin và không có userId
        }

        console.log("🔔 Khởi tạo SignalR - IsAdmin:", isAdmin, "UserId:", userId);

        const hubUrl = '/notificationHub';
        const queryString = isAdmin 
            ? `?isAdmin=true${userId ? '&userId=' + userId : ''}`
            : `?userId=${userId}`;

        connection = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl + queryString)
            .withAutomaticReconnect()
            .build();

        // Nhận thông báo realtime
        connection.on("NhanThongBao", function (notification) {
            console.log("🔔 Nhận thông báo mới qua SignalR:", notification);
            console.log("Loại thông báo:", notification.loaiThongBao);
            console.log("Tiêu đề:", notification.tieuDe);
            console.log("Nội dung:", notification.noiDung);
            addNotificationToList(notification);
            updateBadgeCount();
            showNotificationToast(notification);
        });

        // Debug: Log tất cả events
        connection.onreconnecting(function() {
            console.log("🔄 SignalR đang kết nối lại...");
        });

        connection.onreconnected(function() {
            console.log("✅ SignalR đã kết nối lại");
            loadNotifications();
        });

        connection.onclose(function(error) {
            console.error("❌ SignalR đã đóng kết nối:", error);
        });

        // Kết nối
        connection.start()
            .then(function () {
                console.log("✅ SignalR connected successfully");
                console.log("Connection ID:", connection.connectionId);
                console.log("Is Admin:", isAdmin);
                console.log("User ID:", userId);
                loadNotifications();
            })
            .catch(function (err) {
                console.error("❌ SignalR connection error:", err);
                console.error("Error details:", err.message);
                // Thử kết nối lại sau 3 giây
                setTimeout(function() {
                    console.log("Đang thử kết nối lại SignalR...");
                    initSignalR();
                }, 3000);
            });

        // Xử lý reconnect
        connection.onreconnecting(function () {
            console.log("SignalR reconnecting...");
        });

        connection.onreconnected(function () {
            console.log("SignalR reconnected");
            loadNotifications();
        });
    }

    // Load danh sách thông báo
    function loadNotifications() {
        // Admin không cần truyền userId (sẽ lấy thông báo với NguoiNhanId = null)
        const url = isAdmin 
            ? '/api/notification?skip=0&take=20'
            : `/api/notification?userId=${userId}&skip=0&take=20`;

        console.log("🔔 Đang tải thông báo từ:", url);
        console.log("Is Admin:", isAdmin, "User ID:", userId);

        fetch(url, {
            headers: {
                'Accept': 'application/json'
            }
        })
        .then(response => {
            console.log("Response status:", response.status);
            return response.json();
        })
        .then(data => {
            console.log("📬 Dữ liệu thông báo nhận được:", data);
            if (data.success) {
                console.log("✅ Số lượng thông báo:", data.data?.length || 0);
                console.log("✅ Số chưa đọc:", data.soLuongChuaDoc || 0);
                updateNotificationList(data.data || []);
                updateBadgeCount(data.soLuongChuaDoc || 0);
            } else {
                console.error("❌ Lỗi khi tải thông báo:", data.message);
            }
        })
        .catch(err => {
            console.error("❌ Error loading notifications:", err);
        });
    }

    // Cập nhật danh sách thông báo
    function updateNotificationList(notifications) {
        // Tìm container phù hợp (admin hoặc customer)
        const container = document.getElementById('notificationItems') || document.getElementById('notificationItemsCustomer');
        const empty = document.getElementById('notificationEmpty') || document.getElementById('notificationEmptyCustomer');
        
        if (!container) return;

        container.innerHTML = '';

        if (notifications.length === 0) {
            if (empty) empty.style.display = 'block';
            return;
        }

        if (empty) empty.style.display = 'none';

        notifications.forEach(notif => {
            const item = createNotificationItem(notif);
            container.appendChild(item);
        });
    }

    // Tạo item thông báo
    function createNotificationItem(notif) {
        const li = document.createElement('li');
        li.className = `dropdown-item-text px-3 py-2 ${!notif.daDoc ? 'bg-light' : ''}`;
        li.style.cursor = 'pointer';
        li.onclick = () => markAsRead(notif.id);

        const icon = getNotificationIcon(notif.loaiThongBao);
        const time = formatTime(notif.thoiGianTao);

        let imageHtml = '';
        if (notif.hinhAnh) {
            imageHtml = `<div class="mt-2"><img src="${escapeHtml(notif.hinhAnh)}" alt="Hình ảnh" class="img-thumbnail" style="max-width: 200px; max-height: 200px; cursor: pointer;" onclick="window.open('${escapeHtml(notif.hinhAnh)}', '_blank')"></div>`;
        }

        li.innerHTML = `
            <div class="d-flex align-items-start">
                <div class="me-2">${icon}</div>
                <div class="flex-grow-1">
                    <div class="fw-bold">${escapeHtml(notif.tieuDe)}</div>
                    <div class="small text-muted">${escapeHtml(notif.noiDung)}</div>
                    ${imageHtml}
                    <div class="small text-muted">${time}</div>
                </div>
                ${!notif.daDoc ? '<span class="badge bg-primary rounded-pill">Mới</span>' : ''}
            </div>
        `;

        return li;
    }

    // Lấy icon theo loại thông báo
    function getNotificationIcon(loai) {
        const icons = {
            'ADMIN_BOOKING_NEW': '🕒',
            'ADMIN_BOOKING_CANCEL': '❌',
            'ADMIN_BOOKING_UPDATE': '✏️',
            'ADMIN_ORDER_NEW': '🛒',
            'PET_CHECKIN': '✅',
            'PET_IN_SERVICE': '✂️',
            'PET_DONE': '🎉',
            'PET_HOTEL_DAILY': '🏨',
            'PET_ISSUE': '⚠️',
            'PET_HEALTH_UPDATE': '💊'
        };
        return icons[loai] || '📢';
    }

    // Format thời gian
    function formatTime(dateString) {
        const date = new Date(dateString);
        const now = new Date();
        const diff = now - date;
        const minutes = Math.floor(diff / 60000);
        const hours = Math.floor(minutes / 60);
        const days = Math.floor(hours / 24);

        if (minutes < 1) return 'Vừa xong';
        if (minutes < 60) return `${minutes} phút trước`;
        if (hours < 24) return `${hours} giờ trước`;
        if (days < 7) return `${days} ngày trước`;
        return date.toLocaleDateString('vi-VN');
    }

    // Thêm thông báo mới vào danh sách
    function addNotificationToList(notification) {
        const container = document.getElementById('notificationItems') || document.getElementById('notificationItemsCustomer');
        const empty = document.getElementById('notificationEmpty') || document.getElementById('notificationEmptyCustomer');
        
        if (!container) return;

        if (empty) empty.style.display = 'none';

        const item = createNotificationItem(notification);
        container.insertBefore(item, container.firstChild);
    }

    // Cập nhật badge số lượng
    function updateBadgeCount(count) {
        if (count === undefined) {
            // Lấy từ API
            const url = isAdmin 
                ? '/api/notification/unread-count'
                : `/api/notification/unread-count?userId=${userId}`;

            fetch(url, {
                headers: {
                    'Accept': 'application/json'
                }
            })
            .then(response => response.json())
            .then(data => {
                if (data.success) {
                    setBadgeCount(data.count || 0);
                }
            })
            .catch(err => console.error("Error getting unread count:", err));
        } else {
            setBadgeCount(count);
        }
    }

    function setBadgeCount(count) {
        // Tìm badge phù hợp (admin hoặc customer)
        const badge = document.getElementById('notificationBadge') || document.getElementById('notificationBadgeCustomer');
        if (badge) {
            if (count > 0) {
                badge.textContent = count > 99 ? '99+' : count;
                badge.style.display = 'block';
            } else {
                badge.style.display = 'none';
            }
        }
    }

    // Đánh dấu đã đọc
    function markAsRead(notificationId) {
        fetch('/api/notification/read', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Accept': 'application/json'
            },
            body: JSON.stringify({ id: notificationId })
        })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                loadNotifications(); // Reload để cập nhật UI
            }
        })
        .catch(err => console.error("Error marking as read:", err));
    }

    // Đánh dấu tất cả đã đọc
    document.addEventListener('DOMContentLoaded', function() {
        // Hỗ trợ cả admin và customer
        const markAllReadBtn = document.getElementById('markAllReadBtn') || document.getElementById('markAllReadBtnCustomer');
        if (markAllReadBtn) {
            markAllReadBtn.addEventListener('click', function(e) {
                e.preventDefault();
                const url = isAdmin 
                    ? '/api/notification/read-all'
                    : `/api/notification/read-all?userId=${userId}`;

                fetch(url, {
                    method: 'PUT',
                    headers: {
                        'Accept': 'application/json'
                    }
                })
                .then(response => response.json())
                .then(data => {
                    if (data.success) {
                        loadNotifications();
                    }
                })
                .catch(err => console.error("Error marking all as read:", err));
            });
        }
    });

    // Hiển thị toast notification
    function showNotificationToast(notification) {
        // Tạo toast element nếu chưa có
        let toastContainer = document.getElementById('toastContainer');
        if (!toastContainer) {
            toastContainer = document.createElement('div');
            toastContainer.id = 'toastContainer';
            toastContainer.className = 'toast-container position-fixed top-0 end-0 p-3';
            toastContainer.style.zIndex = '9999';
            document.body.appendChild(toastContainer);
        }

        const toastId = 'toast-' + Date.now();
        const icon = getNotificationIcon(notification.loaiThongBao);
        
        const imageHtml = notification.hinhAnh 
            ? `<div class="mt-2"><img src="${escapeHtml(notification.hinhAnh)}" alt="Hình ảnh" class="img-thumbnail" style="max-width: 200px; max-height: 200px;"></div>` 
            : '';

        const toastHtml = `
            <div id="${toastId}" class="toast" role="alert" aria-live="assertive" aria-atomic="true" data-bs-delay="5000">
                <div class="toast-header">
                    <strong class="me-auto">${icon} ${escapeHtml(notification.tieuDe)}</strong>
                    <button type="button" class="btn-close" data-bs-dismiss="toast" aria-label="Close"></button>
                </div>
                <div class="toast-body">
                    ${escapeHtml(notification.noiDung)}
                    ${imageHtml}
                </div>
            </div>
        `;

        toastContainer.insertAdjacentHTML('beforeend', toastHtml);
        const toastElement = document.getElementById(toastId);
        const toast = new bootstrap.Toast(toastElement);
        toast.show();

        // Xóa element sau khi ẩn
        toastElement.addEventListener('hidden.bs.toast', function() {
            toastElement.remove();
        });
    }

    // Escape HTML
    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    // Khởi tạo khi DOM ready
    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', initSignalR);
    } else {
        initSignalR();
    }
})();

