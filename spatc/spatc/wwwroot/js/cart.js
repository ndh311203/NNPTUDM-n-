// Cart Management with localStorage

const CART_STORAGE_KEY = 'petShopCart';

// Get cart from localStorage
function getCart() {
    const cartJson = localStorage.getItem(CART_STORAGE_KEY);
    return cartJson ? JSON.parse(cartJson) : [];
}

// Save cart to localStorage
function saveCart(cart) {
    localStorage.setItem(CART_STORAGE_KEY, JSON.stringify(cart));
}

// Add item to cart
async function addToCart(productId, productType, quantity = 1) {
    const cart = getCart();
    
    // For pets, only allow quantity = 1
    if (productType === 'pet') {
        quantity = 1;
        // Remove existing pet if any (only one pet allowed)
        const existingPetIndex = cart.findIndex(item => item.productType === 'pet');
        if (existingPetIndex !== -1) {
            cart.splice(existingPetIndex, 1);
        }
    }

    // Check if item already exists
    const existingIndex = cart.findIndex(
        item => item.productId === productId && item.productType === productType
    );

    if (existingIndex !== -1) {
        // Update quantity
        if (productType === 'pet') {
            // Pet can only have quantity 1
            return { success: false, message: 'Thú cưng chỉ có thể mua số lượng 1' };
        } else {
            // Check stock before increasing
            const stockCheck = await checkStock(productId, productType);
            if (stockCheck.success) {
                const newQuantity = cart[existingIndex].quantity + quantity;
                if (newQuantity > stockCheck.stock) {
                    return { success: false, message: `Chỉ còn ${stockCheck.stock} sản phẩm trong kho` };
                }
                cart[existingIndex].quantity = newQuantity;
            } else {
                return stockCheck;
            }
        }
    } else {
        // Check stock
        const stockCheck = await checkStock(productId, productType);
        if (!stockCheck.success) {
            return stockCheck;
        }
        if (quantity > stockCheck.stock) {
            return { success: false, message: `Chỉ còn ${stockCheck.stock} sản phẩm trong kho` };
        }

        // Add new item
        cart.push({
            productId: productId,
            productType: productType,
            quantity: quantity
        });
    }

    saveCart(cart);
    updateCartIcon();
    return { success: true, message: 'Đã thêm vào giỏ hàng' };
}

// Remove item from cart
function removeFromCart(productId, productType) {
    const cart = getCart();
    const filteredCart = cart.filter(
        item => !(item.productId === productId && item.productType === productType)
    );
    saveCart(filteredCart);
    updateCartIcon();
    return filteredCart;
}

// Update item quantity
async function updateQuantity(productId, productType, newQuantity) {
    if (productType === 'pet') {
        return { success: false, message: 'Thú cưng chỉ có thể mua số lượng 1' };
    }

    if (newQuantity <= 0) {
        return removeFromCart(productId, productType);
    }

    // Check stock
    const stockCheck = await checkStock(productId, productType);
    if (!stockCheck.success) {
        return stockCheck;
    }
    if (newQuantity > stockCheck.stock) {
        return { success: false, message: `Chỉ còn ${stockCheck.stock} sản phẩm trong kho` };
    }

    const cart = getCart();
    const itemIndex = cart.findIndex(
        item => item.productId === productId && item.productType === productType
    );

    if (itemIndex !== -1) {
        cart[itemIndex].quantity = newQuantity;
        saveCart(cart);
        updateCartIcon();
        return { success: true };
    }

    return { success: false, message: 'Sản phẩm không tồn tại trong giỏ hàng' };
}

// Clear cart
function clearCart() {
    localStorage.removeItem(CART_STORAGE_KEY);
    updateCartIcon();
}

// Get cart count
function getCartCount() {
    const cart = getCart();
    return cart.reduce((total, item) => total + item.quantity, 0);
}

// Get product info from server
async function getProductInfo(productId, productType) {
    try {
        const response = await fetch('/Cart/GetProductInfo', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                productId: productId,
                productType: productType
            })
        });
        return await response.json();
    } catch (error) {
        return { success: false, message: error.message };
    }
}

// Check stock
async function checkStock(productId, productType) {
    try {
        const response = await fetch('/Cart/CheckStock', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify({
                productId: productId,
                productType: productType
            })
        });
        return await response.json();
    } catch (error) {
        return { success: false, message: error.message };
    }
}

