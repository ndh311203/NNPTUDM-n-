const PhongGrooming = require("../models/PhongGrooming");

exports.getAllRooms = async (req, res) => {
  try {
    const { trangThai, loaiPhong } = req.query;
    const filter = {};
    if (trangThai) filter.trangThai = trangThai;
    if (loaiPhong) filter.loaiPhong = loaiPhong;

    const rooms = await PhongGrooming.find(filter).sort({ createdAt: -1 });
    res.status(200).json({ success: true, data: rooms });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.getRoomById = async (req, res) => {
  try {
    const room = await PhongGrooming.findById(req.params.id);
    if (!room)
      return res.status(404).json({ success: false, message: "Phòng grooming không tồn tại" });
    res.status(200).json({ success: true, data: room });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.createRoom = async (req, res) => {
  try {
    const newRoom = new PhongGrooming(req.body);
    await newRoom.save();
    res.status(201).json({ success: true, message: "Tạo phòng grooming thành công", data: newRoom });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.updateRoom = async (req, res) => {
  try {
    const room = await PhongGrooming.findByIdAndUpdate(req.params.id, req.body, {
      new: true,
      runValidators: true,
    });
    if (!room)
      return res.status(404).json({ success: false, message: "Phòng grooming không tồn tại" });
    res.status(200).json({ success: true, message: "Cập nhật phòng grooming thành công", data: room });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.deleteRoom = async (req, res) => {
  try {
    const room = await PhongGrooming.findByIdAndDelete(req.params.id);
    if (!room)
      return res.status(404).json({ success: false, message: "Phòng grooming không tồn tại" });
    res.status(200).json({ success: true, message: "Xóa phòng grooming thành công" });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.toggleRoomStatus = async (req, res) => {
  try {
    const { trangThai } = req.body;
    const room = await PhongGrooming.findByIdAndUpdate(
      req.params.id,
      { trangThai },
      { new: true, runValidators: true },
    );
    if (!room)
      return res.status(404).json({ success: false, message: "Phòng grooming không tồn tại" });
    res.status(200).json({ success: true, message: "Cập nhật trạng thái phòng thành công", data: room });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};
