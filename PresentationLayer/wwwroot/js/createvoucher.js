function generateRandomCode() {
    const characters = 'ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789';
    let result = '';
    const length = 10; // Độ dài mã voucher

    for (let i = 0; i < length; i++) {
        const randomIndex = Math.floor(Math.random() * characters.length);
        result += characters[randomIndex];
    }

    document.getElementById('voucherCode').value = result;
}
function validateForm() {
    let isValid = true;

    // Lấy các trường input
    const code = document.getElementById('voucherCode');
    const name = document.querySelector('input[name="Name"]');
    const startDate = new Date(document.querySelector('input[name="StartDate"]').value);
    const endDate = new Date(document.querySelector('input[name="EndDate"]').value);
    const quantity = parseInt(document.querySelector('input[name="Quantity"]').value, 10);
    const minimumAmount = parseFloat(document.querySelector('input[name="MinimumAmount"]').value);
    const maximumAmount = parseFloat(document.querySelector('input[name="MaximumAmount"]').value);
    const reducedValue = parseFloat(document.querySelector('input[name="ReducedValue"]').value);
    const applyToAllUsers = document.getElementById('applyToAllUsers').checked;

    // Lấy ngày hiện tại
    const today = new Date();   
    today.setHours(0, 0, 0, 0); // Đặt giờ về 00:00:00 để so sánh chính xác
    let startDate = new Date(startDateInput.value);
    let endDate = new Date(endDateInput.value);
    let quantity = parseInt(quantityInput.value, 10);
    let minimumAmount = parseFloat(minimumAmountInput.value);
    let maximumAmount = parseFloat(maximumAmountInput.value);
    let reducedValue = parseFloat(reducedValueInput.value);
    // Xóa thông báo lỗi trước đó
    document.querySelectorAll('.text-danger').forEach(el => el.textContent = '');

    // Kiểm tra các trường input và cập nhật thông báo lỗi
    if (!code.value) {
        document.querySelector('span[asp-validation-for="Code"]').textContent = 'Mã voucher không được để trống';
        isValid = false;
        console.log('Mã voucher không được để trống');
    } else {
        console.log('Mã voucher: ' + code.value);
    }

    // Kiểm tra Tên voucher
    if (!name.value) {
        document.querySelector('span[asp-validation-for="Name"]').textContent = 'Tên voucher không được để trống';
        isValid = false;
        console.log('Tên voucher không được để trống');
    } else {
        console.log('Tên voucher: ' + name.value);
    }

    // Kiểm tra Ngày bắt đầu
    if (!startDateInput.value || isNaN(startDate.getTime())) {
        document.querySelector('span[asp-validation-for="StartDate"]').textContent = 'Ngày bắt đầu không được để trống và phải là ngày hợp lệ';
        isValid = false;
        console.log('Ngày bắt đầu không hợp lệ');
    } else if (startDate < today) {
        document.querySelector('span[asp-validation-for="StartDate"]').textContent = 'Ngày bắt đầu phải lớn hơn hoặc bằng ngày hiện tại';
        isValid = false;
        console.log('Ngày bắt đầu phải lớn hơn hoặc bằng ngày hiện tại');
    } else {
        console.log('Ngày bắt đầu: ' + startDate.toISOString().split('T')[0]);
    }

    // Kiểm tra Ngày kết thúc
    if (!endDateInput.value || isNaN(endDate.getTime())) {
        document.querySelector('span[asp-validation-for="EndDate"]').textContent = 'Ngày kết thúc không được để trống và phải là ngày hợp lệ';
        isValid = false;
        console.log('Ngày kết thúc không hợp lệ');
    } else if (endDate < today) {
        document.querySelector('span[asp-validation-for="EndDate"]').textContent = 'Ngày kết thúc phải lớn hơn hoặc bằng ngày hiện tại';
        isValid = false;
        console.log('Ngày kết thúc phải lớn hơn hoặc bằng ngày hiện tại');
    } else if (endDate <= startDate) {
        document.querySelector('span[asp-validation-for="EndDate"]').textContent = 'Ngày kết thúc phải lớn hơn ngày bắt đầu';
        isValid = false;
        console.log('Ngày kết thúc phải lớn hơn ngày bắt đầu');
    } else {
        console.log('Ngày kết thúc: ' + endDate.toISOString().split('T')[0]);
    }

    // Kiểm tra Số lượng
    if (isNaN(quantity) || quantity <= 0 || quantity >= 100000) {
        document.querySelector('span[asp-validation-for="Quantity"]').textContent = 'Số lượng phải lớn hơn 0 và nhỏ hơn 100000';
        isValid = false;
        console.log('Số lượng phải lớn hơn 0 và nhỏ hơn 100000');
    } else {
        console.log('Số lượng: ' + quantity);
    }

    // Kiểm tra Số tiền giảm tối thiểu
    if (isNaN(minimumAmount) || minimumAmount <= 0) {
        document.querySelector('span[asp-validation-for="MinimumAmount"]').textContent = 'Số tiền giảm tối thiểu phải lớn hơn 0';
        isValid = false;
        console.log('Số tiền giảm tối thiểu phải lớn hơn 0');
    } else {
        console.log('Số tiền giảm tối thiểu: ' + minimumAmount);
    }

    // Kiểm tra Số tiền giảm tối đa
    if (isNaN(maximumAmount) || maximumAmount <= 0) {
        document.querySelector('span[asp-validation-for="MaximumAmount"]').textContent = 'Số tiền giảm tối đa phải lớn hơn 0';
        isValid = false;
        console.log('Số tiền giảm tối đa phải lớn hơn 0');
    } else if (minimumAmount >= maximumAmount) {
        document.querySelector('span[asp-validation-for="MaximumAmount"]').textContent = 'Số tiền giảm tối đa phải lớn hơn số tiền giảm tối thiểu';
        isValid = false;
        console.log('Số tiền giảm tối đa phải lớn hơn số tiền giảm tối thiểu');
    } else {
        console.log('Số tiền giảm tối đa: ' + maximumAmount);
    }

    // Kiểm tra Số tiền giảm
    if (isNaN(reducedValue) || reducedValue <= 0) {
        document.querySelector('span[asp-validation-for="ReducedValue"]').textContent = 'Số tiền giảm phải lớn hơn 0';
        isValid = false;
        console.log('Số tiền giảm phải lớn hơn 0');
    } else {
        console.log('Số tiền giảm: ' + reducedValue);
    }

    // Kiểm tra khách hàng được chọn nếu không áp dụng cho tất cả khách hàng
    if (!applyToAllUsers) {
        const selectedUsers = document.querySelectorAll('input[name="SelectedUser"]:checked');
        if (selectedUsers.length === 0) {
            document.querySelector('span[asp-validation-for="SelectedUser"]').textContent = 'Vui lòng chọn ít nhất một khách hàng.';
            isValid = false;
            console.log('Vui lòng chọn ít nhất một khách hàng');
        } else {
            console.log('Khách hàng được chọn:');
            selectedUsers.forEach(user => console.log('ID: ' + user.value));
        }
    } else {
        console.log('Áp dụng cho tất cả khách hàng');
    }

    // Hiển thị thông báo lỗi nếu không hợp lệ
    if (!isValid) {
        alert('Vui lòng kiểm tra các trường nhập liệu và sửa lỗi.');
    }

    // Hiển thị thông báo lỗi nếu không hợp lệ
    if (!isValid) {
        alert('Vui lòng kiểm tra các trường nhập liệu và sửa lỗi.');
    }

    return isValid;
}

