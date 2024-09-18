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
function checkAuthentication() {
    if (!jwt || !userId) {
        window.location.href = '/login';
        return false;
    }
    return true;
}
checkAuthentication();

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
function navigateToUpdatePage(ID) {
    window.location.href = `/manager_orderstatus_verII/${ID}`;
}

function navigateToUpdateOrderStatus(ID) {
    window.location.href = `/manager_orderstatus_verII/${ID}`;
}
async function viewDetails(IDOrder) {
    try {
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
        document.getElementById('notes').innerText = data.notes;
        document.getElementById('modalemail').innerText = data.customerEmail;
        document.getElementById('modalshipaddess').innerText = data.shippingAddress;
        document.getElementById('modalshipaddress2').innerText = data.shippingAddressLine2 || "Không có";
        document.getElementById('modalcosts').innerText = data.cotsts.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
        document.getElementById('modaltotal').innerText = data.totalAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
        document.getElementById('modalpaymentmethod').innerText = translatePaymentMethod(data.paymentMethod);
        document.getElementById('modalpaymentstatus').innerHTML = translatePaymentStatus(data.paymentStatus);
        document.getElementById('modalshippingmethod').innerText = translateShippingMethod(data.shippingMethod);
        document.getElementById('modalorderstatus').innerHTML =
            `<span class="badge ${translateOrderStatus(data.orderStatus).class}">
                ${translateOrderStatus(data.orderStatus).text}
            </span>`;
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
                            <td>${history.billOfLadingCode}</td>
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
            return '<span style="background-color: red; color: yellow; padding: 5px; border-radius: 4px;">Chưa thanh toán</span>';
        case 1:
            return '<span style="background-color: green; color: white; padding: 5px; border-radius: 4px;">Đã thanh toán</span>';
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
            return { text: 'Đặt hàng thành công', class: 'bg-secondary' };
        case 1:
            return { text: 'Đã xác nhận', class: 'bg-warning' };
        case 2:
            return { text: 'Đang vận chuyển', class: 'bg-primary' };
        case 3:
            return { text: 'Hoàn thành', class: 'bg-success' };
        case 4:
            return { text: 'Đã hủy', class: 'bg-danger' };
        case 5:
            return { text: 'Đã trả lại', class: 'bg-info' };
        default:
            return { text: status, class: 'bg-secondary' };
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
    console.log('order', order);

    if (Array.isArray(order)) {
        order.forEach(item => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>${item.hexCode}</td>
                <td>${item.customerName}</td>
                <td>${item.customerPhone}</td>
                <td>${formatDateTime(item.createDate)}</td>
                <td>${translatePaymentMethod(item.paymentMethod)}</td>
                <td>${item.totalAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</td>
                <td>
                <span class="badge bg-success">${translateOrderType(item.orderType)}</span>                    
                <br>
                <small style="color: #6c757d; font-size: 12px;">${translatePaymentStatus(item.paymentStatus)}</small>
                </td>
                <td>
                    <span class="badge ${translateOrderStatus(item.orderStatus).class}">
                    ${translateOrderStatus(item.orderStatus).text}
                    </span>
                </td>
                <td>
                ${item.orderStatus === 0 ? `
                    <button class="btn" type="button" onclick="updateOrderStatus('1', '${item.id}')">
                        <i class="fas fa-hourglass-start"> Xác nhận đơn</i>
                    </button>
                ` : ''}

                ${item.orderStatus === 1 ? `
                    <button class="btn" type="button" onclick="updateOrderStatus('2', '${item.id}')">
                        <i class="fas fa-truck"> Vận chuyển</i>
                    </button>
                ` : ''}
                ${item.orderStatus === 2 ? `
                    <button class="btn" type="button" onclick="updateOrderStatus('3', '${item.id}')">
                        <i class="fas fa-box"> Đã giao hàng</i>
                    </button>
                ` : ''}
                 ${item.orderStatus === 0 || item.orderStatus === 1 ? `
                    <button class="btn btn-danger btn-sm cancel" type="button" onclick="updateOrderStatus('4','${item.id}', '${item.hexCode}')" title="Hủy đơn hàng">
                        <i class="fas fa-times-circle"></i> Hủy đơn
                    </button>
                ` : ''}
                <button class="btn btn-primary btn-sm edit" type="button" onclick="navigateToUpdatePage('${item.id}')" title="Sửa">
                    <i class="fa fa-cog"></i>
                </button>
                <button class="btn btn-primary btn-sm view" data-toggle="modal" data-target="#OrderModal" type="button" onclick="viewDetails('${item.id}')" title="Chi tiết">
                    <i class="fas fa-eye"></i>
                </button>               
                </td>

            `;
            orderListBody.appendChild(row);
        });
    } else {
        const row = document.createElement('tr');
        row.innerHTML = `
                <td>${item.hexCode}</td>
                <td>${item.customerName}</td>
                <td>${item.customerPhone}</td>
                <td>${formatDateTime(item.createDate)}</td>
                <td>${translatePaymentMethod(item.paymentMethod)}</td>
                <td>${item.totalAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</td>
                <td><span class="badge bg-success">${translateOrderType(item.orderType)}</span></td>
                <td>
                <span class="badge bg-success">${translateOrderType(item.orderType)}</span>                    
                <br>
                <small style="color: #6c757d; font-size: 12px;">${translatePaymentStatus(item.paymentStatus)}</small>
                </td>
                <td>
                <!-- Hiển thị nút và icon dựa trên trạng thái đơn hàng -->
                ${item.orderStatus === 0 ? `
                    <button class="btn" type="button" onclick="updateOrderStatus('1', '${item.id}')">
                        <i class="fas fa-hourglass-start"> Xác nhận đơn hàng</i>
                    </button>
                ` : ''}

                ${item.orderStatus === 1 ? `
                    <button class="btn" type="button" onclick="updateOrderStatus('2', '${item.id}')">
                        <i class="fas fa-truck"> Đã vận chuyển</i>
                    </button>
                ` : ''}
                ${item.orderStatus === 2 ? `
                    <button class="btn" type="button" onclick="updateOrderStatus('3', '${item.id}')">
                        <i class="fas fa-box"> Đã giao hàng</i>
                    </button>
                ` : ''}
                 ${item.orderStatus === 0 || item.orderStatus === 1 ? `
                     <button class="btn btn-danger btn-sm cancel" type="button" onclick="updateOrderStatus('4','${item.id}', '${item.hexCode}')" title="Hủy đơn hàng">
                        <i class="fas fa-times-circle"></i> Hủy đơn
                    </button>
                ` : ''}
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

    document.querySelectorAll('.trash').forEach(button => {
        button.addEventListener('click', function () {
            const orderId = this.getAttribute('data-id');
            cancelOrder(orderId);
        });
    });
}
function updateOrderStatus(status, idorder, hexCode) {
    const statusMap = {
        0: '',
        1: 'Xác nhận đơn hàng',
        2: 'Vận chuyển',
        3: 'Đã giao hàng',
        4: 'Hủy đơn'
    };

    const inputField = status === '2' || status === '4' ? 'text' : null;

    let inputPlaceholder = 'Nhập ghi chú (tùy chọn)';
    if (status === '2') {
        inputPlaceholder = 'Nhập mã vận đơn';
    } else if (status === '4') {
        inputPlaceholder = 'Nhập lý do hủy đơn';
    }

    let additionalInputField = null;
    let additionalInputPlaceholder = '';
    if (status === '4' && userId === null) {
        additionalInputField = 'text';
        additionalInputPlaceholder = 'Nhập số điện thoại đặt hàng';
    }

    const vietnameseStatus = statusMap[status];
    Swal.fire({
        title: 'Xác nhận',
        text: `Bạn có chắc chắn muốn cập nhật trạng thái đơn hàng thành "${vietnameseStatus}" không?`,
        input: inputField,
        inputPlaceholder: inputPlaceholder,
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Có',
        cancelButtonText: 'Hủy',
        inputAttributes: {
            autocapitalize: 'off'
        },
        preConfirm: (inputValue) => {
            if ((status === '2' || status === '4') && !inputValue) {
                Swal.showValidationMessage(status === '2' ? 'Mã vận đơn là bắt buộc!' : 'Lý do hủy đơn là bắt buộc!');
                return false;
            }
            return inputValue || '';
        }
    }).then((result) => {
        if (result.isConfirmed) {
            const billOfLadingCode = result.value ? String(result.value) : '';
            let additionalPhoneNumber = '';

            if (status === '4' && userId === null) {
                Swal.fire({
                    title: 'Nhập số điện thoại đặt hàng',
                    input: 'text',
                    inputPlaceholder: 'Nhập số điện thoại đặt hàng',
                    showCancelButton: true,
                    confirmButtonColor: '#3085d6',
                    cancelButtonColor: '#d33',
                    confirmButtonText: 'Xác nhận',
                    cancelButtonText: 'Hủy',
                    preConfirm: (phoneNumber) => {
                        if (!phoneNumber) {
                            Swal.showValidationMessage('Số điện thoại đặt hàng là bắt buộc!');
                            return false;
                        }
                        return phoneNumber;
                    }
                }).then((phoneResult) => {
                    if (phoneResult.isConfirmed) {
                        additionalPhoneNumber = phoneResult.value || '';
                        fetchOrderByHexCode(additionalPhoneNumber, billOfLadingCode, status, idorder, hexCode);
                    }
                });
            } else {
                fetchOrderByHexCode('', billOfLadingCode, status, idorder, hexCode);
            }
        }
    });

    function fetchOrderByHexCode(phoneNumber, billOfLadingCode, status, idorder, hexCode) {
        const apiUrl = `https://localhost:7241/api/Order/GetByHexCode/${hexCode}`;

        fetch(apiUrl)
            .then(response => response.json())
            .then(data => {
                const orderPhoneNumber = data.customerPhone;
                if (phoneNumber && phoneNumber !== orderPhoneNumber) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi!',
                        text: 'Số điện thoại không khớp!',
                        confirmButtonText: 'OK'
                    });
                    return;
                }

                sendUpdateRequest(status, idorder, billOfLadingCode, phoneNumber);
            })
            .catch(error => {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi!',
                    text: 'Có lỗi xảy ra khi lấy thông tin đơn hàng.',
                    confirmButtonText: 'OK'
                });
                console.error('Fetch Error:', error);
            });
    }

    function sendUpdateRequest(status, idorder, billOfLadingCode, phoneNumber) {
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
            Swal.close();
            if (xhr.status >= 200 && xhr.status < 300) {
                Swal.fire({
                    icon: 'success',
                    title: 'Thành công!',
                    text: `Trạng thái đơn hàng đã được cập nhật thành "${statusMap[status]}".`,
                    confirmButtonText: 'OK'
                }).then(() => {
                    window.location.reload();
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi!',
                    text: `${xhr.responseText}`,
                    confirmButtonText: 'OK'
                });
                console.error('Error:', xhr.responseText);
            }
        };

        xhr.onerror = function () {
            Swal.close();
            Swal.fire({
                icon: 'error',
                title: 'Lỗi!',
                text: 'Có lỗi xảy ra khi gửi yêu cầu.',
                confirmButtonText: 'OK'
            });
            console.error('Network Error:', xhr.responseText);
        };

        const requestData = {
            status: status,
            idUser: userId || 'Khách vãng lai',
            billOfLadingCode: billOfLadingCode,
            request: "UpdateOrderStatus"
        };
        console.log('requestData', requestData)
        xhr.send(JSON.stringify(requestData));

    }
}
function getOrderStatusButtons(orderStatus, idorder) {
    const statusButtons = {
        0: `
            <button class="btn" type="button" onclick="updateOrderStatus('1', '${idorder}', this.parentElement)">
                <i class="fas fa-hourglass-start"> Xác nhận đơn hàng</i>
            </button>
        `,
        1: `
            <button class="btn" type="button" onclick="updateOrderStatus('2', '${idorder}', this.parentElement)">
                <i class="fas fa-truck"> Đã vận chuyển</i>
            </button>
        `,
        2: `
            <button class="btn" type="button" onclick="updateOrderStatus('3', '${idorder}', this.parentElement)">
                <i class="fas fa-box"> Đã giao hàng</i>
            </button>
        `,
        3: '',
        4: `
            <button class="btn btn-danger btn-sm cancel" type="button" onclick="updateOrderStatus('4', '${idorder}', this.parentElement)" title="Hủy đơn hàng">
                <i class="fas fa-times-circle"></i> Hủy đơn
            </button>
        `
    };

    return `
        ${statusButtons[orderStatus]}
        <button class="btn btn-primary btn-sm edit" type="button" onclick="navigateToUpdatePage('${idorder}')" title="Sửa">
            <i class="fa fa-edit"></i>
        </button>
        <button class="btn btn-primary btn-sm view" data-toggle="modal" data-target="#OrderModal" type="button" onclick="viewDetails('${idorder}')" title="Chi tiết">
            <i class="fas fa-eye"></i>
        </button>
    `;
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
function getOrdersByType(orderType) {
    var xhr = new XMLHttpRequest();
    var url = 'https://localhost:7241/api/Order/GetByOrderType/' + orderType;

    xhr.open('GET', url, true);
    xhr.setRequestHeader('accept', '*/*');

    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
                var response = JSON.parse(xhr.responseText);
                orderList(Array.isArray(response) ? response : [response]);
            } else {
                console.error('Có lỗi xảy ra: ' + xhr.status);
            }
        }
    };

    xhr.send();
}
function filterOrders() {
    var selectElement = document.getElementById('orderTypeFilter');
    if (selectElement && selectElement.value) {
        getOrdersByType(selectElement.value);
    } else {
        Swal.fire({
            icon: 'warning',
            title: 'Chưa chọn trạng thái',
            text: 'Vui lòng chọn trạng thái đơn hàng để lọc kết quả.',
        });
    }
}
function getOrdersByStatus(orderType) {
    var xhr = new XMLHttpRequest();
    var url = 'https://localhost:7241/api/Order/GetByStatus/' + orderType;

    xhr.open('GET', url, true);
    xhr.setRequestHeader('accept', 'application/json');
    if (jwt) {
        xhr.setRequestHeader('Authorization', 'Bearer ' + jwt);
    }

    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
                try {
                    var response = JSON.parse(xhr.responseText);
                    console.log('Parsed Response:', response);
                    if (response.length === 0) {
                        Swal.fire({
                            icon: 'info',
                            title: 'Không có kết quả',
                            text: 'Không có đơn hàng nào phù hợp với tiêu chí lọc.',
                        });
                    } else {
                        orderList(Array.isArray(response) ? response : [response]);
                    }
                } catch (e) {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi!',
                        text: xhr.responseText,
                    });
                    console.error('JSON parse error:', e);
                }
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi!',
                    text: xhr.responseText,
                });
            }
        }
    };

    xhr.onerror = function () {
        Swal.fire({
            icon: 'error',
            title: 'Lỗi mạng!',
            text: 'Không thể kết nối đến máy chủ. Vui lòng kiểm tra kết nối mạng của bạn.',
        });
    };

    xhr.send();
}
function filterOrdersStatus() {
    var selectElement = document.getElementById('orderStatusFilter');

    if (selectElement && selectElement.value) {
        getOrdersByStatus(selectElement.value);
    } else {
        Swal.fire({
            icon: 'warning',
            title: 'Chưa chọn trạng thái',
            text: 'Vui lòng chọn trạng thái đơn hàng để lọc kết quả.',
        });
    }
}
