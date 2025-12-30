$(document).ready(function () {
    inicializarReportes();
});

function inicializarReportes() {
    $('#tblReporte').DataTable({
        "dom": 'Bfrtip',
        "buttons": [
            'copy', 'csv', 'excel', 'pdf', 'print'
        ],
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.25/i18n/Spanish.json"
        },
        "pageLength": 25,
        "order": [[0, 'asc']]
    });

    $('.fecha-reporte').datepicker({
        format: 'yyyy-mm-dd',
        autoclose: true
    });

    $('#txtFechaFin').on('change', validarRangoFechas);
}

function cargarDepartamentosReporte() {
    $.ajax({
        type: "POST",
        url: "/WebServices/DepartamentoService.asmx/ObtenerDepartamentosActivos",
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            const ddlDepartamento = $('#<%= ddlDepartamentoReporte.ClientID %>');
            const departamentos = response.d;

            ddlDepartamento.empty();
            ddlDepartamento.append('<option value="">Todos los departamentos</option>');

            departamentos.forEach(function (depto) {
                ddlDepartamento.append(
                    $('<option></option>').val(depto.DepartamentoId).text(depto.Nombre)
                );
            });
        },
        error: function () {
            console.error("Error al cargar departamentos para reporte");
        }
    });
}

function validarRangoFechas() {
    const fechaInicio = $('#<%= txtFechaInicio.ClientID %>').val();
    const fechaFin = $('#<%= txtFechaFin.ClientID %>').val();

    if (fechaInicio && fechaFin) {
        const inicio = new Date(fechaInicio);
        const fin = new Date(fechaFin);

        if (fin < inicio) {
            alert('La fecha fin no puede ser menor que la fecha inicio');
            $('#<%= txtFechaFin.ClientID %>').val('');
            $('#<%= txtFechaFin.ClientID %>').focus();
            return false;
        }
    }
    return true;
}

function generarReporteAjax() {
    const departamentoId = $('#<%= ddlDepartamentoReporte.ClientID %>').val();
    const fechaInicio = $('#<%= txtFechaInicio.ClientID %>').val();
    const fechaFin = $('#<%= txtFechaFin.ClientID %>').val();

    $('#loadingReporte').show();

    $.ajax({
        type: "POST",
        url: "/WebServices/EmpleadoService.asmx/GenerarReporteEmpleados",
        data: JSON.stringify({
            departamentoId: departamentoId || null,
            fechaInicio: fechaInicio || null,
            fechaFin: fechaFin || null
        }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            $('#loadingReporte').hide();
            const empleados = response.d;

            if (empleados.length === 0) {
                $('#divResultados').html(
                    '<div class="alert alert-info">No hay datos para mostrar con los filtros seleccionados.</div>'
                );
                return;
            }

            let html = '<table class="table table-striped table-bordered">';
            html += '<thead><tr>';
            html += '<th>Empleado</th><th>DPI</th><th>Edad</th><th>Departamento</th><th>Ingreso</th><th>Estado</th>';
            html += '</tr></thead><tbody>';

            empleados.forEach(function (empleado) {
                html += '<tr>';
                html += '<td>' + empleado.Nombres + '</td>';
                html += '<td>' + empleado.DPI + '</td>';
                html += '<td>' + empleado.Edad + ' años</td>';
                html += '<td>' + empleado.NombreDepartamento + '</td>';
                html += '<td>' + new Date(empleado.FechaIngreso).toLocaleDateString() + '</td>';
                html += '<td><span class="badge ' + (empleado.Activo ? 'bg-success' : 'bg-danger') + '">';
                html += empleado.Activo ? 'Activo' : 'Inactivo';
                html += '</span></td>';
                html += '</tr>';
            });

            html += '</tbody></table>';
            $('#divResultados').html(html);

            actualizarEstadisticas(empleados);

            generarGraficoDepartamentos(empleados);
        },
        error: function () {
            $('#loadingReporte').hide();
            alert('Error al generar el reporte');
        }
    });
}

function actualizarEstadisticas(empleados) {
    const total = empleados.length;
    const edadPromedio = empleados.reduce((sum, emp) => sum + emp.Edad, 0) / total;
    const activos = empleados.filter(emp => emp.Activo).length;

    $('#lblTotal').text(total);
    $('#lblEdadPromedio').text(edadPromedio.toFixed(1));
    $('#lblActivos').text(activos);
    $('#lblInactivos').text(total - activos);
}

function generarGraficoDepartamentos(empleados) {
    const datos = {};
    empleados.forEach(function (emp) {
        if (!datos[emp.NombreDepartamento]) {
            datos[emp.NombreDepartamento] = 0;
        }
        datos[emp.NombreDepartamento]++;
    });

    const labels = Object.keys(datos);
    const valores = Object.values(datos);

    if (window.chartDepartamentos) {
        window.chartDepartamentos.destroy();
    }

    const ctx = document.getElementById('chartDepartamentos').getContext('2d');
    window.chartDepartamentos = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: labels,
            datasets: [{
                data: valores,
                backgroundColor: [
                    '#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0',
                    '#9966FF', '#FF9F40', '#C9CBCF', '#7CBF4F'
                ]
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: {
                    position: 'right',
                },
                title: {
                    display: true,
                    text: 'Distribución por Departamento'
                }
            }
        }
    });
}