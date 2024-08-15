let cartDataList = [];

function getJwtFromCookie() {
    return getCookieValue('jwt');
}

function getCookieValue(cookieName) {
    const cookies = document.cookie.split(';');
    for (let cookie of cookies) {
        const [name, value] = cookie.split('=').map(c => c.trim());
        if (name === cookieName) {
            return decodeURIComponent(value);
        }
    }
    return null;
}

function getUserIdFromJwt(jwt) {
    try {
        const tokenPayload = JSON.parse(atob(jwt.split('.')[1]));
        return tokenPayload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'];
    } catch (error) {
        console.error('Error parsing JWT:', error);
        return null;
    }
}

function manageCart() {
    const jwt = getJwtFromCookie();
    let apiUrl;

    if (jwt) {
        const userId = getUserIdFromJwt(jwt);
        if (userId) {
            loadCartFromServer(userId);
        } else {
            console.error('Không thể lấy User ID từ JWT.');
            return;
        }
    } else {
        loadCartFromCookies();
    }
}

function loadCartFromServer(userId) {
    const apiUrl = `https://localhost:7241/api/Cart/cart/user/${userId}`;
        const xhr = new XMLHttpRequest();
        xhr.open('GET', apiUrl, true);
        xhr.onload = function () {
            if (xhr.status === 200) {
                const data = JSON.parse(xhr.responseText);
                console.log('cart', data);
                const cartId = data[0].id;
                console.log('idcart', cartId);

                getCartDetails(cartId);
            } else {
                console.error('Có lỗi xảy ra khi gọi API.', xhr.responseText);
            }
        };
        xhr.onerror = function () {
            console.error('Có lỗi xảy ra khi gọi API.');
        };
        xhr.send();
    }

function loadCartFromCookies() {
    const cartData = getCookieValue('cart');
    if (cartData) {
        const cartDataList = JSON.parse(cartData);
        const promises = cartDataList.map(product => {
            const optionUrl = `https://localhost:7241/api/Options/GetByID/${product.idOptions}`;
            return new Promise((resolve, reject) => {
                const xhr = new XMLHttpRequest();
                xhr.open('GET', optionUrl, true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        const optionData = JSON.parse(xhr.responseText);
                        product.colorName = optionData.colorName;
                        product.productName = optionData.productName;
                        product.retailPrice = optionData.retailPrice;
                        product.sizeName = optionData.sizesName;
                        product.imageURL = optionData.imageURL;
                        resolve(); // Hoàn thành yêu cầu
                    } else {
                        console.error('Có lỗi xảy ra khi gọi API.', xhr.responseText);
                        reject(); // Thất bại yêu cầu
}
                };
                xhr.send();
            });
        });

        Promise.all(promises).then(() => {
            displayCartCookies(cartDataList);
        }).catch(error => {
            console.error('Có lỗi xảy ra khi lấy dữ liệu giỏ hàng:', error);
        });
    }
}
function saveCartToCookies() {
    document.cookie = `cart=${encodeURIComponent(JSON.stringify(cartDataList))}; path=/; max-age=86400`;
}

function getCartDetails(cartId) {
    const apiUrl = `https://localhost:7241/api/CartOptions/GetAllByCartIDAsync/${cartId}`;
    const xhr = new XMLHttpRequest();
    xhr.open('GET', apiUrl, true);
    xhr.onload = function () {
        if (xhr.status === 200) {
            const data = JSON.parse(xhr.responseText);
            displayCart(data);
            console.log(data);
        } else {
            console.error('Có lỗi xảy ra khi gọi API.', xhr.responseText);
        }
    };
    xhr.onerror = function () {
        console.error('Có lỗi xảy ra khi gọi API.');
    };
    xhr.send();
}

