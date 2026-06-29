using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaGestionRoles
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>

        public static ApplicationContext ContextoApp;

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            ContextoApp = new ApplicationContext();

            // Obtenemos la instancia del login
            FrmLogin login = FrmLogin.ObtenerInstancia();
            ContextoApp.MainForm = login;
            

            login.Show();
            Application.Run(ContextoApp);
        }
    }
}
