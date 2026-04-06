// Lưu tin nhắn tạm thời trong bộ nhớ (in-memory)
let messages = [];
let messageIdCounter = 1;

// Lấy lịch sử tin nhắn (có thể lọc theo phòng)
exports.getMessages = (req, res) => {
  try {
    const { room } = req.query;
    const result = room
      ? messages.filter(m => m.room === room)
      : messages;

    res.status(200).json({ success: true, data: result });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

// Gửi tin nhắn mới và phát qua Socket.IO
exports.sendMessage = (req, res) => {
  try {
    const { nguoiGuiId, hoTen, noiDung, room } = req.body;

    if (!noiDung) {
      return res.status(400).json({ success: false, message: "Nội dung tin nhắn không được để trống" });
    }

    const newMsg = {
      _id: (messageIdCounter++).toString(),
      nguoiGuiId: nguoiGuiId || "guest",
      hoTen: hoTen || "Khách",
      noiDung,
      room: room || "general",
      createdAt: new Date(),
    };

    messages.push(newMsg);
    // Giữ tối đa 200 tin nhắn
    if (messages.length > 200) messages.shift();

    // Phát realtime qua Socket.IO
    const io = req.app.get("io");
    if (io) {
      io.to(newMsg.room).emit("new_message", newMsg);
    }

    res.status(201).json({ success: true, data: newMsg });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

// Lấy danh sách các phòng đang có tin nhắn
exports.getConversations = (req, res) => {
  try {
    const rooms = [...new Set(messages.map(m => m.room))];
    const conversations = rooms.map(room => {
      const roomMessages = messages.filter(m => m.room === room);
      const lastMsg = roomMessages[roomMessages.length - 1];
      return { room, soTinNhan: roomMessages.length, tinNhanCuoi: lastMsg };
    });
    res.status(200).json({ success: true, data: conversations });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

// Cập nhật tin nhắn
exports.updateMessage = (req, res) => {
  try {
    const msg = messages.find(m => m._id === req.params.id);
    if (!msg) {
      return res.status(404).json({ success: false, message: "Không tìm thấy tin nhắn" });
    }
    msg.noiDung = req.body.noiDung || msg.noiDung;
    msg.daChinhSua = true;
    res.status(200).json({ success: true, message: "Đã cập nhật", data: msg });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

// Xóa tin nhắn
exports.deleteMessage = (req, res) => {
  try {
    const index = messages.findIndex(m => m._id === req.params.id);
    if (index === -1) {
      return res.status(404).json({ success: false, message: "Không tìm thấy tin nhắn" });
    }
    messages.splice(index, 1);
    res.status(200).json({ success: true, message: "Đã xóa tin nhắn" });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};
