const PhongKhachSan = require("../models/PhongKhachSan");

exports.getAllRooms = async (req, res) => {
  try {
    const { trangThai } = req.query;
    const filter = trangThai ? { trangThai } : {};
    const rooms = await PhongKhachSan.find(filter).sort({ createdAt: -1 });
    res.status(200).json({ success: true, data: rooms });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.getRoomById = async (req, res) => {
  try {
    const room = await PhongKhachSan.findById(req.params.id);
    if (!room)
      return res.status(404).json({ success: false, message: "Không tìm thấy phòng" });
    res.status(200).json({ success: true, data: room });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.createRoom = async (req, res) => {
  try {
    const newRoom = new PhongKhachSan(req.body);
    await newRoom.save();
    res.status(201).json({ success: true, message: "Tạo phòng thành công", data: newRoom });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.updateRoom = async (req, res) => {
  try {
    const room = await PhongKhachSan.findByIdAndUpdate(req.params.id, req.body, {
      new: true,
      runValidators: true,
    });
    if (!room)
      return res.status(404).json({ success: false, message: "Không tìm thấy phòng" });
    res.status(200).json({ success: true, message: "Cập nhật phòng thành công", data: room });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.deleteRoom = async (req, res) => {
  try {
    const room = await PhongKhachSan.findByIdAndDelete(req.params.id);
    if (!room)
      return res.status(404).json({ success: false, message: "Không tìm thấy phòng" });
    res.status(200).json({ success: true, message: "Xóa phòng thành công" });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};
