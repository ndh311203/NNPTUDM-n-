const mongoose = require("mongoose");

const danhMucSchema = new mongoose.Schema(
  {
    tenDanhMuc: {
      type: String,
      required: [true, "Tên danh mục là bắt buộc"],
      trim: true,
      maxlength: [100, "Tên danh mục không quá 100 ký tự"],
    },
    moTa: {
      type: String,
      maxlength: [500, "Mô tả không quá 500 ký tự"],
    },
    loai: {
      type: String,
      enum: ["SAN_PHAM", "THU_CUNG"],
      default: "SAN_PHAM",
    },
    hinhAnh: {
      type: String,
    },
    trangThai: {
      type: Boolean,
      default: true,
    },
    thuTu: {
      type: Number,
      default: 0,
    },
  },
  { timestamps: true },
);

danhMucSchema.index({ tenDanhMuc: 1 });
danhMucSchema.index({ loai: 1 });

module.exports = mongoose.model("DanhMuc", danhMucSchema);
