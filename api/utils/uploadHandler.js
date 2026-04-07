const multer = require("multer");
const path = require("path");
const fs = require("fs");
const { v4: uuidv4 } = require("uuid");

const uploadsDir = process.env.UPLOAD_PATH || "./uploads";
if (!fs.existsSync(uploadsDir)) {
  fs.mkdirSync(uploadsDir, { recursive: true });
}

const extname = (name) => path.extname(name || "").toLowerCase();

/** Postman/Windows đôi khi gửi application/octet-stream cho ảnh chụp màn hình — nhận diện theo đuôi file */
const isImageLike = (file) =>
  file.mimetype.startsWith("image/") ||
  (file.mimetype === "application/octet-stream" &&
    [".jpg", ".jpeg", ".png", ".gif", ".webp"].includes(extname(file.originalname)));

const isVideoLike = (file) =>
  file.mimetype.startsWith("video/") ||
  (file.mimetype === "application/octet-stream" &&
    [".mp4", ".mpeg", ".mov"].includes(extname(file.originalname)));

const isDocLike = (file) =>
  file.mimetype === "application/pdf" ||
  file.mimetype === "application/msword" ||
  file.mimetype ===
    "application/vnd.openxmlformats-officedocument.wordprocessingml.document" ||
  (file.mimetype === "application/octet-stream" &&
    [".pdf", ".doc", ".docx"].includes(extname(file.originalname)));

const storage = multer.diskStorage({
  destination: (req, file, cb) => {
    let folder = "other";

    if (isImageLike(file)) {
      folder = "images";
    } else if (isVideoLike(file)) {
      folder = "videos";
    } else if (file.mimetype.startsWith("audio/")) {
      folder = "audio";
    } else if (isDocLike(file)) {
      folder = "documents";
    }

    const uploadPath = path.join(uploadsDir, folder);

    if (!fs.existsSync(uploadPath)) {
      fs.mkdirSync(uploadPath, { recursive: true });
    }

    cb(null, uploadPath);
  },
  filename: (req, file, cb) => {
    const uniqueSuffix = `${Date.now()}-${uuidv4().slice(0, 8)}`;
    const ext = path.extname(file.originalname);
    const name = path.basename(file.originalname, ext);

    const filename = `${name}-${uniqueSuffix}${ext}`;
    cb(null, filename);
  },
});

const fileFilter = (req, file, cb) => {
  const allowedMimes = [
    "image/jpeg",
    "image/png",
    "image/gif",
    "image/webp",
    "image/jpg",
    "video/mp4",
    "video/mpeg",
    "video/quicktime",
    "application/pdf",
    "application/msword",
    "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
  ];

  if (allowedMimes.includes(file.mimetype)) {
    cb(null, true);
  } else if (
    file.mimetype === "application/octet-stream" &&
    (isImageLike(file) || isVideoLike(file) || isDocLike(file))
  ) {
    cb(null, true);
  } else {
    cb(new Error(`File type not supported: ${file.mimetype}`), false);
  }
};

const multerConfig = multer({
  storage: storage,
  fileFilter: fileFilter,
  limits: {
    fileSize: parseInt(process.env.MAX_FILE_SIZE) || 52428800,
  },
});

const uploadSingle = (fieldName = "file") => {
  return multerConfig.single(fieldName);
};

const uploadMultiple = (fieldName = "files", maxFiles = 5) => {
  return multerConfig.array(fieldName, maxFiles);
};

const uploadFields = (fields = []) => {
  return multerConfig.fields(fields);
};

const getFileUrl = (filePath) => {
  if (!filePath) return null;

  const baseUrl = process.env.API_URL || "http://localhost:3000";
  const relativePath = filePath.replace(/\\/g, "/");

  return `${baseUrl}/${relativePath}`;
};

const deleteFile = (filePath) => {
  try {
    if (fs.existsSync(filePath)) {
      fs.unlinkSync(filePath);
      return true;
    }
    return false;
  } catch (error) {
    console.error(`Error deleting file ${filePath}:`, error);
    return false;
  }
};

const multerErrorHandler = (error, req, res, next) => {
  if (error instanceof multer.MulterError) {
    if (error.code === "FILE_TOO_LARGE") {
      return res.status(400).json({
        success: false,
        message: `File size exceeds maximum limit of ${process.env.MAX_FILE_SIZE || 52428800} bytes`,
      });
    } else if (error.code === "LIMIT_FILE_COUNT") {
      return res.status(400).json({
        success: false,
        message: "Too many files uploaded",
      });
    } else if (error.code === "LIMIT_PART_COUNT") {
      return res.status(400).json({
        success: false,
        message: "Too many parts in request",
      });
    }
  }

  if (error) {
    return res.status(400).json({
      success: false,
      message: error.message,
    });
  }

  next();
};

module.exports = {
  storage,
  multerConfig,
  uploadSingle,
  uploadMultiple,
  uploadFields,
  getFileUrl,
  deleteFile,
  multerErrorHandler,
};
