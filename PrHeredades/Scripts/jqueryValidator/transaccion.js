
$("#formTransaccion").validate({
    errorClass: 'text-danger',
    errorElement: 'li',
    wrapper: 'ul',
    rules: {
        descripcion: {
            required: true
        }
    },
    messages: {
        descripcion: {
            required: "Ingrese una descripción"
        }
    }
});