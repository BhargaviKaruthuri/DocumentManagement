var dtTable;
$(document).ready(function () {
    debugger;
    loadDocumentsListDataTable();
});

function loadDocumentsListDataTable() {
    dtTable = $("#DocumentsDatatable").DataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "ajax": {
            "url": "/api/Document/getAllDocuments",
            "type": "POST",
            "datatype": "json",
            "error": function (jqXHR, textStatus, errorThrown) {
                alert(errorThrown);
            }
        },
        "columns": [
            { "data": "name", "name": "Name", "autoWidth": true },
            { "data": "propertyIdentityNumber", "name": "PropertyIdentityNumber", "autoWidth": true },
            { "data": "description", "name": "Description", "autoWidth": true },
            { "data": "createdBy", "name": "CreatedBy", "autoWidth": true },
            { "data": "updatedBy", "name": "UpdatedBy", "autoWidth": true },
            {
                "data": null,
                "sortable": false,
                "render": function (data, row) {

                    var html = '<div>';
                    html += '<button type="button"  id="' + data.id + '" onclick="ViewDocument(id)" class="btn btn-outline-primary" title="View"><i class="bi bi-eye-fill"></i></button>&nbsp;&nbsp;';
                    html += '<button type="button"  id="' + data.id + '" name="' + data.propertyIdentityNumber + '"  onclick="DownloadDocuments(id,name)" class="btn btn-outline-primary" title="Download"><i class="bi bi-arrow-down-square-fill"></i> </button>';
                    html += '</div>';
                    return html;

                }
            }
        ]
    })
}

function ViewDocument(id) {
    location.href = "/Document/ViewDocuments?id=" + id;
}

function DownloadDocuments(id, pin) {
    debugger;
    var form = $('<form></form>').attr({
        method: 'GET',
        action: '/api/Document/downloadAllDocsforPin',
    });
    $('<input>').attr({
        type: 'hidden',
        name: 'id',
        value: id
    }).appendTo(form);
    $('<input>').attr({
        type: 'hidden',
        name: 'pin',
        value: pin
    }).appendTo(form);
    form.appendTo('body').submit().remove();
}
