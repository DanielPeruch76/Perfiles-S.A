using PERFILES_SA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PERFILES_SA.Services
{
    public interface IDepartamentoService
    {
        RespuestaOperacion CrearDepartamento(Departamento departamento);
        RespuestaOperacion ActualizarDepartamento(Departamento departamento);
        RespuestaOperacion CambiarEstadoDepartamento(int departamentoId, bool activo);
        Departamento ObtenerDepartamento(int departamentoId);
        List<Departamento> ObtenerTodosDepartamentos();
        List<Departamento> ObtenerDepartamentosActivos();
        bool ValidarNombre(string nombre, int? excludeDepartamentoId = null);
    }
}
