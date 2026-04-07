const express = require("express");
const router = express.Router();
const groomingroomController = require("../controllers/groomingroom.controller");

router.get("/", groomingroomController.getAllRooms);
router.get("/:id", groomingroomController.getRoomById);
router.post("/", groomingroomController.createRoom);
router.put("/:id", groomingroomController.updateRoom);
router.delete("/:id", groomingroomController.deleteRoom);
router.patch("/:id/status", groomingroomController.toggleRoomStatus);

module.exports = router;
