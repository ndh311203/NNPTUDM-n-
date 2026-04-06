const express = require("express");
const router = express.Router();
const notiController = require("../controllers/notification.controller");

// Lấy danh sách thông báo
router.get("/", notiController.getNotifications);

// Tạo thông báo mới (đẩy Socket.IO realtime)
router.post("/", notiController.createNotification);

// Đánh dấu thông báo đã đọc
router.put("/:id/read", notiController.markAsRead);

// Xóa thông báo
router.delete("/:id", notiController.deleteNotification);

module.exports = router;
