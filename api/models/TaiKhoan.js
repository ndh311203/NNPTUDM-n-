const mongoose = require("mongoose");

const taiKhoanSchema = new mongoose.Schema(
  {
    email: {
      type: String,
      required: [true, "Email is required"],
      unique: true,
      lowercase: true,
      trim: true,
      match: [
        /^\w+([\\.-]?\w+)*@\w+([\\.-]?\w+)*(\.\w{2,3})+$/,
        "Please provide a valid email",
      ],
    },

    matKhau: {
      type: String,
      required: [true, "Password is required"],
      minlength: 6,
      select: false,
    },

    hoTen: {
      type: String,
      trim: true,
      maxlength: [100, "Name cannot exceed 100 characters"],
    },

    vaiTro: {
      type: String,
      enum: {
        values: ["user", "staff", "admin"],
        message: "Role must be user, staff, or admin",
      },
      default: "user",
    },

    trangThai: {
      type: Boolean,
      default: true,
    },

    provider: {
      type: String,
      enum: ["facebook", "google", null],
      default: null,
    },

    providerId: {
      type: String,
      sparse: true,
    },

    ngayTao: {
      type: Date,
      default: Date.now,
    },

    ngayCapNhat: {
      type: Date,
      default: Date.now,
    },

    lastLogin: {
      type: Date,
      default: null,
    },

    emailVerified: {
      type: Boolean,
      default: false,
    },

    emailVerificationToken: {
      type: String,
      default: null,
    },

    passwordResetToken: {
      type: String,
      default: null,
    },

    passwordResetExpires: {
      type: Date,
      default: null,
    },
  },
  {
    timestamps: true,
  },
);

taiKhoanSchema.index({ email: 1 });
taiKhoanSchema.index({ providerId: 1 });
taiKhoanSchema.index({ vaiTro: 1 });

taiKhoanSchema.methods.toJSON = function () {
  const obj = this.toObject();
  delete obj.matKhau;
  delete obj.passwordResetToken;
  delete obj.emailVerificationToken;
  return obj;
};

taiKhoanSchema.pre("save", function (next) {
  this.ngayCapNhat = Date.now();
  next();
});

module.exports = mongoose.model("TaiKhoan", taiKhoanSchema);
