const mongoose = require('mongoose');

const voucherSchema = new mongoose.Schema({
    mienGiamCode: {
        type: String,
        required: [true, 'Mã giảm giá không được để trống'],
        unique: true,
        trim: true,
        uppercase: true,
        minlength: [4, 'Mã giảm giá phải có ít nhất 4 ký tự'],
        maxlength: [20, 'Mã giảm giá không được vượt quá 20 ký tự']
    },
    tenVoucher: {
        type: String,
        required: [true, 'Tên voucher không được để trống']
    },
    moTa: {
        type: String
    },
    loaiGiamGia: {
        type: String,
        enum: ['PHAN_TRAM', 'SO_TIEN'],
        required: true,
        default: 'PHAN_TRAM'
    },
    giaTriGiam: {
        type: Number,
        required: [true, 'Giá trị giảm không được để trống'],
        min: [0, 'Giá trị giảm không hợp lệ']
    },
    giamToiDa: {
        type: Number, // Dùng cho loại PHẦN_TRĂM
        default: 0
    },
    giaTriDonHangToiThieu: {
        type: Number,
        default: 0
    },
    soLuongToiDa: {
        type: Number,
        required: true,
        min: [1, 'Số lượng tối đa phải lớn hơn 0']
    },
    soLuongDaDung: {
        type: Number,
        default: 0
    },
    ngayBatDau: {
        type: Date,
        required: true,
        default: Date.now
    },
    ngayKetThuc: {
        type: Date,
        required: true
    },
    trangThai: {
        type: Boolean,
        default: true
    }
}, {
    timestamps: true
});

// Virtual field for checking validity based on current date
voucherSchema.virtual('isValid').get(function() {
    const now = new Date();
    return this.trangThai && 
           this.ngayBatDau <= now && 
           this.ngayKetThuc >= now && 
           this.soLuongDaDung < this.soLuongToiDa;
});

// Prevent saving voucher where end date is before start date
voucherSchema.pre('save', function(next) {
    if (this.ngayKetThuc <= this.ngayBatDau) {
        next(new Error('Ngày kết thúc phải diễn ra sau ngày bắt đầu'));
    } else {
        next();
    }
});

module.exports = mongoose.model('Voucher', voucherSchema);
