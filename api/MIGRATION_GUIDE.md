# Migration Guide: ASP.NET to Express Setup

This guide walks you through the structure and next steps for the Node.js Express migration.

## ✅ Completed (Phase 1)

### Infrastructure
- ✅ Project structure created
- ✅ Express.js app configuration
- ✅ MongoDB/Mongoose connection logic
- ✅ Environment configuration (.env)
- ✅ Security middleware (Helmet, CORS)
- ✅ Logging (Morgan)
- ✅ File upload handler (Multer)

### Utilities
- ✅ `utils/authHandler.js` - JWT token generation/verification, password hashing
- ✅ `utils/uploadHandler.js` - File upload with Multer configuration
- ✅ `utils/validator.js` - Input validation chains
- ✅ `utils/idHandler.js` - ID generation (UUID, order IDs, OTP, etc.)
- ✅ `utils/database.js` - MongoDB connection management

### Routes (Stubs)
- ✅ All route files created with placeholder endpoints
- ✅ Middleware integration ready

### Schemas (Sample)
- ✅ `TaiKhoan.js` - Account/authentication schema
- ✅ `KhachHang.js` - Customer profile schema
- ✅ `ThuCung.js` - Pet schema with relationships

### Controllers (Sample)
- ✅ `controllers/auth.controller.js` - Authentication example

## 📋 Next Steps (Phase 2)

### 1. Create Remaining Mongoose Schemas
Create schema files in `schemas/`:
- `DichVu.js` - Services
- `DichVuGrooming.js` - Grooming services
- `PhongKhachSan.js` - Hotel rooms
- `DatLich.js` - Bookings
- `SanPham.js` - Products
- `ShopOrder.js` & `ShopOrderItem.js` - Orders
- `ProductReview.js` & `ServiceReview.js` - Reviews
- `Voucher.js` - Discount codes
- `ThongBao.js` - Notifications
- `ChatMessage.js` - Chat messages
- `CaLam.js`, `CongViec.js`, `LichPhanCong.js` - Shift management

See `schemas/` folder for examples of schema structure.

### 2. Implement Controllers
Create controller files in `controllers/`:
- `auth.controller.js` - ✅ Already created (example)
- `account.controller.js` - Customer account management
- `pet.controller.js` - Pet CRUD operations
- `service.controller.js` - Service management
- `booking.controller.js` - Booking management
- `product.controller.js` - Product management
- `order.controller.js` - Order processing
- `review.controller.js` - Review management
- `upload.controller.js` - File upload handling

Each controller should:
1. Import required schemas
2. Implement CRUD operations
3. Include validation
4. Add error handling
5. Follow REST conventions

### 3. Complete Route Implementations
Update route files in `routes/` with:
- Middleware bindings (validation, authentication)
- Controller method references
- Request/response handling

Example of complete route:
```javascript
const express = require('express');
const router = express.Router();
const { authenticateToken } = require('../utils/authHandler');
const controller = require('../controllers/pet.controller');

// GET /api/pets - List all pets
router.get('/', authenticateToken, controller.getAllPets);

// POST /api/pets - Create pet
router.post('/', authenticateToken, controller.createPet);

// GET /api/pets/:id - Get pet details
router.get('/:id', authenticateToken, controller.getPetById);

// PUT /api/pets/:id - Update pet
router.put('/:id', authenticateToken, controller.updatePet);

// DELETE /api/pets/:id - Delete pet
router.delete('/:id', authenticateToken, controller.deletePet);

module.exports = router;
```

### 4. Add Authentication to Routes
Update route implementations to use:
```javascript
const { authenticateToken, authorizeRole } = require('../utils/authHandler');

// Protect route
router.post('/admin-only', authenticateToken, authorizeRole('admin'), controller.method);
```

