﻿function getJwtFromCookie() {
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

function displayData(data) {
    const tableBody = document.getElementById('table_productdetails');
    tableBody.innerHTML = '';

    data.forEach(productdetails => {
        const formattedSmallestPrice = productdetails.smallestPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
        const formattedBiggestPrice = productdetails.biggestPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });

        let priceToShow = '';
        if (formattedSmallestPrice !== formattedBiggestPrice) {
            priceToShow = `Từ ${formattedSmallestPrice} đến ${formattedBiggestPrice}`;
        } else {
            priceToShow = formattedSmallestPrice;
        }

        const row = document.createElement('tr');
        row.innerHTML = `
            <td width="10"><input type="checkbox" name="check1" value="${productdetails.id}"></td>
            <td>${productdetails.keyCode}</td>
            <td>${productdetails.productName}</td>
            <td>
                <img src="${productdetails.imagePaths[0]}" alt="Ảnh sản phẩm" width="60px;">
            </td>
            <td>${productdetails.totalQuantity}</td>
            <td>
                ${productdetails.totalQuantity >= 1 ? '<span class="badge bg-success">Còn hàng</span>' : '<span class="badge bg-danger">Hết hàng</span>'}
            </td>
            <td>${priceToShow}</td>
            <td>${productdetails.categoryName}</td>
            <td>${productdetails.isActive == true ? '<span class="badge bg-success">Đang bán</span>' : '<span class="badge bg-danger">Đã ngừng bán</span>'}</td>

            <td>
                <button class="btn btn-primary btn-sm trash" type="button" title="${productdetails.isActive ? 'Ẩn bán' : 'Hiện bán'}" onclick="confirmDelete('${productdetails.id}', ${productdetails.isActive})">
                    <i class="${productdetails.isActive ? 'fas fa-ban' : 'fas fa-check-circle text-success'}"></i>
                </button>

                <button class="btn btn-primary btn-sm edit" type="button" title="Sửa" onclick="navigateToUpdatePage('${productdetails.id}')">
                    <i class="fas fa-edit"></i>
                </button>
                <button class="btn btn-primary btn-sm view" type="button" title="Xem chi tiết" onclick="viewDetails('${productdetails.id}')">
                    <i class="fas fa-eye"></i>
                </button>
            </td>
        `;
        tableBody.appendChild(row);
    });

    if ($.fn.dataTable.isDataTable('#sampleTable')) {
        $('#sampleTable').DataTable().destroy();
    }
    $('#sampleTable').DataTable({
        columnDefs: [
            { orderable: false, targets: [0, 8] }
        ]
    });
}
function fetchData() {
    var xhr = new XMLHttpRequest();
    xhr.open('GET', 'https://localhost:7241/api/ProductDetails/GetAll', true);
    xhr.setRequestHeader("Authorization", "Bearer " + jwt);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4) {
            if (xhr.status == 200) {
                var data = JSON.parse(xhr.responseText);
                displayData(data);
            } else {
                console.error('Error fetching data:', xhr.statusText);
            }
        }
    };
    xhr.send();
}
function viewDetails(productID) {
    var xhr = new XMLHttpRequest();
    xhr.open('GET', `https://localhost:7241/api/ProductDetails/GetByIDAsyncVer_1/${productID}`, true);
    xhr.setRequestHeader("Authorization", "Bearer " + jwt);

    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
                var data = JSON.parse(xhr.responseText);

                document.getElementById('modalKeyCode').textContent = data.keyCode;
                document.getElementById('modalProductName').textContent = data.productName;
                document.getElementById('modalManufacturers').textContent = data.manufacturersName;
                document.getElementById('modalMaterial').textContent = data.materialName;
                document.getElementById('modalBrand').textContent = data.brandName;
                document.getElementById('modalCategory').textContent = data.categoryName;
                document.getElementById('modalDescription').textContent = data.description;
                document.getElementById('modalStyle').textContent = data.style;
                document.getElementById('modalOrigin').textContent = data.origin;
                document.getElementById('modalTotalQuantity').textContent = data.totalQuantity;
                document.getElementById('modalStatus').textContent = data.totalQuantity >= 1 ? 'Còn hàng' : 'Hết hàng';

                var carouselIndicators = document.querySelector('.carousel-indicators');
                var carouselInner = document.querySelector('.carousel-inner');

                if (carouselIndicators && carouselInner) {
                    carouselIndicators.innerHTML = '';
                    carouselInner.innerHTML = '';

                    data.imagePaths.forEach(function (path, index) {
                        var indicator = document.createElement('li');
                        indicator.setAttribute('data-target', '#carouselExampleIndicators');
                        indicator.setAttribute('data-slide-to', index);
                        if (index === 0) indicator.classList.add('active');
                        carouselIndicators.appendChild(indicator);

                        var carouselItem = document.createElement('div');
                        carouselItem.classList.add('carousel-item');
                        if (index === 0) carouselItem.classList.add('active');
                        var img = document.createElement('img');
                        img.classList.add('d-block', 'w-100');
                        img.src = path;
                        img.alt = 'Hình ảnh sản phẩm';
                        carouselItem.appendChild(img);
                        carouselInner.appendChild(carouselItem);
                    });
                } else {
                    console.error("Carousel elements not found.");
                }

                var productDetailsBody = document.getElementById('productDetailsBody');
                if (productDetailsBody) {
                    productDetailsBody.innerHTML = '';
                    data.options.forEach(option => {
                        var row = `
                            <tr>
                                <td><img src="${option.imageURL}" alt="Ảnh" width="50px" height="50px"></td>
                                <td>${option.sizesName}</td>
                                <td>${option.colorName}</td>
                                <td>${option.stockQuantity} (<span style="color: ${option.isActive === false ? 'red' : 'green'};">${option.isActive === false ? 'Ngừng bán' : 'Đang bán'}</span>)</td>
                                <td>${option.retailPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' })}</td>
                            </tr>
                        `;
                        productDetailsBody.insertAdjacentHTML('beforeend', row);
                    });
                } else {
                    console.error("productDetailsBody element not found.");
                }

                $('#productModal').modal('show');
            } else {
                console.error('Error fetching product details by Id:', xhr.status);
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi!',
                    text: 'Đã xảy ra lỗi khi lấy dữ liệu sản phẩm. Vui lòng thử lại sau.'
                });
            }
        }
    };
    xhr.send();
}
function navigateToUpdatePage(productId) {
    window.location.href = `/managerupdate_productdetails_ver1/${productId}`;
}
function confirmDelete(productID, isActive) {
    Swal.fire({
        title: 'Bạn có chắc chắn muốn thay đổi trạng thái sản phẩm này?',
        text: "Hành động này sẽ thay đổi trạng thái của sản phẩm!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Có',
        cancelButtonText: 'Hủy bỏ'
    }).then((result) => {
        if (result.isConfirmed) {
            toggleProductStatus(productID, isActive);
        }
    });
}
function toggleProductStatus(productID, isActive) {
    var newIsActive = !isActive;

    var productInfo = {
        idEntity: productID,
        isActive: newIsActive
    };

    var xhr = new XMLHttpRequest();
    xhr.open('POST', `https://localhost:7241/api/ProductDetails/UpdateIsActive`, true);
    xhr.setRequestHeader('Content-Type', 'application/json');
    xhr.setRequestHeader("Authorization", "Bearer " + jwt);

    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
                Swal.fire({
                    icon: 'success',
                    title: 'Thành công!',
                    text: 'Đã cập nhật trạng thái sản phẩm thành công.',
                    timer: 3000,
                    timerProgressBar: true,
                    willClose: () => {
                        window.location.reload();
                    }
                });

                var actionText = updatedIsActive ? 'Ẩn bán' : 'Hiện bán';
                var iconClass = updatedIsActive ? 'fas fa-ban' : 'fas fa-check-circle text-success';
                var button = document.querySelector(`button.trash[data-productid="${productID}"]`);
                if (button) {
                    button.title = actionText;
                    button.innerHTML = `<i class="${iconClass}"></i>`;
                }
            }
            else {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi!',
                    text: 'Đã xảy ra lỗi khi cập nhật trạng thái sản phẩm. Vui lòng thử lại sau.'
                });
            }
        }
    };

    xhr.send(JSON.stringify(productInfo));
}
document.addEventListener('DOMContentLoaded', fetchData);
document.addEventListener('DOMContentLoaded', function () {
    var toggleButton = document.getElementById('toggleDescription');
    var descriptionContainer = document.getElementById('modalDescriptionContainer');

    toggleButton.addEventListener('click', function () {
        if (descriptionContainer.style.maxHeight === '100px') {
            descriptionContainer.style.maxHeight = '500px';
            toggleButton.textContent = 'Thu gọn';
        } else {
            descriptionContainer.style.maxHeight = '100px';
            toggleButton.textContent = 'Xem thêm';
        }
    });
});
