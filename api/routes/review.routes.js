const express = require("express");
const router = express.Router();
const reviewController = require("../controllers/review.controller");
const { authenticateToken, authorizeRole } = require("../utils/authHandler");

router.get("/", reviewController.getAllReviews);
router.post("/product", authenticateToken, reviewController.reviewProduct);
router.post("/service", authenticateToken, reviewController.reviewService);
router.get("/:id", reviewController.getReviewById);
router.put("/:id", authenticateToken, reviewController.updateReview);
router.delete("/:id", authenticateToken, authorizeRole("admin", "staff"), reviewController.deleteReview);

module.exports = router;
