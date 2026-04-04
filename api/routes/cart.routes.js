const express = require('express');
const router = express.Router();
const cartController = require('../controllers/cart.controller');

/**
 * Cart Routes
 * Base: /api/cart
 */

router.get('/:userId?', cartController.getCartByUserId);
router.post('/add', cartController.addToCart);
router.delete('/:productId/:userId?', cartController.removeFromCart);
router.delete('/clear/:userId?', cartController.clearCart);

module.exports = router;
