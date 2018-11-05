$("#formTransaccion").validate({
    errorClass: 'text-danger',
    errorElement: 'li',
    wrapper: 'ul',
    rules: {
        cantidad: {
            required: true,
            number: true
        }
    },
    messages: {
        cantidad: {
            required: "Ingrese el monto a pagar",
            number: "Ingrese un número valido"
        }
    }
});