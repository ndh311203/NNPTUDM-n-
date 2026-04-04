const DichVu = require('../schemas/DichVu');

// GET /api/services
exports.getAllServices = async (req, res) => {
    try {
        const services = await DichVu.find();
        res.status(200).json({ success: true, data: services });
    } catch (error) {
        res.status(500).json({ success: false, message: error.message });
    }
};

// GET /api/services/:id
exports.getServiceById = async (req, res) => {
    try {
        const service = await DichVu.findById(req.params.id);
        if (!service) return res.status(404).json({ success: false, message: 'Dịch vụ không tồn tại' });
        res.status(200).json({ success: true, data: service });
    } catch (error) {
        res.status(500).json({ success: false, message: error.message });
    }
};

// POST /api/services
exports.createService = async (req, res) => {
    try {
        const newService = new DichVu(req.body);
        await newService.save();
        res.status(201).json({ success: true, message: 'Tạo dịch vụ thành công', data: newService });
    } catch (error) {
        res.status(500).json({ success: false, message: error.message });
    }
};

// PUT /api/services/:id
exports.updateService = async (req, res) => {
    try {
        const service = await DichVu.findByIdAndUpdate(req.params.id, req.body, { new: true, runValidators: true });
        if (!service) return res.status(404).json({ success: false, message: 'Dịch vụ không tồn tại' });
        res.status(200).json({ success: true, message: 'Cập nhật dịch vụ thành công', data: service });
    } catch (error) {
        res.status(500).json({ success: false, message: error.message });
    }
};

// DELETE /api/services/:id
exports.deleteService = async (req, res) => {
    try {
        const service = await DichVu.findByIdAndDelete(req.params.id);
        if (!service) return res.status(404).json({ success: false, message: 'Dịch vụ không tồn tại' });
        res.status(200).json({ success: true, message: 'Xóa dịch vụ thành công' });
    } catch (error) {
        res.status(500).json({ success: false, message: error.message });
    }
};
