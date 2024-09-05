const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:7241/productHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

connection.start().catch(err => console.error(err.toString()));

connection.on("ReceiveProductQuantityUpdate", (optionId, newQuantity) => {
    updateStockDisplay(optionId, newQuantity);
});

var selectedProducts = [];
var selectedQuantities = {};
var orderDetails = [];
var selectedVoucherId = null;
let selectedUser = null;
let totalPay = null;
let selectedUserId = null;
let globalShippingFee = 0;
var costs = 0;
const stockQuantities = {};
const couponInput = document.getElementById('coupound');
const tamtinhElement = document.getElementById('temporary_payment_for_goods');
const totalPayElement = document.getElementById('total_pay');
const moneyGivenByGuestsInput = document.getElementById('money_given_by_guests');
const customersStillOweElement = document.getElementById('customers_still_owe');
moneyGivenByGuestsInput.addEventListener('input', formatAndCalculateCustomersStillOwe);
var invoiceNumber = getQueryParameter('invoiceNumber');
let voucherCode = null;
document.getElementById('invoiceNumberDisplay').innerText = 'Hóa đơn số: ' + invoiceNumber;
function showLoader() {
    loader.style.display = 'block';
}
function hideLoader() {
    loader.style.display = 'none';
}
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
    calculateSubtotal();
});
document.addEventListener("DOMContentLoaded", function () {
    showLoader();
    getOptions();
});
function getOptions() {
    var xhr = new XMLHttpRequest();
    xhr.open('GET', 'https://localhost:7241/api/Options/getallactive', true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            hideLoader();
            if (xhr.status === 200) {
                var data = JSON.parse(xhr.responseText);
                renderOptions(data);
            } else {
                console.error('Error fetching data:', xhr.statusText);
            }
        }
    };
    xhr.send();
}
function renderOptions(data) {
    const tableBody = document.getElementById('productTableBody');
    tableBody.innerHTML = '';

    const productMap = {};

    data.forEach(product => {
        const key = `${product.keyCode}_${product.productName}`;
        if (!productMap[key]) {
            productMap[key] = [];
        }
        productMap[key].push(product);
    });

    Object.keys(productMap).forEach(key => {
        const productOptions = productMap[key];
        let isFirstRow = true;

        productOptions.forEach(option => {
            const row = document.createElement('tr');
            const currentStock = stockQuantities[option.id] || option.stockQuantity;

            row.innerHTML = `
                <td>${isFirstRow ? `${option.productName}<br> (${option.keyCode})` : ''}</td>
                <td class="col-2 text-center">${option.colorName} - ${option.sizesName} <p id="stock_quantity_${option.id}">(SLT: ${currentStock})</p></td>
                <td class="col-2 text-center"><img src="${option.imageURL}" alt="${option.productName}" style="width: 50px; height: auto;"></td>
                <td class="col-2 text-center" id="retail_price_options" style="width:70px">${option.retailPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</td>
                <td class="col-2 text-center"><input type="number" id="quantity_options_${option.id}" value="1" min="1" style="width: 80px; text-align: center;"></td>
                <td class="text-center">
                    <button class="btn btn-primary add-product" data-option-id="${option.id}"><i class="fas fa-plus-circle"></i></button>
                    <button class="btn btn-primary btn-sm view" type="button" title="Xem chi tiết"> <i class="fas fa-eye"></i></button>
                </td>
            `;

            tableBody.appendChild(row);
            isFirstRow = false;
        });
    });

    tableBody.removeEventListener('click', handleAddProduct);
    tableBody.addEventListener('click', handleAddProduct);
}
function handleAddProduct(event) {
    const target = event.target;
    if (target.classList.contains('add-product') || target.parentElement.classList.contains('add-product')) {
        const button = target.closest('.add-product');
        const optionId = button.getAttribute('data-option-id');
        addToSelectedProducts(optionId);
    }
}
function formatAndCalculateTotalPay() {
    formatCurrency(couponInput);
    calculateTotalPay();
}
function calculateTotalPay() {
    const shippingFeeElement = document.getElementById('shippingFee');

    const tamtinh = parseFloat(tamtinhElement.textContent.replace(/[^\d,]/g, '').replace(',', '.')) || 0;
    const coupon = parseFloat(couponInput.value.replace(/[^\d,]/g, '').replace(',', '.')) || 0;
    let costs = 0;

    if (shippingFeeElement) {
        const textContent = shippingFeeElement.textContent.trim();
        const match = textContent.match(/[\d,]+/);

        if (match) {
            costs = parseFloat(match[0].replace(/\./g, '').replace(',', '.'));
        }
    }

    totalPay = tamtinh - coupon + costs;
    totalPay = Math.max(totalPay, 0);

    if (totalPayElement) {
        totalPayElement.textContent = totalPay.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
    }
    calculateCustomersStillOwe();
}
function formatCurrency(amount) {
    return new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    }).format(amount);
}
function formatAndCalculateCustomersStillOwe() {
    formatCurrency(moneyGivenByGuestsInput);
    calculateCustomersStillOwe();
}
function calculateCustomersStillOwe() {
    const totalPayElement = document.getElementById('total_pay');
    const moneyGivenByGuestsInput = document.getElementById('money_given_by_guests');
    const customersStillOweElement = document.getElementById('customers_still_owe');

    const totalPay = parseFloat(totalPayElement.textContent.replace(/\D/g, '')) || 0;
    const moneyGiven = parseFloat(moneyGivenByGuestsInput.value.replace(/\D/g, '')) || 0;
    let customersStillOwe = totalPay - moneyGiven;
    customersStillOweElement.textContent = customersStillOwe.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
}
function fillOrderData(users) {
    const note = document.getElementById('note_order').value;
    const paymentMethod = document.getElementById('paymentMethodSelect').value;
    const shippingMethodSelect = document.getElementById('shippingMethod');
    const shippingMethodValue = shippingMethodSelect.value;
    const customerName_gui = document.getElementById('customerName').value.trim();
    const customerPhone_gui = document.getElementById('customerPhone').value.trim();
    const shippingAddress = document.getElementById('shippingAddress').value.trim();
    const shippingFeeElement = document.getElementById('shippingFee');
    if (shippingFeeElement) {
        const textContent = shippingFeeElement.textContent.trim();
        const match = textContent.match(/\d+/);
        if (match) {
            costs = parseInt(match[0]);
        }
    }

    const isProxyOrder = shippingMethodValue === "1" || shippingMethodValue !== "0";

    return createOrderData({
        createBy: users ? users.id : '',
        idUser: users ? users.id : '',
        customerName_gui: users ? users.firstAndLastName : '',
        customerPhone_gui: users ? users.phoneNumber : '',
        customerEmail: users ? users.email : '',
        shippingAddress: shippingAddress || 'Nhận tại cửa hàng',
        voucherCode: voucherCode || null,
        costs: costs,
        note: note,
        paymentMethod: paymentMethod,
        shippingMethodValue: shippingMethodValue,
        orderDetails: orderDetails,
        isProxyOrder: isProxyOrder,
        proxyCustomerName: isProxyOrder ? customerName_gui : '',
        proxyCustomerPhone: isProxyOrder ? customerPhone_gui : ''
    });
}
document.getElementById('customerPhone').addEventListener('blur', function () {
    const phoneRegex = /^(03|05|07|08|09)\d{8}$/;
    const phone = this.value;
    if (!phoneRegex.test(phone)) {
        this.value = '';
        Swal.fire({
            icon: 'error',
            title: 'Lỗi',
            text: 'Số điện thoại không hợp lệ!',
            confirmButtonText: 'OK'
        });
    }
});
function createOrderData({
    idUser,
    customerName_gui,
    customerPhone_gui,
    customerEmail,
    shippingAddress,
    costs,
    note,
    paymentMethod,
    shippingMethodValue,
    orderDetails,
    isProxyOrder,
    proxyCustomerName,
    proxyCustomerPhone
}) {
    function generateHexCode(existingHexCodes) {
        let hexString;
        let randomPart;

        do {
            const now = new Date();
            const dateString = now.toISOString().replace(/[-:.T]/g, '').slice(0, 14);

            randomPart = Math.floor(Math.random() * (9999 - 1000 + 1)) + 1000;
            hexString = dateString + randomPart.toString();
        } while (existingHexCodes.includes(hexString));

        return hexString;
    }

    var orderStatus = isProxyOrder
        ? (parseInt(shippingMethodValue, 10) === 1 ? 1 : 0)
        : 3;
    var customerName = isProxyOrder
        ? (proxyCustomerName || customerName_gui || 'Khách vãng lai')
        : (customerName_gui || 'Khách vãng lai');

    var customerPhone = isProxyOrder
        ? (proxyCustomerPhone || customerPhone_gui || 'xxx.xxx.xxx')
        : (customerPhone_gui || 'xxx.xxx.xxx');

    var finalCustomerEmail = customerEmail || 'xxx.xxx.xxx';


    return {
        createBy: userId || '',
        idUser: idUser || '',
        hexCode: generateHexCode([]),
        customerName: customerName,
        customerPhone: customerPhone,
        customerEmail: finalCustomerEmail,
        shippingAddress: shippingAddress || 'Nhận tại cửa hàng',
        shippingAddressLine2: "",
        shipDate: new Date().toISOString(),
        cotsts: costs || 0,
        voucherCode: voucherCode || null,
        notes: note || "",
        trackingCheck: true,
        paymentMethods: parseInt(paymentMethod, 10),
        paymentStatus: 1,
        shippingMethods: parseInt(shippingMethodValue, 10),
        orderStatus: orderStatus,
        orderType: 1,
        status: 1,
        orderDetailsCreateVM: orderDetails || []
    };
}
function displaySearchResults(users) {
    const resultsContainer = document.getElementById('customerSearchResults');
    resultsContainer.innerHTML = '';

    if (users.length === 0) {
        resultsContainer.innerHTML = 'Không tìm thấy khách hàng nào.';
        Swal.fire({
            icon: 'error',
            title: 'Lỗi!',
            text: 'Không tìm thấy khách hàng nào.',
        });
        return;
    }

    const title = document.createElement('h3');
    title.innerText = 'Kết quả tìm kiếm khách hàng';
    title.style.textAlign = 'center';
    title.style.marginBottom = '15px';
    title.style.fontSize = '1.5em';
    title.style.color = '#333';
    resultsContainer.appendChild(title);

    users.forEach(user => {
        const userItem = document.createElement('div');
        userItem.className = 'customer-result-item';
        userItem.style.borderBottom = '1px solid #ddd';
        userItem.style.padding = '10px 0';

        const statusText = user.status === 1 ? 'Hoạt động' : 'Ngừng hoạt động';
        const statusColor = user.status === 1 ? 'green' : 'red';

        userItem.innerHTML = `
            <div class="row" style="display: flex; align-items: center;">
                <div class="col-md-2" style="flex: 1;">
                    <img src="${user.images}" alt="Ảnh" style="border-radius: 50%; width: 50px; height: 50px; object-fit: cover;">
                </div>
                <div class="col-md-6" style="flex: 2;">
                    <p style="margin: 0; color: #555;">Tên: ${user.firstAndLastName}</p>
                    <p style="margin: 0; color: #555;">Username: ${user.username}</p>
                </div>
                <div class="col-md-4" style="flex: 2;">
                    <p style="margin: 0; color: #555;">Email: ${user.email}</p>
                    <p style="margin: 0; color: #555;">Điện thoại: ${user.phoneNumber}</p>
                    <p style="margin: 0; color: ${statusColor};">Trạng thái: ${statusText}</p>
                </div>
            </div>
        `;
        resultsContainer.appendChild(userItem);
    });
    resultsContainer.style.border = '1px solid #ddd';
    resultsContainer.style.padding = '10px';
    resultsContainer.style.borderRadius = '5px';
    resultsContainer.style.marginTop = '20px';
    resultsContainer.style.backgroundColor = '#f9f9f9';
}
document.getElementById('customerPhoneNumber').addEventListener('blur', function () {
    const phoneNumber = this.value.trim();
    if (phoneNumber.length >= 10) {
        searchCustomerByPhoneNumber(phoneNumber);
    } else {
        document.getElementById('customerSearchResults').innerHTML = '';
        Swal.fire({
            icon: 'error',
            title: 'Lỗi!',
            text: 'Số điện thoại không hợp lệ. Vui lòng nhập ít nhất 10 số.',
        });
        this.value = '';
    }
});
function searchCustomerByPhoneNumber(phoneNumber) {
    const xhr = new XMLHttpRequest();
    xhr.open('GET', `https://localhost:7241/api/ApplicationUser/GetUsersByPhoneNumber?phoneNumber=${phoneNumber}`, true);
    xhr.setRequestHeader('Authorization', `Bearer ${getJwtFromCookie()}`);


    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
                const users = JSON.parse(xhr.responseText);
                displaySearchResults(users);

                if (users.length > 0) {
                    selectedUser = users[0];
                    const data = fillOrderData(selectedUser);
                    selectedUserId = users[0].id;
                    console.log('Selected User ID:', selectedUserId);
                    console.log('data', data);
                } else {
                    console.error('Error fetching customer data:', 'No users found');
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi!',
                        text: 'Không tìm thấy khách hàng nào.',
                    });
                    document.getElementById('customerPhoneNumber').value = '';
                }
            } else {
                console.error('Error fetching customer data:', xhr.statusText);
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi!',
                    text: 'Không tìm thấy khách hàng nào.',
                });
                document.getElementById('customerPhoneNumber').value = '';
            }
        }
    };
    xhr.send();
}
function getQueryParameter(name) {
    var urlParams = new URLSearchParams(window.location.search);
    return urlParams.get(name);
}
function calculateSubtotal() {
    const selectedProducts = document.querySelectorAll('.selected-product-item');
    let subtotal = 0;

    selectedProducts.forEach(product => {
        const totalPriceElement = product.querySelector('.product-total');
        const priceText = totalPriceElement.textContent.trim().replace(/\D/g, '');
        subtotal += parseInt(priceText);
    });

    const subtotalElement = document.querySelector('.control-all-money-tamtinh');
    subtotalElement.textContent = subtotal.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
    calculateTotalPay();
}
function removeFromOrderDetails(optionId) {
    const index = orderDetails.findIndex(item => item.idOptions === optionId);
    if (index !== -1) {
        orderDetails.splice(index, 1);
    }
}
function updateStockDisplay(optionId, newQuantity) {
    const stockElement = document.getElementById(`stock_quantity_${optionId}`);
    if (stockElement) {
        stockElement.textContent = `(SLT: ${newQuantity})`;
    }
}
function increaseQuantity(button) {
    const row = button.closest('tr');
    const optionId = row.dataset.optionId;
    const quantityElement = row.querySelector('.product-quantity');
    let currentQuantity = parseInt(quantityElement.textContent) || 0;

    const xhr = new XMLHttpRequest();
    xhr.open('GET', `https://localhost:7241/api/Options/GetByID/${optionId}`, true);
    xhr.setRequestHeader('Accept', 'application/json');

    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                const option = JSON.parse(xhr.responseText);
                const stockQuantity = option.stockQuantity;

                if (currentQuantity < stockQuantity) {
                    currentQuantity += 1;
                    quantityElement.textContent = currentQuantity;

                    const priceElement = row.querySelector('.product-total');
                    const pricePerUnitElement = document.getElementById('retail_price_options');
                    const pricePerUnit = parseFloat(pricePerUnitElement.value || pricePerUnitElement.textContent.trim().replace(/\D/g, ''));

                    if (!isNaN(pricePerUnit)) {
                        const newPrice = pricePerUnit * currentQuantity;
                        priceElement.textContent = newPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
                    } else {
                        console.error('Invalid pricePerUnit:', pricePerUnit);
                        priceElement.textContent = '0 ₫';
                    }

                    stockQuantities[optionId] = stockQuantity + 1;
                    selectedQuantities[optionId] = currentQuantity;
                    updateProductQuantity(option.id, selectedQuantities[optionId]);

                    console.log('stockQuantities[optionId]', stockQuantities[optionId]);
                    console.log('selectedQuantities[optionId]', selectedQuantities[optionId]);
                    updateStockDisplay(optionId, stockQuantities[optionId]);

                    const xhrUpdate = new XMLHttpRequest();
                    xhrUpdate.open('POST', 'https://localhost:7241/api/Options/decrease-quantity', true);
                    xhrUpdate.setRequestHeader('Content-Type', 'application/json');
                    xhrUpdate.setRequestHeader('Accept', '*/*');

                    xhrUpdate.onreadystatechange = function () {
                        if (xhrUpdate.readyState === XMLHttpRequest.DONE) {
                            if (xhrUpdate.status === 200) {
                                calculateSubtotal();
                            } else {
                                console.error('Error:', xhrUpdate.status, xhrUpdate.statusText);
                                console.error('Response Text:', xhrUpdate.responseText);
                            }
                        }
                    };

                    const requestData = JSON.stringify({
                        idOptions: optionId,
                        quantityToDecrease: 1
                    });
                    xhrUpdate.send(requestData);

                    connection.invoke("UpdateProductQuantity", optionId, stockQuantities[optionId])
                        .catch(err => console.error('Error sending stock update:', err));
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi!',
                        text: 'Không còn đủ số lượng tồn kho.',
                        confirmButtonText: 'OK'
                    });
                }
            } else {
                console.error('Error fetching option:', xhr.statusText);
            }
        }
    };
    xhr.send();
}
function decreaseQuantity(button) {
    const row = button.closest('tr');
    const quantityElement = row.querySelector('.product-quantity');
    let currentQuantity = parseInt(quantityElement.textContent) || 0;
    const optionId = row.dataset.optionId;

    if (currentQuantity > 1) {
        const xhr = new XMLHttpRequest();
        xhr.open('GET', `https://localhost:7241/api/Options/GetByID/${optionId}`, true);
        xhr.setRequestHeader('Accept', 'application/json');

        xhr.onreadystatechange = function () {
            if (xhr.readyState === XMLHttpRequest.DONE) {
                if (xhr.status === 200) {
                    const option = JSON.parse(xhr.responseText);
                    const stockQuantity = option.stockQuantity;

                    currentQuantity -= 1;
                    quantityElement.textContent = currentQuantity;

                    const priceElement = row.querySelector('.product-total');
                    const pricePerUnitElement = document.getElementById('retail_price_options');
                    const pricePerUnit = parseFloat(pricePerUnitElement.value || pricePerUnitElement.textContent.trim().replace(/\D/g, ''));

                    if (!isNaN(pricePerUnit)) {
                        const newPrice = pricePerUnit * currentQuantity;
                        priceElement.textContent = newPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
                    } else {
                        console.error('Invalid pricePerUnit:', pricePerUnit);
                        priceElement.textContent = '0 ₫';
                    }

                    stockQuantities[optionId] = stockQuantity + 1;
                    selectedQuantities[optionId] = currentQuantity;
                    updateProductQuantity(option.id, selectedQuantities[optionId]);
                    console.log('stockQuantities[optionId]', stockQuantities[optionId]);
                    console.log('selectedQuantities[optionId]', selectedQuantities[optionId]);

                    updateStockDisplay(optionId, stockQuantities[optionId]);

                    const xhrUpdate = new XMLHttpRequest();
                    xhrUpdate.open('POST', 'https://localhost:7241/api/Options/increase-quantity', true);
                    xhrUpdate.setRequestHeader('Content-Type', 'application/json');
                    xhrUpdate.setRequestHeader('Accept', '*/*');

                    xhrUpdate.onreadystatechange = function () {
                        if (xhrUpdate.readyState === XMLHttpRequest.DONE) {
                            if (xhrUpdate.status === 200) {
                                calculateSubtotal();
                            } else {
                                console.error('Error:', xhrUpdate.status, xhrUpdate.statusText);
                                console.error('Response Text:', xhrUpdate.responseText);
                            }
                        }
                    };

                    const requestData = JSON.stringify({
                        idOptions: optionId,
                        quantityToDecrease: 1
                    });
                    xhrUpdate.send(requestData);

                    connection.invoke("UpdateProductQuantity", optionId, stockQuantities[optionId])
                        .catch(err => console.error('Error sending stock update:', err));
                } else {
                    console.error('Error fetching option:', xhr.statusText);
                }
            }
        };
        xhr.send();
    } else {
        Swal.fire({
            icon: 'error',
            title: 'Lỗi!',
            text: 'Số lượng không thể giảm xuống dưới 1.',
            confirmButtonText: 'OK'
        });
    }
}
function removeProduct(button) {
    const row = button.closest('tr');
    const optionId = row.dataset.optionId;
    const quantityElement = row.querySelector('.product-quantity');
    const quantity = parseInt(quantityElement.textContent) || 0;

    if (quantity <= 0) {
        console.error('Quantity must be greater than 0');
        return;
    }

    removeFromOrderDetails(optionId);
    row.remove();

    if (!document.querySelector('#selectedProductsList tbody').children.length) {
        const noProductsMessage = document.createElement('tr');
        noProductsMessage.id = 'noProductsMessage';
        noProductsMessage.innerHTML = '<td colspan="7" class="text-center">Không có sản phẩm nào được chọn</td>';
        document.querySelector('#selectedProductsList tbody').appendChild(noProductsMessage);
    }

    updateSubmitButtonState();
    calculateSubtotal();

    const xhrGet = new XMLHttpRequest();
    xhrGet.open('GET', `https://localhost:7241/api/Options/GetByID/${optionId}`, true);
    xhrGet.setRequestHeader('Accept', 'application/json');

    xhrGet.onreadystatechange = function () {
        if (xhrGet.readyState === XMLHttpRequest.DONE) {
            if (xhrGet.status === 200) {
                const option = JSON.parse(xhrGet.responseText);
                const stockQuantity = option.stockQuantity;

                stockQuantities[optionId] = stockQuantity + quantity;
                updateStockDisplay(optionId, stockQuantities[optionId]);

                const xhrUpdate = new XMLHttpRequest();
                xhrUpdate.open('POST', 'https://localhost:7241/api/Options/increase-quantity', true);
                xhrUpdate.setRequestHeader('Content-Type', 'application/json');
                xhrUpdate.setRequestHeader('Accept', 'application/json');

                xhrUpdate.onreadystatechange = function () {
                    if (xhrUpdate.readyState === XMLHttpRequest.DONE) {
                        if (xhrUpdate.status === 200) {
                            calculateSubtotal();
                            toastr.success('Sản phẩm đã được xóa thành công và số lượng tồn kho đã được cập nhật.', 'Thành công');

                        } else {
                            console.error('Error:', xhrUpdate.status, xhrUpdate.statusText);
                            console.error('Response Text:', xhrUpdate.responseText);
                        }
                    }
                };

                const requestData = JSON.stringify({
                    idOptions: optionId,
                    quantityToDecrease: quantity
                });

                xhrUpdate.send(requestData);
                connection.invoke("UpdateProductQuantity", optionId, stockQuantities[optionId])
                    .catch(err => console.error('Error sending stock update:', err));

            } else {
                console.error('Error fetching option:', xhrGet.statusText);
            }
        }
    };

    xhrGet.send();
}
window.addToSelectedProducts = function (optionId) {
    var xhr = new XMLHttpRequest();
    xhr.open('GET', `https://localhost:7241/api/Options/GetByID/${optionId}`, true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4) {
            if (xhr.status == 200) {
                var option = JSON.parse(xhr.responseText);
                const selectedProductsList = document.getElementById('selectedProductsList').querySelector('tbody');
                const quantityInput = document.getElementById(`quantity_options_${optionId}`);
                const newQuantity = quantityInput ? parseInt(quantityInput.value) : 1;

                const currentStockQuantity = stockQuantities[optionId] || option.stockQuantity;
                const totalQuantity = (selectedQuantities[optionId] || 0) + newQuantity;

                if (totalQuantity > currentStockQuantity) {
                    Swal.fire(
                        'Lỗi!',
                        'Số lượng sản phẩm vượt quá số lượng tồn kho!',
                        'error'
                    );
                    return;
                }
                selectedQuantities[optionId] = totalQuantity;
                let totalPrice = option.retailPrice * newQuantity;
                if (Number.isNaN(totalPrice) || !Number.isFinite(totalPrice)) {
                    console.error('Invalid totalPrice:', totalPrice);
                    return;
                }
                const noProductsMessage = document.getElementById('noProductsMessage');
                if (noProductsMessage) {
                    noProductsMessage.remove();
                }

                let productRow = Array.from(selectedProductsList.children).find(row => row.dataset.optionId === optionId);

                if (productRow) {
                    const quantityElement = productRow.querySelector('.product-quantity');
                    const oldQuantity = parseInt(quantityElement.textContent) || 0;
                    quantityElement.textContent = oldQuantity + newQuantity;

                    const priceElement = productRow.querySelector('.product-total');
                    const currentPrice = parseFloat(priceElement.textContent.replace(/\D/g, ''));
                    const newPrice = currentPrice + totalPrice;
                    priceElement.textContent = newPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });

                } else {
                    productRow = createProductRow(option, newQuantity, totalPrice, optionId);
                    selectedProductsList.appendChild(productRow);
                }

                stockQuantities[optionId] = currentStockQuantity - newQuantity;
                updateStockDisplay(optionId, stockQuantities[optionId]);

                const xhrUpdate = new XMLHttpRequest();
                xhrUpdate.open('POST', 'https://localhost:7241/api/Options/decrease-quantity', true);
                xhrUpdate.setRequestHeader('Content-Type', 'application/json');
                xhrUpdate.setRequestHeader('Accept', '*/*');

                xhrUpdate.onreadystatechange = function () {
                    if (xhrUpdate.readyState === XMLHttpRequest.DONE) {
                        if (xhrUpdate.status === 200) {
                            connection.invoke("UpdateProductQuantity", optionId, stockQuantities[optionId])
                                .catch(err => console.error('Error sending stock update:', err));
                        } else {
                            console.error('Error updating stock quantity:', xhrUpdate.statusText);
                        }
                    }
                };

                const requestData = JSON.stringify({
                    idOptions: optionId,
                    quantityToDecrease: newQuantity
                });

                xhrUpdate.send(requestData);

                addToOrderDetails(option.id, newQuantity);
                updateSubmitButtonState();
                calculateSubtotal();

            } else {
                console.error('Error fetching option:', xhr.statusText);
            }

        }
    };
    xhr.send();
}
function createProductRow(option, newQuantity, totalPrice, optionId) {
    const productRow = document.createElement('tr');
    productRow.classList.add('selected-product-item');
    productRow.dataset.optionId = optionId;

    productRow.innerHTML = `
        <input type="text" value="${option.id}" id="idoptions_${option.id}" style="display: none;">
        <td class="text-center product-image-cell">
            <img src="${option.imageURL}" alt="Ảnh" width="50">
            <div class="product-keycode">(${option.keyCode})</div>
        </td>
        <td class="text-center">Size: ${option.sizesName}<br> Color: ${option.colorName}</td>
        <td class="text-center product-price">${option.retailPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</td>
        <td class="text-center">
            <button id="btn_decreaseQuantity" onclick="decreaseQuantity(this)" style="padding: 3px 6px; background-color: #f44336; color: white; border: none; border-radius: 4px; margin-right: 5px;">-</button>
            <span class="product-quantity">${newQuantity}</span>
            <button id="btn_increaseQuantity" onclick="increaseQuantity(this)" style="padding: 3px 6px; background-color: #4CAF50; color: white; border: none; border-radius: 4px; margin-left: 5px;">+</button>
        </td>
        <td class="text-center product-total">${totalPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</td>
        <td class="text-center">
            <button class="btn btn-danger btn-sm" onclick="removeProduct(this)">Xóa</button>
        </td>
    `;

    return productRow;
}
function addToOrderDetails(optionId, quantity) {
    let existingDetail = orderDetails.find(detail => detail.idOptions === optionId);
    if (existingDetail) {
        existingDetail.quantity += quantity;
    } else {
        const newDetail = {
            createBy: userId,
            idOptions: optionId,
            quantity: quantity,
            discount: 0,
            status: 1
        };
        orderDetails.push(newDetail);
    }
}
function updateProductQuantity(optionId, newQuantity) {
    let existingDetail = orderDetails.find(detail => detail.idOptions === optionId);

    if (existingDetail) {
        existingDetail.quantity = newQuantity;
    } else {
        const newDetail = {
            createBy: userId,
            idOptions: optionId,
            quantity: newQuantity,
            discount: 0,
            status: 1
        };
        orderDetails.push(newDetail);
    }
}
function updateSubmitButtonState() {
    const selectedProductsList = document.getElementById('selectedProductsList').querySelector('tbody');
    const submitBtn = document.getElementById('btnLuuDonHang');
    const submitbtnLuu_InHoaDon = document.getElementById('btnLuu_InHoaDon');

    if (selectedProductsList && submitBtn && submitbtnLuu_InHoaDon) {
        if (!selectedProductsList.children.length || selectedProductsList.querySelector('#noProductsMessage')) {
            submitBtn.disabled = true;
            submitbtnLuu_InHoaDon.disabled = true;
        } else {
            submitBtn.disabled = false;
            submitbtnLuu_InHoaDon.disabled = false;
        }
    }
}
document.addEventListener('DOMContentLoaded', function () {
    const invoiceNumber = getQueryParameter('invoiceNumber');
    const orderData = getOrderDataFromCookies(invoiceNumber);

    if (orderData) {
        displayOrders([orderData]);
    } 
});
function saveDataToCookies() {
    const selectedProducts = [];
    document.querySelectorAll('#selectedProductsList tbody .selected-product-item').forEach(function (row) {
        const idoptions = row.querySelector('input[type="text"]').value;
        let quantityText = row.cells[3].innerText.trim();
        const quantity = parseInt(quantityText.replace(/[^0-9]/g, ''), 10);

        if (!isNaN(quantity) && quantity > 0) {
            const product = {
                idoptions: idoptions,
                quantity: quantity,
            };
            selectedProducts.push(product);
        }
    });

    const shippingMethod = document.getElementById('shippingMethod').value;
    const data = {
        selectedProducts,
        shippingMethod: shippingMethod,
        customerName: document.getElementById('customerName').value,
        customerPhone: document.getElementById('customerPhone').value,
        customerPhoneNumber: document.getElementById('customerPhoneNumber').value,
        city: document.getElementById('city').value,
        district: document.getElementById('district').value,
        ward: document.getElementById('ward').value,
        street: document.getElementById('street').value,
        shippingFee: document.getElementById('shippingFee').innerText.trim(),
        shippingFeeDisplay: document.getElementById('shippingFeeDisplay').innerText.trim(),
        noteOrder: document.getElementById('note_order').value,
        paymentMethodSelect: document.getElementById('paymentMethodSelect').value,
        totalPay: document.getElementById('total_pay').innerText.trim(),
        moneyGivenByGuests: document.getElementById('money_given_by_guests').value,
        customersStillOwe: document.getElementById('customers_still_owe').innerText.trim()
    };

    if (shippingMethod === '1') {
        data.shippingAddress = document.getElementById('shippingAddress').value;
    }

    const invoiceNumberDisplay = document.getElementById('invoiceNumberDisplay').innerText;
    const tabName = invoiceNumberDisplay.replace('Hóa đơn số: ', '').replace(/\s/g, '');

    document.cookie = `${'HoaDon' + tabName}=${encodeURIComponent(JSON.stringify(data))}; path=/; max-age=86400;`;

    console.log('Data saved to cookies:', data);
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
function setCookie(name, value, days) {
    let expires = "";
    if (days) {
        const date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}
function loadDataFromCookies() {
    const invoiceNumberDisplay = document.getElementById('invoiceNumberDisplay').innerText;
    const tabName = invoiceNumberDisplay.replace('Hóa đơn số: ', '').replace(/\s/g, '');
    const cookieName = 'HoaDon' + tabName;
    const cookieValue = document.cookie.split('; ').find(row => row.startsWith(cookieName + '='));

    if (cookieValue) {
        const data = JSON.parse(decodeURIComponent(cookieValue.split('=')[1]));

        document.getElementById('customerName').value = data.customerName || '';
        document.getElementById('customerPhone').value = data.customerPhone || '';
        document.getElementById('customerPhoneNumber').value = data.customerPhoneNumber || '';
        document.getElementById('city').value = data.city || '';
        document.getElementById('district').value = data.district || '';
        document.getElementById('ward').value = data.ward || '';
        document.getElementById('street').value = data.street || '';
        document.getElementById('shippingFee').innerText = data.shippingFee || '';
        document.getElementById('shippingFeeDisplay').innerText = data.shippingFeeDisplay || '';
        document.getElementById('note_order').value = data.noteOrder || '';
        document.getElementById('paymentMethodSelect').value = data.paymentMethodSelect || '';
        document.getElementById('money_given_by_guests').value = data.moneyGivenByGuests || '';
        document.getElementById('customers_still_owe').innerText = data.customersStillOwe || '';

        if (data.shippingMethod === '1') {
            document.getElementById('shippingAddress').value = data.shippingAddress || '';
        }

        console.log('Data loaded from cookies:', data);
    }
}
document.addEventListener('DOMContentLoaded', function () {
    loadDataFromCookies();
    attachEventListeners();
});
function attachEventListeners() {
    const elementsToWatch = [
        '#customerPhone',
        '#customerName',
        '#customerPhoneNumber',
        '#city',
        '#district',
        '#ward',
        '#street',
        '#note_order',
        '#paymentMethodSelect',
        '#coupound',
        '#money_given_by_guests',
        '#shippingFee'
    ];

    elementsToWatch.forEach(selector => {
        document.querySelector(selector).addEventListener('input', saveDataToCookies);
    });

    document.querySelectorAll('#selectedProductsList tbody .selected-product-item .quantity-cell').forEach(cell => {
        cell.addEventListener('input', saveDataToCookies);
    });

    const observer = new MutationObserver(saveDataToCookies);
    const targetNode = document.querySelector('#selectedProductsList tbody');
    if (targetNode) {
        observer.observe(targetNode, { childList: true, subtree: true });
    }

    document.querySelectorAll('#selectedProductsList tbody .selected-product-item .product-quantity').forEach(span => {
        const row = span.closest('.selected-product-item');
        const decreaseBtn = document.getElementById('btn_decreaseQuantity');
        const increaseBtn = document.getElementById('btn_increaseQuantity');

        if (decreaseBtn) {
            decreaseBtn.addEventListener('click', saveDataToCookies);
        }
        if (increaseBtn) {
            increaseBtn.addEventListener('click', saveDataToCookies);
        }
    });

    document.querySelectorAll('#selectedProductsList tbody .selected-product-item button[data-option-id]').forEach(button => {
        button.addEventListener('click', saveDataToCookies);
    });
}
function getOrderDataFromCookies(invoiceNumber) {
    var cookieName = 'HoaDon' + invoiceNumber;
    var cookieValue = getCookieValue(cookieName);
    return cookieValue ? JSON.parse(decodeURIComponent(cookieValue)) : null;
}
function displayOrders(orders) {
    orders.forEach(order => updateFormData(order));
}
async function updateFormData(data) {
    if (data.customerPhoneNumber) {
        const phoneNumberInput = document.getElementById('customerPhoneNumber');
        phoneNumberInput.value = data.customerPhoneNumber;
        searchCustomerByPhoneNumber(data.customerPhoneNumber);
    }
    if (data.selectedProducts && Array.isArray(data.selectedProducts)) {
        const selectedProductsList = document.getElementById('selectedProductsList').querySelector('tbody');
        selectedProductsList.innerHTML = '';

        const fetchProductDetails = (idoptions, quantity) => {
            return new Promise((resolve, reject) => {
                var xhr = new XMLHttpRequest();
                xhr.open('GET', `https://localhost:7241/api/Options/GetByID/${idoptions}`, true);
                xhr.onload = function () {
                    if (xhr.status === 200) {
                        resolve({ ...JSON.parse(xhr.responseText), quantity });
                    } else {
                        reject(`Error: ${xhr.statusText}`);
                    }
                };
                xhr.onerror = function () {
                    reject('Network error');
                };
                xhr.send();
            });
        };

        const productPromises = data.selectedProducts.map(product => fetchProductDetails(product.idoptions, product.quantity));

        Promise.all(productPromises)
            .then(products => {
                products.forEach(product => {
                    const { id, imageURL, keyCode, sizesName, colorName, retailPrice, quantity } = product;
                    const totalPrice = retailPrice * quantity;
                    const productRow = createProductRow({ id, imageURL, keyCode, sizesName, colorName, retailPrice }, quantity, totalPrice, id);
                    selectedProductsList.appendChild(productRow);
                    addToOrderDetails(product.id, quantity);
                });
                calculateSubtotal();
                updateSubmitButtonState();
            })
            .catch(error => console.error('Error fetching product details:', error));
    }

    const fields = [
        'customerName', 'customerPhone', 'street', 'note_order',
        'shippingMethod', 'paymentMethodSelect', 'coupound', 'customerPhoneNumber'
    ];

    fields.forEach(field => {
        if (data[field] !== null && data[field] !== undefined) {
            const element = document.getElementById(field);
            if (element) {
                element.value = data[field];
            }
        }
    });

    fields.forEach(field => {
        if (data[field] !== null && data[field] !== undefined) {
            const element = document.getElementById(field);
            if (element) {
                if (field === 'shippingFee') {
                    element.innerText = data[field];
                    const formattedFee = parseFloat(data[field]).toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
                    document.getElementById('shippingFeeDisplay').innerText = formattedFee;
                } else {
                    element.value = data[field];
                }
            }
        }
    });
    if (data.moneyGivenByGuests !== undefined && data.moneyGivenByGuests !== null) {
        const moneyGivenElement = document.getElementById('money_given_by_guests');
        if (moneyGivenElement) {
            moneyGivenElement.value = data.moneyGivenByGuests;
            const formattedMoneyGiven = parseFloat(data.moneyGivenByGuests).toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
            document.getElementById('money_given_by_guests').innerText = formattedMoneyGiven;
        }
    }
    const shippingMethodSelect = document.getElementById("shippingMethod");
    const shippingDetails = document.getElementById("shippingDetails");

    if (data.shippingMethod) {
        shippingMethodSelect.value = data.shippingMethod;
        shippingDetails.style.display = data.shippingMethod === "1" ? 'flex' : 'none';
    }

    shippingMethodSelect.addEventListener('change', function () {
        shippingDetails.style.display = this.value === "1" ? 'flex' : 'none';
    });

    const provinceID = await fetchProvinces(data.city);

    if (!provinceID) {
        console.log("Không tìm thấy mã tỉnh/thành phố.");
        return;
    }

    const districtID = await fetchDistricts(provinceID, data.district);

    if (!districtID) {
        console.log("Không tìm thấy mã huyện/quận.");
        return;
    }

    const wardID = await fetchWards(districtID, data.ward);

    if (!wardID) {
        console.log("Không tìm thấy mã xã/phường.");
        return;
    }
    populateSelectElement('city', data.city);
    populateSelectElement('district', data.district);
    populateSelectElement('ward', data.ward);
    updateShippingAddress();
    try {
        await getShippingFee(provinceID, districtID, wardID);
    } catch (error) {
        console.error('Error fetching shipping fee:', error);
    }
}
document.addEventListener('DOMContentLoaded', function () {
    var shippingMethodSelect = document.getElementById("shippingMethod");
    var citySelect = document.getElementById("city");
    var districtSelect = document.getElementById("district");
    var wardSelect = document.getElementById("ward");
    var streetInput = document.getElementById("street");
    var shippingAddressInput = document.getElementById("shippingAddress");
    var shippingDetails = document.getElementById("shippingDetails");
    var customerName = document.getElementById("customerName");
    var customerPhone = document.getElementById("customerPhone");
    var shippingFee = document.getElementById("shippingFee");
    var shippingFeeDisplay = document.getElementById("shippingFeeDisplay");

    document.getElementById('btnLuuDonHang').addEventListener('click', function (event) {
        if (validateShippingDetails()) {
            submitOrder('https://localhost:7241/api/Order/create?printInvoice=false', false);
        } else {
            event.preventDefault();
        }
    });
    document.getElementById('btnLuu_InHoaDon').addEventListener('click', function (event) {
        if (validateShippingDetails()) {
            submitOrder('https://localhost:7241/api/Order/create?printInvoice=true', true);
        } else {
            event.preventDefault();
        }
    });
    function validateShippingDetails() {
        if (shippingMethodSelect.value === "1") {
            if (customerName.value.trim() === "" || customerPhone.value.trim() === "" || citySelect.value === "" || districtSelect.value === "" || wardSelect.value === "" || streetInput.value.trim() === "") {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi!',
                    text: 'Vui lòng điền đầy đủ thông tin giao hàng.',
                });
                return false;
            }
        }
        return true;
    }
    const cookieName = `HoaDon${tabCount}`;
    let cookieData = getCookie(cookieName);
    const data = JSON.parse(decodeURIComponent(cookieData));
    shippingMethodSelect.addEventListener('change', function () {
        if (this.value === "0") {
            shippingDetails.style.display = 'none';
            shippingAddressInput.value = "Nhận tại cửa hàng";

            data.customerName = null;
            data.customerPhone = null;
            data.city = null;
            data.district = null;
            data.ward = null;
            data.street = null;
            data.shippingFee = "0";
            data.shippingFeeDisplay = "0 ₫";

            setCookie(cookieName, encodeURIComponent(JSON.stringify(data)), 1);

            customerName.value = '';
            customerPhone.value = '';
            streetInput.value = '';
            wardSelect.value = '';
            districtSelect.value = '';
            citySelect.value = '';

            shippingFee.innerText = "0";
            shippingFeeDisplay.innerText = "0 ₫";
            calculateTotalPay();
        } else {
            shippingDetails.style.display = 'flex';
            updateShippingAddress();
        }
    });

    citySelect.addEventListener('change', updateShippingAddress);
    districtSelect.addEventListener('change', updateShippingAddress);
    wardSelect.addEventListener('change', updateShippingAddress);
    streetInput.addEventListener('input', updateShippingAddress);

    if (shippingMethodSelect.value === "0") {
        shippingDetails.style.display = 'none';
    } else {
        shippingDetails.style.display = 'flex';
        updateShippingAddress();
    }

    function submitOrder(url, printInvoice) {
        const orderData = fillOrderData(selectedUser);
        console.log(orderData);
        var modalElement = document.getElementById('qrModal');
        var qrModal = bootstrap.Modal.getOrCreateInstance(modalElement);

        if (orderData.paymentMethods === 0) {
            var totalPayElement = document.getElementById('total_pay');
            var totalPay = totalPayElement ? totalPayElement.textContent : "0";

            var totalPayFormatted = totalPay.replace(/,/g, '').replace(/₫/g, '').replace(/\./g, '');

            const qrUrl = `https://img.vietqr.io/image/TPB-76312003968-compact.png?amount=${totalPayFormatted}&addInfo=${orderData.hexCode}`;

            document.getElementById('qrImage').src = qrUrl;
            document.getElementById('totalPayText').innerText = totalPay;
            document.getElementById('orderHexCode').innerText = orderData.hexCode;

            qrModal.show();

            document.getElementById('confirmPaymentButton').addEventListener('click', function () {
                Swal.fire({
                    title: 'Xác nhận thanh toán',
                    text: 'Bạn có chắc chắn đã thực hiện thanh toán không?',
                    icon: 'question',
                    showCancelButton: true,
                    confirmButtonText: 'Đồng ý',
                    cancelButtonText: 'Hủy'
                }).then((result) => {
                    if (result.isConfirmed) {
                        qrModal.hide();
                        showLoader();
                        sendOrderRequest(url, printInvoice);
                    } else {
                        Swal.fire({
                            title: 'Đã hủy',
                            text: 'Thanh toán chưa được xác nhận.',
                            icon: 'info',
                            confirmButtonText: 'OK'
                        });
                    }
                });
            });

            return;
        }


        var customers_still_owe = document.getElementById('customers_still_owe').innerText;
        var numericValue = customers_still_owe.replace(/[^\d]/g, '');
        var amountOwed = parseFloat(numericValue);

        if (amountOwed !== 0) {
            Swal.fire({
                title: 'Lỗi!',
                text: 'Khách hàng cần thanh toán tiền hàng.',
                icon: 'error',
                confirmButtonText: 'OK'
            });
            return;
        }

        Swal.fire({
            title: printInvoice ? 'Xác nhận lưu và in hóa đơn' : 'Xác nhận lưu đơn hàng',
            text: printInvoice ? 'Bạn có chắc chắn muốn lưu và in hóa đơn này?' : 'Bạn có chắc chắn muốn lưu đơn hàng này?',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Đồng ý',
            cancelButtonText: 'Hủy',
        }).then((result) => {
            if (result.isConfirmed) {
                showLoader();
                sendOrderRequest(url, printInvoice);
            } else if (result.dismiss === Swal.DismissReason.cancel) {
                Swal.fire({
                    title: printInvoice ? 'Đã hủy lưu và in hóa đơn' : 'Đã hủy lưu đơn hàng',
                    text: printInvoice ? 'Đơn hàng chưa được lưu và hóa đơn chưa được in.' : 'Đơn hàng chưa được lưu.',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            }
        });
    }

    function sendOrderRequest(url, printInvoice) {
        var xhr = new XMLHttpRequest();
        xhr.open('POST', url, true);
        xhr.setRequestHeader('Content-Type', 'application/json');

        xhr.onload = function () {
            hideLoader();
            if (xhr.status === 200) {
                var response = JSON.parse(xhr.responseText);
                var pdfUrl = response.pdfUrl;
                document.cookie = 'HoaDon' + invoiceNumber + '=; path=/; max-age=0;';
                Swal.fire({
                    title: 'Thành công!',
                    text: printInvoice ? 'Đơn hàng đã được lưu và hóa đơn đã được in.' : 'Dữ liệu đơn hàng đã được lưu.',
                    icon: 'success',
                    confirmButtonText: 'OK'
                }).then(() => {
                    if (printInvoice) {
                        window.open(`http://127.0.0.1:8080/${pdfUrl}`, '_blank');
                    } else {
                        window.location.href = `/order_success`;
                    }
                });
            } else {
                try {
                    var errorResponse = JSON.parse(xhr.responseText);
                    var errorMessage = errorResponse.message || 'Đã xảy ra lỗi khi lưu đơn hàng.';
                } catch (e) {
                    var errorMessage = 'Đã xảy ra lỗi khi lưu đơn hàng.';
                }

                Swal.fire({
                    title: 'Lỗi!',
                    text: errorMessage,
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            }
        };

        xhr.onerror = function () {
            hideLoader();
            Swal.fire({
                title: 'Lỗi!',
                text: 'Đã xảy ra lỗi kết nối. Vui lòng thử lại sau.' + xhr.responseText,
                icon: 'error',
                confirmButtonText: 'OK'
            });
        };

        xhr.send(JSON.stringify(fillOrderData(selectedUser)));
    }
});
document.addEventListener("DOMContentLoaded", function () {
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
        var citiesSelect = document.getElementById("city");
        var districtsSelect = document.getElementById("district");
        var wardsSelect = document.getElementById("ward");

        citiesSelect.innerHTML = '<option value="" selected>Chọn tỉnh thành</option>';
        data.forEach(function (city) {
            var option = document.createElement("option");
            option.value = city.Name;
            option.setAttribute("data-code", city.Code);
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
                    option.setAttribute("data-code", district.Code);
                    option.textContent = district.Name;
                    districtsSelect.appendChild(option);
                });
            }
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
                        option.setAttribute("data-code", ward.Code);
                        option.textContent = ward.Name;
                        wardsSelect.appendChild(option);
                    });
                }
            }
            calculateShippingFee();
        });

        wardsSelect.addEventListener('change', function () {
            calculateShippingFee();
        });
    }
    async function calculateShippingFee() {
        var city = document.getElementById('city').selectedOptions[0].getAttribute("data-code");
        var district = document.getElementById('district').selectedOptions[0].getAttribute("data-code");
        var ward = document.getElementById('ward').selectedOptions[0].getAttribute("data-code");

        if (city && district && ward) {
            await loadTinhThanh();
        }
    }
    async function loadTinhThanh() {
        var tinhthanh = document.getElementById('city').value;
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
            var quanhuyen = document.getElementById('district').value;
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
            var xaphuong = document.getElementById('ward').value;
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
                console.log('tp: ' + provinceID + ' qh: ' + districtID + ' xã: ' + wardCode);

                await getShippingFee(provinceID, districtID, wardCode);
            } catch (error) {
                console.error('Error:', error);
            }
        }
    }
});
function updateShippingAddress() {
    var city = document.getElementById("city").value;
    var district = document.getElementById("district").value;
    var ward = document.getElementById("ward").value;

    var shippingAddress = "";

    if (ward !== "") {
        shippingAddress += (shippingAddress !== "" ? ', ' : '') + ward;
    }
    if (district !== "") {
        shippingAddress += (shippingAddress !== "" ? ', ' : '') + district;
    }
    if (city !== "") {
        shippingAddress += (shippingAddress !== "" ? ', ' : '') + city;
    }

    document.getElementById("shippingAddress").value = shippingAddress;
}
document.addEventListener('DOMContentLoaded', function () {
    fetchProducts();
    document.getElementById('product').addEventListener('change', function () {
        const selectedProductId = this.value;
        if (selectedProductId) {
            const product = products.find(p => p.id === selectedProductId);
            if (product) {
                updateSizesAndColors(product);
                searchProductById(product.name);
            }
        }
    });
    document.getElementById('size').addEventListener('change', fetchProductDetails);
    document.getElementById('color').addEventListener('change', fetchProductDetails);
});
function fetchProducts() {
    fetch('https://localhost:7241/api/ProductDetails/GetProductDetails_IDNameAsync/ids')
        .then(response => response.json())
        .then(data => {
            products = data;
            const productSelect = document.getElementById('product');
            data.forEach(product => {
                const option = document.createElement('option');
                option.value = product.id;
                option.textContent = product.name;
                productSelect.appendChild(option);
            });
        })
        .catch(error => console.error('Error fetching products:', error));
}
function updateSizesAndColors(product) {
    const sizeSelect = document.getElementById('size');
    const colorSelect = document.getElementById('color');

    sizeSelect.innerHTML = '<option value="">-- Chọn size --</option>';
    const uniqueSizes = [...new Set(product.sizes)];
    uniqueSizes.forEach(size => {
        const option = document.createElement('option');
        option.value = size;
        option.textContent = size;
        sizeSelect.appendChild(option);
    });

    colorSelect.innerHTML = '<option value="">-- Chọn màu --</option>';
    const uniqueColors = [...new Set(product.colors)];
    uniqueColors.forEach(color => {
        const option = document.createElement('option');
        option.value = color;
        option.textContent = color;
        colorSelect.appendChild(option);
    });
}
function fetchProductDetails() {
    const sizeSelect = document.getElementById('size');
    const colorSelect = document.getElementById('color');
    const productSelect = document.getElementById('product');

    const size = encodeURIComponent(sizeSelect.value.trim());
    const color = encodeURIComponent(colorSelect.value.trim());
    const productId = encodeURIComponent(productSelect.value.trim());

    if (productId && size && color && size !== "" && color !== "") {
        const url = `https://localhost:7241/api/ProductDetails/GetProductDetailInfo/${productId}/?size=${size}&color=${color}`;
        console.log('productId:', productId);

        const xhr = new XMLHttpRequest();
        xhr.open('GET', url, true);
        xhr.setRequestHeader('Accept', '*/*');

        xhr.onreadystatechange = function () {
            if (xhr.readyState === XMLHttpRequest.DONE) {
                if (xhr.status === 200) {
                    try {
                        const data = JSON.parse(xhr.responseText);
                        const productDetailsDiv = document.getElementById('productTableBody');
                        productDetailsDiv.innerHTML = `
                            <td>${data.name}</td>
                            <td class="col-2 text-center">${data.color} - ${data.size} <p id="stock_quantity_${data.idOptions}">(SLT: ${data.quantity})</p></td>
                            <td class="col-2 text-center"><img src="${data.urlImg}" alt="${data.productName}" style="width: 50px; height: auto;"></td>
                            <td class="col-2 text-center" style="width:70px" >${data.price.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</td>
                            <td class="col-2 text-center"><input type="number" id="quantity_options_${data.id}"value="1" min="1" style="width: 80px; text-align: center;"></td>
                            <td class="text-center">
                                <button class="btn btn-primary" data-option-id="${data.idOptions}"><i class="fas fa-plus-circle"></i></button>
                                <button class="btn btn-primary btn-sm view" type="button" title="Xem chi tiết")"> <i class="fas fa-eye"></i></button>
                            </td>
                        `;
                        console.log('API Response:', data);
                    } catch (e) {
                        console.error('Error parsing JSON response:', e);
                    }
                } else {
                    console.error('Có lỗi xảy ra khi gọi API. Status:', xhr.status, xhr.statusText);
                }
            }
        };

        xhr.onerror = function () {
            console.error('Có lỗi xảy ra khi gọi API.');
        };

        xhr.send();
    } else {
        console.log('Please select both size and color.');
    }
}
function searchProductById(productName) {
    const xhr = new XMLHttpRequest();
    const encodedProductName = encodeURIComponent(productName);
    const url = 'https://localhost:7241/api/ProductDetails/search_options?productName=' + encodedProductName;
    console.log('url', url);
    xhr.open('GET', url, true);
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.setRequestHeader('Accept', 'application/json');

    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                const response = JSON.parse(xhr.responseText);
                renderOptions(response);
            } else {
                console.error('Error searching products:', xhr.status);
            }
        }
    };

    xhr.send();
}
document.addEventListener('DOMContentLoaded', function () {
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": false,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "10000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };
});
async function fetchProvinces(provinceName) {
    try {
        const response = await fetch('https://online-gateway.ghn.vn/shiip/public-api/master-data/province', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Token': 'd01771f0-3f8b-11ef-8f55-4ee3d82283af'
            }
        });

        const data = await response.json();

        if (data && Array.isArray(data.data)) {
            const cleanedProvinceName = provinceName.replace(/Tỉnh\s*/i, '').trim();

            const province = data.data.find(province => province.ProvinceName.replace(/Tỉnh\s*/i, '').trim() === cleanedProvinceName);
            return province ? province.ProvinceID : null;
        } else {
            console.error('Dữ liệu tỉnh không hợp lệ:', data);
            return null;
        }
    } catch (error) {
        console.error('Lỗi khi lấy dữ liệu tỉnh:', error);
        return null;
    }
}
async function fetchDistricts(provinceID, districtName) {
    try {
        if (!provinceID) {
            console.error('ProvinceID không hợp lệ');
            return null;
        }
        const response = await fetch('https://online-gateway.ghn.vn/shiip/public-api/master-data/district', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Token': 'd01771f0-3f8b-11ef-8f55-4ee3d82283af'
            },
            body: JSON.stringify({ "province_id": provinceID })
        });

        const data = await response.json();

        if (data && Array.isArray(data.data)) {
            const district = data.data.find(district => district.DistrictName === districtName);
            return district ? district.DistrictID : null;
        } else {
            console.error('Dữ liệu quận huyện không hợp lệ:', data);
            return null;
        }
    } catch (error) {
        console.error('Lỗi khi lấy dữ liệu quận huyện:', error);
        return null;
    }
}
async function fetchWards(districtID, wardName) {
    try {
        if (!districtID) {
            console.error('DistrictID không hợp lệ');
            return null;
        }
        const response = await fetch(`https://online-gateway.ghn.vn/shiip/public-api/master-data/ward?district_id=${districtID}`, {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Token': 'd01771f0-3f8b-11ef-8f55-4ee3d82283af'
            }
        });

        const data = await response.json();

        if (data && Array.isArray(data.data)) {
            const ward = data.data.find(ward => ward.WardName.trim() === wardName);
            return ward ? ward.WardCode : null;
        } else {
            console.error('Dữ liệu xã phường không hợp lệ:', data);
            return null;
        }
    } catch (error) {
        console.error('Lỗi khi lấy dữ liệu xã phường:', error);
        return null;
    }
}
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
            shippingFee = responseData.data.total;
            document.getElementById('shippingFee').innerText = shippingFee;
            document.getElementById('shippingFeeDisplay').innerText = shippingFee.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
            calculateTotalPay();
            console.log('Giá:' + shippingFee)
        } else {
            console.error('Error:', data.message);
        }
    } catch (error) {
        console.error('Error:', error);
    }
}
function populateSelectElement(selectElementId, options) {
    const selectElement = document.getElementById(selectElementId);
    if (!(selectElement instanceof HTMLSelectElement)) {
        console.error('selectElement không phải là một phần tử <select>');
        return;
    }

    selectElement.innerHTML = '';

    if (!Array.isArray(options)) {
        options = [options];
    }

    options.forEach(option => {
        const optionElement = document.createElement('option');
        optionElement.value = option.value || option;
        optionElement.textContent = option.text || option;
        selectElement.appendChild(optionElement);
    });
}
document.addEventListener('DOMContentLoaded', function () {
    var openButton = document.getElementById('btn_open_voucher');
    var modal = new bootstrap.Modal(document.getElementById('voucherModal'));
    openButton.addEventListener('click', function () {
        modal.show();
        fetchVouchers(selectedUserId);
    });
});
function fetchVouchers(userId) {
    var xhr = new XMLHttpRequest();
    xhr.open('GET', `https://localhost:7241/api/Voucher/GetVoucherByIDUser/${userId}`, true);
    xhr.setRequestHeader('accept', '*/*');

    xhr.onload = function () {
        if (xhr.status >= 200 && xhr.status < 300) {
            var response = JSON.parse(xhr.responseText);
            var voucherContainer = document.getElementById('voucherContainer');
            voucherContainer.innerHTML = '';
            console.log(response)
            response.forEach(function (voucher) {

                var voucherType = translateOrderType(voucher.type);
                var reducedValue = formatReducedValue(voucher.type, voucher.reducedValue);
                var startDate = formatDate(new Date(voucher.startDate));
                var endDate = formatDate(new Date(voucher.endDate));
                var voucherHtml = `
                    <div class="col-md-4 mb-3 voucher-item" id="voucher_${voucher.id}">
                        <div class="card" style="border: 1px solid #dee2e6;">
                            <div class="card-body" style="padding: 10px;">
                                <h6 class="card-title" style="font-weight: bold;">Mã: <strong class="voucherCode">${voucher.code}</strong></h6>
                                <p class="card-text" style="margin-bottom: 5px;"><strong>Tên:</strong> <span class="voucherName">${voucher.name}</span></p>
                                <p class="card-text" style="margin-bottom: 5px;"><strong>Bắt đầu:</strong> <span class="voucherStartDate">${startDate}</span></p>
                                <p class="card-text" style="margin-bottom: 5px;"><strong>Kết thúc:</strong> <span class="voucherEndDate">${endDate}</span></p>
                                <p class="card-text" style="margin-bottom: 5px;"><strong>Kiểu:</strong> <span class="voucherType">${voucherType}</span></p>
                                <p class="card-text" style="margin-bottom: 5px;"><strong>Giá trị giảm:</strong> <span class="voucherValue">${reducedValue}</span></p>
                                <p class="card-text" style="margin-bottom: 5px;"><strong>Số lượng:</strong> <span class="voucherQuantity">${voucher.quantity}</span></p>
                                <p class="card-text" style="margin-bottom: 5px;"><strong>Số tiền tối thiểu:</strong> <span class="voucherMinimumAmount">${voucher.minimumAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</span></p>
                                <p class="card-text" style="margin-bottom: 5px;"><strong>Số tiền tối đa:</strong> <span class="voucherMaximumAmount">${voucher.maximumAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</span></p>
                                <p class="card-text" style="margin-bottom: 5px;"><strong>Trạng thái:</strong> <span class="voucherStatus">${translateVoucherStatus(voucher.isActive)}</span></p>
                                <button type="button" class="btn btn-primary btn-sm useVoucherBtn" data-voucher-id="${voucher.id}">Áp dụng</button>
                            </div>
                        </div>
                    </div>
                `;
                voucherContainer.innerHTML += voucherHtml;
            });

            document.querySelectorAll('.useVoucherBtn').forEach(button => {
                button.addEventListener('click', function () {
                    const voucherId = this.getAttribute('data-voucher-id');
                    applyVoucher(voucherId);
                });
            });
        } else {
            console.error('Failed to fetch vouchers:', xhr.statusText);
        }
    };

    xhr.onerror = function () {
        console.error('Request error...');
    };

    xhr.send();
}
function applyVoucher(voucherId) {
    if (selectedVoucherId) {
        const previousBtn = document.querySelector(`#voucher_${selectedVoucherId} .useVoucherBtn`);
        previousBtn.classList.remove('btn-success');
        previousBtn.classList.add('btn-primary');
        previousBtn.textContent = 'Áp dụng';
    }

    var xhr = new XMLHttpRequest();
    xhr.open('GET', `https://localhost:7241/api/Voucher/GetByID/${voucherId}`, true);
    xhr.setRequestHeader('accept', '*/*');

    xhr.onload = function () {
        if (xhr.status >= 200 && xhr.status < 300) {
            var voucher = JSON.parse(xhr.responseText);

            var totalAmount = parseFloat(document.getElementById('temporary_payment_for_goods').textContent.replace(/[^0-9]/g, '')) || 0;

            if (voucher.isActive === 0 || voucher.isActive === 2 || voucher.quantity <= 0 || voucher.minimumAmount > totalAmount) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Không thể áp dụng',
                    text: `Voucher ${voucher.code} không khả dụng.`,
                });
                return;
            }

            let reducedValue = 0;
            if (voucher.type === 0) {
                reducedValue = totalAmount * (voucher.reducedValue / 100);
            } else if (voucher.type === 1) {
                reducedValue = voucher.reducedValue;
            }

            const selectedBtn = document.querySelector(`#voucher_${voucherId} .useVoucherBtn`);
            selectedBtn.classList.remove('btn-primary');
            selectedBtn.classList.add('btn-success');
            selectedBtn.textContent = 'Đã chọn';

            selectedVoucherId = voucherId;
            document.getElementById('selectedVoucherCode').textContent = voucher.code;


            const couponInput = document.getElementById('coupound');

            if (couponInput) {
                couponInput.value = reducedValue.toFixed(0);
                formatAndCalculateTotalPay();
            } else {
                console.error('Element with ID couponInput not found.');
            }

            console.log('Mã voucher được chọn:', voucher.code);
            voucherCode = voucher.code;

        } else {
            console.error('Failed to fetch voucher details:', xhr.statusText);
        }
    };

    xhr.onerror = function () {
        console.error('Request error...');
    };

    xhr.send();
}
function translateOrderType(type) {
    switch (type) {
        case 0:
            return 'Giảm phần trăm';
        case 1:
            return 'Giảm tiền mặt';
        default:
            return 'Không xác định';
    }
}
function translateVoucherStatus(type) {
    switch (type) {
        case 0:
            return '<span class="badge bg-warning">Chưa bắt đầu</span>';
        case 1:
            return '<span class="badge bg-success">Đang bắt đầu</span>';
        case 2:
            return '<span class="badge bg-danger">Đã kết thúc</span>';
        default:
            return '<span class="badge bg-secondary">Không xác định</span>';
    }
}
function formatReducedValue(type, value) {
    if (type === 0) {
        return `${value}%`;
    } else if (type === 1) {
        return value.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
    }
    return value;
}
function formatDate(date) {
    return date.toLocaleTimeString() + ' ' + date.toLocaleDateString();
}
//function openModal() {
//    document.getElementById('barcodeModal').style.display = 'block';
//}
//function closeModal() {
//    document.getElementById('barcodeModal').style.display = 'none';
//    stopBarcodeScanner();
//}
//document.getElementById('btn_barcode_sweep').addEventListener('click', function () {
//    openModal();
//    startBarcodeScanner();
//});
//function startBarcodeScanner() {
//    const quaggaConf = {
//        inputStream: {
//            name: "Live",
//            type: "LiveStream",
//            target: document.querySelector("#camera"),
//            constraints: {
//                width: { min: 640 },
//                height: { min: 480 },
//                facingMode: "environment"
//            }
//        },
//        decoder: {
//            readers: ['code_128_reader'],
//            multiple: false
//        },
//        locate: true,
//        frequency: 50
//    };


