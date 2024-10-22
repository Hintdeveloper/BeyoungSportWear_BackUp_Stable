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

function removeDiacritics(str) {
    return str.normalize("NFD").replace(/[\u0300-\u036f]/g, "").normalize("NFC");
}

function getInitials(str) {
    return str.split(' ').map(word => word.charAt(0)).join('').toUpperCase();
}

function generateKeyCode(productName, categoryName) {
    let normalizedProductName = removeDiacritics(productName);
    let normalizedCategoryName = removeDiacritics(categoryName);
    let keyPart = getInitials(normalizedProductName + " " + normalizedCategoryName);
    let randomPart = Math.floor(Math.random() * 9000) + 1000;
    return `${keyPart}-${randomPart}`.toUpperCase();
}

document.addEventListener('DOMContentLoaded', function () {
    const productNameInput = document.getElementById('product_name');
    const categoryNameInput = document.getElementById('category_name');
    const keyCodeInput = document.getElementById('keycode');

    function updateKeyCode() {
        const productName = productNameInput.value;
        const categoryName = categoryNameInput.value;
        const keyCode = generateKeyCode(productName, categoryName);
        keyCodeInput.value = keyCode;
    }

    productNameInput.addEventListener('input', updateKeyCode);
    categoryNameInput.addEventListener('input', updateKeyCode);
});

