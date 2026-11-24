using System;
using System.Collections.Generic;
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
    }
}
