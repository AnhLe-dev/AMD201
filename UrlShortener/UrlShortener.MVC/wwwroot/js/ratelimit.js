document.addEventListener("DOMContentLoaded", function () {

    const createForm = document.getElementById("createLinkForm");

    if (createForm) {

        createForm.addEventListener("submit", function (e) {

            e.preventDefault();

            const spinner = document.getElementById("loadingSpinner");

            spinner.style.display = "flex";

            setTimeout(() => {
                createForm.submit();
            }, 5000); //5s

        });

    }

});