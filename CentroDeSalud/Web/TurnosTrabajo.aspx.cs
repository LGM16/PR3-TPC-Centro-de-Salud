using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dominio.Entidades;
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
                List<TurnoTrabajo> lista = negocio.listar();
                string filtro = txtFiltro.Text.Trim().ToUpper();
                if (filtro != "")
                    lista = lista.FindAll(x => x.Descripcion.ToUpper().Contains(filtro));
                gvTurnosTrabajo.DataSource = lista;
                gvTurnosTrabajo.DataBind();
            }
            catch (Exception)
            {
                mostrarError("No se pudo leer la lista de turnos de trabajo.");
            }
        }

        protected void gvTurnosTrabajo_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());
            try
            {
                switch (e.CommandName)
                {
                    case "Editar":
                        TurnoTrabajo turno = negocio.listar().Find(x => x.Id == id);
                        if (turno != null)
                        {
                            hfId.Value = turno.Id.ToString();
                            txtDescripcion.Text = turno.Descripcion;
                            txtEntrada.Text = turno.HoraEntrada.ToString(@"hh\:mm");
                            txtSalida.Text = turno.HoraSalida.ToString(@"hh\:mm");
                            lblFormulario.Text = "Modificar turno de trabajo";
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
            catch (ArgumentException ex)
            {
                // Regla de Negocio (por ejemplo, hay medicos que usan este turno): se le muestra al usuario.
                mostrarError(ex.Message);
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

            TimeSpan entrada;
            TimeSpan salida;
            if (!TimeSpan.TryParse(txtEntrada.Text, out entrada) || !TimeSpan.TryParse(txtSalida.Text, out salida))
            {
                mostrarError("Las horas no tienen un formato válido.");
                return;
            }

            TurnoTrabajo turno = new TurnoTrabajo();
            turno.Id = int.Parse(hfId.Value);
            turno.Descripcion = txtDescripcion.Text;
            turno.HoraEntrada = entrada;
            turno.HoraSalida = salida;

            try
            {
                if (turno.Id == 0)
                    negocio.agregar(turno);
                else
                    negocio.modificar(turno);

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
                mostrarError("No se pudo guardar el turno de trabajo.");
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
            txtEntrada.Text = string.Empty;
            txtSalida.Text = string.Empty;
            lblFormulario.Text = "Nuevo turno de trabajo";
            lblMensaje.Visible = false;
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            cargarGrilla();
        }

        protected void btnLimpiarFiltro_Click(object sender, EventArgs e)
        {
            txtFiltro.Text = string.Empty;
            cargarGrilla();
        }

        private void mostrarError(string mensaje)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.Visible = true;
        }
    }
}
