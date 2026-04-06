const DatLich = require("../schemas/DatLich");

exports.getAllBookings = async (req, res) => {
  try {
    const bookings = await DatLich.find()
      .populate("khachHangId")
      .populate("thuCungId")
      .populate("dichVuId");
    res.status(200).json({ success: true, data: bookings });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.getBookingById = async (req, res) => {
  try {
    const booking = await DatLich.findById(req.params.id)
      .populate("khachHangId")
      .populate("thuCungId")
      .populate("dichVuId");
    if (!booking)
      return res
        .status(404)
        .json({ success: false, message: "Lịch hẹn không tồn tại" });
    res.status(200).json({ success: true, data: booking });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.createBooking = async (req, res) => {
  try {
    const newBooking = new DatLich(req.body);
    await newBooking.save();
    res
      .status(201)
      .json({
        success: true,
        message: "Đặt lịch thành công",
        data: newBooking,
      });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.updateBooking = async (req, res) => {
  try {
    const booking = await DatLich.findByIdAndUpdate(req.params.id, req.body, {
      new: true,
      runValidators: true,
    });
    if (!booking)
      return res
        .status(404)
        .json({ success: false, message: "Lịch hẹn không tồn tại" });
    res
      .status(200)
      .json({
        success: true,
        message: "Cập nhật lịch hẹn thành công",
        data: booking,
      });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.deleteBooking = async (req, res) => {
  try {
    const booking = await DatLich.findByIdAndDelete(req.params.id);
    if (!booking)
      return res
        .status(404)
        .json({ success: false, message: "Lịch hẹn không tồn tại" });
    res.status(200).json({ success: true, message: "Xóa lịch hẹn thành công" });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};
