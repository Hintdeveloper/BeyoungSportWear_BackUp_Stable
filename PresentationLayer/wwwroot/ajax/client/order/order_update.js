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
    }
    catch (error) {
        console.error('Error parsing JWT:', error);
        return null;
    }
}
const jwt = getJwtFromCookie();
const userId = getUserIdFromJwt(jwt);
document.addEventListener('DOMContentLoaded', function () {
    var currentUrl = window.location.href;
    var urlParts = currentUrl.split('/');
    var ID = urlParts[urlParts.length - 1];
    viewDetails(ID);
    document.getElementById('btnUpdateOrder').addEventListener('click', function (event) {
        event.preventDefault();

        Swal.fire({
            title: 'Bạn có chắc chắn muốn cập nhật đơn hàng?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Có, cập nhật!',
            cancelButtonText: 'Không'
        }).then((result) => {
            if (result.isConfirmed) {
                const apiUrl = `https://localhost:7241/api/Order/UpdateOrder/${ID}/${userId}`;

                const orderDetails = Array.from(document.querySelectorAll('.order-details')).map(row => {
                    const idOptions = document.getElementById('idoptions').value;
                    const quantity = document.getElementById('quantity').value;

                    return {
                        idOptions: idOptions,
                        quantity: parseInt(quantity)
                    };
                });

                const orderData = {
                    modifiedBy: userId,
                    idUser: userId,
                    customerName: document.getElementById('modalcusname').value,
                    customerPhone: document.getElementById('modalcusphone').value,
                    customerEmail: document.getElementById('modalemail').value,
                    shippingAddressLine2: document.getElementById('modalshipaddress2').value,
                    cotsts: 0,
                    orderDetails: orderDetails
                };
                console.log(orderData)
                var xhr = new XMLHttpRequest();
                xhr.open('PUT', apiUrl, true);
                xhr.setRequestHeader('Content-Type', 'application/json');

                xhr.onload = function () {
                    if (xhr.readyState === xhr.DONE) {
                        if (xhr.status === 200) {
                            Swal.fire({
                                title: 'Thành công!',
                                text: 'Cập nhật đơn hàng thành công.',
                                icon: 'success'
                            }).then(() => {
                                window.location.href = '/';
                            });
                        } else {
                            Swal.fire({
                                title: 'Lỗi!',
                                text: 'Có lỗi xảy ra: ' + xhr.responseText,
                                icon: 'error'
                            });
                        }
                    }
                };

                xhr.onerror = function () {
                    Swal.fire({
                        title: 'Lỗi!',
                        text: 'Có lỗi kết nối đến máy chủ.' + xhr.responseText,
                        icon: 'error'
                    });
                };

                xhr.send(JSON.stringify(orderData));
            }
        });
    });
});
async function viewDetails(ID) {
    try {
        //showLoader();
        const response = await fetch(`https://localhost:7241/api/Order/GetByIDAsync/${ID}`);
        if (!response.ok) {
            throw new Error('Error fetching order details');
        }
        const data = await response.json();
        console.log(data)
        document.getElementById('modalcreate').innerText = formatDateTime(data.createDate);
        document.getElementById('modalvoucher').innerText = data.voucherCode || "Không có";
        document.getElementById('modalhexcode').innerText = data.hexCode;
        document.getElementById('modalcusname').value = data.customerName;
        document.getElementById('modalcusphone').value = data.customerPhone;
        document.getElementById('modalemail').value = data.customerEmail;
        document.getElementById('modalshipaddess').value = data.shippingAddress;
        document.getElementById('modalshipaddress2').value = data.shippingAddressLine2 || "Không có";
        document.getElementById('modalcosts').innerText = data.cotsts.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
        document.getElementById('modaltotal').innerText = data.totalAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
        document.getElementById('modalpaymentmethod').innerText = translatePaymentMethod(data.paymentMethod);
        document.getElementById('modalpaymentstatus').innerText = translatePaymentStatus(data.paymentStatus);
        document.getElementById('modalshippingmethod').innerText = translateShippingMethod(data.shippingMethod);
        document.getElementById('modalorderstatus').innerText = translateOrderStatus(data.orderStatus);
        document.getElementById('modalordertype').innerText = translateOrderType(data.orderType);

        const orderBody = document.getElementById('orderBody');
        if (orderBody) {
            orderBody.innerHTML = '';
            if (data.orderDetailsVM && data.orderDetailsVM.length > 0) {
                data.orderDetailsVM.forEach(detail => {
                    var row = document.createElement('tr');
                    row.classList.add('order-detail');
                    row.setAttribute('data-id-options', detail.idOptions);
                    row = `
                        <tr>
                            <input value="${detail.idOptions}" id="idoptions" hidden></input>
                            <td>${detail.productName || 'N/A'}</td>
                            <td>${detail.sizeName || 'N/A'}</td>
                            <td>${detail.colorName || 'N/A'}</td>
                            <td><input id="quantity" value="${detail.quantity}"></input></td>
                            <td>${detail.unitPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</td>
                            <td>${detail.totalAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</td>
                        </tr>
                    `;
                    orderBody.insertAdjacentHTML('beforeend', row);
                });
            } else {
                orderBody.innerHTML = '<tr><td colspan="6">Không có chi tiết đơn hàng</td></tr>';
            }
        } else {
            console.error('Không tìm thấy phần tử có id "orderBody" trong DOM.');
        }

        const orderhistoryBody = document.getElementById('orderhistory_body');
        if (orderhistoryBody) {
            orderhistoryBody.innerHTML = '';
            if (data.orderHistoryVM && data.orderHistoryVM.length > 0) {
                data.orderHistoryVM.forEach(history => {
                    const formattedEditingHistory = history.editingHistory.replace(/\n/g, '<br>');
                    const formattedChangeDetails = history.changeDetails.replace(/\n/g, '<br>');

                    const row = `
                        <tr>
                            <td>${formatDateTime(history.changeDate)}</td>
                            <td>${formattedEditingHistory}</td>
                            <td>${formattedChangeDetails}</td>
                        </tr>
                    `;
                    orderhistoryBody.insertAdjacentHTML('beforeend', row);
                });
            } else {
                orderhistoryBody.innerHTML = '<tr><td colspan="6">Không có chi tiết đơn hàng</td></tr>';
            }
        } else {
            console.error('Không tìm thấy phần tử có id "orderhistory_body" trong DOM.');
        }
    } catch (error) {
        console.error('Error fetching order details:', error.message);
    } finally {
        //hideLoader();
    }
}
function translatePaymentMethod(method) {
    switch (method) {
        case 0:
            return 'Chuyển khoản ngân hàng';
        case 1:
            return 'Tiền mặt khi giao hàng';
        default:
            return method;
    }
}
function translateShippingMethod(method) {
    switch (method) {
        case 0:
            return 'Nhận tại cửa hàng';
        case 1:
            return 'Giao hàng tiêu chuẩn';
        default:
            return method;
    }
}
function translatePaymentStatus(status) {
    switch (status) {
        case 0:
            return 'Chưa thanh toán';
        case 1:
            return 'Đã thanh toán';
        default:
            return status;
    }
}
function translateOrderType(type) {
    switch (type) {
        case 0:
            return 'Đặt hàng online';
        case 1:
            return 'Đặt hàng tại cửa hàng';
        default:
            return type;
    }
}
function translateOrderStatus(status) {
    switch (status) {
        case 0:
            return 'Chưa giải quyết';
        case 1:
            return 'Đang xử lý';
        case 2:
            return 'Đã vận chuyển';
        case 3:
            return 'Đã giao hàng';
        case 4:
            return 'Đã hủy';
        case 5:
            return 'Đã trả lại';
        default:
            return status;
    }
}
function formatDateTime(dateTimeString) {
    const date = new Date(dateTimeString);
    const options = {
        year: 'numeric',
        month: 'numeric',
        day: 'numeric',
        hour: 'numeric',
        minute: 'numeric'
    };
    const hour = date.getHours();
    const period = hour >= 12 ? "chiều" : "sáng";

    return date.toLocaleString('vi-VN', options) + ` ${period}`;
}

