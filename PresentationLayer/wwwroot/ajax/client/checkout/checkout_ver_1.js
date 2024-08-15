var orderDetails = [];
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
console.log(userId)
document.addEventListener('DOMContentLoaded', function () {
    const url = window.location.href;
    const queryString = url.split('?')[1];
    const params = new URLSearchParams(queryString);
    const data = params.get('data');

    if (data) {
        const decodedData = decodeURIComponent(data);
        cartItems = JSON.parse(decodedData);

        cartItems.forEach(item => {
            addToOrderDetails(item.idOptions, item.quantity);
        });

        updateTableWithProductDetails();
    }
});
function updateURL() {
    const encodedData = encodeURIComponent(JSON.stringify(cartItems));
    const newUrl = `${window.location.pathname}?data=${encodedData}`;
    history.replaceState(null, '', newUrl);
}
function fetchProductDetails(idOptions, callback) {
    const xhr = new XMLHttpRequest();
    xhr.open('GET', `https://localhost:7241/api/Options/GetByID/${idOptions}`, true);
    xhr.onload = function () {
        if (xhr.status >= 200 && xhr.status < 300) {
            const productData = JSON.parse(xhr.responseText);
            callback(null, productData);
            console.log(productData)
        } else {
            callback(new Error('Failed to fetch product details'));
        }
    };
    xhr.onerror = function () {
        callback(new Error('Request failed'));
    };
    xhr.send();
}
function updateTableWithProductDetails() {
    const tableBody = document.getElementById('table-shopping-cart');

    const fetchPromises = cartItems.map(item =>
        new Promise((resolve, reject) => {
            fetchProductDetails(item.idOptions, (error, productDetails) => {
                if (error) {
                    reject(error);
                } else {
                    const quantity = item.quantity;
                    const price = productDetails.retailPrice;

                    resolve({
                        item,
                        quantity,
                        price
                    });
                }
            });
        })
    );

    Promise.all(fetchPromises)
        .then(results => {
            let totalPrice = 0;
            const productsList = document.getElementById('table-shopping-cart');
            productsList.innerHTML = '';
            results.forEach(({ item, quantity, price }) => {
                const productItem = document.createElement('li');
                productItem.classList.add('table-shopping-cart');

                const itemTotalPrice = quantity * price;
                productItem.innerHTML = `<img src="${item.imageURL}" style="width: 50px;"></img>
                    ${item.productName} (${item.sizeName} - ${item.colorName})
                    <br> (Số lượng: ${quantity} x ${price.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })})
                    <span>Tổng giá: ${itemTotalPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</span><hr>
                    `;

                productsList.appendChild(productItem);
                totalPrice += itemTotalPrice;
            });

            const totalEstimatedPriceElement = document.getElementById('provisional_fee');
            const totalOrderPriceElement = document.getElementById('total_order');
            if (totalEstimatedPriceElement) {
                totalEstimatedPriceElement.textContent = totalPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
                totalOrderPriceElement.textContent = totalPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
            }
        })
        .catch(error => {
            console.error('Error fetching product details:', error);
        });
}
document.addEventListener('DOMContentLoaded', function () {
    var citySelect = document.getElementById("city");
    var districtSelect = document.getElementById("district");
    var wardSelect = document.getElementById("ward");
    var streetInput = document.getElementById("street");
    var shippingAddressInput = document.getElementById("shippingAddress");

    function updateShippingAddress() {
        var city = citySelect.value;
        var district = districtSelect.value;
        var ward = wardSelect.value;
        var street = streetInput.value;

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

        shippingAddressInput.value = shippingAddress;
    }


    citySelect.addEventListener('change', updateShippingAddress);
    districtSelect.addEventListener('change', updateShippingAddress);
    wardSelect.addEventListener('change', updateShippingAddress);
    streetInput.addEventListener('input', updateShippingAddress);
});

