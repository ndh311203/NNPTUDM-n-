const express = require('express');
const router = express.Router();
const uploadController = require('../controllers/upload.controller');
const { uploadSingle, uploadMultiple, multerErrorHandler } = require('../utils/uploadHandler');

/**
 * Upload Routes
 * Base: /api/upload
 */

router.post('/single', uploadSingle('file'), multerErrorHandler, uploadController.uploadSingleFile);
router.post('/multiple', uploadMultiple('files'), multerErrorHandler, uploadController.uploadMultipleFiles);
router.delete('/', uploadController.deleteUploadedFile);

module.exports = router;
