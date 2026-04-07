/**
 * MongoDB multi-document transactions chỉ chạy trên replica set hoặc sharded cluster.
 * Trên standalone (localhost mặc định), startTransaction sẽ lỗi — dùng hàm này để nhận biết và fallback.
 * @param {Error} err
 * @returns {boolean}
 */
function transactionUnsupported(err) {
  const m = String(err?.message || "");
  return (
    m.includes("Transaction numbers are only allowed") ||
    m.includes("Multi-document transactions are only allowed")
  );
}

module.exports = { transactionUnsupported };
