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
        public DbSet<Viaje_Lugar> Viaje_Lugar { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de la clave primaria compuesta para Viaje_Lugar
            modelBuilder.Entity<Viaje_Lugar>()
                .HasKey(vl => new { vl.IdViaje, vl.IdLugar });

            modelBuilder.Entity<Viaje>()
                .Property(v => v.IdUsuario)
                .HasColumnName("id_usuario");




        }
    }
}
