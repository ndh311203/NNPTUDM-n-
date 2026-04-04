const multer = require('multer');
const path = require('path');
const fs = require('fs');
const { v4: uuidv4 } = require('uuid');

// Create uploads directory if it doesn't exist
const uploadsDir = process.env.UPLOAD_PATH || './uploads';
if (!fs.existsSync(uploadsDir)) {
  fs.mkdirSync(uploadsDir, { recursive: true });
}

/**
 * Storage configuration for multer
 */
const storage = multer.diskStorage({
  destination: (req, file, cb) => {
    // Create subdirectories based on file type
    let folder = 'other';
    
    if (file.mimetype.startsWith('image/')) {
      folder = 'images';
    } else if (file.mimetype.startsWith('video/')) {
      folder = 'videos';
    } else if (file.mimetype.startsWith('audio/')) {
      folder = 'audio';
    } else if (
      file.mimetype === 'application/pdf' ||
      file.mimetype === 'application/msword' ||
      file.mimetype === 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'
    ) {
      folder = 'documents';
    }

    const uploadPath = path.join(uploadsDir, folder);
    
    // Create folder if it doesn't exist
    if (!fs.existsSync(uploadPath)) {
      fs.mkdirSync(uploadPath, { recursive: true });
    }

    cb(null, uploadPath);
  },
  filename: (req, file, cb) => {
    // Generate unique filename
    const uniqueSuffix = `${Date.now()}-${uuidv4().slice(0, 8)}`;
    const ext = path.extname(file.originalname);
    const name = path.basename(file.originalname, ext);
    
    const filename = `${name}-${uniqueSuffix}${ext}`;
    cb(null, filename);
  }
});

/**
 * File filter to accept specific file types
 */
const fileFilter = (req, file, cb) => {
  // Allowed MIME types
  const allowedMimes = [
    // Images
    'image/jpeg',
    'image/png',
    'image/gif',
    'image/webp',
    'image/jpg',
    // Videos
    'video/mp4',
    'video/mpeg',
    'video/quicktime',
    // Documents
    'application/pdf',
    'application/msword',
    'application/vnd.openxmlformats-officedocument.wordprocessingml.document'
  ];

  if (allowedMimes.includes(file.mimetype)) {
    cb(null, true);
  } else {
    cb(new Error(`File type not supported: ${file.mimetype}`), false);
  }
};

/**
 * Multer configuration
 */
const multerConfig = multer({
  storage: storage,
  fileFilter: fileFilter,
  limits: {
    fileSize: parseInt(process.env.MAX_FILE_SIZE) || 52428800 // 50MB default
  }
});

/**
 * Upload single file middleware
 * @param {string} fieldName - Form field name
 */
const uploadSingle = (fieldName = 'file') => {
  return multerConfig.single(fieldName);
};

/**
 * Upload multiple files middleware
 * @param {string} fieldName - Form field name
 * @param {number} maxFiles - Maximum number of files
 */
const uploadMultiple = (fieldName = 'files', maxFiles = 5) => {
  return multerConfig.array(fieldName, maxFiles);
};

/**
 * Upload fields with different names
 * @param {Array} fields - Array of field configurations
 */
const uploadFields = (fields = []) => {
  return multerConfig.fields(fields);
};

/**
 * Get file URL from file path
 * @param {string} filePath - File path
 * @returns {string} File URL
 */
const getFileUrl = (filePath) => {
  if (!filePath) return null;
  
  const baseUrl = process.env.API_URL || 'http://localhost:3000';
  const relativePath = filePath.replace(/\\/g, '/');
  
  return `${baseUrl}/${relativePath}`;
};

/**
 * Delete file from filesystem
 * @param {string} filePath - File path to delete
 */
const deleteFile = (filePath) => {
  try {
    if (fs.existsSync(filePath)) {
      fs.unlinkSync(filePath);
      return true;
    }
    return false;
  } catch (error) {
    console.error(`Error deleting file ${filePath}:`, error);
    return false;
  }
};

/**
 * Multer error handler middleware
 */
const multerErrorHandler = (error, req, res, next) => {
  if (error instanceof multer.MulterError) {
    if (error.code === 'FILE_TOO_LARGE') {
      return res.status(400).json({
        success: false,
        message: `File size exceeds maximum limit of ${process.env.MAX_FILE_SIZE || 52428800} bytes`
      });
    } else if (error.code === 'LIMIT_FILE_COUNT') {
      return res.status(400).json({
        success: false,
        message: 'Too many files uploaded'
      });
    } else if (error.code === 'LIMIT_PART_COUNT') {
      return res.status(400).json({
        success: false,
        message: 'Too many parts in request'
      });
    }
  }

  if (error) {
    return res.status(400).json({
      success: false,
      message: error.message
    });
  }

  next();
};

module.exports = {
  storage,
  multerConfig,
  uploadSingle,
  uploadMultiple,
  uploadFields,
  getFileUrl,
  deleteFile,
  multerErrorHandler
};
