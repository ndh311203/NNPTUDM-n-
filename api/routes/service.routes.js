const express = require('express');
const router = express.Router();
const serviceController = require('../controllers/service.controller');

/**
 * Service Routes
 * Base: /api/services
 */

router.get('/', serviceController.getAllServices);
router.post('/', serviceController.createService);
router.get('/:id', serviceController.getServiceById);
router.put('/:id', serviceController.updateService);
router.delete('/:id', serviceController.deleteService);

module.exports = router;
