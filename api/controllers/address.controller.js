const DiaChi = require("../models/DiaChi");

exports.getAllAddresses = async (req, res) => {
  try {
    const { khachHangId } = req.query;
    const filter = khachHangId ? { khachHangId } : {};
    const addresses = await DiaChi.find(filter).sort({ macDinh: -1, createdAt: -1 });
    res.status(200).json({ success: true, data: addresses });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.getAddressById = async (req, res) => {
  try {
    const address = await DiaChi.findById(req.params.id);
    if (!address)
      return res.status(404).json({ success: false, message: "Địa chỉ không tồn tại" });
    res.status(200).json({ success: true, data: address });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.createAddress = async (req, res) => {
  try {
    const newAddress = new DiaChi(req.body);
    await newAddress.save();
    res.status(201).json({ success: true, message: "Thêm địa chỉ thành công", data: newAddress });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.updateAddress = async (req, res) => {
  try {
    const address = await DiaChi.findByIdAndUpdate(req.params.id, req.body, {
      new: true,
      runValidators: true,
    });
    if (!address)
      return res.status(404).json({ success: false, message: "Địa chỉ không tồn tại" });
    res.status(200).json({ success: true, message: "Cập nhật địa chỉ thành công", data: address });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.deleteAddress = async (req, res) => {
  try {
    const address = await DiaChi.findByIdAndDelete(req.params.id);
    if (!address)
      return res.status(404).json({ success: false, message: "Địa chỉ không tồn tại" });
    res.status(200).json({ success: true, message: "Xóa địa chỉ thành công" });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.setDefaultAddress = async (req, res) => {
  try {
    const address = await DiaChi.findById(req.params.id);
    if (!address)
      return res.status(404).json({ success: false, message: "Địa chỉ không tồn tại" });

    await DiaChi.updateMany(
      { khachHangId: address.khachHangId, _id: { $ne: address._id } },
      { macDinh: false },
    );
    address.macDinh = true;
    await address.save();

    res.status(200).json({ success: true, message: "Đặt địa chỉ mặc định thành công", data: address });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};
