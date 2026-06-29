using SistemaGestionRoles.Entidades;
using SistemaGestionRoles.Datos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using SistemaGestionRoles.Presentacion;

namespace SistemaGestionRoles
{
    public partial class FrmMenuPrincipal : Form
    {
        private string txtClientsPlaceholderText = "Filtrar por nombre o ID...";

        Dictionary<int, string> roleMessage = new Dictionary<int, string>()
        {
            { 1, "Administrador del sistema" },
            { 2, "Supervisor" },
            { 3, "Ejecutor" },
            { 4, "Invitado" }

        };
        public enum RoleMessage
        {
            Administrador,
            Supervisor,
            Ejecutor,
            Invitado
        }
        public Usuario sessionUser;
        private ClienteDAL clienteDAL = new ClienteDAL();
        private int idClienteSeleccionado = 0;
        private DataTable clientesActuales = new DataTable();

        private int paginaActual = 1;
        private int registrosPorPagina = 1;
        private int totalPaginas = 1;

        private Panel pnlPaginacion;
        private Button btnPaginaAnterior;
        private Button btnPaginaSiguiente;
        private Label lblInfoPaginacion;

        public FrmMenuPrincipal(Usuario user)
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
        private static FrmMenuPrincipal _instancia = null;
        public static FrmMenuPrincipal ObtenerInstancia(Usuario sessionUser)
        {
            // Si no existe o fue cerrado (destruido), creamos uno nuevo
            if (_instancia == null || _instancia.IsDisposed)
            {
                _instancia = new FrmMenuPrincipal(sessionUser);
            }

            return _instancia;
        }
        private void FrmMenuPrincipal_Load(object sender, EventArgs e)
        {
            // Maximizar la ventana desde que cargue el formulario   
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.WindowState = FormWindowState.Maximized;

            // Los label tanto del nombre de usuario como del rol se les asigna su texto correspondiente
            lblUsername.Text = sessionUser.NombreUsuario;
            lblRole.Text = roleMessage[sessionUser.IdRol];

            // Se cambia el avatar del usuario dependiendo del id de su rol (1: admin, 2: supervisor, 3: ejecutor); Ademas, se cambian los aspectos visuales de los botones del CRUD segun los roles de los usuarios
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
                    // El color de fondo del panel se cambia, con el objetivo de generar un aspecto de deshabilitado al boton de ELIMINAR
                    pnlDelete.BackColor = Color.Silver;

                    // El color de fondo del panel se cambia, con el objetivo de generar un aspecto de deshabilitado al boton de ANADIR
                    pnlAdd.BackColor = Color.Silver;
                    break;
                    
                case 3:
                    // Se cambia la imagen segun el rol, se accede en la carpeta resources
                    userAvatar.Image = Properties.Resources.ejecutor;
                    // El color de fondo del panel se cambia, con el objetivo de generar un aspecto de deshabilitado al boton de ELIMINAR
                    pnlDelete.BackColor = Color.Silver;

                    // El color de fondo del panel se cambia, con el objetivo de generar un aspecto de deshabilitado al boton de EDITAR
                    pnlEdit.BackColor = Color.Silver;
                    break;
                default:
                    break;
            }
            // placeholder del buscador de clientes
            txtClients.Text = txtClientsPlaceholderText;
            txtClients.ForeColor = Color.Gray;

            //
            ConfigurarDataGridView();
            CrearControlesPaginacion();
            CargarClientes();

            comboBox1.SelectedIndex = 0;

            this.Resize += FrmMenuPrincipal_Resize;
            dataGridView1.Resize += dataGridView1_Resize;

            dataGridView1.CellClick += dataGridView1_CellClick;

            pnlClear.Click += Limpiar_Click;
            pbClear.Click += Limpiar_Click;
            lblClear.Click += Limpiar_Click;

            panel22.Click += Consultar_Click;
            label13.Click += Consultar_Click;
            pictureBox7.Click += Consultar_Click;

