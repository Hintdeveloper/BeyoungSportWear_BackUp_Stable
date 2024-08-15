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

document.querySelectorAll('.status-item').forEach(item => {
    item.addEventListener('click', function () {
        document.querySelectorAll('.status-item').forEach(el => el.classList.remove('selected'));
        this.classList.add('selected');

        const statusValue = this.getAttribute('value');

        const xhr = new XMLHttpRequest();
        xhr.open('GET', `https://localhost:7241/api/Order/GetByStatus?OrderStatus=${statusValue}`, true);
        xhr.setRequestHeader('Accept', '*/*');
        xhr.onload = function () {
            const orderList = document.getElementById('order-list');
            orderList.innerHTML = ''; 

            if (xhr.status >= 200 && xhr.status < 300) {
                const data = JSON.parse(xhr.responseText);

                if (data.length > 0) {
                    const table = document.createElement('table');
                    table.classList.add('order-table');

                    const thead = document.createElement('thead');
                    thead.innerHTML = `
                        <tr>
                            <th>Mã</th>
                            <th>Ngày tạo</th>
                            <th>Trạng thái thanh toán</th>
                            <th>Điện thoại _ Gmail</th>
                            <th>Địa chỉ giao hàng</th>
                            <th>Tổng tiền</th>
                            <th>Chọn hành động</th>
                        </tr>
                    `;
                    table.appendChild(thead);

                    const tbody = document.createElement('tbody');
                    data.forEach(order => {
                        console.log(order);
                        const row = createOrderRow(order);
                        tbody.appendChild(row);
                    });

                    table.appendChild(tbody);

                    orderList.appendChild(table);
                } else {
                    orderList.innerHTML = '<p>Không có đơn hàng nào.</p>';
                }
            } else if (xhr.status === 404) {
                orderList.innerHTML = '<p>Không có đơn hàng nào.</p>';
            } else {
                console.error('Error fetching orders:', xhr.responseText);
                orderList.innerHTML = `<p>Không thể tải danh sách đơn hàng. Vui lòng thử lại sau.</p>`;
            }
        };
        xhr.onerror = function () {
            console.error('Request error');
            const orderList = document.getElementById('order-list');
            orderList.innerHTML = `<p>Không thể kết nối với máy chủ. Vui lòng thử lại sau.</p>`;
        };
        xhr.send();
    });
});
function createOrderRow(order) {
    const row = document.createElement('tr');
    row.innerHTML = `
        <td>${order.hexCode}</td>
        <td>${formatDateTime(order.createDate)}</td>
        <td>${translatePaymentStatus(order.paymentStatus)}</td>
        <td>${order.customerPhone}<br><small>${order.customerEmail}</small></td>
        <td>${order.shippingAddress}</td>
        <td>${order.totalAmount.toLocaleString()} VNĐ</td>
        <td>
            ${order.orderStatus === 0 ? '<button class="btn btn-warning" onclick="updateOrderStatus(\'Processing\', \'' + order.id + '\')">Xử lý</button>' : ''}
            ${order.orderStatus === 1 ? '<button class="btn btn-success" onclick="updateOrderStatus(\'Shipped\', \'' + order.id + '\')">Vận chuyển</button>' : ''}
            ${order.orderStatus === 2 ? '<button class="btn btn-primary" onclick="updateOrderStatus(\'Delivered\', \'' + order.id + '\')">Giao hàng</button>' : ''}
            ${order.orderStatus === 3 ? '' : ''}
            ${order.orderStatus === 4 ? '<button class="btn btn-danger">Đã hủy</button>' : ''}
        </td>
    `;
    return row;
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
function updateOrderStatus(status, idorder) {
    Swal.fire({
        title: 'Xác nhận',
        text: `Bạn có chắc chắn muốn cập nhật trạng thái đơn hàng thành "${status}" không?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Có',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire({
                title: 'Đang xử lý',
                text: 'Vui lòng chờ trong khi cập nhật đơn hàng...',
                allowOutsideClick: false,
                didOpen: () => {
                    Swal.showLoading();
                }
            });
            const apiUrl = `https://localhost:7241/api/Order/UpdateOrderStatus/${idorder}`;

            var xhr = new XMLHttpRequest();
            xhr.open('PUT', apiUrl, true);
            xhr.setRequestHeader('Content-Type', 'application/json');

            xhr.onload = function () {
                if (xhr.status >= 200 && xhr.status < 300) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Thành công!',
                        text: `Trạng thái đơn hàng đã được cập nhật thành "${status}".`,
                        confirmButtonText: 'OK'
                    }).then(() => {
                        const selectedStatus = document.querySelector('.status-item.selected').getAttribute('value');
                        document.querySelectorAll('.status-item').forEach(item => item.classList.remove('selected'));
                        document.querySelector(`.status-item[value="${selectedStatus}"]`).click();

                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi!',
                        text: 'Có lỗi xảy ra khi cập nhật trạng thái đơn hàng.',
                        confirmButtonText: 'OK'
                    });
                    console.error('Error:', xhr.statusText);
                }
            };

            xhr.onerror = function () {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi!',
                    text: 'Có lỗi xảy ra khi gửi yêu cầu.',
                    confirmButtonText: 'OK'
                });
                console.error('Network Error:', xhr.statusText);
            };

            xhr.send(JSON.stringify({
                status: status,
                idUser: userId
            }));
            console.log(status);
            console.log(userId);
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

                const orderList = document.getElementById('order-list');
                orderList.innerHTML = '';

                if (response) {
                    const table = document.createElement('table');
                    table.classList.add('order-table');

                    const thead = document.createElement('thead');
                    thead.innerHTML = `
                        <tr>
                            <th>Mã</th>
                            <th>Ngày tạo</th>
                            <th>Trạng thái thanh toán</th>
                            <th>Điện thoại _ Gmail</th>
                            <th>Địa chỉ giao hàng</th>
                            <th>Tổng tiền</th>
                            <th>Chọn hành động</th>
                        </tr>
                    `;
                    table.appendChild(thead);

                    const tbody = document.createElement('tbody');
                    const orders = Array.isArray(response) ? response : [response];
                    orders.forEach(order => {
                        const row = createOrderRow(order);
                        tbody.appendChild(row);
                    });

                    table.appendChild(tbody);
                    orderList.appendChild(table);
                } else {
                    orderList.innerHTML = '<p>Không có đơn hàng nào.</p>';
                }

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
            console.error('Lỗi khi lấy đơn hàng theo mã hex. Mã trạng thái:', xhr.status);
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

