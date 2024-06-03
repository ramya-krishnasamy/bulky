var dataTable;

$(document).ready(function () {
    onLoadDataTable();
});

function onLoadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/Admin/Product/GetAll' },
        "columns": [
            { data: 'title', "width": "25%" },
            { data: 'author', "width": "15%" },
            { data: 'category.categoryName', "width": "15%" },
            { data: 'listPrice', "width": "15%" },
            {
                data: "id",
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                       <a href="/admin/product/upsert?id=${data}" class="btn btn-primary mx-2"> Edit <i class="bi bi-pencil-square"></i>
                       <a onClick=Delete('/admin/product/DeleteProduct?id=${data}') class="btn btn-primary mx-2"> Delete <i class="bi bi-trash"></i>
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