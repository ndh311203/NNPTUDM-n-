const express = require("express");
const router = express.Router();
const orderController = require("../controllers/order.controller");
const { authenticateToken, authorizeRole } = require("../utils/authHandler");

// Xem tất cả đơn hàng (admin)
router.get("/", authenticateToken, authorizeRole("admin", "staff"), orderController.getAllOrders);

// Tạo đơn hàng (user đã đăng nhập - có Transaction bên trong)
router.post("/", authenticateToken, orderController.createOrder);

// Xem đơn theo ID
router.get("/:id", authenticateToken, orderController.getOrderById);

// Cập nhật đơn hàng (admin/staff)
router.put("/:id", authenticateToken, authorizeRole("admin", "staff"), orderController.updateOrder);

// Xóa đơn hàng + hoàn kho (Transaction - chỉ admin)
router.delete("/:id", authenticateToken, authorizeRole("admin"), orderController.deleteOrder);

module.exports = router;
