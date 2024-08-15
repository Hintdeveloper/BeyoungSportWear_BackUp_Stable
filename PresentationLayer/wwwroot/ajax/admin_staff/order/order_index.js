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




async function fetchOrder() {
    try {
        const response = await fetch('https://localhost:7241/api/Order/allactive');
        if (!response.ok) {
            throw new Error('Error fetching orders');
        }
        const orders = await response.json();
        orderList(orders);
    } catch (error) {
        console.error('Error fetching orders:', error.message);
    } finally {
    }
}
function navigateToUpdatePage(productId) {
    window.location.href = `/managerupdate_order/${productId}`;
}
async function viewDetails(IDOrder) {
    try {
        //showLoader();
        const response = await fetch(`https://localhost:7241/api/Order/GetByIDAsync/${IDOrder}`);
        if (!response.ok) {
            throw new Error('Error fetching order details');
        }
        const data = await response.json();

        document.getElementById('modalcreate').innerText = formatDateTime(data.createDate);
        document.getElementById('modalvoucher').innerText = data.voucherCode || "Không có";
        document.getElementById('modalhexcode').innerText = data.hexCode;
        document.getElementById('modalcusname').innerText = data.customerName;
        document.getElementById('modalcusphone').innerText = data.customerPhone;
        document.getElementById('modalemail').innerText = data.customerEmail;
        document.getElementById('modalshipaddess').innerText = data.shippingAddress;
        document.getElementById('modalshipaddress2').innerText = data.shippingAddressLine2 || "Không có";
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
                    const row = `
                        <tr>
                            <td>${detail.productName || 'N/A'}</td>
                            <td>${detail.sizeName || 'N/A'}</td>
                            <td>${detail.colorName || 'N/A'}</td>
                            <td>${detail.quantity}</td>
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

        //$('#OrderModal').modal('show');
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

fetchOrder();
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
function orderList(order) {
    const orderListBody = document.getElementById('orderListBody');
    orderListBody.innerHTML = '';
    orders.forEach(order => {
        const isCancelled = order.orderStatus === 3 || order.orderStatus === 4;

    if (Array.isArray(order)) {
        order.forEach(item => {
            const isCancelled = item.trackingCheck === true || item.orderStatus === 4;

        const row = document.createElement('tr');
        row.innerHTML = `
                <td width="10"><input type="checkbox" name="check1" value="${item.id}"></td>
                <td>${item.hexCode}</td>
                <td>${item.customerName}</td>
                <td>${item.customerPhone}</td>
                <td>${formatDateTime(item.createDate)}</td>
                <td>${translatePaymentMethod(item.paymentMethod)}</td>
                <td>${item.totalAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</td>
                <td><span class="badge bg-success">${translateOrderType(item.orderType)}</span></td>
                <td><span class="badge bg-success">${translateOrderStatus(item.orderStatus)}</span></td>
             <td>
                    <button class="btn btn-primary btn-sm trash" onclick="markAsTrackingCheck('${item.id}', '${userId}')" type="button" ${isCancelled ? "disabled" : ""}>
                    <i class="fa fa-check"></i>
                </button>
                    <button class="btn btn-danger btn-sm trash" type="button" data-id="${item.id}" id="cancelOrderButton">
                        <i class="fa fa-ban"></i>
                    </button>
                    <button class="btn btn-primary btn-sm edit" type="button" onclick="navigateToUpdatePage('${item.id}')" title="Sửa">
                    <i class="fa fa-edit"></i>
                </button>
                    <button class="btn btn-primary btn-sm view" data-toggle="modal" data-target="#OrderModal" type="button" onclick="viewDetails('${item.id}')" title="Chi tiết">
                    <i class="fas fa-eye"></i>
                </button>
            </td>
        `;
        orderListBody.appendChild(row);
    });
    } else {
        const item = order;
        const isCancelled = item.trackingCheck === true || item.orderStatus === 4;

        const row = document.createElement('tr');
        row.innerHTML = `
            <td width="10"><input type="checkbox" name="check1" value="${item.id}"></td>
            <td>${item.hexCode}</td>
            <td>${item.customerName}</td>
            <td>${item.customerPhone}</td>
            <td>${formatDateTime(item.createDate)}</td>
            <td>${translatePaymentMethod(item.paymentMethod)}</td>
            <td>${item.totalAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</td>
            <td><span class="badge bg-success">${translateOrderType(item.orderType)}</span></td>
            <td><span class="badge bg-success">${translateOrderStatus(item.orderStatus)}</span></td>
            <td>
                <button class="btn btn-primary btn-sm trash" onclick="markAsTrackingCheck('${item.id}', '${userId}')" type="button" ${isCancelled ? "disabled" : ""}>
                    <i class="fa fa-check"></i>
                </button>
                <button class="btn btn-danger btn-sm trash" type="button" data-id="${item.id}" id="cancelOrderButton">
                    <i class="fa fa-ban"></i>
                </button>
                <button class="btn btn-primary btn-sm edit" type="button" onclick="navigateToUpdatePage('${item.id}')" title="Sửa">
                    <i class="fa fa-edit"></i>
                </button>
                <button class="btn btn-primary btn-sm view" data-toggle="modal" data-target="#OrderModal" type="button" onclick="viewDetails('${item.id}')" title="Chi tiết">
                    <i class="fas fa-eye"></i>
                </button>
            </td>
        `;
        orderListBody.appendChild(row);
}
let currentOrderId = null;

    document.querySelectorAll('.trash').forEach(button => {
        button.addEventListener('click', function () {
            const orderId = this.getAttribute('data-id');
            cancelOrder(orderId);
        });
    });
}
function markAsTrackingCheck(orderId, userId) {
    const xhr = new XMLHttpRequest();
    const url = `https://localhost:7241/api/Order/MarkAsTrackingCheckAsync/${orderId}/${userId}`;
    xhr.open("PUT", url, true);
    xhr.setRequestHeader("Accept", "*/*");

    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
                Swal.fire({
                    icon: 'success',
                    title: 'Thành công',
                    text: 'Đơn hàng đã được đánh dấu thành công!'
                }).then(() => {
                    fetchOrder(); 
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi',
                    text: 'Có lỗi xảy ra khi đánh dấu đơn hàng.' + xhr.responseText,  
                });
            }
        }
    };
    xhr.send();
}
function cancelOrder(orderId) {
    Swal.fire({
        title: 'Xác nhận',
        text: "Bạn có chắc chắn muốn hủy đơn hàng này không?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Có',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            const xhr = new XMLHttpRequest();
            xhr.open('PUT', `https://localhost:7241/api/Order/MarkAsCancelled/${orderId}/${userId}`, true);
            xhr.setRequestHeader('Content-Type', 'application/json');
            xhr.setRequestHeader('Authorization', `Bearer ${getJwtFromCookie()}`);

            xhr.onreadystatechange = function () {
                if (xhr.readyState === XMLHttpRequest.DONE) {
                    if (xhr.status === 200) {
                        Swal.fire(
                            'Thành công!',
                            'Đơn hàng đã được hủy.',
                            'success'
                        );
                        fetchOrder();
                    } else {
                        Swal.fire(
                            'Lỗi!',
                            'Có lỗi xảy ra khi hủy đơn hàng.',
                            'error'
                        );
                        console.error('Error:', xhr.statusText);
                    }
                }
            };

            xhr.onerror = function () {
                Swal.fire(
                    'Lỗi!',
                    'Có lỗi xảy ra khi hủy đơn hàng.',
                    'error'
                );
                console.error('Request failed');
            };

            xhr.send();
        }
    });
}
function searchOrder() {
    var hexCode = document.getElementById('searchHexCode').value;

    if (!hexCode) {
        Swal.fire({
            title: 'Lỗi',
            text: 'Vui lòng nhập mã HexCode.',
            icon: 'warning'
        });
        return;
    }

            var xhr = new XMLHttpRequest();
    var url = `https://localhost:7241/api/Order/GetByHexCode/${encodeURIComponent(hexCode)}`;

    xhr.open('GET', url, true);
    xhr.setRequestHeader('Accept', 'application/json');

            xhr.onload = function () {
                if (xhr.status >= 200 && xhr.status < 300) {
            try {
                var response = JSON.parse(xhr.responseText);
                orderList(Array.isArray(response) ? response : [response]);

                Swal.fire({
                    title: 'Thành công',
                    text: 'Đơn hàng đã được lấy thành công!',
                    icon: 'success'
                });
            } catch (e) {
                console.error('Lỗi phân tích phản hồi:', xhr.responseText);
                    Swal.fire({
                    title: 'Lỗi',
                    text: 'Lỗi khi phân tích phản hồi.',
                    icon: 'error'
                    });
            }
                } else {
            console.error('Lỗi khi lấy đơn hàng theo mã hex. Mã trạng thái:', xhr.responseText);
                    Swal.fire({
                title: 'Lỗi',
                text: 'Không thể lấy đơn hàng. Vui lòng thử lại sau.',
                icon: 'error'
                    });
                    console.error('Error:', xhr.statusText);
                }
            };

            xhr.onerror = function () {
        console.error('Yêu cầu thất bại. Lỗi mạng.');
                Swal.fire({
            title: 'Lỗi mạng',
            text: 'Không thể thực hiện yêu cầu. Vui lòng kiểm tra kết nối mạng của bạn.',
            icon: 'error'
                });
                console.error('Network Error:', xhr.statusText);
            };

    xhr.send();
}

function getOrdersByType(orderType) {
    var xhr = new XMLHttpRequest();
    var url = 'https://localhost:7241/api/Order/GetByOrderType/' + orderType;

    xhr.open('GET', url, true);
    xhr.setRequestHeader('accept', '*/*');

    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                var response = JSON.parse(xhr.responseText);
                orderList(Array.isArray(response) ? response : [response]);

                console.log(response);
            } else {
                console.error('Có lỗi xảy ra: ' + xhr.status);
        }
    });
}
    };

    xhr.send();
            }

function filterOrders() {
    var selectElement = document.getElementById('orderTypeFilter');
    var selectedValue = selectElement.value;
    getOrdersByType(selectedValue);
    }
});
