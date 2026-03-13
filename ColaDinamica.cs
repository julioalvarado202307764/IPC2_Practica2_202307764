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

    public void Encolar(Turno nuevoTurno)
    {
        // El tiempo en cola del nuevo paciente es el tiempo acumulado de los que ya están
        nuevoTurno.TiempoEnCola = tiempoEsperaAcumulado;

        Nodo nuevoNodo = new Nodo(nuevoTurno);
        
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

        // Sumamos el tiempo de atención del nuevo paciente al total de la cola
        tiempoEsperaAcumulado += nuevoTurno.TiempoAtencion;
    }

    public Turno Desencolar()
    {
        if (frente == null) return null;

        Turno turnoAtendido = frente.Valor;
        frente = frente.Siguiente;

        if (frente == null)
        {
            final = null;
        }

        // Restamos el tiempo del paciente que acaba de salir
        tiempoEsperaAcumulado -= turnoAtendido.TiempoAtencion;

        // ¡Magia aquí! Actualizamos el tiempo en cola de los que se quedaron esperando
        ActualizarTiemposEnCola();

        return turnoAtendido;
    }

    // Método interno para recalcular la espera de los pacientes restantes
    private void ActualizarTiemposEnCola()
    {
        int tiempoAcumuladoTemp = 0;
        Nodo actual = frente;

        while (actual != null)
        {
            actual.Valor.TiempoEnCola = tiempoAcumuladoTemp;
            tiempoAcumuladoTemp += actual.Valor.TiempoAtencion;
            actual = actual.Siguiente;
        }
    }

    public bool EstaVacia()
    {
        return frente == null;
    }

    public Nodo ObtenerFrente()
    {
        return frente;
    }
}