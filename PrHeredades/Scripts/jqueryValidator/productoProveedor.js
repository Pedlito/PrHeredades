$.validator.addMethod(
    "producto",
    function (value, element, argumento) {
        return argumento !== value;
    },
    "No a seleccionado un producto"
);

$.validator.addMethod(
    "presentacion",
    function (value, element, argumento) {
        return argumento !== value;
    },
    "No a seleccionado una presentacion"
);

$("#agrProducto").validate({
    errorClass: 'text-danger',
    errorElement: 'li',
    wrapper: 'ul',
    rules: {
        codProducto: {
            producto: ""
        },
        codPresentacion: {
            presentacion: ""
        },
        precioCompra: {
            required: true,
            number: true
        }
    },
    messages: {
        precioCompra: {
            required: "Especifique el precio de compra",
            number: "Ingrese un número valido"
        }
    }
});

$("#editProducto").validate({
    errorClass: 'text-danger',
    errorElement: 'li',
    wrapper: 'ul',
    rules: {
        editPrecioCompra: {
            required: true,
            number: true
        }
    },
    messages: {
        editPrecioCompra: {
            required: "Especifique el precio de compra",
            number: "Ingrese un número valido"
        }
    }
});