const express = require('express');
const router = express.Router();
const { uploadSingle, multerErrorHandler } = require('../utils/uploadHandler');

/**
 * Upload Routes
 * Base: /api/upload
 */

// POST /api/upload/single - Upload single file
router.post('/single', uploadSingle('file'), multerErrorHandler, (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/upload/single'
  });
});

// POST /api/upload/multiple - Upload multiple files
router.post('/multiple', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/upload/multiple'
  });
});

// DELETE /api/upload/:id - Delete uploaded file
router.delete('/:id', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/upload/:id'
  });
});

module.exports = router;
