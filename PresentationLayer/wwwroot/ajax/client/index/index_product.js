document.addEventListener("DOMContentLoaded", function () {
    var xhr = new XMLHttpRequest();
    xhr.open("GET", "https://localhost:7241/api/ProductDetails/GetAllActive?pageIndex=0&pageSize=10", true);

    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4 && xhr.status === 200) {
            var products = JSON.parse(xhr.responseText);
            renderProducts(products);
        }
    };

    xhr.send();
});
function formatCurrency(value) {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
}

function renderProducts(products) {
    var productContainer = document.querySelector(".product__filter");
    productContainer.innerHTML = '';
    products.forEach(function (product) {
        var priceHtml;
        if (product.smallestPrice === product.biggestPrice) {
            priceHtml = `
            <span class="stext-105 cl3">${formatCurrency(product.smallestPrice)}</span>
              `;
        } else {
            priceHtml = `
            <span class="stext-105 cl3">${formatCurrency(product.smallestPrice)} - ${formatCurrency(product.biggestPrice)}</span>
              `;
        }
        var productItem = `
            <div class="col-lg-3 col-md-6 col-sm-6 col-md-6 col-sm-6 mix new-arrivals">
                <div class="product__item">
                    <div class="product__item__pic set-bg" data-setbg="${product.imagePaths[0]}">
                        ${product.isNew ? '<span class="label">New</span>' : ''}
                    </div>
                    <div class="product__item__text">
                        <h6>${product.productName}</h6>
                        <a href="#" class="add-cart" onclick="navigateToUpdatePage('${product.id}')">Xem chi tiết</a>
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