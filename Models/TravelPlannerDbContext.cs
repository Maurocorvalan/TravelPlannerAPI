using Microsoft.EntityFrameworkCore;

namespace TravelPlannerAPI.Models
{
    public class TravelPlannerDbContext : DbContext
    {
        public TravelPlannerDbContext(DbContextOptions<TravelPlannerDbContext> options)
            : base(options)
        { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Viaje> Viajes { get; set; }
        public DbSet<Itinerario> Itinerarios { get; set; }
        public DbSet<Gasto> Gastos { get; set; }
        public DbSet<Foto> Fotos { get; set; }
        public DbSet<Lugar> Lugares { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<Preferencia> Preferencias { get; set; }
    }
}
