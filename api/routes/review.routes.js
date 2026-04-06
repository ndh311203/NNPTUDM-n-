const express = require("express");
const router = express.Router();
const reviewController = require("../controllers/review.controller");
const { authenticateToken, authorizeRole } = require("../utils/authHandler");

// Lấy tất cả đánh giá
router.get("/", reviewController.getAllReviews);

// Đánh giá sản phẩm (cần đăng nhập)
router.post("/product", authenticateToken, reviewController.reviewProduct);

// Đánh giá dịch vụ (cần đăng nhập)
router.post("/service", authenticateToken, reviewController.reviewService);

// Lấy đánh giá theo ID
router.get("/:id", reviewController.getReviewById);

// Cập nhật đánh giá (cần đăng nhập)
router.put("/:id", authenticateToken, reviewController.updateReview);

// Xóa đánh giá (admin)
router.delete("/:id", authenticateToken, authorizeRole("admin", "staff"), reviewController.deleteReview);

module.exports = router;
