const express = require("express");
const router = express.Router();
const { register, login, refreshToken, logout, getCurrentUser } = require("../controllers/auth.controller");
const { authenticateToken } = require("../utils/authHandler");

router.post("/register", register);
router.post("/login", login);
router.post("/refresh", refreshToken);
router.post("/logout", logout);
router.get("/me", authenticateToken, getCurrentUser);

module.exports = router;
