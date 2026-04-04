const mongoose = require('mongoose');

/**
 * KhachHang Schema (Customer/Account Owner)
 * Extended profile for customers
 */
const khachHangSchema = new mongoose.Schema(
  {
    // Personal Information
    hoTen: {
      type: String,
      required: [true, 'Full name is required'],
      trim: true,
      maxlength: [100, 'Name cannot exceed 100 characters']
    },
    
    soDienThoai: {
      type: String,
      required: [true, 'Phone number is required'],
      unique: true,
      match: [/^(\+84|0)[0-9]{9,10}$/, 'Invalid Vietnamese phone number'],
      trim: true
    },
    
    email: {
      type: String,
      lowercase: true,
      trim: true,
      sparse: true
    },
    
    // Address
    diaChi: {
      type: String,
      maxlength: [500, 'Address cannot exceed 500 characters']
    },
    
    // Additional Info
    ghiChu: {
      type: String,
      maxlength: [1000, 'Notes cannot exceed 1000 characters']
    },
    
    // Avatar/Profile Picture
    anhDaiDien: {
      type: String,
      default: null
    },
    
    // Link to TaiKhoan (Account)
    taiKhoanId: {
      type: mongoose.Schema.Types.ObjectId,
      ref: 'TaiKhoan',
      unique: true,
      sparse: true
    },
    
    // Pets owned by this customer (will be populated from ThuCung schema)
    // Actual relationship is in ThuCung.khachHangId pointing to this
    
    // Account Status
    trangThai: {
      type: Boolean,
      default: true
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
    
    // Statistics
    tongDonHang: {
      type: Number,
      default: 0
    },
    
    tongDichVu: {
      type: Number,
      default: 0
    },
    
    tongChiTieu: {
      type: Number,
      default: 0
    }
  },
  {
    timestamps: true
  }
);

// Indexes for faster queries
khachHangSchema.index({ soDienThoai: 1 });
khachHangSchema.index({ email: 1 });
khachHangSchema.index({ taiKhoanId: 1 });

// Virtual for full name formatting
khachHangSchema.virtual('fullName').get(function() {
  return this.hoTen;
});

/**
 * Pre-save middleware
 */
khachHangSchema.pre('save', function(next) {
  this.ngayCapNhat = Date.now();
  next();
});

module.exports = mongoose.model('KhachHang', khachHangSchema);
