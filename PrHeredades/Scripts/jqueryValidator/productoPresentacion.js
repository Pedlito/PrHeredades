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
        precio: {
            required: true,
            number: true
        },
        unidades: {
            required: true,
            entero: true
        }
    },
    messages: {
        precio: {
            required: "Especifique el precio",
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
        editPrecio: {
            required: true,
            number: true
        },
        editUnidades: {
            required: true,
            entero: true
        }
    },
    messages: {
        editPrecio: {
            required: "Especifique el precio",
            number: "Ingrese un número valido"
        },
        editUnidades: {
            required: "Especifique las unidades"
        }
    }
});