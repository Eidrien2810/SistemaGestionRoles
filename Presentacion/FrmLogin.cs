using SistemaGestionRoles.Entidades;
using SistemaGestionRoles.Datos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SistemaGestionRoles
{
    public partial class FrmLogin : Form
    {
        private string txtPasswordPlaceholderText = "Ingresa tu contraseña";
        private string txtUserPlaceholderText = "Ingresa tu nombre de usuario";
        public FrmLogin()
        {
            InitializeComponent();
        }
        private void picTogglePassword_MouseDown(object sender, MouseEventArgs e)
        { }
        private void picTogglePassword_MouseUp(object sender, MouseEventArgs e)
        { }
        private void btnExit(object sender, EventArgs e)
        { }
        private void panelLogin_Paint(object sender, PaintEventArgs e)
        { }
        private void btnLogIn_Click(object sender, EventArgs e)
        { }
        private void btnProbarConexion_Click(object sender, EventArgs e)
        { }
        private void txtPassword_Enter(object sender, EventArgs e)
        { }
        private void txtUser_Enter(object sender, EventArgs e)
        { }
        private void txtUser_Leave(object sender, EventArgs e)
        { }
        private void txtPassword_Leave(object sender, EventArgs e)
        { }
        private void lblUsername_Click(object sender, EventArgs e)
        { }
        private void lblPassword_Click(object sender, EventArgs e)
        { }
        private void txtPassword_KeyUp(object sender, KeyEventArgs e)
        { }
        private void txtUser_KeyUp(object sender, KeyEventArgs e)
        { }
        private void btnMenu_Click(object sender, EventArgs e)
        { }
        private void pnlPassword_Click(object sender, EventArgs e)
        { }



        public Usuario sessionFormUser;
        private void Form1_Resize(object sender, EventArgs e)
        {

            pictureBox1.Size = new Size(this.Width / 2, this.Height);
            panelLogin.Left = (this.ClientSize.Width - panelLogin.Width) / 2;
            panelLogin.Top = (this.ClientSize.Height - panelLogin.Height) / 2;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Evitar que el diseñador cargue recursos que causen fallo en tiempo de diseño
            if (!this.DesignMode)
            {
                try
                {
                    this.pictureBox1.Image = SistemaGestionRoles.Properties.Resources.image_login;
                }
                catch
                {
                    // Ignorar errores al cargar la imagen en tiempo de ejecución
                }
            }
            txtUsuario.Text = txtUserPlaceholderText;
            txtUsuario.ForeColor = Color.Gray;
            txtContrasena.Text = txtPasswordPlaceholderText; 
            txtContrasena.ForeColor = Color.Gray;

            txtContrasena.AutoSize = false;
            txtContrasena.Size = new Size(172, 23);
        }

        
        private void signIn_Click(object sender, EventArgs e)
        {
            LogIn();
        }
        private void lblSignIn_Click(object sender, EventArgs e)
        {
            LogIn();
        }
                
        private void btnSalir_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        
              
        private void txtContrasena_Enter(object sender, EventArgs e)
        {
            if (txtContrasena.Text == txtPasswordPlaceholderText)
            {
                //txtContrasena.PasswordChar = '*';
                txtContrasena.UseSystemPasswordChar = true;
                txtContrasena.Text = "";
                txtContrasena.ForeColor = Color.Black;
                txtContrasena.BackColor = Color.White;
                pnlContrasena.BackColor = Color.White;
            }
            //txtContrasena.Select();
        }
        private void txtUsuario_Enter(object sender, EventArgs e)
        {
            if (txtUsuario.Text == txtUserPlaceholderText)
            {
                txtUsuario.Text = "";
                txtUsuario.ForeColor = Color.Black;
                txtUsuario.BackColor = Color.White;
            }
            //txtUsuario.Select();
        }

        private void txtUsuario_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsuario.Text))
            {
                txtUsuario.Text = txtUserPlaceholderText;
                txtUsuario.ForeColor = Color.Gray;
            }
            this.ActiveControl = null;
        }

        private void txtContrasena_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtContrasena.Text))
            {
                //txtContrasena.PasswordChar = '\0';
                txtContrasena.UseSystemPasswordChar = false;
                txtContrasena.Text = txtPasswordPlaceholderText;
                txtContrasena.ForeColor = Color.Gray;
            }
        }
        
        private void lblUsuario_Click(object sender, EventArgs e)
        {
            txtUsuario.Select();
        }

        private void lblContrasena_Click(object sender, EventArgs e)
        {
            txtContrasena.Select();
        }

       
        private void pbToggleContrasena_MouseUp(object sender, MouseEventArgs e)
        {
            picTogglePassword.Image = SistemaGestionRoles.Properties.Resources.ojo_cerrado;
            txtContrasena.UseSystemPasswordChar = true;
        }

        private void pbToggleContrasena_MouseDown(object sender, MouseEventArgs e)
        {
            picTogglePassword.Image = SistemaGestionRoles.Properties.Resources.ojo_abierto;
            txtContrasena.UseSystemPasswordChar = false;
        }
        
        
        private void LogIn()
        {
            string username = txtUsuario.Text.Trim();
            string password = txtContrasena.Text;

            Usuario user = new Usuario();

            if (txtUsuario.Text.Length > 28)
            {
                txtUsuario.ForeColor = Color.Black;
            }
            else
            {
                txtUsuario.ForeColor = Color.Gray;
            }
            
            if(txtContrasena.Text.Length > 21)
            {
                txtContrasena.ForeColor = Color.Black;
            }
            else
            {
                txtUsuario.ForeColor = Color.Gray;
            }
            txtUsuario.BackColor = Color.White;
            txtContrasena.BackColor = Color.White;

            if (!user.IsUsernameValid(username))
            {
                MessageBox.Show(
                    "El texto debe ser un email válido o un usuario de 3 a 16 caracteres, sin espacios.",
                    "Usuario inválido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtUsuario.Select();
                txtUsuario.BackColor = Color.LightSalmon;
                txtUsuario.ForeColor = Color.Red;
                return;
            }

            if (!user.IsPasswordValid(password))
            {
                MessageBox.Show(
                    "La contraseña no puede estar vacía ni contener solo espacios, y debe tener entre 8 y 20 caracteres.",
                    "Contraseña inválida",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtContrasena.Select();
                pnlContrasena.BackColor = Color.LightSalmon;
                txtContrasena.BackColor = Color.LightSalmon;
                txtContrasena.ForeColor = Color.Red;
                return;
            }
            UsuarioDAL usuarioDAL = new UsuarioDAL();

            const int MAX_INTENTOS = 3;
            const int MINUTOS_BLOQUEO = 5;

            // 1. Buscar usuario por nombre
            Usuario usuarioBase = usuarioDAL.ObtenerUsuarioPorNombre(username);

            // 2. Mensaje genérico si el usuario no existe
            if (usuarioBase == null)
            {
                MessageBox.Show(
                    "Usuario o contraseña incorrectos.",
                    "Acceso denegado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );

                txtContrasena.Clear();
                txtUsuario.Select();
                return;
            }

            // 3. Validar si la cuenta está activa
            if (!usuarioBase.Activo)
            {
                MessageBox.Show(
                    "Esta cuenta está inactiva. Contacte al administrador.",
                    "Cuenta inactiva",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtUsuario.Clear();
                txtContrasena.Clear();
                txtUsuario.Select();
                return;
            }

            // 4. Validar si está bloqueado temporalmente
            if (usuarioBase.BloqueadoHasta.HasValue && usuarioBase.BloqueadoHasta.Value > DateTime.Now)
            {
                TimeSpan tiempoRestante = usuarioBase.BloqueadoHasta.Value - DateTime.Now;

                MessageBox.Show(
                    $"Demasiados intentos fallidos. Intente nuevamente en {Math.Ceiling(tiempoRestante.TotalMinutes)} minuto(s).",
                    "Acceso bloqueado temporalmente",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                txtContrasena.Clear();
                txtUsuario.Select();
                return;
            }

            // 5. Validar credenciales reales
            Usuario usuarioEncontrado = usuarioDAL.ValidarCredenciales(username, password);

            if (usuarioEncontrado == null)
            {
                usuarioDAL.IncrementarIntentosFallidos(username);

                Usuario usuarioActualizado = usuarioDAL.ObtenerUsuarioPorNombre(username);

                int intentosActuales = usuarioActualizado.IntentosFallidos;
                int intentosRestantes = MAX_INTENTOS - intentosActuales;

                if (intentosActuales >= MAX_INTENTOS)
                {
                    DateTime bloqueoHasta = DateTime.Now.AddMinutes(MINUTOS_BLOQUEO);
                    usuarioDAL.BloquearUsuarioTemporalmente(username, bloqueoHasta);

                    MessageBox.Show(
                        $"Ha superado el número máximo de intentos. La cuenta estará bloqueada por {MINUTOS_BLOQUEO} minutos.",
                        "Cuenta bloqueada temporalmente",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
                else if (intentosRestantes == 1)
                {
                    MessageBox.Show(
                        "Advertencia: usuario o contraseña incorrectos. Solo le queda 1 intento antes del bloqueo.",
                        "Advertencia de seguridad",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
                else
                {
                    MessageBox.Show(
                        $"Usuario o contraseña incorrectos. Intentos restantes: {intentosRestantes}.",
                        "Acceso denegado",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }

                txtContrasena.Clear();
                txtContrasena.Select();
                return;
            }

            // 6. Si llegó aquí, el login fue correcto
            usuarioDAL.ReiniciarIntentosFallidos(username);

            MessageBox.Show(
                $"Bienvenido {usuarioEncontrado.NombreUsuario}. Rol: {usuarioEncontrado.NombreRol}",
                "Acceso concedido",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );

            sessionFormUser = usuarioEncontrado;
            FrmMenuPrincipal nuevoFormulario = new FrmMenuPrincipal(usuarioEncontrado);
            nuevoFormulario.Show();
            this.Close();
        }

       private void txtUsuario_KeyUp(object sender, EventArgs e)
        {
            txtUsuario.ForeColor = Color.Black;
            txtUsuario.BackColor = Color.White;
        }

        private void txtContrasena_KeyUp(object sender, EventArgs e)
        {
            txtContrasena.ForeColor = Color.Black;
            txtContrasena.BackColor = Color.White;
            pnlContrasena.BackColor = Color.White;
        }

        private void pnlContrasena_Click(object sender, EventArgs e)
        {
            txtContrasena.Select();
        }

        private void txtUsuario_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                
            }
        }
    }
}
