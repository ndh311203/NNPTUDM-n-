# SPATC Pet Shop Management System - REST API

A Node.js Express REST API for a comprehensive pet shop management system, migrated from ASP.NET Core MVC.

## 📋 Project Overview

SPATC is a full-featured pet shop management platform with:
- **User Management**: Account authentication, OAuth integration (Google, Facebook)
- **Pet Management**: Store and manage customer pets
- **Services**: Grooming, boarding, and pet care services
- **Bookings**: Schedule and manage service appointments
- **Hotel System**: Pet hotel/boarding room management
- **Shop**: Pet products, food, accessories with e-commerce features
- **Orders**: Shop order management with payment integration (VietQR)
- **Reviews**: Product and service review system
- **Vouchers**: Discount code management
- **Notifications**: Real-time notifications system
- **Chat**: Customer support and messaging system
- **Real-time Features**: Socket.IO for real-time updates

## 🏗 Project Structure

```
api/
├── bin/
│   └── www                    # Server entry point
├── controllers/               # Route handlers (business logic)
├── routes/                    # API endpoint definitions
├── schemas/                   # Mongoose schemas (models)
├── utils/                     # Utility functions
│   ├── authHandler.js         # JWT + password hashing
│   ├── uploadHandler.js       # Multer file upload config
│   ├── validator.js           # Request validation rules
│   ├── idHandler.js           # ID generation utilities
│   └── database.js            # MongoDB connection
├── uploads/                   # Uploaded files storage
├── app.js                     # Express app configuration
├── package.json               # Project dependencies
├── .env                       # Environment variables
├── .env.example               # Environment template
├── .gitignore                 # Git ignore rules
└── README.md                  # This file
```

## 🔧 Tech Stack

- **Runtime**: Node.js 18+ 
- **Framework**: Express.js 4.21
- **Database**: MongoDB + Mongoose ODM
- **Authentication**: JWT (JSON Web Tokens) with refresh tokens
- **File Upload**: Multer
- **Real-time**: Socket.IO
- **Validation**: Express-validator
- **Security**: Helmet, CORS, Bcrypt
- **Compression**: Gzip middleware

## 📦 Installation

### Prerequisites
- Node.js 18+
- npm or yarn
- MongoDB installed and running locally (or connection URI)

### Setup Steps

1. **Install Dependencies**
   ```bash
   npm install
   ```

2. **Configure Environment**
   ```bash
   cp .env.example .env
   ```
   
   Edit `.env` and configure:
   - `MONGODB_URI`: MongoDB connection string
   - `JWT_SECRET`: Secret key for JWT tokens
   - `GOOGLE_CLIENT_ID` / `GOOGLE_CLIENT_SECRET`: Google OAuth credentials
   - `FACEBOOK_APP_ID` / `FACEBOOK_APP_SECRET`: Facebook OAuth credentials
   - Other settings as needed

3. **Start Development Server**
   ```bash
   npm run dev
   ```
   
   Server will run on `http://localhost:3000`

4. **Start Production Server**
   ```bash
   npm start
   ```

## 🚀 API Endpoints

### Authentication
- `POST /api/auth/register` - Register new user
- `POST /api/auth/login` - Login user
- `POST /api/auth/refresh` - Refresh access token
- `POST /api/auth/logout` - Logout user
- `POST /api/auth/google` - Google OAuth login
- `POST /api/auth/facebook` - Facebook OAuth login

### Account Management
- `GET /api/accounts/:id` - Get account details
- `PUT /api/accounts/:id` - Update account
- `DELETE /api/accounts/:id` - Delete account

### Pets
- `GET /api/pets` - List all pets
- `POST /api/pets` - Create pet
- `GET /api/pets/:id` - Get pet details
- `PUT /api/pets/:id` - Update pet
- `DELETE /api/pets/:id` - Delete pet

### Services
- `GET /api/services` - List services
- `POST /api/services` - Create service
- `GET /api/services/:id` - Get service details
- `PUT /api/services/:id` - Update service
- `DELETE /api/services/:id` - Delete service

### Grooming
- `GET /api/grooming` - List grooming services
- `POST /api/grooming` - Create grooming service
- `GET /api/grooming/:id` - Get grooming service
- `PUT /api/grooming/:id` - Update grooming service
- `DELETE /api/grooming/:id` - Delete grooming service

### Hotel/Boarding
- `GET /api/hotel/rooms` - List hotel rooms
- `POST /api/hotel/rooms` - Create room
- `GET /api/hotel/rooms/:id` - Get room details
- `PUT /api/hotel/rooms/:id` - Update room
- `DELETE /api/hotel/rooms/:id` - Delete room

### Bookings
- `GET /api/bookings` - List bookings
- `POST /api/bookings` - Create booking
- `GET /api/bookings/:id` - Get booking details
- `PUT /api/bookings/:id` - Update booking
- `DELETE /api/bookings/:id` - Cancel booking

### Products/Shop
- `GET /api/products` - List products
- `POST /api/products` - Create product
- `GET /api/products/:id` - Get product details
- `PUT /api/products/:id` - Update product
- `DELETE /api/products/:id` - Delete product