            txtClients.KeyDown += txtClients_KeyDown;
        }
        private void CargarClientes()
        {
            clientesActuales = clienteDAL.ObtenerClientes();
            paginaActual = 1;
            ActualizarPaginacion();
        }
        private void FormatearColumnasDataGridView()
        {
            if (dataGridView1.Columns.Contains("IdCliente"))
            {
                dataGridView1.Columns["IdCliente"].HeaderText = "ID";
            }

            if (dataGridView1.Columns.Contains("Nombre"))
            {
                dataGridView1.Columns["Nombre"].HeaderText = "Nombre";
            }

            if (dataGridView1.Columns.Contains("Telefono"))
            {
                dataGridView1.Columns["Telefono"].HeaderText = "Teléfono";
            }

            if (dataGridView1.Columns.Contains("Correo"))
            {
                dataGridView1.Columns["Correo"].HeaderText = "Correo";
            }

            if (dataGridView1.Columns.Contains("Direccion"))
            {
                dataGridView1.Columns["Direccion"].HeaderText = "Dirección";
            }

            if (dataGridView1.Columns.Contains("Estado"))
            {
                dataGridView1.Columns["Estado"].HeaderText = "Activo";
            }
        }
        private int CalcularRegistrosPorPagina()
        {
            int altoDisponible = dataGridView1.ClientSize.Height - dataGridView1.ColumnHeadersHeight;

            int altoFila = dataGridView1.RowTemplate.Height;

            if (altoFila <= 0)
            {
                altoFila = 22;
            }

            int cantidad = altoDisponible / altoFila;

            if (cantidad < 1)
            {
                cantidad = 1;
            }

            return cantidad;
        }
        private void ActualizarPaginacion()
        {
            if (clientesActuales == null)
            {
                return;
            }

            registrosPorPagina = CalcularRegistrosPorPagina();

            int totalRegistros = clientesActuales.Rows.Count;

            totalPaginas = (int)Math.Ceiling((double)totalRegistros / registrosPorPagina);

            if (totalPaginas < 1)
            {
                totalPaginas = 1;
            }

            if (paginaActual > totalPaginas)
            {
                paginaActual = totalPaginas;
            }

            if (paginaActual < 1)
            {
                paginaActual = 1;
            }

            DataTable tablaPagina = clientesActuales.Clone();

            int inicio = (paginaActual - 1) * registrosPorPagina;
            int fin = Math.Min(inicio + registrosPorPagina, totalRegistros);

            for (int i = inicio; i < fin; i++)
            {
                tablaPagina.ImportRow(clientesActuales.Rows[i]);
            }

            dataGridView1.DataSource = tablaPagina;

            FormatearColumnasDataGridView();

            lblInfoPaginacion.Text = $"Página {paginaActual} de {totalPaginas} | Registros: {totalRegistros}";

            btnPaginaAnterior.Enabled = paginaActual > 1;
            btnPaginaSiguiente.Enabled = paginaActual < totalPaginas;

            dataGridView1.ClearSelection();
            idClienteSeleccionado = 0;
        }
        
