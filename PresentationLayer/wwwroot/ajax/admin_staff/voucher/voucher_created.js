document.addEventListener('DOMContentLoaded', function () {
    function loadUsers() {
        fetch('https://localhost:7241/api/ApplicationUser/GetAllActiveInformationUserAsync')
            .then(response => response.json())
            .then(data => {
                const userListDiv = document.getElementById('userList');
                userListDiv.innerHTML = '';

                data.forEach(user => {
                    const userDiv = document.createElement('div');
                    userDiv.classList.add('variant-item');

                    const checkbox = document.createElement('input');
                    checkbox.type = 'checkbox';
                    checkbox.classList.add('userCheckbox');
                    checkbox.value = user.id;
                    userDiv.appendChild(checkbox);

                    const userInfoDiv = document.createElement('div');
                    userInfoDiv.classList.add('variant-info');

                    const fullNameSpan = document.createElement('span');
                    fullNameSpan.textContent = `Tên: ${user.firstAndLastName}`;
                    userInfoDiv.appendChild(fullNameSpan);

                    const usernameSpan = document.createElement('span');
                    usernameSpan.textContent = `Tài khoản: ${user.username}`;
                    userInfoDiv.appendChild(usernameSpan);

                    userDiv.appendChild(userInfoDiv);

                    userListDiv.appendChild(userDiv);
                });
            })
            .catch(error => {
                console.error('Error loading users:', error);
            });
    }

    loadUsers();

    const checkAllCheckbox = document.getElementById('checkAll');
    if (checkAllCheckbox) {
        checkAllCheckbox.addEventListener('change', function () {
            const checkboxes = document.querySelectorAll('.userCheckbox');
            checkboxes.forEach(checkbox => {
                checkbox.checked = checkAllCheckbox.checked;
            });
        });
    }
});
document.addEventListener("DOMContentLoaded", function () {
    var radioPercentage = document.getElementById('typePercentage');
    var radioCash = document.getElementById('typeCash');
    var labelForReducedValue = document.getElementById('labelForReducedValue');
    var reducedValueSuffix = document.getElementById('reducedValueSuffix');

    function updateLabelAndSuffix() {
        if (radioPercentage.checked) {
            labelForReducedValue.textContent = 'Giá trị giảm (%)';
            reducedValueSuffix.textContent = '%';
            inputReducedValue.type = 'number';
            inputReducedValue.min = 1;
            inputReducedValue.max = 100;
            inputReducedValue.step = 1;
        } else if (radioCash.checked) {
            labelForReducedValue.textContent = 'Giá trị giảm (tiền mặt)';
            reducedValueSuffix.textContent = 'vnđ';
            inputReducedValue.type = 'number'; 
            delete inputReducedValue.min;
            delete inputReducedValue.max;
            delete inputReducedValue.step;
        }
    }

    radioPercentage.addEventListener('change', updateLabelAndSuffix);
    radioCash.addEventListener('change', updateLabelAndSuffix);

    updateLabelAndSuffix();
});


function attachCheckboxEvents() {
    var checkAll = document.getElementById('checkAll');
    var checkboxes = document.querySelectorAll('.productCheckbox');

    checkAll.addEventListener('change', function () {
        checkboxes.forEach(function (checkbox) {
            checkbox.checked = checkAll.checked;
        });
    });

    checkboxes.forEach(function (checkbox) {
        checkbox.addEventListener('change', function () {
            checkAll.checked = Array.from(checkboxes).every(c => c.checked);
        });
    });
}

document.addEventListener("DOMContentLoaded", function () {
    attachCheckboxEvents();
});

attachCheckboxEvents();

