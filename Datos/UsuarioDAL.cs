using SistemaGestionRoles.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace SistemaGestionRoles.Datos
{
    public class UsuarioDAL
    {
        private readonly Conexion conexion = new Conexion();

        public List<Usuario> ObtenerUsuarios()
        {
            List<Usuario> listaUsuarios = new List<Usuario>();

            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                cn.Open();

                string query = @"
                    SELECT 
                        U.IdUsuario,
                        U.NombreUsuario,
                        U.Clave,
                        U.IdRol,
                        R.NombreRol,
                        U.IntentosFallidos,
                        U.Activo
                    FROM Usuarios U
                    INNER JOIN Roles R ON U.IdRol = R.IdRol";

                using (SqlCommand cmd = new SqlCommand(query, cn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Usuario usuario = new Usuario();

                            usuario.IdUsuario = (int)reader["IdUsuario"];
                            usuario.NombreUsuario = reader["NombreUsuario"].ToString();
                            usuario.Clave = reader["Clave"].ToString();
                            usuario.IdRol = (int)reader["IdRol"];
                            usuario.NombreRol = reader["NombreRol"].ToString();
                            usuario.IntentosFallidos = (int)reader["IntentosFallidos"];
                            usuario.Activo = (bool)reader["Activo"];

                            listaUsuarios.Add(usuario);
                        }
                    }
                }
            }

            return listaUsuarios;
        }
        public void IncrementarIntentosFallidos(string nombreUsuario)
        {
            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                string query = @"
            UPDATE Usuarios
            SET IntentosFallidos = IntentosFallidos + 1
            WHERE NombreUsuario = @NombreUsuario";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void BloquearUsuarioTemporalmente(string nombreUsuario, DateTime bloqueadoHasta)
        {
            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                string query = @"
            UPDATE Usuarios
            SET BloqueadoHasta = @BloqueadoHasta
            WHERE NombreUsuario = @NombreUsuario";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                    cmd.Parameters.AddWithValue("@BloqueadoHasta", bloqueadoHasta);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void ReiniciarIntentosFallidos(string nombreUsuario)
        {
            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                string query = @"
            UPDATE Usuarios
            SET 
                IntentosFallidos = 0,
                BloqueadoHasta = NULL,
                UltimoAcceso = GETDATE()
            WHERE NombreUsuario = @NombreUsuario";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void DesactivarUsuario(string nombreUsuario)
        {
            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                string query = @"
            UPDATE Usuarios
            SET Activo = 0
            WHERE NombreUsuario = @NombreUsuario";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public Usuario ObtenerUsuarioPorNombre(string nombreUsuario)
        {
            Usuario usuario = null;

            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                string query = @"
            SELECT 
                U.IdUsuario,
                U.NombreUsuario,
                U.Clave,
                U.IdRol,
                R.NombreRol,
                U.IntentosFallidos,
                U.Activo,
                U.BloqueadoHasta,
                U.UltimoAcceso
            FROM Usuarios U
            INNER JOIN Roles R ON U.IdRol = R.IdRol
            WHERE U.NombreUsuario = @NombreUsuario";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                IdUsuario = Convert.ToInt32(reader["IdUsuario"]),
                                NombreUsuario = reader["NombreUsuario"].ToString(),
                                Clave = reader["Clave"].ToString(),
                                IdRol = Convert.ToInt32(reader["IdRol"]),
                                NombreRol = reader["NombreRol"].ToString(),
                                IntentosFallidos = Convert.ToInt32(reader["IntentosFallidos"]),
                                Activo = Convert.ToBoolean(reader["Activo"])
                            };

                            if (reader["BloqueadoHasta"] != DBNull.Value)
                            {
                                usuario.BloqueadoHasta = Convert.ToDateTime(reader["BloqueadoHasta"]);
                            }
                        }
                    }
                }
            }

            return usuario;
        }
        public Usuario ValidarCredenciales(string nombreUsuario, string clave)
        {
            Usuario usuario = null;

            using (SqlConnection conn = conexion.ObtenerConexion())
            {
                string query = @"
                    SELECT 
                        U.IdUsuario,
                        U.NombreUsuario,
                        U.Clave,
                        U.IdRol,
                        R.NombreRol,
                        U.IntentosFallidos,
                        U.Activo
                    FROM Usuarios U
                    INNER JOIN Roles R ON U.IdRol = R.IdRol
                    WHERE U.NombreUsuario = @NombreUsuario
                    AND U.Clave = @Clave";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                    cmd.Parameters.AddWithValue("@Clave", clave);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                IdUsuario = Convert.ToInt32(reader["IdUsuario"]),
                                NombreUsuario = reader["NombreUsuario"].ToString(),
                                Clave = reader["Clave"].ToString(),
                                IdRol = Convert.ToInt32(reader["IdRol"]),
                                NombreRol = reader["NombreRol"].ToString(),
                                IntentosFallidos = Convert.ToInt32(reader["IntentosFallidos"]),
                                Activo = Convert.ToBoolean(reader["Activo"])
                            };
                        }
                    }
                }
            }

            return usuario;
        }
    }
}