document.addEventListener('DOMContentLoaded', function () {
    const productForm = document.getElementById('productForm');
    productForm.addEventListener('submit', function (event) {
        event.preventDefault();
    });

    const btnSave = document.getElementById('btn_saveproductdetails');
    btnSave.addEventListener('click', async function () {
        var product_product = document.getElementById("product_name").value;
        var keycode = document.getElementById("keycode").value;
        var product_category = document.getElementById("category_name").value;
        var product_manufacture = document.getElementById("manufacture_name").value;
        var product_material = document.getElementById("material_name").value;
        var select_brand = document.getElementById("brand_name").value;
        var product_style = document.getElementById("product_style").value;
        var product_origin = document.getElementById("product_origin").value;
        var product_description = document.getElementById("product_description").value;
        var product_images = document.getElementById("image-upload").files;
        var productId = guid();
       
        if (!product_product || !product_category || !product_manufacture || !product_material || !select_brand || !product_style || !product_origin || !product_description) {
            Swal.fire({
                icon: 'error',
                title: 'Lỗi',
                text: 'Vui lòng điền đầy đủ tất cả các trường thông tin!',
            });
            return;
        }

        Swal.fire({
            title: 'Xác nhận',
            text: 'Bạn có chắc chắn muốn lưu sản phẩm này?',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonText: 'Có, lưu nó!',
            cancelButtonText: 'Không, hủy!',
            reverseButtons: true
        }).then(async (result) => {
            if (result.isConfirmed) {
                try {
                    Swal.fire({
                        title: 'Đang xử lý',
                        text: 'Vui lòng chờ trong khi lưu sản phẩm...',
                        allowOutsideClick: false,
                        didOpen: () => {
                            Swal.showLoading();
                        }
                    });

                    const optionsData = await createOptionsData();

                    await saveProduct({
                        CreateBy: userId,
                        ID: productId,
                        Keycode: keycode,
                        ProductName: product_product,
                        CategoryName: product_category,
                        ManufacturersName: product_manufacture,
                        MaterialName: product_material,
                        BrandName: select_brand,
                        Style: product_style,
                        Origin: product_origin,
                        Description: product_description,
                        IsActive: true,
                        ImagePaths: [],
                        OptionsCreateVM: optionsData,
                    });

                    await uploadImages(productId, product_images);

                    Swal.fire({
                        icon: 'success',
                        title: 'Thành công',
                        text: 'Sản phẩm và ảnh đã được lưu thành công!'
                    }).then(() => {
                        window.location.href = '/home/index_productdetails';
                    });
                } catch (error) {
                    console.error('Đã xảy ra lỗi:', error);
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi',
                        text: `Đã xảy ra lỗi trong quá trình lưu sản phẩm hoặc tải ảnh: ${error}`
                    });
                }
            }
        });
    });

    async function saveProduct(productData) {
        console.log(productData)
        return new Promise((resolve, reject) => {
            var xhr = new XMLHttpRequest();
            var url = 'https://localhost:7241/api/ProductDetails/productdetails_create';
            xhr.open('POST', url, true);
            xhr.setRequestHeader('Content-Type', 'application/json');
            xhr.setRequestHeader("Authorization", "Bearer " + jwt);

            xhr.onreadystatechange = function () {
                if (xhr.readyState === 4) {
                    if (xhr.status === 200) {
                        var response = JSON.parse(xhr.responseText);
                        console.log('Đã lưu sản phẩm thành công:', response);
                        resolve(response.ID);
                    } else {
                        reject(xhr.responseText || 'Lỗi khi lưu sản phẩm');
                    }
                }
            };
            xhr.onerror = function () {
                reject('Lỗi khi gửi yêu cầu lưu sản phẩm');
            };
            xhr.send(JSON.stringify(productData));
        });
    }

    async function uploadImages(productId, product_images) {
        if (product_images.length === 0) {
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

        formData.append('IDProductDetails', productId);
        formData.append('CreateBy', userId);
        formData.append('Status', 1);

        return new Promise((resolve, reject) => {
            var xhr = new XMLHttpRequest();
            xhr.open('POST', 'https://localhost:7241/api/images/upload_images', true);
            xhr.onreadystatechange = function () {
                if (xhr.readyState === 4) {
                    if (xhr.status === 200) {
                        var response = JSON.parse(xhr.responseText);
                        var imagePaths = response.uploadedImageUrls;
                        resolve(imagePaths);
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

    async function createOptionsData() {
        const optionsData = [];
        const rows = document.getElementById('classificationBody').getElementsByTagName('tr');
        for (let i = 0; i < rows.length; i++) {
            const cells = rows[i].getElementsByTagName('td');
            if (cells.length === 6) {
                const color = cells[1].textContent.trim();
                const size = cells[2].textContent.trim();
                const giaBan = cells[3].querySelector('input').value;
                const soLuong = cells[4].querySelector('input').value;
                const imageElement = cells[0].querySelector('img');
                const imageSrc = imageElement ? imageElement.src : '';

                const uploadedImageUrl = await uploadImageSingle(imageSrc);

                const option = {
                    ID: guid(),
                    CreateBy: userId,
                    ColorName: color,
                    SizesName: size,
                    StockQuantity: parseInt(soLuong) || 0,
                    RetailPrice: parseFloat(giaBan) || 0,
                    Discount: null,
                    ImageURL: uploadedImageUrl || '',
                    IsActive: true,
                    Status: 1,
                };

                optionsData.push(option);
            }
        }
        return optionsData;
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

    const addColorButton = document.getElementById('addColorButton');
    addColorButton.addEventListener('click', function () {
        addColor();
    });

    const addSizeButton = document.getElementById('addSizeButton');
    addSizeButton.addEventListener('click', function () {
        addSize();
    });

    function guid() {
        return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0,
                v = c === 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });
    }
});
let colors = [];
let sizes = [];
function addColor() {
    const colorInput = document.getElementById('color');
    const color = colorInput.value.trim();
    if (color && !isColorExist(color)) {
        colors.push(color);
        updateColors();
        updateTable();
        document.getElementById('color').value = '';
        document.getElementById('colorSelect').value = '-- Chọn màu --';
    } else {
        Swal.fire({
            icon: 'error',
            title: 'Lỗi!',
            text: 'Vui lòng nhập màu hợp lệ hoặc màu đã tồn tại.'
        });
    }
}
function addSize() {
    const sizeInput = document.getElementById('size');
    const size = sizeInput.value.trim();

    if (size && !isSizeExist(size)) {
        sizes.push(size);
        updateSizes();
        updateTable();
        document.getElementById('size').value = '';
        document.getElementById('sizeSelect').value = '-- Chọn size --';
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
function isColorExist(color) {
    return colors.some(c => c.toLowerCase() === color.toLowerCase());
}
function isSizeExist(size) {
    return sizes.some(s => s.toLowerCase() === size.toLowerCase());
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
            const row = document.createElement('tr');
            const imagePreviewId = `imagePreview-${color}-${size}`;
            const imageUploadId = `image-upload-${color}-${size}`;
            row.innerHTML = `
                <td class="image-preview-options" id="${imagePreviewId}">
                    <input type="file" id="${imageUploadId}" class="custom-file-input">
                </td>
                <td>${color}</td>
                <td>${size}</td>
                <td><input type="text" placeholder="Nhập giá bán"></td>
                <td><input type="text" placeholder="0"></td>
                <td><button class="remove-button" onclick="removeRow(this)">Xóa</button></td>
            `;
            tableBody.appendChild(row);

            const fileInput = document.getElementById(imageUploadId);
            fileInput.addEventListener('change', function (event) {
                handleImageUpload(event, imagePreviewId);
            });

            const soLuongInput = row.querySelector('input[type="text"][placeholder="0"]');
            soLuongInput.addEventListener('blur', function () {
                validateQuantity(soLuongInput);
            });
        });
    });
}
document.addEventListener("DOMContentLoaded", function () {
    function loadData(url, selectId, defaultText) {
        var xhr = new XMLHttpRequest();
        xhr.open("GET", url, true);
        xhr.setRequestHeader("Authorization", "Bearer " + jwt);
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
function validateQuantity(inputElement) {
    const value = inputElement.value.trim();
    const isValid = /^[0-9]*$/.test(value);

    if (!isValid || parseInt(value) < 0) {
        Swal.fire({
            icon: 'error',
            title: 'Lỗi!',
            text: 'Số lượng không được nhập chữ hoặc số âm.',
        });
        inputElement.value = '';
    }
}

function validateCostPrice(giaNhapInput, giaBanInput) {
    giaNhapInput.addEventListener('blur', function () {
        const value = giaNhapInput.value.trim();
        const isValid = /^[0-9]*$/.test(value);

        if (!isValid || parseFloat(giaNhapInput.value) < 0) {
            Swal.fire({
                icon: 'error',
                title: 'Lỗi!',
                text: 'Giá nhập không được nhập chữ hoặc số âm.',
            });
            giaNhapInput.value = '';
        }
    });
}
function handleImageUpload(event, imagePreviewId) {
    const file = event.target.files[0];
    if (!file) return;

    const imagePreviewElement = document.getElementById(imagePreviewId);

    while (imagePreviewElement.firstChild) {
        imagePreviewElement.removeChild(imagePreviewElement.firstChild);
    }

    const imgElement = document.createElement('img');
    imgElement.style.maxWidth = '100%';
    imgElement.style.maxHeight = '100%';

    const reader = new FileReader();
    reader.onload = function (e) {
        imgElement.src = e.target.result;
    };
    reader.readAsDataURL(file);

    imagePreviewElement.appendChild(imgElement);
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
document.addEventListener("DOMContentLoaded", function () {
    var imageUpload = document.getElementById('image-upload');
    imageUpload.addEventListener('change', function () {
        var files = this.files;
        var maxImages = 7;
        var currentImages = document.querySelectorAll('.image-preview-item').length;

        if (currentImages + files.length > maxImages) {
            Swal.fire({
                icon: 'warning',
                title: 'Vượt quá giới hạn',
                text: 'Bạn chỉ có thể chọn tối đa 6 ảnh.',
                confirmButtonText: 'Đóng',
                allowOutsideClick: false,
                allowEscapeKey: false
            });
            return; 
        }

        for (var i = 0; i < files.length; i++) {
            var reader = new FileReader();
            reader.onload = function (e) {
                var imagePreview = document.createElement('label');
                imagePreview.className = 'image-preview-item';

                var image = document.createElement('img');
                image.src = e.target.result;
                image.alt = 'Preview';
                imagePreview.appendChild(image);

                var removeBtn = document.createElement('span');
                removeBtn.className = 'remove-image';
                removeBtn.innerHTML = '&times;';
                removeBtn.style.position = 'absolute';
                removeBtn.style.top = '5px';
                removeBtn.style.right = '5px';
                removeBtn.style.cursor = 'pointer';
                removeBtn.style.backgroundColor = 'rgba(255, 255, 255, 0.8)';
                removeBtn.style.borderRadius = '50%';
                removeBtn.style.padding = '5px';
                removeBtn.addEventListener('click', function () {
                    this.parentElement.remove();
                });
                imagePreview.appendChild(removeBtn);

                var imageContainer = document.querySelector('.image-preview');
                imageContainer.appendChild(imagePreview);
            };
            reader.readAsDataURL(files[i]);
        }
    });
});

