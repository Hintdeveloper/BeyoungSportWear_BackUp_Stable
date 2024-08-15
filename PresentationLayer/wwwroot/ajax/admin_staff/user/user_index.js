function fetchData() {
    var xhr = new XMLHttpRequest();
    xhr.open('GET', 'https://localhost:7241/api/ApplicationUser/GetAllActiveInformationUserAsync', true);
    xhr.onreadystatechange = function () {
        if (xhr.readyState == 4 && xhr.status == 200) {
            var data = JSON.parse(xhr.responseText);
            displayData(data);
        } else if (xhr.readyState == 4) {
            console.error('Error fetching data:', xhr.statusText);
        }
    };
    xhr.send();
}

function displayData(data) {
    const tableBody = document.querySelector('#sampleTable tbody');
    tableBody.innerHTML = '';
    data.forEach(user => {
        const row = document.createElement('tr');

        row.innerHTML = `
            <td width="10"><input type="checkbox" name="check1" value="${user.id}"></td>
            <td>${user.id}</td>
            <td>${user.firstAndLastName}</td>
            <td>${user.email}</td>
            <td>${user.phoneNumber}</td>
            <td class="table-td-center">
                <button class="btn btn-primary btn-sm trash" type="button" title="Xóa" onclick="deleteFunction(this)">
                    <i class="fas fa-trash-alt"></i>
                </button>
                <button class="btn btn-primary btn-sm edit" type="button" title="Sửa" id="show-emp" data-toggle="modal" data-target="#ModalUP">
                    <i class="fas fa-edit"></i>
                </button>
            </td>
        `;
        tableBody.appendChild(row);
    });
}


document.addEventListener('DOMContentLoaded', fetchData);