const express = require('express');
const router = express.Router();
const orderController = require('../controllers/order.controller');

/**
 * ShopOrder Routes
 * Base: /api/shop-orders
 */

router.get('/', orderController.getAllOrders);
router.post('/', orderController.createOrder);
router.get('/:id', orderController.getOrderById);
router.put('/:id', orderController.updateOrder);
router.delete('/:id', orderController.deleteOrder);

module.exports = router;
