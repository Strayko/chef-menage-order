let dataTable;

$(document).ready(function () {
    loadList();
});

function loadList() {
    dataTable = $('#DT_load').DataTable({
        "ajax": {
            "url": "/api/user",
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "fullName", "width": "25%" },
            { "data": "email", "width": "25%" },
            { "data": "phoneNumber", "width": "25%" },
            {
                "data": { id: "id", lockoutEnd: "lockoutEnd" },
                "render": function (data) {
                    let today = new Date().getTime();
                    let lockout = new Date(data.lockoutEnd).getTime();
                    if (lockout > today) {
                        //currently user is locked
                        return `<div class="text-center">
                        <a  class="btn btn-danger text-white" style="cursor: pointer; width: 30%;" onclick=LockUnlock('${data.id}')>
                            <i class="far fa-edit"></i> Edit
                        </a>`;
                    }
                    return `<div class="text-center">
                        <a href="/Admin/category/upsert?id=${data}" class="btn btn-success text-white" style="cursor: pointer; width: 30%;">
                            <i class="far fa-edit"></i> Edit
                        </a>
                        <a class="btn btn-danger text-white" style="cursor: pointer; width: 30%;" onclick=Delete('/api/category/'+${data})>
                            <i class="far fa-trash-alt"></i> Delete
                        </a>
                    </div>`;
                }, "width": "30%"
            }
        ],
        "language": {
            "emptyTable": "no data found."
        },
        "width": "100%"
    });
}

function Delete(url) {
    swal({
        title: "Are you sure you want to Delete?",
        text: "You will not be able to restore data!",
        icon: "warning",
        buttons: true,
        dangerMode: true
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: 'DELETE',
                url: url,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        dataTable.ajax.reload();
                    } else {
                        toastr.error(data.message);
                    }
                }
            });
        }
    });
}