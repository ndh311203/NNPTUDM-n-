const mongoose = require('mongoose');

const dichVuSchema = new mongoose.Schema({
    tenDichVu: { type: String, required: true },
    moTa: { type: String },
    donGia: { type: Number, required: true, min: 0 },
    thoiGianUocTinh: { type: Number, required: true }, // Minutes
    hinhAnh: [{ type: String }],
    trangThai: { type: Boolean, default: true }
}, { timestamps: true });

module.exports = mongoose.model('DichVu', dichVuSchema);
