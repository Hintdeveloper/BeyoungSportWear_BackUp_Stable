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
    console.log(ID);

    getOrderByID(ID);

    document.getElementById('btn_update_order').addEventListener('click', function (event) {
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
                console.log('ID', ID)
                console.log('userId', userId)

                const orderDetails = Array.from(document.querySelectorAll('.order-detail-row')).map(row => {
                    const idOptions = row.getAttribute('data-id-options');
                    const quantity = row.querySelector('input[id="quantity"]').value;

                    return {
                        idOptions: idOptions,
                        quantity: parseInt(quantity)
                    };
                });
                const orderData = {
                    modifiedBy: userId,
                    idUser: userId,
                    customerName: document.getElementById('customerName').value,
                    customerPhone: document.getElementById('customerPhone').value,
                    customerEmail: document.getElementById('customerEmail').value,
                    shippingAddress: document.getElementById('shippingAddress').value,
                    shippingAddressLine2: document.getElementById('shippingAddressLine2').value,
                    shipDate: document.getElementById('shipDate').value,
                    cotsts: 0,
                    notes: document.getElementById('notes').value,
                    orderDetails: orderDetails
                };
                console.log(orderData)
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
                                window.location.href = '/home/index_order';
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

function getOrderByID(ID) {
    var apiUrl = `https://localhost:7241/api/Order/GetByIDAsync/${ID}`;

    var xhr = new XMLHttpRequest();
    xhr.open('GET', apiUrl, true);

    xhr.onload = function () {
        if (xhr.readyState === xhr.DONE) {
            if (xhr.status === 200) {
                var order = JSON.parse(xhr.responseText);
                console.log(order)
                document.getElementById('hexcode').textContent = order.hexCode;
                document.getElementById('customerName').value = order.customerName;
                document.getElementById('customerPhone').value = order.customerPhone;
                document.getElementById('customerEmail').value = order.customerEmail;
                document.getElementById('shippingAddress').value = order.shippingAddress;
                document.getElementById('shippingAddressLine2').value = order.shippingAddressLine2;
                document.getElementById('shipDate').value = order.shipDate.split('T')[0];
                document.getElementById('notes').value = order.notes;

                var orderHistoryBody = document.getElementById('orderHistoryBody');
                orderHistoryBody.innerHTML = '';
                order.orderHistoryVM.forEach(history => {
                    var row = document.createElement('tr');
                    var formattedEditingHistory = formatLink(history.editingHistory.replace(/\n/g, '<br>'));
                    var formattedChangeDetails = formatLink(history.changeDetails.replace(/\n/g, '<br>'));

                    row.innerHTML = `
                        <td>${formatDateTime(history.changeDate)}</td>
                        <td>${formattedEditingHistory}</td>
                        <td>${formattedChangeDetails}</td>
                    `;
                    orderHistoryBody.appendChild(row);
                });

                var orderDetailsBody = document.getElementById('orderDetailsBody');
                orderDetailsBody.innerHTML = '';
                var count = 1;
                order.orderDetailsVM.forEach(detail => {
                    var row = document.createElement('tr');
                    row.classList.add('order-detail-row');
                    row.setAttribute('data-id-options', detail.idOptions);

                    row.innerHTML = `
                        <td onclick="viewDetails('${detail.idOptions}')">(${count}) ${detail.productName || 'N/A'}</td>
                        <td>${detail.sizeName || 'N/A'}</td>
                        <td>${detail.colorName || 'N/A'}</td>
                        <td><input id="quantity" value="${detail.quantity}"></input></td>
                        <td>${formatCurrency(detail.unitPrice)}</td>
                        <td>${formatCurrency(detail.totalAmount)}</td>
                    `;
                    orderDetailsBody.appendChild(row);
                    count++;
                });

            } else if (xhr.status === 204) {
                console.warn('No content returned. Status:', xhr.responseText);
            } else {
                console.error('Request failed. Status:', xhr.responseText);
            }
        }
    };

    xhr.onerror = function () {
        console.error('Request failed. Network error.');
    };

    xhr.send();
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

function formatDateTime(dateTimeString) {
    var date = new Date(dateTimeString);
    var options = {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit',
        hour: '2-digit',
        minute: '2-digit'
    };
    return date.toLocaleString('vi-VN', options);
}

function formatCurrency(amount) {
    return amount.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
}
