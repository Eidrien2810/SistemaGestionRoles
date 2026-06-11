using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SistemaGestionRoles.Entidades
{
    public class Usuario
    {
        /* public Usuario(int idUsuario, string nombreUsuario, string clave, int idRol, string nombreRol, int intentosFallidos, bool activo)
         {
             IdUsuario = idUsuario;
             NombreUsuario = nombreUsuario;
             Clave = clave;
             IdRol = idRol;
             NombreRol = nombreRol;
             IntentosFallidos = intentosFallidos;
             Activo = activo
         }*/
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Clave { get; set; }
        public int IdRol { get; set; }
        public string NombreRol { get; set; }
        public int IntentosFallidos { get; set; }
        public bool Activo { get; set; }
        public DateTime? BloqueadoHasta { get; set; }
        public bool IsUsernameValid(string username)
        {
            string patron = @"^(?:[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}|[a-zA-Z0-9_]{3,16})$";
            if (!Regex.IsMatch(username.Trim(), patron))
            {
                return false;
            }
            return true;
        }
        public bool IsPasswordValid(string password)
        {
            string trimmedPassword = password;
            if (string.IsNullOrWhiteSpace(trimmedPassword) || trimmedPassword.Length < 8 || trimmedPassword.Length > 20)
            {
                return false;
            }
            return true;
        }
    }
}
