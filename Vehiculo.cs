namespace SistemaControlParqueos.Models;

public class Vehiculo
{
    public int Id { get; set; }
    public int TipoVehiculoId { get; set; }
    public TipoVehiculo? TipoVehiculo { get; set; }
    public string Placa { get; set; } = "";

    public void MostrarDatos()
    {
        Console.WriteLine($"Placa: {Placa}");
        Console.WriteLine($"Tipo: {TipoVehiculo?.Nombre}");
    }
}
