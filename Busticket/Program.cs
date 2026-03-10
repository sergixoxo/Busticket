using Busticket.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ===============================
// DATABASE
// ===============================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// ===============================
// IDENTITY (ESTO YA MANEJA COOKIES)
// ===============================
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ===============================
// COOKIE CONFIG (IDENTITY)
// ===============================
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Auth/Login";
    options.AccessDeniedPath = "/Auth/Login";
    options.SlidingExpiration = true;
});

// ===============================
builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddTransient<Busticket.Services.EmailService>();




var app = builder.Build();

// ===============================
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error/Error500");
    app.UseStatusCodePagesWithReExecute("/Error/HttpStatus/{0}");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseDeveloperExceptionPage();
// ===============================
// STATIC FILES
// ===============================
var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".glb"] = "model/gltf-binary";
provider.Mappings[".json"] = "application/json";

app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider,
    ServeUnknownFileTypes = true
});

// ===============================
// PIPELINE (ORDEN CORRECTO)
// ===============================
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// ===============================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();
