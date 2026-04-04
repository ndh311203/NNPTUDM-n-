const TaiKhoan = require('../schemas/TaiKhoan');
const KhachHang = require('../schemas/KhachHang');

// GET /api/accounts
exports.getAllAccounts = async (req, res) => {
    try {
        const accounts = await TaiKhoan.find().select('-matKhau').populate('khachHangId');
        res.status(200).json({ success: true, data: accounts });
    } catch (error) {
        res.status(500).json({ success: false, message: error.message });
    }
};

// GET /api/accounts/:id
exports.getAccountById = async (req, res) => {
    try {
        const account = await TaiKhoan.findById(req.params.id).select('-matKhau').populate('khachHangId');
        if (!account) return res.status(404).json({ success: false, message: 'Không tìm thấy tài khoản' });
        res.status(200).json({ success: true, data: account });
    } catch (error) {
        res.status(500).json({ success: false, message: error.message });
    }
};

// POST /api/accounts
exports.createAccount = async (req, res) => {
    try {
        // Just standard Mongoose creation for now (for admin tasks)
        // Usually handled by auth/register
        const newAccount = new TaiKhoan(req.body);
        await newAccount.save();
        res.status(201).json({ success: true, message: 'Tạo tài khoản thành công', data: newAccount });
    } catch (error) {
        res.status(500).json({ success: false, message: error.message });
    }
};

// PUT /api/accounts/:id
exports.updateAccount = async (req, res) => {
    try {
        if(req.body.matKhau) delete req.body.matKhau; // Prevent password update via regular PUT
        const account = await TaiKhoan.findByIdAndUpdate(req.params.id, req.body, { new: true, runValidators: true }).select('-matKhau');
        if (!account) return res.status(404).json({ success: false, message: 'Không tìm thấy tài khoản' });
        res.status(200).json({ success: true, message: 'Cập nhật thành công', data: account });
    } catch (error) {
        res.status(500).json({ success: false, message: error.message });
    }
};

// DELETE /api/accounts/:id
exports.deleteAccount = async (req, res) => {
    try {
        const account = await TaiKhoan.findByIdAndDelete(req.params.id);
        if (!account) return res.status(404).json({ success: false, message: 'Không tìm thấy tài khoản' });
        res.status(200).json({ success: true, message: 'Xóa tài khoản thành công' });
    } catch (error) {
        res.status(500).json({ success: false, message: error.message });
    }
};
