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
console.log('userId', userId);
function checkStockAvailability(orderDetails) {
    return new Promise((resolve, reject) => {
        const stockCheckRequests = orderDetails.map(detail => {
            return new Promise((resolve, reject) => {
                const xhr = new XMLHttpRequest();
                xhr.open('GET', `https://localhost:7241/api/Options/GetByID/${detail.idOptions}`, true);
                xhr.onreadystatechange = function () {
                    if (xhr.readyState === XMLHttpRequest.DONE) {
                        if (xhr.status === 200) {
                            const response = JSON.parse(xhr.responseText);
                            console.log('response', response);

                            if (response.stockQuantity >= detail.quantity) {
                                resolve(true);
                            } else {
                                reject(`Ôi không! Bạn chậm tay qua sớm sản phẩm [${response.productName}]-[${response.keyCode}] đã hết hàng rồi!`);
                            }
                        } else {
                            reject('Lỗi kiểm tra hàng tồn kho.');
                        }
                    }
                };
                xhr.send();
            });
        });

        Promise.all(stockCheckRequests)
            .then(() => resolve(true))
            .catch(error => reject(error));
    });
}

async function createOrderAfterPayment() {
    showSwalLoading();
    const orderData = JSON.parse(getCookie('orderData'));
    if (!orderData) {
        console.error('Không tìm thấy dữ liệu đơn hàng.'); Swal.close();
        return;
    }
    if (!Array.isArray(orderData.orderDetailsCreateVM)) {
        console.error('Dữ liệu chi tiết đơn hàng không hợp lệ.'); Swal.close();
        return;
    }
    console.log(orderData.orderDetailsCreateVM);

    try {
        await checkStockAvailability(orderData.orderDetailsCreateVM);

        const cartApiUrl = `https://localhost:7241/api/Cart/cart/user/${userId}`;
        const cartXhr = new XMLHttpRequest();
        cartXhr.open('GET', cartApiUrl, true);
        cartXhr.onreadystatechange = function () {
            if (cartXhr.readyState === XMLHttpRequest.DONE) {
                if (cartXhr.status === 200) {
                    const cartData = JSON.parse(cartXhr.responseText);
                    const cartId = cartData[0]?.id;
                    if (cartId) {
                        const deleteRequests = orderData.orderDetailsCreateVM.map(detail => {
                            return new Promise((resolve, reject) => {
                                deleteCartOption(cartId, detail.idOptions, resolve, reject);
                            });
                        });

                        Promise.all(deleteRequests)
                            .then(() => {
                                orderData.paymentStatus = 1;
                                orderData.orderStatus = 1;

                                const xhr = new XMLHttpRequest();
                                xhr.open('POST', 'https://localhost:7241/api/Order/create?printInvoice=false', true);
                                xhr.setRequestHeader('Content-Type', 'application/json');

                                xhr.onreadystatechange = function () {
                                    if (xhr.readyState === XMLHttpRequest.DONE) {
                                        Swal.close();
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
                                                    window.location.href = '/';
                                                }
                                            });
                                            document.cookie = 'orderData=; path=/; max-age=0';
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
                            })
                            .catch((error) => {
                                console.error('Error deleting cart options:', error); Swal.close();
                                Swal.fire({
                                    title: 'Lỗi!',
                                    text: 'Đã xảy ra lỗi khi xóa các mục giỏ hàng. Vui lòng thử lại.',
                                    icon: 'error',
                                    confirmButtonText: 'OK'
                                });
                            });
                    } else {
                        console.error('Không thể tìm thấy giỏ hàng.'); Swal.close();
                    }
                } else {
                    console.error('Error fetching cart data:', cartXhr.responseText); Swal.close();
                }
            }
        };
        cartXhr.send();
    } catch (error) {
        Swal.close();
        const vnpResponseCode = '01'; 
        Swal.fire({
            title: 'Thanh toán không thành công',
            text: `Đơn hàng không thành công vì: ${error}. Mã lỗi: ${vnpResponseCode}`,
            icon: 'error',
            confirmButtonText: 'OK'
        });

        return;
    }
}

function deleteCartOption(cartId, idOptions, resolve, reject) {
    const xhr = new XMLHttpRequest();
    const url = `https://localhost:7241/api/CartOptions/Delete/${cartId}/${idOptions}`;
    xhr.open('DELETE', url, true);

    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                console.log(`Deleted cart option ${idOptions} from cart ${cartId}`);
                resolve();
            } else {
                console.error('Error deleting cart option:', xhr.responseText);
                reject(xhr.responseText);
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
function getParameterByName(name) {
    name = name.replace(/[\[\]]/g, "\\$&");
    const regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(window.location.href);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}
function showSwalLoading() {
    Swal.fire({
        title: 'Đang xử lý',
        text: 'Vui lòng chờ...',
        allowOutsideClick: false,  
        showConfirmButton: false,  
        didOpen: () => {
            Swal.showLoading(); 
        }
    });
}
function getParameterByName(name) {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get(name);
}

document.addEventListener("DOMContentLoaded", function () {
    document.getElementById('transaction_merchant').innerText = getParameterByName('vnp_TmnCode');
    document.getElementById('time_transaction').innerText = getParameterByName('vnp_PayDate');
    document.getElementById('amount').innerText = (getParameterByName('vnp_Amount') / 100).toLocaleString() + " VND";
    document.getElementById('transaction_response_code').innerText = getParameterByName('vnp_ResponseCode');
    document.getElementById('message').innerText = decodeURIComponent(getParameterByName('vnp_OrderInfo'));
    document.getElementById('code_transaction').innerText = getParameterByName('vnp_TransactionNo');
    document.getElementById('bank').innerText = getParameterByName('vnp_BankCode');
});
document.addEventListener("DOMContentLoaded", function () {
    const responseCode = getParameterByName('vnp_ResponseCode');
    const transactionStatus = getParameterByName('vnp_TransactionStatus');

    if (responseCode === '00' && transactionStatus === '00') {
        document.getElementById('message').innerText = "Giao dịch được thực hiện thành công. Cảm ơn quý khách đã sử dụng dịch vụ.";
    } else {
        document.getElementById('message').innerText = "Có lỗi xảy ra trong quá trình xử lý. Mã lỗi: " + responseCode;
    }
});
