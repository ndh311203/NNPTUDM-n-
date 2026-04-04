const express = require('express');
const router = express.Router();

/**
 * Pet/Thú Cưng Routes
 * Base: /api/pets
 */

// GET /api/pets - Get all pets
router.get('/', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/pets'
  });
});

// POST /api/pets - Create new pet
router.post('/', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/pets'
  });
});

// GET /api/pets/:id - Get pet details
router.get('/:id', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/pets/:id'
  });
});

// PUT /api/pets/:id - Update pet
router.put('/:id', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/pets/:id'
  });
});

// DELETE /api/pets/:id - Delete pet
router.delete('/:id', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/pets/:id'
  });
});

module.exports = router;
