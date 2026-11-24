using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
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
            {
                cargarCombos();
                cargarGrilla();
            }
        }

        // Turno de trabajo y especialidades: solo las ACTIVAS se pueden elegir para un medico nuevo.
        private void cargarCombos()
        {
            try
            {
                ddlTurno.DataSource = new TurnoTrabajoNegocio().listar().FindAll(x => x.Activo);
                ddlTurno.DataTextField = "Descripcion";
                ddlTurno.DataValueField = "Id";
                ddlTurno.DataBind();
                ddlTurno.Items.Insert(0, new ListItem("Seleccionar...", "0"));

                cblEspecialidades.DataSource = new EspecialidadNegocio().listar().FindAll(x => x.Activo);
                cblEspecialidades.DataTextField = "Descripcion";
                cblEspecialidades.DataValueField = "Id";
                cblEspecialidades.DataBind();
            }
            catch (Exception)
            {
                mostrarError("No se pudieron cargar los turnos de trabajo y las especialidades.");
            }
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

        protected void gvMedicos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int id = int.Parse(e.CommandArgument.ToString());
            try
            {
                if (e.CommandName == "Editar")
                {
                    Medico m = negocio.listar().Find(x => x.Id == id);
                    if (m != null)
                    {
                        hfId.Value = m.Id.ToString();
                        txtNombre.Text = m.Nombre;
                        txtApellido.Text = m.Apellido;
                        txtDni.Text = m.Dni;
                        txtMatricula.Text = m.Matricula;
                        txtEmail.Text = m.Email ?? string.Empty;
                        txtTelefono.Text = m.Telefono ?? string.Empty;

                        // Si el turno del medico esta inactivo no figura en la lista: se agrega para poder mostrarlo.
                        string idTurno = m.TurnoTrabajo.Id.ToString();
                        if (ddlTurno.Items.FindByValue(idTurno) == null)
                            ddlTurno.Items.Add(new ListItem(m.TurnoTrabajo.Descripcion + " (inactivo)", idTurno));
                        ddlTurno.SelectedValue = idTurno;

                        cblEspecialidades.ClearSelection();
                        foreach (Especialidad esp in m.Especialidades)
                        {
                            ListItem item = cblEspecialidades.Items.FindByValue(esp.Id.ToString());
                            if (item == null)
                            {
                                item = new ListItem(esp.Descripcion + " (inactiva)", esp.Id.ToString());
                                cblEspecialidades.Items.Add(item);
                            }
                            item.Selected = true;
                        }

                        lblFormulario.Text = "Modificar médico";
                    }
                }
                else if (e.CommandName == "Baja")
                {
                    negocio.cambiarEstado(id, false);
                    cargarGrilla();
                }
                else if (e.CommandName == "Alta")
                {
                    negocio.cambiarEstado(id, true);
                    cargarGrilla();
                }
            }
            catch (ArgumentException ex)
            {
                // Regla de Negocio (por ejemplo, tiene turnos pendientes): se le muestra al usuario.
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

            Medico m = new Medico();
            m.Id = int.Parse(hfId.Value);
            m.Nombre = txtNombre.Text;
            m.Apellido = txtApellido.Text;
            m.Dni = txtDni.Text;
            m.Matricula = txtMatricula.Text;
            m.Email = txtEmail.Text;
            m.Telefono = txtTelefono.Text;

            m.TurnoTrabajo = new TurnoTrabajo();
            m.TurnoTrabajo.Id = int.Parse(ddlTurno.SelectedValue);

            foreach (ListItem item in cblEspecialidades.Items)
            {
                if (item.Selected)
                {
                    Especialidad esp = new Especialidad();
                    esp.Id = int.Parse(item.Value);
                    m.Especialidades.Add(esp);
                }
            }

            try
            {
                if (m.Id == 0)
                    negocio.agregar(m);
                else
                    negocio.modificar(m);

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
                mostrarError("No se pudo guardar el médico.");
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
            txtMatricula.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtTelefono.Text = string.Empty;
            lblFormulario.Text = "Nuevo médico";
            lblMensaje.Visible = false;
            // Se recargan las listas para sacar los items "(inactivo/a)" que pudo agregar la edicion.
            cargarCombos();
        }

        private void mostrarError(string mensaje)
        {
            lblMensaje.Text = mensaje;
            lblMensaje.Visible = true;
        }
    }
}
