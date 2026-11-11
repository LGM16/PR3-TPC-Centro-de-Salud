using System;
using System.Collections.Generic;
using Dominio.Entidades;

namespace Negocio
{
    public class EspecialidadNegocio
    {
        // Lista todas (activas e inactivas): el ABM necesita ver tambien las dadas de baja.
        public List<Especialidad> listar()
        {
            List<Especialidad> lista = new List<Especialidad>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT Id, Descripcion, Activo FROM ESPECIALIDADES ORDER BY Descripcion");
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    Especialidad aux = new Especialidad();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    aux.Activo = (bool)datos.Lector["Activo"];
                    lista.Add(aux);
                }
                return lista;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void agregar(Especialidad nueva)
        {
            validar(nueva);
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("INSERT INTO ESPECIALIDADES (Descripcion, Activo) VALUES (@Descripcion, 1)");
                datos.setearParametro("@Descripcion", nueva.Descripcion.Trim());
                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void modificar(Especialidad esp)
        {
            validar(esp);
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("UPDATE ESPECIALIDADES SET Descripcion = @Descripcion WHERE Id = @Id");
                datos.setearParametro("@Descripcion", esp.Descripcion.Trim());
                datos.setearParametro("@Id", esp.Id);
                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        // Baja logica (activo = false) o reactivacion (activo = true). Nunca se borra la fila.
        public void cambiarEstado(int id, bool activo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("UPDATE ESPECIALIDADES SET Activo = @Activo WHERE Id = @Id");
                datos.setearParametro("@Activo", activo);
                datos.setearParametro("@Id", id);
                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        // Requerida y sin repetir (la base tiene UNIQUE, pero asi damos un mensaje claro).
        private void validar(Especialidad esp)
        {
            if (esp == null || string.IsNullOrWhiteSpace(esp.Descripcion))
                throw new ArgumentException("La descripción de la especialidad es obligatoria.");
            if (esp.Descripcion.Trim().Length > 50)
                throw new ArgumentException("La descripción no puede superar los 50 caracteres.");

            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT COUNT(*) FROM ESPECIALIDADES WHERE Descripcion = @Descripcion AND Id <> @Id");
                datos.setearParametro("@Descripcion", esp.Descripcion.Trim());
                datos.setearParametro("@Id", esp.Id);
                if ((int)datos.ejecutarEscalar() > 0)
                    throw new ArgumentException("Ya existe una especialidad con esa descripción.");
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}
