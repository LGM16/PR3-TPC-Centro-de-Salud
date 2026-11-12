using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dominio.Entidades;
using Negocio;

namespace Web
{
    public partial class Especialidades : Page
    {
        private EspecialidadNegocio negocio = new EspecialidadNegocio();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                cargarGrilla();
        }

        private void cargarGrilla()
        {
            try
            {
                gvEspecialidades.DataSource = negocio.listar();
                gvEspecialidades.DataBind();
            }
            catch (Exception)
            {
                mostrarError("No se pudo leer la lista de especialidades.");
            }
        }

        protected void gvEspecialidades_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());
            try
            {
                switch (e.CommandName)
                {
                    case "Editar":
                        Especialidad esp = negocio.listar().Find(x => x.Id == id);
                        if (esp != null)
                        {
                            hfId.Value = esp.Id.ToString();
                            txtDescripcion.Text = esp.Descripcion;
                            lblFormulario.Text = "Modificar especialidad";
                        }
                        break;
                    case "Baja":
                        negocio.cambiarEstado(id, false);
                        cargarGrilla();
                        break;
                    case "Alta":
                        negocio.cambiarEstado(id, true);
                        cargarGrilla();
                        break;
                }
            }
            catch (Exception)
            {
                mostrarError("No se pudo completar la operación.");
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            Page.Validate();
            if (!Page.IsValid)
                return;

            Especialidad esp = new Especialidad();
            esp.Id = int.Parse(hfId.Value);
            esp.Descripcion = txtDescripcion.Text;

            try
            {
                if (esp.Id == 0)
                    negocio.agregar(esp);
                else
                    negocio.modificar(esp);

                limpiarFormulario();
                cargarGrilla();
            }
            catch (ArgumentException ex)
            {
                // Errores de validacion de Negocio: el mensaje esta pensado para el usuario.
                mostrarError(ex.Message);
            }
            catch (Exception)
            {
                mostrarError("No se pudo guardar la especialidad.");
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            limpiarFormulario();
        }

        private void limpiarFormulario()
        {
            hfId.Value = "0";
            txtDescripcion.Text = string.Empty;
            lblFormulario.Text = "Nueva especialidad";
            lblMensaje.Visible = false;
        }

        private void mostrarError(string mensaje)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.Visible = true;
        }
    }
}
