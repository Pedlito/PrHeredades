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

$("#formDeudor").validate({
    errorClass: 'text-danger',
    errorElement: 'li',
    wrapper: 'ul',
    rules: {
        nombre: {
            required: true,
            soloLetras: true,
            minlength: 3
        },
        telefono: {
            required: true,
            soloNumeros: true
        }
    },
    messages: {
        nombre: {
            required: "Ingrese el nombre del cliente",
            minlength: jQuery.validator.format("Al menos {0} caracteres requeridos!")
        },
        telefono: {
            required: "Ingrese el número telefónico del cliente",
        }
    }
});
