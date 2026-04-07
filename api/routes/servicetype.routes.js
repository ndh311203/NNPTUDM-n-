const express = require("express");
const router = express.Router();
const servicetypeController = require("../controllers/servicetype.controller");

router.get("/", servicetypeController.getAllServiceTypes);
router.get("/:id", servicetypeController.getServiceTypeById);
router.post("/", servicetypeController.createServiceType);
router.put("/:id", servicetypeController.updateServiceType);
router.delete("/:id", servicetypeController.deleteServiceType);

module.exports = router;
