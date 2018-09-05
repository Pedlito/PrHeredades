$.validator.addMethod(
    "soloLetras",
    function (value, element) {
        return /^[a-záéíóúnñ' ]+$/ig.test(value);
    },
    "Solo se permiten letras"
);

$.validator.addMethod(
    "soloNumeros",
    function (value, element) {
        return /^[0-9- ]*$/ig.test(value);
    },
    "Solo se permiten números"
);

$("#formProveedor").validate({
    errorClass: 'text-danger',
    errorElement: 'li',
    wrapper: 'ul',
    rules: {
        proveedor: {
            required: true,
            minlength: 3,
        },
        telefono: {
            soloNumeros: true
        }
    },
    messages: {
        proveedor: {
            required: "Ingrese el nombre del proveedor",
            minlength: jQuery.validator.format("Al menos {0} caracteres requeridos!")
        }
    }
});
