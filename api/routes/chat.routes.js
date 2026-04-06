const express = require("express");
const router = express.Router();
const chatController = require("../controllers/chat.controller");

// Lấy lịch sử tin nhắn (có thể lọc ?room=general)
router.get("/messages", chatController.getMessages);

// Gửi tin nhắn mới (phát Socket.IO realtime)
router.post("/messages", chatController.sendMessage);

// Lấy danh sách các phòng chat
router.get("/conversations", chatController.getConversations);

// Cập nhật tin nhắn
router.put("/messages/:id", chatController.updateMessage);

// Xóa tin nhắn
router.delete("/messages/:id", chatController.deleteMessage);

module.exports = router;
