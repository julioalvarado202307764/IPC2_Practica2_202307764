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
    
    // Propiedad calculada para obtener los minutos fácilmente
    public int TiempoAtencion => (int)Especialidad; 

    public Turno(string nombre, int edad, EspecialidadMedica especialidad)
    {
        Nombre = nombre;
        Edad = edad;
        Especialidad = especialidad;
    }
}