﻿let cartDataList = [];
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
console.log('userId', userId)
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
            const cartId = data[0].id;
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
function getOptionDetails(optionId, callback) {
    const apiUrl = `https://localhost:7241/api/Options/GetByID/${optionId}`;
    const xhr = new XMLHttpRequest();
    xhr.open('GET', apiUrl, true);
    xhr.onload = function () {
        if (xhr.status === 200) {
            const data = JSON.parse(xhr.responseText);
            callback(data);
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
    const cartItemsContainer = document.getElementById('data_cart_table');
    cartItemsContainer.innerHTML = '';

    let totalAmount = 0;
    let hasOutOfStockItem = false;

    if (cartData && cartData.length > 0) {
        let itemsProcessed = 0;
        cartData.forEach(item => {
            getOptionDetails(item.idOptions, optionData => {
                if (!optionData) {
                    console.error(`Không thể lấy thông tin sản phẩm với idOptions: ${item.idOptions}`);
                    return; // Bỏ qua sản phẩm nếu không lấy được dữ liệu
                }

                const quantityDisplay = optionData.stockQuantity > 0 ?
                    `<span style="color: green;">Còn hàng</span>` :
                    `<span style="color: red;">Hết hàng</span>`;

                const rowStyle = optionData.stockQuantity > 0 ?
                    '' : 'background-color: #b7b7b7;';

                const itemElement = document.createElement('tr');
                itemElement.style = rowStyle;

                itemElement.innerHTML = `
                    <td class="product__cart__item">
                        <div class="product__cart__item__pic">
                            <img src="${optionData.imageURL}" style="width: 100px;" alt="">
                        </div>
                        <div class="product__cart__item__text">
                            <h6>${optionData.productName}</h6>
                            <small>Phân loại: ${optionData.colorName} - ${optionData.sizesName}</small>
                            <br>
                            <small>Tình trạng: ${quantityDisplay}</small>
                            <br>
                            <h5 style="color: red;">${optionData.retailPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</h5>
                        </div>
                    </td>
                    <td class="quantity__item">
                        <div class="quantity">
                            <div class="pro-qty-2">
                                <input type="number" value="${item.quantity}" min="1" step="1" ${optionData.stockQuantity <= 0 ? 'disabled' : ''} data-id-cart="${item.idCart}" data-id-options="${item.idOptions}">
                            </div>
                        </div>
                    </td>
                    <td class="cart__price">${optionData.retailPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</td>
                    <td class="total__price">${(optionData.retailPrice * item.quantity).toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</td>
                    <td class="cart__close">
                        <button
                            type="button"
                            class="btn-delete"
                            data-id-cart="${item.idCart}" 
                            data-id-options="${item.idOptions}" 
                            style="background-color: #f44336; color: white; border: none; padding: 8px 16px; text-align: center; text-decoration: none; display: inline-block; font-size: 16px; margin: 4px 2px; cursor: pointer; border-radius: 4px;"
                        >
                            Xóa
                        </button>
                    </td>
                `;
                cartItemsContainer.appendChild(itemElement);

                totalAmount += optionData.retailPrice * item.quantity;

                if (optionData.stockQuantity <= 0) {
                    hasOutOfStockItem = true;
                }

                itemElement.querySelector('input[type="number"]').addEventListener('input', (e) => {
                    const newQuantity = parseInt(e.target.value);
                    if (!isNaN(newQuantity) && newQuantity > 0) {
                        const newTotalPrice = optionData.retailPrice * newQuantity;
                        e.target.closest('tr').querySelector('.total__price').innerText = newTotalPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });

                        totalAmount = cartData.reduce((sum, cartItem) => {
                            const row = cartItemsContainer.querySelector(`input[data-id-cart="${cartItem.idCart}"]`);
                            const currentQuantity = row ? parseInt(row.value) : cartItem.quantity;
                            return sum + (optionData.retailPrice * currentQuantity);
                        }, 0);

                        document.getElementById('total_amount').innerText = `Tổng cộng: ${totalAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}`;

                        updateCartItem(item.idCart, item.idOptions, newQuantity);
                    } else {
                        e.target.value = 1;
                        updateCartItem(item.idCart, item.idOptions, 1);
                        toastr.warning('Định dạng sai, số lượng được đặt về 1', 'Cảnh báo');
                    }
                });

                itemsProcessed++;

                if (itemsProcessed === cartData.length) {
                    document.getElementById('total_amount').innerText = `Tổng cộng: ${totalAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}`;
                    var checkoutButton = document.getElementById('btn_checkout');
                    if (totalAmount <= 0) {
                        checkoutButton.style.backgroundColor = '#b7b7b7';
                        checkoutButton.style.color = '#ffffff';
                        checkoutButton.style.cursor = 'not-allowed';
                        checkoutButton.disabled = true;
                    } else {
                        checkoutButton.style.backgroundColor = 'blue'; 
                        checkoutButton.style.color = '';
                        checkoutButton.style.cursor = '';
                        checkoutButton.disabled = false;
                    }
                }
            });
        });

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
        const optionDetailsPromises = cartData.map(item => {
            return new Promise((resolve, reject) => {
                getOptionDetails(item.idOptions, optionData => {
                    if (!optionData) reject('Không tìm thấy dữ liệu sản phẩm');

                    const itemTotalPrice = item.retailPrice * item.quantity;
                    totalAmount += itemTotalPrice;

                    const quantityDisplay = optionData.stockQuantity > 0 ?
                        `<span style="color: green;">Còn hàng</span>` :
                        `<span style="color: red;">Hết hàng</span>`;

                    const rowStyle = optionData.stockQuantity > 0 ?
                        '' : 'background-color: #b7b7b7;';

                    const itemElement = document.createElement('tr');
                    itemElement.style = rowStyle;
                    itemElement.innerHTML = `
                    <td class="product__cart__item">
                        <div class="product__cart__item__pic">
                            <img src="${item.imageURL}" style="width: 100px;" alt="">
                        </div>
                        <div class="product__cart__item__text">
                            <h6>${item.productName}</h6>
                            <small>Phân loại: ${item.colorName} - ${item.sizeName}</small>
                            <br>
                            <small>Tình trạng: ${quantityDisplay}</small>
                            <br>
                            <h5 style="color: red;">${item.retailPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</h5>
                        </div>
                    </td>
                    <td class="quantity__item">
                        <div class="quantity">
                            <div class="pro-qty-2">
                                <input type="number" value="${item.quantity}" min="1" step="1">
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

                    resolve();
                });
            });
        });

        Promise.all(optionDetailsPromises).then(() => {
            document.getElementById('total_amount').innerText = `Tổng cộng: ${totalAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}`;
            document.getElementById('btn_checkout').disabled = false;
        }).catch(err => {
            console.error(err);
        });
    } else {
        cartItemsContainer.innerHTML = '<li class="header-cart-item flex-w flex-t m-b-12">Giỏ hàng trống</li>';
        document.getElementById('total_amount').innerText = 'Tổng cộng: 0';
        document.getElementById('btn_checkout').disabled = true;
    }
}

document.addEventListener('DOMContentLoaded', function () {
    manageCart();
    const jwt = getJwtFromCookie();
    if (!jwt) {
        console.log('JWT không tồn tại. Đã xóa cookie orderData.');
    }
    const checkOutOfStock = () => {
        return new Promise((resolve) => {
            let hasOutOfStock = false;
            let outOfStockOptions = [];
            let itemsProcessed = 0;

            cartDataList.forEach(item => {
                getOptionDetails(item.idOptions, optionData => {
                    if (optionData && optionData.stockQuantity <= 0) {
                        hasOutOfStock = true;
                        outOfStockOptions.push(item.idOptions);
                    }
                    itemsProcessed++;
                    if (itemsProcessed === cartDataList.length) {
                        resolve({ hasOutOfStock, outOfStockOptions });
                    }
                });
            });
        });
    };

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
                if (!jwt) {
                    console.log('không');
                    checkOutOfStock().then(({ hasOutOfStock, outOfStockOptions }) => {
                        console.log('outOfStockOptions', outOfStockOptions)
                        console.log('hasOutOfStock', hasOutOfStock)
                        if (hasOutOfStock) {
                            event.preventDefault();
                            Swal.fire({
                                icon: 'error',
                                title: 'Có sản phẩm hết hàng!',
                                text: 'Giỏ hàng của bạn chứa sản phẩm hết hàng. Bạn có muốn xóa những sản phẩm này khỏi giỏ hàng không?',
                                showCancelButton: true,
                                confirmButtonText: 'Xóa sản phẩm hết hàng',
                                cancelButtonText: 'Giữ lại sản phẩm'
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    deleteCartOptionCookies_ver1(outOfStockOptions);
                                }
                            });
                        } else {
                            const encodedCartData = encodeURIComponent(JSON.stringify(cartDataList));
                            const checkoutUrl = `checkout_user?data=${encodedCartData}`;
                            window.location.href = checkoutUrl;
                        }
                    }).catch(error => {
                        console.error('Lỗi khi kiểm tra tình trạng hàng:', error);
                        Swal.fire({
                            title: 'Lỗi!',
                            text: 'Không thể kiểm tra tình trạng hàng.',
                            icon: 'error',
                            confirmButtonText: 'OK'
                        });
                    });
                } else {
                    console.log('có');
                    checkOutOfStock().then(hasOutOfStockItem => {
                        if (hasOutOfStockItem) {
                            event.preventDefault();
                            Swal.fire({
                                icon: 'error',
                                title: 'Có sản phẩm hết hàng!',
                                text: 'Giỏ hàng của bạn chứa sản phẩm hết hàng. Bạn có muốn xóa những sản phẩm này khỏi giỏ hàng không?',
                                showCancelButton: true,
                                confirmButtonText: 'Xóa sản phẩm hết hàng',
                                cancelButtonText: 'Giữ lại sản phẩm'
                            }).then((result) => {
                                if (result.isConfirmed) {
                                    removeOutOfStockItems();
                                }
                            });
                        } else {
                            const encodedCartData = encodeURIComponent(JSON.stringify(cartDataList));
                            const checkoutUrl = `checkout_user?data=${encodedCartData}`;
                            window.location.href = checkoutUrl;
                        }
                    }).catch(error => {
                        console.error('Lỗi khi kiểm tra tình trạng hàng:', error);
                        Swal.fire({
                            title: 'Lỗi!',
                            text: 'Không thể kiểm tra tình trạng hàng.',
                            icon: 'error',
                            confirmButtonText: 'OK'
                        });
                    });
                }
             
                
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
                    deleteCartItem(idCart, idOptions);
                } else {
                    console.log('Xóa sản phẩm bị hủy.');
                }
            });
        }
    });
});
function updateCartItem(idCart, idOptions, newQuantity) {
    const xhr = new XMLHttpRequest();
    const apiUrl = `https://localhost:7241/api/CartOptions/${idCart}/${idOptions}`;
    xhr.open('PUT', apiUrl, true);
    xhr.setRequestHeader('Content-Type', 'application/json');

    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                try {
                    const updatedCartData = JSON.parse(xhr.responseText);
                    displayCart(updatedCartData); 
                } catch (e) {
                    console.log(xhr.responseText);
                    toastr.success(xhr.responseText, 'Thành công');
                }
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi!',
                    text: 'Có lỗi xảy ra khi cập nhật giỏ hàng.',
                    confirmButtonText: 'OK'
                });
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
    };

    const requestBody = JSON.stringify({
        modifiedBy: userId,
        quantity: newQuantity
    });

    xhr.send(requestBody);
}
function deleteCartOptionCookies(idOptions) {
    const cartCookie = getCookieValue('cart');
    let cartDataList = cartCookie ? JSON.parse(cartCookie) : [];
    console.log('cartDataList:', cartDataList);
    console.log('idOptions:', idOptions);

    // Kiểm tra từng phần tử trong cartDataList
    cartDataList.forEach(item => {
        console.log('item.idOptions:', item.idOptions);
    });

    const indexToDelete = cartDataList.findIndex(item => item.idOptions === idOptions);
    console.log('indexToDelete:', indexToDelete);
    console.log('cartCookie:', cartCookie);

    if (indexToDelete !== -1) {
        cartDataList.splice(indexToDelete, 1);

        document.cookie = `cart=${encodeURIComponent(JSON.stringify(cartDataList))}; path=/; max-age=${60 * 60 * 24 * 7}`;

        Swal.fire({
            title: 'Thành công!',
            text: 'Sản phẩm đã được xóa khỏi giỏ hàng.',
            icon: 'success',
            confirmButtonText: 'OK'
        }).then(() => {
            displayCartCookies(cartDataList);
            updateTotalAmount();
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
function deleteCartOptionCookies_ver1(idOptionsList) {
    const cartCookie = getCookieValue('cart');
    let cartDataList = cartCookie ? JSON.parse(cartCookie) : [];

    idOptionsList.forEach(idOptions => {
        const indexToDelete = cartDataList.findIndex(item => item.idOptions === idOptions);
        console.log('idOptions:', idOptions);
        console.log('indexToDelete:', indexToDelete);

        if (indexToDelete !== -1) {
            cartDataList.splice(indexToDelete, 1);
        } else {
            console.log('Không tìm thấy sản phẩm với idOptions:', idOptions);
        }
    });

    document.cookie = `cart=${encodeURIComponent(JSON.stringify(cartDataList))}; path=/; max-age=${60 * 60 * 24 * 7}`;

    Swal.fire({
        title: 'Thành công!',
        text: 'Sản phẩm đã được xóa khỏi giỏ hàng.',
        icon: 'success',
        confirmButtonText: 'OK'
    }).then(() => {
        displayCartCookies(cartDataList);
        updateTotalAmount();
        manageCart();
    });
}
function removeOutOfStockItems() {
    const promises = cartDataList.map(item => {
        return new Promise((resolve, reject) => {
            getOptionDetails(item.idOptions, optionData => {
                if (optionData && optionData.stockQuantity <= 0) {
                    deleteCartOption(item.idCart, item.idOptions, resolve, reject);
                } else {
                    resolve();
                }
            });
        });
    });

    Promise.all(promises)
        .then(() => {
            manageCart();
        })
        .catch(error => {
            console.error('Lỗi khi xóa sản phẩm hết hàng:', error);
            Swal.fire({
                title: 'Lỗi!',
                text: 'Không thể xóa sản phẩm hết hàng.',
                icon: 'error',
                confirmButtonText: 'OK'
            });
        });
}
function deleteCartOption(idCart, idOptions, resolve, reject) {
    const xhr = new XMLHttpRequest();
    const url = `https://localhost:7241/api/CartOptions/Delete/${idCart}/${idOptions}`;
    xhr.open('DELETE', url, true);
    xhr.setRequestHeader('Content-Type', 'application/json');

    xhr.onload = function () {
        if (xhr.status === 200) {
            console.log(`Đã xóa tùy chọn ${idOptions} khỏi giỏ hàng ${idCart}`);
            Swal.fire({
                title: 'Thành công!',
                text: 'Tùy chọn đã được xóa khỏi giỏ hàng.',
                icon: 'success',
                confirmButtonText: 'OK'
            }).then(() => {
                resolve(); 
            });
        } else {
            console.error('Lỗi khi xóa tùy chọn:', xhr.statusText);
            reject(new Error('Lỗi khi xóa tùy chọn')); 
        }
    };

    xhr.onerror = function () {
        console.error('Có lỗi xảy ra khi gọi API.');
        reject(new Error('Lỗi khi kết nối đến máy chủ')); 
    };

    xhr.send();
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
