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
const jwt = getJwtFromCookie();
const userId = getUserIdFromJwt(jwt);
function manageCart() {
    if (jwt) {

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
async function createCartIfNotExists(userId) {
    const apiUrl = `https://localhost:7241/api/Cart/cart/user/${userId}`;
    const xhr = new XMLHttpRequest();
    xhr.open('GET', apiUrl, true);

    xhr.onload = function () {
        if (xhr.status === 200) {
            const data = JSON.parse(xhr.responseText);
            if (data && data.length > 0) {
                const cartId = data[0].id;
                console.log('Giỏ hàng đã tồn tại. ID giỏ hàng:', cartId);
                getCartDetails(cartId);
            } else {
                console.log('Người dùng chưa có giỏ hàng, tạo giỏ hàng mới...');
                createNewCart(userId);
                getCartDetails(cartId);
            }
        } else {
            console.error('Có lỗi xảy ra khi gọi API.', xhr.responseText);
        }
    };

    xhr.onerror = function () {
        console.error('Có lỗi xảy ra khi gọi API.');
    };

    xhr.send();
}
function createNewCart(userId) {
    const createCartUrl = 'https://localhost:7241/api/Cart/CartCreate';
    const xhr = new XMLHttpRequest();
    xhr.open('POST', createCartUrl, true);
    xhr.setRequestHeader('Content-Type', 'application/json');

    const requestBody = {
        description: "Giỏ hàng mới",
        idUser: userId,
        status: 1
    };

    xhr.onload = function () {
        if (xhr.status === 200) {
            const response = JSON.parse(xhr.responseText);
            if (response.status === "Success") {
                console.log('Giỏ hàng được tạo thành công.');
                loadCartFromServer(userId);
            } else {
                console.error('Không thể tạo giỏ hàng:', response.message);
            }
        } else {
            console.error('Có lỗi xảy ra khi tạo giỏ hàng.', xhr.responseText);
        }
    };

    xhr.onerror = function () {
        console.error('Có lỗi xảy ra khi gọi API.');
    };

    xhr.send(JSON.stringify(requestBody));
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
            console.log('ID giỏ hàng:', cartId);
            getCartDetails(cartId);
        } else if (xhr.status === 404) {
            console.error('Không tìm thấy API hoặc người dùng không có giỏ hàng.');
            createNewCart(userId);
        } else {
            console.error('Có lỗi xảy ra khi gọi API.', xhr.responseText);
        }
    };

    xhr.onerror = function () {
        console.error('Có lỗi xảy ra khi gọi API.');
    };

    xhr.send();
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
                        resolve();
                    } else {
                        console.error('Có lỗi xảy ra khi gọi API.', xhr.responseText);
                        reject();
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
                        <h5 style="color: red;">${item.unitPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</h5>
                    </div>
                </td>
                <td class="quantity__item">
                    <div class="quantity">
                        <div class="pro-qty-2">
                            <input type="text" value="${item.quantity}" min="1" step="1">
                        </div>
                    </div>
                </td>
                <td class="cart__price">${item.unitPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</td>
                <td class="total__price">${item.totalPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</td>
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
                        data-id-options="${item.idOptions}" 
                        id="btn_delete_cart_cookies"
                        style="background-color: #f44336; color: white; border: none; padding: 8px 16px; text-align: center; text-decoration: none; display: inline-block; font-size: 16px; margin: 4px 2px; cursor: pointer; border-radius: 4px;">
                        Xóa
                    </button>
                </td>
            `;
            cartItemsContainer.appendChild(itemElement);
        });

        document.getElementById('total_amount').innerText = `Tổng cộng: ${totalAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}`;
        document.getElementById('btn_checkout').disabled = false;
    } else {
        cartItemsContainer.innerHTML = '<li class="header-cart-item flex-w flex-t m-b-12">Giỏ hàng trống</li>';
        document.getElementById('total_amount').innerText = 'Tổng cộng: 0';
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
        const target = event.target;

        if (target && target.id === 'btn_delete_cart_cookies') {
            const idOptions = target.getAttribute('data-id-options');
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
                    deleteCartOptionCookies(idOptions);
                } else {
                    console.log('Xóa tùy chọn bị hủy.');
                }
            });
        } else if (target && target.classList.contains('btn-delete')) {
            const idCart = target.getAttribute('data-id-cart');
            const idOptions = target.getAttribute('data-id-options');
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
function deleteCartOptionCookies(idOptions) {
    const cartCookie = getCookieValue('cart');
    let cartDataList = cartCookie ? JSON.parse(cartCookie) : [];

    const indexToDelete = cartDataList.findIndex(item => item.idOptions === idOptions);
    if (indexToDelete !== -1) {
        cartDataList.splice(indexToDelete, 1);

        document.cookie = `cart=${encodeURIComponent(JSON.stringify(cartDataList))}; path=/; max-age=${60 * 60 * 24 * 7}`;

        Swal.fire({
            title: 'Thành công!',
            text: 'Sản phẩm đã được xóa khỏi giỏ hàng.',
            icon: 'success',
            confirmButtonText: 'OK'
        }).then(() => {
            manageCart();
        });
    } else {
        Swal.fire({
            title: 'Lỗi!',
            text: 'Không tìm thấy sản phẩm trong giỏ hàng.',
            icon: 'error',
            confirmButtonText: 'OK'
        });
    }
}
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
                    text: 'Đã xảy ra lỗi khi xóa tùy chọn.',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            }
        }
    };

    xhr.onerror = function () {
        console.error('Có lỗi xảy ra khi gọi API.');
        Swal.fire({
            title: 'Lỗi!',
            text: 'Không thể kết nối đến máy chủ.',
            icon: 'error',
            confirmButtonText: 'OK'
        });
    };

    xhr.send();
}
function updateTotalAmount() {
    let totalAmount = 0;

    cartDataList.forEach(item => {
        totalAmount += item.totalPrice;
    });

    document.getElementById('total_amount').innerText = `Tổng cộng: ${totalAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}`;
    saveCartToCookies();
}
async function addToCartFromCookies(idcart) {
    const cartCookie = getCookieValue('cart');
    const jwtCookie = getCookieValue('jwt');
    if (!cartCookie || !jwtCookie) {
        console.error('Cookies không hợp lệ hoặc không tồn tại.');
        return;
    }

    const cartItems = JSON.parse(cartCookie);
    if (!cartCookie || !jwtCookie) {
        console.error('Cookies không hợp lệ hoặc không tồn tại.');
        return;
    }

    for (const item of cartItems) {
        const requestBody = {
            createBy: userId,
            idOptions: item.idOptions,
            idCart: idcart,
            quantity: item.quantity,
        };
        console.log('requestBody', requestBody)

        try {
            const response = await fetch('https://localhost:7241/api/CartOptions/AddToCart', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${jwtCookie}`
                },
                body: JSON.stringify(requestBody)
            });

            if (response.ok) {
                console.log('Sản phẩm đã được thêm vào giỏ hàng thành công.');
                document.cookie = 'cart=; path=/; max-age=0';
            } else {
                console.error('Có lỗi xảy ra khi thêm sản phẩm vào giỏ hàng.', await response.text());
            }
        } catch (error) {
            console.error('Có lỗi xảy ra khi gọi API.', error);
        }
    }
}

