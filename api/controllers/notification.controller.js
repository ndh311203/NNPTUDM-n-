// Lưu thông báo tạm thời trong bộ nhớ (in-memory), dùng cho mục đích demo
let notifications = [];

// Lấy tất cả thông báo của một user
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

// Tạo thông báo mới và push qua Socket.IO
exports.createNotification = (req, res) => {
  try {
    const { userId, tieude, noiDung, loai } = req.body;

    if (!tieude || !noiDung) {
      return res.status(400).json({ success: false, message: "Tiêu đề và nội dung là bắt buộc" });
    }

    const newNoti = {
      _id: Date.now().toString(),
      userId: userId || "all",
      tieude,
      noiDung,
      loai: loai || "INFO",
      daDoc: false,
      createdAt: new Date(),
    };

    notifications.unshift(newNoti);

    // Gửi realtime qua Socket.IO
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

// Đánh dấu đã đọc
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

// Xóa thông báo
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
