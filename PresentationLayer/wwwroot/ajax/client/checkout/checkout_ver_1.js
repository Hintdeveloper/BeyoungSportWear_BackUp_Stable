var orderDetails = [];
let voucherCode = null;
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
const openModalBtn = document.getElementById('openModalBtn');
if (jwt) {
    openModalBtn.style.display = 'block';
} else {
    openModalBtn.style.display = 'none';
}
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
function loadCartItemsFromURL() {
    const urlParams = new URLSearchParams(window.location.search);
    const encodedData = urlParams.get('data');

    if (encodedData) {
        try {
            const decodedData = JSON.parse(decodeURIComponent(encodedData));

            const fetchPromises = decodedData.map(item => {
                return new Promise((resolve, reject) => {
                    fetchProductDetails(item.idOptions, (error, productData) => {
                        if (error) {
                            reject('Không thể kiểm tra số lượng tồn kho.');
                        } else {
                            if (item.quantity > productData.stockQuantity) {
                                item.quantity = 1;
                                toastr.warning(`Số lượng sản phẩm '${item.productName}' đã bị điều chỉnh về ${1} do vượt quá tồn kho.`, 'Cảnh báo');
                            }
                            resolve(item);
                        }
                    });
                });
            });

            Promise.all(fetchPromises)
                .then(validatedItems => {
                    cartItems = [];

                    validatedItems.forEach(validItem => {
                        cartItems.push(validItem);
                    });

                    updateTableWithProductDetails();
                    updateURL();
                })
                .catch(error => {
                    toastr.error(error, 'Lỗi');
                });

        } catch (e) {
            toastr.error('Dữ liệu giỏ hàng không hợp lệ.', 'Lỗi');
        }
    }
}
window.onload = function () {
    loadCartItemsFromURL();
};
function fetchProductDetails(idOptions, callback) {
    const xhr = new XMLHttpRequest();
    xhr.open('GET', `https://localhost:7241/api/Options/GetByID/${idOptions}`, true);
    xhr.onload = function () {
        if (xhr.status >= 200 && xhr.status < 300) {
            const productData = JSON.parse(xhr.responseText);
            callback(null, productData);
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
    const voucherValue = parseFloat(document.getElementById('voucher')) || 0;
    const fetchPromises = cartItems.map(item =>
        new Promise((resolve, reject) => {
            fetchProductDetails(item.idOptions, (error, productDetails) => {
                if (error) {
                    reject(error);
                } else {
                    const quantity = item.quantity;
                    const price = productDetails.retailPrice;
                    const keyCode = productDetails.keyCode;
                    const id = productDetails.id;

                    resolve({
                        item,
                        quantity,
                        price,
                        keyCode,
                        id
                    });
                }
            });
        })
    );

    Promise.all(fetchPromises)
        .then(results => {
            const productsList = document.getElementById('table-shopping-cart');
            productsList.innerHTML = '';

            results.forEach(({ item, quantity, price, keyCode, id }) => {
                const productItem = document.createElement('li');
                productItem.classList.add('table-shopping-cart');

                const itemTotalPrice = quantity * price;
                productItem.innerHTML =
                    `<div style="display: flex; align-items: center; padding: 10px; border: 1px solid #ccc; margin-bottom: 10px;">
                      <img src="${item.imageURL}" style="width: 50px; height: 50px; border-radius: 5px; margin-right: 10px;">
                      <div>
                          <strong>${item.productName} (${item.sizeName} - ${item.colorName})</strong><br
                          <br>
                            <div style="display: flex; align-items: center; margin-top: 5px;">
                                <button onclick="changeQuantity('${id}', -1)" type="button" style="padding: 5px 10px; font-size: 14px; background-color: #f0f0f0; border: 1px solid #ddd; border-radius: 3px; cursor: pointer;">-</button>
                                <input type="text" id="quantity-${id}" value="${quantity}" min="1" style="width: 150px; 
                                    text-align: center; margin: 0 10px;
                                    padding: 5px; border: 1px solid #ddd;
                                    border-radius: 3px;" oninput="updateQuantity('${id}')">
                                <button onclick="changeQuantity('${id}', 1)" type="button" style="padding: 5px 10px; font-size: 14px; background-color: #f0f0f0; border: 1px solid #ddd; border-radius: 3px; cursor: pointer;">+</button>
                                <span style="color: #333;">(Giá bán: ${price.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })})</span>
                            </div>
                            <br>
                          <button onclick="removeProduct('${id}')" type="button" style="padding: 5px 10px; font-size: 14px; background-color: #f8d7da; border: 1px solid #f5c6cb; border-radius: 3px; cursor: pointer; margin-left: 10px;">Xóa</button>
                          <span style="font-weight: bold;">Tổng giá: ${itemTotalPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</span>
                      </div>
                   </div>`;
                productsList.appendChild(productItem);
            });

            const { totalPrice, discountAmount, totalAfterDiscount } = calculateTotals(results, voucherValue);

            const totalEstimatedPriceElement = document.getElementById('provisional_fee');
            const discountElement = document.getElementById('coupound');
            const totalOrderPriceElement = document.getElementById('total_order');

            if (totalEstimatedPriceElement) {
                totalEstimatedPriceElement.textContent = totalPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
                discountElement.textContent = discountAmount > 0 ? `-${discountAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}` : '0đ';
                totalOrderPriceElement.textContent = totalAfterDiscount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
            }
        })
        .catch(error => {
            console.error('Error fetching product details:', error);
        });
}
function removeProduct(id) {
    cartItems = cartItems.filter(item => item.idOptions !== id);
    orderDetails = orderDetails.filter(detail => detail.idOptions !== id);
    updateTableWithProductDetails();

    updateURL();

    if (cartItems.length === 0) {
        Swal.fire({
            title: 'Thành công!',
            text: 'Giỏ hàng của bạn đã trống. Bạn sẽ được chuyển hướng về trang chính.',
            icon: 'success',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true,
            didOpen: () => {
                Swal.showLoading();
            }
        }).then(() => {
            window.location.href = '/';
        });
    } else {
        toastr.success('Sản phẩm đã được xóa!', 'Thành công');
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

function changeQuantity(id, delta) {
    const quantityInput = document.getElementById(`quantity-${id}`);
    let quantity = parseInt(quantityInput.value, 10);

    quantity += delta;

    if (quantity < 1) {
        quantity = 1;
        toastr.error('Số lượng sản phẩm không thể nhỏ hơn 1.', 'Lỗi');
    } else {
        fetchProductDetails(id, (error, productData) => {
            if (error) {
                toastr.error('Không thể kiểm tra số lượng tồn kho.', 'Lỗi');
            } else {
                if (quantity > productData.stockQuantity) {
                    quantity = productData.stockQuantity;
                    toastr.warning(`Số lượng sản phẩm không thể vượt quá tồn kho (${productData.stockQuantity}).`, 'Cảnh báo');
                } else {
                    toastr.success('Số lượng sản phẩm đã được cập nhật thành công!', 'Thành công');
                }
                quantityInput.value = quantity;

                const item = cartItems.find(item => item.idOptions === id);
                if (item) {
                    item.quantity = quantity;
                } else {
                    cartItems.push({ idOptions: id, quantity: quantity });
                }
                const orderDetail = orderDetails.find(detail => detail.idOptions === id);
                if (orderDetail) {
                    orderDetail.quantity = quantity;
                } else {
                    orderDetails.push({ idOptions: id, quantity: quantity });
                }
                updateURL();
                updateTableWithProductDetails();
            }
        });
    }
}
function updateQuantity(id) {
    const quantityInput = document.getElementById(`quantity-${id}`);
    let quantity = parseInt(quantityInput.value, 10);

    if (isNaN(quantity) || quantity < 1) {
        quantity = 1;
        toastr.error('Số lượng sản phẩm không hợp lệ. Đã được điều chỉnh về 1.', 'Lỗi');
        quantityInput.value = 1;
    } else {
        fetchProductDetails(id, (error, productData) => {
            if (error) {
                toastr.error('Không thể kiểm tra số lượng tồn kho.', 'Lỗi');
            } else {
                if (quantity > productData.stockQuantity) {
                    quantity = productData.stockQuantity;
                    toastr.warning(`Số lượng sản phẩm không thể vượt quá tồn kho (${productData.stockQuantity}).`, 'Cảnh báo');
                    quantityInput.value = productData.stockQuantity;
                } else {
                    toastr.success('Số lượng sản phẩm đã được cập nhật thành công!', 'Thành công');
                }

                const item = cartItems.find(item => item.idOptions === id);
                if (item) {
                    item.quantity = quantity;
                } else {
                    cartItems.push({ idOptions: id, quantity: quantity });
                }
                const orderDetail = orderDetails.find(detail => detail.idOptions === id);
                if (orderDetail) {
                    orderDetail.quantity = quantity;
                } else {
                    orderDetails.push({ idOptions: id, quantity: quantity });
                }

                updateURL();
                updateTableWithProductDetails();
            }
        });
    }
}
document.addEventListener('DOMContentLoaded', function () {
    var citySelect = document.getElementById("city");
    var districtSelect = document.getElementById("district");
    var wardSelect = document.getElementById("ward");
    var shippingAddressInput = document.getElementById("shippingAddress");

    function updateShippingAddress() {
        var city = citySelect.value;
        var district = districtSelect.value;
        var ward = wardSelect.value;

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

        shippingAddressInput.value = shippingAddress;
    }


    citySelect.addEventListener('change', updateShippingAddress);
    districtSelect.addEventListener('change', updateShippingAddress);
    wardSelect.addEventListener('change', updateShippingAddress);
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

                const data = await response.json();

                console.log('API Response:', data);
                if (data.code === 200) {
                    let shippingFee = data.data.total;
                    document.getElementById('fee_ship').innerText = shippingFee;
                    document.getElementById('fee_shipDisplay').innerText = shippingFee.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
                    updateTotalOrder();
                    toastr.warning(
                        `Giá giao hàng theo địa chỉ <strong style="font-size: 18px; color: red;">${shippingFee.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</strong>`,
                        'Phí giao hàng',
                        {
                            timeOut: 5000, // Thời gian hiển thị 5 giây
                            escapeHtml: false, // Cho phép sử dụng HTML trong nội dung toastr
                            positionClass: "toast-top-right", // Vị trí hiện thông báo
                            closeButton: true, // Hiển thị nút đóng
                            progressBar: true // Hiển thị thanh tiến trình
                        }
                    );

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
function updateTotalOrder() {
    try {
        var totalEstimatedPriceText = document.getElementById('provisional_fee').innerText;
        var totalEstimatedPrice = parseFloat(totalEstimatedPriceText.replace(/\D/g, ''));

        var shippingFeeText = document.getElementById('fee_ship').innerText;
        var shippingFee = parseFloat(shippingFeeText.replace(/\D/g, ''));

        var discountText = document.getElementById('coupound').innerText;
        var discount = parseFloat(discountText.replace(/\D/g, ''));

        var totalOrder = totalEstimatedPrice + shippingFee - discount;

        document.getElementById('total_order').innerText = totalOrder.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
    } catch (error) {
        console.error('Error:', error);
    }
}
document.addEventListener('DOMContentLoaded', function () {
    var modal = document.getElementById('addressModal');
    var openModalBtn = document.getElementById('openModalBtn');
    var closeModal = document.getElementsByClassName('close')[0];
    var selectAddressBtn = document.getElementById('selectAddressBtn');
    var userId = getUserIdFromJwt(getJwtFromCookie());
    loadUserAddresses();
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
                            <strong>Địa chỉ:</strong> ${address.commune}, ${address.districtCounty}, ${address.city}<br>
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
                        if (address.isDefault) {
                            defaultAddress = address;
                        }
                    });
                    if (defaultAddress) {
                        fillAddressForm(defaultAddress);
                    }
                    addressList.addEventListener('click', function (event) {
                        if (event.target.tagName === 'LI') {
                            const selectedAddress = event.target;
                            selectAddressBtn.dataset.selectedAddressId = selectedAddress.dataset.id;
                            selectAddressBtn.textContent = `Chọn Địa Chỉ: ${selectedAddress.dataset.specificAddress}`;
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
    async function fillAddressForm(address) {
        document.getElementById('name').value = address.firstAndLastName;
        document.getElementById('phone').value = address.phoneNumber;
        document.getElementById('gmail').value = address.gmail;
        document.getElementById('street').value = address.specificAddress;
        const fullAddress = `${address.commune}, ${address.districtCounty}, ${address.city}`;

        document.getElementById('shippingAddress').value = fullAddress;
        const provinceID = await fetchProvinces(address.city);
        if (provinceID) {
            const districtID = await fetchDistricts(provinceID, address.districtCounty);
            if (districtID) {
                const wardCode = await fetchWards(districtID, address.commune);
                if (wardCode) {
                    await getShippingFee(provinceID, districtID, wardCode);
                }
            }
        }
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
                const fullAddress = `${selectedAddress.dataset.commune}, ${selectedAddress.dataset.districtCounty}, ${selectedAddress.dataset.city}`;
                document.getElementById('shippingAddress').value = fullAddress;

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
    const shippingAddress = document.getElementById('shippingAddress').value;
    const existingHexCodes = [];
    if ((shippingAddress.match(/,/g) || []).length < 2) {
        Swal.fire({
            icon: 'error',
            title: 'Lỗi',
            text: 'Địa chỉ không hợp lệ. Vui lòng điền đầy đủ thành phố, quận/huyện và xã/phường.'
        });
        return null;
    }
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
        validateNotNull(street, "Địa chỉ")

    ) {
        return {
            createBy: userId || "Khách vãng lai",
            idUser: userId || "Khách vãng lai",
            hexCode: newHexCode,
            customerName: customerName,
            customerPhone: customerPhone,
            customerEmail: customerEmail,
            voucherCode: voucherCode,
            trackingCheck: false,
            shipDate: new Date().toISOString(),
            shippingAddress: shippingAddress,
            shippingAddressLine2: street,
            cotsts: shippingFee,
            shippingMethods: 1,
            orderStatus: 0,
            totalAmount: totalOrder,
            paymentMethods: paymentMethod,
            paymentStatus: 0,
            orderType: 0,
            status: 1,
            orderDetailsCreateVM: orderDetails
        };
    } else {
        console.error("Các trường bắt buộc không được để trống.");
        return null;
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
    console.log('orderData', orderData);
    console.log('orderDetailsCreateVM', orderData.orderDetailsCreateVM);

    if (!orderData) {
        return;
    }

    checkStockAvailability(orderData.orderDetailsCreateVM)
        .then(() => {
            if (orderData.paymentMethods === 0) {
                processOrder(orderData);
                return;
            }
            const hasLargeQuantity = orderData.orderDetailsCreateVM.some(detail => detail.quantity > 100);

            if (hasLargeQuantity) {
                $('#notificationModal').modal('show');
                $('body').addClass('modal-open');
                $('.modal-backdrop').hide();

                $('#confirmButton').on('click', function () {
                    $('#notificationModal').modal('hide');
                    $('body').removeClass('modal-open');
                    processOrder(orderData);
                });
            } else {
                processOrder(orderData);
            }
        })
        .catch(error => {
            Swal.fire({
                title: 'Hết hàng!',
                text: error,
                icon: 'error',
                confirmButtonText: 'OK'
            });
        });
}
function checkStockAvailability(orderDetails) {
    return new Promise((resolve, reject) => {
        const stockCheckRequests = orderDetails.map(detail => {
            return new Promise((resolve, reject) => {
                const xhr = new XMLHttpRequest();
                xhr.open('GET', `https://localhost:7241/api/Options/GetByID/${detail.idOptions}`, true);
                xhr.onreadystatechange = function () {
                    if (xhr.readyState === XMLHttpRequest.DONE) {
                        if (xhr.status === 200) {
                            const response = JSON.parse(xhr.responseText);
                            console.log('response', response);

                            if (response.stockQuantity >= detail.quantity) {
                                resolve(true);
                            } else {
                                reject(`Ôi không! Bạn chậm tay qua sớm sản phẩm [${response.productName}]-[${response.keyCode}] đã hết hàng rồi!`);
                            }
                        } else {
                            reject('Lỗi kiểm tra hàng tồn kho.');
                        }
                    }
                };
                xhr.send();
            });
        });

        Promise.all(stockCheckRequests)
            .then(() => resolve(true))
            .catch(error => reject(error));
    });
}
function processOrder(orderData) {
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

            const jwt = getJwtFromCookie();

            if (!jwt) {
                document.cookie = 'cart=; path=/; max-age=0';
            }

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
                            Swal.fire({
                                title: 'Lỗi!',
                                text: xhr.responseText,
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
document.addEventListener('DOMContentLoaded', function () {
    var openButton = document.getElementById('btn_open_voucher');
    var modal = new bootstrap.Modal(document.getElementById('voucherModal'));
    openButton.addEventListener('click', function () {
        modal.show();
        fetchVouchers(userId);
        document.querySelector('.modal-backdrop').remove();

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
            console.log('response', response);
            response.forEach(function (voucher) {
                console.log('response.status', voucher.status);

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
    var selectedVoucherElement = document.getElementById(`voucher_${voucherId}`);
    var voucherStatus = selectedVoucherElement.querySelector('.voucherStatus').textContent;

    if (voucherStatus === 'Công khai') {
        checkVoucherDetails(voucherId);
    } else if (userId !== null) {
        var xhrVoucherUser = new XMLHttpRequest();
        xhrVoucherUser.open('GET', `https://localhost:7241/api/VoucherUser/${voucherId}/${userId}`, true);
        xhrVoucherUser.setRequestHeader('accept', '*/*');

        xhrVoucherUser.onload = function () {
            if (xhrVoucherUser.status >= 200 && xhrVoucherUser.status < 300) {
                var voucherUser = JSON.parse(xhrVoucherUser.responseText);

                if (voucherUser.status === 0) {
                    Swal.fire({
                        icon: 'warning',
                        title: 'Không thể áp dụng',
                        text: `Voucher này đã được sử dụng hoặc không khả dụng cho tài khoản này.`,
                    });
                    return;
                }

                checkVoucherDetails(voucherId);
            } else {
                console.error('Failed to fetch voucher user details:', xhrVoucherUser.responseText);
            }
        };

        xhrVoucherUser.onerror = function () {
            console.error('Request error...');
        };

        xhrVoucherUser.send();
    } else {
        checkVoucherDetails(voucherId);
    }
}

function checkVoucherDetails(voucherId) {
    var xhrVoucher = new XMLHttpRequest();
    xhrVoucher.open('GET', `https://localhost:7241/api/Voucher/GetByID/${voucherId}`, true);
    xhrVoucher.setRequestHeader('accept', '*/*');

    xhrVoucher.onload = function () {
        if (xhrVoucher.status >= 200 && xhrVoucher.status < 300) {
            var voucher = JSON.parse(xhrVoucher.responseText);

            var totalAmount = parseFloat(document.getElementById('provisional_fee').textContent.replace(/[^0-9]/g, '')) || 0;

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
            toastr.success(`Áp dụng thành công voucher ${voucher.code}!`, 'Thành công');
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
function calculateDiscount(voucher, totalAmount) {
    let reducedValue = 0;
    if (voucher.type === 0) {
        reducedValue = totalAmount * (voucher.reducedValue / 100);
    } else if (voucher.type === 1) {
        reducedValue = voucher.reducedValue;
    }

    if (voucher.maximumAmount && reducedValue > voucher.maximumAmount) {
        reducedValue = voucher.maximumAmount;
    }

    return reducedValue;
}
function calculateTotals(cartItems, voucherValue, shippingFee) {
    let totalPrice = 0;
    cartItems.forEach(item => {
        totalPrice += item.quantity * item.price;
    });

    const discountAmount = voucherValue;
    const totalAfterDiscount = totalPrice - discountAmount;
    const finalTotal = totalAfterDiscount + shippingFee;

    return {
        totalPrice,
        discountAmount,
        totalAfterDiscount,
        shippingFee,
        finalTotal
    };
}
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
            const cleanedProvinceName = provinceName.replace(/Tỉnh\s*|Thành phố\s*/i, '').trim();

            //console.log(cleanedProvinceName);
            //console.log('Danh sách các tỉnh/thành phố:');
            //data.data.forEach(province => {
            //    console.log(province.ProvinceName);
            //});

            const province = data.data.find(province => province.ProvinceName.replace(/Tỉnh\s*|Thành phố\s*/i, '').trim() === cleanedProvinceName);

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

        //console.log('API Response:', responseData);
        if (responseData.code === 200) {
            shippingFee = responseData.data.total;
            document.getElementById('fee_ship').innerText = shippingFee;
            document.getElementById('fee_shipDisplay').innerText = shippingFee.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
            updateTotalOrder();
            toastr.success('Địa chỉ mặc định đã được chọn!', 'Thành công');
            toastr.warning(
                `Giá giao hàng theo địa chỉ <strong style="font-size: 18px; color: red;">${shippingFee.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</strong>`,
                'Phí giao hàng',
                {
                    timeOut: 5000,
                    escapeHtml: false,
                    positionClass: "toast-top-right",
                    closeButton: true,
                    progressBar: true
                }
            );

            //console.log('Giá:' + shippingFee)
        } else {
            console.error('Error:', data.message);
        }
    } catch (error) {
        console.error('Error:', error);
    }
}
