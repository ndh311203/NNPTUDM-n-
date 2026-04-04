const mongoose = require('mongoose');

/**
 * ThuCung Schema (Pet)
 * Represents a customer's pet
 */
const thuCungSchema = new mongoose.Schema(
  {
    // Pet Information
    tenThuCung: {
      type: String,
      required: [true, 'Pet name is required'],
      trim: true,
      maxlength: [100, 'Pet name cannot exceed 100 characters']
    },
    
    loaiThuCung: {
      type: String,
      enum: ['dog', 'cat', 'bird', 'rabbit', 'hamster', 'fish', 'other'],
      required: [true, 'Pet type is required']
    },
    
    giongVat: {
      type: String,
      maxlength: [100, 'Breed cannot exceed 100 characters']
    },
    
    mauLong: {
      type: String,
      maxlength: [100, 'Color cannot exceed 100 characters']
    },
    
    canNang: {
      type: Number, // in kg
      min: [0, 'Weight cannot be negative']
    },
    
    ngaySinh: {
      type: Date
    },
    
    gioiTinh: {
      type: String,
      enum: ['male', 'female', 'unknown'],
      default: 'unknown'
    },
    
    // Medical Info
    tieuSuBenh: {
      type: String,
      maxlength: [500, 'Medical history cannot exceed 500 characters']
    },
    
    daCat: {
      type: Boolean,
      default: false
    },
    
    soMicrochip: {
      type: String,
      sparse: true
    },
    
    // Owner Reference
    khachHangId: {
      type: mongoose.Schema.Types.ObjectId,
      ref: 'KhachHang',
      required: [true, 'Customer ID is required']
    },
    
    // Pet Photo
    anhCung: {
      type: String,
      default: null
    },
    
    // Pet Notes
    ghiChu: {
      type: String,
      maxlength: [1000, 'Notes cannot exceed 1000 characters']
    },
    
    // Status
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
    }
  },
  {
    timestamps: true
  }
);

// Indexes
thuCungSchema.index({ khachHangId: 1 });
thuCungSchema.index({ loaiThuCung: 1 });

/**
 * Calculate age of pet
 */
thuCungSchema.virtual('tuoi').get(function() {
  if (!this.ngaySinh) return null;
  
  const today = new Date();
  let age = today.getFullYear() - this.ngaySinh.getFullYear();
  const monthDiff = today.getMonth() - this.ngaySinh.getMonth();
  
  if (monthDiff < 0 || (monthDiff === 0 && today.getDate() < this.ngaySinh.getDate())) {
    age--;
  }
  
  return age;
});

/**
 * Pre-save middleware
 */
thuCungSchema.pre('save', function(next) {
  this.ngayCapNhat = Date.now();
  next();
});

// Populate customer on query
thuCungSchema.pre(/^find/, function(next) {
  if (this.options._recursed) {
    return next();
  }
  this.populate({
    path: 'khachHangId',
    select: 'hoTen soDienThoai'
  });
  next();
});

module.exports = mongoose.model('ThuCung', thuCungSchema);
