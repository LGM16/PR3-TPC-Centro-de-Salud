using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using Dominio.Entidades;
using Negocio;

namespace Web
{
    public partial class Medicos : Page
    {
        private MedicoNegocio negocio = new MedicoNegocio();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                cargarGrilla();
        }

        private void cargarGrilla()
        {
            try
            {
                gvMedicos.DataSource = negocio.listar();
                gvMedicos.DataBind();
            }
            catch (Exception)
            {
                mostrarError("No se pudo leer la lista de médicos.");
            }
        }

        // Se usa desde el .aspx: arma "Clinica medica, Cardiologia" a partir de la lista del medico.
        protected string unirEspecialidades(object especialidades)
        {
            List<Especialidad> lista = especialidades as List<Especialidad>;
            if (lista == null || lista.Count == 0)
                return "-";
            return string.Join(", ", lista.Select(x => x.Descripcion));
        }

        private void mostrarError(string mensaje)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.Visible = true;
        }
    }
}
