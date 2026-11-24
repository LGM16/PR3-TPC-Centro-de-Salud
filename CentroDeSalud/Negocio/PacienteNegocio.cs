using System;
using System.Collections.Generic;
using Dominio.Entidades;

namespace Negocio
{
    public class PacienteNegocio
    {
        // Lista todos (activos e inactivos): el ABM necesita ver tambien los dados de baja.
        public List<Paciente> listar()
        {
            List<Paciente> lista = new List<Paciente>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT Id, Nombre, Apellido, DNI, FechaNacimiento, Email, Telefono, Direccion, Activo FROM PACIENTES ORDER BY Apellido, Nombre");
                datos.ejecutarLectura();
                while (datos.Lector.Read())
                {
                    Paciente aux = new Paciente();

                    aux.Id = (int)datos.Lector["Id"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Apellido = (string)datos.Lector["Apellido"];
                    aux.Dni = (string)datos.Lector["DNI"];
                    aux.FechaNacimiento = (DateTime)datos.Lector["FechaNacimiento"];
                    aux.Email = (string)datos.Lector["Email"];
                    aux.Activo = (bool)datos.Lector["Activo"];

                    if (!(datos.Lector["Telefono"] is DBNull))
                        aux.Telefono = (string)datos.Lector["Telefono"];

                    if (!(datos.Lector["Direccion"] is DBNull))
                        aux.Direccion = (string)datos.Lector["Direccion"];

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
