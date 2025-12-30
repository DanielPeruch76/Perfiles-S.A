$(document).ready(function () {
    inicializarValidaciones();
});

function inicializarValidaciones() {
    $('.campo-requerido').on('blur', validarCampoRequerido);

    $('.campo-numero').on('blur', validarNumero);

    $('.campo-email').on('blur', validarEmail);

    $('.campo-fecha').on('blur', validarFecha);

    $('input, select').on('keypress', function (e) {
        if (e.which === 13 && !$(this).is('textarea')) {
            e.preventDefault();
            return false;
        }
    });
}

function validarCampoRequerido() {
    const campo = $(this);
    const valor = campo.val().trim();

    if (!valor) {
        campo.addClass('is-invalid');
        campo.next('.invalid-feedback').text('Este campo es requerido').show();
        return false;
    } else {
        campo.removeClass('is-invalid');
        campo.addClass('is-valid');
        campo.next('.invalid-feedback').hide();
        return true;
    }
}

function validarNumero() {
    const campo = $(this);
    const valor = campo.val().trim();

    if (valor && !/^\d+(\.\d+)?$/.test(valor)) {
        campo.addClass('is-invalid');
        campo.next('.invalid-feedback').text('Debe ser un número válido').show();
        return false;
    } else {
        campo.removeClass('is-invalid');
        campo.addClass('is-valid');
        campo.next('.invalid-feedback').hide();
        return true;
    }
}

function validarEmail() {
    const campo = $(this);
    const valor = campo.val().trim();
    const regex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

    if (valor && !regex.test(valor)) {
        campo.addClass('is-invalid');
        campo.next('.invalid-feedback').text('Email inválido').show();
        return false;
    } else {
        campo.removeClass('is-invalid');
        campo.addClass('is-valid');
        campo.next('.invalid-feedback').hide();
        return true;
    }
}

function validarFecha() {
    const campo = $(this);
    const valor = campo.val().trim();

    if (valor) {
        const fecha = new Date(valor);
        if (isNaN(fecha.getTime())) {
            campo.addClass('is-invalid');
            campo.next('.invalid-feedback').text('Fecha inválida').show();
            return false;
        }
    }

    campo.removeClass('is-invalid');
    campo.addClass('is-valid');
    campo.next('.invalid-feedback').hide();
    return true;
}

function validarFormularioCompleto(formId) {
    let esValido = true;
    const formulario = $('#' + formId);

    formulario.find('.campo-requerido').each(function () {
        if (!validarCampoRequerido.call(this)) {
            esValido = false;
        }
    });

    formulario.find('.campo-numero').each(function () {
        if (!validarNumero.call(this)) {
            esValido = false;
        }
    });

    formulario.find('.campo-email').each(function () {
        if (!validarEmail.call(this)) {
            esValido = false;
        }
    });

    formulario.find('.campo-fecha').each(function () {
        if (!validarFecha.call(this)) {
            esValido = false;
        }
    });

    return esValido;
}

function limpiarValidaciones(formId) {
    const formulario = $('#' + formId);

    formulario.find('.is-valid, .is-invalid').removeClass('is-valid is-invalid');
    formulario.find('.invalid-feedback').hide();
}

function mostrarMensajeError(elementoId, mensaje) {
    const elemento = $('#' + elementoId);
    elemento.text(mensaje);
    elemento.show();

    setTimeout(function () {
        elemento.fadeOut();
    }, 5000);
}

function mostrarMensajeExito(elementoId, mensaje) {
    const elemento = $('#' + elementoId);
    elemento.text(mensaje);
    elemento.removeClass('alert-danger').addClass('alert-success').show();

    setTimeout(function () {
        elemento.fadeOut();
    }, 5000);
}

function formatearMoneda(valor) {
    return new Intl.NumberFormat('es-GT', {
        style: 'currency',
        currency: 'GTQ',
        minimumFractionDigits: 2
    }).format(valor);
}

function formatearFecha(fecha, incluirHora = false) {
    if (!fecha) return '';

    const fechaObj = new Date(fecha);
    const opciones = {
        year: 'numeric',
        month: '2-digit',
        day: '2-digit'
    };

    if (incluirHora) {
        opciones.hour = '2-digit';
        opciones.minute = '2-digit';
    }

    return fechaObj.toLocaleDateString('es-GT', opciones);
}

function confirmarAccion(mensaje, titulo = 'Confirmar') {
    return new Promise((resolve) => {
        Swal.fire({
            title: titulo,
            text: mensaje,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Sí, continuar',
            cancelButtonText: 'Cancelar'
        }).then((result) => {
            resolve(result.isConfirmed);
        });
    });
}