document.addEventListener("DOMContentLoaded", function () {
    function updateNiceSelect(id) {
        var select = document.getElementById(id);
        var niceSelect = select.nextElementSibling;
        var options = select.querySelectorAll('option');
        var list = niceSelect.querySelector('.list');
        var current = niceSelect.querySelector('.current');

        list.innerHTML = '';
        options.forEach(function (option) {
            var li = document.createElement('li');
            li.textContent = option.textContent;
            li.setAttribute('data-value', option.value);
            li.className = 'option' + (option.selected ? ' selected focus' : '');
            li.addEventListener('click', function () {
                select.value = option.value;
                current.textContent = option.textContent;
                select.dispatchEvent(new Event('change'));
            });
            list.appendChild(li);
        });

        var selectedOption = select.querySelector('option:checked');
        current.textContent = selectedOption ? selectedOption.textContent : 'Chọn';
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
        var citiesSelect = document.getElementById("city");
        var districtsSelect = document.getElementById("district");
        var wardsSelect = document.getElementById("ward");

        citiesSelect.innerHTML = '<option value="" selected>Chọn tỉnh thành</option>';
        data.forEach(function (city) {
            var option = document.createElement("option");
            option.value = city.Name;
            option.textContent = city.Name;
            citiesSelect.appendChild(option);

        });
        updateNiceSelect('city');
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
            updateNiceSelect('district');
            updateNiceSelect('ward');
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
            updateNiceSelect('ward');
            calculateShippingFee();
        });

        wardsSelect.addEventListener('change', function () {
            calculateShippingFee();
        });
    }
    async function calculateShippingFee() {
        var city = document.getElementById('city').value;
        var district = document.getElementById('district').value;
        var ward = document.getElementById('ward').value;

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

                console.log('Mã Tỉnh: ' + provinceID + ', Mã quận: ' + districtID + ', Mã xã: ' + wardCode);
                await getShippingFee(provinceID, districtID, wardCode);

            } catch (error) {
                console.error('Error:', error);
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
                async function updateTotalOrder() {
                    try {
                        var totalEstimatedPriceText = document.getElementById('provisional_fee').innerText;
                        var totalEstimatedPrice = parseFloat(totalEstimatedPriceText.replace(/\D/g, ''));

                        var shippingFeeText = document.getElementById('fee_ship').innerText;
                        var shippingFee = parseFloat(shippingFeeText.replace(/\D/g, ''));

                        var totalOrder = totalEstimatedPrice + shippingFee;

                        document.getElementById('total_order').innerText = totalOrder.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });

                        console.log('Phí ship:', shippingFee);
                        console.log('Giá tạm:', totalEstimatedPrice);
                        console.log('Tổng giá:', totalOrder);
                    } catch (error) {
                        console.error('Error:', error);
                    }
                }
                const data = await response.json();

                console.log('API Response:', data);
                if (data.code === 200) {
                    let shippingFee = data.data.total;
                    document.getElementById('fee_ship').innerText = shippingFee;
                    document.getElementById('fee_shipDisplay').innerText = shippingFee.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
                    updateTotalOrder();
                    console.log('Giá:' + shippingFee);
                } else {
                    console.error('Error:', data.message);
                }
            } catch (error) {
                console.error('Error:', error);
            }
        }

    }
});
document.addEventListener('DOMContentLoaded', function () {
    var modal = document.getElementById('addressModal');
    var openModalBtn = document.getElementById('openModalBtn');
    var closeModal = document.getElementsByClassName('close')[0];
    var selectAddressBtn = document.getElementById('selectAddressBtn');
    var userId = getUserIdFromJwt(getJwtFromCookie()); 

    openModalBtn.onclick = function () {
        modal.style.display = 'block';
        loadUserAddresses();
    }

    closeModal.onclick = function () {
        modal.style.display = 'none';
    }

    window.onclick = function (event) {
        if (event.target == modal) {
            modal.style.display = 'none';
        }
    }

    function loadUserAddresses() {
        const xhr = new XMLHttpRequest();
        xhr.open('GET', `https://localhost:7241/api/Address/user/${userId}`, true);
        xhr.setRequestHeader('Accept', '*/*');

        xhr.onload = function () {
            if (xhr.status >= 200 && xhr.status < 300) {
                try {
                    const response = JSON.parse(xhr.responseText);
                    const addressList = document.getElementById('addressList');
                    addressList.innerHTML = '';

                    response.forEach((address, index) => {
                        const li = document.createElement('li');
                        li.innerHTML = `${index + 1}. 
                            <strong>Họ và tên:</strong> ${address.firstAndLastName} - <strong>Số điện thoại:</strong> ${address.phoneNumber} <small style="color:red;">${address.isDefault ? '(Mặc định)' : ''}</small><br> 
                            <strong>Email:</strong> ${address.gmail}<br>
                            <strong>Thành phố:</strong> ${address.city} - <strong>Quận/Huyện:</strong> ${address.districtCounty} - <strong>Xã/Phường:</strong> ${address.commune}<br>
                            <strong>Địa chỉ cụ thể:</strong> ${address.specificAddress}<br>
                        `;
                        li.dataset.id = address.id;
                        li.dataset.firstAndLastName = address.firstAndLastName;
                        li.dataset.phoneNumber = address.phoneNumber;
                        li.dataset.gmail = address.gmail;
                        li.dataset.city = address.city;
                        li.dataset.districtCounty = address.districtCounty;
                        li.dataset.commune = address.commune;
                        li.dataset.specificAddress = address.specificAddress;
                        addressList.appendChild(li);
                    });

                    addressList.addEventListener('click', function (event) {
                        if (event.target.tagName === 'LI') {
                            const selectedAddress = event.target;
                            selectAddressBtn.dataset.selectedAddressId = selectedAddress.dataset.id;
                            selectAddressBtn.textContent = `Chọn Địa Chỉ: ${selectedAddress.dataset.firstAndLastName}`;
                        }
                    });
                } catch (error) {
                    console.error('Parsing error:', error);
                }
            } else {
                console.error('Request failed with status:', xhr.status);
            }
        };

        xhr.onerror = function () {
            console.error('Request error');
        };

        xhr.send();
    }

    selectAddressBtn.onclick = function () {
        const selectedAddressId = this.dataset.selectedAddressId;
        if (selectedAddressId) {
            const selectedAddress = document.querySelector(`li[data-id="${selectedAddressId}"]`);
            if (selectedAddress) {
                document.getElementById('name').value = selectedAddress.dataset.firstAndLastName;
                document.getElementById('phone').value = selectedAddress.dataset.phoneNumber;
                document.getElementById('gmail').value = selectedAddress.dataset.gmail;
                document.getElementById('city').value = selectedAddress.dataset.city;
                document.getElementById('district').value = selectedAddress.dataset.districtCounty;
                document.getElementById('ward').value = selectedAddress.dataset.commune;
                document.getElementById('street').value = selectedAddress.dataset.specificAddress;
                document.getElementById('shippingAddress').value = selectedAddress.dataset.specificAddress;

                modal.style.display = 'none';
            }
        } else {
            alert('Vui lòng chọn một địa chỉ.');
        }
    }
});

