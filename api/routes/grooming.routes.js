const express = require("express");
const router = express.Router();
const bookingController = require("../controllers/booking.controller");
const { authenticateToken, authorizeRole } = require("../utils/authHandler");

router.get("/", bookingController.getAllBookings);
router.post("/", authenticateToken, bookingController.createBooking);
router.get("/:id", bookingController.getBookingById);
router.put("/:id", authenticateToken, authorizeRole("admin", "staff"), bookingController.updateBooking);
router.delete("/:id", authenticateToken, authorizeRole("admin"), bookingController.deleteBooking);

module.exports = router;
