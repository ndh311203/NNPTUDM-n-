const mongoose = require("mongoose");

const phongGroomingSchema = new mongoose.Schema(
  {
    tenPhong: {
      type: String,
      required: [true, "Tên phòng grooming là bắt buộc"],
      trim: true,
      maxlength: [100, "Tên phòng không quá 100 ký tự"],
    },
    loaiPhong: {
      type: String,
      enum: ["GROOMING", "SPA", "TONG_HOP"],
      default: "GROOMING",
    },
    tienNghi: {
      type: [String],
      default: [],
    },
    sucChua: {
      type: Number,
      default: 2,
      min: 1,
    },
    giaTheoGio: {
      type: Number,
      min: 0,
      default: 0,
    },
    moTa: {
      type: String,
      maxlength: [500, "Mô tả không quá 500 ký tự"],
    },
    hinhAnh: {
      type: String,
    },
    trangThai: {
      type: String,
      enum: ["TRONG", "DANG_SU_DUNG", "BAO_TRI"],
      default: "TRONG",
    },
  },
  { timestamps: true },
);

phongGroomingSchema.index({ trangThai: 1 });

module.exports = mongoose.model("PhongGrooming", phongGroomingSchema);