function formatLink(text) {
    var tempDiv = document.createElement('div');
    tempDiv.innerHTML = text;
    var links = tempDiv.querySelectorAll('a');
    links.forEach(link => {
        link.style.fontWeight = 'bold';
        link.addEventListener('click', function (event) {
            event.preventDefault();
            var userID = link.getAttribute('href');
            getUserInfoByID(userID);
        });
    });
    return tempDiv.innerHTML;
}

function handleUserLinkClick(event) {
    event.preventDefault();
    var userID = this.getAttribute('data-user-id');
    getUserInfoByID(userID);
}

function addClickEventToUserLinks() {
    document.querySelectorAll('a[data-user-id]').forEach(link => {
        link.style.fontWeight = 'bold';
        link.removeEventListener('click', handleUserLinkClick);
        link.addEventListener('click', handleUserLinkClick);
    });
}

const observer = new MutationObserver(addClickEventToUserLinks);
observer.observe(document.body, { childList: true, subtree: true });

addClickEventToUserLinks();

function getUserInfoByID(userID) {
    var apiUrl = `https://localhost:7241/api/ApplicationUser/GetInformationUser/${userID}`;

    var xhr = new XMLHttpRequest();
    xhr.open('GET', apiUrl, true);

    xhr.onload = function () {
        if (xhr.readyState === xhr.DONE) {
            if (xhr.status === 200) {
                var user = JSON.parse(xhr.responseText);

                document.getElementById('userName').textContent = user.username;
                document.getElementById('firstAndLastName').textContent = user.firstAndLastName;
                document.getElementById('userEmail').textContent = user.email;
                document.getElementById('userPhoneNumber').textContent = user.phoneNumber;
                document.getElementById('roleName').textContent = user.roleName;
                document.getElementById('userImage').src = user.images || 'default-image-url.jpg';

                $('#editUserModal').modal('show');
            } else {
                console.error('Request failed. Status:', xhr.status);
            }
        }
    };

    xhr.onerror = function () {
        console.error('Request failed. Network error.');
    };

    xhr.send();
}

