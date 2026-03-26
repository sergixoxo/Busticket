using Busticket.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using Busticket.Services; // Asegúrate de tener este namespace

var builder = WebApplication.CreateBuilder(args);

// ===============================
// 1. BASE DE DATOS
// ===============================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ===============================
// 2. IDENTITY & COOKIES
// ===============================
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false; // Ajuste opcional para facilitar pruebas
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/AccessDenied"; // Ruta recomendada
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true; // Seguridad extra
});

// ===============================
// 3. SERVICIOS ADICIONALES
// ===============================
builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Vital para que el carrito funcione en la nube
});

// Configuración de QuestPDF
QuestPDF.Settings.License = LicenseType.Community;

// Registro de servicios propios
builder.Services.AddTransient<EmailService>();

var app = builder.Build();

// ===============================
// 4. CONFIGURACIÓN DEL ENTORNO (PIPELINE)
// ===============================
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error"); // Ruta estándar de error
    app.UseHsts();
}

app.UseHttpsRedirection();

// Configuración de tipos de archivo estáticos (GLB para modelos 3D, etc.)
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".glb"] = "model/gltf-binary";
provider.Mappings[".json"] = "application/json";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider,
    ServeUnknownFileTypes = true
});

// --- ORDEN CRÍTICO DE MIDDLEWARES ---
app.UseRouting();

app.UseSession();        // 1. Primero la sesión
app.UseAuthentication(); // 2. Luego quién es el usuario
app.UseAuthorization();  // 3. Finalmente qué puede hacer

// ===============================
// 5. RUTAS Y SEEDING
// ===============================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

// Ejecución de Seeders (Roles y Admin)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await IdentitySeeder.SeedRolesAndAdmin(services);
    }
    catch (Exception ex)
    {
        // Esto evita que la app truene si el seeder falla en el primer inicio
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocurrió un error al sembrar la base de datos.");
    }
}

app.Run();