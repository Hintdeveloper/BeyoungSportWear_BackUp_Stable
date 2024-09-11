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
document.addEventListener('DOMContentLoaded', function () {
    var currentDate = new Date();
    var previousMonthDate = new Date(currentDate);
    previousMonthDate.setMonth(previousMonthDate.getMonth() - 1);

    if (currentDate.getDate() > previousMonthDate.getDate()) {
        previousMonthDate.setDate(0);
    }
    var formattedPreviousMonthDate = previousMonthDate.toISOString().split('T')[0];
    document.getElementById('startDate').value = formattedPreviousMonthDate;

    fetchStatistics();
    fetchYearStatistics();
});
function formatDate(dateString) {
    const [year, month, day] = dateString.split('-');
    return `${month}-${day}-${year}`;
}

async function fetchStatistics() {
    var startDate = document.getElementById('startDate').value;
    var endDate = document.getElementById('endDate').value;

    if (!startDate || !endDate) {
        toastr.error('Vui lòng chọn cả ngày bắt đầu và ngày kết thúc.', 'Lỗi');
        return;
    }

    if (new Date(startDate) > new Date(endDate)) {
        toastr.error('Ngày bắt đầu không thể lớn hơn ngày kết thúc.', 'Lỗi');
        return;
    }

    var formattedStartDate = formatDate(startDate);
    var formattedEndDate = formatDate(endDate);
    var xhr = new XMLHttpRequest();
    xhr.open('GET', `https://localhost:7241/api/Statistics/calculate_statistics/${formattedStartDate}/${formattedEndDate}`, true);

    xhr.setRequestHeader('Accept', '*/*');
    xhr.onload = function () {
        if (xhr.status >= 200 && xhr.status < 300) {
            var data = JSON.parse(xhr.responseText);
            console.log('Data received:', data);
            //updateCharts(data);
            updateStatistics(data);
            const bestSellingProducts = data.bestSellingProducts;
            const productDetailsApiUrl = 'https://localhost:7241/api/ProductDetails/GetByIDAsyncVer_1/';

            const orderTableBody = document.getElementById('order_table_body');
            orderTableBody.innerHTML = '';

            async function fetchProductDetails(productId, salesCount) {
                try {
                    const response = await fetch(productDetailsApiUrl + productId);
                    const product = await response.json();
                    const formattedSmallestPrice = product.smallestPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
                    const formattedBiggestPrice = product.biggestPrice.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });

                    let priceToShow = '';
                    if (formattedSmallestPrice !== formattedBiggestPrice) {
                        priceToShow = `Từ ${formattedSmallestPrice} đến ${formattedBiggestPrice}`;
                    } else {
                        priceToShow = formattedSmallestPrice;
                    }

                    const row = document.createElement('tr');
                    row.innerHTML = `
                        <td>${product.keyCode}</td>
                        <td>${product.productName}</td>
                        <td>${priceToShow}</td>
                        <td>${salesCount}</td>
                    `;
                    orderTableBody.appendChild(row);
                } catch (error) {
                    console.error('Error fetching product details:', error);
                }
            }

            for (let productId in bestSellingProducts) {
                fetchProductDetails(productId, bestSellingProducts[productId]);
            }
        } else {
            console.error('Lỗi khi lấy dữ liệu:', xhr.responseText);
        }
    };
    xhr.onerror = function () {
        console.error('Lỗi kết nối đến máy chủ.');
    };
    xhr.send();
}

function updateStatistics(data) {
    if (data) {
        document.getElementById('totalOrder').innerHTML = `<strong>${data.totalOrder}</strong> ( <small>Hoàn thành: ${data.totalOrderssuccess}</small> - <small> Hủy: ${data.totalOrdersnosuccess}</small>)`;
        document.getElementById('productsAlmostOutOfStock').innerHTML = `<strong>${data.productsAlmostOutOfStock}</strong> (dưới 10 sản phẩm)`;
        document.getElementById('totalUser').innerText = data.totalUser;
        document.getElementById('totalQuantityOptions').innerHTML = `<strong>${data.totalProduct}</strong> (<small>Số lượng: ${data.totalQuantityOptions}</small>)`;
    } else {
        console.error('Dữ liệu không hợp lệ:', data);
    }
}

