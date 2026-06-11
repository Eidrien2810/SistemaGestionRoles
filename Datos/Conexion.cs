using System;
using System.Data;
using System.Data.SqlClient;

namespace SistemaGestionRoles.Datos
{
    /// <summary>
    /// Demonstrates how to work with SqlConnection objects
    /// </summary>
    class Conexion
    {
        private readonly string cadenaConexion =
            "Server=localhost;Database=SistemaGestionRolesDB;Trusted_Connection=True;";

        public SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadenaConexion);
        }
    }
}

