const ThanhToan = require("../models/ThanhToan");

exports.getAllPayments = async (req, res) => {
  try {
    const { donHangId, khachHangId, trangThai } = req.query;
    const filter = {};
    if (donHangId) filter.donHangId = donHangId;
    if (khachHangId) filter.khachHangId = khachHangId;
    if (trangThai) filter.trangThai = trangThai;

    const payments = await ThanhToan.find(filter)
      .populate("donHangId")
      .populate("khachHangId", "hoTen soDienThoai")
      .sort({ createdAt: -1 });
    res.status(200).json({ success: true, data: payments });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.getPaymentById = async (req, res) => {
  try {
    const payment = await ThanhToan.findById(req.params.id)
      .populate("donHangId")
      .populate("khachHangId", "hoTen soDienThoai");
    if (!payment)
      return res.status(404).json({ success: false, message: "Thanh toán không tồn tại" });
    res.status(200).json({ success: true, data: payment });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.createPayment = async (req, res) => {
  try {
    const newPayment = new ThanhToan(req.body);
    await newPayment.save();
    res.status(201).json({ success: true, message: "Tạo thanh toán thành công", data: newPayment });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.updatePayment = async (req, res) => {
  try {
    const payment = await ThanhToan.findByIdAndUpdate(req.params.id, req.body, {
      new: true,
      runValidators: true,
    });
    if (!payment)
      return res.status(404).json({ success: false, message: "Thanh toán không tồn tại" });
    res.status(200).json({ success: true, message: "Cập nhật thanh toán thành công", data: payment });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.deletePayment = async (req, res) => {
  try {
    const payment = await ThanhToan.findByIdAndDelete(req.params.id);
    if (!payment)
      return res.status(404).json({ success: false, message: "Thanh toán không tồn tại" });
    res.status(200).json({ success: true, message: "Xóa thanh toán thành công" });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.confirmPayment = async (req, res) => {
  try {
    const payment = await ThanhToan.findByIdAndUpdate(
      req.params.id,
      { trangThai: "THANH_CONG", maGiaoDich: `GD${Date.now()}` },
      { new: true, runValidators: true },
    );
    if (!payment)
      return res.status(404).json({ success: false, message: "Thanh toán không tồn tại" });
    res.status(200).json({ success: true, message: "Xác nhận thanh toán thành công", data: payment });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.refundPayment = async (req, res) => {
  try {
    const payment = await ThanhToan.findByIdAndUpdate(
      req.params.id,
      { trangThai: "HOAN_TIEN" },
      { new: true, runValidators: true },
    );
    if (!payment)
      return res.status(404).json({ success: false, message: "Thanh toán không tồn tại" });
    res.status(200).json({ success: true, message: "Hoàn tiền thành công", data: payment });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};
