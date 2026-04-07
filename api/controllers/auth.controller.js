const TaiKhoan = require("../models/TaiKhoan");
const KhachHang = require("../models/KhachHang");
const {
  hashPassword,
  comparePassword,
  createTokenPair,
  generateAccessToken,
} = require("../utils/authHandler");
const { validateEmail, validatePasswordSimple } = require("../utils/validator");

const register = async (req, res) => {
  try {
    const { email, password, hoTen, soDienThoai } = req.body;

    if (!email || !password || !hoTen || !soDienThoai) {
      return res.status(400).json({
        success: false,
        message: "Missing required fields: email, password, hoTen, soDienThoai",
      });
    }

    const existingTaiKhoan = await TaiKhoan.findOne({ email });
    if (existingTaiKhoan) {
      return res.status(409).json({
        success: false,
        message: "Email already registered",
      });
    }

    const existingKhachHang = await KhachHang.findOne({ soDienThoai });
    if (existingKhachHang) {
      return res.status(409).json({
        success: false,
        message: "Phone number already registered",
      });
    }

    const hashedPassword = await hashPassword(password);

    const newTaiKhoan = await TaiKhoan.create({
      email,
      matKhau: hashedPassword,
      hoTen,
      vaiTro: "user",
      trangThai: true,
    });

    const newKhachHang = await KhachHang.create({
      hoTen,
      soDienThoai,
      email,
      taiKhoanId: newTaiKhoan._id,
      trangThai: true,
    });

    const tokens = createTokenPair({
      userId: newTaiKhoan._id,
      email: newTaiKhoan.email,
      role: newTaiKhoan.vaiTro,
      khachHangId: newKhachHang._id,
    });

    res.status(201).json({
      success: true,
      message: "Registration successful",
      data: {
        taiKhoan: newTaiKhoan.toJSON(),
        khachHang: newKhachHang,
        tokens,
      },
    });
  } catch (error) {
    console.error("Registration error:", error);
    res.status(500).json({
      success: false,
      message: "Registration failed",
      error: error.message,
    });
  }
};

const login = async (req, res) => {
  try {
    const { email, password } = req.body;

    if (!email || !password) {
      return res.status(400).json({
        success: false,
        message: "Email and password are required",
      });
    }

    const taiKhoan = await TaiKhoan.findOne({ email }).select("+matKhau");
    if (!taiKhoan) {
      return res.status(401).json({
        success: false,
        message: "Invalid email or password",
      });
    }

    if (!taiKhoan.trangThai) {
      return res.status(403).json({
        success: false,
        message: "Account is locked",
      });
    }

    const isPasswordValid = await comparePassword(password, taiKhoan.matKhau);
    if (!isPasswordValid) {
      return res.status(401).json({
        success: false,
        message: "Invalid email or password",
      });
    }

    const khachHang = await KhachHang.findOne({ taiKhoanId: taiKhoan._id });

    taiKhoan.lastLogin = new Date();
    await taiKhoan.save();

    const tokens = createTokenPair({
      userId: taiKhoan._id,
      email: taiKhoan.email,
      role: taiKhoan.vaiTro,
      khachHangId: khachHang?._id,
    });

    res.status(200).json({
      success: true,
      message: "Login successful",
      data: {
        taiKhoan: taiKhoan.toJSON(),
        khachHang,
        tokens,
      },
    });
  } catch (error) {
    console.error("Login error:", error);
    res.status(500).json({
      success: false,
      message: "Login failed",
      error: error.message,
    });
  }
};

const refreshToken = async (req, res) => {
  try {
    const { refreshToken } = req.body;

    if (!refreshToken) {
      return res.status(400).json({
        success: false,
        message: "Refresh token is required",
      });
    }

    const { verifyRefreshToken } = require("../utils/authHandler");
    const decoded = verifyRefreshToken(refreshToken);

    const taiKhoan = await TaiKhoan.findById(decoded.userId);
    if (!taiKhoan || !taiKhoan.trangThai) {
      return res.status(401).json({
        success: false,
        message: "Invalid or expired refresh token",
      });
    }

    const tokens = createTokenPair({
      userId: taiKhoan._id,
      email: taiKhoan.email,
      role: taiKhoan.vaiTro,
      khachHangId: decoded.khachHangId,
    });

    res.status(200).json({
      success: true,
      message: "Token refreshed successfully",
      data: { tokens },
    });
  } catch (error) {
    console.error("Refresh token error:", error);
    res.status(401).json({
      success: false,
      message: "Token refresh failed",
      error: error.message,
    });
  }
};

const logout = async (req, res) => {
  try {
    res.status(200).json({
      success: true,
      message: "Logout successful",
    });
  } catch (error) {
    res.status(500).json({
      success: false,
      message: "Logout failed",
      error: error.message,
    });
  }
};

const getCurrentUser = async (req, res) => {
  try {
    const taiKhoan = await TaiKhoan.findById(req.user.userId);
    const khachHang = await KhachHang.findOne({ taiKhoanId: req.user.userId });

    if (!taiKhoan) {
      return res.status(404).json({
        success: false,
        message: "User not found",
      });
    }

    res.status(200).json({
      success: true,
      data: {
        taiKhoan: taiKhoan.toJSON(),
        khachHang,
      },
    });
  } catch (error) {
    console.error("Get current user error:", error);
    res.status(500).json({
      success: false,
      message: "Failed to fetch user profile",
      error: error.message,
    });
  }
};

module.exports = {
  register,
  login,
  refreshToken,
  logout,
  getCurrentUser,
};
