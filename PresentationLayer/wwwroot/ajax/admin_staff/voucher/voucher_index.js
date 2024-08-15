function fetchVouchers() {
    var url = 'https://localhost:7241/api/Voucher/getall';
    var xhr = new XMLHttpRequest();

    xhr.open('GET', url, true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
                var data = JSON.parse(xhr.responseText);
                displayVouchers(data);
            } else {
                console.error('Error fetching data. Status:', xhr.responseText);
            }
        }
    };
    xhr.send();
}
function displayVouchers(vouchers) {
    var voucherBody = document.getElementById('voucherBody');
    voucherBody.innerHTML = '';
    vouchers.forEach(function (voucher) {
        var row = document.createElement('tr');

        row.innerHTML = `
            <td>${voucher.code}</td>
            <td>${voucher.name}</td>
            <td style="font-weight: bold;">
                ${new Date(voucher.startDate).toLocaleString('en-GB')} <br> ${new Date(voucher.endDate).toLocaleString('en-GB')}
            </td>
            <td>${voucher.quantity}</td>
            <td>
                ${voucher.type === 'Percent' ? '%' : 'vnđ'}
            </td>
            <td>${voucher.minimumAmount}</td>
            <td>${voucher.maximumAmount}</td>
            <td>${voucher.reducedValue}</td>
            <td>${voucher.isActive}</td>
            <td>
                <button class="btn btn-primary btn-sm trash" type="button" title="Xóa" onclick="confirmDelete('${voucher.id}', 'userID')">
                    <i class="fas fa-trash-alt"></i>
                </button>
                |
                <button class="btn btn-primary btn-sm edit" type="button" title="Sửa" onclick="navigateToUpdatePage('${voucher.id}')">
                    <i class="fas fa-edit"></i>
                </button>
                |
                <a title="View" onclick="openVoucherModal('${voucher.id}')">
                    <i class="fas fa-eye"></i>
                </a>
            </td>

        `;
        voucherBody.appendChild(row);
    });
}
function navigateToUpdatePage(voucherId) {
    window.location.href = `/managerupdate_voucher/${voucherId}`;
}

document.addEventListener('DOMContentLoaded', function () {
    fetchVouchers();
});

function openVoucherModal(voucherId) {
    var xhr = new XMLHttpRequest();
    xhr.open('GET', `https://localhost:7241/api/Voucher/GetByID/${voucherId}`, true);

    xhr.onload = function () {
        if (xhr.status >= 200 && xhr.status < 300) {
            var data = JSON.parse(xhr.responseText);

            var modalvoucher = document.getElementById('modalvoucher');
            var modalvoucherName = document.getElementById('modalvoucherName');
            var modalvoucherStart = document.getElementById('modalvoucherStart');
            var modalvoucherEnd = document.getElementById('modalvoucherEnd');
            var modalvoucherType = document.getElementById('modalvoucherType');
            var modalvoucherQuantity = document.getElementById('modalvoucherQuantity');
            var modalvoucherMinimum = document.getElementById('modalvoucherMinimum');
            var modalvoucherMax = document.getElementById('modalvoucherMax');
            var modalvoucherValue = document.getElementById('modalvoucherValue');
            var modalStatus = document.getElementById('modalStatus');
            var voucherBody = document.getElementById('voucherBody');

            if (modalvoucher) modalvoucher.textContent = data.code;
            if (modalvoucherName) modalvoucherName.textContent = data.name;
            if (modalvoucherStart) modalvoucherStart.textContent = new Date(data.startDate).toLocaleString('en-GB');
            if (modalvoucherEnd) modalvoucherEnd.textContent = new Date(data.endDate).toLocaleString('en-GB');
            if (modalvoucherType) modalvoucherType.textContent = data.type === 0 ? 'Percent' : 'vnđ';
            if (modalvoucherQuantity) modalvoucherQuantity.textContent = data.quantity;
            if (modalvoucherMinimum) modalvoucherMinimum.textContent = data.minimumAmount;
            if (modalvoucherMax) modalvoucherMax.textContent = data.maximumAmount;
            if (modalvoucherValue) modalvoucherValue.textContent = data.reducedValue;
            if (modalStatus) modalStatus.textContent = data.isActive ? 'Active' : 'Inactive';
            //console.log(data.id)
            //console.log(data.idUser)
            //if (voucherBody) {
            //    voucherBody.innerHTML = '';
            //    data.idUser.forEach(function (user) {
            //        var row = document.createElement('tr');
            //        row.innerHTML = `
            //            <td>${user.id}</td>
            //            <td>${user.name}</td>
            //            <td>${user.account}</td>
            //            <td>${user.status}</td>
            //        `;
            //        voucherBody.appendChild(row);
            //    });
            //}

            var modalElement = document.getElementById('voucherModal');
            var modal = new bootstrap.Modal(modalElement);
            modal.show();
        } else {
            console.error('Error fetching voucher data', xhr.statusText);
        }
    };

    xhr.onerror = function () {
        console.error('Request failed');
    };

    xhr.send();
}

document.addEventListener('DOMContentLoaded', function () {
    var buttons = document.querySelectorAll('.view-voucher-btn');
    buttons.forEach(function (button) {
        button.addEventListener('click', function () {
            var voucherId = this.getAttribute('data-voucher-id');
            openVoucherModal(voucherId);
        });
    });
});

function deletevoucher(IDVoucher, userID) {
    var xhr = new XMLHttpRequest();
    xhr.open('DELETE', `https://localhost:7241/api/Voucher/${IDVoucher}/${userID}`, true);
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
                Swal.fire({
                    icon: 'success',
                    title: 'Thành công!',
                    text: 'Đã xóa voucher thành công.',
                    timer: 3000,
                    timerProgressBar: true,
                    willClose: () => {
                        window.location.href = '/home/index_voucher';
                    }
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi!',
                    text: 'Đã xảy ra lỗi khi xóa voucher. Vui lòng thử lại sau.'
                });
            }
        }
    };

    try {
        console.log(IDVoucher)
        console.log(userID)
        xhr.send();

    } catch (error) {
        console.error('Error sending request:', error);
        Swal.fire({
            icon: 'error',
            title: 'Lỗi!',
            text: 'Đã xảy ra lỗi khi gửi yêu cầu xóa voucher.'
        });
    }
}
function confirmDelete(IDVoucher, userID) {
    Swal.fire({
        title: 'Bạn có chắc chắn muốn xóa voucher này?',
        text: "Hành động này không thể hoàn tác!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Có, xóa nó!',
        cancelButtonText: 'Hủy bỏ'
    }).then((result) => {
        if (result.isConfirmed) {
            deletevoucher(IDVoucher, userID);
        }
    });
}
