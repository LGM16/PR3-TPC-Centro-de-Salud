using System;
using System.Collections.Generic;
using Dominio.Entidades;

namespace Negocio
{
    public class TurnoTrabajoNegocio
    {
        // Lista todos (activos e inactivos): el ABM necesita ver tambien los dados de baja.
        public List<TurnoTrabajo> listar()
        {
            List<TurnoTrabajo> lista = new List<TurnoTrabajo>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT Id, Descripcion, HoraEntrada, HoraSalida, Activo FROM TURNOS_TRABAJO ORDER BY HoraEntrada");
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    TurnoTrabajo aux = new TurnoTrabajo();

                    aux.Id = (int)datos.Lector["Id"];
                    aux.Descripcion = (string)datos.Lector["Descripcion"];
                    aux.HoraEntrada = (TimeSpan)datos.Lector["HoraEntrada"];
                    aux.HoraSalida = (TimeSpan)datos.Lector["HoraSalida"];
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

        public void agregar(TurnoTrabajo nuevo)
        {
            validar(nuevo);
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("INSERT INTO TURNOS_TRABAJO (Descripcion, HoraEntrada, HoraSalida, Activo) VALUES (@Descripcion, @HoraEntrada, @HoraSalida, 1)");
                datos.setearParametro("@Descripcion", nuevo.Descripcion.Trim());
                datos.setearParametro("@HoraEntrada", nuevo.HoraEntrada);
                datos.setearParametro("@HoraSalida", nuevo.HoraSalida);
                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void modificar(TurnoTrabajo turno)
        {
            validar(turno);
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("UPDATE TURNOS_TRABAJO SET Descripcion = @Descripcion, HoraEntrada = @HoraEntrada, HoraSalida = @HoraSalida WHERE Id = @Id");
                datos.setearParametro("@Descripcion", turno.Descripcion.Trim());
                datos.setearParametro("@HoraEntrada", turno.HoraEntrada);
                datos.setearParametro("@HoraSalida", turno.HoraSalida);
                datos.setearParametro("@Id", turno.Id);
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
            if (!activo)
            {
                AccesoDatos datosValidacion = new AccesoDatos();

                try
                {
                    datosValidacion.setearConsulta(
                        "SELECT COUNT(*) AS CantidadMedicos FROM MEDICOS WHERE IdTurnoTrabajo = @Id AND Activo = 1"
                    );

                    datosValidacion.setearParametro("@Id", id);
                    datosValidacion.ejecutarLectura();

                    if (datosValidacion.Lector.Read())
                    {
                        int cantidad = (int)datosValidacion.Lector["CantidadMedicos"];

                        if (cantidad > 0)
                        {
                            throw new ArgumentException("El turno tiene médicos activos asociados.");
                        }
                    }
                }
                finally
                {
                    datosValidacion.cerrarConexion();
                }

            }

            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("UPDATE TURNOS_TRABAJO SET Activo = @Activo WHERE Id = @Id");
                datos.setearParametro("@Activo", activo);
                datos.setearParametro("@Id", id);
                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        // Las reglas viven en Negocio: la pantalla ayuda, pero no es la que manda.
        // La base tambien las frena con CHECK, pero asi el usuario recibe un mensaje claro.
        private void validar(TurnoTrabajo turno)
        {
            if (turno == null || string.IsNullOrWhiteSpace(turno.Descripcion))
                throw new ArgumentException("La descripción del turno de trabajo es obligatoria.");
            if (turno.Descripcion.Trim().Length > 50)
                throw new ArgumentException("La descripción no puede superar los 50 caracteres.");

            
            if (turno.HoraEntrada >= turno.HoraSalida)
                throw new ArgumentException("La hora de entrada debe ser anterior a la hora de salida.");

            if (turno.HoraEntrada.Minutes != 0 || turno.HoraEntrada.Seconds != 0)
                throw new ArgumentException("La hora de entrada debe ser una hora entera (minutos y segundos en cero).");
            if (turno.HoraSalida.Minutes != 0 || turno.HoraSalida.Seconds != 0)
                throw new ArgumentException("La hora de salida debe ser una hora entera (minutos y segundos en cero).");
        }
    }
}
