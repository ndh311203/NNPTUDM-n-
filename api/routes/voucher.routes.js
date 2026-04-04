const express = require('express');
const router = express.Router();
const voucherController = require('../controllers/voucher.controller');

/**
 * Voucher Routes
 * Base: /api/vouchers
 */

// GET /api/vouchers - Get all vouchers
router.get('/', voucherController.getAllVouchers);

// POST /api/vouchers - Create new voucher
router.post('/', voucherController.createVoucher);

// GET /api/vouchers/:id - Get voucher details
router.get('/:id', voucherController.getVoucherById);

// PUT /api/vouchers/:id - Update voucher
router.put('/:id', voucherController.updateVoucher);

// DELETE /api/vouchers/:id - Delete voucher
router.delete('/:id', voucherController.deleteVoucher);

// POST /api/vouchers/validate - Validate voucher code
router.post('/validate', voucherController.validateVoucher);

module.exports = router;
