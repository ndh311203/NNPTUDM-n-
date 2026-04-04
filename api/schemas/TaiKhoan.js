const mongoose = require('mongoose');

/**
 * TaiKhoan Schema (Account)
 * Represents user account for authentication
 */
const taiKhoanSchema = new mongoose.Schema(
  {
    // Basic Information
    email: {
      type: String,
      required: [true, 'Email is required'],
      unique: true,
      lowercase: true,
      trim: true,
      match: [/^\w+([\\.-]?\w+)*@\w+([\\.-]?\w+)*(\.\w{2,3})+$/, 'Please provide a valid email']
    },
    
    matKhau: {
      type: String,
      required: [true, 'Password is required'],
      minlength: 6,
      select: false // Don't return password by default
    },
    
    hoTen: {
      type: String,
      trim: true,
      maxlength: [100, 'Name cannot exceed 100 characters']
    },
    
    // Role and Permissions
    vaiTro: {
      type: String,
      enum: {
        values: ['user', 'staff', 'admin'],
        message: 'Role must be user, staff, or admin'
      },
      default: 'user'
    },
    
    // Account Status
    trangThai: {
      type: Boolean,
      default: true // true = active, false = locked
    },
    
    // OAuth Provider Information
    provider: {
      type: String,
      enum: ['facebook', 'google', null],
      default: null
    },
    
    providerId: {
      type: String,
      sparse: true // Allow null values but unique constraints
    },
    
    // Timestamps
    ngayTao: {
      type: Date,
      default: Date.now
    },
    
    ngayCapNhat: {
      type: Date,
      default: Date.now
    },
    
    // Last Login
    lastLogin: {
      type: Date,
      default: null
    },
    
    // Verification
    emailVerified: {
      type: Boolean,
      default: false
    },
    
    emailVerificationToken: {
      type: String,
      default: null
    },
    
    // Password Reset
    passwordResetToken: {
      type: String,
      default: null
    },
    
    passwordResetExpires: {
      type: Date,
      default: null
    }
  },
  {
    timestamps: true // Auto add createdAt, updatedAt
  }
);

// Indexes for faster queries
taiKhoanSchema.index({ email: 1 });
taiKhoanSchema.index({ providerId: 1 });
taiKhoanSchema.index({ vaiTro: 1 });

// Hide sensitive fields in JSON output
taiKhoanSchema.methods.toJSON = function() {
  const obj = this.toObject();
  delete obj.matKhau;
  delete obj.passwordResetToken;
  delete obj.emailVerificationToken;
  return obj;
};

/**
 * Pre-save middleware
 * Hash password before saving (implement in service layer)
 */
taiKhoanSchema.pre('save', function(next) {
  // Password hashing will be handled in authHandler.js
  // Just update the updateDate
  this.ngayCapNhat = Date.now();
  next();
});

module.exports = mongoose.model('TaiKhoan', taiKhoanSchema);
