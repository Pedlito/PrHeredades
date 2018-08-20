$.validator.addMethod(
    "soloLetras",
    function (value, element) {
        return /^[a-záéíóúnñ' ]+$/ig.test(value);
    },
    "Solo se permiten letras"
);



$("#formCategoria").validate({
    errorClass: 'text-danger',
    errorElement: 'li',
    wrapper: 'ul',
    rules: {
        categoria: {
            required: true,
            soloLetras: true
        }
    },
    messages: {
        categoria: {
            required: "Ingrese la categoría"
        }
    }
});