function displayCart(cartData) {
    cartDataList = cartData;
    console.log('cartData', cartData)
    const cartItemsContainer = document.getElementById('data_cart_table');
    cartItemsContainer.innerHTML = '';

    let totalAmount = 0;

    if (cartData && cartData.length > 0) {
        cartData.forEach(item => {
            console.log('data cart', item);
            const itemElement = document.createElement('tr');
            itemElement.innerHTML = `
                <td class="product__cart__item">
                    <div class="product__cart__item__pic">
                        <img src="${item.imageURL}"  style="width: 70px;" alt="">
                    </div>
                    <div class="product__cart__item__text">
                        <h6>${item.productName}</h6><small>Phân loại: ${item.colorName} - ${item.sizeName}</small>
                        <h5 style="color: red;">${item.unitPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' }) }</h5>
                    </div>
                </td>
                <td class="quantity__item">
                    <div class="quantity">
                        <div class="pro-qty-2">
                            <input type="text" value="${item.quantity}" min="1" step="1">
                        </div>
                    </div>
                </td>
                <td class="cart__price">${item.unitPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' }) }</td>
                <td class="total__price">${item.totalPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' }) }</td>
                <td class="cart__close">
                    <button
                        type="button"
                        class="btn-delete"
                        data-id-cart="${item.idCart}" 
                        data-id-options="${item.idOptions}" 
                        style="background-color: #f44336; color: white; border: none; padding: 8px 16px; text-align: center; text-decoration: none; display: inline-block; font-size: 16px; margin: 4px 2px; cursor: pointer; border-radius: 4px;">
                        Xóa
                    </button>
                </td>
            `;
            cartItemsContainer.appendChild(itemElement);
            totalAmount += item.totalPrice;
        });

        document.getElementById('total_amount').innerText = `Tổng cộng: ${totalAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}`;
    } else {
        cartItemsContainer.innerHTML = '<li class="header-cart-item flex-w flex-t m-b-12">Giỏ hàng trống</li>';
        document.getElementById('btn_checkout').disabled = true;
    }
}
function displayCartCookies(cartData) {
    cartDataList = cartData;
    const cartItemsContainer = document.getElementById('data_cart_table');
    cartItemsContainer.innerHTML = '';

    let totalAmount = 0;

    if (cartData && cartData.length > 0) {
        cartData.forEach(item => {
            const itemTotalPrice = item.retailPrice * item.quantity;

            totalAmount += itemTotalPrice;

            const itemElement = document.createElement('tr');
            itemElement.innerHTML = `
                <td class="product__cart__item">
                    <div class="product__cart__item__pic">
                        <img src="${item.imageURL}" style="width: 70px;" alt="">
                    </div>
                    <div class="product__cart__item__text">
                        <h6>${item.productName}</h6>
                        <small>Phân loại: ${item.colorName} - ${item.sizeName}</small>
                        <h5 style="color: red;">${item.retailPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</h5>
                    </div>
                </td>
                <td class="quantity__item">
                    <div class="quantity">
                        <div class="pro-qty-2">
                            <input type="text" value="${item.quantity}" min="1" step="1">
                        </div>
                    </div>
                </td>
                <td class="cart__price">${item.retailPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</td>
                <td class="total__price">${itemTotalPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</td>
                <td class="cart__close">
                    <button
                        type="button"
                        class="btn-delete"
                        data-id-cart="${item.idCart}" 
                        data-id-options="${item.idOptions}" 
                        style="background-color: #f44336; color: white; border: none; padding: 8px 16px; text-align: center; text-decoration: none; display: inline-block; font-size: 16px; margin: 4px 2px; cursor: pointer; border-radius: 4px;">
                        Xóa
                    </button>
                </td>
            `;
            cartItemsContainer.appendChild(itemElement);
        });

        document.getElementById('total_amount').innerText = `Tổng cộng: ${totalAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}`;
    } else {
        cartItemsContainer.innerHTML = '<li class="header-cart-item flex-w flex-t m-b-12">Giỏ hàng trống</li>';
        document.getElementById('btn_checkout').disabled = true;
    }
}

