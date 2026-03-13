using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Practica2_202307764
{
    public partial class Form1 : Form
    {
        private ColaDinamica colaTurnos;

        public Form1()
        {
            InitializeComponent();
            InicializarControles();
            colaTurnos = new ColaDinamica();
        }
        // 1. Declaración de las variables para que desaparezca el rojo
        private TextBox txtNombre;
        private NumericUpDown numEdad;
        private ComboBox cmbEspecialidad;
        private Button btnRegistrar;
        private Button btnAtender;
        private Label lblInfoPaciente;
        private Label lblTiempoEstimado;
        private PictureBox picGrafo;
        private Label lblTituloNombre;
        private Label lblTituloEdad;
        private Label lblTituloEspecialidad;
        private MenuStrip menuPrincipal;
        private ToolStripMenuItem menuNuevoTurno;
        private ToolStripMenuItem menuAtenderPaciente;
        private ToolStripMenuItem menuVerCola;
        private ToolStripMenuItem menuSalir;

        // 2. Método para dibujarlos en la ventana
private void InicializarControles()
{
    // Configuración básica del formulario
    this.Text = "Sistema de Gestión de Turnos Médicos";
    this.Size = new Size(850, 380); // Ajustamos un poco el alto ya que quitamos los botones
    this.BackColor = Color.FromArgb(240, 248, 255);
    this.StartPosition = FormStartPosition.CenterScreen;

    Font fuenteEtiquetas = new Font("Segoe UI", 10, FontStyle.Bold);
    Font fuenteTextos = new Font("Segoe UI", 10, FontStyle.Regular);

    // Menú Principal
    menuPrincipal = new MenuStrip();
    menuPrincipal.BackColor = Color.LightSteelBlue;
    menuPrincipal.Font = new Font("Segoe UI", 10, FontStyle.Regular);

    menuNuevoTurno = new ToolStripMenuItem("Nuevo Turno");
    menuAtenderPaciente = new ToolStripMenuItem("Atender Paciente");
    menuVerCola = new ToolStripMenuItem("Ver Cola de Turnos");
    menuSalir = new ToolStripMenuItem("Salir");

    menuPrincipal.Items.Add(menuNuevoTurno);
    menuPrincipal.Items.Add(menuAtenderPaciente);
    menuPrincipal.Items.Add(menuVerCola);
    menuPrincipal.Items.Add(menuSalir);

    // Conectamos las opciones del menú con los métodos que ya tienes
    menuNuevoTurno.Click += new EventHandler(btnRegistrar_Click); 
    menuAtenderPaciente.Click += new EventHandler(btnAtender_Click); 
    menuVerCola.Click += new EventHandler(menuVerCola_Click); 
    menuSalir.Click += (s, e) => Application.Exit(); 

    // Etiquetas y Controles (Desplazados hacia abajo para no chocar con el menú)
    lblTituloNombre = new Label() { Text = "Nombre del Paciente:", Location = new Point(20, 45), AutoSize = true, Font = fuenteEtiquetas, ForeColor = Color.DarkSlateGray };
    txtNombre = new TextBox() { Location = new Point(20, 70), Width = 230, Font = fuenteTextos };
    
    lblTituloEdad = new Label() { Text = "Edad:", Location = new Point(20, 110), AutoSize = true, Font = fuenteEtiquetas, ForeColor = Color.DarkSlateGray };
    numEdad = new NumericUpDown() { Location = new Point(20, 135), Width = 230, Font = fuenteTextos, Minimum = 0, Maximum = 120 };
    
    lblTituloEspecialidad = new Label() { Text = "Especialidad Requerida:", Location = new Point(20, 175), AutoSize = true, Font = fuenteEtiquetas, ForeColor = Color.DarkSlateGray };
    cmbEspecialidad = new ComboBox() { Location = new Point(20, 200), Width = 230, Font = fuenteTextos, DropDownStyle = ComboBoxStyle.DropDownList };
    cmbEspecialidad.DataSource = Enum.GetValues(typeof(EspecialidadMedica));

    // Etiquetas de información destacadas
    lblTiempoEstimado = new Label() { Text = "Tiempo estimado: --", Location = new Point(20, 245), AutoSize = true, Font = fuenteEtiquetas, ForeColor = Color.DarkRed };
    lblInfoPaciente = new Label() { Text = "Paciente actual: Ninguno", Location = new Point(20, 275), AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.DarkBlue };

    // PictureBox para Graphviz
    picGrafo = new PictureBox() { 
        Location = new Point(340, 45), 
        Width = 470, 
        Height = 270, 
        SizeMode = PictureBoxSizeMode.Zoom,
        BackColor = Color.White,
        BorderStyle = BorderStyle.FixedSingle
    };

    // Agregar todo al formulario
    this.Controls.Add(menuPrincipal);
    this.MainMenuStrip = menuPrincipal;
    
    this.Controls.Add(lblTituloNombre);
    this.Controls.Add(txtNombre);
    this.Controls.Add(lblTituloEdad);
    this.Controls.Add(numEdad);
    this.Controls.Add(lblTituloEspecialidad);
    this.Controls.Add(cmbEspecialidad);
    
    this.Controls.Add(lblTiempoEstimado);
    this.Controls.Add(lblInfoPaciente);
    this.Controls.Add(picGrafo);
}

        private void Form1_Load(object sender, EventArgs e)
        {
            // Cargamos las especialidades en el ComboBox
            cmbEspecialidad.DataSource = Enum.GetValues(typeof(EspecialidadMedica));
            ActualizarGrafo(); // Mostramos la cola vacía al iniciar
        }
        private void menuVerCola_Click(object sender, EventArgs e)
        {
            if (colaTurnos.EstaVacia())
            {
                MessageBox.Show("Actualmente no hay pacientes en la cola.", "Ver Cola de Turnos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string reporteCola = "--- ESTADO ACTUAL DE LA COLA ---\n\n";
            Nodo actual = colaTurnos.ObtenerFrente();
            int posicion = 1;

            while (actual != null)
            {
                Turno t = actual.Valor;
                reporteCola += $"Turno {posicion}: {t.Nombre} ({t.Edad} años) - {t.Especialidad}\n" +
                               $"   ⏱ Atención: {t.TiempoAtencion} min | ⏳ Cola: {t.TiempoEnCola} min | ⌛ Total: {t.TiempoTotal} min\n\n";

                actual = actual.Siguiente;
                posicion++;
            }

            MessageBox.Show(reporteCola, "Ver Cola de Turnos", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Por favor, ingresa el nombre del paciente.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string nombre = txtNombre.Text;
            int edad = (int)numEdad.Value;
            EspecialidadMedica especialidad = (EspecialidadMedica)cmbEspecialidad.SelectedItem;

            Turno nuevoTurno = new Turno(nombre, edad, especialidad);

            // Encolamos y obtenemos el tiempo estimado (espera + atención)
            // Encolamos el turno
            colaTurnos.Encolar(nuevoTurno);

            // Leemos el tiempo total directamente del objeto que acabamos de crear
            lblTiempoEstimado.Text = $"Tiempo estimado para {nombre}: {nuevoTurno.TiempoTotal} minutos.";
            MessageBox.Show($"Turno registrado.\nTiempo en cola: {nuevoTurno.TiempoEnCola} min\nTiempo de atención: {nuevoTurno.TiempoAtencion} min\nTiempo total: {nuevoTurno.TiempoTotal} min", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            // Limpiamos los campos
            txtNombre.Clear();
            numEdad.Value = 0;

            ActualizarGrafo();
        }

        private void btnAtender_Click(object sender, EventArgs e)
        {
            Turno pacienteAtendido = colaTurnos.Desencolar();

            if (pacienteAtendido == null)
            {
                lblInfoPaciente.Text = "No hay pacientes en espera.";
                lblTiempoEstimado.Text = "Tiempo estimado: --";
                MessageBox.Show("No hay turnos pendientes para atender.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Actualizamos la interfaz
                lblInfoPaciente.Text = $"Atendiendo a: {pacienteAtendido.Nombre} | {pacienteAtendido.Especialidad}";

                if (colaTurnos.EstaVacia())
                {
                    lblTiempoEstimado.Text = "Tiempo estimado: --";
                }

                // Mostramos el MessageBox con el desglose exacto que pide la rúbrica
                string mensajeAtencion = $"--- PACIENTE ATENDIDO ---\n\n" +
                                         $"Nombre: {pacienteAtendido.Nombre}\n" +
                                         $"Edad: {pacienteAtendido.Edad}\n" +
                                         $"Especialidad: {pacienteAtendido.Especialidad}\n\n" +
                                         $"⏱ Tiempo de atención: {pacienteAtendido.TiempoAtencion} min\n" +
                                         $"⏳ Tiempo en cola: {pacienteAtendido.TiempoEnCola} min\n" +
                                         $"⌛ Tiempo total: {pacienteAtendido.TiempoTotal} min";

                MessageBox.Show(mensajeAtencion, "Turno Atendido", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            ActualizarGrafo();
        }
        private void ActualizarGrafo()
        {
            // Generamos la nueva imagen con Graphviz
            ReporteGraphviz.GenerarGraficoCola(colaTurnos);

            string imagePath = "cola_turnos.png";

            if (File.Exists(imagePath))
            {
                // Usamos un FileStream para no bloquear el archivo de imagen. 
                // Si usamos picGrafo.Load(), Windows bloquea el archivo y Graphviz no podría sobreescribirlo en el siguiente turno.
                using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    picGrafo.Image = Image.FromStream(fs);
                }
            }
        }
    }
}