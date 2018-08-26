$.validator.addMethod(
    "soloLetras",
    function (value, element) {
        return /^[a-záéíóúnñ' ]+$/ig.test(value);
    },
    "Solo se permiten letras"
);

$.validator.addMethod(
    "categoria",
    function (value, element, argumento) {
        return argumento !== value;
    },
    "No a seleccionado una categoría"
);


$("#formProducto").validate({
    errorClass: 'text-danger',
    errorElement: 'li',
    wrapper: 'ul',
    rules: {
        producto: {
            required: true,
            minlength: 3,
        },
        codCategoria: {
            categoria: ""
        }
    },
    messages: {
        producto: {
            required: "Ingrese el nombre del producto",
            minlength: jQuery.validator.format("Al menos {0} caracteres requeridos!")
        }
    }
});
