const mongoose = require("mongoose");

const danhGiaSchema = new mongoose.Schema(
  {
    khachHangId: {
      type: mongoose.Schema.Types.ObjectId,
      ref: "KhachHang",
      required: true,
    },
    // Đánh giá cho sản phẩm HOẶC dịch vụ (1 trong 2)
    sanPhamId: { type: mongoose.Schema.Types.ObjectId, ref: "SanPham", default: null },
    dichVuId: { type: mongoose.Schema.Types.ObjectId, ref: "DichVu", default: null },
    soSao: { type: Number, required: true, min: 1, max: 5 },
    noiDung: { type: String, maxlength: 1000 },
    hinhAnh: [{ type: String }],
    trangThai: { type: Boolean, default: true },
  },
  { timestamps: true }
);

module.exports = mongoose.model("DanhGia", danhGiaSchema);
