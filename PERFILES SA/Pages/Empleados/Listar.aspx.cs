using PERFILES_SA.Models;
using PERFILES_SA.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PERFILES_SA.Pages.Empleados
{
    public partial class Listar : System.Web.UI.Page
    {
        private readonly IEmpleadoService _empleadoService;
        private List<Empleado> _empleados;

        public Listar()
        {
            _empleadoService = new PERFILES_SA.Services.EmpleadoService();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarDepartamentosFiltro();
                CargarEmpleados();
                ActualizarEstadisticas();
            }
        }

        private void CargarDepartamentosFiltro()
        {
            try
            {
                var departamentos = _empleadoService.ObtenerDepartamentosActivos();

                ddlFiltroDepartamento.Items.Clear();
                ddlFiltroDepartamento.Items.Add(new ListItem("Todos los departamentos", ""));

                foreach (var depto in departamentos)
                {
                    ddlFiltroDepartamento.Items.Add(new ListItem(depto.Nombre, depto.DepartamentoId.ToString()));
                }
            }
            catch (Exception ex)
            {
                MostrarError($"Error al cargar departamentos: {ex.Message}");
            }
        }

        private void CargarEmpleados()
        {
            try
            {
                _empleados = _empleadoService.ObtenerTodosEmpleados();
                foreach(Empleado empleado in _empleados)
                {
                    Debug.WriteLine(empleado.Activo);
                    Debug.WriteLine(empleado.Nombres);
                }
                AplicarFiltros();
                EnlazarGridView();
            }
            catch (Exception ex)
            {
                MostrarError($"Error al cargar empleados: {ex.Message}");
                _empleados = new List<Empleado>();
                EnlazarGridView();
            }
        }

        private void AplicarFiltros()
        {
            if (_empleados == null) return;

            var empleadosFiltrados = _empleados.AsEnumerable();

            if (!string.IsNullOrEmpty(ddlFiltroDepartamento.SelectedValue))
            {
                int departamentoId = int.Parse(ddlFiltroDepartamento.SelectedValue);
                empleadosFiltrados = empleadosFiltrados.Where(e => e.DepartamentoId == departamentoId);
            }

            if (!string.IsNullOrEmpty(ddlFiltroEstado.SelectedValue))
            {
                bool estado = bool.Parse(ddlFiltroEstado.SelectedValue);
                empleadosFiltrados = empleadosFiltrados.Where(e => e.Activo == estado);
            }

            if (!string.IsNullOrEmpty(txtBuscar.Text))
            {
                string busqueda = txtBuscar.Text.ToLower();
                empleadosFiltrados = empleadosFiltrados.Where(e =>
                    (e.DPI != null && e.DPI.ToLower().Contains(busqueda)) ||
                    (e.Nombres != null && e.Nombres.ToLower().Contains(busqueda)));
            }

            _empleados = empleadosFiltrados.ToList();
        }

        private void EnlazarGridView()
        {
            gvEmpleados.DataSource = _empleados;
            gvEmpleados.DataBind();

            if (_empleados != null && _empleados.Count > 0)
            {
                gvEmpleados.HeaderRow.Cells[0].Visible = false; 
                foreach (GridViewRow row in gvEmpleados.Rows)
                {
                    row.Cells[0].Visible = false; 
                }
            }
        }

        private void ActualizarEstadisticas()
        {
            if (_empleados == null || _empleados.Count == 0)
            {
                lblTotalEmpleados.InnerText = "0";
                lblActivos.InnerText = "0";
                lblInactivos.InnerText = "0";
                lblPromedioEdad.InnerText = "0";
                return;
            }

            int total = _empleados.Count;
            int activos = _empleados.Count(e => e.Activo);
            int inactivos = total - activos;
            double promedioEdad = _empleados.Average(e => e.Edad);

            lblTotalEmpleados.InnerText = total.ToString("N0");
            lblActivos.InnerText = activos.ToString("N0");
            lblInactivos.InnerText = inactivos.ToString("N0");
            lblPromedioEdad.InnerText = Math.Round(promedioEdad, 1).ToString();
        }

        public int CalcularEdad(DateTime fechaNacimiento)
        {
            var today = DateTime.Today;
            var age = today.Year - fechaNacimiento.Year;
            if (fechaNacimiento.Date > today.AddYears(-age)) age--;
            return age;
        }

        protected void ddlFiltroDepartamento_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarEmpleados();
            ActualizarEstadisticas();
        }

        protected void ddlFiltroEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarEmpleados();
            ActualizarEstadisticas();
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            CargarEmpleados();
            ActualizarEstadisticas();
        }

        protected void gvEmpleados_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvEmpleados.PageIndex = e.NewPageIndex;
            EnlazarGridView();
        }

        protected void gvEmpleados_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "CambiarEstado")
            {
                int empleadoId = Convert.ToInt32(e.CommandArgument);

                var empleado = _empleadoService.ObtenerEmpleado(empleadoId);

                if (empleado != null)
                {
                    try
                    {
                        RespuestaOperacion resultado;

                        if (empleado.Activo)
                        {
                            resultado = _empleadoService.DesactivarEmpleado(empleadoId);
                        }
                        else
                        {
                            resultado = _empleadoService.ActivarEmpleado(empleadoId);
                        }

                        if (resultado.Exitoso)
                        {
                            Debug.WriteLine("Si esta actualizando las cosas ");
                            CargarEmpleados();
                            ActualizarEstadisticas();

                            ScriptManager.RegisterStartupScript(this, GetType(), "MensajeExito",
                                $"toastr.success('{resultado.Mensaje}');", true);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, GetType(), "MensajeError",
                                $"toastr.error('{resultado.Mensaje}');", true);
                        }
                    }
                    catch (Exception ex)
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), "MensajeError",
                            $"toastr.error('Error: {ex.Message}');", true);
                    }
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