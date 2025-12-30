using PERFILES_SA.Models;
using PERFILES_SA.Services;
using System;
using System.Collections.Generic;
using System.Web.Services;

namespace PERFILES_SA.WebServices
{

    [WebService(Namespace = "http://perfilessa.com/webservices/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class EmpleadoService : WebService
    {
        private readonly IEmpleadoService _empleadoService;

        public EmpleadoService()
        {
            _empleadoService = new PERFILES_SA.Services.EmpleadoService();
        }


        [WebMethod(Description = "Registra un nuevo empleado")]
        public RespuestaOperacion RegistrarEmpleado(
            string dpi,
            string nombres,
            DateTime fechaNacimiento,
            string sexo,
            DateTime fechaIngreso,
            string direccion,
            string nit,
            int departamentoId)
        {
            try
            {
                var empleado = new Empleado
                {
                    DPI = dpi,
                    Nombres = nombres,
                    FechaNacimiento = fechaNacimiento,
                    Sexo = sexo,
                    FechaIngreso = fechaIngreso,
                    Direccion = direccion,
                    NIT = nit,
                    DepartamentoId = departamentoId
                };

                return _empleadoService.RegistrarEmpleado(empleado);
            }
            catch (Exception ex)
            {
                return RespuestaOperacion.Error($"Error en el servicio: {ex.Message}");
            }
        }

        [WebMethod(Description = "Registra empleado con objeto completo")]
        public RespuestaOperacion RegistrarEmpleadoCompleto(Empleado empleado)
        {
            try
            {
                return _empleadoService.RegistrarEmpleado(empleado);
            }
            catch (Exception ex)
            {
                return RespuestaOperacion.Error($"Error en el servicio: {ex.Message}");
            }
        }


        [WebMethod(Description = "Actualiza datos de empleado")]
        public RespuestaOperacion ActualizarEmpleado(
            int empleadoId,
            string dpi,
            string nombres,
            DateTime fechaNacimiento,
            string sexo,
            DateTime fechaIngreso,
            string direccion,
            string nit,
            int departamentoId)
        {
            try
            {
                var empleado = new Empleado
                {
                    EmpleadoId = empleadoId,
                    DPI = dpi,
                    Nombres = nombres,
                    FechaNacimiento = fechaNacimiento,
                    Sexo = sexo,
                    FechaIngreso = fechaIngreso,
                    Direccion = direccion,
                    NIT = nit,
                    DepartamentoId = departamentoId
                };

                return _empleadoService.ActualizarEmpleado(empleado);
            }
            catch (Exception ex)
            {
                return RespuestaOperacion.Error($"Error en el servicio: {ex.Message}");
            }
        }


        [WebMethod(Description = "Obtiene empleado por ID")]
        public Empleado ObtenerEmpleadoPorId(int empleadoId)
        {
            try
            {
                return _empleadoService.ObtenerEmpleado(empleadoId);
            }
            catch
            {
                return null;
            }
        }


        [WebMethod(Description = "Obtiene todos los empleados")]
        public List<Empleado> ObtenerTodosEmpleados()
        {
            try
            {
                return _empleadoService.ObtenerTodosEmpleados();
            }
            catch
            {
                return new List<Empleado>();
            }
        }


        [WebMethod(Description = "Obtiene empleados por departamento")]
        public List<Empleado> ObtenerEmpleadosPorDepartamento(int departamentoId)
        {
            try
            {
                return _empleadoService.ObtenerEmpleadosPorDepartamento(departamentoId);
            }
            catch
            {
                return new List<Empleado>();
            }
        }


        [WebMethod(Description = "Genera reporte de empleados")]
        public List<Empleado> GenerarReporteEmpleados(
            int? departamentoId = null,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null)
        {
            try
            {
                var filtro = new FiltroEmpleados
                {
                    DepartamentoId = departamentoId,
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin
                };

                return _empleadoService.GenerarReporte(filtro);
            }
            catch
            {
                return new List<Empleado>();
            }
        }


        [WebMethod(Description = "Valida DPI único")]
        public bool ValidarDPI(string dpi, int? excludeEmpleadoId = null)
        {
            try
            {
                return _empleadoService.ValidarDPI(dpi, excludeEmpleadoId);
            }
            catch
            {
                return false;
            }
        }


        [WebMethod(Description = "Obtiene departamentos activos")]
        public List<Departamento> ObtenerDepartamentosActivos()
        {
            try
            {
                return _empleadoService.ObtenerDepartamentosActivos();
            }
            catch
            {
                return new List<Departamento>();
            }
        }


        [WebMethod(Description = "Desactiva un empleado")]
        public RespuestaOperacion DesactivarEmpleado(int empleadoId)
        {
            try
            {
                return _empleadoService.DesactivarEmpleado(empleadoId);
            }
            catch (Exception ex)
            {
                return RespuestaOperacion.Error($"Error en el servicio: {ex.Message}");
            }
        }
    }
}
