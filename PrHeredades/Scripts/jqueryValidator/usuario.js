$.validator.addMethod(
    "soloLetras",
    function (value, element) {
        return /^[a-záéíóúnñ' ]+$/ig.test(value);
    },
    "Solo se permiten letras"
);

$.validator.addMethod(
    "noEspacios",
    function (value, element) {
        return !/\s/ig.test(value);
    },
    "No se permiten espacios en blanco"
);


$().ready(function () {
    $("#formUsuario").validate({
        errorClass: 'text-danger',
        errorElement: 'li',
        wrapper: 'ul',
        rules: {
            usuario: {
                required: true,
                minlength: 4,
                noEspacios: true
            },
            password: {
                required: true,
                minlength: 4
            },
            nombre: {
                required: true,
                minlength: 10,
                soloLetras: true
            },
            repetir: {
                required: true,
                minlength: 4,
                equalTo: "#password"
            }
        },
        messages: {
            usuario: {
                required: "El usuario es obligatorio",
                minlength: jQuery.validator.format("Al menos {0} caracteres requeridos!") 
            },
            password: {
                required: "La contraseña es obligatoria",
                minlength: jQuery.validator.format("Al menos {0} caracteres requeridos!")
            },
            nombre: {
                required: "El nombre es obligatorio",
                minlength: jQuery.validator.format("Al menos {0} caracteres requeridos!")
            },
            repetir: {
                required: "Repita la contraseña",
                minlength: jQuery.validator.format("Al menos {0} caracteres requeridos!"),
                equalTo: "Las contraseñas no coinciden"
            }
        }
    });
});