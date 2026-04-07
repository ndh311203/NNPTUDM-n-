const LoaiDichVu = require("../models/LoaiDichVu");

exports.getAllServiceTypes = async (req, res) => {
  try {
    const types = await LoaiDichVu.find({ trangThai: true }).sort({ thuTu: 1, createdAt: -1 });
    res.status(200).json({ success: true, data: types });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.getServiceTypeById = async (req, res) => {
  try {
    const type = await LoaiDichVu.findById(req.params.id);
    if (!type)
      return res.status(404).json({ success: false, message: "Loại dịch vụ không tồn tại" });
    res.status(200).json({ success: true, data: type });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.createServiceType = async (req, res) => {
  try {
    const newType = new LoaiDichVu(req.body);
    await newType.save();
    res.status(201).json({ success: true, message: "Tạo loại dịch vụ thành công", data: newType });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.updateServiceType = async (req, res) => {
  try {
    const type = await LoaiDichVu.findByIdAndUpdate(req.params.id, req.body, {
      new: true,
      runValidators: true,
    });
    if (!type)
      return res.status(404).json({ success: false, message: "Loại dịch vụ không tồn tại" });
    res.status(200).json({ success: true, message: "Cập nhật loại dịch vụ thành công", data: type });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.deleteServiceType = async (req, res) => {
  try {
    const type = await LoaiDichVu.findByIdAndDelete(req.params.id);
    if (!type)
      return res.status(404).json({ success: false, message: "Loại dịch vụ không tồn tại" });
    res.status(200).json({ success: true, message: "Xóa loại dịch vụ thành công" });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};
