using PERFILES_SA.Models;
using PERFILES_SA.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PERFILES_SA.Services
{
    public class EmpleadoService : IEmpleadoService
    {
        private readonly IEmpleadoRepository _empleadoRepository;
        private readonly IDepartamentoRepository _departamentoRepository;

        public EmpleadoService()
        {
            _empleadoRepository = new EmpleadoRepository();
            _departamentoRepository = new DepartamentoRepository();
        }

        public EmpleadoService(IEmpleadoRepository empleadoRepository, IDepartamentoRepository departamentoRepository)
        {
            _empleadoRepository = empleadoRepository;
            _departamentoRepository = departamentoRepository;
        }

        public RespuestaOperacion RegistrarEmpleado(Empleado empleado)
        {
            if (empleado == null)
                return RespuestaOperacion.Error("Los datos del empleado son requeridos.");

            if (string.IsNullOrWhiteSpace(empleado.DPI))
                return RespuestaOperacion.Error("El DPI es requerido.");

            if (string.IsNullOrWhiteSpace(empleado.Nombres))
                return RespuestaOperacion.Error("Los nombres son requeridos.");

            if (empleado.FechaNacimiento >= System.DateTime.Now)
                return RespuestaOperacion.Error("La fecha de nacimiento debe ser en el pasado.");

            if (empleado.FechaIngreso > System.DateTime.Now)
                return RespuestaOperacion.Error("La fecha de ingreso no puede ser futura.");

            if (empleado.FechaIngreso < empleado.FechaNacimiento)
                return RespuestaOperacion.Error("La fecha de ingreso no puede ser anterior a la fecha de nacimiento.");

            if (empleado.DepartamentoId <= 0)
                return RespuestaOperacion.Error("Debe seleccionar un departamento.");

            var departamento = _departamentoRepository.ObtenerPorId(empleado.DepartamentoId);
            if (departamento == null)
                return RespuestaOperacion.Error("El departamento seleccionado no existe.");

            if (!departamento.Activo)
                return RespuestaOperacion.Error("El departamento seleccionado no está activo.");

            if (_empleadoRepository.ExisteDPI(empleado.DPI))
                return RespuestaOperacion.Error("El DPI ya está registrado en el sistema.");

            var edadMinima = System.DateTime.Now.AddYears(-18);
            if (empleado.FechaNacimiento > edadMinima)
                return RespuestaOperacion.Error("El empleado debe ser mayor de 18 años.");

            Console.WriteLine("Si se va a mandar a guardar en el service");
            return _empleadoRepository.Insertar(empleado);
        }

        public RespuestaOperacion ActualizarEmpleado(Empleado empleado)
        {
            if (empleado == null)
                return RespuestaOperacion.Error("Los datos del empleado son requeridos.");

            if (empleado.EmpleadoId <= 0)
                return RespuestaOperacion.Error("ID de empleado inválido.");

            var empleadoExistente = _empleadoRepository.ObtenerPorId(empleado.EmpleadoId);
            if (empleadoExistente == null)
                return RespuestaOperacion.Error("El empleado no existe.");

            var departamento = _departamentoRepository.ObtenerPorId(empleado.DepartamentoId);
            if (departamento == null)
                return RespuestaOperacion.Error("El departamento seleccionado no existe.");

            if (!departamento.Activo)
                return RespuestaOperacion.Error("El departamento seleccionado no está activo.");

            if (_empleadoRepository.ExisteDPI(empleado.DPI, empleado.EmpleadoId))
                return RespuestaOperacion.Error("El DPI ya está registrado en el sistema.");

            return _empleadoRepository.Actualizar(empleado);
        }

        public RespuestaOperacion DesactivarEmpleado(int empleadoId)
        {
            if (empleadoId <= 0)
                return RespuestaOperacion.Error("ID de empleado inválido.");

            return _empleadoRepository.Desactivar(empleadoId);
        }

        public RespuestaOperacion ActivarEmpleado(int empleadoId)
        {
            if (empleadoId <= 0)
                return RespuestaOperacion.Error("ID de empleado inválido.");

            return _empleadoRepository.Activar(empleadoId);
        }

        public Empleado ObtenerEmpleado(int empleadoId)
        {
            return _empleadoRepository.ObtenerPorId(empleadoId);
        }

        public List<Empleado> ObtenerTodosEmpleados()
        {
            return _empleadoRepository.ObtenerTodos();
        }

        public List<Empleado> ObtenerEmpleadosPorDepartamento(int departamentoId)
        {
            return _empleadoRepository.ObtenerPorDepartamento(departamentoId);
        }

        public List<Empleado> GenerarReporte(FiltroEmpleados filtro)
        {
            return _empleadoRepository.ObtenerReporte(filtro);
        }

        public bool ValidarDPI(string dpi, int? excludeEmpleadoId = null)
        {
            return !_empleadoRepository.ExisteDPI(dpi, excludeEmpleadoId);
        }

        public List<Departamento> ObtenerDepartamentosActivos()
        {
            return _departamentoRepository.ObtenerActivos();
        }
    }
}