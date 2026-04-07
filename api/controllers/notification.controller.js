let notifications = [];

exports.getNotifications = (req, res) => {
  try {
    const userId = req.query.userId || "all";
    const userNoti = userId === "all"
      ? notifications
      : notifications.filter(n => n.userId === userId || n.userId === "all");
    res.status(200).json({ success: true, data: userNoti });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.createNotification = (req, res) => {
  try {
    const tieuDe =
      req.body.tieude || req.body.tieuDe ||
      req.body.tieu_de || req.body.title;
    const noiDung =
      req.body.noiDung || req.body.noidung || req.body.content;
    const userId =
      req.body.userId || req.body.nguoiNhanId || req.body.nguoi_nhan_id;
    const loai =
      req.body.loai || req.body.loaiThongBao || req.body.loai_thong_bao || "INFO";

    if (!tieuDe || !noiDung) {
      return res.status(400).json({ success: false, message: "Tiêu đề và nội dung là bắt buộc" });
    }

    const newNoti = {
      _id: Date.now().toString(),
      userId: userId || "all",
      tieude: tieuDe,
      noiDung,
      loai: loai || "INFO",
      daDoc: false,
      createdAt: new Date(),
    };

    notifications.unshift(newNoti);

    const io = req.app.get("io");
    if (io) {
      if (userId && userId !== "all") {
        io.to(`user_${userId}`).emit("new_notification", newNoti);
      } else {
        io.emit("new_notification", newNoti);
      }
    }

    res.status(201).json({ success: true, message: "Đã tạo thông báo", data: newNoti });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.markAsRead = (req, res) => {
  try {
    const noti = notifications.find(n => n._id === req.params.id);
    if (!noti) {
      return res.status(404).json({ success: false, message: "Không tìm thấy thông báo" });
    }
    noti.daDoc = true;
    res.status(200).json({ success: true, message: "Đã đánh dấu đọc", data: noti });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.deleteNotification = (req, res) => {
  try {
    const index = notifications.findIndex(n => n._id === req.params.id);
    if (index === -1) {
      return res.status(404).json({ success: false, message: "Không tìm thấy thông báo" });
    }
    notifications.splice(index, 1);
    res.status(200).json({ success: true, message: "Đã xóa thông báo" });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};
