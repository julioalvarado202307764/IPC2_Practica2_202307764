using System;
using System.Diagnostics;
using System.IO;

public static class ReporteGraphviz
{
    // Método principal que genera el archivo .dot y luego la imagen
    public static void GenerarGraficoCola(ColaDinamica cola)
    {
        // Configuramos el documento Graphviz
        // rankdir=LR hace que la cola se dibuje de izquierda a derecha
        string dotContenido = "digraph G {\n";
        dotContenido += "  rankdir=LR;\n"; 
        dotContenido += "  node [shape=record, style=filled, fillcolor=lightblue, fontname=\"Arial\"];\n";

        Nodo actual = cola.ObtenerFrente();
        int contador = 0;

        if (actual == null)
        {
            // Si la cola está vacía, mostramos un nodo indicándolo
            dotContenido += "  Vacia [label=\"Cola Vacía\", shape=box, fillcolor=lightcoral];\n";
        }
        else
        {
            // Primera pasada: crear todos los nodos de la cola
            Nodo temp = actual;
            while (temp != null)
            {
                string nombreEspecialidad = temp.Valor.Especialidad.ToString();
                // Usamos formato de registro (record) para mostrar los datos ordenados
                dotContenido += $"  nodo{contador} [label=\"{{ Turno: {contador + 1} | Paciente: {temp.Valor.Nombre} | {nombreEspecialidad} }}\"];\n";
                contador++;
                temp = temp.Siguiente;
            }

            // Segunda pasada: crear las flechas que conectan los nodos (FIFO)
            for (int i = 0; i < contador - 1; i++)
            {
                dotContenido += $"  nodo{i} -> nodo{i + 1};\n";
            }
        }

        dotContenido += "}\n";

        // Rutas de los archivos (se guardarán en la carpeta bin/Debug de tu proyecto)
        string dotPath = "cola_turnos.dot";
        string imagePath = "cola_turnos.png";

        // 1. Guardar el texto en el archivo .dot
        File.WriteAllText(dotPath, dotContenido);

        // 2. Ejecutar Graphviz para compilar la imagen
        CompilarImagen(dotPath, imagePath);
    }

    private static void CompilarImagen(string dotPath, string imagePath)
    {
        try
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "dot", 
                Arguments = $"-Tpng {dotPath} -o {imagePath}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(startInfo))
            {
                process.WaitForExit();
            }
        }
        catch (Exception ex)
        {
            // Útil para debuggear si Graphviz no está en el PATH
            Console.WriteLine("Error al generar la imagen con Graphviz: " + ex.Message);
        }
    }
}