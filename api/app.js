const express = require("express");
const cors = require("cors");
const helmet = require("helmet");
const compression = require("compression");
const morgan = require("morgan");
require("dotenv").config();

const app = express();

app.use(helmet());

app.use(compression());

app.use(
  cors({
    origin: (origin, callback) => {
      const allowed = !origin || origin === "null" || /^http:\/\/(localhost|127\.0\.0\.1)(:\d+)?$/.test(origin);
      if (allowed) return callback(null, true);
      return callback(null, false);
    },
    credentials: true,
    optionsSuccessStatus: 200,
    methods: ["GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS"],
    allowedHeaders: ["Content-Type", "Authorization"],
  }),
);

if (process.env.NODE_ENV === "development") {
  app.use(morgan("dev"));
} else {
  app.use(morgan("combined"));
}

app.use(express.json({ limit: "50mb" }));
app.use(express.urlencoded({ limit: "50mb", extended: true }));

app.use("/uploads", express.static("uploads"));

const { connectDB } = require("./utils/database");
connectDB();

app.get("/health", (req, res) => {
  res.status(200).json({
    status: "OK",
    message: "SPATC Pet Shop API is running",
    timestamp: new Date().toISOString(),
    environment: process.env.NODE_ENV || "development",
  });
});

app.use("/api/auth", require("./routes/auth.routes"));

app.use("/api/accounts", require("./routes/account.routes"));

app.use("/api/pets", require("./routes/pet.routes"));

app.use("/api/services", require("./routes/service.routes"));

app.use("/api/grooming", require("./routes/grooming.routes"));

app.use("/api/hotel", require("./routes/hotel.routes"));

app.use("/api/bookings", require("./routes/booking.routes"));

app.use("/api/products", require("./routes/product.routes"));

app.use("/api/shop-orders", require("./routes/shopOrder.routes"));

app.use("/api/cart", require("./routes/cart.routes"));

app.use("/api/reviews", require("./routes/review.routes"));

app.use("/api/vouchers", require("./routes/voucher.routes"));

app.use("/api/notifications", require("./routes/notification.routes"));

app.use("/api/chat", require("./routes/chat.routes"));

app.use("/api/upload", require("./routes/upload.routes"));

app.use("/api/categories", require("./routes/category.routes"));

app.use("/api/service-types", require("./routes/servicetype.routes"));

app.use("/api/payments", require("./routes/payment.routes"));

app.use("/api/addresses", require("./routes/address.routes"));

app.use("/api/grooming-rooms", require("./routes/groomingroom.routes"));

app.use("/api/health-records", require("./routes/healthrecord.routes"));

app.use("/api/import-receipts", require("./routes/importreceipt.routes"));

app.use("*", (req, res) => {
  res.status(404).json({
    success: false,
    message: "Route not found",
    path: req.originalUrl,
  });
});

app.use((err, req, res, next) => {
  console.error("Error:", err);

  const status = err.status || err.statusCode || 500;
  const message = err.message || "Internal Server Error";

  res.status(status).json({
    success: false,
    status,
    message,
    ...(process.env.NODE_ENV === "development" && { stack: err.stack }),
  });
});

module.exports = app;
