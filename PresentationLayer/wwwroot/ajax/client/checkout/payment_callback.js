﻿function getJwtFromCookie() {
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
    }
    catch (error) {
        console.error('Error parsing JWT:', error);
        return null;
    }
}
const jwt = getJwtFromCookie();
const userId = getUserIdFromJwt(jwt);

function createOrderAfterPayment() {
    const orderData = JSON.parse(getCookie('orderData'));
    if (!orderData) {
        console.error('Không tìm thấy dữ liệu đơn hàng.');
        return;
    }
    orderData.paymentStatus = 1;

    const xhr = new XMLHttpRequest();
    xhr.open('POST', 'https://localhost:7241/api/Order/create?printInvoice=false', true);
    xhr.setRequestHeader('Content-Type', 'application/json');

    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                const response = JSON.parse(xhr.responseText);
                console.log('Success:', response);
                Swal.fire({
                    title: 'Thành công!',
                    text: 'Đơn hàng của bạn đã được gửi thành công.',
                    icon: 'success',
                    confirmButtonText: 'OK',
                    timer: 5000,
                    timerProgressBar: true,
                    showConfirmButton: true,
                    willClose: () => {
                        if (orderData && orderData.orderDetails) {
                            const cartApiUrl = `https://localhost:7241/api/Cart/cart/user/${userId}`;
                            const cartXhr = new XMLHttpRequest();
                            cartXhr.open('GET', cartApiUrl, true);
                            cartXhr.onreadystatechange = function () {
                                if (cartXhr.readyState === XMLHttpRequest.DONE) {
                                    if (cartXhr.status === 200) {
                                        const cartData = JSON.parse(cartXhr.responseText);
                                        const cartId = cartData[0]?.id;
                                        if (cartId) {
                                            orderData.orderDetails.forEach(detail => {
                                                deleteCartOption(cartId, detail.idOptions);
                                            });
                                        } else {
                                            console.error('Không thể tìm thấy giỏ hàng.');
                                        }
                                    } else {
                                        console.error('Error fetching cart data:', cartXhr.responseText);
                                    }
                                }
                            };
                            cartXhr.send();
                        }
                        window.location.href = '/';
                    }
                });
                eraseCookie('orderData');
            } else {
                console.error('Error:', xhr.responseText);
                Swal.fire({
                    title: 'Lỗi!',
                    text: 'Đã xảy ra lỗi khi gửi đơn hàng. Vui lòng thử lại.',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            }
        }
    };

    xhr.send(JSON.stringify(orderData));
}

function deleteCartOption(cartId, idOptions) {
    const xhr = new XMLHttpRequest();
    const url = `https://localhost:7241/api/CartOptions/Delete/${cartId}/${idOptions}`;
    xhr.open('DELETE', url, true);

    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                console.log(`Deleted cart option ${idOptions} from cart ${cartId}`);
            } else {
                console.error('Error deleting cart option:', xhr.statusText);
            }
        }
    };

    xhr.send();
}

window.onload = function () {
    if (getParameterByName('vnp_ResponseCode') === '00') {
        createOrderAfterPayment();
    }
}

function setCookie(name, value, days) {
    let expires = "";
    if (days) {
        const date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

function getCookie(name) {
    const nameEQ = name + "=";
    const ca = document.cookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) === ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function eraseCookie(name) {
    document.cookie = name + '=; Max-Age=-99999999;';
}

function getParameterByName(name) {
    name = name.replace(/[\[\]]/g, "\\$&");
    const regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(window.location.href);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}
