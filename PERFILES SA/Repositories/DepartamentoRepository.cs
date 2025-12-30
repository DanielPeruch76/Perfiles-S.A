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
    public class DepartamentoRepository : IDepartamentoRepository
    {
        private readonly DatabaseHelper _dbHelper;

        public DepartamentoRepository()
        {
            _dbHelper = new DatabaseHelper();
        }

        public RespuestaOperacion Insertar(Departamento departamento)
        {
            try
            {

                if (ExisteNombre(departamento.Nombre))
                {
                    return RespuestaOperacion.Error("Ya existe un departamento con ese nombre.");
                }

                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PerfilesSAConnection"].ConnectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("usp_InsertarDepartamento", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@Nombre", departamento.Nombre);

                        if (string.IsNullOrEmpty(departamento.Descripcion))
                            command.Parameters.AddWithValue("@Descripcion", DBNull.Value);
                        else
                            command.Parameters.AddWithValue("@Descripcion", departamento.Descripcion);

                        var nuevoIdParam = new SqlParameter("@NuevoId", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(nuevoIdParam);

                        int rowsAffected = command.ExecuteNonQuery();

                        if (nuevoIdParam.Value != DBNull.Value && nuevoIdParam.Value != null)
                        {
                            int nuevoId = Convert.ToInt32(nuevoIdParam.Value);

                            departamento.DepartamentoId = nuevoId;
                            return RespuestaOperacion.Exito(departamento.DepartamentoId, "Departamento creado exitosamente.");
                        }
                        else
                        {
                            Console.WriteLine("❌ No se pudo obtener el ID generado");
                            return RespuestaOperacion.Error("No se pudo obtener el ID del departamento creado.");
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
                else if (ex.Number == 2627) 
                {
                    return RespuestaOperacion.Error("Ya existe un departamento con ese nombre.");
                }
                else if (ex.Number == 515) 
                {
                    return RespuestaOperacion.Error("El nombre del departamento es requerido.");
                }
                else if (ex.Number == 547) 
                {
                    return RespuestaOperacion.Error("Error de restricción de integridad referencial.");
                }

                return RespuestaOperacion.Error($"Error de base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error general en Insertar Departamento: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                return RespuestaOperacion.Error($"Error inesperado: {ex.Message}");
            }
        }

        public RespuestaOperacion Actualizar(Departamento departamento)
        {
            try
            {
                Console.WriteLine("=== ACTUALIZAR DEPARTAMENTO ===");
                Console.WriteLine($"Departamento ID: {departamento.DepartamentoId}");
                Console.WriteLine($"Nombre: {departamento.Nombre}");

                if (ExisteNombre(departamento.Nombre, departamento.DepartamentoId))
                {
                    Console.WriteLine("❌ Nombre de departamento ya existe para otro departamento");
                    return RespuestaOperacion.Error("Ya existe un departamento con ese nombre.");
                }

                using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["PerfilesSAConnection"].ConnectionString))
                {
                    connection.Open();

                    using (var command = new SqlCommand("usp_ActualizarDepartamento", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@DepartamentoId", departamento.DepartamentoId);
                        command.Parameters.AddWithValue("@Nombre", departamento.Nombre);
                        command.Parameters.AddWithValue("@Descripcion",
                            string.IsNullOrEmpty(departamento.Descripcion) ? (object)DBNull.Value : departamento.Descripcion);

                        command.ExecuteNonQuery();

                        return RespuestaOperacion.Exito(null, "Departamento actualizado exitosamente.");
                    }
                }
            }
            catch (SqlException ex)
            {

                if (ex.Number == 50000) 
                {
                    return RespuestaOperacion.Error(ex.Message);
                }
                else if (ex.Number == 2627) 
                {
                    return RespuestaOperacion.Error("Ya existe un departamento con ese nombre.");
                }

                return RespuestaOperacion.Error($"Error de base de datos: {ex.Message}");
            }
            catch (Exception ex)
            {
                return RespuestaOperacion.Error($"Error inesperado: {ex.Message}");
            }
        }

        public RespuestaOperacion CambiarEstado(int departamentoId, bool activo)
        {
            try
            {
                var parameters = new SqlParameter[]
                {
            new SqlParameter("@DepartamentoId", departamentoId),
            new SqlParameter("@Activo", activo)
                };



                var result = _dbHelper.ExecuteNonQuery("usp_CambiarEstadoDepartamento", parameters);


                if (result >= 0)
                {
                    string mensaje = activo ?
                        "Departamento activado exitosamente." :
                        "Departamento desactivado exitosamente. Los empleados asignados también fueron desactivados.";

                    return RespuestaOperacion.Exito(null, mensaje);
                }
                else if (result == -1)
                {
                    return RespuestaOperacion.Error("Departamento no encontrado.");
                }

                return RespuestaOperacion.Error("Error al cambiar el estado del departamento.");
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

        public Departamento ObtenerPorId(int departamentoId)
        {
            try
            {
                using (var connection = _dbHelper.GetConnection())
                using (var command = new SqlCommand(
                    "SELECT * FROM Departamentos WHERE DepartamentoId = @DepartamentoId",
                    connection))
                {
                    command.Parameters.AddWithValue("@DepartamentoId", departamentoId);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Departamento
                            {
                                DepartamentoId = Convert.ToInt32(reader["DepartamentoId"]),
                                Nombre = reader["Nombre"].ToString(),
                                Descripcion = reader["Descripcion"] != DBNull.Value ? reader["Descripcion"].ToString() : null,
                                Activo = Convert.ToBoolean(reader["Activo"]),
                                FechaCreacion = Convert.ToDateTime(reader["FechaCreacion"]),
                                FechaActualizacion = reader["FechaActualizacion"] != DBNull.Value ?
                                    Convert.ToDateTime(reader["FechaActualizacion"]) : (DateTime?)null
                            };
                        }
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public List<Departamento> ObtenerTodos()
        {
            var departamentos = new List<Departamento>();

            try
            {
                var dataTable = _dbHelper.ExecuteStoredProcedure("usp_ObtenerTodosDepartamentos");

                foreach (DataRow row in dataTable.Rows)
                {
                    departamentos.Add(new Departamento
                    {
                        DepartamentoId = Convert.ToInt32(row["DepartamentoId"]),
                        Nombre = row["Nombre"].ToString(),
                        Descripcion = row["Descripcion"] != DBNull.Value ? row["Descripcion"].ToString() : null,
                        Activo = Convert.ToBoolean(row["Activo"]),
                        FechaCreacion = Convert.ToDateTime(row["FechaCreacion"]),
                        FechaActualizacion = row["FechaActualizacion"] != DBNull.Value ?
                            Convert.ToDateTime(row["FechaActualizacion"]) : (DateTime?)null
                    });
                }
            }
            catch
            {

            }

            return departamentos;
        }

        public List<Departamento> ObtenerActivos()
        {
            var departamentos = new List<Departamento>();

            try
            {
                var dataTable = _dbHelper.ExecuteStoredProcedure("usp_ObtenerDepartamentosActivos");

                foreach (DataRow row in dataTable.Rows)
                {
                    departamentos.Add(new Departamento
                    {
                        DepartamentoId = Convert.ToInt32(row["DepartamentoId"]),
                        Nombre = row["Nombre"].ToString(),
                        Descripcion = row["Descripcion"] != DBNull.Value ? row["Descripcion"].ToString() : null,
                        Activo = Convert.ToBoolean(row["Activo"])
                    });
                }
            }
            catch
            {

            }

            return departamentos;
        }

        public bool ExisteNombre(string nombre, int? excludeDepartamentoId = null)
        {
            try
            {
                using (var connection = _dbHelper.GetConnection())
                using (var command = new SqlCommand(
                    "SELECT COUNT(1) FROM Departamentos WHERE Nombre = @Nombre AND (@ExcludeId IS NULL OR DepartamentoId != @ExcludeId)",
                    connection))
                {
                    command.Parameters.AddWithValue("@Nombre", nombre);
                    command.Parameters.AddWithValue("@ExcludeId", excludeDepartamentoId.HasValue ?
                        (object)excludeDepartamentoId.Value : DBNull.Value);

                    var count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}