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
var hexCode;
var currentUrl = window.location.href;
var urlParts = currentUrl.split('/');
var ID = urlParts[urlParts.length - 1];
console.log(ID)
handleApiCall(ID);

function updateStatus(orderStatus, orderHistories) {
    var steps = document.querySelectorAll('.step');
    steps.forEach(function (step) {
        step.classList.remove('active');
        step.classList.remove('cancelled');
    });

    var statusMap = {
        0: 'step-0',
        1: 'step-1',
        2: 'step-2',
        3: 'step-3',
        4: 'cancel-order'
    };

    if (orderStatus >= 0 && orderStatus <= 3) {
        for (let i = 0; i <= orderStatus; i++) {
            let stepId = `step-${i}`;
            let step = document.getElementById(stepId);
            if (step) {
                step.classList.add('active');
            }
        }
    } else if (orderStatus === 4) {
        steps.forEach((step) => {
            if (step.id !== 'cancel-order') {
                step.classList.add('cancelled');
            }
        });
        const iconElement = document.querySelector('.icon i');
        if (iconElement) {
            iconElement.style.color = 'green';
        }
    } else {
        var currentStepId = statusMap[orderStatus] || 'step-0';
        var step = document.getElementById(currentStepId);
        if (step) {
            step.classList.add('active');
        }
    }

    const timeIds = {
        0: 'time_success',
        1: 'time_processing',
        2: 'time_ship',
        3: 'time_done',
        4: 'time_cancel'
    };

    for (let i = 0; i <= 4; i++) {
        const timeId = timeIds[i];
        if (timeId) {
            const timeElement = document.getElementById(timeId);
            if (timeElement) {
                const histories = orderHistories.filter(h => h.changeType === i.toString());
                if (histories.length > 0) {
                    const timeList = histories.map(h => formatDate(new Date(h.changeDate)));
                    timeElement.textContent = timeList.join(', ');
                } else {
                    timeElement.textContent = 'Chưa có dữ liệu';
                }
            }
        }
    }
}
function viewOrderDetails(orderData) {
    document.getElementById('hexcode').textContent = orderData.hexCode;
    hexCode = orderData.hexCode;

    document.getElementById('customer_name').value = orderData.customerName;
    document.getElementById('customer_phone').value = orderData.customerPhone;
    document.getElementById('customer_gmail').value = orderData.customerEmail;
    document.getElementById('shippingadress').textContent = orderData.shippingAddress;
    document.getElementById('shippingadress2').value = orderData.shippingAddressLine2;
    document.getElementById('total_price').textContent = `${orderData.totalAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })} (Phí giao hàng:${orderData.cotsts.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })})`;
    document.getElementById('paymentstatus').textContent = translatePaymentStatus(orderData.paymentStatus);
    document.getElementById('orderstatus').textContent = translateOrderStatus(orderData.orderStatus);
    document.getElementById('shippingmethod').textContent = translateShippingMethod(orderData.shippingMethod);
    document.getElementById('orrdertype').textContent = translateOrderType(orderData.orderType);

    if (orderData.orderDetailsVM.length > 0) {
        const productContainer = document.getElementById('product_list');
        productContainer.innerHTML = '';

        orderData.orderDetailsVM.forEach(details => {
            const productHTML = `
        <li class="col-md-4" data-id-options="${details.idOptions}" style="padding: 10px; box-sizing: border-box;">
            <figure class="itemside"
                    style="display: flex; align-items: center; border: 1px solid #ddd; border-radius: 5px; padding: 10px; background-color: #fff;">
                <div class="aside" style="flex-shrink: 0; margin-right: 10px;">
                    <img src="${details.imageURL}" class="img-sm" style="max-width: 100%; height: auto; border-radius: 5px;">
                </div>
                <figcaption class="info" style="flex: 1;">
                    <p class="title" style="margin: 0 0 10px 0; font-size: 1.2em; font-weight: bold;">
                        ${details.productName}<br>${details.sizeName}, ${details.colorName} 
                    </p>
                    <div class="d-flex align-items-center" style="margin-bottom: 10px;">
                        <label for="quantity_${details.id}" class="me-2" style="margin: 0;">Số lượng:</label>
                        <input type="number" id="quantity_${details.id}" name="quantity" value="${details.quantity}" min="1"
                               class="form-control form-control-sm"
                               style="width: 80px; padding: 5px; margin: 0;"
                               oninput="updateTotalPrice('${details.id}', ${details.unitPrice})">
                    </div>
                    <span class="text-muted" style="font-size: 1.2em; font-weight: bold;">
                        Giá bán: ${details.unitPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}                           
                    </span>
                    <br>
                    <span class="text-muted" style="font-size: 1.2em; font-weight: bold;">
                        Tổng giá: <span id="total_${details.id}">${details.totalAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</span>
                    </span>
                </figcaption>
            </figure>
        </li>
    `;
            productContainer.insertAdjacentHTML('beforeend', productHTML);
        });

    }


    updateStatus(orderData.orderStatus, orderData.orderHistoryVM);
    const buttonContainer = document.getElementById('button_update_status');
    buttonContainer.innerHTML = `
    ${orderData.orderStatus === 0 ? `
        <button class="btn btn-primary" type="button" onclick="updateOrderStatus('1', '${orderData.id}')">
            <i class="fas fa-hourglass-start" style="color: #FFFF00;"> Xác nhận đơn hàng</i>
        </button>
    ` : ''}

    ${orderData.orderStatus === 1 ? `
        <button class="btn btn-primary" type="button" onclick="updateOrderStatus('2', '${orderData.id}')">
            <i class="fas fa-truck" style="color: #FFFF00;"> Đã vận chuyển</i>
        </button>
    ` : ''}

    ${orderData.orderStatus === 2 ? `
        <button class="btn btn-primary" type="button" onclick="updateOrderStatus('3', '${orderData.id}')">
            <i class="fas fa-box" style="color: #FFFF00;"> Đã giao hàng</i>
        </button>
    ` : ''}

    ${orderData.orderStatus === 0 || orderData.orderStatus === 1 ? `
        <button class="btn btn-danger btn-sm cancel" type="button" onclick="updateOrderStatus('4','${orderData.id}')" title="Hủy đơn hàng">
            <i class="fas fa-times-circle"></i> Hủy đơn
        </button>
    ` : ''}

        <button
        type="button"
        id="btn_view_orderhistory"
        style="
            background-color: blue;
            color: white;
            border: none;
            padding: 10px 10px;
            font-size: 0.875rem;
            border-radius: 4px;
            display: inline-block;
            align-items: center;
            justify-content: center;
            cursor: pointer;
        ">
        <i class="fas fa-times-circle"></i> Xem lịch sử đơn hàng
        </button>
    `;
    document.getElementById('btn_view_orderhistory').addEventListener('click', function () {
        fetchOrderHistory(orderData.id);
    });
}
document.getElementById('btn_printf_order_pdf').addEventListener('click', function () {
    printf_order_pdf(hexCode);
});
function printf_order_pdf(hexCode) {
    Swal.fire({
        title: 'Xác nhận in hóa đơn?',
        text: "Bạn có chắc chắn muốn in hóa đơn này?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'In'
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire({
                title: 'Đang xử lý',
                text: 'Vui lòng chờ...',
                icon: 'info',
                allowOutsideClick: false,
                showConfirmButton: false,
                willOpen: () => {
                    Swal.showLoading();
                }
            });

            var xhr = new XMLHttpRequest();
            xhr.open('GET', `https://localhost:7241/api/Order/printf_order_pdf/${hexCode}`, true);
            xhr.setRequestHeader('Accept', '*/*');
            xhr.responseType = 'json';

            xhr.onload = function () {
                Swal.close();

                if (xhr.status === 200) {
                    var response = xhr.response;
                    window.open(`http://127.0.0.1:8080/${response.fileName}`, '_blank');
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi',
                        text: 'Không thể in hóa đơn. Vui lòng thử lại sau!'
                    });
                }
            };

            xhr.onerror = function () {
                Swal.close();

                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi',
                    text: 'Không thể kết nối đến máy chủ. Vui lòng kiểm tra lại kết nối mạng!'
                });
            };

            xhr.send();
        }
    });
}
function updateTotalPrice(productId, unitPrice) {
    const quantityInput = document.getElementById(`quantity_${productId}`);
    const totalPriceElement = document.getElementById(`total_${productId}`);

    const newQuantity = parseInt(quantityInput.value);
    const newTotalPrice = newQuantity * unitPrice;

    totalPriceElement.textContent = newTotalPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
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
function formatDate(date) {
    return date.toLocaleTimeString() + ' ' + date.toLocaleDateString();
}
function handleApiCall(orderId) {
    const apiUrl = `https://localhost:7241/api/Order/GetByIDAsync/${orderId}`;
    const xhr = new XMLHttpRequest();
    xhr.open('GET', apiUrl, true);
    xhr.setRequestHeader('accept', '*/*');

    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
                const data = JSON.parse(xhr.responseText);

                viewOrderDetails(data);
            } else {
                console.error('Error fetching order details:', xhr.status, xhr.statusText);
            }
        }
    };

    xhr.send();
}
document.getElementById('btn_update_order').addEventListener('click', function (event) {
    event.preventDefault();

    const customerPhone = document.getElementById('customer_phone').value;
    const customerEmail = document.getElementById('customer_gmail').value;

    const phoneRegex = /^(0|\+84)[3|5|7|8|9][0-9]{8}$/;
    const emailRegex = /^[a-zA-Z0-9._%+-]+@gmail\.com$/;

    if (!phoneRegex.test(customerPhone)) {
        Swal.fire({
            title: 'Lỗi!',
            text: 'Số điện thoại không hợp lệ. Vui lòng nhập số điện thoại chính xác.',
            icon: 'error'
        });
        return;
    }

    if (!emailRegex.test(customerEmail)) {
        Swal.fire({
            title: 'Lỗi!',
            text: 'Email không hợp lệ. Vui lòng nhập email chính xác.',
            icon: 'error'
        });
        return;
    }

    Swal.fire({
        title: 'Bạn có chắc chắn muốn cập nhật đơn hàng?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Có, cập nhật!',
        cancelButtonText: 'Không'
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire({
                title: 'Đang xử lý',
                text: 'Vui lòng chờ trong khi lưu sản phẩm...',
                allowOutsideClick: false,
                didOpen: () => {
                    Swal.showLoading();
                }
            });

            const apiUrl = `https://localhost:7241/api/Order/UpdateOrder/${ID}/${userId}`;

            const orderDetails = Array.from(document.querySelectorAll('#product_list .col-md-4')).map(row => {
                const idOptions = row.getAttribute('data-id-options');
                const quantity = row.querySelector('input[name="quantity"]').value;

                return {
                    idOptions: idOptions,
                    quantity: parseInt(quantity)
                };
            });

            const orderData = {
                modifiedBy: userId,
                idUser: userId,
                customerName: document.getElementById('customer_name').value,
                customerPhone: customerPhone,
                customerEmail: customerEmail,
                shippingAddressLine2: document.getElementById('shippingadress2').value,
                cotsts: 0,
                orderDetails: orderDetails
            };

            var xhr = new XMLHttpRequest();
            xhr.open('PUT', apiUrl, true);
            xhr.setRequestHeader('Content-Type', 'application/json');

            xhr.onload = function () {
                if (xhr.readyState === xhr.DONE) {
                    Swal.close();
                    if (xhr.status === 200) {
                        Swal.fire({
                            title: 'Thành công!',
                            text: 'Cập nhật đơn hàng thành công.',
                            icon: 'success'
                        }).then(() => {
                            window.location.href = '/home/index_order';
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
                Swal.close();
                Swal.fire({
                    title: 'Lỗi!',
                    text: 'Có lỗi kết nối đến máy chủ.',
                    icon: 'error'
                });
            };

            xhr.send(JSON.stringify(orderData));
        }
    });
});
function updateOrderStatus(status, idorder) {
    const statusMap = {
        0: '',
        1: 'Xác nhận đơn hàng',
        2: 'Vận chuyển',
        3: 'Đã giao hàng',
        4: 'Hủy đơn'
    };

    const vietnameseStatus = statusMap[status];
    Swal.fire({
        title: 'Xác nhận',
        text: `Bạn có chắc chắn muốn cập nhật trạng thái đơn hàng thành "${vietnameseStatus}" không?`,
        input: 'text',
        inputPlaceholder: 'Nhập ghi chú (tùy chọn)',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Có',
        cancelButtonText: 'Hủy',
        preConfirm: (billOfLadingCode) => {
            if (!billOfLadingCode) {
                Swal.showValidationMessage('Mã vận đơn là bắt buộc!');
            }
            return billOfLadingCode;
        }
    }).then((result) => {
        if (result.isConfirmed) {
            const billOfLadingCode = result.value;
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
                        text: `Trạng thái đơn hàng đã được cập nhật thành "${vietnameseStatus}".`,
                        confirmButtonText: 'OK'
                    }).then(() => {
                        window.location.reload();
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
                Swal.close();
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
                idUser: userId,
                billOfLadingCode: billOfLadingCode
            }));
        }
    });
}
function fetchOrderHistory(orderId) {
    var xhr = new XMLHttpRequest();
    xhr.open('GET', `https://localhost:7241/api/OrderHistory/by-order/${orderId}`, true);
    xhr.setRequestHeader('Accept', '*/*');

    xhr.onload = function () {
        if (xhr.status >= 200 && xhr.status < 300) {
            var response = JSON.parse(xhr.responseText);
            populateOrderHistoryModal(response);
            $('#orderHistoryModal').modal('show');
        } else {
            console.error('Lỗi khi lấy dữ liệu lịch sử đơn hàng:', xhr.statusText);
        }
    };

    xhr.onerror = function () {
        console.error('Lỗi kết nối mạng hoặc máy chủ.');
    };

    xhr.send();
}
function populateOrderHistoryModal(orderHistoryData) {
    var orderhistoryBody = document.getElementById('orderhistory_body');
    orderhistoryBody.innerHTML = '';

    if (orderHistoryData && orderHistoryData.length > 0) {
        orderHistoryData.forEach(function (history) {
            var formattedEditingHistory = formatLink(history.editingHistory.replace(/\n/g, '<br>'));
            var formattedChangeDetails = formatLink(history.changeDetails.replace(/\n/g, '<br>'));

            var row = `
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
        orderhistoryBody.innerHTML = '<tr><td colspan="3">Không có chi tiết đơn hàng</td></tr>';
    }
    var userLinks = orderhistoryBody.querySelectorAll('a[data-user-id]');
    userLinks.forEach(link => {
        link.addEventListener('click', handleUserLinkClick);
    });

}
function formatDateTime(dateTime) {
    var date = new Date(dateTime);
    return date.toLocaleString('vi-VN');
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
