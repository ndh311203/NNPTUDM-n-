const express = require('express');
const router = express.Router();

/**
 * Service/Dịch Vụ Routes
 * Base: /api/services
 */

// GET /api/services - Get all services
router.get('/', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/services'
  });
});

// POST /api/services - Create new service
router.post('/', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/services'
  });
});

// GET /api/services/:id - Get service details
router.get('/:id', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/services/:id'
  });
});

// PUT /api/services/:id - Update service
router.put('/:id', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/services/:id'
  });
});

// DELETE /api/services/:id - Delete service
router.delete('/:id', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/services/:id'
  });
});

module.exports = router;
