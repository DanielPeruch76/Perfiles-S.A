using PERFILES_SA.Models;
using PERFILES_SA.Services;
using System;
using System.Web.UI;

namespace PERFILES_SA.Pages.Departamentos
{
    public partial class Registrar : System.Web.UI.Page
    {
        private readonly IDepartamentoService _departamentoService;

        public Registrar()
        {
            _departamentoService = new PERFILES_SA.Services.DepartamentoService();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidarCampos())
                    return;

                var departamento = new Departamento
                {
                    Nombre = txtNombre.Text.Trim(),
                    Descripcion = txtDescripcion.Text.Trim()
                };

                var resultado = _departamentoService.CrearDepartamento(departamento);

                if (resultado.Exitoso)
                {
                    MostrarMensaje($" {resultado.Mensaje}. ID: {resultado.Data}", "success");
                    LimpiarFormulario();

                    string script = @"
                        setTimeout(function() {
                            window.location.href = 'ListarDepartamentos.aspx';
                        }, 2000);";
                    ScriptManager.RegisterStartupScript(this, GetType(), "Redireccionar", script, true);
                }
                else
                {
                    MostrarMensaje($" {resultado.Mensaje}", "danger");
                }
            }
            catch (Exception ex)
            {
                MostrarMensaje($" Error inesperado: {ex.Message}", "danger");
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

        private void LimpiarFormulario()
        {
            txtNombre.Text = string.Empty;
            txtDescripcion.Text = string.Empty;
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