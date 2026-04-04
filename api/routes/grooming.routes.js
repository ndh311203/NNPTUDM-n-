const express = require('express');
const router = express.Router();

/**
 * Grooming Service Routes
 * Base: /api/grooming
 */

// GET /api/grooming - Get all grooming services
router.get('/', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/grooming'
  });
});

// POST /api/grooming - Create new grooming service
router.post('/', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/grooming'
  });
});

// GET /api/grooming/:id - Get grooming service details
router.get('/:id', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/grooming/:id'
  });
});

// PUT /api/grooming/:id - Update grooming service
router.put('/:id', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/grooming/:id'
  });
});

// DELETE /api/grooming/:id - Delete grooming service
router.delete('/:id', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/grooming/:id'
  });
});

module.exports = router;
