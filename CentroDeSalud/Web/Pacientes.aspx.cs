using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Dominio.Entidades;
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

        protected void gvPacientes_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());
            try
            {
                if (e.CommandName == "Editar")
                {
                    Paciente p = negocio.listar().Find(x => x.Id == id);
                    if (p != null)
                    {
                        hfId.Value = p.Id.ToString();
                        txtNombre.Text = p.Nombre;
                        txtApellido.Text = p.Apellido;
                        txtDni.Text = p.Dni;
                        // El control de fecha solo acepta el formato yyyy-MM-dd.
                        txtFecha.Text = p.FechaNacimiento.ToString("yyyy-MM-dd");
                        txtEmail.Text = p.Email;
                        txtTelefono.Text = p.Telefono ?? string.Empty;
                        txtDireccion.Text = p.Direccion ?? string.Empty;
                        lblFormulario.Text = "Modificar paciente";
                    }
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

            DateTime fecha;
            if (!DateTime.TryParse(txtFecha.Text, out fecha))
            {
                mostrarError("La fecha de nacimiento no tiene un formato válido.");
                return;
            }

            Paciente p = new Paciente();
            p.Id = int.Parse(hfId.Value);
            p.Nombre = txtNombre.Text;
            p.Apellido = txtApellido.Text;
            p.Dni = txtDni.Text;
            p.FechaNacimiento = fecha;
            p.Email = txtEmail.Text;
            p.Telefono = txtTelefono.Text;
            p.Direccion = txtDireccion.Text;

            try
            {
                if (p.Id == 0)
                    negocio.agregar(p);
                else
                    negocio.modificar(p);

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
                mostrarError("No se pudo guardar el paciente.");
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            limpiarFormulario();
        }

        private void limpiarFormulario()
        {
            hfId.Value = "0";
            txtNombre.Text = string.Empty;
            txtApellido.Text = string.Empty;
            txtDni.Text = string.Empty;
            txtFecha.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtTelefono.Text = string.Empty;
            txtDireccion.Text = string.Empty;
            lblFormulario.Text = "Nuevo paciente";
            lblMensaje.Visible = false;
        }

        private void mostrarError(string mensaje)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.Visible = true;
        }
    }
}