function updateCartInCookies(selectedIdOptions, quantity) {
    const product = {
        idOptions: selectedIdOptions,
        quantity: quantity,
    };

    const cartCookie = getCookieValue('cart');
    let cartDataList = cartCookie ? JSON.parse(cartCookie) : [];

    const existingProductIndex = cartDataList.findIndex(item => item.idOptions === product.idOptions);

    if (existingProductIndex !== -1) {
        cartDataList[existingProductIndex].quantity = product.quantity;
    } else {
        cartDataList.push(product);
    }

    document.cookie = `cart=${encodeURIComponent(JSON.stringify(cartDataList))}; path=/; max-age=${60 * 60 * 24 * 7}`;

    Swal.fire({
        title: 'Thành công!',
        text: 'Sản phẩm đã được cập nhật trong giỏ hàng.',
        icon: 'success',
        showCancelButton: true,
        confirmButtonText: 'Đến giỏ hàng',
        cancelButtonText: 'Tiếp tục mua sắm'
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = '/cart_index';
        }
    });
}

function updateCartOnServer() {
    const cartItems = getCartItems();

    for (const item of cartItems) {
        if (isNaN(item.quantity) || item.quantity <= 0) {
            Swal.fire({
                icon: 'error',
                title: 'Lỗi!',
                text: 'Số lượng sản phẩm không hợp lệ.',
                confirmButtonText: 'OK'
            });
            return;
        }
    }

    cartItems.forEach(item => {
        const xhr = new XMLHttpRequest();
        const apiUrl = `https://localhost:7241/api/CartOptions/${item.idCart}/${item.idOptions}`;
        xhr.open('PUT', apiUrl, true);
        xhr.setRequestHeader('Content-Type', 'application/json');

        xhr.onreadystatechange = function () {
            if (xhr.readyState === XMLHttpRequest.DONE) {
                if (xhr.status === 200) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Cập nhật thành công!',
                        text: 'Giỏ hàng đã được cập nhật.',
                        confirmButtonText: 'OK'
                    }).then(() => {
                        manageCart();
                    });
                } else {
                    let errorMessage = xhr.responseText;
                    if (xhr.responseText) {
                        try {
                            const response = JSON.parse(xhr.responseText);
                            errorMessage = response.message || errorMessage;
                        } catch (e) {
                            console.error('Error parsing response:', e);
                        }
                    }
                    Swal.fire({
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
            Swal.fire({
                icon: 'error',
                title: 'Lỗi!',
                text: 'Có lỗi xảy ra trong quá trình gửi yêu cầu.',
                confirmButtonText: 'OK'
            });
            console.error('Network Error:', xhr.statusText);
        };

        const requestBody = JSON.stringify({
            modifiedBy: userId,
            quantity: item.quantity
        });

        xhr.send(requestBody);
    });
}
function getCartItems() {
    const items = [];
    const rows = document.querySelectorAll('#data_cart_table tr');

    rows.forEach(row => {
        const idCart = row.querySelector('[data-id-cart]').getAttribute('data-id-cart');
        const idOptions = row.querySelector('[data-id-options]').getAttribute('data-id-options');
        const quantity = parseInt(row.querySelector('input').value, 10);

        items.push({ idCart, idOptions, quantity });
    });

    return items;
}
