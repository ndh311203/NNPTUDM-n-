const express = require("express");
const router = express.Router();
const hotelController = require("../controllers/hotel.controller");
const { authenticateToken, authorizeRole } = require("../utils/authHandler");

router.get("/rooms", hotelController.getAllRooms);
router.post("/rooms", authenticateToken, authorizeRole("admin", "staff"), hotelController.createRoom);
router.get("/rooms/:id", hotelController.getRoomById);
router.put("/rooms/:id", authenticateToken, authorizeRole("admin", "staff"), hotelController.updateRoom);
router.delete("/rooms/:id", authenticateToken, authorizeRole("admin"), hotelController.deleteRoom);

module.exports = router;
