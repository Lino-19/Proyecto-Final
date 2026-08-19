using Microsoft.EntityFrameworkCore;
using SistemaControlParqueos.Data;
using SistemaControlParqueos.Models;

using var repositorio = new ParqueoContext();

repositorio.Database.EnsureCreated();
CrearDatosIniciales(repositorio);

string opcion;
do
{
    Console.Clear();
    Console.WriteLine("=== SISTEMA DE CONTROL DE PARQUEOS ===");
    Console.WriteLine("1. Registrar vehículo");
    Console.WriteLine("2. Registrar entrada");
    Console.WriteLine("3. Registrar salida");
    Console.WriteLine("4. Consultar espacios");
    Console.WriteLine("5. Consultar vehículos dentro");
    Console.WriteLine("6. Consultar tickets");
    Console.WriteLine("7. Salir");
    Console.Write("Elige una opción: ");
    opcion = Console.ReadLine() ?? "";

    switch (opcion)
    {
        case "1": RegistrarVehiculo(repositorio); break;
        case "2": RegistrarEntrada(repositorio); break;
        case "3": RegistrarSalida(repositorio); break;
        case "4": MostrarEspacios(repositorio); break;
        case "5": MostrarVehiculosDentro(repositorio); break;
        case "6": MostrarTickets(repositorio); break;
        case "7": Console.WriteLine("Programa finalizado."); break;
        default: Console.WriteLine("Opción no válida."); Pausar(); break;
    }
}
while (opcion != "7");

void CrearDatosIniciales(ParqueoContext repositorio)
{
    Parqueo? parqueo = repositorio.Parqueos.FirstOrDefault();

    if (parqueo == null)
    {
        parqueo = new Parqueo();
        parqueo.Nombre = "Parqueo Principal";
        repositorio.Parqueos.Add(parqueo);
        repositorio.SaveChanges();
    }

    if (repositorio.Espacios.Any()) return;

    for (int numero = 1; numero <= 5; numero++)
    {
        EspacioParqueo espacio = new EspacioParqueo();
        espacio.ParqueoId = parqueo.Id;
        espacio.Numero = numero;
        espacio.EstaOcupado = false;
        repositorio.Espacios.Add(espacio);
    }

    repositorio.SaveChanges();
}

void RegistrarVehiculo(ParqueoContext repositorio)
{
    Console.Clear();
    Console.WriteLine("=== REGISTRAR VEHÍCULO ===");
    Console.Write("Placa: ");
    string placa = (Console.ReadLine() ?? "").Trim().ToUpper();

    if (string.IsNullOrWhiteSpace(placa))
    {
        Console.WriteLine("La placa es obligatoria.");
        Pausar();
        return;
    }

    if (repositorio.Vehiculos.Any(vehiculo => vehiculo.Placa == placa))
    {
        Console.WriteLine("Este vehículo ya está registrado.");
        Pausar();
        return;
    }

    Console.Write("Tipo de vehículo: ");
    string nombreTipo = (Console.ReadLine() ?? "").Trim();

    if (string.IsNullOrWhiteSpace(nombreTipo))
    {
        Console.WriteLine("El tipo de vehículo es obligatorio.");
        Pausar();
        return;
    }

    TipoVehiculo? tipo = repositorio.TiposVehiculo.FirstOrDefault(t => t.Nombre == nombreTipo);

    if (tipo == null)
    {
        tipo = new TipoVehiculo();
        tipo.Nombre = nombreTipo;
        repositorio.TiposVehiculo.Add(tipo);
        repositorio.SaveChanges();
    }

    Vehiculo vehiculoNuevo = new Vehiculo();
    vehiculoNuevo.Placa = placa;
    vehiculoNuevo.TipoVehiculoId = tipo.Id;
    repositorio.Vehiculos.Add(vehiculoNuevo);
    repositorio.SaveChanges();

    Console.WriteLine("Vehículo registrado correctamente.");
    Pausar();
}

