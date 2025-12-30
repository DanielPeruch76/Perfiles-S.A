using PERFILES_SA.Models;
using PERFILES_SA.Services;
using System;
using System.Web.UI;

namespace PERFILES_SA.Pages.Departamentos
{
    public partial class Editar : System.Web.UI.Page
    {
        private readonly IDepartamentoService _departamentoService;
        private readonly IEmpleadoService _empleadoService;
        private int _departamentoId;

        public Editar()
        {
            _departamentoService = new PERFILES_SA.Services.DepartamentoService();
            _empleadoService = new PERFILES_SA.Services.EmpleadoService();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out _departamentoId))
                {
                    hdnDepartamentoId.Value = _departamentoId.ToString();
                    CargarDatosDepartamento();
                }
                else
                {
                    MostrarMensaje("ID de departamento no válido", "danger");
                    btnGuardar.Enabled = false;
                    btnDesactivar.Enabled = false;
                }
            }
            else
            {
                _departamentoId = int.Parse(hdnDepartamentoId.Value);
            }
        }

        private void CargarDatosDepartamento()
        {
            try
            {
                var departamento = _departamentoService.ObtenerDepartamento(_departamentoId);

                if (departamento != null)
                {
                    txtNombre.Text = departamento.Nombre;
                    txtDescripcion.Text = departamento.Descripcion;
                    ddlEstado.SelectedValue = departamento.Activo.ToString().ToLower();

                    lblFechaCreacion.InnerText = departamento.FechaCreacion.ToString("dd/MM/yyyy HH:mm");
                    lblFechaActualizacion.InnerText = departamento.FechaActualizacion?.ToString("dd/MM/yyyy HH:mm") ?? "Nunca";

                    try
                    {
                        var empleados = _empleadoService.ObtenerEmpleadosPorDepartamento(_departamentoId);
                        int totalEmpleados = empleados?.Count ?? 0;
                        lblEmpleadosAsignados.InnerText = totalEmpleados.ToString();
                    }
                    catch
                    {
                        lblEmpleadosAsignados.InnerText = "Error al contar";
                    }
                }
                else
                {
                    MostrarMensaje("Departamento no encontrado", "danger");
                    btnGuardar.Enabled = false;
                    btnDesactivar.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar datos: {ex.Message}", "danger");
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarCampos())
                    return;

                var departamento = new Departamento
                {
                    DepartamentoId = _departamentoId,
                    Nombre = txtNombre.Text.Trim(),
                    Descripcion = txtDescripcion.Text.Trim(),
                    Activo = bool.Parse(ddlEstado.SelectedValue)
                };

                var resultado = _departamentoService.ActualizarDepartamento(departamento);

                if (resultado.Exitoso)
                {
                    MostrarMensaje($"{resultado.Mensaje}", "success");
                    CargarDatosDepartamento();
                }
                else
                {
                    MostrarMensaje($"{resultado.Mensaje}", "danger");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error inesperado: {ex.Message}", "danger");
            }
        }

        protected void btnDesactivar_Click(object sender, EventArgs e)
        {
            try
            {
                var resultado = _departamentoService.CambiarEstadoDepartamento(_departamentoId, false);

                if (resultado.Exitoso)
                {
                    MostrarMensaje($" {resultado.Mensaje}", "success");
                    ddlEstado.SelectedValue = "false";
                    btnDesactivar.Enabled = false;
                    CargarDatosDepartamento();
                }
                else
                {
                    MostrarMensaje($" {resultado.Mensaje}", "danger");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($" Error: {ex.Message}", "danger");
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("ListarDepartamentos.aspx");
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MostrarMensaje("El nombre del departamento es requerido", "warning");
                return false;
            }

            if (txtNombre.Text.Length > 100)
            {
                MostrarMensaje("El nombre no puede exceder 100 caracteres", "warning");
                return false;
            }

            if (!string.IsNullOrEmpty(txtDescripcion.Text) && txtDescripcion.Text.Length > 500)
            {
                MostrarMensaje("La descripción no puede exceder 500 caracteres", "warning");
                return false;
            }

            return true;
        }

        private void MostrarMensaje(string mensaje, string tipo)
        {
            divMensaje.Visible = true;
            lblMensaje.Text = mensaje;
            divMensaje.Attributes["class"] = $"alert alert-{tipo} alert-dismissible fade show";

            string script = @"
                setTimeout(function() {
                    $('#" + divMensaje.ClientID + @"').fadeOut('slow');
                }, 5000);";

            ScriptManager.RegisterStartupScript(this, GetType(), "OcultarMensaje", script, true);
        }
    }
}