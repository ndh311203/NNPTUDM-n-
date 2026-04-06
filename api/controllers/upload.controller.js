const { getFileUrl, deleteFile } = require("../utils/uploadHandler");

exports.uploadSingleFile = (req, res) => {
  try {
    if (!req.file) {
      return res
        .status(400)
        .json({ success: false, message: "Vui lòng chọn một file đính kèm" });
    }

    const fileUrl = getFileUrl(req.file.path);

    res.status(200).json({
      success: true,
      message: "Tải file thành công",
      data: {
        fileName: req.file.filename,
        url: fileUrl,
        mimetype: req.file.mimetype,
        size: req.file.size,
      },
    });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.uploadMultipleFiles = (req, res) => {
  try {
    if (!req.files || req.files.length === 0) {
      return res
        .status(400)
        .json({ success: false, message: "Vui lòng chọn file đính kèm" });
    }

    const uploadedFiles = req.files.map((file) => ({
      fileName: file.filename,
      url: getFileUrl(file.path),
      mimetype: file.mimetype,
      size: file.size,
    }));

    res.status(200).json({
      success: true,
      message: "Tải các file thành công",
      data: uploadedFiles,
    });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.deleteUploadedFile = (req, res) => {
  try {
    const filePath = req.body.filePath;
    if (!filePath) {
      return res
        .status(400)
        .json({ success: false, message: "Đường dẫn file không hợp lệ" });
    }

    const deleted = deleteFile(filePath);
    if (!deleted) {
      return res
        .status(404)
        .json({
          success: false,
          message: "Không tìm thấy file để xóa hoặc lỗi",
        });
    }

    res.status(200).json({ success: true, message: "Đã xóa file" });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};
