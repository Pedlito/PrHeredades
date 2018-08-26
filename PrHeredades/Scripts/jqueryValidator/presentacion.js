$.validator.addMethod(
    "soloLetras",
    function (value, element) {
        return /^[a-záéíóúnñ' ]+$/ig.test(value);
    },
    "Solo se permiten letras"
);



$("#formPresentacion").validate({
    errorClass: 'text-danger',
    errorElement: 'li',
    wrapper: 'ul',
    rules: {
        presentacion: {
            required: true,
            soloLetras: true
        }
    },
    messages: {
        presentacion: {
            required: "Ingrese la presentación"
        }
    }
});