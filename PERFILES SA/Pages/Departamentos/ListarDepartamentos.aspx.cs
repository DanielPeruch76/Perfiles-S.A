using PERFILES_SA.Models;
using PERFILES_SA.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PERFILES_SA.Pages.Departamentos
{
    public partial class ListarDepartamentos : System.Web.UI.Page
    {
        private readonly IDepartamentoService _departamentoService;
        private readonly IEmpleadoService _empleadoService;
        private List<Departamento> _departamentos;

        public ListarDepartamentos()
        {
            _departamentoService = new PERFILES_SA.Services.DepartamentoService();
            _empleadoService = new PERFILES_SA.Services.EmpleadoService();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarDepartamentos();
                ActualizarEstadisticas();
            }
        }

        private void CargarDepartamentos()
        {
            try
            {
                _departamentos = _departamentoService.ObtenerTodosDepartamentos();

                Debug.WriteLine("=== LISTA COMPLETA DE DEPARTAMENTOS ===");
                foreach (var depto in _departamentos)
                {
                    Debug.WriteLine($"ID: {depto.DepartamentoId}, " +
                                   $"Nombre: {depto.Nombre}, " +
                                   $"Activo: {depto.Activo}");
                }

                AplicarFiltros();
                EnlazarGridView();
            }
            catch (Exception ex)
            {
                MostrarError($"Error al cargar departamentos: {ex.Message}");
                _departamentos = new List<Departamento>();
                EnlazarGridView();
            }
        }

        private void AplicarFiltros()
        {
            if (_departamentos == null) return;

            var departamentosFiltrados = _departamentos.AsEnumerable();

            if (!string.IsNullOrEmpty(ddlFiltroEstado.SelectedValue))
            {
                bool estado = bool.Parse(ddlFiltroEstado.SelectedValue);
                departamentosFiltrados = departamentosFiltrados.Where(d => d.Activo == estado);
            }

            if (!string.IsNullOrEmpty(txtBuscar.Text))
            {
                string busqueda = txtBuscar.Text.ToLower();
                departamentosFiltrados = departamentosFiltrados.Where(d =>
                    (d.Nombre != null && d.Nombre.ToLower().Contains(busqueda)) ||
                    (d.Descripcion != null && d.Descripcion.ToLower().Contains(busqueda)));
            }

            _departamentos = departamentosFiltrados.ToList();
        }

        private void EnlazarGridView()
        {
            gvDepartamentos.DataSource = _departamentos;
            gvDepartamentos.DataBind();

            if (_departamentos != null && _departamentos.Count > 0)
            {
                gvDepartamentos.HeaderRow.Cells[0].Visible = false; 
                foreach (GridViewRow row in gvDepartamentos.Rows)
                {
                    row.Cells[0].Visible = false; 
                }
            }
        }

        public int ContarEmpleadosPorDepartamento(int departamentoId)
        {
            try
            {
                var empleados = _empleadoService.ObtenerEmpleadosPorDepartamento(departamentoId);
                return empleados?.Count ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        private void ActualizarEstadisticas()
        {
            if (_departamentos == null || _departamentos.Count == 0)
            {
                lblTotalDepartamentos.InnerText = "0";
                lblActivos.InnerText = "0";
                lblInactivos.InnerText = "0";
                lblTotalEmpleados.InnerText = "0";
                return;
            }

            int total = _departamentos.Count;
            int activos = _departamentos.Count(d => d.Activo);
            int inactivos = total - activos;

            int totalEmpleados = 0;
            try
            {
                var todosEmpleados = _empleadoService.ObtenerTodosEmpleados();
                totalEmpleados = todosEmpleados?.Count(e => e.Activo) ?? 0;
            }
            catch { }

            lblTotalDepartamentos.InnerText = total.ToString("N0");
            lblActivos.InnerText = activos.ToString("N0");
            lblInactivos.InnerText = inactivos.ToString("N0");
            lblTotalEmpleados.InnerText = totalEmpleados.ToString("N0");
        }

        protected void ddlFiltroEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarDepartamentos();
            ActualizarEstadisticas();
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarDepartamentos();
            ActualizarEstadisticas();
        }

        protected void gvDepartamentos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDepartamentos.PageIndex = e.NewPageIndex;
            EnlazarGridView();
        }

        protected void gvDepartamentos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "CambiarEstado")
            {
                int departamentoId = Convert.ToInt32(e.CommandArgument);

                try
                {
                    var departamento = _departamentoService.ObtenerDepartamento(departamentoId);
                    if (departamento != null)
                    {
                        Debug.WriteLine("Si entro a actualizar departamento");
                        bool nuevoEstado = !departamento.Activo;

                        var resultado = _departamentoService.CambiarEstadoDepartamento(departamentoId, nuevoEstado);

                        if (resultado.Exitoso)
                        {
                            CargarDepartamentos();
                            ActualizarEstadisticas();

                            string accion = nuevoEstado ? "activado" : "desactivado";
                            ScriptManager.RegisterStartupScript(this, GetType(), "MensajeExito",
                                $"toastr.success('Departamento {accion} exitosamente.');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "MensajeError",
                                $"toastr.error('{resultado.Mensaje}');", true);
                        }
                    }
                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "MensajeError",
                        $"toastr.error('Error: {ex.Message}');", true);
                }
            }
        }

        private void MostrarError(string mensaje)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "MostrarError",
                $"toastr.error('{mensaje}');", true);
        }
    }
}