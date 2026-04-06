const express = require("express");
const router = express.Router();
const bookingController = require("../controllers/booking.controller");
const { authenticateToken, authorizeRole } = require("../utils/authHandler");

// Lấy tất cả lịch grooming (admin/staff xem tất cả)
router.get("/", bookingController.getAllBookings);

// Tạo lịch grooming mới (user đã đăng nhập)
router.post("/", authenticateToken, bookingController.createBooking);

// Lấy lịch grooming theo ID
router.get("/:id", bookingController.getBookingById);

// Cập nhật lịch grooming (staff/admin)
router.put("/:id", authenticateToken, authorizeRole("admin", "staff"), bookingController.updateBooking);

// Xóa lịch grooming (admin)
router.delete("/:id", authenticateToken, authorizeRole("admin"), bookingController.deleteBooking);

module.exports = router;
