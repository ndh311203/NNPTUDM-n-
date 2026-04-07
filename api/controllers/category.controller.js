const DanhMuc = require("../models/DanhMuc");

exports.getAllCategories = async (req, res) => {
  try {
    const { loai } = req.query;
    const filter = loai ? { loai, trangThai: true } : { trangThai: true };
    const categories = await DanhMuc.find(filter).sort({ thuTu: 1, createdAt: -1 });
    res.status(200).json({ success: true, data: categories });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.getCategoryById = async (req, res) => {
  try {
    const category = await DanhMuc.findById(req.params.id);
    if (!category)
      return res.status(404).json({ success: false, message: "Danh mục không tồn tại" });
    res.status(200).json({ success: true, data: category });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.createCategory = async (req, res) => {
  try {
    const newCategory = new DanhMuc(req.body);
    await newCategory.save();
    res.status(201).json({ success: true, message: "Tạo danh mục thành công", data: newCategory });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.updateCategory = async (req, res) => {
  try {
    const category = await DanhMuc.findByIdAndUpdate(req.params.id, req.body, {
      new: true,
      runValidators: true,
    });
    if (!category)
      return res.status(404).json({ success: false, message: "Danh mục không tồn tại" });
    res.status(200).json({ success: true, message: "Cập nhật danh mục thành công", data: category });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.deleteCategory = async (req, res) => {
  try {
    const category = await DanhMuc.findByIdAndDelete(req.params.id);
    if (!category)
      return res.status(404).json({ success: false, message: "Danh mục không tồn tại" });
    res.status(200).json({ success: true, message: "Xóa danh mục thành công" });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};
