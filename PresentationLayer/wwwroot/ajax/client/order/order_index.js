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
console.log(userId);
function OrderList(orders) {
    const orderListBody = document.getElementById('orders');
    orderListBody.innerHTML = '';

    const orderArray = Array.isArray(orders) ? orders : [orders];

    orderArray.forEach(order => {
        const row = document.createElement('tr');
        const isCancelled = order.orderStatus === 2 || order.orderStatus === 3 || order.orderStatus === 4;

        row.innerHTML = `
            <td>${order.hexCode}</td>
            <td>${formatDateTime(order.createDate)}</td>
            <td>${translatePaymentMethod(order.paymentMethod)}<br><small style="color:red;">(${translatePaymentStatus(order.paymentStatus)})</small></td>
            <td>${order.totalAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</td>
            <td><span class="badge bg-success">${translateOrderStatus(order.orderStatus)}</span></td>
            <td>
                <button class="btn btn-danger btn-sm trash" type="button" data-id="${order.id}" id="cancelOrderButton" ${isCancelled ? "disabled" : ""}>
                    <i class="fa fa-ban"></i>
                </button>
                 <button class="btn btn-primary btn-sm edit" type="button" title="Sửa" onclick="navigateToUpdatePageEdit('${order.id}')">
                    <i class="fas fa-edit"></i>
                </button>
                <button class="btn btn-warning btn-sm edit" onclick="navigateToUpdatePage('${order.id}')" type="button" title="Sửa">
                    <i class="fa fa-eye"></i>
                </button>
            </td>
        `;
        orderListBody.appendChild(row);
    });

    document.querySelectorAll('.trash').forEach(button => {
        button.addEventListener('click', function () {
            const orderId = this.getAttribute('data-id');
            cancelOrder(orderId);
        });
    });
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
                        fetchOrderList();
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
function fetchOrderList() {
    const xhr = new XMLHttpRequest();
    xhr.open('GET', `https://localhost:7241/api/Order/customer/${userId}`, true);

    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                try {
                    const orders = JSON.parse(xhr.responseText);
                    OrderList(orders);
                    console.log(orders)
                } catch (error) {
                    console.error('Error parsing JSON:', error.message);
                }
            } else {
                console.error('Error fetching orders:', xhr.statusText);
            }
        }
    };

    xhr.onerror = function () {
        console.error('Request failed');
    };

    xhr.send();
}
fetchOrderList();
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
function navigateToUpdatePage(IDOrder) {
    window.location.href = `/order_details_user/${IDOrder}`;
}
function navigateToUpdatePageEdit(IDOrder) {
    window.location.href = `/order_update_user/${IDOrder}`;
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
                OrderList(Array.isArray(response) ? response : [response]);

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
        }
    };

    xhr.onerror = function () {
        console.error('Yêu cầu thất bại. Lỗi mạng.');
        Swal.fire({
            title: 'Lỗi mạng',
            text: 'Không thể thực hiện yêu cầu. Vui lòng kiểm tra kết nối mạng của bạn.',
            icon: 'error'
        });
    };

    xhr.send();
}

