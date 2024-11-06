(() => {
    'use strict'
    // Fetch all the forms we want to apply custom Bootstrap validation styles to
    const forms = document.querySelectorAll('.needs-validation')

    // Loop over them and prevent submission
    Array.from(forms).forEach(form => {
        form.addEventListener('submit', event => {
            if (!form.checkValidity()) {
                event.preventDefault()
                event.stopPropagation()
            }

            form.classList.add('was-validated')
        }, false)
    })
})();
function validateFormCustom(buttonid, formid, savefunction) {
    const customButton = document.getElementById(buttonid);
    const form = document.getElementById(formid);
    customButton.addEventListener('click', event => {
        if (form.checkValidity()) {
            savefunction();
        } else {
            event.preventDefault();
            event.stopPropagation();
            form.classList.add('was-validated');
            alert("Enter all required details");
        }
    }, false);
}
