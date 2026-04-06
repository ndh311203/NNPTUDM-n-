const { v4: uuidv4 } = require("uuid");

const generateUUID = () => {
  return uuidv4();
};

const generateShortId = () => {
  return uuidv4().slice(0, 8).toUpperCase();
};

const generateOrderId = (prefix = "ORD") => {
  const timestamp = Date.now().toString().slice(-8);
  const random = Math.floor(Math.random() * 10000)
    .toString()
    .padStart(4, "0");
  return `${prefix}-${timestamp}${random}`;
};

const generateBookingId = () => {
  return generateOrderId("BOK");
};

const generateInvoiceId = () => {
  return generateOrderId("INV");
};

const generateTransactionId = () => {
  return generateOrderId("TXN");
};

const generateReferralCode = (length = 8) => {
  const chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
  let result = "";
  for (let i = 0; i < length; i++) {
    result += chars.charAt(Math.floor(Math.random() * chars.length));
  }
  return result;
};

const generateCouponCode = (length = 12) => {
  const chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
  let result = "CP-";
  for (let i = 0; i < length; i++) {
    result += chars.charAt(Math.floor(Math.random() * chars.length));
  }
  return result;
};

const generateResetToken = (length = 32) => {
  const chars =
    "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
  let result = "";
  for (let i = 0; i < length; i++) {
    result += chars.charAt(Math.floor(Math.random() * chars.length));
  }
  return result;
};

const generateOTP = () => {
  return Math.floor(100000 + Math.random() * 900000).toString();
};

const generateVietQRReference = (accountNumber) => {
  const timestamp = Date.now().toString().slice(-10);
  const random = Math.random().toString(36).substring(2, 7).toUpperCase();
  return `${accountNumber}-${timestamp}-${random}`;
};

const isValidMongoId = (id) => {
  return /^[0-9a-fA-F]{24}$/.test(id);
};

const isValidUUID = (uuid) => {
  const uuidRegex =
    /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;
  return uuidRegex.test(uuid);
};

const parseId = (id) => {
  if (isValidMongoId(id)) {
    return { isValid: true, type: "mongo", value: id };
  }

  if (isValidUUID(id)) {
    return { isValid: true, type: "uuid", value: id };
  }

  return { isValid: false, type: "invalid", value: id };
};

const generateRandomColor = () => {
  return (
    "#" +
    Math.floor(Math.random() * 16777215)
      .toString(16)
      .padStart(6, "0")
  );
};

const generateNumericCode = (length = 6) => {
  let result = "";
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
  generateNumericCode,
};
