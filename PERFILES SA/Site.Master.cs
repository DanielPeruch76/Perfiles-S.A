using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PERFILES_SA
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["UsuarioNombre"] != null)
                {
                    lblUsuario.Text = Session["UsuarioNombre"].ToString();
                }
                else
                {
                    lblUsuario.Text = "Invitado";
                }

                lblFechaServidor.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }
        }

        protected void lnkCerrarSesion_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();

            FormsAuthentication.SignOut();

            Response.Redirect("~/Default.aspx");
        }
    }
}