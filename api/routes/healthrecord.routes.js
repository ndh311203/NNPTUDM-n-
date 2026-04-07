const express = require("express");
const router = express.Router();
const healthrecordController = require("../controllers/healthrecord.controller");

router.get("/", healthrecordController.getAllHealthRecords);
router.get("/pet/:petId", healthrecordController.getHealthHistoryByPet);
router.get("/:id", healthrecordController.getHealthRecordById);
router.post("/", healthrecordController.createHealthRecord);
router.put("/:id", healthrecordController.updateHealthRecord);
router.delete("/:id", healthrecordController.deleteHealthRecord);

module.exports = router;
