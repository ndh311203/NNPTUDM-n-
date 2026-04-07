const DanhGia = require("../models/DanhGia");

exports.getAllReviews = async (req, res) => {
  try {
    const reviews = await DanhGia.find({ trangThai: true })
      .populate("khachHangId", "hoTen")
      .populate("sanPhamId", "tenSanPham")
      .populate("dichVuId", "tenDichVu")
      .sort({ createdAt: -1 });
    res.status(200).json({ success: true, data: reviews });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.getReviewById = async (req, res) => {
  try {
    const review = await DanhGia.findById(req.params.id)
      .populate("khachHangId", "hoTen")
      .populate("sanPhamId", "tenSanPham")
      .populate("dichVuId", "tenDichVu");
    if (!review)
      return res.status(404).json({ success: false, message: "Không tìm thấy đánh giá" });
    res.status(200).json({ success: true, data: review });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.reviewProduct = async (req, res) => {
  try {
    const { khachHangId, sanPhamId, soSao, noiDung } = req.body;
    if (!khachHangId || !sanPhamId || !soSao) {
      return res.status(400).json({ success: false, message: "Thiếu thông tin đánh giá (khachHangId, sanPhamId, soSao)" });
    }
    const newReview = new DanhGia({ khachHangId, sanPhamId, soSao, noiDung });
    await newReview.save();
    res.status(201).json({ success: true, message: "Đánh giá sản phẩm thành công", data: newReview });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.reviewService = async (req, res) => {
  try {
    const { khachHangId, dichVuId, soSao, noiDung } = req.body;
    if (!khachHangId || !dichVuId || !soSao) {
      return res.status(400).json({ success: false, message: "Thiếu thông tin đánh giá (khachHangId, dichVuId, soSao)" });
    }
    const newReview = new DanhGia({ khachHangId, dichVuId, soSao, noiDung });
    await newReview.save();
    res.status(201).json({ success: true, message: "Đánh giá dịch vụ thành công", data: newReview });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.updateReview = async (req, res) => {
  try {
    const review = await DanhGia.findByIdAndUpdate(req.params.id, req.body, {
      new: true,
      runValidators: true,
    });
    if (!review)
      return res.status(404).json({ success: false, message: "Không tìm thấy đánh giá" });
    res.status(200).json({ success: true, message: "Cập nhật đánh giá thành công", data: review });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.deleteReview = async (req, res) => {
  try {
    const review = await DanhGia.findByIdAndUpdate(
      req.params.id,
      { trangThai: false },
      { new: true }
    );
    if (!review)
      return res.status(404).json({ success: false, message: "Không tìm thấy đánh giá" });
    res.status(200).json({ success: true, message: "Đã xóa đánh giá" });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};
