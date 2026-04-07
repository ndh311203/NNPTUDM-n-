const express = require("express");
const router = express.Router();
const serviceController = require("../controllers/service.controller");
const { authenticateToken, authorizeRole } = require("../utils/authHandler");

router.get("/", serviceController.getAllServices);
router.get("/:id", serviceController.getServiceById);
router.post("/", authenticateToken, authorizeRole("admin", "staff"), serviceController.createService);
router.put("/:id", authenticateToken, authorizeRole("admin", "staff"), serviceController.updateService);
router.delete("/:id", authenticateToken, authorizeRole("admin"), serviceController.deleteService);

module.exports = router;
