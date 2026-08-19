using Microsoft.EntityFrameworkCore;
using SistemaControlParqueos.Models;

namespace SistemaControlParqueos.Data;

public class ParqueoContext : DbContext
{
    public DbSet<Parqueo> Parqueos => Set<Parqueo>();
    public DbSet<TipoVehiculo> TiposVehiculo => Set<TipoVehiculo>();
    public DbSet<Vehiculo> Vehiculos => Set<Vehiculo>();
    public DbSet<EspacioParqueo> Espacios => Set<EspacioParqueo>();
    public DbSet<Ticket> Tickets => Set<Ticket>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            @"Server=(localdb)\MSSQLLocalDB;Database=SistemaControlParqueosRequisitosBasicosDB;Trusted_Connection=True;TrustServerCertificate=True;");
    }
}
