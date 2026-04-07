const ShopOrder = require("../models/ShopOrder");
const SanPham = require("../models/SanPham");
const mongoose = require("mongoose");
const { transactionUnsupported } = require("../utils/mongoTransaction");

exports.getAllOrders = async (req, res) => {
  try {
    const orders = await ShopOrder.find()
      .populate("khachHangId")
      .populate("items.sanPhamId")
      .populate("voucherSuDung")
      .sort({ createdAt: -1 });
    res.status(200).json({ success: true, data: orders });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.getOrderById = async (req, res) => {
  try {
    const order = await ShopOrder.findById(req.params.id)
      .populate("khachHangId")
      .populate("items.sanPhamId")
      .populate("voucherSuDung");
    if (!order)
      return res
        .status(404)
        .json({ success: false, message: "Đơn hàng không tồn tại" });
    res.status(200).json({ success: true, data: order });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

/** @param {import('mongoose').ClientSession | null} session */
async function persistOrderFromBody(body, session) {
  const { items, khachHangId, voucherSuDung, diaChiGiaoHang, tongTien } = body;

  for (const item of items) {
    const q = SanPham.findById(item.sanPhamId);
    const sanPham = session ? await q.session(session) : await q;
    if (!sanPham || sanPham.soLuongTon < item.soLuong) {
      throw new Error(
        `Sản phẩm ${sanPham ? sanPham.tenSanPham : item.sanPhamId} không đủ số lượng tồn kho`,
      );
    }
    sanPham.soLuongTon -= item.soLuong;
    if (session) await sanPham.save({ session });
    else await sanPham.save();
  }

  const newOrder = new ShopOrder({
    khachHangId,
    items,
    tongTien,
    voucherSuDung,
    diaChiGiaoHang,
    trangThaiThanhToan: "CHUA_THANH_TOAN",
    trangThaiGiaoHang: "DANG_XU_LY",
  });
  if (session) await newOrder.save({ session });
  else await newOrder.save();
  return newOrder;
}

exports.createOrder = async (req, res) => {
  const session = await mongoose.startSession();
  try {
    session.startTransaction();
    const newOrder = await persistOrderFromBody(req.body, session);
    await session.commitTransaction();
    return res.status(201).json({
      success: true,
      message: "Tạo đơn hàng thành công",
      data: newOrder,
    });
  } catch (error) {
    await session.abortTransaction().catch(() => {});
    if (transactionUnsupported(error)) {
      try {
        const newOrder = await persistOrderFromBody(req.body, null);
        return res.status(201).json({
          success: true,
          message: "Tạo đơn hàng thành công",
          data: newOrder,
        });
      } catch (e) {
        return res.status(500).json({ success: false, message: e.message });
      }
    }
    return res.status(500).json({ success: false, message: error.message });
  } finally {
    session.endSession();
  }
};

exports.updateOrder = async (req, res) => {
  try {
    const order = await ShopOrder.findByIdAndUpdate(req.params.id, req.body, {
      new: true,
      runValidators: true,
    });
    if (!order)
      return res
        .status(404)
        .json({ success: false, message: "Đơn hàng không tồn tại" });
    res
      .status(200)
      .json({ success: true, message: "Cập nhật thành công", data: order });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

/** @param {import('mongoose').ClientSession | null} session */
async function deleteOrderById(orderId, session) {
  const q = ShopOrder.findById(orderId);
  const order = session ? await q.session(session) : await q;
  if (!order) {
    throw new Error("Đơn hàng không tồn tại");
  }

  for (const item of order.items) {
    const sq = SanPham.findById(item.sanPhamId);
    const sanPham = session ? await sq.session(session) : await sq;
    if (sanPham) {
      sanPham.soLuongTon += item.soLuong;
      if (session) await sanPham.save({ session });
      else await sanPham.save();
    }
  }

  if (session) {
    await ShopOrder.findByIdAndDelete(orderId).session(session);
  } else {
    await ShopOrder.findByIdAndDelete(orderId);
  }
}

exports.deleteOrder = async (req, res) => {
  const session = await mongoose.startSession();
  try {
    session.startTransaction();
    await deleteOrderById(req.params.id, session);
    await session.commitTransaction();
    return res.status(200).json({ success: true, message: "Xóa đơn hàng thành công" });
  } catch (error) {
    await session.abortTransaction().catch(() => {});
    if (transactionUnsupported(error)) {
      try {
        await deleteOrderById(req.params.id, null);
        return res.status(200).json({ success: true, message: "Xóa đơn hàng thành công" });
      } catch (e) {
        return res.status(500).json({ success: false, message: e.message });
      }
    }
    return res.status(500).json({ success: false, message: error.message });
  } finally {
    session.endSession();
  }
};
