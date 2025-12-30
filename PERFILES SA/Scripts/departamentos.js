$(document).ready(function () {
    inicializarDepartamentos();
});

function inicializarDepartamentos() {
    $('#tblDepartamentos').DataTable({
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.25/i18n/Spanish.json"
        },
        "pageLength": 10,
        "responsive": true
    });

    $('.nombre-departamento').on('blur', validarNombreDepartamento);
}

function validarNombreDepartamento() {
    const txtNombre = $(this);
    const hdnDepartamentoId = $('#hdnDepartamentoId');

    if (!txtNombre.val().trim()) return;

    const nombre = txtNombre.val().trim();
    const departamentoId = hdnDepartamentoId.val() || null;

    $.ajax({
        type: "POST",
        url: "/WebServices/DepartamentoService.asmx/ValidarNombreDepartamento",
        data: JSON.stringify({
            nombre: nombre,
            excludeDepartamentoId: departamentoId
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            const esValido = response.d;

            if (!esValido) {
                mostrarErrorCampo('errorNombre', 'Ya existe un departamento con este nombre');
                txtNombre.focus();
            } else {
                limpiarErrorCampo('errorNombre');
            }
        },
        error: function () {
            console.error("Error al validar nombre de departamento");
        }
    });
}

function mostrarErrorCampo(campoId, mensaje) {
    $('#' + campoId).text(mensaje).show();
    $('#' + campoId).closest('.mb-3').find('.form-control').addClass('is-invalid');
}

function limpiarErrorCampo(campoId) {
    $('#' + campoId).text('').hide();
    $('#' + campoId).closest('.mb-3').find('.form-control').removeClass('is-invalid');
}