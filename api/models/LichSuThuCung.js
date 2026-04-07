const mongoose = require("mongoose");

const lichSuThuCungSchema = new mongoose.Schema(
  {
    thuCungId: {
      type: mongoose.Schema.Types.ObjectId,
      ref: "ThuCung",
      required: [true, "ID thú cưng là bắt buộc"],
    },
    khachHangId: {
      type: mongoose.Schema.Types.ObjectId,
      ref: "KhachHang",
    },
    loaiLichSu: {
      type: String,
      enum: ["TIEM_PHONG", "KHAM_BENH", "TUYET_LONG", "TAM", "KHAC"],
      required: [true, "Loại lịch sử là bắt buộc"],
    },
    ngayThucHien: {
      type: Date,
      required: [true, "Ngày thực hiện là bắt buộc"],
      default: Date.now,
    },
    moTa: {
      type: String,
      maxlength: [1000, "Mô tả không quá 1000 ký tự"],
    },
    canNang: {
      type: Number,
      min: [0, "Cân nặng không hợp lệ"],
    },
    nhietDo: {
      type: Number,
    },
    chanDoan: {
      type: String,
      maxlength: [500, "Chẩn đoán không quá 500 ký tự"],
    },
    donViThuoc: {
      type: String,
      maxlength: [200, "Đơn thuốc không quá 200 ký tự"],
    },
    chiPhi: {
      type: Number,
      min: 0,
      default: 0,
    },
    nhanVienThucHien: {
      type: String,
      maxlength: [100, "Tên nhân viên không quá 100 ký tự"],
    },
    lichHenTiepTheo: {
      type: Date,
    },
    ghiChu: {
      type: String,
      maxlength: [1000, "Ghi chú không quá 1000 ký tự"],
    },
  },
  { timestamps: true },
);

lichSuThuCungSchema.index({ thuCungId: 1 });
lichSuThuCungSchema.index({ loaiLichSu: 1 });
lichSuThuCungSchema.index({ ngayThucHien: -1 });

module.exports = mongoose.model("LichSuThuCung", lichSuThuCungSchema);
