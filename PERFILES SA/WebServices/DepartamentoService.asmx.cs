using PERFILES_SA.Models;
using PERFILES_SA.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace PERFILES_SA.WebServices
{

    [WebService(Namespace = "http://perfilessa.com/webservices/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class DepartamentoService : WebService
    {
        private readonly IDepartamentoService _departamentoService;

        public DepartamentoService()
        {
            _departamentoService = new PERFILES_SA.Services.DepartamentoService();
        }

   
        [WebMethod(Description = "Crea un nuevo departamento")]
        public RespuestaOperacion CrearDepartamento(string nombre, string descripcion = null)
        {
            try
            {
                var departamento = new Departamento
                {
                    Nombre = nombre,
                    Descripcion = descripcion
                };

                return _departamentoService.CrearDepartamento(departamento);
            }
            catch (Exception ex)
            {
                return RespuestaOperacion.Error($"Error en el servicio: {ex.Message}");
            }
        }


        [WebMethod(Description = "Actualiza un departamento")]
        public RespuestaOperacion ActualizarDepartamento(int departamentoId, string nombre, string descripcion = null)
        {
            try
            {
                var departamento = new Departamento
                {
                    DepartamentoId = departamentoId,
                    Nombre = nombre,
                    Descripcion = descripcion
                };

                return _departamentoService.ActualizarDepartamento(departamento);
            }
            catch (Exception ex)
            {
                return RespuestaOperacion.Error($"Error en el servicio: {ex.Message}");
            }
        }

        [WebMethod(Description = "Cambia estado de departamento")]
        public RespuestaOperacion CambiarEstadoDepartamento(int departamentoId, bool activo)
        {
            try
            {
                return _departamentoService.CambiarEstadoDepartamento(departamentoId, activo);
            }
            catch (Exception ex)
            {
                return RespuestaOperacion.Error($"Error en el servicio: {ex.Message}");
            }
        }


        [WebMethod(Description = "Obtiene departamento por ID")]
        public Departamento ObtenerDepartamentoPorId(int departamentoId)
        {
            try
            {
                return _departamentoService.ObtenerDepartamento(departamentoId);
            }
            catch
            {
                return null;
            }
        }

        [WebMethod(Description = "Obtiene todos los departamentos")]
        public List<Departamento> ObtenerTodosDepartamentos()
        {
            try
            {
                return _departamentoService.ObtenerTodosDepartamentos();
            }
            catch
            {
                return new List<Departamento>();
            }
        }

        [WebMethod(Description = "Obtiene departamentos activos")]
        public List<Departamento> ObtenerDepartamentosActivos()
        {
            try
            {
                return _departamentoService.ObtenerDepartamentosActivos();
            }
            catch
            {
                return new List<Departamento>();
            }
        }

 
        [WebMethod(Description = "Valida nombre único de departamento")]
        public bool ValidarNombreDepartamento(string nombre, int? excludeDepartamentoId = null)
        {
            try
            {
                return _departamentoService.ValidarNombre(nombre, excludeDepartamentoId);
            }
            catch
            {
                return false;
            }
        }
    }
}