// Update cart icon in navbar
function updateCartIcon() {
    const count = getCartCount();
    const cartIcon = document.getElementById('cartIcon');
    const cartBadge = document.getElementById('cartBadge');
    
    if (cartIcon && cartBadge) {
        if (count > 0) {
            cartBadge.textContent = count;
            cartBadge.style.display = 'inline-block';
        } else {
            cartBadge.style.display = 'none';
        }
    }
}

// Load cart page
async function loadCart() {
    const cart = getCart();
    
    if (!cart || cart.length === 0) {
        document.getElementById('cartEmpty').style.display = 'block';
        document.getElementById('cartContent').style.display = 'none';
        return;
    }

    document.getElementById('cartEmpty').style.display = 'none';
    document.getElementById('cartContent').style.display = 'block';

    let html = '';
    let subtotal = 0;

    for (const item of cart) {
        const productInfo = await getProductInfo(item.productId, item.productType);
        if (productInfo.success) {
            const itemTotal = productInfo.product.price * item.quantity;
            subtotal += itemTotal;

            html += `
                <div class="d-flex align-items-center mb-4 pb-4 border-bottom cart-item" data-product-id="${item.productId}" data-product-type="${item.productType}">
                    <div class="flex-shrink-0 me-3">
                        ${productInfo.image ? `<img src="${productInfo.image}" style="width: 100px; height: 100px; object-fit: cover; border-radius: 8px;">` : '<div style="width: 100px; height: 100px; background: #f0f0f0; border-radius: 8px;"></div>'}
                    </div>
                    <div class="flex-grow-1">
                        <h5 class="mb-1">${productInfo.product.name}</h5>
                        <p class="text-muted mb-2">${formatPrice(productInfo.product.price)} đ</p>
                        <div class="d-flex align-items-center">
                            ${item.productType === 'pet' ? 
                                `<span class="badge bg-primary me-2">Số lượng: 1</span>` :
                                `<div class="input-group" style="width: 120px;">
                                    <button class="btn btn-sm btn-outline-secondary" type="button" onclick="decreaseQuantity(${item.productId}, '${item.productType}')">-</button>
                                    <input type="number" class="form-control form-control-sm text-center" value="${item.quantity}" min="1" readonly>
                                    <button class="btn btn-sm btn-outline-secondary" type="button" onclick="increaseQuantity(${item.productId}, '${item.productType}')">+</button>
                                </div>`
                            }
                            <button class="btn btn-sm btn-danger ms-3" onclick="removeItem(${item.productId}, '${item.productType}')">
                                <i class="bi bi-trash"></i> Xóa
                            </button>
                        </div>
                    </div>
                    <div class="text-end">
                        <h5 class="text-primary mb-0">${formatPrice(itemTotal)} đ</h5>
                    </div>
                </div>
            `;
        }
    }

    document.getElementById('cartItems').innerHTML = html;
    document.getElementById('subtotal').textContent = formatPrice(subtotal) + ' đ';
    document.getElementById('total').textContent = formatPrice(subtotal) + ' đ';
}

// Increase quantity
async function increaseQuantity(productId, productType) {
    const cart = getCart();
    const item = cart.find(i => i.productId === productId && i.productType === productType);
    if (item) {
        const result = await updateQuantity(productId, productType, item.quantity + 1);
        if (result.success) {
            loadCart();
        } else {
            alert(result.message);
        }
    }
}

// Decrease quantity
async function decreaseQuantity(productId, productType) {
    const cart = getCart();
    const item = cart.find(i => i.productId === productId && i.productType === productType);
    if (item && item.quantity > 1) {
        const result = await updateQuantity(productId, productType, item.quantity - 1);
        if (result.success) {
            loadCart();
        } else {
            alert(result.message);
        }
    }
}

// Remove item
async function removeItem(productId, productType) {
    if (confirm('Bạn có chắc muốn xóa sản phẩm này khỏi giỏ hàng?')) {
        removeFromCart(productId, productType);
        loadCart();
    }
}

// Format price
function formatPrice(price) {
    return new Intl.NumberFormat('vi-VN').format(price);
}

// Initialize cart icon on page load
document.addEventListener('DOMContentLoaded', function() {
    updateCartIcon();
});