function fetchYearStatistics() {
    var xhr = new XMLHttpRequest();
    var currentYear = new Date().getFullYear();

    var formattedYear = `${currentYear}-01-01`;
    xhr.open('GET', `https://localhost:7241/api/Statistics/year?year=${formattedYear}`, true);

    xhr.setRequestHeader('Accept', '*/*');
    xhr.onload = function () {
        if (xhr.status >= 200 && xhr.status < 300) {
            var data = JSON.parse(xhr.responseText);
            updateLineChart(data);
            console.log('updateLineChart', data)
        } else {
            console.error('Lỗi khi lấy dữ liệu:', xhr.responseText);
        }
    };
    xhr.onerror = function () {
        console.error('Lỗi kết nối đến máy chủ.');
    };
    xhr.send();
}

var pieChart;
var lineChart;  // Đảm bảo biến này được khai báo ngoài phạm vi hàm để có thể truy cập toàn cục

function updateLineChart(data) {
    var lineChartCtx = document.getElementById('lineChartDemo').getContext('2d');

    // Kiểm tra và hủy biểu đồ cũ nếu đã tồn tại
    if (lineChart) {
        lineChart.destroy();
    }

    // Kiểm tra dữ liệu
    const revenueData = data.monthlyRevenues || [];
    console.log('Dữ liệu doanh thu:', revenueData); // Kiểm tra dữ liệu
    if (revenueData.length !== 12) {
        console.error('Dữ liệu doanh thu không hợp lệ. Cần có 12 tháng dữ liệu.');
        return;
    }

    // Tạo biểu đồ mới
    lineChart = new Chart(lineChartCtx, {
        type: 'line',
        data: {
            labels: generateMonthlyLabels(), // ["Tháng 1", "Tháng 2", ..., "Tháng 12"]
            datasets: [{
                label: 'Doanh thu theo tháng trong năm',
                data: revenueData,
                borderColor: 'rgba(75, 192, 192, 1)',
                borderWidth: 1,
                fill: false
            }]
        },
        options: {
            responsive: true,
            scales: {
                x: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Tháng'
                    }
                },
                y: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Doanh thu (VND)'
                    },
                    ticks: {
                        callback: function (value) {
                            return value.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
                        }
                    }
                }
            }
        }
    });
}

// Hàm để sinh các nhãn tháng
function generateMonthlyLabels() {
    return ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"];
}
function updateCharts(data) {
    var lineChartCtx = document.getElementById('lineChartDemo').getContext('2d');
    var pieChartCtx = document.getElementById('pieChart').getContext('2d');

    // Xóa biểu đồ cũ nếu có
    if (lineChart) {
        lineChart.destroy();
    }
    if (pieChart) {
        pieChart.destroy();
    }

    var revenueData = data.monthlyRevenues || [];
    var labels = generateMonthlyLabels();

    // Tạo biểu đồ đường mới
    lineChart = new Chart(lineChartCtx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Doanh thu theo tháng trong năm',
                data: revenueData,
                borderColor: 'rgba(75, 192, 192, 1)',
                borderWidth: 1,
                fill: false
            }]
        },
        options: {
            responsive: true,
            scales: {
                x: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Tháng'
                    }
                },
                y: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Doanh thu (VND)'
                    },
                    ticks: {
                        callback: function (value) {
                            return value.toLocaleString('vi-VN', { style: 'currency', currency: 'VND' });
                        }
                    }
                }
            }
        }
    });

    // Tạo biểu đồ tròn mới
    pieChart = new Chart(pieChartCtx, {
        type: 'pie',
        data: {
            labels: ['Đơn hàng không thành công', 'Đơn hàng thành công'],
            datasets: [{
                data: [data.totalOrdersnosuccess, data.totalOrderssuccess],
                backgroundColor: ['#FF6384', '#36A2EB'],
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false
        }
    });
}
document.addEventListener('DOMContentLoaded', function () {
    toastr.options = {
        "closeButton": true,
        "debug": false,
        "newestOnTop": false,
        "progressBar": true,
        "positionClass": "toast-top-right",
        "preventDuplicates": false,
        "onclick": null,
        "showDuration": "300",
        "hideDuration": "1000",
        "timeOut": "10000",
        "extendedTimeOut": "1000",
        "showEasing": "swing",
        "hideEasing": "linear",
        "showMethod": "fadeIn",
        "hideMethod": "fadeOut"
    };
});
