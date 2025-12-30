using PERFILES_SA.Models;
using PERFILES_SA.Services;
using System;
using System.Web.UI;

namespace PERFILES_SA.Pages.Empleados
{
    public partial class Editar : System.Web.UI.Page
    {
        private readonly IEmpleadoService _empleadoService;
        private int _empleadoId;

        public Editar()
        {
            _empleadoService = new PERFILES_SA.Services.EmpleadoService();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out _empleadoId))
                {
                    hdnEmpleadoId.Value = _empleadoId.ToString();
                    CargarDatosEmpleado();
                    CargarDepartamentos();
                }
                else
                {
                    MostrarMensaje("ID de empleado no válido", "danger");
                    btnGuardar.Enabled = false;
                    btnDesactivar.Enabled = false;
                }
            }
            else
            {
                _empleadoId = int.Parse(hdnEmpleadoId.Value);
            }
        }

        private void CargarDatosEmpleado()
        {
            try
            {
                var empleado = _empleadoService.ObtenerEmpleado(_empleadoId);

                if (empleado != null)
                {
                    txtDPI.Text = empleado.DPI;
                    txtNombres.Text = empleado.Nombres;
                    txtFechaNacimiento.Text = empleado.FechaNacimiento.ToString("yyyy-MM-dd");
                    ddlSexo.SelectedValue = empleado.Sexo;
                    txtFechaIngreso.Text = empleado.FechaIngreso.ToString("yyyy-MM-dd");
                    txtDireccion.Text = empleado.Direccion;
                    txtNIT.Text = empleado.NIT;
                    ddlEstado.SelectedValue = empleado.Activo.ToString().ToLower();

                    lblFechaCreacion.InnerText = empleado.FechaCreacion.ToString("dd/MM/yyyy HH:mm");
                    lblFechaActualizacion.InnerText = empleado.FechaActualizacion?.ToString("dd/MM/yyyy HH:mm") ?? "Nunca";
                    lblDepartamentoActual.InnerText = empleado.NombreDepartamento;

                }
                else
                {
                    MostrarMensaje("Empleado no encontrado", "danger");
                    btnGuardar.Enabled = false;
                    btnDesactivar.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar datos: {ex.Message}", "danger");
            }
        }

        private void CargarDepartamentos()
        {
            try
            {
                var departamentos = _empleadoService.ObtenerDepartamentosActivos();

                ddlDepartamento.Items.Clear();
                ddlDepartamento.Items.Add(new System.Web.UI.WebControls.ListItem("Seleccionar Departamento", ""));

                foreach (var depto in departamentos)
                {
                    ddlDepartamento.Items.Add(new System.Web.UI.WebControls.ListItem(
                        depto.Nombre,
                        depto.DepartamentoId.ToString()
                    ));
                }

                if (!IsPostBack)
                {
                    var empleado = _empleadoService.ObtenerEmpleado(_empleadoId);
                    if (empleado != null)
                    {
                        ddlDepartamento.SelectedValue = empleado.DepartamentoId.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar departamentos: {ex.Message}", "warning");
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarCampos())
                    return;

                var empleado = new Empleado
                {
                    EmpleadoId = _empleadoId,
                    DPI = txtDPI.Text.Trim(),
                    Nombres = txtNombres.Text.Trim(),
                    FechaNacimiento = DateTime.Parse(txtFechaNacimiento.Text),
                    Sexo = ddlSexo.SelectedValue,
                    FechaIngreso = DateTime.Parse(txtFechaIngreso.Text),
                    Direccion = txtDireccion.Text.Trim(),
                    NIT = txtNIT.Text.Trim(),
                    DepartamentoId = int.Parse(ddlDepartamento.SelectedValue),
                    Activo = bool.Parse(ddlEstado.SelectedValue)
                };

                var resultado = _empleadoService.ActualizarEmpleado(empleado);

                if (resultado.Exitoso)
                {
                    MostrarMensaje($" {resultado.Mensaje}", "success");
                    CargarDatosEmpleado();
                }
                else
                {
                    MostrarMensaje($" {resultado.Mensaje}", "danger");
                }
            }
            catch (FormatException)
            {
                MostrarMensaje(" Error en el formato de las fechas", "danger");
            }
            catch (Exception ex)
            {
                MostrarMensaje($" Error inesperado: {ex.Message}", "danger");
            }
        }

        protected void btnDesactivar_Click(object sender, EventArgs e)
        {
            try
            {
                var resultado = _empleadoService.DesactivarEmpleado(_empleadoId);

                if (resultado.Exitoso)
                {
                    MostrarMensaje($" {resultado.Mensaje}", "success");
                    ddlEstado.SelectedValue = "false";
                    btnDesactivar.Enabled = false;
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
            Response.Redirect("Listar.aspx");
        }

        private bool ValidarCampos()
        {
            if (string.IsNullOrWhiteSpace(txtDPI.Text))
            {
                MostrarMensaje("El DPI es requerido", "warning");
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNombres.Text))
            {
                MostrarMensaje("Los nombres son requeridos", "warning");
                return false;
            }

            if (string.IsNullOrWhiteSpace(ddlDepartamento.SelectedValue))
            {
                MostrarMensaje("Debe seleccionar un departamento", "warning");
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