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

    const btnSuccessUpdate = document.getElementById('btn_success_update');
    btnSuccessUpdate.addEventListener('click', function (event) {
        event.preventDefault();

        var productData = {
            ModifiedBy: userId,
            ProductName: document.getElementById('product_name').value,
            CategoryName: document.getElementById('category_name').value,
            ManufacturersName: document.getElementById('manufacture_name').value,
            MaterialName: document.getElementById('material_name').value,
            BrandName: document.getElementById('brand_name').value,
            Style: document.getElementById('product_style').value,
            Origin: document.getElementById('product_origin').value,
            Description: document.getElementById('product_description').value,
            ImagePaths: [],
            NewOptions: gatherOptionsData(ID)
        };

        const imageUpload = document.getElementById('image-upload');
        productData.ImagePaths = getImagePaths(imageUpload);

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
        }).catch(error => {
            console.error('Đã xảy ra lỗi:', error);
            Swal.fire({
                icon: 'error',
                title: 'Lỗi',
                text: `Đã xảy ra lỗi trong quá trình cập nhật sản phẩm hoặc tải ảnh: ${error}`
            });
        });

        console.log(productData, imageUpload);
        console.log(productData.NewOptions);
    });
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
    document.body.addEventListener('click', function (event) {
        if (event.target.classList.contains('remove-button')) {
            event.preventDefault();
            var optionId = event.target.getAttribute('data-option-id');
            removeRow(optionId);
        }
    });
    document.body.addEventListener('click', function (event) {
        if (event.target.classList.contains('toggle-button')) {
            event.preventDefault();
            var optionId = event.target.getAttribute('data-option-id');
            var isActive = event.target.getAttribute('data-isactive') === 'true';

            var newIsActive = !isActive;

            var data = {
                idEntity: optionId,
                isActive: newIsActive
            };

            var xhr = new XMLHttpRequest();
            xhr.open("POST", "https://localhost:7241/api/Options/UpdateIsActive", true);
            xhr.setRequestHeader("Content-Type", "application/json");

            xhr.onload = function () {
                if (xhr.status === 200) {
                    Swal.fire({
                        icon: 'success',
                        title: 'Thành công',
                        text: newIsActive ? 'Đã kích hoạt thành công.' : 'Đã hủy kích hoạt thành công.'
                    }).then(() => {
                        console.log("Đã cập nhật thành công trạng thái isActive có id: " + optionId);
                        event.target.setAttribute('data-isactive', newIsActive.toString());
                        event.target.textContent = newIsActive ? 'Hủy kích hoạt' : 'Kích hoạt';

                        var statusText = newIsActive ? '<span style="color: green;">Đang bán</span>' : '<span style="color: red;">Ngừng bán</span>';
                        var statusElement = event.target.parentNode.previousElementSibling.lastElementChild;
                        statusElement.innerHTML = `<span>(${statusText})</span>`;

                    });
                }
                else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Lỗi',
                        text: newIsActive ? 'Đã xảy ra lỗi khi kích hoạt.' : 'Đã xảy ra lỗi khi hủy kích hoạt.'
                    });
                }
            };

            xhr.onerror = function () {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi kết nối',
                    text: 'Đã xảy ra lỗi kết nối khi gửi yêu cầu.'
                });
                console.error("Đã xảy ra lỗi kết nối khi gửi yêu cầu cập nhật trạng thái isActive có id: " + optionId);
            };

            xhr.send(JSON.stringify(data));
        }
    });
    function removeRow(id) {
        var data = {
            idEntity: id,
            isActive: false
        };

        var xhr = new XMLHttpRequest();
        xhr.open("POST", "https://localhost:7241/api/Options/UpdateIsActive", true);
        xhr.setRequestHeader("Content-Type", "application/json");

        xhr.onload = function () {
            if (xhr.status === 200) {
                Swal.fire({
                    icon: 'success',
                    title: 'Thành công',
                    text: 'Đã hủy kích hoạt thành công.'
                }).then(() => {
                    console.log("Đã cập nhật thành công trạng thái isActive có id: " + id);
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi',
                    text: 'Đã xảy ra lỗi khi hủy kích hoạt.'
                });
                console.error("Đã xảy ra lỗi khi cập nhật trạng thái isActive có id: " + id);
                console.error(xhr.responseText);
                console.log(data)
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
async function uploadImages(productId, product_images) {
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

    formData.append('IDProductDetails', productId);
    formData.append('CreateBy', 'John Doe');
    formData.append('Status', 1);

    console.log(productId);

    return new Promise((resolve, reject) => {
        var xhr = new XMLHttpRequest();
        xhr.open('POST', 'https://localhost:7241/api/images/upload_images', true);

        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4) {
                if (xhr.status === 200) {
                    console.log('Ảnh đã được tải lên thành công');
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
            xhr.setRequestHeader("Authorization", "Bearer " + jwt);

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
function displayProductImages(imagePaths) {
    var imagePreviewContainer = document.querySelector('.image-preview');
    imagePaths.forEach(function (imagePath) {
        var imgElement = document.createElement('img');
        imgElement.src = imagePath;
        imgElement.alt = 'Preview';
        imgElement.style.maxWidth = '150px';

        var labelElement = document.createElement('label');
        labelElement.classList.add('image-preview-item');

        labelElement.appendChild(imgElement);
        imagePreviewContainer.appendChild(labelElement);
    });
}
function viewDetails(IDOptions) {
    if (!IDOptions) {
        console.error('No option ID provided');
        return;
    }
    var xhr = new XMLHttpRequest();
    xhr.open('GET', `https://localhost:7241/api/Options/GetByID/${IDOptions}`, true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
                var data = JSON.parse(xhr.responseText);
                document.getElementById('optionColor').value = data.colorName || '';
                document.getElementById('optionSize').value = data.sizesName || '';
                document.getElementById('optionPrice').value = data.retailPrice || '';
                document.getElementById('optionQuantity').value = data.stockQuantity || '';
                var optionImagePreview = document.getElementById('optionImagePreview');
                if (data.imageURL) {
                    optionImagePreview.src = data.imageURL;
                    optionImagePreview.style.display = 'block';
                } else {
                    optionImagePreview.src = '#';
                    optionImagePreview.style.display = 'none';
                }
                var modalElement = document.getElementById('options_update_modal');
                var modal = new bootstrap.Modal(modalElement);
                modal.show();
                document.getElementById('updateOptionsForm').setAttribute('data-id', IDOptions);

            } else {
                console.error('Error fetching option details:', xhr.status);
            }
        }
    }

    xhr.onerror = function () {
        console.error('Request error');
    };

    xhr.send();
}
document.getElementById('saveOptionBtn').addEventListener('click', function () {
    var form = document.getElementById('updateOptionsForm');
    var ID = form.getAttribute('data-id');

    if (!ID) {
        console.error('No option ID found');
        return;
    }

    var colorName = document.getElementById('optionColor').value;
    var sizeName = document.getElementById('optionSize').value;
    var retailPrice = document.getElementById('optionPrice').value;
    var stockQuantity = document.getElementById('optionQuantity').value;
    var imageFile = document.getElementById('optionImage').files[0];

    var formData = new FormData();
    formData.append('ColorName', colorName);
    formData.append('SizesName', sizeName);
    formData.append('RetailPrice', retailPrice);
    formData.append('StockQuantity', stockQuantity);
    formData.append('IsActive', true);
    formData.append('ModifiedBy', userId);
    if (imageFile) {
        formData.append('ImageFile', imageFile);
    }

    var xhr = new XMLHttpRequest();
    xhr.open('PUT', `https://localhost:7241/api/Options/update/${ID}`, true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
                Swal.fire({
                    icon: 'success',
                    title: 'Cập nhật thành công',
                    text: 'Dữ liệu đã được cập nhật thành công!',
                    confirmButtonText: 'OK'
                }).then(function () {
                    location.reload();
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: 'Lỗi cập nhật',
                    text: `Không thể cập nhật dữ liệu. Mã lỗi: ${xhr.responseText}`,
                    confirmButtonText: 'OK'
                });
                console.error('Error updating option:', xhr.responseText);
            }
        }
    };

    xhr.onerror = function () {
        Swal.fire({
            icon: 'error',
            title: 'Lỗi kết nối',
            text: 'Có lỗi xảy ra khi gửi yêu cầu. Vui lòng thử lại.',
            confirmButtonText: 'OK'
        });
        console.error('Request error');
    };

    xhr.send(formData);
});
document.getElementById('optionImage').addEventListener('change', function (event) {
    var file = event.target.files[0];
    var reader = new FileReader();

    if (file) {
        reader.onload = function (e) {
            var preview = document.getElementById('optionImagePreview');
            preview.src = e.target.result;
            preview.style.display = 'block';
        };
        reader.readAsDataURL(file);
    } else {
        document.getElementById('optionImagePreview').style.display = 'none';
    }
});
function getProductDetailsById(ID) {
    var xhr = new XMLHttpRequest();
    xhr.open('GET', `https://localhost:7241/api/ProductDetails/GetByIDAsyncVer_1/${ID}`, true);
    xhr.setRequestHeader("Authorization", "Bearer " + jwt);

    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
                var data = JSON.parse(xhr.responseText);
                console.log(data)
                document.getElementById('keycode').value = data.keyCode;
                document.getElementById('product_name').value = data.productName;
                document.getElementById('category_name').value = data.categoryName;
                document.getElementById('manufacture_name').value = data.manufacturersName;
                document.getElementById('material_name').value = data.materialName;
                document.getElementById('brand_name').value = data.brandName;
                document.getElementById('product_style').value = data.style;
                document.getElementById('product_origin').value = data.origin;
                document.getElementById('product_description').value = data.description;
                displayProductImages(data.imagePaths);
                var optionsTableBody = document.getElementById('oldClassificationBody');
                optionsTableBody.innerHTML = '';
                if (data.options && data.options.length > 0) {
                    data.options.forEach(function (option) {
                        var statusText = option.isActive === false ? '<span style="color: red;">Ngừng bán</span>' : '<span style="color: green;">Đang bán</span>';

                        var row = `<tr>
                            <td><img src="${option.imageURL}" alt="Hình ảnh" width="60px"></td>
                            <td>${option.colorName}</td>
                            <td>${option.sizesName}</td>
                            <td>${formatCurrency(option.retailPrice)}</td>
                            <td>${option.stockQuantity} <span>(${statusText})</span></td>
                            <td>
                               <button class="toggle-button" data-option-id="${option.id}" data-isactive="${option.isActive}"
                                    style="background-color: ${option.isActive ? 'red' : 'green'}; color: white; padding: 5px 10px; border: none; cursor: pointer;">
                                    ${option.isActive ? 'Hủy kích hoạt' : 'Kích hoạt'}
                                </button>

                                <button class="btn btn-primary btn-sm edit" id="btn_edit_options" type="button" onclick="viewDetails('${option.id}')" >
                                    <i class="fas fa-edit"></i>
                                </button>
                            </td>
                        </tr>`;
                        optionsTableBody.insertAdjacentHTML('beforeend', row);
                    });
                } else {
                    optionsTableBody.innerHTML = '<tr><td class="no-data" colspan="7">Không có dữ liệu</td></tr>';
                }
            } else {
                console.error('Error fetching product details by Id:', xhr.status);
                alert('Đã xảy ra lỗi khi lấy dữ liệu sản phẩm. Vui lòng thử lại sau.');
            }
        }
    };
    xhr.send();
}
function formatCurrency(amount) {
    const formatter = new Intl.NumberFormat('vi-VN', {
        style: 'currency',
        currency: 'VND'
    });
    return formatter.format(amount);
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
            const row = document.createElement('tr');
            const imagePreviewId = `imagePreview-${color}-${size}`;
            row.innerHTML = `
                <td class="image-preview-options" id="${imagePreviewId}">
                    <input type="file" id="image-upload-${imagePreviewId}" class="custom-file-input">
                </td>
                <td>${color}</td>
                <td>${size}</td>
                <td><input id="giaBan" type="text"  placeholder="Nhập giá bán"></td>
                <td><input id="soLuong" type="text" placeholder="0"></td>
                <td><button class="remove-button" onclick="removeRow(this)">Xóa</button></td>
            `;
            tableBody.appendChild(row);

            const fileInput = row.querySelector(`#image-upload-${imagePreviewId}`);
            fileInput.addEventListener('change', function (event) {
                handleImageUpload(event, imagePreviewId);
            });
        });
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
document.addEventListener("DOMContentLoaded", function () {
    var imageUpload = document.getElementById('image-upload');
    imageUpload.addEventListener('change', function () {
        var files = this.files;
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
