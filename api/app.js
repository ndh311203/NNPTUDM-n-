const express = require('express');
const cors = require('cors');
const helmet = require('helmet');
const compression = require('compression');
const morgan = require('morgan');
require('dotenv').config();

// Initialize Express app
const app = express();

// ==============================================
// Middleware Setup
// ==============================================

// Security Headers
app.use(helmet());

// Compression middleware
app.use(compression());

// CORS configuration
app.use(cors({
  origin: process.env.API_URL || 'http://localhost:3000',
  credentials: true,
  optionsSuccessStatus: 200,
  methods: ['GET', 'POST', 'PUT', 'DELETE', 'PATCH', 'OPTIONS'],
  allowedHeaders: ['Content-Type', 'Authorization']
}));

// Logging middleware
if (process.env.NODE_ENV === 'development') {
  app.use(morgan('dev'));
} else {
  app.use(morgan('combined'));
}

// Body parsers
app.use(express.json({ limit: '50mb' }));
app.use(express.urlencoded({ limit: '50mb', extended: true }));

// Static files for uploads
app.use('/uploads', express.static('uploads'));

// ==============================================
// Database Connection
// ==============================================
const { connectDB } = require('./utils/database');
connectDB();

// ==============================================
// Health Check Endpoint
// ==============================================
app.get('/health', (req, res) => {
  res.status(200).json({
    status: 'OK',
    message: 'SPATC Pet Shop API is running',
    timestamp: new Date().toISOString(),
    environment: process.env.NODE_ENV || 'development'
  });
});

// ==============================================
// API Routes
// ==============================================
// Auth Routes
app.use('/api/auth', require('./routes/auth.routes'));

// Account Routes
app.use('/api/accounts', require('./routes/account.routes'));

// Pet Routes
app.use('/api/pets', require('./routes/pet.routes'));

// Service Routes
app.use('/api/services', require('./routes/service.routes'));

// Grooming Routes
app.use('/api/grooming', require('./routes/grooming.routes'));

// Hotel/Khách sạn Routes
app.use('/api/hotel', require('./routes/hotel.routes'));

// Booking Routes
app.use('/api/bookings', require('./routes/booking.routes'));

// Shop/Products Routes
app.use('/api/products', require('./routes/product.routes'));

// Shop Orders Routes
app.use('/api/shop-orders', require('./routes/shopOrder.routes'));

// Reviews Routes
app.use('/api/reviews', require('./routes/review.routes'));

// Vouchers Routes
app.use('/api/vouchers', require('./routes/voucher.routes'));

// Notifications Routes
app.use('/api/notifications', require('./routes/notification.routes'));

// Chat Routes
app.use('/api/chat', require('./routes/chat.routes'));

// Upload Routes
app.use('/api/upload', require('./routes/upload.routes'));

// ==============================================
// Error Handling
// ==============================================

// 404 Not Found
app.use('*', (req, res) => {
  res.status(404).json({
    success: false,
    message: 'Route not found',
    path: req.originalUrl
  });
});

// Global error handler
app.use((err, req, res, next) => {
  console.error('Error:', err);

  const status = err.status || err.statusCode || 500;
  const message = err.message || 'Internal Server Error';

  res.status(status).json({
    success: false,
    status,
    message,
    ...(process.env.NODE_ENV === 'development' && { stack: err.stack })
  });
});

module.exports = app;
