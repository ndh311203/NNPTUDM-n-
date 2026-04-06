const express = require("express");
const router = express.Router();
const voucherController = require("../controllers/voucher.controller");

router.get("/", voucherController.getAllVouchers);

router.post("/", voucherController.createVoucher);

router.get("/:id", voucherController.getVoucherById);

router.put("/:id", voucherController.updateVoucher);

router.delete("/:id", voucherController.deleteVoucher);

router.post("/validate", voucherController.validateVoucher);

module.exports = router;
