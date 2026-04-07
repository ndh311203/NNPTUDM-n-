const express = require("express");
const router = express.Router();
const importreceiptController = require("../controllers/importreceipt.controller");

router.get("/", importreceiptController.getAllImportReceipts);
router.get("/:id", importreceiptController.getImportReceiptById);
router.post("/", importreceiptController.createImportReceipt);
router.put("/:id", importreceiptController.updateImportReceipt);
router.delete("/:id", importreceiptController.deleteImportReceipt);
router.patch("/:id/status", importreceiptController.updateReceiptStatus);

module.exports = router;
