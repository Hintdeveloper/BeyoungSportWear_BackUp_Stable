let allProducts = [];
let newProducts = [];
let hotSaleProducts = [];
let filteredProducts = []; 
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
            filteredProducts = allProducts; 
            renderProducts(filteredProducts);
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
                filteredProducts = allProducts; 
            } else if (filterClass === '.new-arrivals') {
                filteredProducts = newProducts; 
            } else if (filterClass === '.hot-sales') {
                filteredProducts = hotSaleProducts;
            }
            renderProducts(filteredProducts, 1); 
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

function renderProducts(products, currentPage = 1, pageSize = 8) {
    console.log('products', products);
    var productContainer = document.querySelector(".product__filter");
    productContainer.innerHTML = '';

    if (products.length === 0) {
        productContainer.innerHTML = '<p>Không có sản phẩm nào để hiển thị.</p>';
        return;
    }
    products.sort(function (a, b) {
        return (a.totalQuantity <= 0 ? 1 : 0) - (b.totalQuantity <= 0 ? 1 : 0);
    });
    const start = (currentPage - 1) * pageSize;
    const end = start + pageSize;
    const paginatedProducts = products.slice(start, end);

    if (paginatedProducts.length === 0) {
        productContainer.innerHTML = '<p>Không có sản phẩm nào để hiển thị cho trang này.</p>';
        return;
    }

    paginatedProducts.forEach(function (product) {
        var isOutOfStock = product.totalQuantity <= 0;
        var productStyle = isOutOfStock ? 'opacity: 0.5; color: gray; pointer-events: none;' : '';
        var outOfStockText = isOutOfStock ? '<strong style="color: red;">Hết hàng</strong>' : '';

        var priceHtml;
        if (product.smallestPrice === product.biggestPrice) {
            priceHtml = `<span class="stext-105 cl3">${formatCurrency(product.smallestPrice)}</span>`;
        } else {
            priceHtml = `<span class="stext-105 cl3">${formatCurrency(product.smallestPrice)} - ${formatCurrency(product.biggestPrice)}</span>`;
        }

        const isNew = calculateIsNew(product.createDate);
        const isHotSale = product.isHotSale || false;

        var productItem = `
            <div class="col-lg-3 col-md-6 col-sm-6 col-md-6 col-sm-6 mix ${isHotSale ? 'hot-sales' : ''}" style="${productStyle}">
                <div class="product__item" style="border: 2px solid black; border-radius: 4px; padding: 10px;">
                    <div class="product__item__pic set-bg" data-setbg="${product.imagePaths[0]}" style="border-bottom: 2px solid black;">
                        ${isNew ? '<span class="label" style="background-color: red; color: yellow;">New</span>' : ''}
                        ${isHotSale ? '<span class="label" style="background-color: green; color: white;">Hot Sale</span>' : ''}
                    </div>
                    <div class="product__item__text">
                        <h6>${product.productName}</h6>
                        <a href="#" class="add-cart" onclick="${isOutOfStock ? 'return false;' : `navigateToUpdatePage('${product.id}')`}">+ Thêm vào giỏ hàng</a>
                        <h5>${priceHtml}</h5><small>Tổng kho: ${product.totalQuantity} ${outOfStockText}</small>
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

    updatePaginationControls(products, currentPage, Math.ceil(products.length / pageSize));
}

function updatePaginationControls(products, currentPage, totalPages) {
    const pagination = document.querySelector('.pagination');
    if (!pagination) return;

    pagination.innerHTML = '';

    const createPageLink = (page, text) => {
        const link = document.createElement('a');
        link.href = '#';
        link.textContent = text;
        link.className = page === currentPage ? 'page-number active' : 'page-number';
        link.setAttribute('data-page', page);
        return link;
    };

    const prevLink = createPageLink(currentPage - 1, '«');
    prevLink.classList.add('prev');
    if (currentPage === 1) {
        prevLink.style.visibility = 'hidden';
    }
    pagination.appendChild(prevLink);

    for (let i = 1; i <= totalPages; i++) {
        pagination.appendChild(createPageLink(i, i));
    }

    const nextLink = createPageLink(currentPage + 1, '»');
    nextLink.classList.add('next');
    if (currentPage === totalPages) {
        nextLink.style.visibility = 'hidden';
    }
    pagination.appendChild(nextLink);

    pagination.querySelectorAll('.page-number').forEach(function (pageLink) {
        pageLink.addEventListener('click', function (e) {
            e.preventDefault();
            const page = parseInt(pageLink.getAttribute('data-page'));
            if (page >= 1 && page <= totalPages) {
                renderProducts(filteredProducts, page); 
            }
        });
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
