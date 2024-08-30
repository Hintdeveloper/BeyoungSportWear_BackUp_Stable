let allProducts = [];
let newProducts = [];
let hotSaleProducts = [];

document.addEventListener("DOMContentLoaded", function () {
    loadAllProducts();

    loadBestSellingProducts();
});

function loadAllProducts() {
    var xhr = new XMLHttpRequest();
    xhr.open("GET", "https://localhost:7241/api/ProductDetails/GetAllActive?pageIndex=0&pageSize=10", true);

    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            allProducts = JSON.parse(xhr.responseText);
            newProducts = allProducts.filter(product => calculateIsNew(product.createDate));
            hotSaleProducts = allProducts.filter(product => product.isHotSale);
            renderProducts(allProducts); 
            setupFilterControls();
        }
    };

    xhr.send();
}

function loadBestSellingProducts() {
    var xhr = new XMLHttpRequest();
    xhr.open("GET", "https://localhost:7241/api/Statistics/best-selling-products?startDate=8-24-2024&endDate=9-25-2024", true);

    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            var bestSellingProducts = JSON.parse(xhr.responseText);
            loadBestSellingProductDetails(bestSellingProducts);
        }
    };

    xhr.send();
}

function setupFilterControls() {
    document.querySelectorAll('.filter__controls li').forEach(function (filter) {
        filter.addEventListener('click', function () {
            document.querySelectorAll('.filter__controls li').forEach(function (el) {
                el.classList.remove('active');
            });

            this.classList.add('active');

            var filterClass = this.getAttribute('data-filter');
            if (filterClass === '*') {
                renderProducts(allProducts);
            } else if (filterClass === '.new-arrivals') {
                renderProducts(newProducts);
            } else if (filterClass === '.hot-sales') {
                renderProducts(hotSaleProducts);
            }
        });
    });
}

function calculateIsNew(createDate) {
    if (!createDate) {
        return false;
    }

    const currentDate = new Date();
    const createdDate = new Date(createDate);

    if (isNaN(createdDate.getTime())) {
        return false;
    }

    const diffTime = Math.abs(currentDate - createdDate);
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

    return diffDays <= 7;
}

function renderProducts(products) {
    var productContainer = document.querySelector(".product__filter");
    productContainer.innerHTML = '';
    products.forEach(function (product) {
        var priceHtml;
        if (product.smallestPrice === product.biggestPrice) {
            priceHtml = `<span class="stext-105 cl3">${formatCurrency(product.smallestPrice)}</span>`;
        } else {
            priceHtml = `<span class="stext-105 cl3">${formatCurrency(product.smallestPrice)} - ${formatCurrency(product.biggestPrice)}</span>`;
        }

        const isNew = calculateIsNew(product.createDate);
        const isHotSale = product.isHotSale || false;

        var productItemClass = `col-lg-3 col-md-6 col-sm-6 col-md-6 col-sm-6 mix ${isHotSale ? 'hot-sales' : ''} ${isNew ? 'new-arrivals' : ''}`;

        var productItem = `
            <div class="${productItemClass}">
                <div class="product__item">
                     <div class="product__item__pic set-bg" data-setbg="${product.imagePaths[0]}">
                        ${isNew ? '<span class="label" style="background-color: red; color: yellow;">New</span>' : ''}
                        ${isHotSale ? '<span class="label" style="background-color: green; color: white;">Hot Sale</span>' : ''}
                    </div>
                    <div class="product__item__text">
                        <h6>${product.productName}</h6>
                        <a href="#" class="add-cart" onclick="navigateToUpdatePage('${product.id}')">+ Thêm vào giỏ hàng</a>
                        <h5>${priceHtml}</h5>
                    </div>
                </div>
            </div>
        `;
        productContainer.insertAdjacentHTML('beforeend', productItem);
    });

    document.querySelectorAll('.set-bg').forEach(function (element) {
        var bg = element.getAttribute('data-setbg');
        element.style.backgroundImage = 'url(' + bg + ')';
    });
}

function navigateToUpdatePage(ID) {
    window.location.href = `/details_product/${ID}`;
}

function formatCurrency(value) {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
}

function loadBestSellingProductDetails(bestSellingProducts) {
    var productIds = Object.keys(bestSellingProducts);
    var totalProducts = productIds.length;
    var productsDetails = [];

    productIds.forEach(function (productId, index) {
        var xhr = new XMLHttpRequest();
        xhr.open("GET", `https://localhost:7241/api/ProductDetails/GetByIDAsyncVer_1/${productId}`, true);

        xhr.onreadystatechange = function () {
            if (xhr.readyState === 4 && xhr.status === 200) {
                var productDetail = JSON.parse(xhr.responseText);
                productDetail.isHotSale = true;
                productsDetails.push(productDetail);

                if (productsDetails.length === totalProducts) {
                    hotSaleProducts = productsDetails;
                    if (document.querySelector('.filter__controls li.active').getAttribute('data-filter') === '.hot-sales') {
                        renderProducts(hotSaleProducts);
                    }
                }
            }
        };

        xhr.send();
    });
}