//    Quagga.init(quaggaConf, function (err) {
//        if (err) {
//            console.error("Quagga initialization error: ", err);
//            return;
//        }
//        console.log("Quagga initialized");
//        Quagga.start();
//    });

//    Quagga.onDetected(function (result) {
//        console.log("Detected barcode: " + result.codeResult.code);
//        Quagga.stop();

//    });
//}
//function stopBarcodeScanner() {
//    Quagga.stop();
//}
//document.querySelector('.close-button').addEventListener('click', function () {
//    closeModal();
//});

document.getElementById('openModalBtn').addEventListener('click', function () {
    var modal = new bootstrap.Modal(document.getElementById('modal_user'));
    console.log('click')
    modal.show();
});
document.getElementById('saveUserBtn').addEventListener('click', function () {
    var username = document.getElementById('username').value;
    var name = document.getElementById('name_user').value;
    var email = document.getElementById('email_user').value;
    var phone = document.getElementById('phone').value;

    Swal.fire({
        title: 'Xác nhận?',
        text: "Bạn có chắc chắn muốn tạo mới khách hàng này?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Xác nhận',
        cancelButtonText: 'Hủy bỏ'
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire({
                title: 'Đang xử lý...',
                text: 'Vui lòng chờ trong giây lát',
                allowOutsideClick: false,
                didOpen: () => {
                    Swal.showLoading();
                }
            });
            var data = JSON.stringify({
                "firstAndLastName": name,
                "username": username,
                "email": email,
                "phoneNumber": phone
            });

            var xhr = new XMLHttpRequest();
            xhr.open("POST", "https://localhost:7241/api/ApplicationUser/register_with_random_password?role=client", true);
            xhr.setRequestHeader("Content-Type", "application/json");

            xhr.onreadystatechange = function () {
                if (xhr.readyState === 4) {
                    Swal.close();

                    if (xhr.status === 200) {
                        var response = JSON.parse(xhr.responseText);

                        Swal.fire({
                            title: 'Thành công!',
                            text: response.message,
                            icon: 'success'
                        }).then(() => {
                            $('#modal_user').modal('hide');
                            document.getElementById('customerPhoneNumber').value = phone;
                        });
                    } else {
                        Swal.fire({
                            title: 'Lỗi!',
                            text: 'Có lỗi xảy ra. Vui lòng thử lại.',
                            icon: 'error'
                        });
                    }
                }
            };
            xhr.send(data);
        }
    });
});
