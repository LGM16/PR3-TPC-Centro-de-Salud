using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Negocio
{
    // Centraliza la conexion a CLINICA_DB. La cadena vive en Web.config (clave "CLINICA_DB"),
    // asi que si cambia el servidor se edita en un solo lugar.
    public class AccesoDatos
    {
        private SqlConnection conexion;
        private SqlCommand comando;
        private SqlDataReader lector;

        public SqlDataReader Lector
        {
            get { return lector; }
        }

        public AccesoDatos()
        {
            string cadena = ConfigurationManager.ConnectionStrings["CLINICA_DB"].ConnectionString;
            conexion = new SqlConnection(cadena);
            comando = new SqlCommand();
        }

        public void setearConsulta(string consulta)
        {
            comando.CommandType = System.Data.CommandType.Text;
            comando.CommandText = consulta;
        }

        public void setearParametro(string nombre, object valor)
        {
            comando.Parameters.AddWithValue(nombre, valor);
        }

        public void ejecutarLectura()
        {
            comando.Connection = conexion;
            conexion.Open();
            lector = comando.ExecuteReader();
        }

        // Para INSERT, UPDATE y DELETE. Devuelve la cantidad de filas afectadas.
        public int ejecutarAccion()
        {
            comando.Connection = conexion;
            conexion.Open();
            return comando.ExecuteNonQuery();
        }

        // Para consultas que devuelven un unico valor (por ejemplo COUNT).
        public object ejecutarEscalar()
        {
            comando.Connection = conexion;
            conexion.Open();
            return comando.ExecuteScalar();
        }

        public void cerrarConexion()
        {
            if (lector != null)
                lector.Close();
            conexion.Close();
        }
    }
}
