const express = require("express");
const router = express.Router();
const addressController = require("../controllers/address.controller");

router.get("/", addressController.getAllAddresses);
router.get("/:id", addressController.getAddressById);
router.post("/", addressController.createAddress);
router.put("/:id", addressController.updateAddress);
router.delete("/:id", addressController.deleteAddress);
router.patch("/:id/set-default", addressController.setDefaultAddress);

module.exports = router;
