
$("#formPagoDeudor").validate({
    errorClass: 'text-danger',
    errorElement: 'li',
    wrapper: 'ul',
    rules: {
        pago: {
            required: true,
            number: true
        }
    },
    messages: {
        pago: {
            required: "Ingrese el monto a debitar",
            number: "Ingrese un número valido"
        }
    }
});