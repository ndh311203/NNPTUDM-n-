// =============================================
// SPATC PET SHOP - API WRAPPER
// Gọi tất cả API Backend qua file này
// =============================================

const API_BASE = 'http://localhost:3000/api';
const API_UPLOAD = 'http://localhost:3000';

// ============ HELPERS ============

// Lấy token từ localStorage
function getToken() {
  return localStorage.getItem('accessToken');
}

// Lưu thông tin user vào localStorage
function saveAuth(data) {
  localStorage.setItem('accessToken', data.tokens.accessToken);
  localStorage.setItem('refreshToken', data.tokens.refreshToken);
  localStorage.setItem('user', JSON.stringify(data.taiKhoan));
  if (data.khachHang) {
    localStorage.setItem('khachHang', JSON.stringify(data.khachHang));
  }
}

// Lấy thông tin user hiện tại
function getCurrentUser() {
  try {
    return JSON.parse(localStorage.getItem('user'));
  } catch { return null; }
}
function getKhachHang() {
  try {
    return JSON.parse(localStorage.getItem('khachHang'));
  } catch { return null; }
}

// Xóa session
function clearAuth() {
  localStorage.removeItem('accessToken');
  localStorage.removeItem('refreshToken');
  localStorage.removeItem('user');
  localStorage.removeItem('khachHang');
}

// Kiểm tra đã đăng nhập chưa
function isLoggedIn() {
  return !!getToken();
}

// Kiểm tra role admin
function isAdmin() {
  const user = getCurrentUser();
  return user && (user.vaiTro === 'admin' || user.vaiTro === 'staff');
}

// Hàm gọi API chính
async function apiCall(endpoint, options = {}) {
  const token = getToken();
  const headers = {
    'Content-Type': 'application/json',
    ...options.headers,
  };
  if (token) headers['Authorization'] = `Bearer ${token}`;

  // Nếu có file upload, không set Content-Type để browser tự set boundary
  if (options.body instanceof FormData) {
    delete headers['Content-Type'];
  }

  try {
    const res = await fetch(`${API_BASE}${endpoint}`, {
      ...options,
      headers,
    });
    const data = await res.json();
    if (!res.ok) {
      throw new Error(data.message || `HTTP ${res.status}`);
    }
    return data;
  } catch (err) {
    console.error(`API Error [${endpoint}]:`, err);
    throw err;
  }
}

// ============ AUTH APIs ============
const Auth = {
  register: (body) => apiCall('/auth/register', { method: 'POST', body: JSON.stringify(body) }),
  login: async (body) => {
    const res = await apiCall('/auth/login', { method: 'POST', body: JSON.stringify(body) });
    if (res.success && res.data) saveAuth(res.data);
    return res;
  },
  logout: async () => {
    await apiCall('/auth/logout', { method: 'POST' }).catch(() => {});
    clearAuth();
  },
  me: () => apiCall('/auth/me'),
  refresh: (refreshToken) => apiCall('/auth/refresh', { method: 'POST', body: JSON.stringify({ refreshToken }) }),
};

// ============ PRODUCTS APIs ============
const Products = {
  getAll: () => apiCall('/products'),
  getById: (id) => apiCall(`/products/${id}`),
  create: (body) => apiCall('/products', { method: 'POST', body: JSON.stringify(body) }),
  update: (id, body) => apiCall(`/products/${id}`, { method: 'PUT', body: JSON.stringify(body) }),
  delete: (id) => apiCall(`/products/${id}`, { method: 'DELETE' }),
};

// ============ SERVICES APIs ============
const Services = {
  getAll: () => apiCall('/services'),
  getById: (id) => apiCall(`/services/${id}`),
  create: (body) => apiCall('/services', { method: 'POST', body: JSON.stringify(body) }),
  update: (id, body) => apiCall(`/services/${id}`, { method: 'PUT', body: JSON.stringify(body) }),
  delete: (id) => apiCall(`/services/${id}`, { method: 'DELETE' }),
};

