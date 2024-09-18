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
function checkAuthentication() {
    if (!jwt || !userId) {
        window.location.href = '/login';
        return false;
    }
    return true;
}
checkAuthentication();

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
document.getElementById('btn_reload').addEventListener('click', () => {
    getOptions();
});
function renderOptions(data) {
    const tableBody = document.getElementById('productTableBody');
    tableBody.innerHTML = '';

    if (Array.isArray(data)) {
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
                    <td class="col-2 text-center"><input type="number" id="quantity_options_${option.id}" value="1" min="1" style="width: 80px; text-align: center;" oninput="validateQuantity(this)"></td>
                    <td class="text-center">
                        <button class="btn btn-primary add-product" data-option-id="${option.id}"><i class="fas fa-plus-circle"></i></button>
                         <button id="btn_view_options" class="btn btn-primary btn-sm view" type="button" title="Xem chi tiết" data-option-id="${option.id}">
                                <i class="fas fa-eye"></i>
                            </button>
                            </td>
                `;

                tableBody.appendChild(row);
                isFirstRow = false;
            });
        });
    } else if (data && typeof data === 'object') {
        const row = document.createElement('tr');
        const currentStock = stockQuantities[data.id] || data.stockQuantity;

        row.innerHTML = `
            <td>${data.productName}<br> (${data.keyCode})</td>
            <td class="col-2 text-center">${data.colorName} - ${data.sizesName} <p id="stock_quantity_${data.id}">(SLT: ${currentStock})</p></td>
            <td class="col-2 text-center"><img src="${data.imageURL}" alt="${data.productName}" style="width: 50px; height: auto;"></td>
            <td class="col-2 text-center" id="retail_price_options" style="width:70px">${data.retailPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</td>
            <td class="col-2 text-center"><input type="number" id="quantity_options_${data.id}" value="1" min="1" style="width: 80px; text-align: center;" oninput="validateQuantity(this)"></td>
            <td class="text-center">
                <button class="btn btn-primary add-product" data-option-id="${data.id}"><i class="fas fa-plus-circle"></i></button>
                <button id="btn_view_options" class="btn btn-primary btn-sm view" type="button" title="Xem chi tiết" data-option-id="${data.id}">
                    <i class="fas fa-eye"></i>
                </button>
            </td>
        `;

        tableBody.appendChild(row);
    } else {
        console.error('Dữ liệu trả về không phải là mảng hoặc đối tượng mong đợi.');
    }

    document.getElementById('productTableBody').addEventListener('click', function (event) {
        if (event.target.closest('.view')) {
            const optionId = event.target.closest('.view').getAttribute('data-option-id');
            fetchOptionDetails(optionId);
        }
    });

    function fetchOptionDetails(optionId) {
        fetch(`https://localhost:7241/api/Options/GetByID/${optionId}`)
            .then(response => response.json())
            .then(data => {
                if (data) {

                    document.getElementById('imageURL').src = data.imageURL;
                    document.getElementById('productDetails').value = data.productName;
                    document.getElementById('color_options').value = data.colorName;
                    document.getElementById('size_options').value = data.sizesName;
                    document.getElementById('stockQuantity').value = data.stockQuantity;
                    document.getElementById('retailPrice').value = data.retailPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
                    document.getElementById('isActive').value = data.isActive ? 'Đang bán' : 'Ngừng bán';

                    const optionsModal = new bootstrap.Modal(document.getElementById('optionsModal'));
                    optionsModal.show();
                }
            })
            .catch(error => console.error('Có lỗi xảy ra:', error));
    }
    tableBody.removeEventListener('click', handleAddProduct);
    tableBody.addEventListener('click', handleAddProduct);
}
function validateQuantity(input) {
    const min = 1;
    let value = parseInt(input.value);

    if (isNaN(value) || value < min) {
        input.value = min;
        toastr.error('Số lượng phải là số nguyên dương và lớn hơn 0.', 'Lỗi nhập liệu');
    }

    if (!Number.isInteger(value)) {
        input.value = Math.floor(value);
    }
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
function fillOrderData(users) {
    const note = document.getElementById('note_order').value;
    const paymentMethod = document.getElementById('paymentMethodSelect').value;
    const shippingMethodSelect = document.getElementById('shippingMethod');
    const shippingMethodValue = shippingMethodSelect.value;
    const customerName_gui = document.getElementById('customerName').value.trim();
    const customerPhone_gui = document.getElementById('customerPhone').value.trim();
    const shippingAddress = document.getElementById('shippingAddress').value.trim();
    const streetInput = document.getElementById('street').value.trim();
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
        shippingAddressLine2: streetInput || 'Nhận tại cửa hàng',
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
function createOrderData({
    idUser,
    customerName_gui,
    customerPhone_gui,
    customerEmail,
    shippingAddress,
    shippingAddressLine2,
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
        shippingAddressLine2: shippingAddressLine2 || 'Nhận tại cửa hàng',
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
async function displaySearchResults(users) {
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
    const shippingMethodSelect = document.getElementById('shippingMethod');

    const selectedValue = shippingMethodSelect.value;

    if (selectedValue === '1') {
        if (users.length > 0) {
            const defaultAddress = users[0].addressVMs.find(address => address.isDefault);

            if (defaultAddress) {
                document.getElementById('customerName').value = defaultAddress.firstAndLastName;
                document.getElementById('customerPhone').value = defaultAddress.phoneNumber;
                document.getElementById('street').value = defaultAddress.specificAddress;

                try {
                    const provinceID = await fetchProvinces(defaultAddress.city);

                    if (!provinceID) return;

                    const districtID = await fetchDistricts(provinceID, defaultAddress.districtCounty);

                    if (!districtID) {
                        console.log("Không tìm thấy mã huyện/quận.");
                        return;
                    }

                    const wardID = await fetchWards(districtID, defaultAddress.commune);

                    if (!wardID) {
                        console.log("Không tìm thấy mã xã/phường.");
                        return;
                    }

                    populateSelectElement('city', defaultAddress.city);
                    populateSelectElement('district', defaultAddress.districtCounty);
                    populateSelectElement('ward', defaultAddress.commune);
                    updateShippingAddress();

                    await getShippingFee(provinceID, districtID, wardID);
                } catch (error) {
                    console.error('Error fetching shipping fee:', error);
                }
            } else {
                console.warn('Không tìm thấy địa chỉ mặc định cho số điện thoại này');
            }
        }
    }
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
                if (stockQuantity === 0) {
                    Swal.fire(
                        'Lỗi!',
                        'Số lượng sản phẩm không đủ!',
                        'error'
                    );
                    return;
                }
                if (currentQuantity !== 0) {
                    currentQuantity += 1;
                    quantityElement.textContent = currentQuantity;

                    const priceElement = row.querySelector('.product-total');
                    const pricePerUnitElement = document.getElementById(`retal_price_options_cart_${optionId}`);
                    if (!pricePerUnitElement) {
                        console.error('Không tìm thấy phần tử giá với id:', `retal_price_options_cart_${optionId}`);
                        return;
                    }
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

                    const xhrUpdate = new XMLHttpRequest();
                    xhrUpdate.open('POST', 'https://localhost:7241/api/Options/decrease-quantity', true);
                    xhrUpdate.setRequestHeader('Content-Type', 'application/json');
                    xhrUpdate.setRequestHeader('Accept', '*/*');

                    xhrUpdate.onreadystatechange = function () {
                        if (xhrUpdate.readyState === XMLHttpRequest.DONE) {
                            if (xhrUpdate.status === 200) {
                                updateProductQuantity(option.id, selectedQuantities[optionId]);
                                updateStockDisplay(optionId, stockQuantity - 1);
                                calculateSubtotal();

                                connection.invoke("UpdateProductQuantity", optionId, stockQuantities[optionId])
                                    .catch(err => console.error('Error sending stock update:', err));
                            } else {
                                console.error('Error updating quantity:', xhrUpdate.status, xhrUpdate.statusText);
                                console.error('Response Text:', xhrUpdate.responseText);
                            }
                        }
                    };

                    const requestData = JSON.stringify({
                        idOptions: optionId,
                        quantityToDecrease: 1
                    });
                    xhrUpdate.send(requestData);

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
                    const pricePerUnitElement = document.getElementById(`retal_price_options_cart_${optionId}`);
                    if (!pricePerUnitElement) {
                        console.error('Không tìm thấy phần tử giá với id:', `retal_price_options_cart_${optionId}`);
                        return;
                    }
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

                    updateStockDisplay(optionId, stockQuantity + 1);
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
        return;
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
                            getOptions();
                            toastr.success('Cập nhật thành công!', 'Thành công');
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
                toastr.error(xhrGet.responseText, 'Lỗi');

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

                if (currentStockQuantity === 0) {
                    Swal.fire(
                        'Lỗi!',
                        'Số lượng sản phẩm không đủ!',
                        'error'
                    );
                    return;
                }

                const xhrUpdate = new XMLHttpRequest();
                xhrUpdate.open('POST', 'https://localhost:7241/api/Options/decrease-quantity', true);
                xhrUpdate.setRequestHeader('Content-Type', 'application/json');
                xhrUpdate.setRequestHeader('Accept', '*/*');

                xhrUpdate.onreadystatechange = function () {
                    if (xhrUpdate.readyState === XMLHttpRequest.DONE) {
                        if (xhrUpdate.status === 200) {
                            selectedQuantities[optionId] = totalQuantity;
                            let totalPrice = option.retailPrice * newQuantity;
                            if (Number.isNaN(totalPrice) || !Number.isFinite(totalPrice)) {
                                toastr.error('Số lượng không hợp lệ.', 'Lỗi nhập liệu');
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

                            connection.invoke("UpdateProductQuantity", optionId, stockQuantities[optionId])
                                .catch(err => console.error('Error sending stock update:', err));

                            addToOrderDetails(option.id, newQuantity);
                            updateSubmitButtonState();
                            calculateSubtotal();

                        } else {
                            console.error('Error updating stock quantity:', xhrUpdate.responseText);
                            toastr.error(xhrUpdate.responseText, 'Lỗi');
                        }
                    }
                };

                const requestData = JSON.stringify({
                    idOptions: optionId,
                    quantityToDecrease: newQuantity
                });

                xhrUpdate.send(requestData);

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
        <td class="text-center product-price" id="retal_price_options_cart_${option.id}">${option.retailPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</td>
        <td class="text-center">
            <button id="btn_decreaseQuantity" onclick="decreaseQuantity(this)" 
                style="padding: 3px 6px; background-color: #f44336; color: white; border: none; border-radius: 4px; margin-right: 5px;">-</button>
            <span class="product-quantity">${newQuantity}</span>
            <button id="btn_increaseQuantity" onclick="increaseQuantity(this)" 
                style="padding: 3px 6px; background-color: #4CAF50; color: white; border: none; border-radius: 4px; margin-left: 5px;">+</button>
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
        customerPhoneNumber: document.getElementById('customerPhoneNumber').value,
        noteOrder: document.getElementById('note_order').value,
        paymentMethodSelect: document.getElementById('paymentMethodSelect').value,
        moneyGivenByGuests: document.getElementById('money_given_by_guests').value,
    };

    if (shippingMethod === '1') {
        data.shippingAddress = document.getElementById('shippingAddress').value;
    }

    const invoiceNumberDisplay = document.getElementById('invoiceNumberDisplay').innerText;
    const tabName = invoiceNumberDisplay.replace('Hóa đơn số: ', '').replace(/\s/g, '');

    document.cookie = `${'HoaDon' + tabName}=${encodeURIComponent(JSON.stringify(data))}; path=/; max-age=86400;`;
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
        '#note_order',
        '#paymentMethodSelect',
        '#money_given_by_guests',
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
            decreaseBtn.addEventListener('click', function (event) {
                event.preventDefault();
                event.stopPropagation();
                saveDataToCookies();
            });
        }
        if (increaseBtn) {
            increaseBtn.addEventListener('click', function (event) {
                event.preventDefault();
                event.stopPropagation();
                saveDataToCookies();
            });
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
async function updateAddressFromUser(user) {
    if (user && user.addressVMs) {
        const defaultAddress = user.addressVMs.find(address => address.isDefault);

        if (defaultAddress) {
            document.getElementById('customerName').value = defaultAddress.firstAndLastName;
            document.getElementById('customerPhone').value = defaultAddress.phoneNumber;
            document.getElementById('street').value = defaultAddress.specificAddress;

            try {
                const provinceID = await fetchProvinces(defaultAddress.city);
                const districtID = await fetchDistricts(provinceID, defaultAddress.districtCounty);
                const wardID = await fetchWards(districtID, defaultAddress.commune);

                populateSelectElement('city', defaultAddress.city);
                populateSelectElement('district', defaultAddress.districtCounty);
                populateSelectElement('ward', defaultAddress.commune);
                await updateShippingAddress();
                await getShippingFee(provinceID, districtID, wardID);
            } catch (error) {
                console.error('Error fetching address details:', error);
            }
        } else {
            console.warn('Không tìm thấy địa chỉ mặc định cho số điện thoại này');
        }
    }
}
function setupShippingMethodEvents() {
    const shippingMethodSelect = document.getElementById('shippingMethod');
    const shippingDetails = document.getElementById('shippingDetails');
    const citySelect = document.getElementById('city');
    const districtSelect = document.getElementById('district');
    const wardSelect = document.getElementById('ward');
    const streetInput = document.getElementById('street');
    const shippingAddressInput = document.getElementById('shippingAddress');

    shippingMethodSelect.addEventListener('change', async function () {
        if (this.value === "0") {
            shippingDetails.style.display = 'none';
            shippingAddressInput.value = "Nhận tại cửa hàng";
            calculateTotalPay();
        } else {
            shippingDetails.style.display = 'flex';

            if (selectedUser) {
                await updateAddressFromUser(selectedUser);

            } else {
                updateShippingAddress();
            }
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
        if (selectedUser) {
            updateAddressFromUser(selectedUser);
        } else {
            updateShippingAddress();
        }
    }
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
        shippingDetails.style.display = (data.customerPhoneNumber && data.shippingMethod === "1") ? 'flex' : 'none';
    }

    shippingMethodSelect.addEventListener('change', function () {
        const shouldShowDetails = this.value === "1" && document.getElementById('customerPhoneNumber').value;
        shippingDetails.style.display = shouldShowDetails ? 'flex' : 'none';
    });


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
    var customerPhoneNumber = document.getElementById("customerPhoneNumber");

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
            if (customerName.value.trim() === "" ||
                customerPhone.value.trim() === "" ||
                citySelect.value === "" ||
                districtSelect.value === "" ||
                wardSelect.value === "" ||
                streetInput.value.trim() === "") {

                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi!',
                    text: 'Vui lòng điền đầy đủ thông tin giao hàng.',
                });
                return false;
            }

            if (customerPhoneNumber.value.trim() === '') {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi!',
                    text: 'Phải có tài khoản mới có thể đặt giao hàng.',
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

            setCookie(cookieName, encodeURIComponent(JSON.stringify(data)), 1);

            calculateTotalPay();
        } else {
            shippingDetails.style.display = 'flex';
            setupShippingMethodEvents();

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
                    }
                    window.location.href = `/order_success`;
                });
            } else {
                Swal.fire({
                    title: 'Lỗi!',
                    text: xhr.responseText,
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
        const url = `https://localhost:7241/api/Options/find-by-standard?IDProductDetails=${productId}&size=${size}&color=${color}`;
        console.log('productId:', productId);

        const xhr = new XMLHttpRequest();
        xhr.open('GET', url, true);
        xhr.setRequestHeader('Accept', '*/*');

        xhr.onreadystatechange = function () {
            if (xhr.readyState === XMLHttpRequest.DONE) {
                if (xhr.status === 200) {
                    try {
                        const data = JSON.parse(xhr.responseText);
                        console.log('API Response:', data);

                        renderOptions(data);
                    } catch (e) {
                        console.error('Error parsing JSON response:', e);
                        console.error(xhr.responseText);
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
    const url = 'https://localhost:7241/api/ProductDetails/search-options?productName=' + encodedProductName;
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

        if (responseData.code === 200) {
            shippingFee = responseData.data.total;
            document.getElementById('shippingFee').innerText = shippingFee;
            document.getElementById('shippingFeeDisplay').innerText = shippingFee.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
            calculateTotalPay();
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
    xhr.open('GET', `https://localhost:7241/api/Voucher/vouchers-public-private?idUser=${userId}`, true);
    xhr.setRequestHeader('accept', '*/*');

    xhr.onload = function () {
        if (xhr.status >= 200 && xhr.status < 300) {
            var response = JSON.parse(xhr.responseText);
            var voucherContainer = document.getElementById('voucherContainer');
            voucherContainer.innerHTML = '';
            response.forEach(function (voucher) {

                var voucherType = translateOrderType(voucher.type);
                var reducedValue = formatReducedValue(voucher.type, voucher.reducedValue);
                var startDate = formatDate(new Date(voucher.startDate));
                var endDate = formatDate(new Date(voucher.endDate));
                var voucherHtml = `
                    <div class="col-md-4 mb-3 voucher-item" id="voucher_${voucher.id}">
                        <div class="card" style="border: 1px solid #dee2e6;">
                            <div class="card-body" style="padding: 10px;">
                                <h6 class="card-title" style="font-weight: bold;">
                                    Mã: <strong class="voucherCode">${voucher.code}</strong>
                                    <small style="color: ${voucher.isUsed === 1 ? 'green' : 'red'};">
                                        ${voucher.isUsed === 0 ? 'Chưa dùng' : 'Đã dùng'}
                                    </small>
                                </h6>
                                <p class="card-text" style="margin-bottom: 5px;"><strong>Tên:</strong> <span class="voucherName">${voucher.name}</span></p>
                                <p class="card-text" style="margin-bottom: 5px;"><strong>Bắt đầu:</strong> <span class="voucherStartDate">${startDate}</span></p>
                                <p class="card-text" style="margin-bottom: 5px;"><strong>Kết thúc:</strong> <span class="voucherEndDate">${endDate}</span></p>
                                <p class="card-text" style="margin-bottom: 5px;"><strong>Kiểu:</strong> <span class="voucherType">${voucherType}</span></p>
                                <p class="card-text" style="margin-bottom: 5px;"><strong>Giá trị giảm:</strong> <span class="voucherValue">${reducedValue}</span></p>
                                <p class="card-text" style="margin-bottom: 5px;"><strong>Số lượng:</strong> <span class="voucherQuantity">${voucher.quantity}</span></p>
                                <p class="card-text" style="margin-bottom: 5px;">
                                    <strong>Kiểu voucher:</strong>
                                    <span class="voucherStatus">${voucher.status === 1 ? 'Cá nhân' : 'Công khai'}</span>
                                </p>
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

            if (voucher.isActive === 0) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Không thể áp dụng',
                    text: `Voucher ${voucher.code} chưa kích hoạt hoặc đã hết hạn.`,
                });
                return;
            }

            if (voucher.isActive === 2) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Không thể áp dụng',
                    text: `Voucher ${voucher.code} đã bị hủy.`,
                });
                return;
            }

            if (voucher.quantity <= 0) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Không thể áp dụng',
                    text: `Voucher ${voucher.code} đã hết số lượng.`,
                });
                return;
            }

            if (voucher.minimumAmount > totalAmount) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Không thể áp dụng',
                    text: `Voucher ${voucher.code} yêu cầu số tiền tối thiểu là ${voucher.minimumAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}, nhưng tổng tiền của bạn chỉ là ${totalAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}.`,
                });
                return;
            }


            let reducedValue = 0;
            if (voucher.type === 0) {
                reducedValue = totalAmount * (voucher.reducedValue / 100);
            } else if (voucher.type === 1) {
                reducedValue = voucher.reducedValue;
            }
            if (reducedValue > voucher.maximumAmount) {
                reducedValue = voucher.maximumAmount;
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
            toastr.success(`Đã áp dụng thành công voucher ${voucherCode}`, 'Thành công');

        } else {
            console.error('Failed to fetch voucher details:', xhr.statusText);
        }
    };

    xhr.onerror = function () {
        console.error('Request error...');
    };

    xhr.send();
}
function checkVoucherDetails(voucherId) {
    var xhrVoucher = new XMLHttpRequest();
    xhrVoucher.open('GET', `https://localhost:7241/api/Voucher/GetByID/${voucherId}`, true);
    xhrVoucher.setRequestHeader('accept', '*/*');

    xhrVoucher.onload = function () {
        if (xhrVoucher.status >= 200 && xhrVoucher.status < 300) {
            var voucher = JSON.parse(xhrVoucher.responseText);

            var totalAmount = parseFloat(document.getElementById('provisional_fee').textContent.replace(/[^0-9]/g, '')) || 0;

            if (voucher.isActive === 0 || voucher.isActive === 2 || voucher.quantity <= 0 || voucher.minimumAmount > totalAmount) {
                Swal.fire({
                    icon: 'warning',
                    title: 'Không thể áp dụng',
                    text: `Voucher ${voucher.code} không khả dụng.`,
                });
                return;
            }

            const reducedValue = calculateDiscount(voucher, totalAmount);

            const selectedBtn = document.querySelector(`#voucher_${voucherId} .useVoucherBtn`);
            selectedBtn.classList.remove('btn-primary');
            selectedBtn.classList.add('btn-success');
            selectedBtn.textContent = 'Đã chọn';

            selectedVoucherId = voucherId;
            document.getElementById('selectedVoucherCode').textContent = voucher.code;

            const couponInput = document.getElementById('coupound');
            if (couponInput) {
                couponInput.textContent = reducedValue.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
            } else {
                console.error('Element with ID coupound not found.');
            }

            updateTotalOrder();
            console.log('Mã voucher được chọn:', voucher.code);
            voucherCode = voucher.code;

        } else {
            console.error('Failed to fetch voucher details:', xhrVoucher.statusText);
        }
    };

    xhrVoucher.onerror = function () {
        console.error('Request error...');
    };

    xhrVoucher.send();
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

document.getElementById('searchByNameBtn').addEventListener('click', () => {
    const name = document.getElementById('searchProduct').value.trim();
    if (name === '') {
        Swal.fire({
            icon: 'warning',
            title: 'Lỗi',
            text: 'Vui lòng nhập tên sản phẩm!'
        });
        return;
    }

    searchProduct(name);
});
function searchProduct(name) {
    const xhr = new XMLHttpRequest();
    xhr.open('GET', `https://localhost:7241/api/Options/get-options-by-name/${encodeURIComponent(name)}`, true);
    xhr.setRequestHeader('Accept', '*/*');

    xhr.onload = function () {
        if (xhr.status === 200) {
            const data = JSON.parse(xhr.responseText);
            if (data && data.length > 0) {
                console.log('data', data)

                renderOptions(data);
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi',
                    text: 'Không tìm thấy sản phẩm với tên này!'
                });
            }
        } else {
            Swal.fire({
                icon: 'error',
                title: 'Lỗi',
                text: 'Có lỗi xảy ra khi tìm kiếm sản phẩm!'
            });
        }
    };

    xhr.onerror = function () {
        Swal.fire({
            icon: 'error',
            title: 'Lỗi',
            text: 'Đã xảy ra lỗi khi gửi yêu cầu!'

        });
    };

    xhr.send();
}