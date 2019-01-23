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
        precioVentaMinimo: {
            required: true,
            number: true
        },
        precioVentaMedio: {
            required: true,
            number: true
        },
        precioVentaMaximo: {
            required: true,
            number: true
        },
        unidades: {
            required: true,
            entero: true
        },
        existencia: {
            required: true,
            number: true
        }
    },
    messages: {
        precioVentaMinimo: {
            required: "Especifique el precio de venta mínimo",
            number: "Ingrese un número valido"
        },
        precioVentaMedio: {
            required: "Especifique el precio de venta medio",
            number: "Ingrese un número valido"
        },
        precioVentaMaximo: {
            required: "Especifique el precio de venta máximo",
            number: "Ingrese un número valido"
        },
        unidades: {
            required: "Especifique las unidades"
        },
        existencia: {
            required: "Especifique la existencia inicial",
            number: "Ingrese un número valido"
        }
    }
});

$("#editPresentacion").validate({
    errorClass: 'text-danger',
    errorElement: 'li',
    wrapper: 'ul',
    rules: {
        editPrecioVentaMinimo: {
            required: true,
            number: true
        },
        editPrecioVentaMedio: {
            required: true,
            number: true
        },
        editPrecioVentaMaximo: {
            required: true,
            number: true
        },
        editUnidades: {
            required: true,
            entero: true
        },
        editExistencia: {
            required: true,
            number: true
        }
    },
    messages: {
        editPrecioVentaMinimo: {
            required: "Especifique el precio de venta mínimo",
            number: "Ingrese un número valido"
        },
        editPrecioVentaMedio: {
            required: "Especifique el precio de venta medio",
            number: "Ingrese un número valido"
        },
        editPrecioVentaMaximo: {
            required: "Especifique el precio de venta máximo",
            number: "Ingrese un número valido"
        },
        editUnidades: {
            required: "Especifique las unidades"
        },
        editExistencia: {
            required: "Especifique la existencia inicial",
            number: "Ingrese un número valido"
        }
    }
});