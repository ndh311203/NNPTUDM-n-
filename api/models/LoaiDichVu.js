const mongoose = require("mongoose");

const loaiDichVuSchema = new mongoose.Schema(
  {
    tenLoai: {
      type: String,
      required: [true, "Tên loại dịch vụ là bắt buộc"],
      trim: true,
      maxlength: [100, "Tên loại không quá 100 ký tự"],
    },
    moTa: {
      type: String,
      maxlength: [500, "Mô tả không quá 500 ký tự"],
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

loaiDichVuSchema.index({ tenLoai: 1 });

module.exports = mongoose.model("LoaiDichVu", loaiDichVuSchema);
