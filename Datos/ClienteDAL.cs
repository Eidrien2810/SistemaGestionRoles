using SistemaGestionRoles.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace SistemaGestionRoles.Datos
{
    public class ClienteDAL
    {
        private Conexion conexion = new Conexion();

        public DataTable ObtenerClientes()
        {
            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                string query = @"
                    SELECT 
                        IdCliente,
                        Nombre,
                        Telefono,
                        Correo,
                        Direccion,
                        Estado
                    FROM Clientes";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable tabla = new DataTable();
                        adapter.Fill(tabla);
                        return tabla;
                    }
                }
            }
        }

        public DataTable BuscarClientes(string filtro)
        {
            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                string query = @"
                    SELECT 
                        IdCliente,
                        Nombre,
                        Telefono,
                        Correo,
                        Direccion,
                        Estado
                    FROM Clientes
                    WHERE 
                        Nombre LIKE @Filtro
                        OR Telefono LIKE @Filtro
                        OR Correo LIKE @Filtro
                        OR CAST(IdCliente AS VARCHAR) LIKE @Filtro";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Filtro", "%" + filtro + "%");

                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable tabla = new DataTable();
                        adapter.Fill(tabla);
                        return tabla;
                    }
                }
            }
        }

        public bool InsertarCliente(Cliente cliente)
        {
            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                string query = @"
                    INSERT INTO Clientes (Nombre, Telefono, Correo, Direccion, Estado)
                    VALUES (@Nombre, @Telefono, @Correo, @Direccion, @Estado)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                    cmd.Parameters.AddWithValue("@Telefono", cliente.Telefono);
                    cmd.Parameters.AddWithValue("@Correo", cliente.Correo);
                    cmd.Parameters.AddWithValue("@Direccion", cliente.Direccion);
                    cmd.Parameters.AddWithValue("@Estado", cliente.Estado);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool ActualizarCliente(Cliente cliente)
        {
            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                string query = @"
                    UPDATE Clientes
                    SET 
                        Nombre = @Nombre,
                        Telefono = @Telefono,
                        Correo = @Correo,
                        Direccion = @Direccion,
                        Estado = @Estado
                    WHERE IdCliente = @IdCliente";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IdCliente", cliente.IdCliente);
                    cmd.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                    cmd.Parameters.AddWithValue("@Telefono", cliente.Telefono);
                    cmd.Parameters.AddWithValue("@Correo", cliente.Correo);
                    cmd.Parameters.AddWithValue("@Direccion", cliente.Direccion);
                    cmd.Parameters.AddWithValue("@Estado", cliente.Estado);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool EliminarCliente(int idCliente)
        {
            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                string query = "DELETE FROM Clientes WHERE IdCliente = @IdCliente";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@IdCliente", idCliente);

                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}