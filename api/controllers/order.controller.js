const ShopOrder = require("../schemas/ShopOrder");
const SanPham = require("../schemas/SanPham");
const mongoose = require("mongoose");

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

exports.createOrder = async (req, res) => {
  const session = await mongoose.startSession();
  session.startTransaction();
  try {
    const { items, khachHangId, voucherSuDung, diaChiGiaoHang, tongTien } =
      req.body;

    for (const item of items) {
      const sanPham = await SanPham.findById(item.sanPhamId).session(session);
      if (!sanPham || sanPham.soLuongTon < item.soLuong) {
        throw new Error(
          `Sản phẩm ${sanPham ? sanPham.tenSanPham : item.sanPhamId} không đủ số lượng tồn kho`,
        );
      }
      sanPham.soLuongTon -= item.soLuong;
      await sanPham.save({ session });
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

    await newOrder.save({ session });
    await session.commitTransaction();
    session.endSession();

    res
      .status(201)
      .json({
        success: true,
        message: "Tạo đơn hàng thành công",
        data: newOrder,
      });
  } catch (error) {
    await session.abortTransaction();
    session.endSession();
    res.status(500).json({ success: false, message: error.message });
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

exports.deleteOrder = async (req, res) => {
  const session = await mongoose.startSession();
  session.startTransaction();
  try {
    const order = await ShopOrder.findById(req.params.id).session(session);
    if (!order) {
      throw new Error("Đơn hàng không tồn tại");
    }

    for (const item of order.items) {
      const sanPham = await SanPham.findById(item.sanPhamId).session(session);
      if (sanPham) {
        sanPham.soLuongTon += item.soLuong;
        await sanPham.save({ session });
      }
    }

    await ShopOrder.findByIdAndDelete(req.params.id).session(session);
    await session.commitTransaction();
    session.endSession();

    res.status(200).json({ success: true, message: "Xóa đơn hàng thành công" });
  } catch (error) {
    await session.abortTransaction();
    session.endSession();
    res.status(500).json({ success: false, message: error.message });
  }
};
