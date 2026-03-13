using System;
using System.Diagnostics;
using System.IO;

public static class ReporteGraphviz
{
    // Método principal que genera el archivo .dot y luego la imagen
public static void GenerarGraficoCola(ColaDinamica cola)
    {
        string dotContenido = "digraph G {\n";
        dotContenido += "  rankdir=LR;\n"; 
        // Quitamos el fillcolor global para poder personalizar cada nodo individualmente
        dotContenido += "  node [shape=record, style=filled, fontname=\"Segoe UI\"];\n";

        Nodo actual = cola.ObtenerFrente();
        int contador = 0;

        if (actual == null)
        {
            dotContenido += "  Vacia [label=\"Cola Vacía\", shape=box, fillcolor=\"#e0e0e0\"];\n";
        }
        else
        {
            Nodo temp = actual;
            while (temp != null)
            {
                string nombreEspecialidad = temp.Valor.Especialidad.ToString();
                string colorNodo = "white"; // Color por defecto
                
                // Asignamos un color en formato hexadecimal según la especialidad
                switch (temp.Valor.Especialidad)
                {
                    case EspecialidadMedica.MedicinaGeneral: 
                        colorNodo = "\"#add8e6\""; // Azul claro
                        break;
                    case EspecialidadMedica.Pediatria: 
                        colorNodo = "\"#98fb98\""; // Verde pálido
                        break;
                    case EspecialidadMedica.Ginecologia: 
                        colorNodo = "\"#ffb6c1\""; // Rosa claro
                        break;
                    case EspecialidadMedica.Dermatologia: 
                        colorNodo = "\"#fffacd\""; // Amarillo suave
                        break;
                }

                // Aplicamos el colorNodo específico a este paciente
                dotContenido += $"  nodo{contador} [label=\"{{ Turno: {contador + 1} | Paciente: {temp.Valor.Nombre} | {nombreEspecialidad} }}\", fillcolor={colorNodo}];\n";
                contador++;
                temp = temp.Siguiente;
            }

            // Hacemos que las flechas se vean un poco más gruesas y de color gris oscuro
            for (int i = 0; i < contador - 1; i++)
            {
                dotContenido += $"  nodo{i} -> nodo{i + 1} [color=\"#555555\", penwidth=2.0];\n";
            }
        }

        dotContenido += "}\n";

        string dotPath = "cola_turnos.dot";
        string imagePath = "cola_turnos.png";

        File.WriteAllText(dotPath, dotContenido);
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