function getFormData() {
    const customerName = document.getElementById('name').value;
    const customerPhone = document.getElementById('phone').value;
    const customerEmail = document.getElementById('gmail').value;
    const street = document.getElementById('street').value;
    const city = document.getElementById('city').value;
    const district = document.getElementById('district').value;
    const ward = document.getElementById('ward').value;
    const existingHexCodes = [];
    const shippingAddress = `${street}, ${ward}, ${district}, ${city}`;
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
    const newHexCode = generateHexCode(existingHexCodes);


    const shippingFeeElement = document.getElementById('fee_ship');
    let shippingFee = 0;

    if (shippingFeeElement) {
        const textContent = shippingFeeElement.textContent.trim();
        const match = textContent.match(/\d+/);

        if (match) {
            shippingFee = parseInt(match[0]);
        }
    }

    const totalOrderElement = document.getElementById('total_order');
    const totalOrderText = totalOrderElement.textContent.replace('₫', '').trim();
    const totalOrder = parseFloat(totalOrderText.replace(/\./g, '').replace(/,/g, '.')) || 0;
    const paymentMethod = parseInt(document.querySelector('input[name="payment"]:checked').value);

    function validateNotNull(value, fieldName) {
        if (!value || value.trim() === "") {
            Swal.fire({
                icon: 'error',
                title: 'Lỗi',
                text: fieldName + " không được để trống."
            });
            return false;
        }
        return true;
    }

    if (validateNotNull(customerName, "Tên khách hàng") &&
        validateNotNull(customerPhone, "Số điện thoại") &&
        validateNotNull(customerEmail, "Email") &&
        validateNotNull(street, "Địa chỉ") &&
        validateNotNull(city, "Thành phố") &&
        validateNotNull(district, "Quận/Huyện") &&
        validateNotNull(ward, "Phường/Xã")) {
        return {
            createBy: userId || "Khách vãng lai",
            idUser: userId || "Khách vãng lai",
            hexCode: newHexCode,
            customerName: customerName,
            customerPhone: customerPhone,
            customerEmail: customerEmail,
            voucherCode: "",
            trackingCheck: false,
            shipDate: new Date().toISOString(),
            shippingAddress: shippingAddress,
            shippingAddressLine2: street,
            cotsts: shippingFee,
            shippingMethods: 1,
            orderStatus: 0,
            totalAmount: totalOrder,
            paymentMethods: paymentMethod,
            orderType: 0,
            status: 1,
            orderDetailsCreateVM: orderDetails
        };
    } else {
        console.error("Các trường bắt buộc không được để trống.");
        return null;
    }
}
function addToOrderDetails(optionId, quantity) {
    let existingDetail = orderDetails.find(detail => detail.idOptions === optionId);
    if (existingDetail) {
        existingDetail.quantity += quantity;
    } else {
        const newDetail = {
            createBy: userId || "Khách vãng lai",
            idOptions: optionId,
            quantity: quantity,
            discount: 0,
            status: 1
        };
        orderDetails.push(newDetail);
    }
}
function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

