using PERFILES_SA.DataAccess;
using PERFILES_SA.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace PERFILES_SA.Repositories
{
    public class EmpleadoRepository : IEmpleadoRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public EmpleadoRepository()
        {
            _dbHelper = new DatabaseHelper();
        }

        public RespuestaOperacion Insertar(Empleado empleado)
        {
            try
            {

                if (ExisteDPI(empleado.DPI))
                {
                    return RespuestaOperacion.Error("El DPI ya está registrado en el sistema.");
                }

                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PerfilesSAConnection"].ConnectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("usp_InsertarEmpleado", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@DPI", empleado.DPI);
                        command.Parameters.AddWithValue("@Nombres", empleado.Nombres);
                        command.Parameters.AddWithValue("@FechaNacimiento", empleado.FechaNacimiento);
                        command.Parameters.AddWithValue("@Sexo", empleado.Sexo);
                        command.Parameters.AddWithValue("@FechaIngreso", empleado.FechaIngreso);

                        if (string.IsNullOrEmpty(empleado.Direccion))
                            command.Parameters.AddWithValue("@Direccion", DBNull.Value);
                        else
                            command.Parameters.AddWithValue("@Direccion", empleado.Direccion);

                        if (string.IsNullOrEmpty(empleado.NIT))
                            command.Parameters.AddWithValue("@NIT", DBNull.Value);
                        else
                            command.Parameters.AddWithValue("@NIT", empleado.NIT);

                        command.Parameters.AddWithValue("@DepartamentoId", empleado.DepartamentoId);

                        var nuevoIdParam = new SqlParameter("@NuevoId", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(nuevoIdParam);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (nuevoIdParam.Value != DBNull.Value && nuevoIdParam.Value != null)
                        {
                            int nuevoId = Convert.ToInt32(nuevoIdParam.Value);

                            empleado.EmpleadoId = nuevoId;
                            return RespuestaOperacion.Exito(empleado.EmpleadoId, "Empleado registrado exitosamente.");
                        }
                        else
                        {
                            return RespuestaOperacion.Error("No se pudo obtener el ID del empleado registrado.");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {

                if (ex.Number == 50000)
                {
                    return RespuestaOperacion.Error(ex.Message);
                }

                return RespuestaOperacion.Error($"Error de base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                return RespuestaOperacion.Error($"Error inesperado: {ex.Message}");
            }
        }

        public RespuestaOperacion Actualizar(Empleado empleado)
        {
            try
            {
                if (ExisteDPI(empleado.DPI, empleado.EmpleadoId))
                {
                    return RespuestaOperacion.Error("El DPI ya está registrado para otro empleado.");
                }

                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PerfilesSAConnection"].ConnectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("usp_ActualizarEmpleado", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@EmpleadoId", empleado.EmpleadoId);
                        command.Parameters.AddWithValue("@DPI", empleado.DPI);
                        command.Parameters.AddWithValue("@Nombres", empleado.Nombres);
                        command.Parameters.AddWithValue("@FechaNacimiento", empleado.FechaNacimiento);
                        command.Parameters.AddWithValue("@Sexo", empleado.Sexo);
                        command.Parameters.AddWithValue("@FechaIngreso", empleado.FechaIngreso);
                        command.Parameters.AddWithValue("@Direccion",
                            string.IsNullOrEmpty(empleado.Direccion) ? (object)DBNull.Value : empleado.Direccion);
                        command.Parameters.AddWithValue("@NIT",
                            string.IsNullOrEmpty(empleado.NIT) ? (object)DBNull.Value : empleado.NIT);
                        command.Parameters.AddWithValue("@DepartamentoId", empleado.DepartamentoId);

                        command.ExecuteNonQuery();

                        Console.WriteLine($"✅ Empleado {empleado.EmpleadoId} procesado");
                        return RespuestaOperacion.Exito(null, "Empleado actualizado exitosamente.");
                    }
                }
            }
            catch (SqlException ex)
            {
                string mensajeError = ex.Message;
                if (ex.Number == 50000 && mensajeError.Contains("El departamento seleccionado no está activo"))
                {
                    mensajeError = "El departamento seleccionado no está activo.";
                }

                return RespuestaOperacion.Error(mensajeError);
            }
            catch (Exception ex)
            {
                return RespuestaOperacion.Error($"Error: {ex.Message}");
            }
        }

        public RespuestaOperacion Desactivar(int empleadoId)
        {
            try
            {

                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PerfilesSAConnection"].ConnectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand(
                        "UPDATE Empleados SET Activo = 0, FechaActualizacion = GETDATE() WHERE EmpleadoId = @EmpleadoId",
                        connection))
                    {
                        command.Parameters.AddWithValue("@EmpleadoId", empleadoId);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            return RespuestaOperacion.Exito(null, "Empleado desactivado exitosamente.");
                        }
                        else
                        {
                            return RespuestaOperacion.Error("Empleado no encontrado.");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {

                return RespuestaOperacion.Error($"Error de base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
  
                return RespuestaOperacion.Error($"Error inesperado: {ex.Message}");
            }
        }

        public RespuestaOperacion Activar(int empleadoId)
        {
            try
            {

                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PerfilesSAConnection"].ConnectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand(
                        "UPDATE Empleados SET Activo = 1, FechaActualizacion = GETDATE() WHERE EmpleadoId = @EmpleadoId",
                        connection))
                    {
                        command.Parameters.AddWithValue("@EmpleadoId", empleadoId);

                        int result = command.ExecuteNonQuery();

                        if (result > 0)
                        {
                            return RespuestaOperacion.Exito(null, "Empleado desactivado exitosamente.");
                        }
                        else
                        {
                            return RespuestaOperacion.Error("Empleado no encontrado.");
                        }
                    }
                }
            }
            catch (SqlException ex)
            {

                return RespuestaOperacion.Error($"Error de base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                return RespuestaOperacion.Error($"Error inesperado: {ex.Message}");
            }
        }

        public Empleado ObtenerPorId(int empleadoId)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@EmpleadoId", empleadoId)
                };

                var dataTable = _dbHelper.ExecuteStoredProcedure("usp_ObtenerEmpleadoPorId", parameters);

                if (dataTable.Rows.Count > 0)
                {
                    var row = dataTable.Rows[0];
                    return MapearEmpleado(row);
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public List<Empleado> ObtenerTodos()
        {
            var empleados = new List<Empleado>();

            try
            {
                var dataTable = _dbHelper.ExecuteStoredProcedure("usp_ObtenerTodosEmpleados");

                foreach (DataRow row in dataTable.Rows)
                {
                    empleados.Add(MapearEmpleadoSimple(row));
                }
            }
            catch
            {
            }

            return empleados;
        }

        public List<Empleado> ObtenerPorDepartamento(int departamentoId)
        {
            var empleados = new List<Empleado>();

            try
            {
                var parameters = new SqlParameter[]
                {
                    new SqlParameter("@DepartamentoId", departamentoId)
                };

                var dataTable = _dbHelper.ExecuteStoredProcedure("usp_ReporteEmpleadosPorDepartamento", parameters);

                foreach (DataRow row in dataTable.Rows)
                {
                    empleados.Add(MapearEmpleadoReporte(row));
                }
            }
            catch
            {
            }

            return empleados;
        }

        public List<Empleado> ObtenerReporte(FiltroEmpleados filtro)
        {
            var empleados = new List<Empleado>();

            try
            {
                var parameters = new List<SqlParameter>();

                if (filtro.DepartamentoId.HasValue)
                {
                    parameters.Add(new SqlParameter("@DepartamentoId", filtro.DepartamentoId.Value));
                }
                else
                {
                    parameters.Add(new SqlParameter("@DepartamentoId", DBNull.Value));
                }

                if (filtro.FechaInicio.HasValue)
                {
                    parameters.Add(new SqlParameter("@FechaInicio", filtro.FechaInicio.Value));
                }
                else
                {
                    parameters.Add(new SqlParameter("@FechaInicio", DBNull.Value));
                }

                if (filtro.FechaFin.HasValue)
                {
                    parameters.Add(new SqlParameter("@FechaFin", filtro.FechaFin.Value));
                }
                else
                {
                    parameters.Add(new SqlParameter("@FechaFin", DBNull.Value));
                }

                var dataTable = _dbHelper.ExecuteStoredProcedure("usp_ReporteEmpleadosPorDepartamento", parameters.ToArray());

                foreach (DataRow row in dataTable.Rows)
                {
                    empleados.Add(MapearEmpleadoReporte(row));
                }
            }
            catch
            {
            }

            return empleados;
        }

        public bool ExisteDPI(string dpi, int? excludeEmpleadoId = null)
        {
            try
            {
                using (var connection = _dbHelper.GetConnection())
                using (var command = new SqlCommand(
                    "SELECT COUNT(1) FROM Empleados WHERE DPI = @DPI AND (@ExcludeId IS NULL OR EmpleadoId != @ExcludeId)",
                    connection))
                {
                    command.Parameters.AddWithValue("@DPI", dpi);
                    command.Parameters.AddWithValue("@ExcludeId", excludeEmpleadoId.HasValue ? (object)excludeEmpleadoId.Value : DBNull.Value);

                    var count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        private Empleado MapearEmpleado(DataRow row)
        {
            return new Empleado
            {
                EmpleadoId = Convert.ToInt32(row["EmpleadoId"]),
                DPI = row["DPI"].ToString(),
                Nombres = row["Nombres"].ToString(),
                FechaNacimiento = Convert.ToDateTime(row["FechaNacimiento"]),
                Sexo = row["Sexo"].ToString(),
                FechaIngreso = Convert.ToDateTime(row["FechaIngreso"]),
                Direccion = row["Direccion"] != DBNull.Value ? row["Direccion"].ToString() : null,
                NIT = row["NIT"] != DBNull.Value ? row["NIT"].ToString() : null,
                DepartamentoId = Convert.ToInt32(row["DepartamentoId"]),
                Activo = Convert.ToBoolean(row["Activo"]),
                FechaCreacion = Convert.ToDateTime(row["FechaCreacion"]),
                FechaActualizacion = row["FechaActualizacion"] != DBNull.Value ? Convert.ToDateTime(row["FechaActualizacion"]) : (DateTime?)null,
                NombreDepartamento = row["NombreDepartamento"].ToString(),
                DepartamentoActivo = Convert.ToBoolean(row["DepartamentoActivo"])
            };
        }

        private Empleado MapearEmpleadoSimple(DataRow row)
        {
            return new Empleado
            {
                EmpleadoId = Convert.ToInt32(row["EmpleadoId"]),
                DPI = row["DPI"].ToString(),
                Nombres = row["Nombres"].ToString(),
                FechaNacimiento = Convert.ToDateTime(row["FechaNacimiento"]),
                Sexo = row["Sexo"].ToString(),
                FechaIngreso = Convert.ToDateTime(row["FechaIngreso"]),
                Direccion = row["Direccion"] != DBNull.Value ? row["Direccion"].ToString() : null,
                NIT = row["NIT"] != DBNull.Value ? row["NIT"].ToString() : null,
                DepartamentoId = Convert.ToInt32(row["DepartamentoId"]),
                Activo = Convert.ToBoolean(row["Activo"]),
                NombreDepartamento = row["NombreDepartamento"].ToString()
            };
        }

        private Empleado MapearEmpleadoReporte(DataRow row)
        {
            var empleado = MapearEmpleadoSimple(row);
            empleado.DepartamentoActivo = Convert.ToBoolean(row["DepartamentoActivo"]);
            return empleado;
        }
    }
}