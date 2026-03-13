public class Nodo
{
    public Turno Valor { get; set; }
    public Nodo Siguiente { get; set; }

    public Nodo(Turno valor)
    {
        Valor = valor;
        Siguiente = null;
    }
}