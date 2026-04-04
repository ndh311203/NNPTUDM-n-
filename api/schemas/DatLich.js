const mongoose = require('mongoose');

const datLichSchema = new mongoose.Schema({
    khachHangId: { type: mongoose.Schema.Types.ObjectId, ref: 'KhachHang', required: true },
    thuCungId: { type: mongoose.Schema.Types.ObjectId, ref: 'ThuCung' },
    dichVuId: { type: mongoose.Schema.Types.ObjectId, ref: 'DichVu', required: true },
    ngayHen: { type: Date, required: true },
    trangThai: { type: String, enum: ['CHO_XAC_NHAN', 'DA_XAC_NHAN', 'HOAN_THANH', 'DA_HUY'], default: 'CHO_XAC_NHAN' },
    ghiChu: { type: String }
}, { timestamps: true });

module.exports = mongoose.model('DatLich', datLichSchema);
