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
                    // TODO(human): crear el TurnoTrabajo, cargarle las 5 columnas y agregarlo a la lista
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
    }
}