void RegistrarEntrada(ParqueoContext repositorio)
{
    Console.Clear();
    Console.WriteLine("=== REGISTRAR ENTRADA ===");
    Console.Write("Placa: ");
    string placa = (Console.ReadLine() ?? "").Trim().ToUpper();
    Vehiculo? vehiculo = repositorio.Vehiculos.FirstOrDefault(v => v.Placa == placa);

    if (vehiculo == null)
    {
        Console.WriteLine("El vehículo no está registrado.");
        Pausar();
        return;
    }

    if (repositorio.Tickets.Any(ticket => ticket.VehiculoId == vehiculo.Id && ticket.HoraSalida == null))
    {
        Console.WriteLine("Este vehículo ya está dentro del parqueo.");
        Pausar();
        return;
    }

    EspacioParqueo? espacio = repositorio.Espacios.FirstOrDefault(e => e.EstaOcupado == false);

    if (espacio == null)
    {
        Console.WriteLine("No hay espacios disponibles.");
        Pausar();
        return;
    }

    Ticket ticketNuevo = new Ticket();
    ticketNuevo.VehiculoId = vehiculo.Id;
    ticketNuevo.EspacioParqueoId = espacio.Id;
    ticketNuevo.HoraEntrada = DateTime.Now;
    espacio.Ocupar();
    repositorio.Tickets.Add(ticketNuevo);
    repositorio.SaveChanges();

    Console.WriteLine("Entrada registrada correctamente.");
    Console.WriteLine($"Número de ticket: {ticketNuevo.Id}");
    Console.WriteLine($"Espacio asignado: {espacio.Numero}");
    Console.WriteLine($"Hora de entrada: {ticketNuevo.HoraEntrada:dd/MM/yyyy HH:mm}");
    Pausar();
}

void RegistrarSalida(ParqueoContext repositorio)
{
    Console.Clear();
    Console.WriteLine("=== REGISTRAR SALIDA ===");
    Console.Write("Número de ticket: ");

    if (!int.TryParse(Console.ReadLine(), out int ticketId))
    {
        Console.WriteLine("Debes escribir un número válido.");
        Pausar();
        return;
    }

    Ticket? ticket = repositorio.Tickets
        .Include(t => t.Vehiculo)
        .Include(t => t.EspacioParqueo)
        .FirstOrDefault(t => t.Id == ticketId && t.HoraSalida == null);

    if (ticket == null)
    {
        Console.WriteLine("No se encontró un ticket activo con ese número.");
        Pausar();
        return;
    }

    ticket.HoraSalida = DateTime.Now;

    if (ticket.EspacioParqueo != null)
        ticket.EspacioParqueo.Liberar();

    repositorio.SaveChanges();

    Console.WriteLine("Salida registrada correctamente.");
    Console.WriteLine($"Vehículo: {ticket.Vehiculo?.Placa}");
    Console.WriteLine($"Hora de salida: {ticket.HoraSalida:dd/MM/yyyy HH:mm}");
    Console.WriteLine($"Tiempo de permanencia: {ticket.CalcularMinutos()} minuto(s)");
    Pausar();
}

void MostrarEspacios(ParqueoContext repositorio)
{
    Console.Clear();
    Console.WriteLine("=== ESPACIOS DE PARQUEO ===");
    List<EspacioParqueo> espacios = repositorio.Espacios.OrderBy(e => e.Numero).ToList();

    foreach (EspacioParqueo espacio in espacios)
        espacio.MostrarEstado();

    Pausar();
}

void MostrarVehiculosDentro(ParqueoContext repositorio)
{
    Console.Clear();
    Console.WriteLine("=== VEHÍCULOS DENTRO DEL PARQUEO ===");
    List<Ticket> tickets = repositorio.Tickets
        .Include(t => t.Vehiculo)
        .ThenInclude(v => v!.TipoVehiculo)
        .Where(t => t.HoraSalida == null)
        .ToList();

    if (tickets.Count == 0)
    {
        Console.WriteLine("No hay vehículos dentro del parqueo.");
    }
    else
    {
        foreach (Ticket ticket in tickets)
            ticket.Vehiculo?.MostrarDatos();
    }

    Pausar();
}

void MostrarTickets(ParqueoContext repositorio)
{
    Console.Clear();
    Console.WriteLine("=== TICKETS REGISTRADOS ===");
    List<Ticket> tickets = repositorio.Tickets
        .Include(t => t.Vehiculo)
        .Include(t => t.EspacioParqueo)
        .OrderBy(t => t.Id)
        .ToList();

    if (tickets.Count == 0)
    {
        Console.WriteLine("No hay tickets para mostrar.");
    }
    else
    {
        foreach (Ticket ticket in tickets)
        {
            ticket.MostrarDatos();
            Console.WriteLine();
        }
    }

    Pausar();
}

void Pausar()
{
    Console.WriteLine("\nPresiona una tecla para continuar...");
    Console.ReadKey();
}
