﻿document.addEventListener('DOMContentLoaded', function () {
    const btnLogin = document.getElementById('btn_login');

    btnLogin.addEventListener('click', function (event) {
        event.preventDefault();

        const username = document.getElementById('username_login').value;
        const password = document.getElementById('password_login').value;

        Swal.fire({
            title: 'Đang đăng nhập...',
            html: 'Vui lòng đợi trong giây lát.',
            allowOutsideClick: false,
            showConfirmButton: false,
            willOpen: () => {
                Swal.showLoading();
            }
        });

        const xhr = new XMLHttpRequest();
        xhr.open('POST', 'https://localhost:7241/api/ApplicationUser/Login');
        xhr.setRequestHeader('Content-Type', 'application/json');

        xhr.onload = function () {
            if (xhr.status === 200) {
                const response = JSON.parse(xhr.responseText);
                const { token, role } = response;

                document.cookie = `jwt=${token}; path=/`;

                Swal.fire({
                    icon: 'success',
                    title: 'Đăng nhập thành công!',
                    showConfirmButton: false,
                    timer: 1500
                }).then(() => {
                    if (role === 'Admin' || role == 'Staff') {
                        window.location.href = '/admin/home/index';
                    } else {
                        window.location.href = '/main';
                    }
                });
            } else {
                console.error('Đăng nhập thất bại:', xhr.responseText);
                Swal.fire({
                    icon: 'error',
                    title: 'Đăng nhập thất bại!',
                    text: 'Vui lòng kiểm tra lại tài khoản và mật khẩu.',
                    confirmButtonColor: '#3085d6',
                    confirmButtonText: 'Đã hiểu'
                });
            }
        };

        xhr.onerror = function () {
            console.error('Lỗi kết nối.');
            Swal.fire({
                icon: 'error',
                title: 'Lỗi kết nối!',
                text: 'Vui lòng thử lại sau.',
                confirmButtonColor: '#3085d6',
                confirmButtonText: 'Đã hiểu'
            });
        };

        const data = JSON.stringify({
            userName: username,
            passWord: password
        });

        xhr.send(data);
    });
});

function getJwtTokenFromCookies() {
    const cookieName = 'jwt=';
    const decodedCookies = decodeURIComponent(document.cookie);
    const cookiesArray = decodedCookies.split(';');
    for (let i = 0; i < cookiesArray.length; i++) {
        let cookie = cookiesArray[i].trim();
        if (cookie.indexOf(cookieName) === 0) {
            return cookie.substring(cookieName.length, cookie.length);
        }
    }
    return null;
    console.log(cookieName)
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

document.getElementById('btn_register').addEventListener('click', function () {
    Swal.fire({
        title: 'Xác nhận',
        text: 'Bạn có chắc chắn muốn đăng ký không?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Đăng ký',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            registerUser();
        }
    });
});

function registerUser() {

    var formData = new FormData();

    formData.append('Username', document.getElementById('username').value);
    formData.append('FirstAndLastName', document.getElementById('firstandlastname').value);
    formData.append('Email', document.getElementById('gmail').value);
    formData.append('PhoneNumber', document.getElementById('phoneNumber').value);
    var genderSelect = document.getElementById('gender');
    var selectedGender = genderSelect.options[genderSelect.selectedIndex].value;
    formData.append('Gender', selectedGender);
    formData.append('DateOfBirth', document.getElementById('dateorbirth').value);
    formData.append('Password', document.getElementById('pass').value);
    formData.append('ConfirmPassword', document.getElementById('confirm_pass').value);
    formData.append('City', document.getElementById('city').value);
    formData.append('DistrictCounty', document.getElementById('district').value);
    formData.append('Commune', document.getElementById('ward').value);
    formData.append('SpecificAddress', document.getElementById('specificAddress').value);

    var fileInput = document.getElementById('fileInput');
    if (fileInput.files.length > 0) {
        formData.append('Images', fileInput.files[0]);
    }

    var xhr = new XMLHttpRequest();
    xhr.open('POST', 'https://localhost:7241/api/ApplicationUser/Register?role=client', true);
    xhr.onload = function () {
        if (xhr.status >= 200 && xhr.status < 300) {
            Swal.fire({
                title: 'Thành công!',
                text: 'Bạn đã đăng ký thành công.',
                icon: 'success',
                confirmButtonText: 'OK'
            }).then(() => {
                window.location.href = '/login'; 
            });
        } else {
            Swal.fire({
                title: 'Lỗi!',
                text: 'Có lỗi xảy ra trong quá trình đăng ký.',
                icon: 'error',
                confirmButtonText: 'OK'
            });
            console.log(xhr.responseText)

        }
    };
    xhr.onerror = function () {
        Swal.fire({
            title: 'Lỗi!',
            text: 'Có lỗi xảy ra trong quá trình gửi yêu cầu.',
            icon: 'error',
            confirmButtonText: 'OK'
        });
        console.log(xhr.responseText)
    };
    console.log(formData)
    xhr.send(formData);
}