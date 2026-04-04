const express = require('express');
const router = express.Router();

/**
 * Review Routes (Products & Services)
 * Base: /api/reviews
 */

// GET /api/reviews - Get all reviews
router.get('/', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/reviews'
  });
});

// POST /api/reviews/product - Create product review
router.post('/product', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/reviews/product'
  });
});

// POST /api/reviews/service - Create service review
router.post('/service', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/reviews/service'
  });
});

// GET /api/reviews/:id - Get review details
router.get('/:id', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/reviews/:id'
  });
});

// PUT /api/reviews/:id - Update review
router.put('/:id', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/reviews/:id'
  });
});

// DELETE /api/reviews/:id - Delete review
router.delete('/:id', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/reviews/:id'
  });
});

module.exports = router;
