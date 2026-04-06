const SanPham = require("../schemas/SanPham");

exports.getAllProducts = async (req, res) => {
  try {
    const products = await SanPham.find();
    res.status(200).json({ success: true, data: products });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.getProductById = async (req, res) => {
  try {
    const product = await SanPham.findById(req.params.id);
    if (!product)
      return res
        .status(404)
        .json({ success: false, message: "Sản phẩm không tồn tại" });
    res.status(200).json({ success: true, data: product });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.createProduct = async (req, res) => {
  try {
    const newProduct = new SanPham(req.body);
    await newProduct.save();
    res
      .status(201)
      .json({
        success: true,
        message: "Tạo sản phẩm thành công",
        data: newProduct,
      });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.updateProduct = async (req, res) => {
  try {
    const product = await SanPham.findByIdAndUpdate(req.params.id, req.body, {
      new: true,
      runValidators: true,
    });
    if (!product)
      return res
        .status(404)
        .json({ success: false, message: "Sản phẩm không tồn tại" });
    res
      .status(200)
      .json({ success: true, message: "Cập nhật thành công", data: product });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};

exports.deleteProduct = async (req, res) => {
  try {
    const product = await SanPham.findByIdAndDelete(req.params.id);
    if (!product)
      return res
        .status(404)
        .json({ success: false, message: "Sản phẩm không tồn tại" });
    res.status(200).json({ success: true, message: "Xóa sản phẩm thành công" });
  } catch (error) {
    res.status(500).json({ success: false, message: error.message });
  }
};