// ============ BOOKINGS APIs ============
const Bookings = {
  getAll: () => apiCall('/bookings'),
  getById: (id) => apiCall(`/bookings/${id}`),
  create: (body) => apiCall('/bookings', { method: 'POST', body: JSON.stringify(body) }),
  update: (id, body) => apiCall(`/bookings/${id}`, { method: 'PUT', body: JSON.stringify(body) }),
  delete: (id) => apiCall(`/bookings/${id}`, { method: 'DELETE' }),
};

// ============ CART APIs ============
const Cart = {
  get: (userId) => apiCall(`/cart/${userId || 'guest'}`),
  add: (body) => apiCall('/cart/add', { method: 'POST', body: JSON.stringify(body) }),
  remove: (productId, userId) => apiCall(`/cart/${productId}/${userId || 'guest'}`, { method: 'DELETE' }),
  clear: (userId) => apiCall(`/cart/clear/${userId || 'guest'}`, { method: 'DELETE' }),
};

// ============ ORDERS APIs ============
const Orders = {
  getAll: () => apiCall('/shop-orders'),
  getById: (id) => apiCall(`/shop-orders/${id}`),
  create: (body) => apiCall('/shop-orders', { method: 'POST', body: JSON.stringify(body) }),
  update: (id, body) => apiCall(`/shop-orders/${id}`, { method: 'PUT', body: JSON.stringify(body) }),
  delete: (id) => apiCall(`/shop-orders/${id}`, { method: 'DELETE' }),
};

// ============ VOUCHERS APIs ============
const Vouchers = {
  getAll: () => apiCall('/vouchers'),
  getById: (id) => apiCall(`/vouchers/${id}`),
  create: (body) => apiCall('/vouchers', { method: 'POST', body: JSON.stringify(body) }),
  update: (id, body) => apiCall(`/vouchers/${id}`, { method: 'PUT', body: JSON.stringify(body) }),
  delete: (id) => apiCall(`/vouchers/${id}`, { method: 'DELETE' }),
  validate: (body) => apiCall('/vouchers/validate', { method: 'POST', body: JSON.stringify(body) }),
};

// ============ ACCOUNTS APIs ============
const Accounts = {
  getAll: () => apiCall('/accounts'),
  getById: (id) => apiCall(`/accounts/${id}`),
  update: (id, body) => apiCall(`/accounts/${id}`, { method: 'PUT', body: JSON.stringify(body) }),
  delete: (id) => apiCall(`/accounts/${id}`, { method: 'DELETE' }),
};

// ============ NOTIFICATIONS APIs ============
const Notifications = {
  getAll: (userId) => apiCall(`/notifications${userId ? '?userId=' + userId : ''}`),
  create: (body) => apiCall('/notifications', { method: 'POST', body: JSON.stringify(body) }),
  markRead: (id) => apiCall(`/notifications/${id}/read`, { method: 'PUT' }),
  delete: (id) => apiCall(`/notifications/${id}`, { method: 'DELETE' }),
};

// ============ CHAT APIs ============
const ChatAPI = {
  getMessages: (room) => apiCall(`/chat/messages${room ? '?room=' + room : ''}`),
  sendMessage: (body) => apiCall('/chat/messages', { method: 'POST', body: JSON.stringify(body) }),
  getConversations: () => apiCall('/chat/conversations'),
  deleteMessage: (id) => apiCall(`/chat/messages/${id}`, { method: 'DELETE' }),
};

// ============ UPLOAD APIs ============
const Upload = {
  single: async (file) => {
    const formData = new FormData();
    formData.append('file', file);
    const token = getToken();
    const headers = {};
    if (token) headers['Authorization'] = `Bearer ${token}`;
    const res = await fetch(`${API_BASE}/upload/single`, {
      method: 'POST',
      headers,
      body: formData,
    });
    const data = await res.json();
    if (!res.ok) throw new Error(data.message || 'Upload thất bại');
    return data;
  },
  multiple: async (files) => {
    const formData = new FormData();
    for (const file of files) formData.append('files', file);
    const token = getToken();
    const headers = {};
    if (token) headers['Authorization'] = `Bearer ${token}`;
    const res = await fetch(`${API_BASE}/upload/multiple`, {
      method: 'POST',
      headers,
      body: formData,
    });
    const data = await res.json();
    if (!res.ok) throw new Error(data.message || 'Upload thất bại');
    return data;
  },
};

