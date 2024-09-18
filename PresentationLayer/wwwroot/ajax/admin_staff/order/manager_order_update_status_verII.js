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
console.log('userId', userId);

function checkAuthentication() {
    if (!jwt || !userId) {
        window.location.href = '/login';
        return false;
    }
    return true;
}
checkAuthentication();

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
        step.style.display = 'none'; 
    });

    var statusMap = {
        0: 'step-0',
        1: 'step-1',
        2: 'step-2',
        3: 'step-3',
        4: 'cancel-order'
    };

    if (orderStatus === 4) {
        var cancelOrderStep = document.getElementById('cancel-order');
        if (cancelOrderStep) {
            cancelOrderStep.style.display = 'block'; 
            cancelOrderStep.classList.add('cancelled');
        }
        const iconElement = document.querySelector('.icon i');
        if (iconElement) {
            iconElement.style.color = 'green';
        }
    } else if (orderStatus >= 0 && orderStatus <= 3) {
        for (let i = 0; i <= 3; i++) {
            let stepId = `step-${i}`;
            let step = document.getElementById(stepId);
            if (step) {
                step.style.display = 'block'; 
                if (i <= orderStatus) {
                    step.classList.add('active');
                }
            }
        }
        var cancelOrderStep = document.getElementById('cancel-order');
        if (cancelOrderStep) {
            cancelOrderStep.style.display = 'none'; 
        }
    } else {
        var currentStepId = statusMap[orderStatus] || 'step-0';
        var step = document.getElementById(currentStepId);
        if (step) {
            step.style.display = 'block'; 
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
    document.getElementById('shippingadress').value = orderData.shippingAddress;
    const voucherCodeElement = document.getElementById('voucher_code');

    if (orderData.voucherCode) {
        voucherCodeElement.innerHTML = `
        <strong>
            <button type="button" onclick="openOffersModal('${orderData.voucherCode}')">
                Xem ưu đãi
            </button> 
            ${orderData.voucherCode}
        </strong>`;
    } else {
        voucherCodeElement.innerHTML = 'Không có';
    }
    document.getElementById('shippingadress2').value = orderData.shippingAddressLine2;
    document.getElementById('total_price').textContent = `${orderData.totalAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}`;
    document.getElementById('costs_display_1').textContent = ` (Phí giao hàng: ${orderData.cotsts.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })})`;
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
        <button class="btn btn-danger btn-sm cancel" type="button" onclick="updateOrderStatus('4','${orderData.id}', '${orderData.hexCode}')" title="Hủy đơn hàng">
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
    const updateOrderButton = document.getElementById('btn_update_order');
    const hiddenStatuses = [2,3, 4]; // 2: Vận chuyển hoàn thành, 4: Hủy
    if (hiddenStatuses.includes(orderData.orderStatus)) {
        updateOrderButton.style.display = 'none';
    } else {
        updateOrderButton.style.display = 'inline-block'; // Hoặc 'block', tùy vào cách bạn muốn hiển thị nút
    }
}
document.getElementById('btn_printf_order_pdf').addEventListener('click', function () {
    printf_order_pdf(hexCode);
});
function openOffersModal(voucherCode) {
    var xhr = new XMLHttpRequest();
    xhr.open('GET', 'https://localhost:7241/api/Voucher/' + voucherCode, true);
    xhr.setRequestHeader('Accept', '*/*');

    xhr.onload = function () {
        if (xhr.status === 200) {
            var data = JSON.parse(xhr.responseText);

            var offersContent = document.getElementById('offersContent');
            var voucherCodeElement = document.getElementById('voucher_code');

            offersContent.innerHTML = `
                <div style="border: 1px solid #ddd; border-radius: 8px; padding: 15px; background-color: #f9f9f9; box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);">
                    <div style="display: flex; align-items: center; margin-bottom: 10px;">
                        <i class="fa fa-gift" style="font-size: 24px; color: #e53935; margin-right: 10px;"></i>
                        <div style="font-size: 18px; font-weight: bold; color: #333;">Ưu đãi: ${data.name}</div>
                    </div>
                    <div style="margin-bottom: 15px;">
                        <p><strong>Mã Voucher:</strong> ${data.code}</p>
                        <p><strong>Giảm:</strong> ${data.type === 0 ? formatPercentage(data.reducedValue) : formatCurrency(data.reducedValue)} </p>
                        <p><strong>Tối thiểu:</strong> ${formatCurrency(data.minimumAmount)}</p>
                        <p><strong>Giảm tối đa:</strong> ${formatCurrency(data.maximumAmount)}</p>
                        <div style="display: flex; gap: 10px;">
                            <p><strong>Từ:</strong> ${formatDate_ver1(data.startDate)}</p>
                            <p><strong>Đến:</strong> ${formatDate_ver1(data.endDate)}</p>
                        </div>
                        <p><strong>Số lượng còn lại:</strong> ${data.quantity}</p>
                    </div>
                    <div>
                        <strong>Hạn sử dụng còn:</strong> <span id="time-left" style="font-weight: bold; color: #e53935;"></span>
                    </div>
                </div>
            `;

            document.getElementById('offersModal').style.display = 'block';
            var countdownInterval = setInterval(function () {
                calculateTimeLeft(data.endDate);
            }, 1000);
        } else {
            console.error('Failed to fetch data');
        }
    };

    xhr.onerror = function () {
        console.error('Request error');
    };

    xhr.send();
}
function formatCurrency(value) {
    return value.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
}

function formatPercentage(value) {
    return value.toFixed(0) + ' %';
}
function formatDate_ver1(date) {
    const dateObj = new Date(date);

    const datePart = dateObj.toLocaleDateString('vi-VN', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric'
    });

    const timePart = dateObj.toLocaleTimeString('vi-VN', {
        hour: '2-digit',
        minute: '2-digit',
        second: '2-digit'
    });

    return `${datePart} ${timePart}`;
}
function calculateTimeLeft(endDate) {
    var end = new Date(endDate).getTime();
    var now = new Date().getTime();
    var timeLeft = end - now;

    if (timeLeft <= 0) {
        document.getElementById('time-left').innerHTML = 'Đã hết hạn';
        clearInterval(countdownInterval); 
        return;
    }

    var days = Math.floor(timeLeft / (1000 * 60 * 60 * 24));
    var hours = Math.floor((timeLeft % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    var minutes = Math.floor((timeLeft % (1000 * 60 * 60)) / (1000 * 60));
    var seconds = Math.floor((timeLeft % (1000 * 60)) / 1000);

    document.getElementById('time-left').innerHTML = days + ' ngày ' + hours + ' giờ ' + minutes + ' phút ' + seconds + ' giây';
}
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
            return 'Hoàn thành';
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
            function formatCurrencyToDecimal(currencyString) {
                const cleanString = currencyString.replace(/[^\d,]/g, '');

                const decimalString = cleanString.replace(',', '.');

                const number = parseFloat(decimalString);

                return Math.round(number);
            }

            const costsElement = document.getElementById('costs_display_1').innerHTML;
            const cotsts = formatCurrencyToDecimal(costsElement);
            console.log('cotsts', cotsts);

            if (isNaN(cotsts)) {
                console.error("Giá trị cotsts không hợp lệ:", costsElement);
            }
            const orderData = {
                modifiedBy: userId,
                idUser: userId,
                customerName: document.getElementById('customer_name').value,
                customerPhone: customerPhone,
                customerEmail: customerEmail,
                shippingAddress: document.getElementById('shippingadress').value,
                shippingAddressLine2: document.getElementById('shippingadress2').value,
                cotsts: cotsts,
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
document.addEventListener("DOMContentLoaded", function () {
    var modal = document.getElementById("addressModal");
    var btn = document.getElementById("openAddressModalBtn1");
    var span = document.getElementsByClassName("close")[0];

    btn.onclick = function () {
        modal.style.display = "block";
    }

    span.onclick = function () {
        modal.style.display = "none";
    }

    window.onclick = function (event) {
        if (event.target === modal) {
            modal.style.display = "none";
        }
    }

    var token = 'd01771f0-3f8b-11ef-8f55-4ee3d82283af';
    var Parameter = {
        url: "https://raw.githubusercontent.com/kenzouno1/DiaGioiHanhChinhVN/master/data.json",
        method: "GET",
        responseType: "json",
    };

    var xhr = new XMLHttpRequest();
    xhr.open('GET', Parameter.url, true);
    xhr.responseType = 'json';
    xhr.onload = function () {
        if (xhr.status === 200) {
            renderCity(xhr.response);
        } else {
            console.error('Error fetching data: ', xhr.statusText);
        }
    };
    xhr.onerror = function () {
        console.error('Request failed');
    };
    xhr.send();

    function renderCity(data) {
        var citiesSelect = document.getElementById("city_1");
        var districtsSelect = document.getElementById("district_1");
        var wardsSelect = document.getElementById("ward_1");

        citiesSelect.innerHTML = '<option value="" selected>Chọn tỉnh thành</option>';
        data.forEach(function (city) {
            var option = document.createElement("option");
            option.value = city.Name;
            option.textContent = city.Name;
            citiesSelect.appendChild(option);
        });

        citiesSelect.addEventListener('change', function () {
            districtsSelect.innerHTML = '<option value="" selected>Chọn quận huyện</option>';
            wardsSelect.innerHTML = '<option value="" selected>Chọn phường xã</option>';

            var selectedCity = data.find(city => city.Name === this.value);
            if (selectedCity) {
                selectedCity.Districts.forEach(function (district) {
                    var option = document.createElement("option");
                    option.value = district.Name;
                    option.textContent = district.Name;
                    districtsSelect.appendChild(option);
                });
            }
            updateAddress();
            calculateShippingFee();
        });

        districtsSelect.addEventListener('change', function () {
            wardsSelect.innerHTML = '<option value="" selected>Chọn phường xã</option>';
            var selectedCity = data.find(city => city.Name === citiesSelect.value);
            if (selectedCity) {
                var selectedDistrict = selectedCity.Districts.find(district => district.Name === this.value);
                if (selectedDistrict) {
                    selectedDistrict.Wards.forEach(function (ward) {
                        var option = document.createElement("option");
                        option.value = ward.Name;
                        option.textContent = ward.Name;
                        wardsSelect.appendChild(option);
                    });
                }
            }
            updateAddress();
            calculateShippingFee();
        });

        wardsSelect.addEventListener('change', function () {
            updateAddress();
            calculateShippingFee();
        });

        function updateAddress() {
            var city = citiesSelect.value;
            var district = districtsSelect.value;
            var ward = wardsSelect.value;
            var fullAddress = `${city}, ${district}, ${ward}`;
            document.getElementById('shippingadress').value = fullAddress;
        }
    }

    async function calculateShippingFee() {
        var city = document.getElementById('city_1').value;
        var district = document.getElementById('district_1').value;
        var ward = document.getElementById('ward_1').value;

        if (city && district && ward) {
            await loadTinhThanh();
        }
    }

    async function loadTinhThanh() {
        var tinhthanh = document.getElementById('city_1').value;
        let provinceID;
        let districtID;
        let wardCode;
        try {
            const response = await fetch('https://online-gateway.ghn.vn/shiip/public-api/master-data/province', {
                method: 'GET',
                headers: {
                    'Token': token
                }
            });

            const data = await response.json();

            data.data.forEach(function (item) {
                if (item.ProvinceName.trim() === tinhthanh.replace(/^Tỉnh\s+|\s+$/g, '').trim()) {
                    provinceID = item.ProvinceID;
                }
            });
            await loadQuanHuyen(provinceID);
        } catch (error) {
            alert('Không tìm thấy mã tỉnh.');
        }

        async function loadQuanHuyen(provinceId) {
            var quanhuyen = document.getElementById('district_1').value;
            try {
                const response = await fetch('https://online-gateway.ghn.vn/shiip/public-api/master-data/district', {
                    method: 'GET',
                    headers: {
                        'Token': token
                    }
                });

                const data = await response.json();

                data.data.forEach(function (item) {
                    if (item.DistrictName.trim() === quanhuyen.trim()) {
                        districtID = item.DistrictID;
                    }
                });
                await loadXaPhuong(districtID);
            } catch (error) {
                console.error('Error:', error);
            }
        }

        async function loadXaPhuong(districtID) {
            var xaphuong = document.getElementById('ward_1').value;
            try {
                const response = await fetch('https://online-gateway.ghn.vn/shiip/public-api/master-data/ward?district_id=' + districtID, {
                    method: 'GET',
                    headers: {
                        'Token': token
                    }
                });

                const data = await response.json();

                data.data.forEach(function (item) {
                    if (item.WardName.trim() === xaphuong.trim()) {
                        wardCode = item.WardCode;
                    }
                });
                getShippingFee(provinceID, districtID, wardCode);
                console.log('Mã Tỉnh: ' + provinceID + ', Mã quận: ' + districtID + ', Mã xã: ' + wardCode);
            } catch (error) {
                console.error('Error:', error);
            }
        }
    }
});
async function getShippingFee(provinceID, districtID, wardCode) {
    const shopID = 4145900;
    var token = 'd01771f0-3f8b-11ef-8f55-4ee3d82283af';

    try {
        const response = await fetch('https://online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/fee', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Token': token
            },
            body: JSON.stringify({
                "service_type_id": 2,
                "from_district_id": 3440,
                "from_ward_code": "13009",
                "to_district_id": districtID,
                "to_ward_code": wardCode,
                "height": 20,
                "length": 30,
                "weight": 3000,
                "width": 40,
                "insurance_value": 0,
                "coupon": null
            })
        });

        const responseData = await response.json();

        console.log('API Response:', responseData);

        if (responseData.code === 200) {
            let shippingFee = responseData.data.total;
            console.log('Giá:', shippingFee);

            var costsElement = document.getElementById('costs_1');
            var costsDisplayElement = document.getElementById('costs_display_1');
            console.log('costs_1:', document.getElementById('costs_1'));
            console.log('costs_display_1:', document.getElementById('costs_display_1'));


            if (costsElement && costsDisplayElement) {
                costsElement.innerText = shippingFee;
                costsDisplayElement.innerText = ` (Phí giao hàng: ${shippingFee.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })})`; 
            } else {
                console.error('Elements not found: costs_1 or costs_display_1');
            }

          
        } else {
            console.error('API Error:', responseData.message);
        }
    } catch (error) {
        console.error('Fetch Error:', error);
    }
}
