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
    var currentUrl = window.location.href;
    var urlParts = currentUrl.split('/');
    var ID = urlParts[urlParts.length - 1];
    viewDetails(ID);

    document.getElementById('btnUpdateOrder').addEventListener('click', function (event) {
        event.preventDefault();

        Swal.fire({
            title: 'Bạn có chắc chắn muốn cập nhật đơn hàng?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Có, cập nhật!',
            cancelButtonText: 'Không'
        }).then((result) => {
            if (result.isConfirmed) {
                const apiUrl = `https://localhost:7241/api/Order/UpdateOrder/${ID}/${userId}`;

                const orderDetails = Array.from(document.querySelectorAll('.order-details')).map(row => {
                    const idOptions = document.getElementById('idoptions').value;
                    const quantity = document.getElementById('quantity').value;

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

                const costsElement = document.getElementById('modalcosts').innerHTML;
                const cotsts = formatCurrencyToDecimal(costsElement);
                console.log('cotsts', cotsts);

                if (isNaN(cotsts)) {
                    console.error("Giá trị cotsts không hợp lệ:", costsElement);
                }
                const orderData = {
                    modifiedBy: userId,
                    idUser: userId,
                    customerName: document.getElementById('modalcusname').value,
                    customerPhone: document.getElementById('modalcusphone').value,
                    customerEmail: document.getElementById('modalemail').value,
                    shippingAddress: document.getElementById('modalshipaddess').value,
                    shippingAddressLine2: document.getElementById('modalshipaddress2').value,
                    cotsts: cotsts,
                    orderDetails: orderDetails
                };

                // Hiển thị thông báo chờ
                Swal.fire({
                    title: 'Đang xử lý...',
                    text: 'Vui lòng chờ trong giây lát!',
                    allowOutsideClick: false,
                    allowEscapeKey: false,
                    didOpen: () => {
                        Swal.showLoading();
                    }
                });

                var xhr = new XMLHttpRequest();
                xhr.open('PUT', apiUrl, true);
                xhr.setRequestHeader('Content-Type', 'application/json');

                xhr.onload = function () {
                    if (xhr.readyState === xhr.DONE) {
                        if (xhr.status === 200) {
                            Swal.fire({
                                title: 'Thành công!',
                                text: 'Cập nhật đơn hàng thành công.',
                                icon: 'success'
                            }).then(() => {
                                window.location.href = '/';
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
                    Swal.fire({
                        title: 'Lỗi!',
                        text: 'Có lỗi kết nối đến máy chủ.' + xhr.responseText,
                        icon: 'error'
                    });
                };

                xhr.send(JSON.stringify(orderData));
            }
        });
    });
});
async function viewDetails(ID) {
    try {
        //showLoader();
        const response = await fetch(`https://localhost:7241/api/Order/GetByIDAsync/${ID}`);
        if (!response.ok) {
            throw new Error('Error fetching order details');
        }
        const data = await response.json();
        console.log(data)
        document.getElementById('modalcreate').innerText = formatDateTime(data.createDate);
        document.getElementById('modalvoucher').innerText = data.voucherCode || "Không có";
        document.getElementById('modalhexcode').innerText = data.hexCode;
        document.getElementById('modalcusname').value = data.customerName;
        document.getElementById('modalcusphone').value = data.customerPhone;
        document.getElementById('modalemail').value = data.customerEmail;
        document.getElementById('modalshipaddess').value = data.shippingAddress;
        document.getElementById('modalshipaddress2').value = data.shippingAddressLine2 || "Không có";
        document.getElementById('modalcosts').innerText = data.cotsts.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
        document.getElementById('modaltotal').innerText = data.totalAmount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
        document.getElementById('modalpaymentmethod').innerText = translatePaymentMethod(data.paymentMethod);
        document.getElementById('modalpaymentstatus').innerText = translatePaymentStatus(data.paymentStatus);
        document.getElementById('modalshippingmethod').innerText = translateShippingMethod(data.shippingMethod);
        document.getElementById('modalorderstatus').innerText = translateOrderStatus(data.orderStatus);
        document.getElementById('modalordertype').innerText = translateOrderType(data.orderType);
        const updateOrderButton = document.getElementById('btnUpdateOrder');
        const hiddenStatuses = [2,3, 4]; // 2: Vận chuyển hoàn thành, 4: Hủy
        if (hiddenStatuses.includes(data.orderStatus)) {
            updateOrderButton.style.display = 'none';
        } else {
            updateOrderButton.style.display = 'inline-block'; // Hoặc 'block', tùy vào cách bạn muốn hiển thị nút
        }
        const orderBody = document.getElementById('orderBody');
        if (orderBody) {
            orderBody.innerHTML = '';
            if (data.orderDetailsVM && data.orderDetailsVM.length > 0) {
                data.orderDetailsVM.forEach(detail => {
                    var row = document.createElement('tr');
                    row.classList.add('order-detail');
                    row.setAttribute('data-id-options', detail.idOptions);
                    row = `
                        <tr>
                            <input value="${detail.idOptions}" id="idoptions" hidden></input>
                            <td>${detail.productName || 'N/A'}</td>
                            <td>${detail.sizeName || 'N/A'}</td>
                            <td>${detail.colorName || 'N/A'}</td>
                            <td><input id="quantity" value="${detail.quantity}"></input></td>
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

    } catch (error) {
        console.error('Error fetching order details:', error.message);
    } finally {
        //hideLoader();
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

document.addEventListener('DOMContentLoaded', function () {
    var citySelect = document.getElementById("city");
    var districtSelect = document.getElementById("district");
    var wardSelect = document.getElementById("ward");
    var shippingAddressInput = document.getElementById("modalshipaddess");

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
    var modal = document.getElementById("addressModal");
    var btn = document.getElementById("openAddressModalBtn");
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
                    document.getElementById('modalcosts').innerText = shippingFee;
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
