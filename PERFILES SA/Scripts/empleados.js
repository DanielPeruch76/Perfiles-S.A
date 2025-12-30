$(document).ready(function () {
    inicializarComponentes();
});

function inicializarComponentes() {
    $('.datepicker').datepicker({
        format: 'yyyy-mm-dd',
        autoclose: true,
        todayHighlight: true,
        language: 'es'
    });

    $('.dpi-mask').inputmask('9999999999999');
    $('.nit-mask').inputmask('99999999-9');

    $('[data-toggle="tooltip"]').tooltip();
}

function cargarDepartamentos() {
    const ddlDepartamento = document.getElementById('<%= ddlDepartamento.ClientID %>');
    if (!ddlDepartamento) return;

    $.ajax({
        type: "POST",
        url: "/WebServices/DepartamentoService.asmx/ObtenerDepartamentosActivos",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            const departamentos = response.d;
            ddlDepartamento.innerHTML = '<option value="">Seleccionar Departamento</option>';

            departamentos.forEach(function (depto) {
                const option = document.createElement('option');
                option.value = depto.DepartamentoId;
                option.textContent = depto.Nombre;
                ddlDepartamento.appendChild(option);
            });
        },
        error: function (xhr, status, error) {
            console.error("Error al cargar departamentos:", error);
            mostrarError("Error al cargar departamentos. Intente nuevamente.");
        }
    });
}

function validarFormulario() {
    let esValido = true;
    const errores = [];

    const dpi = document.getElementById('<%= txtDPI.ClientID %>').value.trim();
    if (!dpi) {
        mostrarErrorCampo('errorDPI', 'El DPI es requerido');
        esValido = false;
    } else if (dpi.length < 13) {
        mostrarErrorCampo('errorDPI', 'El DPI debe tener 13 caracteres');
        esValido = false;
    } else {
        limpiarErrorCampo('errorDPI');
    }

    const nombres = document.getElementById('<%= txtNombres.ClientID %>').value.trim();
    if (!nombres) {
        mostrarErrorCampo('errorNombres', 'Los nombres son requeridos');
        esValido = false;
    } else {
        limpiarErrorCampo('errorNombres');
    }

    const fechaNacimiento = document.getElementById('<%= txtFechaNacimiento.ClientID %>').value;
    if (!fechaNacimiento) {
        mostrarErrorCampo('errorFechaNacimiento', 'La fecha de nacimiento es requerida');
        esValido = false;
    } else {
        const fechaNac = new Date(fechaNacimiento);
        const hoy = new Date();
        const edadMinima = new Date(hoy.getFullYear() - 18, hoy.getMonth(), hoy.getDate());

        if (fechaNac > edadMinima) {
            mostrarErrorCampo('errorFechaNacimiento', 'Debe ser mayor de 18 años');
            esValido = false;
        } else {
            limpiarErrorCampo('errorFechaNacimiento');
        }
    }

    const sexo = document.getElementById('<%= ddlSexo.ClientID %>').value;
    if (!sexo) {
        mostrarErrorCampo('errorSexo', 'El sexo es requerido');
        esValido = false;
    } else {
        limpiarErrorCampo('errorSexo');
    }

    const fechaIngreso = document.getElementById('<%= txtFechaIngreso.ClientID %>').value;
    if (!fechaIngreso) {
        mostrarErrorCampo('errorFechaIngreso', 'La fecha de ingreso es requerida');
        esValido = false;
    } else {
        const fechaIng = new Date(fechaIngreso);
        const hoy = new Date();

        if (fechaIng > hoy) {
            mostrarErrorCampo('errorFechaIngreso', 'No puede ser fecha futura');
            esValido = false;
        } else {
            limpiarErrorCampo('errorFechaIngreso');
        }
    }

    const departamento = document.getElementById('<%= ddlDepartamento.ClientID %>').value;
    if (!departamento) {
        mostrarErrorCampo('errorDepartamento', 'El departamento es requerido');
        esValido = false;
    } else {
        limpiarErrorCampo('errorDepartamento');
    }

    if (fechaNacimiento && fechaIngreso) {
        const fechaNac = new Date(fechaNacimiento);
        const fechaIng = new Date(fechaIngreso);

        if (fechaIng < fechaNac) {
            mostrarErrorCampo('errorFechaIngreso', 'No puede ser anterior a la fecha de nacimiento');
            esValido = false;
        }
    }

    return esValido;
}

