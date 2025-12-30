using PERFILES_SA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PERFILES_SA.Repositories
{
    public interface IEmpleadoRepository
    {
        RespuestaOperacion Insertar(Empleado empleado);
        RespuestaOperacion Actualizar(Empleado empleado);
        RespuestaOperacion Desactivar(int empleadoId);
        RespuestaOperacion Activar(int empleadoId);
        Empleado ObtenerPorId(int empleadoId);
        List<Empleado> ObtenerTodos();
        List<Empleado> ObtenerPorDepartamento(int departamentoId);
        List<Empleado> ObtenerReporte(FiltroEmpleados filtro);
        bool ExisteDPI(string dpi, int? excludeEmpleadoId = null);
    }
}
