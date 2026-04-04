/**
 * Cart Controller
 * Represents a session-based or generic Cart logic.
 * Note: Cart data is usually saved in sessions or frontend LocalStorage.
 * Here we provide standard standard CRUD APIs for server-side carts.
 */

// Giả lập database Giỏ hàng trong Memory để đáp ứng CRUD (Nên chuyển sang MongoDB Schema `Cart` sau)
let carts = {};

exports.getCartByUserId = (req, res) => {
    try {
        const userId = req.params.userId || req.user?.id || 'guest';
        const userCart = carts[userId] || { userId, items: [], total: 0 };
        res.status(200).json({ success: true, data: userCart });
    } catch (error) {
        res.status(500).json({ success: false, message: error.message });
    }
};

exports.addToCart = (req, res) => {
    try {
        const userId = req.body.userId || 'guest';
        const { sanPhamId, soLuong, donGia } = req.body;
        
        if (!carts[userId]) {
            carts[userId] = { userId, items: [], total: 0 };
        }
        
        const cart = carts[userId];
        const existingItem = cart.items.find(item => item.sanPhamId === sanPhamId);
        
        if (existingItem) {
            existingItem.soLuong += (soLuong || 1);
        } else {
            cart.items.push({ sanPhamId, soLuong: soLuong || 1, donGia });
        }
        
        cart.total = cart.items.reduce((sum, item) => sum + (item.soLuong * item.donGia), 0);
        res.status(200).json({ success: true, message: 'Thêm vào giỏ hàng thành công', data: cart });
    } catch (error) {
        res.status(500).json({ success: false, message: error.message });
    }
};

exports.removeFromCart = (req, res) => {
    try {
        const userId = req.params.userId || 'guest';
        const sanPhamId = req.params.productId;
        
        if (carts[userId]) {
            const cart = carts[userId];
            cart.items = cart.items.filter(item => item.sanPhamId !== sanPhamId);
            cart.total = cart.items.reduce((sum, item) => sum + (item.soLuong * item.donGia), 0);
        }
        
        res.status(200).json({ success: true, message: 'Đã xóa sản phẩm khỏi giỏ', data: carts[userId] });
    } catch (error) {
        res.status(500).json({ success: false, message: error.message });
    }
};

exports.clearCart = (req, res) => {
    try {
        const userId = req.params.userId || 'guest';
        carts[userId] = { userId, items: [], total: 0 };
        res.status(200).json({ success: true, message: 'Đã làm sạch giỏ hàng' });
    } catch (error) {
        res.status(500).json({ success: false, message: error.message });
    }
};
