const express = require("express");
const router = express.Router();
const hotelController = require("../controllers/hotel.controller");
const { authenticateToken, authorizeRole } = require("../utils/authHandler");

// Lấy danh sách phòng (tất cả, filter theo trang thái: ?trangThai=TRONG)
router.get("/rooms", hotelController.getAllRooms);

// Tạo phòng mới (chỉ admin/staff)
router.post("/rooms", authenticateToken, authorizeRole("admin", "staff"), hotelController.createRoom);

// Lấy phòng theo ID
router.get("/rooms/:id", hotelController.getRoomById);

// Cập nhật phòng (chỉ admin/staff)
router.put("/rooms/:id", authenticateToken, authorizeRole("admin", "staff"), hotelController.updateRoom);

// Xóa phòng (chỉ admin)
router.delete("/rooms/:id", authenticateToken, authorizeRole("admin"), hotelController.deleteRoom);

module.exports = router;