function sendOrderData() {
    const orderData = getFormData();
    if (!orderData) {
        return;
    }
    console.log('orderData:', orderData);

    Swal.fire({
        title: 'Xác nhận gửi đơn hàng',
        text: 'Bạn có chắc chắn muốn gửi đơn hàng này không?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Gửi',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            Swal.fire({
                title: 'Đang xử lý',
                text: 'Vui lòng chờ trong khi gửi đơn hàng...',
                allowOutsideClick: false,
                didOpen: () => {
                    Swal.showLoading();
                }
            });
            if (orderData.paymentMethods === 0) {
                const xhr = new XMLHttpRequest();
                xhr.open('POST', 'https://localhost:7241/api/VnPay/create-payment-url', true);
                xhr.setRequestHeader('Content-Type', 'application/json');

                xhr.onreadystatechange = function () {
                    if (xhr.readyState === XMLHttpRequest.DONE) {
                        if (xhr.status === 200) {
                            const response = JSON.parse(xhr.responseText);
                            console.log('Success:', response);

                            window.location.href = response.url;
                            setCookie('orderData', JSON.stringify(orderData), 1);
                        } else {
                            console.error('Error:', xhr.responseText);
                            Swal.fire({
                                title: 'Lỗi!',
                                text: 'Đã xảy ra lỗi khi chuyển tới trang thanh toán. Vui lòng thử lại.',
                                icon: 'error',
                                confirmButtonText: 'OK'
                            });
                        }
                    }
                };

                xhr.send(JSON.stringify(orderData));
            } else {
                const xhr = new XMLHttpRequest();
                xhr.open('POST', 'https://localhost:7241/api/Order/create?printInvoice=false', true);
                xhr.setRequestHeader('Content-Type', 'application/json');

                xhr.onreadystatechange = function () {
                    if (xhr.readyState === XMLHttpRequest.DONE) {
                        if (xhr.status === 200) {
                            const response = JSON.parse(xhr.responseText);
                            console.log('Success:', response);

                            const jwt = getJwtFromCookie();
                            if (jwt) {
                                const userId = getUserIdFromJwt(jwt);
                                if (userId) {
                                    const cartApiUrl = `https://localhost:7241/api/Cart/cart/user/${userId}`;
                                    const cartXhr = new XMLHttpRequest();
                                    cartXhr.open('GET', cartApiUrl, true);
                                    cartXhr.onreadystatechange = function () {
                                        if (cartXhr.readyState === XMLHttpRequest.DONE) {
                                            if (cartXhr.status === 200) {
                                                const cartData = JSON.parse(cartXhr.responseText);
                                                const cartId = cartData[0]?.id; 
                                                if (cartId) {
                                                    orderData.orderDetailsCreateVM.forEach(detail => {
                                                        deleteCartOption(cartId, detail.idOptions);
                                                    });
                                                } else {
                                                    console.error('Không thể tìm thấy giỏ hàng.');
                                                }
                                            } else {
                                                console.error('Error fetching cart data:', cartXhr.responseText);
                                            }
                                        }
                                    };
                                    cartXhr.send();
                                } else {
                                    console.error('Không thể lấy User ID từ JWT.');
                                }
                            }

                            Swal.fire({
                                title: 'Thành công!',
                                text: 'Đơn hàng của bạn đã được gửi thành công.',
                                icon: 'success',
                                confirmButtonText: 'OK'
                            }).then(() => {
                                window.location.href = '/';
                            });
                        } else {
                            console.error('Error:', xhr.responseText);

                            Swal.fire({
                                title: 'Lỗi!',
                                text: 'Đã xảy ra lỗi khi gửi đơn hàng. Vui lòng thử lại.',
                                icon: 'error',
                                confirmButtonText: 'OK'
                            });
                        }
                    }
                };

                xhr.send(JSON.stringify(orderData));
            }
        } else {
            console.log('Order submission cancelled.');
        }
    });
}

function deleteCartOption(cartId, idOptions) {
    const xhr = new XMLHttpRequest();
    const url = `https://localhost:7241/api/CartOptions/Delete/${cartId}/${idOptions}`;
    xhr.open('DELETE', url, true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                console.log(`Deleted cart option ${idOptions} from cart ${cartId}`);
            } else {
                console.error('Error deleting cart option:', xhr.responseText);
            }
        }
    };
    xhr.send();
}