        private void CrearControlesPaginacion()
        {
            pnlPaginacion = new Panel();
            pnlPaginacion.Dock = DockStyle.Bottom;
            pnlPaginacion.Height = 42;
            pnlPaginacion.BackColor = Color.WhiteSmoke;
            pnlPaginacion.Padding = new Padding(8);

            btnPaginaAnterior = new Button();
            btnPaginaAnterior.Text = "Anterior";
            btnPaginaAnterior.Width = 90;
            btnPaginaAnterior.Height = 26;
            btnPaginaAnterior.Left = 8;
            btnPaginaAnterior.Top = 8;
            btnPaginaAnterior.Click += btnPaginaAnterior_Click;

            lblInfoPaginacion = new Label();
            lblInfoPaginacion.AutoSize = true;
            lblInfoPaginacion.Left = 110;
            lblInfoPaginacion.Top = 13;
            lblInfoPaginacion.Text = "Página 1 de 1";

            btnPaginaSiguiente = new Button();
            btnPaginaSiguiente.Text = "Siguiente";
            btnPaginaSiguiente.Width = 90;
            btnPaginaSiguiente.Height = 26;
            btnPaginaSiguiente.Left = 250;
            btnPaginaSiguiente.Top = 8;
            btnPaginaSiguiente.Click += btnPaginaSiguiente_Click;

            pnlPaginacion.Controls.Add(btnPaginaAnterior);
            pnlPaginacion.Controls.Add(lblInfoPaginacion);
            pnlPaginacion.Controls.Add(btnPaginaSiguiente);

            panel11.Controls.Add(pnlPaginacion);

            dataGridView1.Dock = DockStyle.Fill;

            panel11.PerformLayout();
            dataGridView1.PerformLayout();
        }
        private void txtClients_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Consultar_Click(sender, e);
            }
        }
        private void Consultar_Click(object sender, EventArgs e)
        {
            string filtro = txtClients.Text.Trim();

            if (filtro == txtClientsPlaceholderText || string.IsNullOrWhiteSpace(filtro))
            {
                CargarClientes();
                return;
            }

            clientesActuales = clienteDAL.BuscarClientes(filtro);
            paginaActual = 1;
            ActualizarPaginacion();

            if (clientesActuales.Rows.Count == 0)
            {
                MessageBox.Show(
                    "No se encontraron clientes con ese criterio de búsqueda.",
                    "Sin resultados",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );
            }
        }
        /*private void Consultar_Click(object sender, EventArgs e)
        {
            string filtro = txtClients.Text.Trim();

            if (filtro == txtClientsPlaceholderText || string.IsNullOrWhiteSpace(filtro))
            {
                CargarClientes();
                return;
            }

            dataGridView1.DataSource = clienteDAL.BuscarClientes(filtro);
        }*/
        private void DeleteRecord()
        {
            if (sessionUser.IdRol == 2)
            {
                MessageBox.Show(
                    "Como Supervisor no tienes permitido eliminar registros.",
                    "Permisos insuficientes",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (sessionUser.IdRol == 3)
            {
                MessageBox.Show(
                    "Como Ejecutor no tienes permitido eliminar registros.",
                    "Permisos insuficientes",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (idClienteSeleccionado == 0)
            {
                MessageBox.Show(
                    "Debe seleccionar un cliente para eliminar.",
                    "Cliente no seleccionado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            DialogResult result = MessageBox.Show(
                "¿Está seguro de que desea eliminar este cliente?",
                "Confirmar eliminación",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes)
            {
                return;
            }

            bool eliminado = clienteDAL.EliminarCliente(idClienteSeleccionado);

            if (eliminado)
            {
                MessageBox.Show(
                    "Cliente eliminado correctamente.",
                    "Eliminación exitosa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                CargarClientes();
                LimpiarFormulario();
            }
            else
            {
                MessageBox.Show(
                    "No se pudo eliminar el cliente.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        private void EditRecord()
        {
            if (sessionUser.IdRol == 3)
            {
                MessageBox.Show(
                    "Como Ejecutor no tienes permitido modificar registros.",
                    "Permisos insuficientes",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (idClienteSeleccionado == 0)
            {
                MessageBox.Show(
                    "Debe seleccionar un cliente para modificar.",
                    "Cliente no seleccionado",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (!ValidarFormularioCliente())
            {
                return;
            }

            Cliente cliente = ObtenerClienteDesdeFormulario();

            bool resultado = clienteDAL.ActualizarCliente(cliente);

            if (resultado)
            {
                MessageBox.Show(
                    "Cliente modificado correctamente.",
                    "Modificación exitosa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                CargarClientes();
                LimpiarFormulario();
            }
            else
            {
                MessageBox.Show(
                    "No se pudo modificar el cliente.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        private void AddRecord()
        {
            if (sessionUser.IdRol != 1 && sessionUser.IdRol != 3)
            {
                MessageBox.Show(
                    "Tu rol no tiene permitido añadir registros.",
                    "Permisos insuficientes",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            if (!ValidarFormularioCliente())
            {
                return;
            }

            Cliente cliente = ObtenerClienteDesdeFormulario();

            bool resultado = clienteDAL.InsertarCliente(cliente);

            if (resultado)
            {
                MessageBox.Show(
                    "Cliente agregado correctamente.",
                    "Registro exitoso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                CargarClientes();
                LimpiarFormulario();
            }
            else
            {
                MessageBox.Show(
                    "No se pudo agregar el cliente.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }
        
        private bool ValidarFormularioCliente()
        {
            string nombre = textBox1.Text.Trim();
            string telefono = textBox2.Text.Trim();
            string correo = textBox3.Text.Trim();
            string direccion = textBox4.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show(
                    "Debe ingresar el nombre del cliente.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                textBox1.Select();
                return false;
            }

            if (nombre.Length < 3 || nombre.Length > 100)
            {
                MessageBox.Show(
                    "El nombre debe tener entre 3 y 100 caracteres.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                textBox1.Select();
                return false;
            }

            if (string.IsNullOrWhiteSpace(telefono))
            {
                MessageBox.Show(
                    "Debe ingresar el teléfono del cliente.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                textBox2.Select();
                return false;
            }

            if (!EsTelefonoValido(telefono))
            {
                MessageBox.Show(
                    "El teléfono no tiene un formato válido.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                textBox2.Select();
                return false;
            }

            if (string.IsNullOrWhiteSpace(correo))
            {
                MessageBox.Show(
                    "Debe ingresar el correo del cliente.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                textBox3.Select();
                return false;
            }

            if (!EsCorreoValido(correo))
            {
                MessageBox.Show(
                    "El correo electrónico no tiene un formato válido.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                textBox3.Select();
                return false;
            }

            if (string.IsNullOrWhiteSpace(direccion))
            {
                MessageBox.Show(
                    "Debe ingresar la dirección del cliente.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                textBox4.Select();
                return false;
            }

            if (direccion.Length < 5 || direccion.Length > 200)
            {
                MessageBox.Show(
                    "La dirección debe tener entre 5 y 200 caracteres.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                textBox4.Select();
                return false;
            }

            if (comboBox1.SelectedIndex == -1)
            {
                MessageBox.Show(
                    "Debe seleccionar el estado del cliente.",
                    "Validación",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                comboBox1.Select();
                return false;
            }

            return true;
        }
        private Cliente ObtenerClienteDesdeFormulario()
        {
            Cliente cliente = new Cliente();

            cliente.IdCliente = idClienteSeleccionado;
            cliente.Nombre = textBox1.Text.Trim();
            cliente.Telefono = textBox2.Text.Trim();
            cliente.Correo = textBox3.Text.Trim();
            cliente.Direccion = textBox4.Text.Trim();
            cliente.Estado = comboBox1.SelectedItem.ToString() == "Activo";

            return cliente;
        }
        private void LimpiarFormulario()
        {
            idClienteSeleccionado = 0;

            textBox1.Clear();
            textBox2.Clear();
            textBox3.Clear();
            textBox4.Clear();

            comboBox1.SelectedIndex = 0;

            dataGridView1.ClearSelection();

            textBox1.Select();
        }
        private void Limpiar_Click(object sender, EventArgs e)
        {
            LimpiarFormulario();
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            DataGridViewRow fila = dataGridView1.Rows[e.RowIndex];

            idClienteSeleccionado = Convert.ToInt32(fila.Cells["IdCliente"].Value);

            textBox1.Text = fila.Cells["Nombre"].Value.ToString();
            textBox2.Text = fila.Cells["Telefono"].Value.ToString();
            textBox3.Text = fila.Cells["Correo"].Value.ToString();
            textBox4.Text = fila.Cells["Direccion"].Value.ToString();

            bool estado = Convert.ToBoolean(fila.Cells["Estado"].Value);
            comboBox1.SelectedItem = estado ? "Activo" : "Inactivo";
        }
        
        private void ConfigurarDataGridView()
        {
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dataGridView1.RowHeadersVisible = false;
            dataGridView1.RowTemplate.Height = 28;
            dataGridView1.AllowUserToResizeRows = false;
        }
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

        private void label6_Click(object sender, EventArgs e)
        {
            ExitClick();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ExitClick();
        }

        private void searchClients_Click(object sender, EventArgs e)
        {
            txtClients.Select();
        }

        private void pbSearch_Click(object sender, EventArgs e)
        {
            txtClients.Select();
        }
        private void txtClients_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtClients.Text))
            {
                txtClients.Text = txtClientsPlaceholderText;
                txtClients.ForeColor = Color.Gray;
            }
            this.ActiveControl = null;
        }
        private void txtClients_Enter(object sender, EventArgs e)
        {
            if (txtClients.Text == txtClientsPlaceholderText)
            {
                txtClients.Text = "";
                txtClients.ForeColor = Color.Black;
                txtClients.BackColor = Color.White;
            }
            //txtUsuario.Select();
        }

        private void pnlAdd_Click(object sender, EventArgs e)
        {
            AddRecord();
        }

        private void lblAdd_Click(object sender, EventArgs e)
        {
            AddRecord();

        }

        private void pbAdd_Click(object sender, EventArgs e)
        {
            AddRecord();

        }

        private void lblDelete_Click(object sender, EventArgs e)
        {
            DeleteRecord();
        }

        private void pbDelete_Click(object sender, EventArgs e)
        {
            DeleteRecord();
        }
        private void pnlDelete_Click(object sender, EventArgs e)
        {
            DeleteRecord();

        }
        
        private void lblEdit_Click(object sender, EventArgs e)
        {
            EditRecord();
        }

        private void pbEdit_Click(object sender, EventArgs e)
        {
            EditRecord();
        }

        private void pnlEdit_Click(object sender, EventArgs e)
        {
            EditRecord();
        }
        private void btnPaginaAnterior_Click(object sender, EventArgs e)
        {
            if (paginaActual > 1)
            {
                paginaActual--;
                ActualizarPaginacion();
                LimpiarFormulario();
            }
        }
        private void btnPaginaSiguiente_Click(object sender, EventArgs e)
        {
            if (paginaActual < totalPaginas)
            {
                paginaActual++;
                ActualizarPaginacion();
                LimpiarFormulario();
            }
        }
        private void ActualizarPaginacionPorResize()
        {
            if (clientesActuales == null || clientesActuales.Rows.Count == 0)
            {
                return;
            }

            int nuevoRegistrosPorPagina = CalcularRegistrosPorPagina();

            if (nuevoRegistrosPorPagina != registrosPorPagina)
            {
                ActualizarPaginacion();
            }
        }
        private void FrmMenuPrincipal_Resize(object sender, EventArgs e)
        {
            ActualizarPaginacionPorResize();
        }
        private void dataGridView1_Resize(object sender, EventArgs e)
        {
            ActualizarPaginacionPorResize();
        }
        private bool EsCorreoValido(string correo)
        {
            string patron = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(correo.Trim(), patron);
        }
        private bool EsTelefonoValido(string telefono)
        {
            string patron = @"^[0-9\-\+\s\(\)]{7,20}$";
            return Regex.IsMatch(telefono.Trim(), patron);
        }

        
        private void pnlUsuarios_Click(object sender, EventArgs e)
        {
            // Validamos los roles para permitir solamente al administrador ingresar a este modulo
            if (sessionUser.IdRol == 2)
            {
                MessageBox.Show(
                    "Como Supervisor no tienes permisos suficientes para acceder al modulo de Usuarios",
                    "Permisos insuficientes",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
            if (sessionUser.IdRol == 3)
            {
                MessageBox.Show(
                    "Como Ejecutor no tienes permisos suficientes para acceder al modulo de Usuarios",
                    "Permisos insuficientes",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }
            // Creamos una instancia de la interfaz de usuarios y ocultamos la actual
            var menuUsuarios = FrmUsuarios.ObtenerInstancia(sessionUser);
            Program.ContextoApp.MainForm = menuUsuarios;
            menuUsuarios.Show();
            menuUsuarios.BringToFront();
            this.Hide();


        }

        private void pnlProductos_Click(object sender, EventArgs e)
        {
            // Creamos una instancia de la interfaz de productos y ocultamos la actual
            var menuProductos = FrmProductos.ObtenerInstancia(sessionUser);
            Program.ContextoApp.MainForm = menuProductos;
            menuProductos.Show();
            menuProductos.BringToFront();
            this.Hide();
        }
    }
}
