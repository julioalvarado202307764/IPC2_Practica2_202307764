public class ColaDinamica
{
    private Nodo frente;
    private Nodo final;
    private int tiempoEsperaAcumulado;

    public ColaDinamica()
    {
        frente = null;
        final = null;
        tiempoEsperaAcumulado = 0;
    }

    public int Encolar(Turno nuevoTurno)
    {
        Nodo nuevoNodo = new Nodo(nuevoTurno);
       // El tiempo estimado para este paciente es el tiempo acumulado de los que ya están en cola MÁS su propio tiempo de atención.
        int tiempoEstimadoPaciente = tiempoEsperaAcumulado + nuevoTurno.TiempoAtencion;

        if (frente == null)
        {
            frente = nuevoNodo;
            final = nuevoNodo;
        }
        else
        {
            final.Siguiente = nuevoNodo;
            final = nuevoNodo;
        }
        
        // Actualizamos el tiempo total de la cola
        tiempoEsperaAcumulado += nuevoTurno.TiempoAtencion; 
        
        return tiempoEstimadoPaciente;
    }

    public Turno Desencolar()
    {
        if (frente == null) return null; // Retorna null si no hay turnos pendientes[cite: 37].

        Turno turnoAtendido = frente.Valor;
        frente = frente.Siguiente;

        if (frente == null)
        {
            final = null;
        }

        // Al atender a un paciente, restamos su tiempo del total acumulado en la cola
        tiempoEsperaAcumulado -= turnoAtendido.TiempoAtencion;

        return turnoAtendido;
    }

    public bool EstaVacia()
    {
        return frente == null;
    }
    
    // Método extra para facilitar la generación de Graphviz más adelante
    public Nodo ObtenerFrente()
    {
        return frente;
    }
}