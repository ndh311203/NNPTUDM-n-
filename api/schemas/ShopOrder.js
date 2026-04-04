const mongoose = require('mongoose');

const donHangSchema = new mongoose.Schema({
    khachHangId: { type: mongoose.Schema.Types.ObjectId, ref: 'KhachHang', required: true },
    items: [{
        sanPhamId: { type: mongoose.Schema.Types.ObjectId, ref: 'SanPham', required: true },
        soLuong: { type: Number, required: true, min: 1 },
        donGiaLieu: { type: Number, required: true }
    }],
    tongTien: { type: Number, required: true },
    voucherSuDung: { type: mongoose.Schema.Types.ObjectId, ref: 'Voucher' },
    trangThaiThanhToan: { type: String, enum: ['CHUA_THANH_TOAN', 'DA_THANH_TOAN', 'HOAN_TIEN'], default: 'CHUA_THANH_TOAN' },
    trangThaiGiaoHang: { type: String, enum: ['DANG_XU_LY', 'DANG_GIAO', 'HOAN_THANH', 'DA_HUY'], default: 'DANG_XU_LY' },
    diaChiGiaoHang: { type: String }
}, { timestamps: true });

module.exports = mongoose.model('ShopOrder', donHangSchema);
