const express = require('express');
const router = express.Router();
const accountController = require('../controllers/account.controller');

/**
 * Account Routes
 * Base: /api/accounts
 */

router.get('/', accountController.getAllAccounts);
router.post('/', accountController.createAccount);
router.get('/:id', accountController.getAccountById);
router.put('/:id', accountController.updateAccount);
router.delete('/:id', accountController.deleteAccount);

module.exports = router;
