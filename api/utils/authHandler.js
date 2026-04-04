const jwt = require('jsonwebtoken');
const bcrypt = require('bcryptjs');
require('dotenv').config();

/**
 * Token Generation and Verification
 */

/**
 * Generate JWT access token
 * @param {Object} payload - Data to encode in token
 * @param {number} expiresIn - Token expiration time
 * @returns {string} JWT token
 */
const generateAccessToken = (payload) => {
  const expirestIn = process.env.JWT_EXPIRE || '7d';
  
  return jwt.sign(payload, process.env.JWT_SECRET, {
    expiresIn: expirestIn,
    issuer: 'spatc-api',
    subject: payload.userId || payload.id
  });
};

/**
 * Generate JWT refresh token
 * @param {Object} payload - Data to encode in token
 * @returns {string} Refresh token
 */
const generateRefreshToken = (payload) => {
  const expiresIn = process.env.REFRESH_TOKEN_EXPIRE || '30d';
  
  return jwt.sign(payload, process.env.REFRESH_TOKEN_SECRET, {
    expiresIn: expiresIn,
    issuer: 'spatc-api'
  });
};

/**
 * Verify JWT access token
 * @param {string} token - JWT token to verify
 * @returns {Object} Decoded token payload
 * @throws {Error} If token is invalid or expired
 */
const verifyAccessToken = (token) => {
  try {
    return jwt.verify(token, process.env.JWT_SECRET);
  } catch (error) {
    if (error.name === 'TokenExpiredError') {
      throw new Error('Token has expired');
    } else if (error.name === 'JsonWebTokenError') {
      throw new Error('Invalid token');
    }
    throw error;
  }
};

/**
 * Verify JWT refresh token
 * @param {string} token - Refresh token to verify
 * @returns {Object} Decoded token payload
 * @throws {Error} If token is invalid or expired
 */
const verifyRefreshToken = (token) => {
  try {
    return jwt.verify(token, process.env.REFRESH_TOKEN_SECRET);
  } catch (error) {
    if (error.name === 'TokenExpiredError') {
      throw new Error('Refresh token has expired');
    } else if (error.name === 'JsonWebTokenError') {
      throw new Error('Invalid refresh token');
    }
    throw error;
  }
};

/**
 * Hash password using bcrypt
 * @param {string} password - Plain text password
 * @param {number} saltRounds - Number of salt rounds (default: 10)
 * @returns {Promise<string>} Hashed password
 */
const hashPassword = async (password, saltRounds = 10) => {
  try {
    const hashed = await bcrypt.hash(password, saltRounds);
    return hashed;
  } catch (error) {
    throw new Error(`Password hashing failed: ${error.message}`);
  }
};

/**
 * Compare plain password with hashed password
 * @param {string} plainPassword - Plain text password
 * @param {string} hashedPassword - Hashed password to compare
 * @returns {Promise<boolean>} True if passwords match
 */
const comparePassword = async (plainPassword, hashedPassword) => {
  try {
    return await bcrypt.compare(plainPassword, hashedPassword);
  } catch (error) {
    throw new Error(`Password comparison failed: ${error.message}`);
  }
};

/**
 * Extract token from Authorization header
 * @param {string} authHeader - Authorization header value
 * @returns {string|null} Token string or null
 */
const extractTokenFromHeader = (authHeader) => {
  if (!authHeader) return null;
  
  const parts = authHeader.split(' ');
  if (parts.length === 2 && parts[0].toLowerCase() === 'bearer') {
    return parts[1];
  }
  
  return null;
};

/**
 * Create token pair (access + refresh)
 * @param {Object} payload - User data
 * @returns {Object} { accessToken, refreshToken }
 */
const createTokenPair = (payload) => {
  return {
    accessToken: generateAccessToken(payload),
    refreshToken: generateRefreshToken(payload)
  };
};

/**
 * Middleware: Authenticate JWT token
 */
const authenticateToken = (req, res, next) => {
  try {
    const authHeader = req.headers['authorization'];
    const token = extractTokenFromHeader(authHeader);

    if (!token) {
      return res.status(401).json({
        success: false,
        message: 'No token provided'
      });
    }

    const decoded = verifyAccessToken(token);
    req.user = decoded;
    next();
  } catch (error) {
    return res.status(401).json({
      success: false,
      message: error.message
    });
  }
};

/**
 * Middleware: Authorize based on role
 * @param {string[]} roles - Required roles
 */
const authorizeRole = (...roles) => {
  return (req, res, next) => {
    if (!req.user) {
      return res.status(401).json({
        success: false,
        message: 'Unauthorized'
      });
    }

    if (!roles.includes(req.user.role)) {
      return res.status(403).json({
        success: false,
        message: 'Insufficient permissions'
      });
    }

    next();
  };
};

module.exports = {
  generateAccessToken,
  generateRefreshToken,
  verifyAccessToken,
  verifyRefreshToken,
  hashPassword,
  comparePassword,
  extractTokenFromHeader,
  createTokenPair,
  authenticateToken,
  authorizeRole
};
