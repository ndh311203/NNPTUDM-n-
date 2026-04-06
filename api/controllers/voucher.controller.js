const Voucher = require("../schemas/Voucher");
const { validationResult } = require("express-validator");

exports.getAllVouchers = async (req, res) => {
  try {
    const vouchers = await Voucher.find({
      trangThai: true,
      ngayKetThuc: { $gte: new Date() },
    }).sort({ ngayTao: -1 });

    res.status(200).json({
      success: true,
      data: vouchers,
    });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.createVoucher = async (req, res) => {
  try {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
      return res.status(400).json({ success: false, errors: errors.array() });
    }

    const newVoucher = new Voucher(req.body);
    await newVoucher.save();

    res.status(201).json({
      success: true,
      message: "Tạo voucher thành công",
      data: newVoucher,
    });
  } catch (error) {
    if (error.code === 11000) {
      return res
        .status(400)
        .json({ success: false, message: "Mã voucher đã tồn tại" });
    }
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.getVoucherById = async (req, res) => {
  try {
    const voucher = await Voucher.findById(req.params.id);
    if (!voucher) {
      return res
        .status(404)
        .json({ success: false, message: "Không tìm thấy voucher" });
    }
    res.status(200).json({ success: true, data: voucher });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.updateVoucher = async (req, res) => {
  try {
    const errors = validationResult(req);
    if (!errors.isEmpty()) {
      return res.status(400).json({ success: false, errors: errors.array() });
    }

    const voucher = await Voucher.findByIdAndUpdate(req.params.id, req.body, {
      new: true,
      runValidators: true,
    });

    if (!voucher) {
      return res
        .status(404)
        .json({ success: false, message: "Không tìm thấy voucher" });
    }

    res.status(200).json({
      success: true,
      message: "Cập nhật voucher thành công",
      data: voucher,
    });
  } catch (error) {
    if (error.code === 11000) {
      return res
        .status(400)
        .json({ success: false, message: "Mã voucher đã được sử dụng" });
    }
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.deleteVoucher = async (req, res) => {
  try {
    const voucher = await Voucher.findByIdAndDelete(req.params.id);
    if (!voucher) {
      return res
        .status(404)
        .json({ success: false, message: "Không tìm thấy voucher" });
    }
    res.status(200).json({ success: true, message: "Xóa voucher thành công" });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.validateVoucher = async (req, res) => {
  try {
    const { code, orderValue } = req.body;

    if (!code) {
      return res
        .status(400)
        .json({ success: false, message: "Vui lòng cung cấp mã voucher" });
    }

    const voucher = await Voucher.findOne({ mienGiamCode: code.toUpperCase() });

    if (!voucher) {
      return res
        .status(404)
        .json({ success: false, message: "Voucher không tồn tại" });
    }

    if (!voucher.trangThai) {
      return res
        .status(400)
        .json({ success: false, message: "Voucher đã bị vô hiệu hóa" });
    }

    const now = new Date();
    if (voucher.ngayBatDau > now) {
      return res
        .status(400)
        .json({
          success: false,
          message: "Voucher chưa tới thời gian sử dụng",
        });
    }

    if (voucher.ngayKetThuc < now) {
      return res
        .status(400)
        .json({ success: false, message: "Voucher đã hết hạn" });
    }

    if (voucher.soLuongDaDung >= voucher.soLuongToiDa) {
      return res
        .status(400)
        .json({ success: false, message: "Voucher đã hết lượt sử dụng" });
    }

    if (
      orderValue !== undefined &&
      voucher.giaTriDonHangToiThieu > orderValue
    ) {
      return res.status(400).json({
        success: false,
        message: `Đơn hàng phải từ ${voucher.giaTriDonHangToiThieu}đ để áp dụng voucher này`,
      });
    }

    let discountAmount = 0;
    if (voucher.loaiGiamGia === "PHAN_TRAM") {
      discountAmount = (orderValue * voucher.giaTriGiam) / 100;
      if (voucher.giamToiDa && discountAmount > voucher.giamToiDa) {
        discountAmount = voucher.giamToiDa;
      }
    } else {
      discountAmount = voucher.giaTriGiam;
    }

    res.status(200).json({
      success: true,
      message: "Voucher hợp lệ",
      data: {
        voucher: voucher,
        discountAmount: discountAmount,
      },
    });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};
