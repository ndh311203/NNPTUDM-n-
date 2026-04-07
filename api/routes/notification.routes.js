const express = require("express");
const router = express.Router();
const notiController = require("../controllers/notification.controller");

router.get("/", notiController.getNotifications);
router.post("/", notiController.createNotification);
router.put("/:id/read", notiController.markAsRead);
router.delete("/:id", notiController.deleteNotification);

module.exports = router;
