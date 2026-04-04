const TaiKhoan = require('../schemas/TaiKhoan');
const KhachHang = require('../schemas/KhachHang');
const { hashPassword, comparePassword, createTokenPair, generateAccessToken } = require('../utils/authHandler');
const { validateEmail, validatePasswordSimple } = require('../utils/validator');

/**
 * Authentication Controller
 * Handles user registration, login, token refresh, and OAuth integration
 */

/**
 * Register new user
 * POST /api/auth/register
 */
const register = async (req, res) => {
  try {
    const { email, password, hoTen, soDienThoai } = req.body;

    // Validate inputs
    if (!email || !password || !hoTen || !soDienThoai) {
      return res.status(400).json({
        success: false,
        message: 'Missing required fields: email, password, hoTen, soDienThoai'
      });
    }

    // Check if user already exists
    const existingTaiKhoan = await TaiKhoan.findOne({ email });
    if (existingTaiKhoan) {
      return res.status(409).json({
        success: false,
        message: 'Email already registered'
      });
    }

    // Check if phone already registered
    const existingKhachHang = await KhachHang.findOne({ soDienThoai });
    if (existingKhachHang) {
      return res.status(409).json({
        success: false,
        message: 'Phone number already registered'
      });
    }

    // Hash password
    const hashedPassword = await hashPassword(password);

    // Create TaiKhoan (Account)
    const newTaiKhoan = await TaiKhoan.create({
      email,
      matKhau: hashedPassword,
      hoTen,
      vaiTro: 'user',
      trangThai: true
    });

    // Create KhachHang (Customer Profile)
    const newKhachHang = await KhachHang.create({
      hoTen,
      soDienThoai,
      email,
      taiKhoanId: newTaiKhoan._id,
      trangThai: true
    });

    // Link KhachHang to TaiKhoan
    // (Optional: you may want to add a reference in TaiKhoan if needed)

    // Generate tokens
    const tokens = createTokenPair({
      userId: newTaiKhoan._id,
      email: newTaiKhoan.email,
      role: newTaiKhoan.vaiTro,
      khachHangId: newKhachHang._id
    });

    res.status(201).json({
      success: true,
      message: 'Registration successful',
      data: {
        taiKhoan: newTaiKhoan.toJSON(),
        khachHang: newKhachHang,
        tokens
      }
    });
  } catch (error) {
    console.error('Registration error:', error);
    res.status(500).json({
      success: false,
      message: 'Registration failed',
      error: error.message
    });
  }
};

/**
 * Login user
 * POST /api/auth/login
 */
const login = async (req, res) => {
  try {
    const { email, password } = req.body;

    // Validate inputs
    if (!email || !password) {
      return res.status(400).json({
        success: false,
        message: 'Email and password are required'
      });
    }

    // Find user by email
    const taiKhoan = await TaiKhoan.findOne({ email }).select('+matKhau');
    if (!taiKhoan) {
      return res.status(401).json({
        success: false,
        message: 'Invalid email or password'
      });
    }

    // Check if account is active
    if (!taiKhoan.trangThai) {
      return res.status(403).json({
        success: false,
        message: 'Account is locked'
      });
    }

    // Compare password
    const isPasswordValid = await comparePassword(password, taiKhoan.matKhau);
    if (!isPasswordValid) {
      return res.status(401).json({
        success: false,
        message: 'Invalid email or password'
      });
    }

    // Get KhachHang info
    const khachHang = await KhachHang.findOne({ taiKhoanId: taiKhoan._id });

    // Update last login
    taiKhoan.lastLogin = new Date();
    await taiKhoan.save();

    // Generate tokens
    const tokens = createTokenPair({
      userId: taiKhoan._id,
      email: taiKhoan.email,
      role: taiKhoan.vaiTro,
      khachHangId: khachHang?._id
    });

    res.status(200).json({
      success: true,
      message: 'Login successful',
      data: {
        taiKhoan: taiKhoan.toJSON(),
        khachHang,
        tokens
      }
    });
  } catch (error) {
    console.error('Login error:', error);
    res.status(500).json({
      success: false,
      message: 'Login failed',
      error: error.message
    });
  }
};

/**
 * Refresh access token
 * POST /api/auth/refresh
 */
const refreshToken = async (req, res) => {
  try {
    const { refreshToken } = req.body;

    if (!refreshToken) {
      return res.status(400).json({
        success: false,
        message: 'Refresh token is required'
      });
    }

    // Verify refresh token and get user data
    const { verifyRefreshToken } = require('../utils/authHandler');
    const decoded = verifyRefreshToken(refreshToken);

    // Find user
    const taiKhoan = await TaiKhoan.findById(decoded.userId);
    if (!taiKhoan || !taiKhoan.trangThai) {
      return res.status(401).json({
        success: false,
        message: 'Invalid or expired refresh token'
      });
    }

    // Generate new tokens
    const tokens = createTokenPair({
      userId: taiKhoan._id,
      email: taiKhoan.email,
      role: taiKhoan.vaiTro,
      khachHangId: decoded.khachHangId
    });

    res.status(200).json({
      success: true,
      message: 'Token refreshed successfully',
      data: { tokens }
    });
  } catch (error) {
    console.error('Refresh token error:', error);
    res.status(401).json({
      success: false,
      message: 'Token refresh failed',
      error: error.message
    });
  }
};

/**
 * Logout user
 * POST /api/auth/logout
 */
const logout = async (req, res) => {
  try {
    // JWT is stateless, logout is handled client-side by removing token
    // Server can maintain a blacklist if needed
    res.status(200).json({
      success: true,
      message: 'Logout successful'
    });
  } catch (error) {
    res.status(500).json({
      success: false,
      message: 'Logout failed',
      error: error.message
    });
  }
};

/**
 * Get current user profile
 * GET /api/auth/me
 * Requires authentication
 */
const getCurrentUser = async (req, res) => {
  try {
    const taiKhoan = await TaiKhoan.findById(req.user.userId);
    const khachHang = await KhachHang.findOne({ taiKhoanId: req.user.userId });

    if (!taiKhoan) {
      return res.status(404).json({
        success: false,
        message: 'User not found'
      });
    }

    res.status(200).json({
      success: true,
      data: {
        taiKhoan: taiKhoan.toJSON(),
        khachHang
      }
    });
  } catch (error) {
    console.error('Get current user error:', error);
    res.status(500).json({
      success: false,
      message: 'Failed to fetch user profile',
      error: error.message
    });
  }
};

module.exports = {
  register,
  login,
  refreshToken,
  logout,
  getCurrentUser
};
