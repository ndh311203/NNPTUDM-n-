const { v4: uuidv4 } = require('uuid');

/**
 * ID Generation and Handling Utilities
 */

/**
 * Generate UUID v4 (random unique identifier)
 * @returns {string} UUID v4 string
 */
const generateUUID = () => {
  return uuidv4();
};

/**
 * Generate short ID (8 characters)
 * @returns {string} Short unique ID
 */
const generateShortId = () => {
  return uuidv4().slice(0, 8).toUpperCase();
};

/**
 * Generate order ID with timestamp
 * @param {string} prefix - Prefix for order ID (e.g., 'ORD', 'INV')
 * @returns {string} Generated order ID
 */
const generateOrderId = (prefix = 'ORD') => {
  const timestamp = Date.now().toString().slice(-8);
  const random = Math.floor(Math.random() * 10000).toString().padStart(4, '0');
  return `${prefix}-${timestamp}${random}`;
};

/**
 * Generate booking ID
 * @returns {string} Booking ID
 */
const generateBookingId = () => {
  return generateOrderId('BOK');
};

/**
 * Generate invoice ID
 * @returns {string} Invoice ID
 */
const generateInvoiceId = () => {
  return generateOrderId('INV');
};

/**
 * Generate transaction ID
 * @returns {string} Transaction ID
 */
const generateTransactionId = () => {
  return generateOrderId('TXN');
};

/**
 * Generate referral code
 * @param {number} length - Length of code (default: 8)
 * @returns {string} Referral code
 */
const generateReferralCode = (length = 8) => {
  const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
  let result = '';
  for (let i = 0; i < length; i++) {
    result += chars.charAt(Math.floor(Math.random() * chars.length));
  }
  return result;
};

/**
 * Generate coupon code
 * @param {number} length - Length of code (default: 12)
 * @returns {string} Coupon code
 */
const generateCouponCode = (length = 12) => {
  const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
  let result = 'CP-';
  for (let i = 0; i < length; i++) {
    result += chars.charAt(Math.floor(Math.random() * chars.length));
  }
  return result;
};

/**
 * Generate reset token (for password reset, email verification, etc.)
 * @param {number} length - Length of token (default: 32)
 * @returns {string} Reset token
 */
const generateResetToken = (length = 32) => {
  const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
  let result = '';
  for (let i = 0; i < length; i++) {
    result += chars.charAt(Math.floor(Math.random() * chars.length));
  }
  return result;
};

/**
 * Generate OTP (One-Time Password) - 6 digits
 * @returns {string} 6-digit OTP
 */
const generateOTP = () => {
  return Math.floor(100000 + Math.random() * 900000).toString();
};

/**
 * Generate VietQR transaction reference
 * @param {string} accountNumber - Bank account number
 * @returns {string} VietQR reference
 */
const generateVietQRReference = (accountNumber) => {
  const timestamp = Date.now().toString().slice(-10);
  const random = Math.random().toString(36).substring(2, 7).toUpperCase();
  return `${accountNumber}-${timestamp}-${random}`;
};

/**
 * Check if string is valid MongoDB ObjectId
 * @param {string} id - ID to check
 * @returns {boolean} True if valid MongoDB ID
 */
const isValidMongoId = (id) => {
  return /^[0-9a-fA-F]{24}$/.test(id);
};

/**
 * Check if string is valid UUID
 * @param {string} uuid - UUID to check
 * @returns {boolean} True if valid UUID
 */
const isValidUUID = (uuid) => {
  const uuidRegex = /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;
  return uuidRegex.test(uuid);
};

/**
 * Parse and validate ID (supports both MongoDB ID and UUID)
 * @param {string} id - ID to parse
 * @returns {Object} { isValid: boolean, type: 'mongo' | 'uuid' | 'invalid', value: string }
 */
const parseId = (id) => {
  if (isValidMongoId(id)) {
    return { isValid: true, type: 'mongo', value: id };
  }
  
  if (isValidUUID(id)) {
    return { isValid: true, type: 'uuid', value: id };
  }
  
  return { isValid: false, type: 'invalid', value: id };
};

/**
 * Generate random color hex code
 * @returns {string} Hex color code
 */
const generateRandomColor = () => {
  return '#' + Math.floor(Math.random() * 16777215).toString(16).padStart(6, '0');
};

/**
 * Generate random numeric code
 * @param {number} length - Length of code
 * @returns {string} Numeric code
 */
const generateNumericCode = (length = 6) => {
  let result = '';
  for (let i = 0; i < length; i++) {
    result += Math.floor(Math.random() * 10).toString();
  }
  return result;
};

module.exports = {
  generateUUID,
  generateShortId,
  generateOrderId,
  generateBookingId,
  generateInvoiceId,
  generateTransactionId,
  generateReferralCode,
  generateCouponCode,
  generateResetToken,
  generateOTP,
  generateVietQRReference,
  isValidMongoId,
  isValidUUID,
  parseId,
  generateRandomColor,
  generateNumericCode
};
