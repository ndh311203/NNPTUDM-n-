const mongoose = require("mongoose");

const thanhToanSchema = new mongoose.Schema(
  {
    donHangId: {
      type: mongoose.Schema.Types.ObjectId,
      ref: "ShopOrder",
    },
    khachHangId: {
      type: mongoose.Schema.Types.ObjectId,
      ref: "KhachHang",
    },
    soTien: {
      type: Number,
      required: [true, "Số tiền thanh toán là bắt buộc"],
      min: [0, "Số tiền không hợp lệ"],
    },
    phuongThuc: {
      type: String,
      enum: ["TIEN_MAT", "CHUYEN_KHOAN", "MOMO", "ZALO_PAY", "VNPAY"],
      default: "TIEN_MAT",
    },
    trangThai: {
      type: String,
      enum: ["CHO_XU_LY", "THANH_CONG", "THAT_BAI", "HOAN_TIEN"],
      default: "CHO_XU_LY",
    },
    maGiaoDich: {
      type: String,
    },
    ghiChu: {
      type: String,
      maxlength: [500, "Ghi chú không quá 500 ký tự"],
    },
  },
  { timestamps: true },
);

thanhToanSchema.index({ donHangId: 1 });
thanhToanSchema.index({ khachHangId: 1 });
thanhToanSchema.index({ trangThai: 1 });

module.exports = mongoose.model("ThanhToan", thanhToanSchema);
