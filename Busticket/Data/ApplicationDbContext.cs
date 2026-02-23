using Busticket.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Busticket.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Ruta> Ruta { get; set; }
        public DbSet<Empresa> Empresa { get; set; }
        public DbSet<Bus> Bus { get; set; }
        public DbSet<Conductor> Conductor { get; set; }
        public DbSet<Itinerario> Itinerario { get; set; }
        public DbSet<Boleto> Boleto { get; set; }
        public DbSet<Oferta> Oferta { get; set; }
        public DbSet<Resena> Resena { get; set; }
        public DbSet<Reporte> Reporte { get; set; }
        public DbSet<Asiento> Asiento { get; set; }
        public DbSet<Venta> Venta { get; set; }
        public DbSet<Ciudad> Ciudad { get; set; }
        public DbSet<PasswordReset> PasswordReset { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---------------- TABLAS EN SINGULAR ----------------
            modelBuilder.Entity<Ruta>().ToTable("Ruta");
            modelBuilder.Entity<Empresa>().ToTable("Empresa");
            modelBuilder.Entity<Bus>().ToTable("Bus");
            modelBuilder.Entity<Conductor>().ToTable("Conductor");
            modelBuilder.Entity<Itinerario>().ToTable("Itinerario");
            modelBuilder.Entity<Boleto>().ToTable("Boleto");
            modelBuilder.Entity<Oferta>().ToTable("Oferta");
            modelBuilder.Entity<Resena>().ToTable("Resena");
            modelBuilder.Entity<Reporte>().ToTable("Reporte");
            modelBuilder.Entity<Asiento>().ToTable("Asiento");
            modelBuilder.Entity<Venta>().ToTable("Venta");
            modelBuilder.Entity<Ciudad>().ToTable("Ciudad");
            modelBuilder.Entity<PasswordReset>().ToTable("PasswordReset");

            // ---------------- PROPIEDADES ----------------
            modelBuilder.Entity<Ruta>()
                .Property(r => r.Precio)
                .HasColumnType("decimal(18,2)");

            // ---------------- RUTA ----------------
            modelBuilder.Entity<Ruta>()
                .HasOne(r => r.CiudadOrigen)
                .WithMany()
                .HasForeignKey(r => r.CiudadOrigenId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Ruta>()
                .HasOne(r => r.CiudadDestino)
                .WithMany()
                .HasForeignKey(r => r.CiudadDestinoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------------- ITINERARIO ----------------
            modelBuilder.Entity<Itinerario>()
                .HasOne(i => i.Ruta)
                .WithMany()
                .HasForeignKey(i => i.RutaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Itinerario>()
                .HasOne(i => i.Bus)
                .WithMany()
                .HasForeignKey(i => i.BusId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Itinerario>()
                .HasOne(i => i.Conductor)
                .WithMany()
                .HasForeignKey(i => i.ConductorId)
                .OnDelete(DeleteBehavior.Restrict);


            //-----------------Cliente----------------
            modelBuilder.Entity<Cliente>().ToTable("Cliente");

            modelBuilder.Entity<Cliente>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------------- ASIENTO ----------------
            modelBuilder.Entity<Asiento>()
                .HasOne(a => a.Ruta)
                .WithMany(r => r.Asiento)
                .HasForeignKey(a => a.RutaId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------------- VENTA ----------------
            modelBuilder.Entity<Venta>()
                .HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Empresa)
                .WithMany()
                .HasForeignKey(v => v.EmpresaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Venta>()
                .HasOne(v => v.Ruta)
                .WithMany()
                .HasForeignKey(v => v.RutaId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------------- BOLETO ----------------
            // ÚNICO CASCADE PERMITIDO
            modelBuilder.Entity<Boleto>()
                .HasOne(b => b.Venta)
                .WithMany(v => v.Boletos)
                .HasForeignKey(b => b.VentaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Boleto>()
                .HasOne(b => b.User)
                .WithMany()
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Boleto>()
                .HasOne(b => b.Ruta)
                .WithMany()
                .HasForeignKey(b => b.RutaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Boleto>()
                .HasOne(b => b.Asiento)
                .WithMany()
                .HasForeignKey(b => b.AsientoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------------- RESENA ----------------
            modelBuilder.Entity<Resena>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Resena>()
                .HasOne(r => r.Ruta)
                .WithMany()
                .HasForeignKey(r => r.RutaId)
                .OnDelete(DeleteBehavior.Restrict);

            // ---------------- SEED CIUDADES ----------------
            modelBuilder.Entity<Ciudad>().HasData(
                new Ciudad { CiudadId = 1, Nombre = "Bogotá", Lat = 4.60971, Lng = -74.08175 },
                new Ciudad { CiudadId = 2, Nombre = "Medellín", Lat = 6.2442, Lng = -75.58121 },
                new Ciudad { CiudadId = 3, Nombre = "Cali", Lat = 3.43722, Lng = -76.5225 },
                new Ciudad { CiudadId = 4, Nombre = "Cartagena", Lat = 10.4, Lng = -75.5 },
                new Ciudad { CiudadId = 5, Nombre = "Barranquilla", Lat = 10.96389, Lng = -74.79639 },
                new Ciudad { CiudadId = 6, Nombre = "Bucaramanga", Lat = 7.12539, Lng = -73.1198 }
            );
        }
    }
}
