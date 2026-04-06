const mongoose = require("mongoose");

const sanPhamSchema = new mongoose.Schema(
  {
    tenSanPham: { type: String, required: true },
    phanLoai: {
      type: String,
      enum: ["THUC_AN", "DO_CHOI", "THUOC", "PHU_KIEN", "KHAC"],
      default: "KHAC",
    },
    soLuongTon: { type: Number, required: true, min: 0, default: 0 },
    donGia: { type: Number, required: true, min: 0 },
    hinhAnh: [{ type: String }],
    moTa: { type: String },
    nhaCungCap: { type: String },
    trangThai: { type: Boolean, default: true },
  },
  { timestamps: true },
);

module.exports = mongoose.model("SanPham", sanPhamSchema);
