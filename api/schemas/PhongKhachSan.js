const mongoose = require("mongoose");

// Schema phòng khách sạn thú cưng
const phongKhachSanSchema = new mongoose.Schema(
  {
    tenPhong: { type: String, required: true },
    loaiPhong: {
      type: String,
      enum: ["STANDARD", "DELUXE", "VIP"],
      default: "STANDARD",
    },
    sucChua: { type: Number, default: 1 }, // số thú cưng tối đa
    giaTheoNgay: { type: Number, required: true, min: 0 },
    moTa: { type: String },
    hinhAnh: [{ type: String }],
    trangThai: {
      type: String,
      enum: ["TRONG", "DA_DAT", "BAO_TRI"],
      default: "TRONG",
    },
  },
  { timestamps: true }
);

module.exports = mongoose.model("PhongKhachSan", phongKhachSanSchema);
