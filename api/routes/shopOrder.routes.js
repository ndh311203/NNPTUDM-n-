const express = require("express");
const router = express.Router();
const orderController = require("../controllers/order.controller");
const { authenticateToken, authorizeRole } = require("../utils/authHandler");

router.get("/", authenticateToken, authorizeRole("admin", "staff"), orderController.getAllOrders);
router.post("/", authenticateToken, orderController.createOrder);
router.get("/:id", authenticateToken, orderController.getOrderById);
router.put("/:id", authenticateToken, authorizeRole("admin", "staff"), orderController.updateOrder);
router.delete("/:id", authenticateToken, authorizeRole("admin"), orderController.deleteOrder);

module.exports = router;
