const mongoose = require("mongoose");

const diaChiSchema = new mongoose.Schema(
  {
    khachHangId: {
      type: mongoose.Schema.Types.ObjectId,
      ref: "KhachHang",
      required: [true, "ID khách hàng là bắt buộc"],
    },
    hoTen: {
      type: String,
      required: [true, "Họ tên người nhận là bắt buộc"],
      trim: true,
      maxlength: [100, "Họ tên không quá 100 ký tự"],
    },
    soDienThoai: {
      type: String,
      required: [true, "Số điện thoại là bắt buộc"],
      match: [/^(\+84|0)[0-9]{9,10}$/, "Số điện thoại không hợp lệ"],
    },
    tinhThanh: {
      type: String,
      required: [true, "Tỉnh/Thành phố là bắt buộc"],
    },
    quanHuyen: {
      type: String,
      required: [true, "Quận/Huyện là bắt buộc"],
    },
    phuongXa: {
      type: String,
    },
    diaChiCuThe: {
      type: String,
      required: [true, "Địa chỉ cụ thể là bắt buộc"],
      maxlength: [300, "Địa chỉ không quá 300 ký tự"],
    },
    loaiDiaChi: {
      type: String,
      enum: ["NHA_RIENG", "CO_QUAN", "KHAC"],
      default: "NHA_RIENG",
    },
    macDinh: {
      type: Boolean,
      default: false,
    },
    ghiChu: {
      type: String,
      maxlength: [500, "Ghi chú không quá 500 ký tự"],
    },
  },
  { timestamps: true },
);

diaChiSchema.index({ khachHangId: 1 });
diaChiSchema.index({ macDinh: 1 });

diaChiSchema.pre("save", async function (next) {
  if (this.macDinh) {
    await this.constructor.updateMany(
      { khachHangId: this.khachHangId, _id: { $ne: this._id } },
      { macDinh: false },
    );
  }
  next();
});

module.exports = mongoose.model("DiaChi", diaChiSchema);
