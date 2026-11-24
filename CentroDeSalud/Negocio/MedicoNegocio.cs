using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Dominio.Entidades;

namespace Negocio
{
    public class MedicoNegocio
    {
        // Lista todos (activos e inactivos) con su turno de trabajo y sus especialidades.
        // Un medico con dos especialidades llega en DOS filas: la consulta repite sus datos
        // y cambia solo la especialidad. Hay que agrupar las filas en un unico Medico.
        public List<Medico> listar()
        {
            List<Medico> lista = new List<Medico>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(@"SELECT M.Id, M.Nombre, M.Apellido, M.DNI, M.Matricula, M.Email, M.Telefono, M.Activo,
                                              T.Id AS IdTurno, T.Descripcion AS Turno,
                                              E.Id AS IdEsp, E.Descripcion AS Esp
                                       FROM MEDICOS M
                                       INNER JOIN TURNOS_TRABAJO T ON T.Id = M.IdTurnoTrabajo
                                       LEFT JOIN MEDICOS_ESPECIALIDADES ME ON ME.IdMedico = M.Id
                                       LEFT JOIN ESPECIALIDADES E ON E.Id = ME.IdEspecialidad
                                       ORDER BY M.Apellido, M.Nombre, M.Id");
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    int id = (int)datos.Lector["Id"];
                    Medico aux = lista.Find(x => x.Id == id);

                    if (aux == null)
                    {
                        aux = cargarMedico(datos);
                        lista.Add(aux);
                    }

                    if (!(datos.Lector["IdEsp"] is DBNull))
                    {
                        Especialidad esp = new Especialidad();
                        esp.Id = (int)datos.Lector["IdEsp"];
                        esp.Descripcion = (string)datos.Lector["Esp"];
                        aux.Especialidades.Add(esp);
                    }
                }
                return lista;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        // Arma un Medico con los datos de la fila actual del lector (sin sus especialidades).
        private Medico cargarMedico(AccesoDatos datos)
        {
            Medico m = new Medico();
            m.Id = (int)datos.Lector["Id"];
            m.Nombre = (string)datos.Lector["Nombre"];
            m.Apellido = (string)datos.Lector["Apellido"];
            m.Dni = (string)datos.Lector["DNI"];
            m.Matricula = (string)datos.Lector["Matricula"];
            m.Activo = (bool)datos.Lector["Activo"];

            // Email y Telefono admiten NULL en la base.
            if (!(datos.Lector["Email"] is DBNull))
                m.Email = (string)datos.Lector["Email"];
            if (!(datos.Lector["Telefono"] is DBNull))
                m.Telefono = (string)datos.Lector["Telefono"];

            m.TurnoTrabajo = new TurnoTrabajo();
            m.TurnoTrabajo.Id = (int)datos.Lector["IdTurno"];
            m.TurnoTrabajo.Descripcion = (string)datos.Lector["Turno"];
            return m;
        }

        // Baja logica (activo = false) o reactivacion (activo = true). Nunca se borra la fila.
        public void cambiarEstado(int id, bool activo)
        {
            if (!activo)
            {
                if (!activo)
                {
                    AccesoDatos datosValidacion = new AccesoDatos();
                    try
                    {
                        datosValidacion.setearConsulta("SELECT COUNT(*) FROM TURNOS WHERE IdMedico = @Id AND IdEstado IN (1, 2) AND Fecha >= CAST(GETDATE() AS DATE)");
                        datosValidacion.setearParametro("@Id", id);

                        int cantidad = Convert.ToInt32(datosValidacion.ejecutarEscalar());

                        if (cantidad > 0)
                            throw new ArgumentException("No se puede dar de baja al médico porque tiene turnos pendientes.");
                    }
                    finally
                    {
                        datosValidacion.cerrarConexion();
                    }
                }
            }

            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("UPDATE MEDICOS SET Activo = @Activo WHERE Id = @Id");
                datos.setearParametro("@Activo", activo);
                datos.setearParametro("@Id", id);
                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        // El medico y sus especialidades se guardan JUNTOS, en una transaccion: o se graba todo o no se graba nada.
        // Si fallara un INSERT de especialidad, no puede quedar un medico guardado sin especialidades.
        public void agregar(Medico nuevo)
        {
            validar(nuevo);
            AccesoDatos datos = new AccesoDatos();
            try
            {
                string consulta = "SET XACT_ABORT ON; BEGIN TRAN; " +
                    "INSERT INTO MEDICOS (Nombre, Apellido, DNI, Matricula, Email, Telefono, IdTurnoTrabajo, Activo) " +
                    "VALUES (@Nombre, @Apellido, @Dni, @Matricula, @Email, @Telefono, @IdTurno, 1); " +
                    "DECLARE @IdMedico INT = SCOPE_IDENTITY(); " +
                    insertarEspecialidades(datos, nuevo.Especialidades) +
                    "COMMIT;";
                datos.setearConsulta(consulta);
                cargarParametros(datos, nuevo);
                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        // Al modificar: se actualizan los datos, se borran las especialidades que tenia y se insertan las nuevas.
        public void modificar(Medico medico)
        {
            validar(medico);
            AccesoDatos datos = new AccesoDatos();
            try
            {
                string consulta = "SET XACT_ABORT ON; BEGIN TRAN; " +
                    "UPDATE MEDICOS SET Nombre = @Nombre, Apellido = @Apellido, DNI = @Dni, Matricula = @Matricula, " +
                    "Email = @Email, Telefono = @Telefono, IdTurnoTrabajo = @IdTurno WHERE Id = @IdMedico; " +
                    "DELETE FROM MEDICOS_ESPECIALIDADES WHERE IdMedico = @IdMedico; " +
                    insertarEspecialidades(datos, medico.Especialidades) +
                    "COMMIT;";
                datos.setearConsulta(consulta);
                cargarParametros(datos, medico);
                datos.setearParametro("@IdMedico", medico.Id);
                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        // Devuelve el texto SQL con UN insert en MEDICOS_ESPECIALIDADES por cada especialidad del medico
        // y, a la vez, le agrega a 'datos' un parametro por cada una (@Esp0, @Esp1, ...).
        // El insert usa @IdMedico, que ya existe en el texto SQL de agregar() y de modificar().
        private string insertarEspecialidades(AccesoDatos datos, List<Especialidad> especialidades)
        {
            string consulta = "";

            for (int i = 0; i < especialidades.Count; i++)
            {
                consulta += "INSERT INTO MEDICOS_ESPECIALIDADES (IdMedico, IdEspecialidad) VALUES (@IdMedico, @Esp" + i + "); ";
                datos.setearParametro("@Esp" + i, especialidades[i].Id);
            }

            return consulta;
        }

        private void cargarParametros(AccesoDatos datos, Medico m)
        {
            datos.setearParametro("@Nombre", m.Nombre.Trim());
            datos.setearParametro("@Apellido", m.Apellido.Trim());
            datos.setearParametro("@Dni", m.Dni.Trim());
            datos.setearParametro("@Matricula", m.Matricula.Trim());
            datos.setearParametro("@Email", valorOpcional(m.Email));
            datos.setearParametro("@Telefono", valorOpcional(m.Telefono));
            datos.setearParametro("@IdTurno", m.TurnoTrabajo.Id);
        }

        // Email y Telefono son opcionales: si vienen vacios se guarda NULL, no un string vacio.
        private object valorOpcional(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return DBNull.Value;
            return texto.Trim();
        }

        // Las reglas viven en Negocio: la pantalla ayuda, pero no es la que manda.
        private void validar(Medico m)
        {
            if (m == null || string.IsNullOrWhiteSpace(m.Nombre) || string.IsNullOrWhiteSpace(m.Apellido))
                throw new ArgumentException("El nombre y el apellido son obligatorios.");
            if (string.IsNullOrWhiteSpace(m.Dni))
                throw new ArgumentException("El DNI es obligatorio.");
            if (string.IsNullOrWhiteSpace(m.Matricula))
                throw new ArgumentException("La matrícula es obligatoria.");

            if (m.Nombre.Trim().Length > 50 || m.Apellido.Trim().Length > 50)
                throw new ArgumentException("El nombre y el apellido no pueden superar los 50 caracteres.");
            if (m.Matricula.Trim().Length > 20)
                throw new ArgumentException("La matrícula no puede superar los 20 caracteres.");
            if (!string.IsNullOrWhiteSpace(m.Email) && m.Email.Trim().Length > 100)
                throw new ArgumentException("El email no puede superar los 100 caracteres.");
            if (!string.IsNullOrWhiteSpace(m.Telefono) && m.Telefono.Trim().Length > 30)
                throw new ArgumentException("El teléfono no puede superar los 30 caracteres.");

            if (!Regex.IsMatch(m.Dni.Trim(), @"^[0-9]{7,8}$"))
                throw new ArgumentException("El DNI debe tener entre 7 y 8 dígitos.");
            if (!string.IsNullOrWhiteSpace(m.Email) && !Regex.IsMatch(m.Email.Trim(), @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                throw new ArgumentException("El email no tiene un formato válido.");

            if (m.TurnoTrabajo == null || m.TurnoTrabajo.Id <= 0)
                throw new ArgumentException("El turno de trabajo es obligatorio.");
            if (m.Especialidades == null || m.Especialidades.Count == 0)
                throw new ArgumentException("El médico debe tener al menos una especialidad.");

            // DNI y matricula unicos. "Id <> @Id" evita que un medico choque consigo mismo al modificarlo.
            if (contar("SELECT COUNT(*) FROM MEDICOS WHERE DNI = @Valor AND Id <> @Id", m.Dni.Trim(), m.Id) > 0)
                throw new ArgumentException("Ya existe un médico con ese DNI.");
            if (contar("SELECT COUNT(*) FROM MEDICOS WHERE Matricula = @Valor AND Id <> @Id", m.Matricula.Trim(), m.Id) > 0)
                throw new ArgumentException("Ya existe un médico con esa matrícula.");
        }

        private int contar(string consulta, string valor, int id)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta(consulta);
                datos.setearParametro("@Valor", valor);
                datos.setearParametro("@Id", id);
                return (int)datos.ejecutarEscalar();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}
