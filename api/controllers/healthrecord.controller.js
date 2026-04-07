const LichSuThuCung = require("../models/LichSuThuCung");

exports.getAllHealthRecords = async (req, res) => {
  try {
    const { thuCungId, loaiLichSu } = req.query;
    const filter = {};
    if (thuCungId) filter.thuCungId = thuCungId;
    if (loaiLichSu) filter.loaiLichSu = loaiLichSu;

    const records = await LichSuThuCung.find(filter)
      .populate("thuCungId", "tenThuCung loaiThuCung giongVat")
      .populate("khachHangId", "hoTen soDienThoai")
      .sort({ ngayThucHien: -1 });
    res.status(200).json({ success: true, data: records });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.getHealthRecordById = async (req, res) => {
  try {
    const record = await LichSuThuCung.findById(req.params.id)
      .populate("thuCungId", "tenThuCung loaiThuCung giongVat canNang")
      .populate("khachHangId", "hoTen soDienThoai");
    if (!record)
      return res.status(404).json({ success: false, message: "Lịch sử thú cưng không tồn tại" });
    res.status(200).json({ success: true, data: record });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.createHealthRecord = async (req, res) => {
  try {
    const newRecord = new LichSuThuCung(req.body);
    await newRecord.save();
    const populated = await LichSuThuCung.findById(newRecord._id)
      .populate("thuCungId", "tenThuCung loaiThuCung")
      .populate("khachHangId", "hoTen");
    res.status(201).json({ success: true, message: "Thêm lịch sử thú cưng thành công", data: populated });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.updateHealthRecord = async (req, res) => {
  try {
    const record = await LichSuThuCung.findByIdAndUpdate(req.params.id, req.body, {
      new: true,
      runValidators: true,
    });
    if (!record)
      return res.status(404).json({ success: false, message: "Lịch sử thú cưng không tồn tại" });
    res.status(200).json({ success: true, message: "Cập nhật lịch sử thú cưng thành công", data: record });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.deleteHealthRecord = async (req, res) => {
  try {
    const record = await LichSuThuCung.findByIdAndDelete(req.params.id);
    if (!record)
      return res.status(404).json({ success: false, message: "Lịch sử thú cưng không tồn tại" });
    res.status(200).json({ success: true, message: "Xóa lịch sử thú cưng thành công" });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.getHealthHistoryByPet = async (req, res) => {
  try {
    const records = await LichSuThuCung.find({ thuCungId: req.params.petId })
      .sort({ ngayThucHien: -1 });
    res.status(200).json({ success: true, data: records });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};