### Shop Orders
- `GET /api/shop-orders` - List orders
- `POST /api/shop-orders` - Create order
- `GET /api/shop-orders/:id` - Get order details
- `PUT /api/shop-orders/:id` - Update order status
- `DELETE /api/shop-orders/:id` - Cancel order

### Reviews
- `GET /api/reviews` - List reviews
- `POST /api/reviews/product` - Create product review
- `POST /api/reviews/service` - Create service review
- `GET /api/reviews/:id` - Get review details
- `PUT /api/reviews/:id` - Update review
- `DELETE /api/reviews/:id` - Delete review

### Vouchers
- `GET /api/vouchers` - List vouchers
- `POST /api/vouchers` - Create voucher
- `GET /api/vouchers/:id` - Get voucher details
- `PUT /api/vouchers/:id` - Update voucher
- `DELETE /api/vouchers/:id` - Delete voucher
- `POST /api/vouchers/validate` - Validate voucher code

### Notifications
- `GET /api/notifications` - List notifications
- `POST /api/notifications` - Create notification
- `GET /api/notifications/:id` - Get notification
- `PUT /api/notifications/:id/read` - Mark as read
- `DELETE /api/notifications/:id` - Delete notification

### Chat
- `GET /api/chat/messages` - List messages
- `POST /api/chat/messages` - Send message
- `GET /api/chat/conversations` - List conversations
- `PUT /api/chat/messages/:id` - Edit message
- `DELETE /api/chat/messages/:id` - Delete message

### File Upload
- `POST /api/upload/single` - Upload single file
- `POST /api/upload/multiple` - Upload multiple files
- `DELETE /api/upload/:id` - Delete file

### Health Check
- `GET /health` - Server health status

## 🔐 Authentication

The API uses JWT (JSON Web Token) for authentication:

1. **Login** → Receive `accessToken` and `refreshToken`
2. **Include** `accessToken` in `Authorization: Bearer <token>` header
3. **Token expires** in 7 days by default
4. **Refresh** token when expired using `/api/auth/refresh`

Example request:
```bash
curl -H "Authorization: Bearer eyJhbGc..." https://localhost:3000/api/pets
```

## 📤 File Upload

Files are uploaded to the `uploads/` directory with automatic subdirectories:
- `uploads/images/` - Image files
- `uploads/videos/` - Video files
- `uploads/documents/` - PDF, Word files
- `uploads/audio/` - Audio files
- `uploads/other/` - Other file types

Max file size: 50MB (configurable via `MAX_FILE_SIZE` in `.env`)

## 🧪 Testing

Run tests with:
```bash
npm test
```

## 🛣 Roadmap

### Phase 1: Core Features (Current)
- [x] Project structure setup
- [x] Database connection
- [x] Authentication utilities
- [x] Upload handler
- [x] Validation utilities
- [ ] Mongoose schemas
- [ ] Controllers implementation
- [ ] Route handlers implementation

### Phase 2: Business Logic
- [ ] User registration and login
- [ ] Pet management
- [ ] Service bookings
- [ ] Order management
- [ ] Payment integration

### Phase 3: Advanced Features
- [ ] Real-time notifications (Socket.IO)
- [ ] Chat system
- [ ] Admin dashboard
- [ ] Analytics and reports
- [ ] Performance optimization

## 📝 Environment Variables

Copy `.env.example` to `.env` and configure:

```env
# Server
NODE_ENV=development
PORT=3000
API_URL=http://localhost:3000

# Database
MONGODB_URI=mongodb://localhost:27017/spatc

# JWT
JWT_SECRET=your-secret-key-here
JWT_EXPIRE=7d
REFRESH_TOKEN_SECRET=your-refresh-secret
REFRESH_TOKEN_EXPIRE=30d

# OAuth
GOOGLE_CLIENT_ID=your-google-client-id
GOOGLE_CLIENT_SECRET=your-google-secret
FACEBOOK_APP_ID=your-facebook-app-id
FACEBOOK_APP_SECRET=your-facebook-secret

# File Upload
MAX_FILE_SIZE=52428800
UPLOAD_PATH=./uploads

# VietQR Payment
VIETQR_BANK_NAME=MB Bank
VIETQR_ACCOUNT_NUMBER=0842609749
VIETQR_ACCOUNT_NAME=NGUYEN DUC HUY
```

## 🐛 Troubleshooting

### MongoDB Connection Failed
- Ensure MongoDB is running: `mongod`
- Check `MONGODB_URI` in `.env`
- Check network connectivity

### JWT Token Errors
- Ensure `JWT_SECRET` is set in `.env`
- Check token hasn't expired
- Verify token format: `Bearer <token>`

### File Upload Issues
- Check upload path exists and has write permissions
- Verify `MAX_FILE_SIZE` setting
- Check disk space

## 📚 Migration Notes

This API is a direct migration from the ASP.NET Core C# project:
- Entity relationships are preserved in Mongoose schemas
- Authentication logic adapted from C# to Node.js
- File upload handlers adapted from ASP.NET to Multer
- Validation rules maintained for consistency

## 🤝 Contributing

Guidelines for contributing (to be added)

## 📄 License

(License to be determined)

## 👨‍💼 Author

Migrated from C# ASP.NET Core to Node.js Express

---

**Status**: 🟡 In Development - Core structure complete, awaiting schema and controller implementation
