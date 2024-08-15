document.addEventListener("DOMContentLoaded", function () {
    getStatisticsmonth();
    getStatisticsyear();
});

function getStatisticsmonth() {
    var month = document.getElementById('month').value;
    var xhr = new XMLHttpRequest();
    xhr.open("GET", "https://localhost:7241/api/Statistics/monthly?month=" + month, true);
    xhr.setRequestHeader("Accept", "application/json");
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
                var response = JSON.parse(xhr.responseText);
                console.log(response);
                updateStatistics(response);
            } else {
                console.error('Lỗi khi gọi API:', xhr.status, xhr.statusText);
            }
        }
    };
    xhr.send();
}
function getStatisticsyear() {
    var year = document.getElementById('month').value;
    var xhr = new XMLHttpRequest();
    xhr.open("GET", "https://localhost:7241/api/Statistics/year?year=" + year, true);
    xhr.setRequestHeader("Accept", "application/json");
    xhr.onreadystatechange = function () {
        if (xhr.readyState === 4) {
            if (xhr.status === 200) {
                var response = JSON.parse(xhr.responseText);
                console.log(response);
                updateCharts(response);
            } else {
                console.error('Lỗi khi gọi API:', xhr.status, xhr.statusText);
            }
        }
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


function updateCharts(data) {
    var lineChartCtx = document.getElementById('lineChartDemo').getContext('2d');
    var pieChartCtx = document.getElementById('pieChart').getContext('2d');

    if (window.lineChart) window.lineChart.destroy();
    if (window.barChart) window.barChart.destroy();

    var revenueData = data.monthlyRevenues;

    var labels = generateMonthlyLabels();

    window.lineChart = new Chart(lineChartCtx, {
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
                    beginAtZero: true
                },
                y: {
                    beginAtZero: true
                }
            }
        }
    });

    window.pieChart = new Chart(pieChartCtx, {
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
    console.log('totalOrdersnosuccess',data.totalOrdersnosuccess)

}

function generateMonthlyLabels() {
    return ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12"];
}
