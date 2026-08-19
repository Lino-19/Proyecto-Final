namespace SistemaControlParqueos.Models;

public class EspacioParqueo
{
    public int Id { get; set; }
    public int ParqueoId { get; set; }
    public Parqueo? Parqueo { get; set; }
    public int Numero { get; set; }
    public bool EstaOcupado { get; set; }

    public void Ocupar()
    {
        EstaOcupado = true;
    }

    public void Liberar()
    {
        EstaOcupado = false;
    }

    public void MostrarEstado()
    {
        string estado = EstaOcupado ? "Ocupado" : "Disponible";
        Console.WriteLine($"Espacio {Numero}: {estado}");
    }
}
