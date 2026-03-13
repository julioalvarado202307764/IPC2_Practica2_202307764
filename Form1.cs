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

        // 2. Método para dibujarlos en la ventana
        private void InicializarControles()
        {
            // Instanciar y darles posición/tamaño
            txtNombre = new TextBox() { Location = new Point(20, 20), Width = 150 };
            numEdad = new NumericUpDown() { Location = new Point(20, 60), Width = 150 };
            cmbEspecialidad = new ComboBox() { Location = new Point(20, 100), Width = 150 };
            cmbEspecialidad.DataSource = Enum.GetValues(typeof(EspecialidadMedica));
            btnRegistrar = new Button() { Text = "Registrar", Location = new Point(20, 140) };
            btnAtender = new Button() { Text = "Atender", Location = new Point(120, 140) };
            lblTiempoEstimado = new Label() { Text = "Tiempo estimado: ", Location = new Point(20, 180), AutoSize = true };
            lblInfoPaciente = new Label() { Text = "Paciente actual: ", Location = new Point(20, 210), AutoSize = true };
            picGrafo = new PictureBox() { Location = new Point(250, 20), Width = 500, Height = 300, SizeMode = PictureBoxSizeMode.Zoom };

            // Conectar los botones con los eventos que ya creamos
            btnRegistrar.Click += new EventHandler(btnRegistrar_Click);
            btnAtender.Click += new EventHandler(btnAtender_Click);

            // Agregar todo al formulario para que sea visible
            this.Controls.Add(txtNombre);
            this.Controls.Add(numEdad);
            this.Controls.Add(cmbEspecialidad);
            this.Controls.Add(btnRegistrar);
            this.Controls.Add(btnAtender);
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
            int tiempoEstimado = colaTurnos.Encolar(nuevoTurno);

            lblTiempoEstimado.Text = $"Tiempo estimado para {nombre}: {tiempoEstimado} minutos.";
            MessageBox.Show($"Turno registrado. Tiempo estimado: {tiempoEstimado} minutos.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // Limpiamos los campos
            txtNombre.Clear();
            numEdad.Value = 0;

            ActualizarGrafo();
        }

        private void btnAtender_Click(object sender, EventArgs e)
        {
            Turno pacienteAtendido = colaTurnos.Desencolar();

            if (pacienteAtendido == null)
            {// Si la cola está vacía, el sistema deberá indicar que no hay turnos pendientes[cite: 37].
                lblInfoPaciente.Text = "No hay pacientes en espera.";
                MessageBox.Show("No hay turnos pendientes para atender.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Al atender, se extrae el turno y se muestra la información[cite: 36].
                lblInfoPaciente.Text = $"Atendiendo a: {pacienteAtendido.Nombre} | Edad: {pacienteAtendido.Edad} | Especialidad: {pacienteAtendido.Especialidad}";
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