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

            modelBuilder.Entity<Empresa>()
                .Property(e => e.FechaRegistro)
                .HasDefaultValueSql("GETUTCDATE()");

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
                .WithMany(r => r.Asientos)
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
       new Ciudad { CiudadId = 6, Nombre = "Bucaramanga", Lat = 7.12539, Lng = -73.1198 },
       new Ciudad { CiudadId = 7, Nombre = "Pereira", Lat = 4.81333, Lng = -75.69611 },
       new Ciudad { CiudadId = 8, Nombre = "Manizales", Lat = 5.07028, Lng = -75.51389 },
       new Ciudad { CiudadId = 9, Nombre = "Santa Marta", Lat = 11.24078, Lng = -74.19904 },
       new Ciudad { CiudadId = 10, Nombre = "Ibagué", Lat = 4.43889, Lng = -75.23222 },
       new Ciudad { CiudadId = 11, Nombre = "Neiva", Lat = 2.92767, Lng = -75.28194 },
       new Ciudad { CiudadId = 12, Nombre = "Cúcuta", Lat = 7.8891, Lng = -72.4969 },
       new Ciudad { CiudadId = 13, Nombre = "Sincelejo", Lat = 9.30444, Lng = -75.39722 },
       new Ciudad { CiudadId = 14, Nombre = "Valledupar", Lat = 10.46333, Lng = -73.25306 },
       new Ciudad { CiudadId = 15, Nombre = "Popayán", Lat = 2.4441, Lng = -76.6145 },
       new Ciudad { CiudadId = 16, Nombre = "Pasto", Lat = 1.21361, Lng = -77.28111 },
       new Ciudad { CiudadId = 17, Nombre = "Tunja", Lat = 5.53389, Lng = -73.36778 },
       new Ciudad { CiudadId = 18, Nombre = "Montería", Lat = 8.74778, Lng = -75.88111 },
       new Ciudad { CiudadId = 19, Nombre = "Floridablanca", Lat = 7.06944, Lng = -73.08778 },
       new Ciudad { CiudadId = 20, Nombre = "Riohacha", Lat = 11.54472, Lng = -72.90722 },
       new Ciudad { CiudadId = 21, Nombre = "Yopal", Lat = 5.33778, Lng = -72.39111 },
       new Ciudad { CiudadId = 22, Nombre = "Arauca", Lat = 7.08806, Lng = -70.76222 },
       new Ciudad { CiudadId = 23, Nombre = "Malambo", Lat = 10.83361, Lng = -74.83361 },
       new Ciudad { CiudadId = 24, Nombre = "Itagüí", Lat = 6.17444, Lng = -75.61194 },
       new Ciudad { CiudadId = 25, Nombre = "Envigado", Lat = 6.17028, Lng = -75.5775 },
       new Ciudad { CiudadId = 26, Nombre = "Soledad", Lat = 10.91639, Lng = -74.76639 },
       new Ciudad { CiudadId = 27, Nombre = "Bello", Lat = 6.3375, Lng = -75.55833 },
       new Ciudad { CiudadId = 28, Nombre = "Tunja", Lat = 5.53611, Lng = -73.3675 },
       new Ciudad { CiudadId = 29, Nombre = "Buenaventura", Lat = 3.89361, Lng = -77.06722 },
       new Ciudad { CiudadId = 30, Nombre = "Santa Rosa de Cabal", Lat = 4.89444, Lng = -75.31611 }
   );
            // ---------------- SEED ROLES (DATOS INICIALES) ----------------
            // Definimos IDs fijos para que no cambien nunca
            string adminRoleId = "83ca341c-300c-4034-93c6-291771120464";
            string clienteRoleId = "543f9a78-295b-4330-80e3-4672e8790089";
            string EmpresaRoleId = "543f9a78-123b-4330-80e4-4672y8794549";

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
            {
                Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "2ca686af-face-4aef-aff1-a6f990c5b846"
            },
            new IdentityRole
            {
                Id = clienteRoleId,
                    Name = "Cliente",
                    NormalizedName = "CLIENTE",
                    ConcurrencyStamp = "cceacb20-2f45-4a2f-a1ca-585359e59bdb"
            },
            new IdentityRole
            {
                Id = EmpresaRoleId,
                    Name = "Empresa",
                    NormalizedName = "EMPRESA",
                    ConcurrencyStamp = "c3026a58-0ded-44aa-bf93-2e783d4b556c"
            }
            );
        }
    }
}
