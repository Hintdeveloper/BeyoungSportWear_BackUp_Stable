var selectedProducts = [];
var selectedQuantities = {};
var orderDetails = [];
let selectedUser = null;
let globalShippingFee = 0;
const stockQuantities = {};
const tamtinhElement = document.querySelector('.control-all-money-tamtinh');
const totalPayElement = document.querySelector('.control-all-money-total');
const couponInput = document.getElementById('coupound');
const moneyGivenByGuestsInput = document.getElementById('money_given_by_guests');
const customersStillOweElement = document.getElementById('customers_still_owe');
const shippingFeeElement = document.getElementById('shippingFeeElement');
couponInput.addEventListener('input', formatAndCalculateTotalPay);
moneyGivenByGuestsInput.addEventListener('input', formatAndCalculateCustomersStillOwe);
var invoiceNumber = getQueryParameter('invoiceNumber');
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
function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}
function getCookie(cookieName) {
    const cookies = document.cookie.split(';');
    for (let cookie of cookies) {
        const [name, value] = cookie.split('=').map(c => c.trim());
        if (name === cookieName) {
            return decodeURIComponent(value);
        }
    }
    return null;
}
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
                updateStockQuantitiesInStorage(data);
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
            const productMap = {};

            data.forEach(product => {
                const key = `${product.keyCode}_${product.productName}`;
                if (!productMap[key]) {
                    productMap[key] = [];
                }
                productMap[key].push(product);
            });

    const stockQuantities = getStockQuantitiesFromStorage(); 

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
                <td class="col-2 text-center" style="width:70px">${option.retailPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</td>
                <td class="col-2 text-center"><input type="number" id="quantity_options_${option.id}" value="1" min="1" style="width: 80px; text-align: center;"></td>
                        <td class="text-center">
                            <button class="btn btn-primary" data-option-id="${option.id}"><i class="fas fa-plus-circle"></i></button>
                    <button class="btn btn-primary btn-sm view" type="button" title="Xem chi tiết"> <i class="fas fa-eye"></i></button>
                        </td>
                    `;

                    tableBody.appendChild(row);
                    isFirstRow = false;
                });
    });

            tableBody.addEventListener('click', function (event) {
        const target = event.target;
        if (target.classList.contains('btn-primary')) {
            const optionId = target.getAttribute('data-option-id');
                    addToSelectedProducts(optionId);
                }
            });
}
window.addToSelectedProducts = function (optionId) {
    var xhr = new XMLHttpRequest();
    xhr.open('GET', `https://localhost:7241/api/Options/GetByID/${optionId}`, true);

    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4) {
            if (xhr.status == 200) {
                var option = JSON.parse(xhr.responseText);
                const selectedProductsList = document.getElementById('selectedProductsList').querySelector('tbody');
                const currentQuantity = selectedQuantities[optionId] || 0;
                const quantityInput = document.getElementById(`quantity_options_${optionId}`);
                const newQuantity = quantityInput ? parseInt(quantityInput.value) : 1;
                const totalQuantity = currentQuantity + newQuantity;

                let stockQuantities = getStockQuantitiesFromStorage();
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
                localStorage.setItem('stockQuantities', JSON.stringify(stockQuantities));

                const stockElement = document.getElementById(`stock_quantity_${optionId}`);
                if (stockElement) {
                    stockElement.textContent = `(SLT: ${stockQuantities[optionId]})`;
                }

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
        <td class="text-center">Size: ${option.sizesName} Color: ${option.colorName}</td>
        <td class="text-center product-price">${option.retailPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</td>
        <td class="text-center"><span class="product-quantity">${newQuantity}</span></td>
        <td class="text-center product-total">${totalPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</td>
        <td class="text-center">
            <button class="btn btn-danger btn-sm" onclick="removeProduct(this)">Xóa</button>
        </td>
    `;

    return productRow;
}
function submitOrder(url, printInvoice) {
    const orderData = fillOrderData(selectedUser);
    console.log(orderData);

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

            var xhr = new XMLHttpRequest();
            xhr.open('POST', url, true);
            xhr.setRequestHeader('Content-Type', 'application/json');

            xhr.onload = function () {
                hideLoader();
                if (xhr.status === 200) {
                    var response = JSON.parse(xhr.responseText);
                    var pdfUrl = response.pdfUrl;

                    if (printInvoice && pdfUrl) {
                        var link = document.createElement('a');
                        link.href = pdfUrl;
                        link.download = pdfUrl.split('/').pop();
                        document.body.appendChild(link);
                        link.click();
                        window.open(pdfUrl, '_blank');
                    }

                    Swal.fire({
                        title: 'Thành công!',
                        text: printInvoice ? 'Đơn hàng đã được lưu và hóa đơn đã được in.' : 'Dữ liệu đơn hàng đã được lưu.',
                        icon: 'success',
                        confirmButtonText: 'OK'
                    }).then(() => {
                        if (!printInvoice) {
                            window.location.href = '/home/index_order';
                        }
                    });
                } else {
                    Swal.fire({
                        title: 'Lỗi!',
                        text: 'Đã xảy ra lỗi khi lưu đơn hàng: ' + xhr.statusText,
                        icon: 'error',
                        confirmButtonText: 'OK'
                    });
                }
            };

            xhr.onerror = function () {
                hideLoader();
                Swal.fire({
                    title: 'Lỗi!',
                    text: 'Đã xảy ra lỗi kết nối. Vui lòng thử lại sau.',
                    icon: 'error',
                    confirmButtonText: 'OK'
                });
            };

            xhr.send(JSON.stringify(orderData));
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
function fillOrderData(users) {
    var note = document.getElementById('note_order').value;
    var paymentMethod = document.getElementById('paymentMethodSelect').value;
    var shippingMethodSelect = document.getElementById('shippingMethod');
    var shippingMethodValue = shippingMethodSelect.value;

    const customerName_gui = document.getElementById('customerName').value.trim();
    const customerPhone_gui = document.getElementById('customerPhone').value.trim();
    var shippingAddress = "";
    var shippingMethods = 1;
    var shippingFeeElement = document.getElementById('shippingFee');
    var costs = 0;

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

    const existingHexCodes = [];
    if (shippingFeeElement) {
        const textContent = shippingFeeElement.textContent.trim();
        const match = textContent.match(/\d+/);
        if (match) {
            costs = parseInt(match[0]);
        }
    }

    if (!users || Object.keys(users).length === 0) {
        if (shippingMethodValue === "0") {
            const newHexCode = generateHexCode(existingHexCodes);

            return {
                createBy: userId,
                hexCode: newHexCode,
                idUser: '',
                customerName: customerName_gui || 'Khách vãng lai',
                customerPhone: customerPhone_gui || 'xxx.xxx.xxx',
                customerEmail: 'xxx.xxx.xxx',
                shippingAddress: "Nhận tại cửa hàng",
                shippingAddressLine2: "",
                shipDate: new Date().toISOString(),
                cotsts: costs,
                voucherCode: "",
                notes: note,
                trackingCheck: true,
                paymentMethods: parseInt(paymentMethod),
                paymentStatus: 1,
                shippingMethods: 0,
                orderStatus: 3,
                orderType: 1,
                status: 1,
                orderDetailsCreateVM: orderDetails
            };
        } 
    }

    const idUser = users.id || '';
    const customerEmail = users.email || '';
    const customerPhone = users.phoneNumber || '';
    const customerName = users.firstAndLastName || '';
    const newHexCode = generateHexCode(existingHexCodes);

    if (shippingMethodValue === "1") {
        shippingAddress = document.getElementById('shippingAddress').value.trim();
        shippingMethods = parseInt(shippingMethodValue);
    } else {
        shippingAddress = "Nhận tại cửa hàng";
        shippingMethods = 0;
    }

    return {
        createBy: userId,
        idUser: idUser,
        hexCode: newHexCode,
        customerName: customerName_gui || customerName,
        customerPhone: customerPhone_gui || customerPhone,
        customerEmail: customerEmail,
        shippingAddress: shippingAddress,
        shippingAddressLine2: "",
        shipDate: new Date().toISOString(),
        cotsts: costs,
        voucherCode: "",
        notes: note,
        trackingCheck: true,
        paymentMethods: parseInt(paymentMethod),
        paymentStatus: 1,
        shippingMethods: shippingMethods,
        orderStatus: 3,
        orderType: 1,
        status: users.status || 1,
        orderDetailsCreateVM: orderDetails
    };
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
function calculateCustomersStillOwe() {
    const totalPayElement = document.getElementById('total_pay');
    const moneyGivenByGuestsInput = document.getElementById('money_given_by_guests');
    const customersStillOweElement = document.getElementById('customers_still_owe');

    const totalPay = parseFloat(totalPayElement.textContent.replace(/\D/g, '')) || 0;
    const moneyGiven = parseFloat(moneyGivenByGuestsInput.value.replace(/\D/g, '')) || 0;
    let customersStillOwe = totalPay - moneyGiven;
    customersStillOweElement.textContent = customersStillOwe.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
}
function calculateTotalPay() {
    const tamtinhElement = document.querySelector('.control-all-money-tamtinh');
    const couponInput = document.getElementById('coupound');
    const shippingFeeElement = document.getElementById('shippingFee');
    const totalPayElement = document.getElementById('total_pay');

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

    let totalPay = tamtinh - coupon + costs;
    totalPay = Math.max(totalPay, 0);

    if (totalPayElement) {
        totalPayElement.textContent = totalPay.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
    }
    calculateCustomersStillOwe();
}
function removeProduct(button) {
    const row = button.closest('tr');
    const optionId = row.dataset.optionId;
    const quantityElement = row.querySelector('.product-quantity');
    const quantity = parseInt(quantityElement.textContent) || 0;

    updateProductQuantityDisplay(optionId, quantity);

    const stockQuantities = getStockQuantitiesFromLocalStorage();
    if (stockQuantities[optionId] !== undefined) {
        stockQuantities[optionId] += quantity;
    } else {
        stockQuantities[optionId] = quantity;
    }
    localStorage.setItem('stockQuantities', JSON.stringify(stockQuantities));

    removeFromOrderDetails(optionId);

    row.remove();

    if (!document.querySelector('#selectedProductsList tbody').children.length) {
        const noProductsMessage = document.createElement('tr');
        noProductsMessage.id = 'noProductsMessage';
        noProductsMessage.innerHTML = '<td colspan="7" class="text-center">Không có sản phẩm nào được chọn</td>';
        document.querySelector('#selectedProductsList tbody').appendChild(noProductsMessage);
    }
    updateSubmitButtonState();

    updateSubmitButtonState();
    calculateSubtotal();
}

function getStockQuantitiesFromLocalStorage() {
    const stockQuantities = localStorage.getItem('stockQuantities');
    return stockQuantities ? JSON.parse(stockQuantities) : {};
}
function updateProductQuantityDisplay(optionId, quantity) {
    const stockQuantityElement = document.getElementById(`stock_quantity_${optionId}`);
    if (stockQuantityElement) {
        let currentStockQuantity = parseInt(stockQuantityElement.textContent.replace(/\D/g, '')) || 0;
        currentStockQuantity += quantity;
        stockQuantityElement.textContent = `(SLT: ${currentStockQuantity})`;
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
function removeFromOrderDetails(optionId) {
    const index = orderDetails.findIndex(item => item.idOptions === optionId);
    if (index !== -1) {
        orderDetails.splice(index, 1);
    }
}
function updateProductQuantityDisplay(optionId, quantity) {
    const stockQuantityElement = document.getElementById(`stock_quantity_${optionId}`);
    if (stockQuantityElement) {
        let currentStockQuantity = parseInt(stockQuantityElement.textContent.replace(/\D/g, '')) || 0;
        currentStockQuantity += quantity;
        stockQuantityElement.textContent = `(SLT: ${currentStockQuantity})`;
    }
}
const tamtinhElement = document.querySelector('.control-all-money-tamtinh');
const totalPayElement = document.querySelector('.control-all-money-total');
const couponInput = document.getElementById('coupound');
const moneyGivenByGuestsInput = document.getElementById('money_given_by_guests');
const customersStillOweElement = document.getElementById('customers_still_owe');
const shippingFeeElement = document.getElementById('shippingFeeElement');
couponInput.addEventListener('input', formatAndCalculateTotalPay);
moneyGivenByGuestsInput.addEventListener('input', formatAndCalculateCustomersStillOwe);
var invoiceNumber = getQueryParameter('invoiceNumber');
document.getElementById('invoiceNumberDisplay').innerText = 'Hóa đơn số: ' + invoiceNumber;
function getQueryParameter(name) {
    var urlParams = new URLSearchParams(window.location.search);
    return urlParams.get(name);
}
function validateNumberInput(input) {
    input.value = input.value.replace(/[^\d.-]/g, '');

    let dotCount = input.value.split('.').length - 1;
    if (dotCount > 1) {
        input.value = input.value.slice(0, input.value.lastIndexOf('.'));
    }

    input.value = formatCurrency(input.value);
}
function formatCurrency(amount) {
    return amount.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + " đ";
}
function formatAndCalculateTotalPay() {
    formatCurrency(couponInput);
    calculateTotalPay();
}
function formatAndCalculateCustomersStillOwe() {
    formatCurrency(moneyGivenByGuestsInput);
    calculateCustomersStillOwe();
}
function searchCustomerByPhoneNumber(phoneNumber) {
    const xhr = new XMLHttpRequest();
    xhr.open('GET', `https://localhost:7241/api/ApplicationUser/GetUsersByPhoneNumber?phoneNumber=${phoneNumber}`, true);

    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
                const users = JSON.parse(xhr.responseText);
                displaySearchResults(users);
                if (users.length > 0) {
                    const data = fillOrderData(users[0]);
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
                    <p style="margin: 0; color: #555;">ID: ${user.id}</p>
                    <p style="margin: 0; color: #555;">Tên: ${user.firstAndLastName}</p>
                    <p style="margin: 0; color: #555;">Username: ${user.username}</p>
                </div>
                <div class="col-md-4" style="flex: 2;">
                    <p style="margin: 0; color: #555;">Email: ${user.email}</p>
                    <p style="margin: 0; color: #555;">Điện thoại: ${user.phoneNumber}</p>
                    <p style="margin: 0; color: #555;">Chức vụ: ${user.roleName}</p>
                    <p style="margin: 0; color: ${statusColor};">Trạng thái: ${statusText}</p>
                </div>
            </div>
        `;
        resultsContainer.appendChild(userItem);
    });
    if (users.length > 0) {
        selectedUser = users[0];
    } else {
        selectedUser = null;
    }
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

    function updateShippingAddress() {
        var city = citySelect.value;
        var district = districtSelect.value;
        var ward = wardSelect.value;
        var street = streetInput.value;

        var shippingAddress = "";

        if (shippingMethodSelect.value !== "0") {
            if (street !== "") {
                shippingAddress += street;
            }
            if (ward !== "") {
                shippingAddress += (shippingAddress !== "" ? ', ' : '') + ward;
            }
            if (district !== "") {
                shippingAddress += (shippingAddress !== "" ? ', ' : '') + district;
            }
            if (city !== "") {
                shippingAddress += (shippingAddress !== "" ? ', ' : '') + city;
            }
        }

        shippingAddressInput.value = shippingAddress;
    }

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

    shippingMethodSelect.addEventListener('change', function () {
        if (this.value === "0") {
            shippingDetails.style.display = 'none';
            shippingAddressInput.value = "Nhận tại cửa hàng";
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
        updateShippingAddress();
    } else {
        shippingDetails.style.display = 'flex';
        updateShippingAddress();
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
        var shippingFee;
        async function getShippingFee(provinceID, districtID, wardCode) {
            const shopID = 4145900;
            var token = 'd01771f0-3f8b-11ef-8f55-4ee3d82283af';
            var xhr = new XMLHttpRequest();
            xhr.open('POST', 'https://online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/fee', true);
            xhr.setRequestHeader('Content-Type', 'application/json');
            xhr.setRequestHeader('Token', token);
            xhr.onreadystatechange = function () {
                if (xhr.readyState === XMLHttpRequest.DONE) {
                    if (xhr.status === 200) {
                        var responseData = JSON.parse(xhr.responseText);
                        console.log('API Response:', responseData);
                        if (responseData.code === 200) {
                            shippingFee = responseData.data.total;
                            document.getElementById('shippingFee').innerText = shippingFee;
                            document.getElementById('shippingFeeDisplay').innerText = shippingFee.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
                            calculateTotalPay();
                            console.log('Giá:' + shippingFee)
                        } else {
                            console.error('Error:', responseData.message);
                        }
                    } else {
                        console.error('Request failed. Status:', xhr.status);
                    }
                }
            };

            var payload = JSON.stringify({
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
            });

            xhr.send(payload);
        }
    }
});
document.addEventListener('DOMContentLoaded', function () {
    fetchProducts();
    document.getElementById('product').addEventListener('change', function () {
        const selectedProductId = this.value;
        if (selectedProductId) {
            const product = products.find(p => p.id === selectedProductId);
            if (product) {
                updateSizesAndColors(product);
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
        })
        .catch(error => console.error('Error fetching product:', error));
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
                        let isFirstRow = true;
                        const data = JSON.parse(xhr.responseText);
                        const productDetailsDiv = document.getElementById('productTableBody');
                        productDetailsDiv.innerHTML = `
                            <td>${data.name}</td>
                            <td class="col-2 text-center">${data.color} - ${data.size} <p id="stock_quantity_${data.idOptions}">(SLT: ${data.quantity})</p></td>
                            <td class="col-2 text-center"><img src="${data.urlImg}" alt="${data.productName}" style="width: 50px; height: auto;"></td>
                            <td class="col-2 text-center" style="width:70px" >${data.price.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' }) }</td>
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
function parseTabsCookie() {
    const tabsCookie = getCookie('tabs');
    if (tabsCookie) {
        return JSON.parse(decodeURIComponent(tabsCookie));
    }
    return [];
}
function getOrderDataFromCookies(tabs) {
    return tabs.map(tab => {
        const cookieName = tab.invoiceNumber.toString();
        const orderData = getCookie(cookieName);
        if (orderData) {
            return JSON.parse(decodeURIComponent(orderData));
        }
        return null;
    }).filter(order => order !== null);
}
document.getElementById('luu_cookies').addEventListener('click', function () {
    const selectedProducts = [];
    document.querySelectorAll('#selectedProductsList tbody .selected-product-item').forEach(function (row) {
        const idoptions = row.querySelector('input[type="text"]').value;
        const quantity = row.cells[3].innerText.trim();
        const product = {
            idoptions: idoptions,
            quantity: quantity,
        };
        selectedProducts.push(product);
    });
    const shippingMethod = document.getElementById('shippingMethod').value;
    const data = {
        selectedProducts,
        customerPhoneNumber: document.getElementById('customerPhoneNumber').value,
        shippingMethod: document.getElementById('shippingMethod').value,
        customerName: document.getElementById('customerName').value,
        customerPhone: document.getElementById('customerPhone').value,
        city: document.getElementById('city').value,
        district: document.getElementById('district').value,
        ward: document.getElementById('ward').value,
        street: document.getElementById('street').value,
        shippingFee: document.getElementById('shippingFee').innerText,
        noteOrder: document.getElementById('note_order').value,
        paymentMethodSelect: document.getElementById('paymentMethodSelect').value,
        coupound: document.getElementById('coupound').value,
        totalPay: document.getElementById('total_pay').innerText,
        moneyGivenByGuests: document.getElementById('money_given_by_guests').value,
        customersStillOwe: document.getElementById('customers_still_owe').innerText
    };
    if (shippingMethod === '1') {
        data.customerName = document.getElementById('customerName').value;
        data.customerPhone = document.getElementById('customerPhone').value;
        data.city = document.getElementById('city').value;
        data.district = document.getElementById('district').value;
        data.ward = document.getElementById('ward').value;
        data.street = document.getElementById('street').value;
        data.shippingFee = document.getElementById('shippingFee').innerText;
        data.shippingAddress = document.getElementById('shippingAddress').value;
    }
    if (selectedProducts.length === 0 || !data.totalPay) {
        Swal.fire({
            icon: 'error',
            title: 'Lỗi',
            text: 'Vui lòng điền đầy đủ thông tin.',
            confirmButtonText: 'OK'
        });
    } else {
        const invoiceNumberDisplay = document.getElementById('invoiceNumberDisplay').innerText;
        const tabName = invoiceNumberDisplay.replace('Hóa đơn số: ', '').replace(/\s/g, '');

        document.cookie = `${tabName}=${encodeURIComponent(JSON.stringify(data))}; path=/; max-age=86400;`;
        window.parent.postMessage(data, '*');

        Swal.fire({
            icon: 'success',
            title: 'Lưu thành công!',
            text: 'Dữ liệu đã được lưu thành công.',
            confirmButtonText: 'OK'
        });
    }
});
function displayOrders(orders) {
    orders.forEach(order => {
        function updateFormData(data) {

            if (data.customerPhoneNumber !== null && data.customerPhoneNumber !== undefined) {
                const phoneNumberInput = document.getElementById('customerPhoneNumber');
                phoneNumberInput.value = data.customerPhoneNumber;
                searchCustomerByPhoneNumber(data.customerPhoneNumber); 
            }
            if (data.selectedProducts && Array.isArray(data.selectedProducts)) {
                const selectedProductsList = document.getElementById('selectedProductsList').querySelector('tbody');
                selectedProductsList.innerHTML = '';

                function fetchProductDetails(idoptions, quantity) {
                    return new Promise((resolve, reject) => {
                        var xhr = new XMLHttpRequest();
                        xhr.open('GET', `https://localhost:7241/api/Options/GetByID/${idoptions}`, true);
                        xhr.onreadystatechange = function () {
                            if (xhr.readyState === 4) {
                                if (xhr.status === 200) {
                                    resolve({ ...JSON.parse(xhr.responseText), quantity });
                                } else {
                                    reject(xhr.statusText);
                                }
                            }
                        };
                        xhr.send();
                    });
                }

                const productPromises = data.selectedProducts.map(product => {
                    return fetchProductDetails(product.idoptions, product.quantity);
                });

                Promise.all(productPromises)
                    .then(products => {
                        products.forEach(product => {
                            const { id, imageURL, keyCode, sizesName, colorName, retailPrice, quantity, stockQuantity } = product;
                            const orderedQuantity = data.selectedProducts.find(p => p.idoptions === id).quantity;
                            const initialStockQuantity = stockQuantity; 
                            const currentStockQuantity = initialStockQuantity - orderedQuantity;
                            const remainingStockQuantity = currentStockQuantity - orderedQuantity;

                            if (remainingStockQuantity < 0) {
                                console.error(`Số lượng tồn kho không đủ cho sản phẩm ${id}.`);
                                return;
                            }

                            const totalPrice = retailPrice * quantity;
                            const productRow = createProductRow({ id, imageURL, keyCode, sizesName, colorName, retailPrice }, quantity, totalPrice, id);
                            selectedProductsList.appendChild(productRow);
                            addToOrderDetails(product.id, product.quantity);
                        });
                        calculateSubtotal();
                        updateSubmitButtonState();
                    })
                    .catch(error => console.error('Error fetching product details:', error));
            }
            
            if (data.city !== null && data.city !== undefined) {
                document.getElementById('city').value = data.city;
                updateCityOptions(data.city, data.district, data.ward);
            }
            if (data.street !== null && data.street !== undefined) {
                document.getElementById('street').value = data.street;
            }
            if (data.customerName !== null && data.customerName !== undefined) {
                document.getElementById('customerName').value = data.customerName;
            }
            if (data.customerPhone !== null && data.customerPhone !== undefined) {
                document.getElementById('customerPhone').value = data.customerPhone;
            }
            if (data.city !== null && data.city !== undefined) {
                document.getElementById('city').value = data.city;
            }
            if (data.district !== null && data.district !== undefined) {
                document.getElementById('district').value = data.district;
            }
            if (data.ward !== null && data.ward !== undefined) {
                document.getElementById('ward').value = data.ward;
            }
            if (data.street !== null && data.street !== undefined) {
                document.getElementById('street').value = data.street;
            }
            if (data.note_order !== null && data.note_order !== undefined) {
                document.getElementById('note_order').value = data.note_order;
            }
            
            var shippingMethodSelect = document.getElementById("shippingMethod");
            var shippingDetails = document.getElementById("shippingDetails");
            if (data.shippingMethod) {
                shippingMethodSelect.value = data.shippingMethod;
                if (data.shippingMethod === "1") {
                    shippingDetails.style.display = 'flex';
                } else {
                    shippingDetails.style.display = 'none';
                }
            }

            shippingMethodSelect.addEventListener('change', function () {
                if (this.value === "1") {
                    shippingDetails.style.display = 'flex';
                } else {
                    shippingDetails.style.display = 'none';
                }
            });


            if (data.shippingMethod !== null && data.shippingMethod !== undefined) {
                document.getElementById('shippingMethod').value = data.shippingMethod;
            }
            if (data.paymentMethod !== null && data.paymentMethod !== undefined) {
                document.getElementById('paymentMethodSelect').value = data.paymentMethod;
            }
            if (data.discount !== null && data.discount !== undefined) {
                document.getElementById('coupound').value = data.discount;
            }
            if (data.moneyGivenByGuests !== null && data.moneyGivenByGuests !== undefined) {
                document.getElementById('money_given_by_guests').value = data.moneyGivenByGuests;
            }
            if (data.customersStillOwe !== null && data.customersStillOwe !== undefined) {
                document.getElementById('customers_still_owe').textContent = data.customersStillOwe;
            }
            if (data.shippingFee) {
                document.getElementById('shippingFee').innerText = data.shippingFee;
                const formattedFee = parseFloat(data.shippingFee).toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
                document.getElementById('shippingFeeDisplay').innerText = formattedFee;      
            }
        }
        updateFormData(order);

    });
}
function getOrderDataFromCookies(invoiceNumber) {
    var cookieName = invoiceNumber;
    var cookieValue = getCookie(cookieName);
    return cookieValue ? JSON.parse(decodeURIComponent(cookieValue)) : null;
}
document.addEventListener('DOMContentLoaded', function () {
    const invoiceNumber = getQueryParameter('invoiceNumber');
    const orderData = getOrderDataFromCookies(invoiceNumber);

    if (orderData) {
        displayOrders([orderData]);  
    } else {
        console.error('Không tìm thấy dữ liệu cho hóa đơn số: ', invoiceNumber);
    }
});
function updateCityOptions(selectedCityName, selectedDistrictName, selectedWardName) {
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
            var data = xhr.response;

            var citiesSelect = document.getElementById("city");
            citiesSelect.innerHTML = '<option value="" selected>Chọn tỉnh/thành phố</option>';
            data.forEach(function (cityObj) {
                var option = document.createElement("option");
                option.value = cityObj.Name;
                option.textContent = cityObj.Name;
                citiesSelect.appendChild(option);
            });
            citiesSelect.value = selectedCityName;

            if (selectedCityName) {
                updateDistrictOptions(selectedCityName, selectedDistrictName, selectedWardName);
            }

            updateShippingAddress();
        } else {
            console.error('Error fetching data: ', xhr.statusText);
        }
    };
    xhr.onerror = function () {
        console.error('Request failed');
    };
    xhr.send();
}
function updateDistrictOptions(city, district, ward) {
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
            var data = xhr.response;
            var selectedCity = data.find(cityObj => cityObj.Name === city);
            if (selectedCity) {
                var districtsSelect = document.getElementById("district");
                districtsSelect.innerHTML = '<option value="" selected>Chọn quận huyện</option>';
                selectedCity.Districts.forEach(function (districtObj) {
                    var option = document.createElement("option");
                    option.value = districtObj.Name;
                    option.textContent = districtObj.Name;
                    districtsSelect.appendChild(option);
                });
                districtsSelect.value = district;

                updateWardOptions(selectedCity.Districts, district, ward);
                updateShippingAddress();

            } else {
                console.error('Không tìm thấy thành phố');
            }
        } else {
            console.error('Error fetching data: ', xhr.statusText);
        }
    };
    xhr.onerror = function () {
        console.error('Request failed');
    };
    xhr.send();
}
function updateWardOptions(districts, district, ward) {
    var selectedDistrict = districts.find(districtObj => districtObj.Name === district);
    if (selectedDistrict) {
        var wardsSelect = document.getElementById("ward");
        wardsSelect.innerHTML = '<option value="" selected>Chọn phường xã</option>';
        selectedDistrict.Wards.forEach(function (wardObj) {
            var option = document.createElement("option");
            option.value = wardObj.Name;
            option.textContent = wardObj.Name;
            wardsSelect.appendChild(option);
        });
        wardsSelect.value = ward;
        updateShippingAddress();

    } else {
        console.error('Không tìm thấy quận/huyện');
    }
}
function updateShippingAddress() {
    var city = document.getElementById("city").value;
    var district = document.getElementById("district").value;
    var ward = document.getElementById("ward").value;
    var street = document.getElementById("street").value;

    var shippingAddress = "";

    if (street !== "") {
        shippingAddress += street;
    }
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
function updateStockQuantitiesInStorage(data) {
    let stockQuantities = getStockQuantitiesFromStorage();

    data.forEach(product => {
        if (!stockQuantities[product.id]) {
            stockQuantities[product.id] = product.stockQuantity;
        }
    });

    localStorage.setItem('stockQuantities', JSON.stringify(stockQuantities));
}

function updateStockQuantity(optionId, newStockQuantity) {
    const stockQuantityElement = document.getElementById(`stock_quantity_${optionId}`);
    if (stockQuantityElement) {
        stockQuantityElement.textContent = `(SLT: ${newStockQuantity})`;
    }
}

function getStockQuantitiesFromStorage() {
    const storageValue = localStorage.getItem('stockQuantities');
    return storageValue ? JSON.parse(storageValue) : {};
}

function checkAndSyncStockQuantities() {
    const latestStockQuantities = getStockQuantitiesFromStorage();

    Object.keys(latestStockQuantities).forEach(optionId => {
        const stockElement = document.getElementById(`stock_quantity_${optionId}`);
        if (stockElement) {
            stockElement.textContent = `(SLT: ${latestStockQuantities[optionId]})`;
        }
    });
}

window.addEventListener('storage', function (event) {
    if (event.key === 'stockQuantities') {
        checkAndSyncStockQuantities();
    }
});
