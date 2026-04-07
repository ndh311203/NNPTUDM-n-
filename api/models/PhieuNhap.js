const mongoose = require("mongoose");

const phieuNhapSchema = new mongoose.Schema(
  {
    maPhieu: {
      type: String,
      required: [true, "Mã phiếu nhập là bắt buộc"],
      unique: true,
      trim: true,
    },
    nhaCungCap: {
      type: String,
      required: [true, "Nhà cung cấp là bắt buộc"],
      maxlength: [200, "Tên nhà cung cấp không quá 200 ký tự"],
    },
    nguoiTao: {
      type: mongoose.Schema.Types.ObjectId,
      ref: "TaiKhoan",
    },
    items: [
      {
        sanPhamId: {
          type: mongoose.Schema.Types.ObjectId,
          ref: "SanPham",
        },
        tenSanPham: {
          type: String,
        },
        soLuong: {
          type: Number,
          required: true,
          min: 1,
        },
        donGiaNhap: {
          type: Number,
          required: true,
          min: 0,
        },
        thanhTien: {
          type: Number,
        },
      },
    ],
    tongTien: {
      type: Number,
      required: false,
      default: 0,
      min: 0,
    },
    ghiChu: {
      type: String,
      maxlength: [1000, "Ghi chú không quá 1000 ký tự"],
    },
    trangThai: {
      type: String,
      enum: ["CHO_XAC_NHAN", "DA_NHAP", "DA_HUY"],
      default: "CHO_XAC_NHAN",
    },
  },
  { timestamps: true },
);

phieuNhapSchema.index({ trangThai: 1 });
phieuNhapSchema.index({ nguoiTao: 1 });

phieuNhapSchema.pre("save", function (next) {
  this.tongTien = this.items.reduce(
    (sum, item) => sum + (item.soLuong * item.donGiaNhap),
    0,
  );
  this.items.forEach((item) => {
    item.thanhTien = item.soLuong * item.donGiaNhap;
  });
  next();
});

module.exports = mongoose.model("PhieuNhap", phieuNhapSchema);
