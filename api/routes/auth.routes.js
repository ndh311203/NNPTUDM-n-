const express = require("express");
const router = express.Router();
const { register, login, refreshToken, logout, getCurrentUser } = require("../controllers/auth.controller");
const { authenticateToken } = require("../utils/authHandler");

// Đăng ký tài khoản mới
router.post("/register", register);

// Đăng nhập
router.post("/login", login);

// Làm mới access token
router.post("/refresh", refreshToken);

// Đăng xuất
router.post("/logout", logout);

// Lấy thông tin user hiện tại (cần token)
router.get("/me", authenticateToken, getCurrentUser);

module.exports = router;