// ============ TOAST NOTIFICATION ============
function showToast(title, msg, type = 'success') {
  const icons = { success: '✅', error: '❌', warning: '⚠️', info: 'ℹ️' };
  let area = document.getElementById('toast-area');
  if (!area) {
    area = document.createElement('div');
    area.id = 'toast-area';
    area.className = 'toast-area';
    document.body.appendChild(area);
  }
  const toast = document.createElement('div');
  toast.className = `toast toast-${type}`;
  toast.innerHTML = `
    <span class="toast-icon">${icons[type] || '💬'}</span>
    <div class="toast-content">
      <div class="toast-title">${title}</div>
      ${msg ? `<div class="toast-msg">${msg}</div>` : ''}
    </div>
    <button class="toast-close" onclick="this.parentElement.remove()">✕</button>
  `;
  area.appendChild(toast);
  setTimeout(() => toast.remove(), 4000);
}

// ============ FORMAT HELPERS ============
function formatPrice(n) {
  if (!n && n !== 0) return '—';
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(n);
}
function formatDate(d) {
  if (!d) return '—';
  return new Date(d).toLocaleDateString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric' });
}
function formatDateTime(d) {
  if (!d) return '—';
  return new Date(d).toLocaleString('vi-VN');
}
function getProductEmoji(cat) {
  const map = { THUC_AN: '🍖', DO_CHOI: '🎾', THUOC: '💊', PHU_KIEN: '🎀', KHAC: '📦' };
  return map[cat] || '📦';
}
function getStatusBadge(status) {
  const map = {
    CHO_XAC_NHAN: ['warning', 'Chờ xác nhận'],
    DA_XAC_NHAN: ['info', 'Đã xác nhận'],
    HOAN_THANH: ['success', 'Hoàn thành'],
    DA_HUY: ['danger', 'Đã hủy'],
    DANG_XU_LY: ['info', 'Đang xử lý'],
    DA_GIAO: ['success', 'Đã giao'],
    CHUA_THANH_TOAN: ['warning', 'Chưa TT'],
    DA_THANH_TOAN: ['success', 'Đã TT'],
    HOAN_TIEN: ['danger', 'Hoàn tiền'],
  };
  const [type, label] = map[status] || ['info', status];
  return `<span class="badge badge-${type}">${label}</span>`;
}

// ============ LOADING ============
function showLoading() {
  const el = document.getElementById('spinner-overlay');
  if (el) el.classList.add('show');
}
function hideLoading() {
  const el = document.getElementById('spinner-overlay');
  if (el) el.classList.remove('show');
}

// ============ MODAL HELPERS ============
function openModal(id) {
  document.getElementById(id)?.classList.add('open');
}
function closeModal(id) {
  document.getElementById(id)?.classList.remove('open');
}

// ============ NAV ACTIVE STATE ============
function setActiveNav() {
  const path = window.location.pathname.split('/').pop() || 'index.html';
  document.querySelectorAll('.nav-link, .sidebar-link').forEach(el => {
    const href = el.getAttribute('href');
    if (href && href.includes(path)) el.classList.add('active');
  });
}
document.addEventListener('DOMContentLoaded', setActiveNav);

// Expose globals
window.Auth = Auth;
window.Products = Products;
window.Services = Services;
window.Bookings = Bookings;
window.Cart = Cart;
window.Orders = Orders;
window.Vouchers = Vouchers;
window.Accounts = Accounts;
window.Notifications = Notifications;
window.ChatAPI = ChatAPI;
window.Upload = Upload;
window.showToast = showToast;
window.formatPrice = formatPrice;
window.formatDate = formatDate;
window.formatDateTime = formatDateTime;
window.getProductEmoji = getProductEmoji;
window.getStatusBadge = getStatusBadge;
window.showLoading = showLoading;
window.hideLoading = hideLoading;
window.openModal = openModal;
window.closeModal = closeModal;
window.getCurrentUser = getCurrentUser;
window.getKhachHang = getKhachHang;
window.isLoggedIn = isLoggedIn;
window.isAdmin = isAdmin;
window.clearAuth = clearAuth;
window.API_UPLOAD = API_UPLOAD;
