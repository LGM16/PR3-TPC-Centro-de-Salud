using System;
using System.Web.UI;
using Negocio;

namespace Web
{
    public partial class TurnosTrabajo : Page
    {
        private TurnoTrabajoNegocio negocio = new TurnoTrabajoNegocio();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                cargarGrilla();
        }

        private void cargarGrilla()
        {
            try
            {
                gvTurnosTrabajo.DataSource = negocio.listar();
                gvTurnosTrabajo.DataBind();
            }
            catch (Exception)
            {
                mostrarError("No se pudo leer la lista de turnos de trabajo.");
            }
        }

        private void mostrarError(string mensaje)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.Visible = true;
        }
    }
}
