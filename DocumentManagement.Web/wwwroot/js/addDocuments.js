
$(document).ready(function () {
    validateFormCustom('btnSave', 'addDocumentform', saveDocument);
})
function saveDocument() {
    var model = new FormData();
    var nooffiles = $("#document").get(0).files;

    if (nooffiles.length > 0) {
        for (let i = 0; i < nooffiles.length; i++) {
            model.append("files", nooffiles[i]);
        }
    }
    var documentDetails = {
        Name: $("#name").val(),
        Description: $("#description").val(),
        PropertyIdentityNumber: $("#pin").val()
    }
    model.append("documentDetails", JSON.stringify(documentDetails));

    $.ajax({
        url: "/api/Document/addDocument",
        type: "POST",
        data: model,
        success: saveDocumentSuccess,
        error: saveDocumentError,
        contentType: false,
        processData: false,
    });
}

function saveDocumentSuccess(success) {
    if (success) {
        $("#addDocumentform").get(0).reset();
        location.href = "/Document/ListView";
        alert("Documents info saved Successfully");

    } else {
        alert("Unable to save the Documents");
    }
}
function saveDocumentError(error) {
    alert(error.responseText);
}

function showList() {
    location.href = "/Document/ListView";
}