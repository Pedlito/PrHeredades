$.validator.addMethod(
    "soloLetras",
    function (value, element) {
        return /^[a-záéíóúnñ' ]+$/ig.test(value);
    },
    "Solo se permiten letras"
);


//$().ready(function () {
$("#formRol").validate({
    errorClass: 'text-danger',
    errorElement: 'li',
    wrapper: 'ul',
    rules: {
        rol: {
            required: true,
            soloLetras: true
        }
    },
    messages: {
        rol: {
            required: "Ingrese un rol",
        }
    }
});
//});