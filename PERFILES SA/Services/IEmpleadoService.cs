using PERFILES_SA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PERFILES_SA.Services
{
    public interface IEmpleadoService
    {
        RespuestaOperacion RegistrarEmpleado(Empleado empleado);
        RespuestaOperacion ActualizarEmpleado(Empleado empleado);
        RespuestaOperacion DesactivarEmpleado(int empleadoId);
        RespuestaOperacion ActivarEmpleado(int empleadoId);
        Empleado ObtenerEmpleado(int empleadoId);
        List<Empleado> ObtenerTodosEmpleados();
        List<Empleado> ObtenerEmpleadosPorDepartamento(int departamentoId);
        List<Empleado> GenerarReporte(FiltroEmpleados filtro);
        bool ValidarDPI(string dpi, int? excludeEmpleadoId = null);
        List<Departamento> ObtenerDepartamentosActivos();
    }
}