### 5. Implement Socket.IO Features
For real-time features (chat, notifications):
1. Create `hubs/` folder (similar to C# Hubs)
2. Implement Socket event handlers
3. Connect to app.io instance

Example:
```javascript
const io = app.get('io');

io.on('connection', (socket) => {
  console.log('User connected:', socket.id);
  
  socket.on('send-message', (data) => {
    io.emit('receive-message', data);
  });
});
```

### 6. Add Service Layer (Optional but Recommended)
Create `services/` folder for business logic:
- `services/authService.js`
- `services/petService.js`
- `services/orderService.js`
- etc.

This separates business logic from HTTP handling.

### 7. Testing
- Create `tests/` folder
- Write Jest tests for controllers and services
- Run: `npm test`

## 📊 Data Mapping Reference

### C# to Mongoose Type Mapping

| C# Type | Mongoose Type |
|---------|---------------|
| int | Number |
| string | String |
| bool | Boolean |
| DateTime | Date |
| decimal | Number (or use `mongoose-currency`) |
| Guid | ObjectId or String |
| List<T> | Array |
| virtual List<T> | ref array |
| Foreign Key | ObjectId with ref |

### Relationship Mapping

#### 1-to-1 Relationships
```javascript
// C#: TaiKhoan.KhachHang (1-1)
taiKhoanId: {
  type: mongoose.Schema.Types.ObjectId,
  ref: 'KhachHang'
}
```

#### 1-to-Many Relationships
```javascript
// C#: KhachHang.ThuCungs (1-many)
// In KhachHang schema:
// Pets are referenced in ThuCung.khachHangId

// In ThuCung schema:
khachHangId: {
  type: mongoose.Schema.Types.ObjectId,
  ref: 'KhachHang'
}
```

#### Many-to-Many (if needed)
```javascript
// Example for ShopOrder.items (many-to-many)
items: [{
  type: mongoose.Schema.Types.ObjectId,
  ref: 'SanPham'
}]
```

## 🔑 Key Implementation Patterns

### 1. Controller Response Format
```javascript
// Success response
res.status(200).json({
  success: true,
  message: 'Operation successful',
  data: { /* returned data */ }
});

// Error response
res.status(400).json({
  success: false,
  message: 'Error description',
  errors: [/* validation errors */]
});
```

### 2. Validation in Routes
```javascript
const { validateEmail, validatePhoneNumber, handleValidationErrors } = require('../utils/validator');

router.post('/register', 
  validateEmail(),
  validatePhoneNumber(),
  handleValidationErrors,
  controller.register
);
```

### 3. Authentication Middleware
```javascript
const { authenticateToken, authorizeRole } = require('../utils/authHandler');

// Protected route
router.get('/profile', authenticateToken, (req, res) => {
  console.log(req.user); // Contains userId, email, role
});

// Role-based protection
router.delete('/user', authenticateToken, authorizeRole('admin'), controller.delete);
```

### 4. File Upload
```javascript
const { uploadSingle, multerErrorHandler } = require('../utils/uploadHandler');

router.post('/upload',
  authenticateToken,
  uploadSingle('file'),
  multerErrorHandler,
  controller.handleUpload
);
```

## 🚀 Quick Start Checklist

- [ ] Install dependencies: `npm install`
- [ ] Configure `.env` file
- [ ] Start MongoDB
- [ ] Create Mongoose schemas for remaining entities
- [ ] Implement controllers with business logic
- [ ] Complete route implementations
- [ ] Test all endpoints
- [ ] Add Socket.IO handlers for real-time features
- [ ] Implement admin dashboard routes
- [ ] Add payment integration
- [ ] Deploy to production

## 📚 File Organization Summary

```
api/
├── bin/www                    # Server entry point ✅
├── controllers/               # Business logic implementations
│   ├── auth.controller.js     # ✅ Example
│   ├── account.controller.js
│   ├── pet.controller.js
│   └── ...
├── routes/                    # API endpoint definitions  
│   ├── auth.routes.js         # Stubs with example
│   ├── pet.routes.js
│   └── ...
├── schemas/                   # Mongoose models
│   ├── TaiKhoan.js            # ✅ Example
│   ├── KhachHang.js           # ✅ Example  
│   ├── ThuCung.js             # ✅ Example
│   └── ...
├── services/                  # Business logic layer (optional)
├── hubs/                      # Socket.IO handlers (optional)
├── utils/                     # Utility functions
│   ├── authHandler.js         # ✅ JWT, passwords
│   ├── uploadHandler.js       # ✅ Multer config
│   ├── validator.js           # ✅ Validation rules
│   ├── idHandler.js           # ✅ ID generation
│   └── database.js            # ✅ DB connection
├── uploads/                   # Uploaded files
├── app.js                     # Express app ✅
├── package.json               # Dependencies ✅
├── .env                       # Configuration ✅
├── .env.example               # Config template ✅
├── .gitignore                 # Git rules ✅
└── README.md                  # Documentation ✅
```

## 💡 Tips for Implementation

1. **Test as you go** - Use Postman/Insomnia to test each endpoint
2. **Follow REST conventions** - Use proper HTTP methods and status codes
3. **Consistent error handling** - Always return error responses in the same format
4. **Input validation** - Validate all inputs before processing
5. **Database indexes** - Add indexes for frequently queried fields
6. **Error logging** - Log errors for debugging production issues
7. **Rate limiting** - Add rate limiting for security (express-rate-limit)
8. **Documentation** - Document each endpoint with endpoint comments

## 🔗 MongoDB Connection Troubleshooting

If MongoDB connection fails:

1. **Ensure MongoDB is running**:
   ```bash
   # For Windows
   mongod --dbpath "C:\path\to\data"
   
   # Or use MongoDB as service
   net start MongoDB
   ```

2. **Check MONGODB_URI in .env** - Should be `mongodb://localhost:27017/spatc`

3. **Test connection**:
   ```bash
   mongo mongodb://localhost:27017/spatc
   ```

4. **Check firewall** - Ensure port 27017 is open

## ✨ Next Implementation: Pet Controller Example

See `controllers/auth.controller.js` for a complete example. Follow the same pattern for other controllers:

1. Import schema and utilities
2. Create async functions for each operation
3. Include validation
4. Add error handling
5. Return consistent response format
6. Export all functions

---

**Status**: Ready for Phase 2 implementation! 🎉
