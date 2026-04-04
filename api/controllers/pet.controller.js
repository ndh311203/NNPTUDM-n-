const ThuCung = require('../schemas/ThuCung');

// GET /api/pets
exports.getAllPets = async (req, res) => {
    try {
        const pets = await ThuCung.find().populate('khachHangId');
        res.status(200).json({ success: true, data: pets });
    } catch (error) {
        res.status(500).json({ success: false, message: error.message });
    }
};

// GET /api/pets/:id
exports.getPetById = async (req, res) => {
    try {
        const pet = await ThuCung.findById(req.params.id).populate('khachHangId');
        if (!pet) return res.status(404).json({ success: false, message: 'Thú cưng không tồn tại' });
        res.status(200).json({ success: true, data: pet });
    } catch (error) {
        res.status(500).json({ success: false, message: error.message });
    }
};

// POST /api/pets
exports.createPet = async (req, res) => {
    try {
        const newPet = new ThuCung(req.body);
        await newPet.save();
        res.status(201).json({ success: true, message: 'Thêm thú cưng thành công', data: newPet });
    } catch (error) {
        res.status(500).json({ success: false, message: error.message });
    }
};

// PUT /api/pets/:id
exports.updatePet = async (req, res) => {
    try {
        const pet = await ThuCung.findByIdAndUpdate(req.params.id, req.body, { new: true, runValidators: true });
        if (!pet) return res.status(404).json({ success: false, message: 'Thú cưng không tồn tại' });
        res.status(200).json({ success: true, message: 'Cập nhật thú cưng thành công', data: pet });
    } catch (error) {
        res.status(500).json({ success: false, message: error.message });
    }
};

// DELETE /api/pets/:id
exports.deletePet = async (req, res) => {
    try {
        const pet = await ThuCung.findByIdAndDelete(req.params.id);
        if (!pet) return res.status(404).json({ success: false, message: 'Thú cưng không tồn tại' });
        res.status(200).json({ success: true, message: 'Xóa thú cưng thành công' });
    } catch (error) {
        res.status(500).json({ success: false, message: error.message });
    }
};
