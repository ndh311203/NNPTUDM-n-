const { body, param, query, validationResult } = require('express-validator');
const validate = require('validator');

/**
 * Validation helper function to handle validation errors
 */
const handleValidationErrors = (req, res, next) => {
  const errors = validationResult(req);
  
  if (!errors.isEmpty()) {
    return res.status(400).json({
      success: false,
      message: 'Validation error',
      errors: errors.array().map(err => ({
        field: err.param,
        message: err.msg,
        value: err.value
      }))
    });
  }
  
  next();
};

/**
 * Email validation
 */
const validateEmail = () => {
  return body('email')
    .trim()
    .isEmail()
    .withMessage('Invalid email format')
    .normalizeEmail();
};

/**
 * Password validation - Must be at least 6 characters
 */
const validatePassword = (fieldName = 'password') => {
  return body(fieldName)
    .isLength({ min: 6 })
    .withMessage('Password must be at least 6 characters long')
    .matches(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)/)
    .withMessage('Password must contain uppercase, lowercase, and numbers (optional for now, can be enforced)');
};

/**
 * Simple password validation - Just minimum length
 */
const validatePasswordSimple = (fieldName = 'password') => {
  return body(fieldName)
    .isLength({ min: 6 })
    .withMessage('Password must be at least 6 characters long');
};

/**
 * Vietnamese phone number validation
 */
const validatePhoneNumber = (fieldName = 'phoneNumber') => {
  return body(fieldName)
    .trim()
    .matches(/^(\+84|0)[0-9]{9,10}$/)
    .withMessage('Invalid Vietnamese phone number format');
};

/**
 * Validate required string field
 */
const validateRequiredString = (fieldName, minLength = 1, maxLength = 255) => {
  return body(fieldName)
    .trim()
    .notEmpty()
    .withMessage(`${fieldName} is required`)
    .isLength({ min: minLength, max: maxLength })
    .withMessage(`${fieldName} must be between ${minLength} and ${maxLength} characters`);
};

/**
 * Validate optional string field
 */
const validateOptionalString = (fieldName, minLength = 1, maxLength = 1000) => {
  return body(fieldName)
    .optional()
    .trim()
    .isLength({ min: minLength, max: maxLength })
    .withMessage(`${fieldName} must be between ${minLength} and ${maxLength} characters`);
};

/**
 * Validate number field
 */
const validateNumber = (fieldName, min = null, max = null) => {
  let validationChain = body(fieldName)
    .notEmpty()
    .withMessage(`${fieldName} is required`)
    .isNumeric()
    .withMessage(`${fieldName} must be a number`);

  if (min !== null) {
    validationChain = validationChain.isFloat({ min })
      .withMessage(`${fieldName} must be at least ${min}`);
  }

  if (max !== null) {
    validationChain = validationChain.isFloat({ max })
      .withMessage(`${fieldName} must not exceed ${max}`);
  }

  return validationChain;
};

/**
 * Validate date field
 */
const validateDate = (fieldName) => {
  return body(fieldName)
    .notEmpty()
    .withMessage(`${fieldName} is required`)
    .isISO8601()
    .withMessage(`${fieldName} must be a valid date (ISO 8601 format)`);
};

/**
 * Validate MongoDB ObjectId
 */
const validateObjectId = (fieldName) => {
  return param(fieldName)
    .isMongoId()
    .withMessage(`${fieldName} must be a valid MongoDB ID`);
};

/**
 * Validate array of required items
 */
const validateArray = (fieldName, minItems = 1) => {
  return body(fieldName)
    .isArray({ min: minItems })
    .withMessage(`${fieldName} must be an array with at least ${minItems} item(s)`);
};

/**
 * Validate enum/select field
 */
const validateEnum = (fieldName, allowedValues) => {
  return body(fieldName)
    .notEmpty()
    .withMessage(`${fieldName} is required`)
    .isIn(allowedValues)
    .withMessage(`${fieldName} must be one of: ${allowedValues.join(', ')}`);
};

/**
 * Validate URL
 */
const validateUrl = (fieldName) => {
  return body(fieldName)
    .isURL()
    .withMessage(`${fieldName} must be a valid URL`);
};

/**
 * Validate pagination query parameters
 */
const validatePagination = () => {
  return [
    query('page')
      .optional()
      .isInt({ min: 1 })
      .withMessage('Page must be a positive integer')
      .toInt(),
    query('limit')
      .optional()
      .isInt({ min: 1, max: 100 })
      .withMessage('Limit must be between 1 and 100')
      .toInt()
  ];
};

/**
 * Validate sort query parameter
 */
const validateSort = () => {
  return query('sort')
    .optional()
    .trim()
    .matches(/^[-+]?[a-zA-Z_][a-zA-Z0-9_]*$/)
    .withMessage('Sort parameter must be a valid field name (prefix with - for descending)');
};

/**
 * Common validation chains for auth
 */
const authValidationRules = () => {
  return [
    validateEmail(),
    validatePasswordSimple(),
    handleValidationErrors
  ];
};

/**
 * Common validation chains for register
 */
const registerValidationRules = () => {
  return [
    body('email')
      .trim()
      .isEmail()
      .withMessage('Invalid email format')
      .normalizeEmail(),
    body('password')
      .isLength({ min: 6 })
      .withMessage('Password must be at least 6 characters long'),
    body('hoTen')
      .trim()
      .notEmpty()
      .withMessage('Full name is required')
      .isLength({ min: 2, max: 100 })
      .withMessage('Full name must be between 2 and 100 characters'),
    body('soDienThoai')
      .trim()
      .matches(/^(\+84|0)[0-9]{9,10}$/)
      .withMessage('Invalid Vietnamese phone number format'),
    handleValidationErrors
  ];
};

module.exports = {
  handleValidationErrors,
  validateEmail,
  validatePassword,
  validatePasswordSimple,
  validatePhoneNumber,
  validateRequiredString,
  validateOptionalString,
  validateNumber,
  validateDate,
  validateObjectId,
  validateArray,
  validateEnum,
  validateUrl,
  validatePagination,
  validateSort,
  authValidationRules,
  registerValidationRules
};