document.addEventListener('DOMContentLoaded', function () {
    manageCart();

    const checkoutButton = document.getElementById('btn_checkout');
    if (checkoutButton) {
        checkoutButton.addEventListener('click', function (event) {
            if (cartDataList.length === 0) {
            event.preventDefault();
                Swal.fire({
                    icon: 'error',
                    title: 'Giỏ hàng rỗng!',
                    text: 'Vui lòng thêm sản phẩm vào giỏ hàng trước khi thanh toán.',
                    confirmButtonText: 'OK'
                });
            } else {
            const encodedCartData = encodeURIComponent(JSON.stringify(cartDataList));
            const checkoutUrl = `checkout_user?data=${encodedCartData}`;

            window.location.href = checkoutUrl;
            }
        });
    }

    document.addEventListener('click', function (event) {
        if (event.target && event.target.classList.contains('btn-delete')) {
            const idCart = event.target.getAttribute('data-id-cart');
            const idOptions = event.target.getAttribute('data-id-options');

            Swal.fire({
                title: 'Xác nhận xóa',
                text: 'Bạn có chắc chắn muốn xóa tùy chọn này khỏi giỏ hàng?',
                icon: 'warning',
                showCancelButton: true,
                confirmButtonText: 'Xóa',
                cancelButtonText: 'Hủy',
                reverseButtons: true
            }).then((result) => {
                if (result.isConfirmed) {
                    deleteCartOption(idCart, idOptions);
                } else {
                    console.log('Xóa tùy chọn bị hủy.');
                }
            });
        }
    });
});

function deleteCartOption(idCart, idOptions) {
    const xhr = new XMLHttpRequest();
    const url = `https://localhost:7241/api/CartOptions/Delete/${idCart}/${idOptions}`;
    xhr.open('DELETE', url, true);
    xhr.setRequestHeader('Content-Type', 'application/json');

    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                console.log(`Đã xóa tùy chọn ${idOptions} khỏi giỏ hàng ${idCart}`);
                Swal.fire({
                    title: 'Thành công!',
                    text: 'Tùy chọn đã được xóa khỏi giỏ hàng.',
                    icon: 'success',
                    confirmButtonText: 'OK'
                }).then(() => {
                    const button = document.querySelector(`[data-id-cart="${idCart}"][data-id-options="${idOptions}"]`);
                    if (button) {
                        button.closest('tr').remove();
                    }

                    updateTotalAmount();
                    manageCart();
                });
            } else {
                console.error('Lỗi khi xóa tùy chọn:', xhr.statusText);
                Swal.fire({
                    title: 'Lỗi!',
                    text: 'Đã xảy ra lỗi khi xóa tùy chọn. Vui lòng thử lại.',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            }
        }
    };

    xhr.send();
}

function updateTotalAmount() {
    let totalAmount = 0;
    const cartItemsContainer = document.getElementById('data_cart_table');
    cartItemsContainer.querySelectorAll('tr').forEach(row => {
        const totalPriceCell = row.querySelector('.total__price');
        if (totalPriceCell) {
            const priceText = totalPriceCell.innerText;
            const totalPrice = parseFloat(priceText.replace(/\./g, '').replace(/[^0-9.-]/g, ''));
            totalAmount += totalPrice;
        }
    });

    document.getElementById('total_amount').innerText = `Tổng cộng: ${totalAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}`;
}

document.addEventListener('DOMContentLoaded', function () {
    const updateCartBtn = document.getElementById('update-cart-btn');

    if (updateCartBtn) {
        updateCartBtn.addEventListener('click', function (event) {
            event.preventDefault(); 
            updateCart();
        });
    }
});
function updateCart() {
    const cartItems = getCartItems();

    for (const item of cartItems) {
        if (isNaN(item.quantity) || item.quantity <= 0) {
            Swal.fire({
                icon: 'error',
                title: 'Lỗi!',
                    text: 'Đã xảy ra lỗi khi xóa tùy chọn.',
                        icon: 'error',
                        title: 'Lỗi!',
                        text: errorMessage,
                        confirmButtonText: 'OK'
                    });
                    console.error('Error:', xhr.statusText);
                }
            }
        };

        xhr.onerror = function () {
        console.error('Có lỗi xảy ra khi gọi API.');
            Swal.fire({
            title: 'Lỗi!',
            text: 'Không thể kết nối đến máy chủ.',
                icon: 'error',
                title: 'Lỗi!',
                text: 'Có lỗi xảy ra trong quá trình gửi yêu cầu.',
                confirmButtonText: 'OK'
            });
            console.error('Network Error:', xhr.statusText);
        };

    xhr.send();
}
function getCartItems() {
    const items = [];
    const rows = document.querySelectorAll('#data_cart_table tr');

function updateTotalAmount() {
    let totalAmount = 0;

    cartDataList.forEach(item => {
        totalAmount += item.totalPrice;
    });

    document.getElementById('total_amount').innerText = `Tổng cộng: ${totalAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}`;
    saveCartToCookies();
}
