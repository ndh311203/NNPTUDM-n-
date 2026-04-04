const express = require('express');
const router = express.Router();

/**
 * Hotel/Khách Sạn Routes
 * Base: /api/hotel
 */

// GET /api/hotel/rooms - Get all rooms
router.get('/rooms', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/hotel/rooms'
  });
});

// POST /api/hotel/rooms - Create new room
router.post('/rooms', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/hotel/rooms'
  });
});

// GET /api/hotel/rooms/:id - Get room details
router.get('/rooms/:id', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/hotel/rooms/:id'
  });
});

// PUT /api/hotel/rooms/:id - Update room
router.put('/rooms/:id', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/hotel/rooms/:id'
  });
});

// DELETE /api/hotel/rooms/:id - Delete room
router.delete('/rooms/:id', (req, res) => {
  res.status(501).json({
    message: 'Route not implemented yet',
    path: '/api/hotel/rooms/:id'
  });
});

module.exports = router;
