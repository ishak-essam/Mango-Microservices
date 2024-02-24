var dataTable;

$(document).ready(function () {
    var url = window.location.search;
    if (url.includes("approved")) {
        loadDataTable("approved");
    }
    else {
        if (url.includes("readyforpickup")) {
            loadDataTable("readyforpickup");
        }
        else {
            if (url.includes("cancelled")) {
                loadDataTable("cancelled");
            }
            else {
                loadDataTable("all");
            }
        }
    }
});

function loadDataTable(status) {
    dataTables = $('#tblData').DataTable({
        "ajax": { url: "/order/getall?status=" + status },
        order: [[0, 'desc']],
        "columns": [
            { data: 'orderHeaderId', "width": "3%" },
            { data: 'email', "width": "5%" },
            { data: 'name', "width": "5%" },
            { data: 'phone', "width": "3%" },
            { data: 'status', "width": "3%" },
            { data: 'orderTotal', "width": "3%" },
            {
                data: 'orderHeaderId',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                    <a href="/Order/orderDetail?orderId=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i></a>
                    </div>`
                },
                "width": "10%"
            }
        ],
    });
}
