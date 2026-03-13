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

        // 2. Método para dibujarlos en la ventana
        private void InicializarControles()
        {
            // Configuración básica del formulario
            this.Text = "Sistema de Gestión de Turnos Médicos";
            this.Size = new Size(850, 420);
            this.BackColor = Color.FromArgb(240, 248, 255); // Un tono azul claro "médico" (AliceBlue)
            this.StartPosition = FormStartPosition.CenterScreen;

            // Fuentes modernas
            Font fuenteEtiquetas = new Font("Segoe UI", 10, FontStyle.Bold);
            Font fuenteTextos = new Font("Segoe UI", 10, FontStyle.Regular);

            // Etiquetas descriptivas (¡Para que el usuario sepa qué llenar!)
            lblTituloNombre = new Label() { Text = "Nombre del Paciente:", Location = new Point(20, 20), AutoSize = true, Font = fuenteEtiquetas, ForeColor = Color.DarkSlateGray };
            lblTituloEdad = new Label() { Text = "Edad:", Location = new Point(20, 80), AutoSize = true, Font = fuenteEtiquetas, ForeColor = Color.DarkSlateGray };
            lblTituloEspecialidad = new Label() { Text = "Especialidad Requerida:", Location = new Point(20, 140), AutoSize = true, Font = fuenteEtiquetas, ForeColor = Color.DarkSlateGray };

            // Campos de entrada
            txtNombre = new TextBox() { Location = new Point(20, 45), Width = 230, Font = fuenteTextos };
            numEdad = new NumericUpDown() { Location = new Point(20, 105), Width = 230, Font = fuenteTextos, Minimum = 0, Maximum = 120 };
            cmbEspecialidad = new ComboBox() { Location = new Point(20, 165), Width = 230, Font = fuenteTextos, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbEspecialidad.DataSource = Enum.GetValues(typeof(EspecialidadMedica));

            // Botones con estilo "Flat" moderno
            btnRegistrar = new Button()
            {
                Text = "Registrar Turno",
                Location = new Point(20, 210),
                Size = new Size(130, 40),
                Font = fuenteEtiquetas,
                BackColor = Color.MediumSeaGreen, // Verde para agregar
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnRegistrar.FlatAppearance.BorderSize = 0;

            btnAtender = new Button()
            {
                Text = "Atender Siguiente",
                Location = new Point(160, 210),
                Size = new Size(150, 40),
                Font = fuenteEtiquetas,
                BackColor = Color.SteelBlue, // Azul para la acción principal
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAtender.FlatAppearance.BorderSize = 0;

            // Etiquetas de información destacadas
            lblTiempoEstimado = new Label() { Text = "Tiempo estimado: --", Location = new Point(20, 270), AutoSize = true, Font = fuenteEtiquetas, ForeColor = Color.DarkRed };
            lblInfoPaciente = new Label() { Text = "Paciente actual: Ninguno", Location = new Point(20, 300), AutoSize = true, Font = new Font("Segoe UI", 11, FontStyle.Bold), ForeColor = Color.DarkBlue };

            // PictureBox para Graphviz con fondo blanco para que resalte
            picGrafo = new PictureBox()
            {
                Location = new Point(340, 45),
                Width = 470,
                Height = 300,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Conectar eventos
            btnRegistrar.Click += new EventHandler(btnRegistrar_Click);
            btnAtender.Click += new EventHandler(btnAtender_Click);

            // Agregar todo al formulario
            this.Controls.Add(lblTituloNombre);
            this.Controls.Add(lblTituloEdad);
            this.Controls.Add(lblTituloEspecialidad);
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
            {
                // Si la cola ya estaba vacía
                lblInfoPaciente.Text = "No hay pacientes en espera.";
                lblTiempoEstimado.Text = "Tiempo estimado: --"; // <- Reiniciamos el tiempo
                MessageBox.Show("No hay turnos pendientes para atender.", "Información", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                // Al atender, mostramos la info del paciente
                lblInfoPaciente.Text = $"Atendiendo a: {pacienteAtendido.Nombre} | Edad: {pacienteAtendido.Edad} | Especialidad: {pacienteAtendido.Especialidad}";

                // Validamos si este era el ÚLTIMO paciente de la fila
                if (colaTurnos.EstaVacia())
                {
                    lblTiempoEstimado.Text = "Tiempo estimado: --"; // <- Reiniciamos el tiempo si ya no queda nadie atrás
                }
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