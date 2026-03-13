public enum EspecialidadMedica
{
    MedicinaGeneral = 10,
    Pediatria = 15,
    Ginecologia = 20,
    Dermatologia = 25
}

public class Turno
{
    public string Nombre { get; set; }
    public int Edad { get; set; }
    public EspecialidadMedica Especialidad { get; set; }
    
    // Tiempos requeridos por la rúbrica (5 puntos cada uno)
    public int TiempoAtencion => (int)Especialidad; 
    public int TiempoEnCola { get; set; }
    public int TiempoTotal => TiempoEnCola + TiempoAtencion;

    public Turno(string nombre, int edad, EspecialidadMedica especialidad)
    {
        Nombre = nombre;
        Edad = edad;
        Especialidad = especialidad;
        TiempoEnCola = 0; // Se calcula automáticamente al entrar a la cola
    }
}