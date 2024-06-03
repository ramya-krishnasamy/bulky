var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("in_process")) {
        onLoadDataTable("in_process");
    } else if (url.includes("pending")) {
        onLoadDataTable("pending");
    } else if (url.includes("completed")) {
        onLoadDataTable("completed");
    } else if (url.includes("approved")) {
        onLoadDataTable("approved");
    } else if (url.includes("all")) {
        onLoadDataTable("all");
    }
});

function onLoadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/admin/order/getall?status=' + status },
        "columns": [
            { data: 'id', "width": "5%" },
            { data: 'name', "width": "15%" },
            { data: 'phoneNumber', "width": "15%" },
            { data: 'applicationUser.email', "width": "15%" },
            { data: 'orderStatus', "width": "15%" },
            { data: 'orderTotal', "width": "15%" },
            {
                data: "id",
                "render": function (data) {
                    return `<div class="w-100">
                       <a href="/admin/order/details?id=${data}" class="btn btn-primary mx-2"> Details <i class="bi bi-pencil-square"></i>
                    </div>`
                },
                "width": "25%"
            }
        ]
    });
}

function Delete(url) {
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function (data) {
                    dataTable.ajax.reload();
                    toastr.success(data.message);
                }
            })
        }
    });
}