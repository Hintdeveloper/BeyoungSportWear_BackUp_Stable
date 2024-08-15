(function () {
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

    async function fetchUserInfo() {
        const apiUrl = `https://localhost:7241/api/ApplicationUser/GetInformationUser/${userId}`;
        try {
            const response = await fetch(apiUrl, {
                method: 'GET',
                headers: {
                    'Authorization': `Bearer ${jwt}`,
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error(`HTTP error! Status: ${response.status}`);
            }

            const userData = await response.json();

            document.getElementById('name_user').innerText = userData.firstAndLastName;
            document.getElementById('image_user').src = userData.images;

        } catch (error) {
            console.error('Fetch error:', error);
        }
    }

    fetchUserInfo();
})();
