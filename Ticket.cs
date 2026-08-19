namespace SistemaControlParqueos.Models;

public class Ticket
{
    public int Id { get; set; }
    public int VehiculoId { get; set; }
    public Vehiculo? Vehiculo { get; set; }
    public int EspacioParqueoId { get; set; }
    public EspacioParqueo? EspacioParqueo { get; set; }
    public DateTime HoraEntrada { get; set; }
    public DateTime? HoraSalida { get; set; }

    public double CalcularMinutos()
    {
        DateTime horaFinal = HoraSalida ?? DateTime.Now;
        TimeSpan tiempo = horaFinal - HoraEntrada;
        return Math.Ceiling(tiempo.TotalMinutes);
    }

    public void MostrarDatos()
    {
        Console.WriteLine($"Ticket: #{Id}");
        Console.WriteLine($"Placa: {Vehiculo?.Placa}");
        Console.WriteLine($"Espacio: {EspacioParqueo?.Numero}");
        Console.WriteLine($"Entrada: {HoraEntrada:dd/MM/yyyy HH:mm}");

        if (HoraSalida != null)
        {
            Console.WriteLine($"Salida: {HoraSalida.Value:dd/MM/yyyy HH:mm}");
            Console.WriteLine($"Tiempo: {CalcularMinutos()} minuto(s)");
        }
        else
        {
            Console.WriteLine("Estado: El vehículo sigue dentro del parqueo.");
        }
    }
}
