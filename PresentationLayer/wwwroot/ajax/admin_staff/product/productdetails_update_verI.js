
let previousImageURLs = {}
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
function checkAuthentication() {
    if (!jwt || !userId) {
        window.location.href = '/login';
        return false;
    }
    return true;
}
checkAuthentication();
function checkAuthentication() {
    if (!jwt || !userId) {
        window.location.href = '/login';
        return false;
    }
    return true;
}
checkAuthentication();

document.addEventListener('DOMContentLoaded', function () {
    var currentUrl = window.location.href;
    var urlParts = currentUrl.split('/');
    var ID = urlParts[urlParts.length - 1];

    const btnSuccessUpdate = document.getElementById('btn_success_update');
    btnSuccessUpdate.addEventListener('click', function (event) {
        event.preventDefault();

        Swal.fire({
            title: 'Xác nhận lưu',
            text: 'Bạn có chắc chắn muốn lưu các thay đổi này?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Lưu',
            cancelButtonText: 'Hủy'
        }).then((result) => {
            if (result.isConfirmed) {
                var productData = {
                    ModifiedBy: userId,
                    ProductName: document.getElementById('product_name').value,
                    CategoryName: document.getElementById('category_name').value,
                    ManufacturersName: document.getElementById('manufacture_name').value,
                    MaterialName: document.getElementById('material_name').value,
                    BrandName: document.getElementById('brand_name').value,
                    Style: document.getElementById('Style').value,
                    Origin: document.getElementById('Origin').value,
                    Description: document.getElementById('Description').value,
                    ImagePaths: [],
                    NewOptions: gatherOptionsData(ID)
                };

                const imageUpload = document.getElementById('ImagePaths');
                productData.ImagePaths = getImagePaths(imageUpload);
                console.log('imageUpload', imageUpload)
                console.log('productData.ImagePaths', productData.ImagePaths)

                function getImagePaths(input) {
                    const files = input.files;
                    const imagePaths = [];

                    for (let i = 0; i < files.length; i++) {
                        const file = files[i];
                        const objectUrl = URL.createObjectURL(file);
                        imagePaths.push(objectUrl);
                    }

                    return imagePaths;
                }

                Swal.fire({
                    title: 'Đang xử lý',
                    text: 'Vui lòng chờ trong khi lưu sản phẩm...',
                    allowOutsideClick: false,
                    didOpen: () => {
                        Swal.showLoading();
                    }
                });

                updateProduct(ID, productData).then(() => {
                    return uploadImages(ID, imageUpload.files);
                }).then(uploadedImageUrls => {
                    console.log('Ảnh đã tải lên:', uploadedImageUrls);
                    Swal.fire({
                        icon: 'success',
                        title: 'Thành công',
                        text: 'Sản phẩm và ảnh đã được cập nhật thành công!'
                    }).then(() => {
                        window.location.href = '/home/index_productdetails';
                    });
                    console.log(productData, imageUpload);

                }).catch(error => {
                    console.error('Đã xảy ra lỗi:', error);
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi',
                        text: `Đã xảy ra lỗi trong quá trình cập nhật sản phẩm hoặc tải ảnh: ${error}`
                    });
                    console.log(productData, imageUpload);

                });
            }
        });
    });

    const addColorButton = document.getElementById('addColorButton');
    addColorButton.addEventListener('click', function (event) {
        event.preventDefault();
        addColor();
    });

    const addSizeButton = document.getElementById('addSizeButton');
    addSizeButton.addEventListener('click', function (event) {
        event.preventDefault();
        addSize();
    });

    if (ID) {
        getProductDetailsById(ID);
    } else {
        alert('Không có ID được cung cấp.');
    }
});
let colors = [];
let sizes = [];
function addColor() {
    const color = document.getElementById('color').value;
    if (color && !colors.includes(color)) {
        colors.push(color);
        updateColors();
        updateTable();
        document.getElementById('color').value = '';
    } else {
        Swal.fire({
            icon: 'error',
            title: 'Lỗi!',
            text: 'Vui lòng nhập màu hợp lệ hoặc màu đã tồn tại.'
        });
    }
}
function addSize() {
    const size = document.getElementById('size').value;
    if (size && !sizes.includes(size)) {
        sizes.push(size);
        updateSizes();
        updateTable();
        document.getElementById('size').value = '';
    } else {
        Swal.fire({
            icon: 'error',
            title: 'Lỗi!',
            text: 'Vui lòng nhập size hợp lệ hoặc size đã tồn tại.'
        });
    }
}
function updateColors() {
    const addedColors = document.getElementById('addedColors');
    addedColors.innerHTML = '';
    colors.forEach(color => {
        const span = document.createElement('span');
        span.textContent = color;
        addedColors.appendChild(span);
    });
}
function updateSizes() {
    const addedSizes = document.getElementById('addedSizes');
    addedSizes.innerHTML = '';
    sizes.forEach(size => {
        const span = document.createElement('span');
        span.textContent = size;
        addedSizes.appendChild(span);
    });
}
function updateTable() {
    const tableBody = document.getElementById('classificationBody');
    tableBody.innerHTML = '';

    if (colors.length === 0 || sizes.length === 0) {
        const row = document.createElement('tr');
        const cell = document.createElement('td');
        cell.colSpan = 7;
        cell.textContent = 'Không có dữ liệu';
        cell.className = 'no-data';
        row.appendChild(cell);
        tableBody.appendChild(row);
        return;
    }

    colors.forEach(color => {
        sizes.forEach(size => {
            const match = dataMap.find(item => item.color === color && item.size === size);
            const row = document.createElement('tr');
            const imagePreviewId = `imagePreview-${color}-${size}`;
            row.innerHTML = `
               <td class="image-preview-options" id="${imagePreviewId}">
                    <div class="image-container">
                        <img id="${imagePreviewId}_img" src="${match ? match.imageURL : ''}" alt="Image Preview" style="width: 40px; height: 40px; object-fit: cover; display: ${match ? 'block' : 'none'};">
                        <button id="${imagePreviewId}_button" type="button" style="display: ${match};">Thêm ảnh</button>
                    </div>
                    <input type="file" id="${imagePreviewId}_fileInput" style="display: none;" accept="image/*">
                </td>

                <td>${color}</td>
                <td>${size}</td>
                <td><input type="text" id="retal_price" value="${match ? match.retailPrice : ''}" placeholder="Nhập giá bán"></td>
                <td><input type="text"  id="quantity" value="${match ? match.stockQuantity : ''}" placeholder="0"></td>
             <td>
                ${match ? `
                    <button type="button" class="toggle-button btn ${match.isActive ? 'btn-danger' : 'btn-success'}" data-option-id="${match.id}" data-isactive="${match.isActive}" onclick="toggleStatus('${match.id}', ${match.isActive})">
                        ${match.isActive ? 'Ngừng hoạt động' : 'Hoạt động'}
                    </button>
                ` : `
                    <button class="remove-button btn btn-danger" onclick="removeRow(this)">Xóa</button>
                `}
            </td>
            `;
            tableBody.appendChild(row);

            document.getElementById(`${imagePreviewId}_button`).addEventListener('click', function () {
                document.getElementById(`${imagePreviewId}_fileInput`).click();
            });

            const retal_priceInput = document.getElementById('retal_price');
            retal_priceInput.addEventListener('blur', function () {
                validatePrice(retal_priceInput);
            });
            const soLuongInput = document.getElementById('quantity');
            soLuongInput.addEventListener('blur', function () {
                validateQuantity(soLuongInput);
            });
            document.getElementById(`${imagePreviewId}_fileInput`).addEventListener('change', function (event) {
                const input = event.target;
                const imgElement = document.getElementById(`${imagePreviewId}_img`);
                if (input.files && input.files[0]) {
                    const reader = new FileReader();
                    reader.onload = function (e) {
                        imgElement.src = e.target.result;
                        imgElement.style.display = 'block';
                        document.getElementById(`${imagePreviewId}_button`).style.display = 'none';

                        if (match) {
                            match.imageURL = e.target.result;
                            match.isImageChanged = true;
                            console.log(`Hình ảnh của phân loại ${color} - ${size} đã thay đổi`, match.isImageChanged); 
                        }
                    };
                    reader.readAsDataURL(input.files[0]);
                }
            });
        });
    });
}
function removeRow(button) {
    const row = button.parentElement.parentElement;
    const color = row.cells[1].textContent;
    const size = row.cells[2].textContent;

    row.remove();

    const rows = document.querySelectorAll('#classificationBody tr');
    let colorExists = false;
    let sizeExists = false;

    rows.forEach(row => {
        if (row.cells[1].textContent === color) {
            colorExists = true;
        }
        if (row.cells[2].textContent === size) {
            sizeExists = true;
        }
    });

    if (!colorExists) {
        colors = colors.filter(c => c !== color);
        updateColors();
    }

    if (!sizeExists) {
        sizes = sizes.filter(s => s !== size);
        updateSizes();
    }

    if (rows.length === 0) {
        const tableBody = document.getElementById('classificationBody');
        const row = document.createElement('tr');
        const cell = document.createElement('td');
        cell.colSpan = 7;
        cell.textContent = 'Không có dữ liệu';
        cell.className = 'no-data';
        row.appendChild(cell);
        tableBody.appendChild(row);
    }
}
function getProductDetailsById(ID) {
    var xhr = new XMLHttpRequest();
    xhr.open('GET', `https://localhost:7241/api/ProductDetails/GetByIDAsyncVer_1/${ID}`, true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
                var data = JSON.parse(xhr.responseText);
                document.getElementById('keycode').value = data.keyCode;
                document.getElementById('product_name').value = data.productName;
                document.getElementById('category_name').value = data.categoryName;
                document.getElementById('manufacture_name').value = data.manufacturersName;
                document.getElementById('material_name').value = data.materialName;
                document.getElementById('brand_name').value = data.brandName;
                document.getElementById('Style').value = data.style;
                document.getElementById('Origin').value = data.origin;
                document.getElementById('Description').value = data.description;
                colors = Array.from(new Set((data.options || []).map(option => option.colorName)));
                sizes = Array.from(new Set((data.options || []).map(option => option.sizesName)));
                dataMap = (data.options || []).map(option => ({
                    id: option.id,
                    imageURL: option.imageURL,
                    color: option.colorName,
                    size: option.sizesName,
                    retailPrice: option.retailPrice,
                    isActive: option.isActive,
                    stockQuantity: option.stockQuantity
                }));
                updateProductImages(data.imageVM);
                updateColors();
                updateSizes();
                updateTable();
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi cập nhật',
                    text: `Đã xảy ra lỗi khi lấy dữ liệu sản phẩm. Vui lòng thử lại sau. ${xhr.responseText}`,
                    confirmButtonText: 'OK'
                });
            }
        }
    };
    xhr.send();
}
document.body.addEventListener('click', function (event) {
    if (event.target.classList.contains('remove-button')) {
        event.preventDefault();
        var optionId = event.target.getAttribute('data-option-id');
        removeRow(optionId);
    }
});
function toggleStatus(id, currentStatus) {
    const newStatus = !currentStatus;

    Swal.fire({
        title: 'Xác nhận thay đổi trạng thái',
        text: `Bạn muốn ${newStatus ? 'kích hoạt' : 'vô hiệu hóa'} tùy chọn này?`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Có, thay đổi!',
        cancelButtonText: 'Hủy'
    }).then((result) => {
        if (result.isConfirmed) {
            var data = {
                idEntity: id,
                isActive: newStatus
            };

            var xhr = new XMLHttpRequest();
            xhr.open("POST", "https://localhost:7241/api/Options/UpdateIsActive", true);
            xhr.setRequestHeader("Content-Type", "application/json");

            xhr.onload = function () {
                if (xhr.status === 200) {
                    const button = document.querySelector(`button[data-option-id='${id}']`);

                    if (button) {
                        if (newStatus) {
                            button.classList.remove('btn-success');
                            button.classList.add('btn-danger');
                            button.textContent = 'Ngừng hoạt động';
                        } else {
                            button.classList.remove('btn-danger');
                            button.classList.add('btn-success');
                            button.textContent = 'Hoạt động';
                        }

                        button.setAttribute('data-isactive', newStatus);
                    } else {
                        console.error(`Nút với id '${id}' không tìm thấy.`);
                    }

                    Swal.fire({
                        icon: 'success',
                        title: 'Thành công',
                        text: 'Đã cập nhật trạng thái thành công.'
                    }).then(() => {
                        console.log("Đã cập nhật thành công trạng thái isActive có id: " + id);
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi',
                        text: 'Đã xảy ra lỗi khi cập nhật trạng thái.'
                    });
                }
            };

            xhr.onerror = function () {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi kết nối',
                    text: 'Đã xảy ra lỗi kết nối khi gửi yêu cầu.'
                });
                console.error("Đã xảy ra lỗi kết nối khi gửi yêu cầu cập nhật trạng thái isActive có id: " + id);
            };

            xhr.send(JSON.stringify(data));
        }
    });
}
document.addEventListener("DOMContentLoaded", function () {
    function loadData(url, selectId, defaultText) {
        var xhr = new XMLHttpRequest();
        xhr.open("GET", url, true);
        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4 && xhr.status === 200) {
                var data = JSON.parse(xhr.responseText);
                var selectElement = document.getElementById(selectId);
                selectElement.innerHTML = '';

                var defaultOption = document.createElement('option');
                defaultOption.text = defaultText;
                selectElement.add(defaultOption);

                data.forEach(function (item) {
                    var option = document.createElement('option');
                    option.value = item.id;
                    option.text = item.name;
                    option.setAttribute('data-name', item.name);
                    selectElement.add(option);
                });
            } else if (xhr.readyState === 4) {
                console.error('Error:', xhr.statusText);
            }
        };
        xhr.send();
    }
    function addChangeListener(selectId, inputId) {
        var selectElement = document.getElementById(selectId);
        var inputElement = document.getElementById(inputId);

        selectElement.addEventListener('change', function () {
            var selectedOption = this.options[this.selectedIndex];
            if (selectedOption.value) {
                inputElement.value = selectedOption.getAttribute('data-name');
            } else {
                inputElement.value = '';
            }
        });
    }
    loadData("https://localhost:7241/api/Colors/GetAllActive", 'colorSelect', '-- Chọn màu --');
    addChangeListener('colorSelect', 'color');
    loadData("https://localhost:7241/api/Sizes/GetAllActive", 'sizeSelect', '-- Chọn size --');
    addChangeListener('sizeSelect', 'size');
    loadData("https://localhost:7241/api/Material/GetAllActive", 'product_material', '-- Chọn chất liệu --');
    addChangeListener('product_material', 'material_name');
    loadData("https://localhost:7241/api/Brand/GetAllActive", 'product_brand', '-- Chọn thương hiệu --');
    addChangeListener('product_brand', 'brand_name');
    loadData("https://localhost:7241/api/Manufacturer/GetAllActive", 'product_manufacture', '-- Chọn nhà sản xuất --');
    addChangeListener('product_manufacture', 'manufacture_name');
    loadData("https://localhost:7241/api/Category/GetAllActive", 'product_category', '-- Chọn danh mục --');
    addChangeListener('product_category', 'category_name');
});
function updateProductImages(images) {
    const imagePreview = document.getElementById('imagePreview');
    imagePreview.innerHTML = '';

    function openImageModal(imagePath) {
        const modalImage = document.getElementById('modalImage');
        modalImage.src = imagePath;
        const imageModal = new bootstrap.Modal(document.getElementById('imageModal'));
        imageModal.show();
    }

    function deleteImage(imageID, container) {
        event.preventDefault(); 
        Swal.fire({
            title: 'Bạn có chắc chắn muốn xóa?',
            text: "Hành động này không thể hoàn tác!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Vâng, xóa nó!'
        }).then((result) => {
            if (result.isConfirmed) {
                const xhr = new XMLHttpRequest();
                xhr.open("DELETE", `https://localhost:7241/api/images/${imageID}`, true);
                xhr.setRequestHeader('accept', '*/*');
                xhr.onreadystatechange = function () {
                    if (xhr.readyState === 4) {
                        if (xhr.status === 200) {
                            container.remove();
                            Swal.fire(
                                'Đã xóa!',
                                'Hình ảnh đã được xóa.',
                                'success'
                            );
                        } else {
                            Swal.fire(
                                'Thất bại!',
                                'Không thể xóa hình ảnh.',
                                'error'
                            );
                        }
                    }
                };
                xhr.send();
            }
        });
    }

    function createImageElement(image) {
        const container = document.createElement('div');
        container.style.position = 'relative';
        container.style.display = 'inline-block';
        container.style.margin = '5px';

        const img = document.createElement('img');
        img.src = image.path;
        img.alt = 'Ảnh sản phẩm';
        img.style.maxWidth = '150px';
        img.style.border = '1px solid #ddd';
        img.style.borderRadius = '5px';
        img.style.cursor = 'pointer';

        const removeButton = document.createElement('button');
        removeButton.style.position = 'absolute';
        removeButton.style.top = '5px';
        removeButton.style.right = '5px';
        removeButton.style.backgroundColor = 'red';
        removeButton.style.color = 'white';
        removeButton.style.border = 'none';
        removeButton.style.borderRadius = '50%';
        removeButton.style.width = '25px';
        removeButton.style.height = '25px';
        removeButton.style.textAlign = 'center';
        removeButton.style.lineHeight = '25px';
        removeButton.style.cursor = 'pointer';
        removeButton.innerText = 'X';

        removeButton.onclick = function () {
            deleteImage(image.id, container);
        };

        img.onclick = function () {
            openImageModal(image.path);
        };

        container.appendChild(img);
        container.appendChild(removeButton);
        imagePreview.appendChild(container);
    }

    if (Array.isArray(images)) {
        images.forEach(image => {
            if (image.path) {
                createImageElement(image);
            }
        });
    } else if (images && images.imagesVM && Array.isArray(images.imagesVM)) {
        images.imagesVM.forEach(image => {
            if (image.path) {
                createImageElement(image);
            }
        });
    } else if (images && images.path) {
        createImageElement(images);
    } else {
        console.warn('Dữ liệu hình ảnh không hợp lệ.');
    }
}
function openImageModal(imagePath) {
    const modalImage = document.getElementById('modalImage');
    modalImage.src = imagePath;

    const modal = new bootstrap.Modal(document.getElementById('imageModal'));
    modal.show();
}
document.getElementById('ImagePaths').addEventListener('change', function (event) {
    const imagePreview = document.getElementById('imagePreview');

    const files = event.target.files;
    if (files) {
        Array.from(files).forEach(file => {
            const reader = new FileReader();
            reader.onload = function (e) {
                const container = document.createElement('div');
                container.style.position = 'relative';
                container.style.display = 'inline-block';
                container.style.margin = '5px';

                const img = document.createElement('img');
                img.src = e.target.result;
                img.alt = 'Ảnh sản phẩm';
                img.style.width = '150px';   // Đặt chiều rộng cố định
                img.style.height = '194px';  // Đặt chiều cao cố định
                img.style.objectFit = 'cover'; // Cắt ảnh theo kích thước
                img.style.border = '1px solid #ddd';
                img.style.borderRadius = '5px';
                img.style.cursor = 'pointer';

                const removeButton = document.createElement('button');
                removeButton.style.position = 'absolute';
                removeButton.style.top = '5px';
                removeButton.style.right = '5px';
                removeButton.style.backgroundColor = 'red';
                removeButton.style.color = 'white';
                removeButton.style.border = 'none';
                removeButton.style.borderRadius = '50%';
                removeButton.style.width = '25px';
                removeButton.style.height = '25px';
                removeButton.style.textAlign = 'center';
                removeButton.style.lineHeight = '25px';
                removeButton.style.cursor = 'pointer';
                removeButton.innerText = 'X';

                removeButton.onclick = function () {
                    container.remove(); // Xóa ảnh khỏi phần xem trước
                };

                img.onclick = function () {
                    openImageModal(e.target.result);
                };

                container.appendChild(img);
                container.appendChild(removeButton);
                imagePreview.appendChild(container);
            };
            reader.readAsDataURL(file);
        });
    }
});
async function uploadImages(IDProductDetails, product_images) {
    if (!product_images || product_images.length === 0) {
        console.error('Không có tệp nào để tải lên.');
        return [];
    }

    const maxFileSize = 2 * 1024 * 1024;
    const formData = new FormData();
    for (let i = 0; i < product_images.length; i++) {
        const file = product_images[i];
        if (file.size > maxFileSize) {
            Swal.fire({
                icon: 'error',
                title: 'Tệp quá lớn',
                text: `Kích thước tệp ${file.name} vượt quá giới hạn 2MB.`
            });
            return;
        }
        formData.append('Path', file);
    }

    formData.append('IDProductDetails', IDProductDetails);
    formData.append('CreateBy', userId);
    formData.append('Status', 1);

    return new Promise((resolve, reject) => {
        var xhr = new XMLHttpRequest();
        xhr.open('POST', 'https://localhost:7241/api/images/upload_images', true);
        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4) {
                if (xhr.status === 200) {
                    console.log('Ảnh đã được tải lên thành công');
                    try {
                        var response = JSON.parse(xhr.responseText);
                        console.log('Phản hồi từ API:', response);
                        var imagePaths = response.uploadedImageUrls || [];
                        resolve(imagePaths);
                    } catch (e) {
                        console.error('Lỗi khi phân tích phản hồi API:', e);
                        reject('Lỗi khi phân tích phản hồi API');
                    }
                } else {
                    reject(xhr.responseText || 'Lỗi khi tải lên ảnh');
                }
            }
        };
        xhr.onerror = function () {
            reject('Lỗi khi gửi yêu cầu tải lên ảnh');
        };
        xhr.send(formData);
    });
}
async function uploadImageSingle(imageSrc) {
    if (!imageSrc) {
        console.error('Không có tệp nào để tải lên.');
        return '';
    }

    const formData = new FormData();
    const blob = await fetch(imageSrc).then(r => r.blob());
    formData.append('imageFile', blob, 'image.jpg');

    return new Promise((resolve, reject) => {
        var xhr = new XMLHttpRequest();
        xhr.open('POST', 'https://localhost:7241/api/images/upload_single', true);
        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4) {
                if (xhr.status === 200) {
                    console.log('Ảnh đã được tải lên thành công');
                    var response = JSON.parse(xhr.responseText);
                    var imagePath = response.imageUrl;
                    resolve(imagePath);
                } else {
                    reject(xhr.responseText || 'Lỗi khi tải lên ảnh');
                }
            }
        };
        xhr.onerror = function () {
            reject('Lỗi khi gửi yêu cầu tải lên ảnh');
        };
        xhr.send(formData);
    });
}
async function gatherOptionsData(ID) {
    var options = [];

    var newTableBody = document.getElementById('classificationBody');
    var newTableRows = newTableBody.getElementsByTagName('tr');

    if (newTableRows.length === 1 && newTableRows[0].classList.contains('no-data')) {
        return options;
    }

    for (var j = 0; j < newTableRows.length; j++) {
        var newRow = newTableRows[j];
        var cells = newRow.getElementsByTagName('td');

        if (cells.length < 5) {
            continue;
        }

        const color = cells[1].textContent.trim();
        const size = cells[2].textContent.trim();
        const giaBan = cells[3].querySelector('input').value;
        const soLuong = cells[4].querySelector('input').value;
        const imageElement = cells[0].querySelector('img');
        const imageSrc = imageElement ? imageElement.src : '';

        const uploadedImageUrl = await uploadImageSingle(imageSrc);
        var newOption = {
            IDProductDetails: ID,
            ColorName: color,
            SizesName: size,
            RetailPrice: giaBan,
            StockQuantity: parseInt(soLuong.replace(/[,.đ]/g, ''), 10),
            CreateBy: userId,
            IsActive: true,
            ImageURL: uploadedImageUrl || '',
            Status: 1,
        };
        options.push(newOption);
    }

    return options;
}
async function updateProduct(ID, productData) {
    try {
        const resolvedProductData = {
            ...productData,
            NewOptions: await productData.NewOptions
        };

        const response = await new Promise((resolve, reject) => {
            var xhr = new XMLHttpRequest();
            xhr.open('PUT', `https://localhost:7241/api/ProductDetails/Update/${ID}`, true);
            xhr.setRequestHeader('Content-Type', 'application/json');
            xhr.setRequestHeader('Authorization', 'Bearer ' + jwt);
            xhr.onreadystatechange = function () {
                if (xhr.readyState === 4) {
                    if (xhr.status === 200) {
                        const response = JSON.parse(xhr.responseText);
                        console.log('Đã cập nhật sản phẩm thành công:', response);
                        resolve(response);
                    } else {
                        reject(xhr.responseText || 'Lỗi khi cập nhật sản phẩm');
                    }
                }
            };
            xhr.onerror = function () {
                reject('Lỗi khi gửi yêu cầu cập nhật sản phẩm');
            };
            xhr.send(JSON.stringify(resolvedProductData));
        });

        return response;
    } catch (error) {
        console.error('Đã xảy ra lỗi:', error);
        throw error;
    }
}
document.addEventListener('DOMContentLoaded', function () {
    const imagePreviewId = '${imagePreviewId}';
    const imgElement = document.getElementById(`${imagePreviewId}_img`);
    const buttonElement = document.getElementById(`${imagePreviewId}_button`);
    const fileInputElement = document.getElementById(`${imagePreviewId}_fileInput`);
    buttonElement.addEventListener('click', function (event) {
        event.preventDefault(); 
        fileInputElement.click();
    });

    fileInputElement.addEventListener('change', function (event) {
        const file = event.target.files[0];
        if (file) {
            uploadImage(file).then(imageURL => {
                imgElement.src = imageURL;
                imgElement.style.display = 'block';
                buttonElement.style.display = 'none';
            }).catch(error => {
                console.error('Lỗi khi tải ảnh lên:', error);
            });
        }
    });

    function uploadImage(file) {
        return new Promise((resolve, reject) => {
            const reader = new FileReader();
            reader.onload = function (event) {
                resolve(event.target.result); 
            };
            reader.onerror = function () {
                reject('Lỗi khi đọc tệp ảnh');
            };
            reader.readAsDataURL(file);
        });
    }
});
function validateQuantity(input) {
    const value = input.value.trim();
    const isValid = /^[0-9]*$/.test(value);

    if (!isValid || parseInt(value) < 0) {
        input.style.borderColor = 'red';
        Swal.fire({
            icon: 'error',
            title: 'Lỗi!',
            text: 'Số lượng không được nhập chữ hoặc số âm.',
        });
        input.value = '';
    }
    else {
        input.style.borderColor = '';
    }
}
function validatePrice(input) {
    const value = input.value.trim();
    if (!/^\d+(\.\d{1,2})?$/.test(value) || parseFloat(value) < 0) {
        input.style.borderColor = 'red';
        Swal.fire({
            icon: 'error',
            title: 'Lỗi',
            text: 'Giá bán không hợp lệ. Vui lòng nhập giá hợp lệ (số dương, không chứa ký tự đặc biệt).'
        });
        input.value = '';
    } else {
        input.style.borderColor = '';
    }
}