function submitForm() {
    var code = document.getElementById('code').value;
    var name = document.getElementById('name').value;
    var startdate = document.getElementById('startdate').value;
    var enddate = document.getElementById('enddate').value;
    var quantity = document.getElementById('quantity').value;
    var minimum = document.getElementById('miniamount').value;
    var maxmum = document.getElementById('maxamount').value;
    var reduce = document.getElementById('ReducedValue').value;
    //var isActive = document.getElementById('isActive').checked;
    var selectedUser = [];
    var checkboxes = document.querySelectorAll('#userList .userCheckbox:checked');
    checkboxes.forEach(function (checkbox) {
        selectedUser.push(checkbox.value);
    });


    var type;

    var radioPercentage = document.getElementById('typePercentage');
    var radioCash = document.getElementById('typeCash');
    if (radioPercentage.checked) {
        type = 0;
    } else if (radioCash.checked) {
        type = 1;
    }
    if (!code || !name || !startdate || !enddate || !quantity || !minimum || !maxmum || !reduce || type === undefined) {
        Swal.fire({
            title: 'Lỗi',
            text: 'Vui lòng điền đầy đủ thông tin các trường bắt buộc.',
            icon: 'error',
        });
        return;
    }
    if (startdate > enddate) {
        Swal.fire({
            title: 'Lỗi',
            text: 'Ngày bắt đầu phải trước hoặc bằng ngày kết thúc.',
            icon: 'error',
        });
        return;
    }
    if (type === 0) {
        var reducedValue = parseFloat(reduce);
        if (isNaN(reducedValue) || reducedValue <= 0 || reducedValue > 100) {
            Swal.fire({
                title: 'Lỗi',
                text: 'Giá trị giảm (%) phải là một số lớn hơn 0 và nhỏ hơn hoặc bằng 100.',
                icon: 'error',
            });
            return;
        }
    } else if (type === 1) { 
        var reducedValue = parseFloat(reduce);
        if (isNaN(reducedValue) || reducedValue <= 0) {
            Swal.fire({
                title: 'Lỗi',
                text: 'Giá trị giảm (tiền mặt) phải là một số lớn hơn 0.',
                icon: 'error',
            });
            return;
        }
    }
    var data = {
        CreateBy: 'acb',
        Code: code,
        Name: name,
        StartDate: startdate,
        EndDate: enddate,
        Quantity: quantity,
        MinimumAmount: minimum,
        MaximumAmount: maxmum,
        ReducedValue: reduce,
        //IsActive: isActive,
        Types: type,
        Status: 1,
        SelectedUser: selectedUser
    };
    console.log("data:", data);

    Swal.fire({
        title: 'Đang xử lý...',
        text: 'Vui lòng chờ trong khi chúng tôi xử lý yêu cầu của bạn.',
        icon: 'info',
        allowOutsideClick: false,
        didOpen: () => {
            Swal.showLoading();
        }
    });

    var xhr = new XMLHttpRequest();
    xhr.open('POST', 'https://localhost:7241/api/Voucher/create', true);
    xhr.setRequestHeader('Content-Type', 'application/json');

    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            Swal.close(); 
            if (xhr.status === 200) {
                Swal.fire({
                    title: 'Thành công',
                    text: 'Chương trình voucher đã được tạo.',
                    icon: 'success',
                }).then(() => {
                    window.location.href = '/home/index_voucher';
                });
            } else {
                Swal.fire({
                    title: 'Lỗi',
                    text: 'Có lỗi xảy ra khi tạo voucher khuyến mãi.',
                    icon: 'error',
                });
            }
        }
    };

    xhr.onerror = function () {
        Swal.close(); 
        Swal.fire({
            title: 'Lỗi',
            text: 'Có lỗi xảy ra trong quá trình gửi yêu cầu.',
            icon: 'error',
        });
        console.error('Lỗi:', xhr.statusText);
    };

    xhr.send(JSON.stringify(data));
}

document.getElementById('btn_create').addEventListener('click', function () {
    Swal.fire({
        title: 'Xác nhận tạo chương trình voucher?',
        text: 'Bạn có chắc muốn tạo chương trình này?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Đồng ý',
        cancelButtonText: 'Hủy',
    }).then((result) => {
        if (result.isConfirmed) {
            submitForm();
        }
    });
});