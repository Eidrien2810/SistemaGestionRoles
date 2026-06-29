using SistemaGestionRoles.Entidades;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SistemaGestionRoles.Presentacion
{
    public partial class FrmProductos : Form
    {
        Dictionary<int, string> roleMessage = new Dictionary<int, string>()
        {
            { 1, "Administrador del sistema" },
            { 2, "Supervisor" },
            { 3, "Ejecutor" },
            { 4, "Invitado" }

        };
        public Usuario sessionUser;

        public FrmProductos(Usuario user)
        {
            InitializeComponent();
            sessionUser = user;
            // Activar el dobluebuffer para eliminar el parpadeo de colores a la hora de pintar la ui
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);
            this.UpdateStyles();
        }
        private static FrmProductos _instancia = null;
        public static FrmProductos ObtenerInstancia(Usuario sessionUser)
        {
            // Si no existe o fue cerrado (destruido), creamos uno nuevo
            if (_instancia == null || _instancia.IsDisposed)
            {
                _instancia = new FrmProductos(sessionUser);
            }

            return _instancia;
        }
        private void FrmProductos_Load(object sender, EventArgs e) { 
            // Maximizar la ventana desde que cargue el formulario   
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Maximized;

            // Los label tanto del nombre de usuario como del rol se les asigna su texto correspondiente
            lblUsername.Text = sessionUser.NombreUsuario;
            lblRole.Text = roleMessage[sessionUser.IdRol];

            // Se cambia el avatar del usuario dependiendo del id de su rol (1: admin, 2: supervisor, 3: ejecutor); 
            // Nota: esto es experimental; ya que, en un punto del proyecto a la hora de implementar el formulario de sign in deberia dejar seleccionar una foto de perfil
            switch (sessionUser.IdRol)
            {
                case 1:
                    // Se cambia la imagen segun el rol, se accede en la carpeta resources
                    userAvatar.Image = Properties.Resources.admin;
                    break;
                case 2:
                    // Se cambia la imagen segun el rol, se accede en la carpeta resources
                    userAvatar.Image = Properties.Resources.supervisor;
                  
                    break;

                case 3:
                    // Se cambia la imagen segun el rol, se accede en la carpeta resources
                    userAvatar.Image = Properties.Resources.ejecutor;
                   
                    break;
                default:
                    break;
            }
        }
        // Funcion que se ejecuta en el evento click, para mover al usuario a una nueva instancia del modulo de usuarios
        private void pnlClientes_Click(object sender, EventArgs e)
        {
            FrmMenuPrincipal nuevoFormulario = FrmMenuPrincipal.ObtenerInstancia(sessionUser);
            Program.ContextoApp.MainForm = nuevoFormulario;
            nuevoFormulario.Show();
            nuevoFormulario.BringToFront();
            this.Hide();
        }
        // Funcion que se ejecuta en el evento click, para mover al usuario a una nueva instancia del modulo de clientes

        private void pnlUsuarios_Click(object sender, EventArgs e)
        {
            var menuUsuarios = FrmUsuarios.ObtenerInstancia(sessionUser);
            Program.ContextoApp.MainForm = menuUsuarios;
            menuUsuarios.Show();
            menuUsuarios.BringToFront();
            this.Hide();
        }
        // Funcion de Logout, primero pregunta al usuario si quiere cerrar la sesion si es verdadero lo mueve a la ventana de login, si es falso lo deja en la misma ventana
        private void ExitClick()
        {
            DialogResult result = MessageBox.Show(
                    "Quieres salir de la aplicacion?",
                    "Confirmacion de salida",
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Question
                );
            if (result == DialogResult.OK)
            {
                this.Close();
            }

        }

        private void pnlExit_Click(object sender, EventArgs e)
        {
            ExitClick();
        }

        
    }
}
