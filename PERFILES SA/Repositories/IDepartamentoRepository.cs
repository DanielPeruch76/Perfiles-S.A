using PERFILES_SA.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PERFILES_SA.Repositories
{
    public interface IDepartamentoRepository
    {
        RespuestaOperacion Insertar(Departamento departamento);
        RespuestaOperacion Actualizar(Departamento departamento);
        RespuestaOperacion CambiarEstado(int departamentoId, bool activo);
        Departamento ObtenerPorId(int departamentoId);
        List<Departamento> ObtenerTodos();
        List<Departamento> ObtenerActivos();
        bool ExisteNombre(string nombre, int? excludeDepartamentoId = null);
    }
}
