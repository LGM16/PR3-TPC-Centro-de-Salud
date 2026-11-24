using System;
using System.Web.UI;
using Negocio;

namespace Web
{
    public partial class Pacientes : Page
    {
        private PacienteNegocio negocio = new PacienteNegocio();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                cargarGrilla();
        }

        private void cargarGrilla()
        {
            try
            {
                gvPacientes.DataSource = negocio.listar();
                gvPacientes.DataBind();
            }
            catch (Exception)
            {
                mostrarError("No se pudo leer la lista de pacientes.");
            }
        }

        private void mostrarError(string mensaje)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.Visible = true;
        }
    }
}
