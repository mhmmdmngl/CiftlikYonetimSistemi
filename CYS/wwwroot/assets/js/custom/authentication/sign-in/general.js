"use strict";

// Class definition
var KTSigninGeneral = function () {
    // Elements
    var form;
    var submitButton;
    var validator;

    // Handle form
    var handleForm = function (e) {
        // Init form validation rules. For more info check the FormValidation plugin's official documentation:https://formvalidation.io/
        validator = FormValidation.formValidation(
            form,
            {
                fields: {
                    'email': {
                        validators: {
                            notEmpty: {
                                message: 'Kullanıcı Adınız Gerekli'
                            }

                        }
                    },
                    'password': {
                        validators: {
                            notEmpty: {
                                message: 'Parola Gerekli'
                            }
                        }
                    }
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap5({
                        rowSelector: '.fv-row'
                    })
                }
            }
        );

        // Handle form submit
        submitButton.addEventListener('click', function (e) {
            // Prevent button default action
            e.preventDefault();

            // Validate form
            validator.validate().then(function (status) {
                if (status == 'Valid') {
                    // Show loading indication
                    submitButton.setAttribute('data-kt-indicator', 'on');

                    // Disable button to avoid multiple click 
                    submitButton.disabled = true;


                    // Simulate ajax request
                    setTimeout(function () {
                        // Hide loading indication
                        submitButton.removeAttribute('data-kt-indicator');

                        // Enable button
                        submitButton.disabled = false;

                        $.ajax({
                            url: "/Login/girisKontrolJson",
                            type: "POST",
                            data: {
                                "username": form.querySelector('[name="email"]').value, "password": form.querySelector('[name="password"]').value
                            },
                            success: function (returnData) {

                                if (returnData["status"] != "error") {
                                    Swal.fire({
                                        text: returnData["message"],
                                        icon: "error",
                                        buttonsStyling: false,
                                        confirmButtonText: "Tekrar Eklemeyi dene!",
                                        customClass: {
                                            confirmButton: "btn btn-primary"
                                        }
                                    });
                                    window.location.href = '/';

                                }
                                else {
                                    // Show message popup. For more info check the plugin's official documentation: https://sweetalert2.github.io/
                                    Swal.fire({
                                        text: "Başarı ile Giriş Yaptınız",
                                        icon: "success",
                                        buttonsStyling: false,
                                        confirmButtonText: "Yönlen",
                                        customClass: {
                                            confirmButton: "btn btn-primary"
                                        }
                                    }).then(function (result) {
                                        if (result.isConfirmed) {
                                            form.querySelector('[name="email"]').value = "";
                                            form.querySelector('[name="password"]').value = "";
                                            //form.submit(); // submit form
                                        }
                                    });
                                }
                            }
                        });


                       
                    }, 2000);
                } 
            });
        });
    }

    // Public functions
    return {
        // Initialization
        init: function () {
            form = document.querySelector('#kt_sign_in_form');
            submitButton = document.querySelector('#kt_sign_in_submit');

            handleForm();
        }
    };
}();

// On document ready
KTUtil.onDOMContentLoaded(function () {
    KTSigninGeneral.init();
});
