
var dataTables;
$(document).ready(function () {
        loadDataTable();
});
function LoadDataTable() {
    dataTables = $('tbData').DataTable({
        "ajax": { url: "/order/getall" +  },
        "columns": [
            { data: 'OrderHeaderId', "width": "5%" },
            { data: 'Email', "width": "25%" },
            { data: 'Name', "width": "20%" },
            { data: 'Phone', "width": "10%" },
            { data: 'Status', "width": "10%" },
            { data: 'OrderTotal', "width": "10%" },
            {
                data: 'OrderHeaderId',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                    <a href="/order/orderDetail?orderId=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i></a>
                    </div>`
                },
                "width": "10%"
            }
        ],
    });
}