const express = require("express");
const router = express.Router();
const accountController = require("../controllers/account.controller");
const { authenticateToken, authorizeRole } = require("../utils/authHandler");

// Chỉ admin mới quản lý tài khoản
router.get("/", authenticateToken, authorizeRole("admin"), accountController.getAllAccounts);
router.post("/", authenticateToken, authorizeRole("admin"), accountController.createAccount);
router.get("/:id", authenticateToken, authorizeRole("admin"), accountController.getAccountById);
router.put("/:id", authenticateToken, authorizeRole("admin"), accountController.updateAccount);
router.delete("/:id", authenticateToken, authorizeRole("admin"), accountController.deleteAccount);

module.exports = router;
