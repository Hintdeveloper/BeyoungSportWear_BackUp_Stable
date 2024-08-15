function getCookie(name) {
    const cookies = document.cookie.split(';');
    for (const cookie of cookies) {
        const [cookieName, cookieValue] = cookie.trim().split('=');
        if (cookieName === name) {
            return cookieValue;
        }
    }
    return null;
}

function checkJWTAndUpdateUI() {
    var token = getCookie("jwt");

    if (token === null) {
        document.getElementById('function_view').style.display = 'none';
        document.getElementById('login_message').style.display = 'block';
    } else {
        var tokenData = parseJwt(token);

        var tokenExpiration = tokenData.exp * 1000;
        var currentTimestamp = new Date().getTime();
        if (currentTimestamp >= tokenExpiration) {
            removeCookie("jwt");
            document.getElementById('function_view').style.display = 'none';
            document.getElementById('login_message').style.display = 'block';
        } else {
            document.getElementById('function_view').style.display = 'block';
            document.getElementById('login_message').style.display = 'none';
        }
    }
}

function removeCookie(name) {
    document.cookie = `${name}=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/;`;
}

function parseJwt(token) {
    var base64Url = token.split('.')[1];
    var base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    var jsonPayload = decodeURIComponent(atob(base64).split('').map(function (c) {
        return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
    }).join(''));
    return JSON.parse(jsonPayload);
}

document.addEventListener('DOMContentLoaded', checkJWTAndUpdateUI);
