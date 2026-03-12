using Microsoft.EntityFrameworkCore;
using static DonarumaAPI_DTOs.Compras.MetodoPagoDto;
using DonarumaAPI_DTOs.Compras;

namespace DonarumaAPI_Data.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<MetodoPago> MetodosPago { get; set; }
        public DbSet<Perfume> Perfumes { get; set; }

        // Entidad sin llave para resultados de funciones
        public DbSet<UsuarioDatosDto> UsuarioDatosDtos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Forzamos el mapeo por si los atributos del modelo no bastan
            modelBuilder.Entity<Usuario>().ToTable("usuarios");
            modelBuilder.Entity<Perfume>().ToTable("perfumes");
            modelBuilder.Entity<MetodoPago>().ToTable("metodos_pago");
            // Configuración para el DTO que no tiene ID
            modelBuilder.Entity<UsuarioDatosDto>().HasNoKey();

            base.OnModelCreating(modelBuilder);
        }
    }
}