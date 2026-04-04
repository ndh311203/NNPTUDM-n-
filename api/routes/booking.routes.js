const express = require('express');
const router = express.Router();

/**
 * Booking/Đặt Lịch Routes
 * Base: /api/bookings
 */

// GET /api/bookings - Get all bookings
router.get('/', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/bookings'
  });
});

// POST /api/bookings - Create new booking
router.post('/', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/bookings'
  });
});

// GET /api/bookings/:id - Get booking details
router.get('/:id', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/bookings/:id'
  });
});

// PUT /api/bookings/:id - Update booking
router.put('/:id', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/bookings/:id'
  });
});

// DELETE /api/bookings/:id - Cancel booking
router.delete('/:id', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/bookings/:id'
  });
});

module.exports = router;