function calcularEdad() {
    const fechaNacimiento = document.getElementById('<%= txtFechaNacimiento.ClientID %>');
    const lblEdad = document.getElementById('lblEdad');

    if (!fechaNacimiento || !fechaNacimiento.value || !lblEdad) return;

    const fechaNac = new Date(fechaNacimiento.value);
    const hoy = new Date();

    let edad = hoy.getFullYear() - fechaNac.getFullYear();
    const mes = hoy.getMonth() - fechaNac.getMonth();

    if (mes < 0 || (mes === 0 && hoy.getDate() < fechaNac.getDate())) {
        edad--;
    }

    lblEdad.textContent = edad + ' años';

    if (edad < 18) {
        lblEdad.className = 'badge bg-danger';
    } else if (edad < 30) {
        lblEdad.className = 'badge bg-success';
    } else if (edad < 50) {
        lblEdad.className = 'badge bg-primary';
    } else {
        lblEdad.className = 'badge bg-warning';
    }
}

function calcularTiempoLaborando() {
    const fechaIngreso = document.getElementById('<%= txtFechaIngreso.ClientID %>');
    const lblTiempo = document.getElementById('lblTiempoLaborando');

    if (!fechaIngreso || !fechaIngreso.value || !lblTiempo) return;

    const fechaIng = new Date(fechaIngreso.value);
    const hoy = new Date();

    let años = hoy.getFullYear() - fechaIng.getFullYear();
    let meses = hoy.getMonth() - fechaIng.getMonth();

    if (meses < 0) {
        años--;
        meses += 12;
    }

    let texto = '';
    if (años > 0) {
        texto += años + ' año' + (años !== 1 ? 's' : '');
    }
    if (meses > 0) {
        if (texto) texto += ', ';
        texto += meses + ' mes' + (meses !== 1 ? 'es' : '');
    }

    if (!texto) {
        texto = 'Menos de 1 mes';
    }

    lblTiempo.textContent = texto;
}

function validarDPIUnico() {
    const txtDPI = document.getElementById('<%= txtDPI.ClientID %>');
    const errorDPI = document.getElementById('errorDPI');
    const hdnEmpleadoId = document.getElementById('<%= hdnEmpleadoId.ClientID %>');

    if (!txtDPI || !txtDPI.value.trim()) return;

    const dpi = txtDPI.value.trim();
    const empleadoId = hdnEmpleadoId ? hdnEmpleadoId.value : null;

    $.ajax({
        type: "POST",
        url: "/WebServices/EmpleadoService.asmx/ValidarDPI",
        data: JSON.stringify({
            dpi: dpi,
            excludeEmpleadoId: empleadoId
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            const esValido = response.d;

            if (!esValido) {
                mostrarErrorCampo('errorDPI', 'El DPI ya está registrado en el sistema');
                txtDPI.focus();
            } else {
                limpiarErrorCampo('errorDPI');
            }
        },
        error: function () {
            console.error("Error al validar DPI");
        }
    });
}

function mostrarErrorCampo(idElemento, mensaje) {
    const elemento = document.getElementById(idElemento);
    if (elemento) {
        elemento.textContent = mensaje;
        elemento.style.display = 'block';
    }
}

function limpiarErrorCampo(idElemento) {
    const elemento = document.getElementById(idElemento);
    if (elemento) {
        elemento.textContent = '';
        elemento.style.display = 'none';
    }
}

function mostrarError(mensaje) {
    alert(mensaje);
}

function formatearNumero(numero) {
    return numero.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

function confirmarAccion(mensaje) {
    return confirm(mensaje || '¿Está seguro de realizar esta acción?');
}