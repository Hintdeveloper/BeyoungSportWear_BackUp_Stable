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
    saveStockQuantitiesToCookies();
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
            if (tab.getAttribute('onclick').includes(tabId)) {
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

function closeTab(event, tabId) {
    event.stopPropagation();

    var tabToRemove = document.getElementById(tabId);
    var wasActive = tabToRemove.classList.contains('active');

    let stockQuantities = getStockQuantitiesFromLocalStorage();
    let tabStockQuantities = getCookie(tabCount); 
    if (tabStockQuantities) {
        tabStockQuantities = JSON.parse(tabStockQuantities);
        Object.keys(tabStockQuantities).forEach(optionId => {
            if (stockQuantities[optionId] !== undefined) {
                stockQuantities[optionId] += tabStockQuantities[optionId];
            } else {
                stockQuantities[optionId] = tabStockQuantities[optionId];
            }
        });

        localStorage.setItem('stockQuantities', JSON.stringify(stockQuantities));
    }

    deleteCookie(tabCount);

    tabToRemove.remove();
    event.target.closest('.tab').remove();

    tabCount--;

    if (wasActive && document.querySelectorAll('.tab').length > 0) {
        var firstTab = document.querySelector('.tab');
        var firstTabId = firstTab.getAttribute('onclick').match(/'(.*?)'/)[1];
        openTab({ target: firstTab }, firstTabId);
    }

    updateInvoiceNumbers();
    saveTabsToCookies();
}

function deleteCookie(name) {
    document.cookie = name + '=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
}

function getStockQuantitiesFromLocalStorage() {
    const stockQuantities = localStorage.getItem('stockQuantities');
    return stockQuantities ? JSON.parse(stockQuantities) : {};
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

function updateInvoiceNumbers() {
    var tabs = document.querySelectorAll('.tab');
    tabs.forEach(function (tab, index) {
        var newInvoiceNumber = index + 1;
        var tabId = 'HoaDon' + newInvoiceNumber;
        var oldTabId = tab.getAttribute('onclick').match(/'(.*?)'/)[1];

        tab.setAttribute('onclick', 'openTab(event, \'' + tabId + '\')');
        tab.querySelector('.close-btn').setAttribute('onclick', 'closeTab(event, \'' + tabId + '\')');
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
            }
        }
    });

    document.cookie = 'tabs=' + encodeURIComponent(JSON.stringify(tabsState)) + '; path=/; max-age=86400';
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
    if (!document.getElementById(tabId)) {
        tabCount++;
        var tabsContainer = document.querySelector('.tabs');
        var newTab = document.createElement('div');
        newTab.className = 'tab';
        newTab.setAttribute('onclick', 'openTab(event, \'' + tabId + '\')');
        newTab.innerHTML = 'Hóa đơn ' + invoiceNumber +
            '<span class="close-btn" onclick="closeTab(event, \'' + tabId + '\')">&times;</span>';

        tabsContainer.appendChild(newTab);

        var tabContainer = document.querySelector('#tabContainer');
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
function saveStockQuantitiesToCookies() {
    var tabs = document.querySelectorAll('.tab');
    tabs.forEach(function (tab) {
        var tabId = tab.getAttribute('onclick').match(/'(.*?)'/)[1];
        var stockQuantities = getStockQuantitiesFromLocalStorage();
        var tabStockQuantities = {};

        document.cookie = tabId + '=' + encodeURIComponent(JSON.stringify(tabStockQuantities)) + '; path=/; max-age=86400';
    });
}