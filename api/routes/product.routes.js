const express = require('express');
const router = express.Router();

/**
 * Product/Sản Phẩm Routes
 * Base: /api/products
 */

// GET /api/products - Get all products
router.get('/', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/products'
  });
});

// POST /api/products - Create new product
router.post('/', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/products'
  });
});

// GET /api/products/:id - Get product details
router.get('/:id', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/products/:id'
  });
});

// PUT /api/products/:id - Update product
router.put('/:id', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/products/:id'
  });
});

// DELETE /api/products/:id - Delete product
router.delete('/:id', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/products/:id'
  });
});

module.exports = router;
