var docDetailId, dtTable;
$("document").ready(function () {
    debugger;
    var urlParams = new URLSearchParams(window.location.search);
    var docDetailId = urlParams.get('id');
    $("#docDetailId").val(docDetailId);
    loadDocumentsRelatedPin(docDetailId);
});


function loadDocumentsRelatedPin(docDetailId) {
    debugger;
    $.get("/api/Document/viewAllDocsRelatedtoPIN?docDetailId=" + docDetailId).done(function (data) {
        $("#lblName").html(data.name);
        $("#lblDescription").html(data.description);
        $("#lblPropertyIdentityNumber").html(data.propertyIdentityNumber);
        loadDataTableData(data.documents);
    }).fail(function (error) {
        alert(error.responseJSON);
    })
}

function loadDataTableData(data) {
    debugger;
    dtTable = $("#viewDocs").DataTable({
        "filter": false,
        info: true,
        ordering: false,
        paging: false,
        "aaData": data,
        "columns": [
            {
                "data": "fileName", "name": "File Name", "autoWidth": true
            },
            {
                "data": "fileType", "name": "File Type", "autoWidth": true
            },
            {
                "data": "id", "name": "View Document", "autoWidth": true,
                "render": function (data, row) {
                    var html = '<button type="button"  id="' + data + '" onclick="ViewDocumentContent(id)" class="btn btn-outline-primary"><i class="bi bi-eye-fill"></i></button>';
                    return html;
                }
            },
            {
                "data": "fileUrl", "name": "Download", "autoWidth": true,
                "render": function (data, row) {
                    let filepath = "/uploads/" + data;
                    var html = '<a href="' + filepath + '"   download><i class="bi bi-download" style="font-size: 24px;"></i></a>';
                    return html;
                }
            },

        ],
        "rowCallback": function (row, data) {
            let filepath = "/uploads/" + data.fileUrl;
            $('td:eq(0)', row).html('<a href="' + filepath + '" target="_blank">' + data.fileName + '</a>');
            let filetype = data.fileName.split('.').pop();
            $('td:eq(1)', row).html(filetype);
        }
    });



}
function DownloadAllFiles() {
    var urlParams = new URLSearchParams(window.location.search);
    var docDetailId = urlParams.get('id');
    var pin = $("#lblPropertyIdentityNumber").html();
    DownloadDocuments(docDetailId, pin);
}
function showList() {
    location.href = "/Document/ListView";
}
function ViewDocumentContent(docId) {
    $.get("/api/Document/getDocumentContent?docId=" + docId, function (response) {
        debugger;
        var base64Content = response.fileContent;
        var documentViewer = $('#documentViewer');
        var contentType = response.filetype;
        if (contentType === 'application/pdf' || contentType.startsWith('image/') || contentType === 'text/plain') {
            documentViewer.attr('src', 'data:' + contentType + ';base64,' + base64Content);
        }
        else {
            $("#iframediv").hide();
            alert('Unsupported file type');
            return;
        }
        $("#iframediv").show();
    });
}

function Removeiframe() {
    $("#iframediv").hide();
}