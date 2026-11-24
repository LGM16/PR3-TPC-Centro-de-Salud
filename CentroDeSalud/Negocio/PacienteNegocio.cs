using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

        public void agregar(Paciente nuevo)
        {
            validar(nuevo);
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("INSERT INTO PACIENTES (Nombre, Apellido, DNI, FechaNacimiento, Email, Telefono, Direccion, Activo) VALUES (@Nombre, @Apellido, @Dni, @FechaNacimiento, @Email, @Telefono, @Direccion, 1)");
                cargarParametros(datos, nuevo);
                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void modificar(Paciente paciente)
        {
            validar(paciente);
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("UPDATE PACIENTES SET Nombre = @Nombre, Apellido = @Apellido, DNI = @Dni, FechaNacimiento = @FechaNacimiento, Email = @Email, Telefono = @Telefono, Direccion = @Direccion WHERE Id = @Id");
                cargarParametros(datos, paciente);
                datos.setearParametro("@Id", paciente.Id);
                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        private void cargarParametros(AccesoDatos datos, Paciente p)
        {
            datos.setearParametro("@Nombre", p.Nombre.Trim());
            datos.setearParametro("@Apellido", p.Apellido.Trim());
            datos.setearParametro("@Dni", p.Dni.Trim());
            datos.setearParametro("@FechaNacimiento", p.FechaNacimiento.Date);
            datos.setearParametro("@Email", p.Email.Trim());
            datos.setearParametro("@Telefono", valorOpcional(p.Telefono));
            datos.setearParametro("@Direccion", valorOpcional(p.Direccion));
        }

        // Telefono y Direccion son opcionales: si vienen vacios se guarda NULL, no un string vacio.
        private object valorOpcional(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return DBNull.Value;
            return texto.Trim();
        }

        // Las reglas viven en Negocio: la pantalla ayuda, pero no es la que manda.
        private void validar(Paciente p)
        {
            if (p == null || string.IsNullOrWhiteSpace(p.Nombre) || string.IsNullOrWhiteSpace(p.Apellido))
                throw new ArgumentException("El nombre y el apellido son obligatorios.");
            if (string.IsNullOrWhiteSpace(p.Dni))
                throw new ArgumentException("El DNI es obligatorio.");
            if (string.IsNullOrWhiteSpace(p.Email))
                throw new ArgumentException("El email es obligatorio.");

            if (p.Nombre.Trim().Length > 50 || p.Apellido.Trim().Length > 50)
                throw new ArgumentException("El nombre y el apellido no pueden superar los 50 caracteres.");
            if (p.Email.Trim().Length > 100)
                throw new ArgumentException("El email no puede superar los 100 caracteres.");
            if (!string.IsNullOrWhiteSpace(p.Telefono) && p.Telefono.Trim().Length > 30)
                throw new ArgumentException("El teléfono no puede superar los 30 caracteres.");
            if (!string.IsNullOrWhiteSpace(p.Direccion) && p.Direccion.Trim().Length > 100)
                throw new ArgumentException("La dirección no puede superar los 100 caracteres.");

            // TODO(human): validar los formatos. Lanzar ArgumentException con un mensaje claro si:
            //   1) el DNI no tiene entre 7 y 8 digitos (solo numeros)
            //   2) el email no tiene un formato valido (algo@dominio.ext)
            //   3) la fecha de nacimiento es futura (comparar con DateTime.Today)

            // DNI unico. "Id <> @Id" evita que un paciente choque consigo mismo al modificarlo.
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT COUNT(*) FROM PACIENTES WHERE DNI = @Dni AND Id <> @Id");
                datos.setearParametro("@Dni", p.Dni.Trim());
                datos.setearParametro("@Id", p.Id);
                if ((int)datos.ejecutarEscalar() > 0)
                    throw new ArgumentException("Ya existe un paciente con ese DNI.");
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}
