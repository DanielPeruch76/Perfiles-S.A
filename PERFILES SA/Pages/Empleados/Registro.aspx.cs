using PERFILES_SA.Models;
using PERFILES_SA.Services;
using System;
using System.Web.UI;

namespace PERFILES_SA.Pages.Empleados
{
    public partial class Registro : System.Web.UI.Page
    {
        private readonly IEmpleadoService _empleadoService;

        public Registro()
        {
            _empleadoService = new PERFILES_SA.Services.EmpleadoService();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarDepartamentos();

                txtFechaIngreso.Text = DateTime.Now.ToString("yyyy-MM-dd");
                txtFechaNacimiento.Text = DateTime.Now.AddYears(-25).ToString("yyyy-MM-dd");
            }
        }

        private void CargarDepartamentos()
        {
            try
            {
                var departamentos = _empleadoService.ObtenerDepartamentosActivos();
                Console.WriteLine(departamentos);

                ddlDepartamento.Items.Clear();
                ddlDepartamento.Items.Add(new System.Web.UI.WebControls.ListItem("Seleccionar Departamento", ""));

                foreach (var depto in departamentos)
                {
                    ddlDepartamento.Items.Add(new System.Web.UI.WebControls.ListItem(
                        depto.Nombre,
                        depto.DepartamentoId.ToString()
                    ));
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($"Error al cargar departamentos: {ex.Message}", "danger");
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
                    DPI = txtDPI.Text.Trim(),
                    Nombres = txtNombres.Text.Trim(),
                    FechaNacimiento = DateTime.Parse(txtFechaNacimiento.Text),
                    Sexo = ddlSexo.SelectedValue,
                    FechaIngreso = DateTime.Parse(txtFechaIngreso.Text),
                    Direccion = txtDireccion.Text.Trim(),
                    NIT = txtNIT.Text.Trim(),
                    DepartamentoId = int.Parse(ddlDepartamento.SelectedValue)
                };

                var resultado = _empleadoService.RegistrarEmpleado(empleado);

                if (resultado.Exitoso)
                {
                    MostrarMensaje($"✅ {resultado.Mensaje}. ID: {resultado.Data}", "success");
                    LimpiarFormulario();

                }
                else
                {
                    MostrarMensaje($"❌ {resultado.Mensaje}", "danger");
                }
            }
            catch (FormatException)
            {
                MostrarMensaje("❌ Error en el formato de las fechas", "danger");
            }
            catch (Exception ex)
            {
                MostrarMensaje($"❌ Error inesperado: {ex.Message}", "danger");
            }
        }

        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
            MostrarMensaje("Formulario limpiado", "info");
        }

        private bool ValidarCampos()
        {
            bool valido = true;

            if (string.IsNullOrWhiteSpace(txtDPI.Text))
            {
                MostrarMensaje("El DPI es requerido", "warning");
                valido = false;
            }

            if (string.IsNullOrWhiteSpace(txtNombres.Text))
            {
                MostrarMensaje("Los nombres son requeridos", "warning");
                valido = false;
            }

            if (string.IsNullOrWhiteSpace(txtFechaNacimiento.Text))
            {
                MostrarMensaje("La fecha de nacimiento es requerida", "warning");
                valido = false;
            }

            if (string.IsNullOrWhiteSpace(ddlSexo.SelectedValue))
            {
                MostrarMensaje("El sexo es requerido", "warning");
                valido = false;
            }

            if (string.IsNullOrWhiteSpace(txtFechaIngreso.Text))
            {
                MostrarMensaje("La fecha de ingreso es requerida", "warning");
                valido = false;
            }

            if (string.IsNullOrWhiteSpace(ddlDepartamento.SelectedValue))
            {
                MostrarMensaje("Debe seleccionar un departamento", "warning");
                valido = false;
            }

            if (!string.IsNullOrWhiteSpace(txtFechaNacimiento.Text))
            {
                var fechaNacimiento = DateTime.Parse(txtFechaNacimiento.Text);
                var edadMinima = DateTime.Now.AddYears(-18);
                if (fechaNacimiento > edadMinima)
                {
                    MostrarMensaje("El empleado debe ser mayor de 18 años", "warning");
                    valido = false;
                }
            }

            if (!string.IsNullOrWhiteSpace(txtFechaIngreso.Text))
            {
                var fechaIngreso = DateTime.Parse(txtFechaIngreso.Text);
                if (fechaIngreso > DateTime.Now)
                {
                    MostrarMensaje("La fecha de ingreso no puede ser futura", "warning");
                    valido = false;
                }
            }

            return valido;
        }

        private void LimpiarFormulario()
        {
            txtDPI.Text = string.Empty;
            txtNombres.Text = string.Empty;
            txtFechaNacimiento.Text = DateTime.Now.AddYears(-25).ToString("yyyy-MM-dd");
            ddlSexo.SelectedIndex = 0;
            txtFechaIngreso.Text = DateTime.Now.ToString("yyyy-MM-dd");
            txtDireccion.Text = string.Empty;
            txtNIT.Text = string.Empty;
            ddlDepartamento.SelectedIndex = 0;
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