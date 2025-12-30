using PERFILES_SA.Models;
using PERFILES_SA.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace PERFILES_SA.Services
{
    public class DepartamentoService : IDepartamentoService
    {
        private readonly IDepartamentoRepository _departamentoRepository;

        public DepartamentoService()
        {
            _departamentoRepository = new DepartamentoRepository();
        }

        public DepartamentoService(IDepartamentoRepository departamentoRepository)
        {
            _departamentoRepository = departamentoRepository;
        }

        public RespuestaOperacion CrearDepartamento(Departamento departamento)
        {
            if (departamento == null)
                return RespuestaOperacion.Error("Los datos del departamento son requeridos.");

            if (string.IsNullOrWhiteSpace(departamento.Nombre))
                return RespuestaOperacion.Error("El nombre del departamento es requerido.");

            if (departamento.Nombre.Length > 100)
                return RespuestaOperacion.Error("El nombre no puede exceder 100 caracteres.");

            if (_departamentoRepository.ExisteNombre(departamento.Nombre))
                return RespuestaOperacion.Error("Ya existe un departamento con ese nombre.");

            return _departamentoRepository.Insertar(departamento);
        }

        public RespuestaOperacion ActualizarDepartamento(Departamento departamento)
        {
            if (departamento == null)
                return RespuestaOperacion.Error("Los datos del departamento son requeridos.");

            if (departamento.DepartamentoId <= 0)
                return RespuestaOperacion.Error("ID de departamento inválido.");

            if (string.IsNullOrWhiteSpace(departamento.Nombre))
                return RespuestaOperacion.Error("El nombre del departamento es requerido.");

            var departamentoExistente = _departamentoRepository.ObtenerPorId(departamento.DepartamentoId);
            if (departamentoExistente == null)
                return RespuestaOperacion.Error("El departamento no existe.");

            if (_departamentoRepository.ExisteNombre(departamento.Nombre, departamento.DepartamentoId))
                return RespuestaOperacion.Error("Ya existe un departamento con ese nombre.");

            return _departamentoRepository.Actualizar(departamento);
        }

        public RespuestaOperacion CambiarEstadoDepartamento(int departamentoId, bool activo)
        {
            if (departamentoId <= 0)
            {
                return RespuestaOperacion.Error("ID de departamento inválido.");
            }

            var departamento = _departamentoRepository.ObtenerPorId(departamentoId);;
            if (departamento == null)
            {
                return RespuestaOperacion.Error("El departamento no existe.");
            }
            return _departamentoRepository.CambiarEstado(departamentoId, activo);
        }

        public Departamento ObtenerDepartamento(int departamentoId)
        {
            return _departamentoRepository.ObtenerPorId(departamentoId);
        }

        public List<Departamento> ObtenerTodosDepartamentos()
        {
            return _departamentoRepository.ObtenerTodos();
        }

        public List<Departamento> ObtenerDepartamentosActivos()
        {
            return _departamentoRepository.ObtenerActivos();
        }

        public bool ValidarNombre(string nombre, int? excludeDepartamentoId = null)
        {
            return !_departamentoRepository.ExisteNombre(nombre, excludeDepartamentoId);
        }
    }
}