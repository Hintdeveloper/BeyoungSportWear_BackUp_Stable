

var tabCount = 1;

function addTab() {
    tabCount++;
    var newTabId = 'HoaDon' + tabCount;
    var newTabContent = '<div id="' + newTabId + '" class="tab-content">' +
        '<iframe src="https://localhost:7065/viewshare_sale_at_the_counter?invoiceNumber=' + tabCount + '"></iframe>' +
        '</div>';

    var tabsContainer = document.querySelector('.tabs');
    var newTab = document.createElement('div');
    newTab.className = 'tab';
    newTab.setAttribute('onclick', 'openTab(event, \'' + newTabId + '\')');
    newTab.innerHTML = 'Hóa đơn ' + tabCount +
        '<span class="close-btn" onclick="closeTab(event, \'' + newTabId + '\')">&times;</span>';

    tabsContainer.appendChild(newTab);

    var tabContainer = document.querySelector('#tabContainer');
    tabContainer.insertAdjacentHTML('beforeend', newTabContent);

    tabContainer.scrollLeft = tabContainer.scrollWidth;

    openTab(null, newTabId);
    saveTabsToCookies();
}
function closeTab(event, tabId) {
    event.stopPropagation();
    event.preventDefault();

    // Hiển thị SweetAlert2 để yêu cầu xác nhận
    Swal.fire({
        title: 'Bạn có chắc chắn muốn đóng tab này?',
        text: "Tất cả các dữ liệu sẽ bị xóa!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Xác nhận',
        cancelButtonText: 'Hủy bỏ'
    }).then((result) => {
        if (result.isConfirmed) {
            var tabElement = document.querySelector(`[onclick="openTab(event, '${tabId}')"]`);
            var tabContentElement = document.getElementById(tabId);
            if (tabElement) tabElement.remove();
            if (tabContentElement) tabContentElement.remove();

            var cookieData = getCookie(tabId);
            if (cookieData) {
                try {
                    var decodedData = decodeURIComponent(cookieData);
                    var parsedData = JSON.parse(decodedData);

                    if (parsedData.selectedProducts) {
                        parsedData.selectedProducts.forEach(product => {
                            const xhrGet = new XMLHttpRequest();
                            xhrGet.open('GET', `https://localhost:7241/api/Options/GetByID/${product.idoptions}`, true);
                            xhrGet.setRequestHeader('Accept', 'application/json');

                            xhrGet.onreadystatechange = function () {
                                if (xhrGet.readyState === 4) {
                                    if (xhrGet.status === 200) {
                                        const option = JSON.parse(xhrGet.responseText);
                                        const stockQuantity = option.stockQuantity;

                                        updateStockDisplay(product.idoptions, stockQuantity);
                                        const xhrUpdate = new XMLHttpRequest();
                                        xhrUpdate.open('POST', 'https://localhost:7241/api/Options/increase-quantity', true);
                                        xhrUpdate.setRequestHeader('Content-Type', 'application/json');
                                        xhrUpdate.setRequestHeader('Accept', 'application/json');

                                        xhrUpdate.onreadystatechange = function () {
                                            if (xhrUpdate.readyState === XMLHttpRequest.DONE) {
                                                if (xhrUpdate.status === 200) {
                                                    toastr.success(`Đã kết thúc phiên giao dịch`, 'Thành công');
                                                } else {
                                                    toastr.error(`Bạn vừa bấm vào nút xóa ${tabId} nhưng mà lỗi`, 'Lỗi');
                                                    console.error('Error:', xhrUpdate.status, xhrUpdate.statusText);
                                                    console.error('Response Text:', xhrUpdate.responseText);
                                                }
                                            }
                                        };

                                        const requestData = JSON.stringify({
                                            idOptions: product.idoptions,
                                            quantityToDecrease: product.quantity
                                        });

                                        xhrUpdate.send(requestData);

                                        connection.invoke("UpdateProductQuantity", product.idoptions, stockQuantity)
                                            .catch(err => console.error('Error sending stock update:', err));
                                    } else {
                                        console.error('Error fetching option:', xhrGet.status, xhrGet.statusText);
                                        console.error('Response Text:', xhrGet.responseText);
                                    }
                                }
                            };

                            xhrGet.send();
                        });
                    }
                } catch (e) {
                    console.error('Error parsing cookie data:', e);
                }
            }

            var tabs = document.querySelectorAll('.tab');
            if (tabs.length > 0) {
                var activeTab = tabs[0];
                var activeTabId = activeTab.getAttribute('onclick').match(/'(.*?)'/)[1];
                openTab(null, activeTabId);
            }

            let cookieTabs = getCookie('tabs');
            if (cookieTabs) {
                try {
                    let decodedData = decodeURIComponent(cookieTabs);
                    let tabs = JSON.parse(decodedData);
                    tabs = tabs.filter(tab => tab.id !== tabId);
                    document.cookie = 'tabs=' + encodeURIComponent(JSON.stringify(tabs)) + '; path=/; max-age=86400';
                } catch (e) {
                    console.error('Error parsing cookie data:', e);
                }
            }

            deleteCookie(tabId);
            updateInvoiceNumbers();
            saveTabsToCookies();

            toastr.success(`Đã kết thúc phiên giao dịch`, 'Thành công');
        }
    });
}

