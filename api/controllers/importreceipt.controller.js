const PhieuNhap = require("../models/PhieuNhap");
const SanPham = require("../models/SanPham");
const mongoose = require("mongoose");
const { transactionUnsupported } = require("../utils/mongoTransaction");

exports.getAllImportReceipts = async (req, res) => {
  try {
    const { trangThai } = req.query;
    const filter = trangThai ? { trangThai } : {};

    const receipts = await PhieuNhap.find(filter)
      .populate("nguoiTao", "hoTen email")
      .sort({ createdAt: -1 });
    res.status(200).json({ success: true, data: receipts });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.getImportReceiptById = async (req, res) => {
  try {
    const receipt = await PhieuNhap.findById(req.params.id)
      .populate("nguoiTao", "hoTen email");
    if (!receipt)
      return res.status(404).json({ success: false, message: "Phiếu nhập không tồn tại" });
    res.status(200).json({ success: true, data: receipt });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

function tongTienFromItems(items) {
  if (!items || !items.length) return 0;
  return items.reduce(
    (sum, item) =>
      sum + (Number(item.soLuong) || 0) * (Number(item.donGiaNhap) || 0),
    0,
  );
}

/** @param {import('mongoose').ClientSession | null} session */
async function persistImportReceipt(body, session) {
  const { maPhieu, nhaCungCap, nguoiTao, items, ghiChu } = body;
  const tongTien =
    body.tongTien != null && body.tongTien !== ""
      ? Number(body.tongTien)
      : tongTienFromItems(items);

  for (const item of items) {
    if (item.sanPhamId) {
      const q = SanPham.findById(item.sanPhamId);
      const sanPham = session ? await q.session(session) : await q;
      if (sanPham) {
        sanPham.soLuongTon += item.soLuong;
        if (session) await sanPham.save({ session });
        else await sanPham.save();
      }
    }
  }

  const newReceipt = new PhieuNhap({
    maPhieu,
    nhaCungCap,
    nguoiTao,
    items,
    tongTien,
    ghiChu,
    trangThai: "DA_NHAP",
  });
  if (session) await newReceipt.save({ session });
  else await newReceipt.save();
  return newReceipt;
}

exports.createImportReceipt = async (req, res) => {
  const { items } = req.body;
  if (!items || items.length === 0) {
    return res.status(400).json({ success: false, message: "Danh sách sản phẩm không được trống" });
  }

  const session = await mongoose.startSession();
  try {
    session.startTransaction();
    const newReceipt = await persistImportReceipt(req.body, session);
    await session.commitTransaction();
    return res.status(201).json({ success: true, message: "Tạo phiếu nhập thành công", data: newReceipt });
  } catch (error) {
    await session.abortTransaction().catch(() => {});
    if (error.code === 11000) {
      await session.endSession().catch(() => {});
      return res.status(400).json({ success: false, message: "Mã phiếu nhập đã tồn tại" });
    }
    if (transactionUnsupported(error)) {
      try {
        const newReceipt = await persistImportReceipt(req.body, null);
        return res.status(201).json({ success: true, message: "Tạo phiếu nhập thành công", data: newReceipt });
      } catch (e) {
        if (e.code === 11000) {
          return res.status(400).json({ success: false, message: "Mã phiếu nhập đã tồn tại" });
        }
        return res.status(500).json({ success: false, message: e.message });
      }
    }
    return res.status(500).json({ success: false, message: error.message });
  } finally {
    session.endSession();
  }
};

exports.updateImportReceipt = async (req, res) => {
  try {
    const receipt = await PhieuNhap.findByIdAndUpdate(req.params.id, req.body, {
      new: true,
      runValidators: true,
    });
    if (!receipt)
      return res.status(404).json({ success: false, message: "Phiếu nhập không tồn tại" });
    res.status(200).json({ success: true, message: "Cập nhật phiếu nhập thành công", data: receipt });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.deleteImportReceipt = async (req, res) => {
  try {
    const receipt = await PhieuNhap.findByIdAndDelete(req.params.id);
    if (!receipt)
      return res.status(404).json({ success: false, message: "Phiếu nhập không tồn tại" });
    res.status(200).json({ success: true, message: "Xóa phiếu nhập thành công" });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.updateReceiptStatus = async (req, res) => {
  try {
    const { trangThai } = req.body;
    const receipt = await PhieuNhap.findByIdAndUpdate(
      req.params.id,
      { trangThai },
      { new: true, runValidators: true },
    );
    if (!receipt)
      return res.status(404).json({ success: false, message: "Phiếu nhập không tồn tại" });
    res.status(200).json({ success: true, message: "Cập nhật trạng thái phiếu nhập thành công", data: receipt });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};
