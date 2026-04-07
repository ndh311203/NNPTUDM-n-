const express = require("express");
const router = express.Router();
const chatController = require("../controllers/chat.controller");

router.get("/messages", chatController.getMessages);
router.post("/messages", chatController.sendMessage);
router.get("/conversations", chatController.getConversations);
router.put("/messages/:id", chatController.updateMessage);
router.delete("/messages/:id", chatController.deleteMessage);

module.exports = router;
