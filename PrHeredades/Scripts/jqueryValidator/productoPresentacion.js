$.validator.addMethod(
    "presentacion",
    function (value, element, argumento) {
        return argumento !== value;
    },
    "No a seleccionado una presentación"
);

$.validator.addMethod(
    "entero",
    function (value, element) {
        return /^\d*[0-9]$/ig.test(value);
    },
    "Ingrese un número entero"
);


$("#agrPresentacion").validate({
    errorClass: 'text-danger',
    errorElement: 'li',
    wrapper: 'ul',
    rules: {
        codPresentacion: {
            presentacion: ""
        },
        precioVenta: {
            required: true,
            number: true
        },
        unidades: {
            required: true,
            entero: true
        }
    },
    messages: {
        precioVenta: {
            required: "Especifique el precio de venta",
            number: "Ingrese un número valido"
        },
        unidades: {
            required: "Especifique las unidades"
        }
    }
});

$("#editPresentacion").validate({
    errorClass: 'text-danger',
    errorElement: 'li',
    wrapper: 'ul',
    rules: {
        editPrecioVenta: {
            required: true,
            number: true
        },
        editUnidades: {
            required: true,
            entero: true
        }
    },
    messages: {
        editPrecioVenta: {
            required: "Especifique el precio de venta",
            number: "Ingrese un número valido"
        },
        editUnidades: {
            required: "Especifique las unidades"
        }
    }
});