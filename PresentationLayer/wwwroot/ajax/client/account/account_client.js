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

function fetchUserData() {
    const xhr = new XMLHttpRequest();
    xhr.open('GET', `https://localhost:7241/api/ApplicationUser/GetInformationUser/${userId}`, true);
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.setRequestHeader('Authorization', `Bearer ${getJwtFromCookie()}`);

    xhr.onload = function () {
        if (xhr.status >= 200 && xhr.status < 300) {
            try {
                const data = JSON.parse(xhr.responseText);
                console.log('data:', data)
                console.log('firstAndLastName:', data.firstAndLastName)
                if (data) {
                    document.getElementById('images').src = data.images || '';
                    document.getElementById('username').textContent = data.username || '';
                    document.getElementById('name_user_1').value = data.firstAndLastName;
                    document.getElementById('gmail').value = data.email || '';
                    document.getElementById('inputPhone').value = data.phoneNumber || '';
                    document.getElementById('inputBirthday').value = data.dateOfBirth ? data.dateOfBirth.split('T')[0] : '';

                    var genderInputs = document.getElementsByName('Gender');
                    genderInputs.forEach(input => {
                        if (input.value == data.status) {
                            input.checked = true;
                        }
                    });
                }
                loadUserAddresses(userId);
            }
            catch (error) {
                console.error('Error parsing response:', error);
            }
        } else {
            console.error('Request failed. Status:', xhr.status);
        }
    };

    xhr.onerror = function () {
        console.error('Request error');
    };

    xhr.send();
}

document.addEventListener('DOMContentLoaded', function () {
    const jwt = getJwtFromCookie();
    if (!jwt) {
        console.error('JWT not found in cookies.');
        return;
    }

    const userId = getUserIdFromJwt(jwt);
    if (!userId) {
        console.error('User ID not found in JWT.');
        return;
    }

    fetchUserData(userId);
});
function loadUserAddresses(userId) {
    var xhr = new XMLHttpRequest();
    xhr.open('GET', `https://localhost:7241/api/Address/user/${userId}`, true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            var data = JSON.parse(xhr.responseText);

            var addressList = document.getElementById('address-list');
            addressList.innerHTML = '';

            data.forEach(function (address) {
                var addressItem = document.createElement('li');
                addressItem.classList.add('address-item');
                addressItem.innerHTML = `
                    <div class="address-details">
                        <strong id="name_address">${address.firstAndLastName}</strong> |
                        <strong id="phonenumber_address">${address.phoneNumber}</strong><small style="color:red;"> ${address.isDefault ? '(Mặc định)' : ''}</small>
                        <br>
                        <span id="address">${address.specificAddress}, ${address.commune}, ${address.districtCounty}, ${address.city}</span>
                    </div>
                    <div class="address-actions">
                        <button class="btn-update btn btn-warning btn-sm trash" onclick="updateAddress('${address.id}')"> 
                            <i class="fas fa-pencil-alt"></i>
                        </button>
                        <button class="btn btn-danger btn-sm trash" onclick="deleteAddress('${address.id}')" type="button">
                            <i class="fa fa-ban"></i>
                        </button>
                    </div>
                `;
                addressList.appendChild(addressItem);
            });
        } else if (xhr.readyState === 4) {
            console.error('Error fetching user addresses:', xhr.responseText);
            alert('Đã xảy ra lỗi khi lấy dữ liệu địa chỉ. Vui lòng thử lại sau.');
        }
    };
    xhr.send();
}
document.getElementById('logoutButton').addEventListener('click', function (event) {
    event.preventDefault();

    Swal.fire({
        title: 'Bạn có chắc chắn?',
        text: "Bạn sẽ bị đăng xuất khỏi tài khoản của mình.",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#dc3545',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Đăng xuất',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            document.cookie.split(';').forEach(function (c) {
                document.cookie = c.trim().split('=')[0] + '=;expires=Thu, 01 Jan 1970 00:00:00 UTC;path=/;';
            });

            window.location.href = '/login';
        }
    });
});
function saveAddress() {
    const firstAndLastName = document.getElementById('firstAndLastName').value;
    const phoneNumber = document.getElementById('phoneNumber').value;
    const gmail = document.getElementById('gmail').value;
    const city = document.getElementById('city').value;
    const districtCounty = document.getElementById('district').value;
    const commune = document.getElementById('ward').value;
    const specificAddress = document.getElementById('specificAddress').value;

    const isDefault = document.getElementById('IsDefault').checked;

    if (firstAndLastName === '' || phoneNumber === '' || gmail === '' || city === '' || districtCounty === '' || commune === '' || specificAddress === '') {
        Swal.fire(
            'Lỗi!',
            'Thông tin không được bỏ trống!',
            'error'
        );
        return;
    }
    const addressData = {
        firstAndLastName: firstAndLastName,
        phoneNumber: phoneNumber,
        gmail: gmail,
        createBy: userId,
        idUser: userId,
        city: city,
        districtCounty: districtCounty,
        commune: commune,
        isDefault: isDefault,
        specificAddress: specificAddress
    };
    console.log(addressData)
    var xhr = new XMLHttpRequest();
    xhr.open('POST', 'https://localhost:7241/api/Address/create_address', true);
    xhr.setRequestHeader('Content-Type', 'application/json');

    xhr.onreadystatechange = function () {
        if (xhr.readyState === XMLHttpRequest.DONE) {
            if (xhr.status === 200) {
                Swal.fire({
                    icon: 'success',
                    title: 'Thành công!',
                    text: 'Địa chỉ đã được thêm thành công.',
                    confirmButtonText: 'OK'
                }).then(() => {
                    document.getElementById('addAddressModal').classList.remove('show');
                    document.getElementById('addAddressModal').style.display = 'none';
                    document.body.classList.remove('modal-open');
                    document.querySelectorAll('.modal-backdrop').forEach(el => el.remove());
                    loadUserAddresses(userId);
                });
            }
        } else if (xhr.status === 400) {
            Swal.fire({
                icon: 'error',
                title: 'Lỗi!',
                text: 'Số điện thoại đã tồn tại. Vui lòng kiểm tra lại.',
                confirmButtonText: 'OK'
            })
        } else {
            Swal.fire({
                icon: 'error',
                title: 'Lỗi!',
                text: 'Đã xảy ra lỗi khi thêm địa chỉ. Vui lòng thử lại.',
                confirmButtonText: 'OK'
            });
        }
    };

    xhr.send(JSON.stringify(addressData));
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
            } catch (error) {
                console.error('Error:', error);
            }
        }
    }
});
function deleteAddress(id) {
    Swal.fire({
        title: 'Bạn có chắc chắn muốn xóa địa chỉ này?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Xóa',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            const xhr = new XMLHttpRequest();
            xhr.open('DELETE', `https://localhost:7241/api/Address/DeleteAddress/${id}?IDUserDelete=${userId}`, true);
            xhr.onreadystatechange = function () {
                if (xhr.readyState === XMLHttpRequest.DONE) {
                    if (xhr.status === 200) {
                        Swal.fire(
                            'Thành công!',
                            'Địa chỉ đã được xóa thành công!',
                            'success'
                        );
                        loadUserAddresses(userId);
                    } else {
                        Swal.fire(
                            'Lỗi!',
                            'Đã có lỗi xảy ra khi xóa địa chỉ!' + xhr.responseText,
                            'error'
                        );
                    }
                }
            };
            xhr.send();
        }
    });
}