function deleteCookie(name) {
    document.cookie = name + '=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/';
}

function openTab(event, tabId) {
    var tabs = document.querySelectorAll('.tab');
    tabs.forEach(function (tab) {
        tab.classList.remove('active');
    });

    if (event) {
        event.target.closest('.tab').classList.add('active');
    } else {
        document.querySelectorAll('.tab').forEach(function (tab) {
            if (tab.getAttribute('onclick').includes(tabCount)) {
                tab.classList.add('active');
            }
        });
    }

    var tabContents = document.querySelectorAll('.tab-content');
    tabContents.forEach(function (content) {
        content.classList.remove('active');
    });

    document.getElementById(tabId).classList.add('active');
}
function updateInvoiceNumbers() {
    var tabs = document.querySelectorAll('.tab');
    tabs.forEach(function (tab, index) {
        var newInvoiceNumber = index + 1;
        var tabId = 'HoaDon' + newInvoiceNumber;
        var oldTabId = tab.getAttribute('onclick').match(/'(.*?)'/)[1];

        tab.setAttribute('onclick', 'openTab(event, \'' + tabId + '\')');
        tab.childNodes[0].nodeValue = 'Hóa đơn ' + newInvoiceNumber;

        var oldTabContent = document.getElementById(oldTabId);
        oldTabContent.id = tabId;
        var iframe = oldTabContent.querySelector('iframe');
        iframe.src = 'https://localhost:7065/viewshare_sale_at_the_counter?invoiceNumber=' + newInvoiceNumber;
    });

    tabCount = tabs.length;
    saveTabsToCookies();
}
function saveTabsToCookies() {
    var tabs = document.querySelectorAll('.tab');
    var tabsState = [];
    tabs.forEach(function (tab, index) {
        var tabId = tab.getAttribute('onclick').match(/'(.*?)'/)[1];
        var iframeSrc = document.getElementById(tabId).querySelector('iframe').src;

        tabsState.push({
            id: tabId,
            invoiceNumber: index + 1,
            src: iframeSrc
        });
    });

    setCookie('tabs', encodeURIComponent(JSON.stringify(tabsState)), 1);
}
function getCookie(name) {
    const nameEQ = name + "=";
    const ca = document.cookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) === ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}
function setCookie(name, value, days) {
    let expires = "";
    if (days) {
        const date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}
function loadTabsFromCookies() {
    var tabsCookie = getCookie('tabs');
    if (tabsCookie) {
        var tabs = JSON.parse(decodeURIComponent(tabsCookie));
        tabs.forEach(function (tab) {
            addTabFromCookie(tab.id, tab.invoiceNumber, tab.src);
        });

        var firstTab = document.querySelector('.tab');
        if (firstTab) {
            var firstTabId = firstTab.getAttribute('onclick').match(/'(.*?)'/)[1];
            openTab({ target: firstTab }, firstTabId);
        }
    }
}
function addTabFromCookie(tabId, invoiceNumber, iframeSrc) {
    // Kiểm tra sự tồn tại của phần tử tabsContainer
    var tabsContainer = document.querySelector('.tabs');
    if (!tabsContainer) {
        return; // Ngừng thực hiện nếu phần tử không tồn tại
    }

    // Kiểm tra sự tồn tại của phần tử tabContainer
    var tabContainer = document.querySelector('#tabContainer');
    if (!tabContainer) {
        console.error('Element with id "#tabContainer" not found.');
        return; // Ngừng thực hiện nếu phần tử không tồn tại
    }

    // Nếu tab không tồn tại, tạo và thêm tab mới
    if (!document.getElementById(tabId)) {
        tabCount++;
        var newTab = document.createElement('div');
        newTab.className = 'tab';
        newTab.setAttribute('onclick', 'openTab(event, \'' + tabId + '\')');
        newTab.innerHTML = 'Hóa đơn ' + invoiceNumber +
            '<span class="close-btn" onclick="closeTab(event, \'' + tabId + '\')">&times;</span>';

        tabsContainer.appendChild(newTab);

        var newTabContent = '<div id="' + tabId + '" class="tab-content">' +
            '<iframe src="' + iframeSrc + '"></iframe>' +
            '</div>';

        tabContainer.insertAdjacentHTML('beforeend', newTabContent);

        openTab(null, tabId);
    }
}
document.addEventListener("DOMContentLoaded", function () {
    loadTabsFromCookies();
});
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
function updateStockDisplay(optionId, newQuantity) {
    const stockElement = document.getElementById(`stock_quantity_${optionId}`);
    if (stockElement) {
        stockElement.textContent = `(SLT: ${newQuantity})`;
    }
}
