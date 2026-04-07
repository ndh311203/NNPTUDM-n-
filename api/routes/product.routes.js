const express = require("express");
const router = express.Router();
const productController = require("../controllers/product.controller");
const { authenticateToken, authorizeRole } = require("../utils/authHandler");

router.get("/", productController.getAllProducts);
router.get("/:id", productController.getProductById);
router.post("/", authenticateToken, authorizeRole("admin", "staff"), productController.createProduct);
router.put("/:id", authenticateToken, authorizeRole("admin", "staff"), productController.updateProduct);
router.delete("/:id", authenticateToken, authorizeRole("admin"), productController.deleteProduct);

module.exports = router;
