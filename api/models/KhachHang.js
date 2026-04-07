const mongoose = require("mongoose");

const khachHangSchema = new mongoose.Schema(
  {
    hoTen: {
      type: String,
      required: [true, "Full name is required"],
      trim: true,
      maxlength: [100, "Name cannot exceed 100 characters"],
    },

    soDienThoai: {
      type: String,
      required: [true, "Phone number is required"],
      unique: true,
      match: [/^(\+84|0)[0-9]{9,10}$/, "Invalid Vietnamese phone number"],
      trim: true,
    },

    email: {
      type: String,
      lowercase: true,
      trim: true,
      sparse: true,
    },

    diaChi: {
      type: String,
      maxlength: [500, "Address cannot exceed 500 characters"],
    },

    ghiChu: {
      type: String,
      maxlength: [1000, "Notes cannot exceed 1000 characters"],
    },

    anhDaiDien: {
      type: String,
      default: null,
    },

    taiKhoanId: {
      type: mongoose.Schema.Types.ObjectId,
      ref: "TaiKhoan",
      unique: true,
      sparse: true,
    },

    trangThai: {
      type: Boolean,
      default: true,
    },

    ngayTao: {
      type: Date,
      default: Date.now,
    },

    ngayCapNhat: {
      type: Date,
      default: Date.now,
    },

    tongDonHang: {
      type: Number,
      default: 0,
    },

    tongDichVu: {
      type: Number,
      default: 0,
    },

    tongChiTieu: {
      type: Number,
      default: 0,
    },
  },
  {
    timestamps: true,
  },
);

khachHangSchema.virtual("fullName").get(function () {
  return this.hoTen;
});

khachHangSchema.pre("save", function (next) {
  this.ngayCapNhat = Date.now();
  next();
});

module.exports = mongoose.model("KhachHang", khachHangSchema);
