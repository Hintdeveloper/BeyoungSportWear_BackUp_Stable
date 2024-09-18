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
        return null;
    }
}
const jwt = getJwtFromCookie();
const userId = getUserIdFromJwt(jwt);
function OrderList(orders) {
    const orderListBody = document.getElementById('orders');
    orderListBody.innerHTML = '';

    const orderArray = Array.isArray(orders) ? orders : [orders];
    if (orderArray.length === 0) { 
        const emptyRow = document.createElement('tr');
        emptyRow.innerHTML = `
            <td colspan="6" style="text-align: center; font-style: italic; color: gray;">
                Không có đơn hàng nào
            </td>`;
        orderListBody.appendChild(emptyRow);
        return;
    }
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
                <button class="btn btn-danger btn-sm trash" onclick="updateOrderStatus('4', '${order.id}', '${order.hexCode}')" type="button" data-id="${order.id}" id="cancelOrderButton" ${isCancelled ? "disabled" : ""}>
                    <i class="fa fa-ban"></i>
                </button>
                <button class="btn btn-primary btn-sm edit" type="button" title="Sửa" onclick="navigateToUpdatePageEdit('${order.id}')" ${isCancelled ? "disabled" : ""}>
                    <i class="fas fa-edit"></i>
                </button>
                    <button class="btn btn-warning btn-sm edit" onclick="viewDetails('${order.id}')" type="button" title="Sửa">
                        <i class="fa fa-eye"></i>
                    </button>
            </td>
        `;
        orderListBody.appendChild(row);
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
function fetchOrderList() {
    const xhr = new XMLHttpRequest();
    xhr.open('GET', `https://localhost:7241/api/Order/customer/${userId}`, true);

    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                try {
                    const orders = JSON.parse(xhr.responseText);
                    OrderList(orders);
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
            return 'Đặt hàng thành công';
        case 1:
            return 'Đã xác nhận';
        case 2:
            return 'Đang vận chuyển';
        case 3:
            return 'Hoàn thành';
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

function maskPhoneNumber(phoneNumber) {
    if (phoneNumber && phoneNumber.length > 4) {
        return phoneNumber.slice(0, 2) + '*****' + phoneNumber.slice(-3);
    }
    return phoneNumber;
}
function maskEmail(email) {
    if (email) {
        const [localPart, domain] = email.split('@');
        const maskedLocalPart = localPart.slice(0, 2) + '********' + localPart.slice(-1);
        return maskedLocalPart + '@' + domain.replace(/(?<=.{0}).+(?=.{2})/, '*****');
    }
    return email;
}
async function viewDetails(ID) {
    try {
        const response = await fetch(`https://localhost:7241/api/Order/GetByIDAsync/${ID}`);
        if (!response.ok) {
            throw new Error('Error fetching order details');
        }
        const data = await response.json();
        console.log(data)
        document.getElementById('modalcreate').innerText = formatDateTime(data.createDate);
        document.getElementById('modalvoucher').innerText = data.voucherCode || "Không có";
        document.getElementById('modalhexcode').innerText = data.hexCode;
        document.getElementById('modalcusname').innerText = data.customerName;
        if (!jwt) {
            document.getElementById('modalcusphone').innerText = maskPhoneNumber(data.customerPhone);
            document.getElementById('modalemail').innerText = maskEmail(data.customerEmail);
        } else {
            document.getElementById('modalcusphone').innerText = data.customerPhone;
            document.getElementById('modalemail').innerText = data.customerEmail;
        }
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

        $('#orderModal').modal('show');
    } catch (error) {
        console.error('Error fetching order details:', error.message);
    } finally {
        //hideLoader();
    }
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

