const mongoose = require("mongoose");
require("dotenv").config();

const connectDB = async () => {
  try {
    const mongoUri =
      process.env.MONGODB_URI || "mongodb://localhost:27017/spatc";

    const conn = await mongoose.connect(mongoUri, {
      useNewUrlParser: true,
      useUnifiedTopology: true,
      serverSelectionTimeoutMS: 5000,
      retryWrites: true,
    });

    console.log(`✓ MongoDB Connected: ${conn.connection.host}`);
    console.log(`✓ Database: ${conn.connection.name}`);

    mongoose.connection.on("disconnected", () => {
      console.warn("⚠ MongoDB disconnected");
    });

    mongoose.connection.on("error", (err) => {
      console.error("✗ MongoDB connection error:", err);
    });

    mongoose.connection.on("reconnected", () => {
      console.log("✓ MongoDB reconnected");
    });

    return conn;
  } catch (error) {
    console.error("✗ MongoDB connection failed:", error.message);
    console.error("MongoDB URI:", process.env.MONGODB_URI);

    console.log("Retrying connection in 5 seconds...");
    setTimeout(connectDB, 5000);
  }
};

const disconnectDB = async () => {
  try {
    await mongoose.disconnect();
    console.log("✓ MongoDB disconnected");
  } catch (error) {
    console.error("✗ Error disconnecting from MongoDB:", error);
  }
};

const getConnection = () => {
  return mongoose.connection;
};

module.exports = {
  connectDB,
  disconnectDB,
  getConnection,
  mongoose,
};
