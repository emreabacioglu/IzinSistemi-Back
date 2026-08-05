using IzinSistemi_Back.Data;
using IzinSistemi_Back.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CORS SERVİSİ (Hem Local hem de Vercel/Canlı Ortam İçin Tüm İsteklere İzin Verir) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// --- 2. VERİTABANI & DİĞER SERVİSLER ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddMemoryCache();

var app = builder.Build();

// --- 3. PIPELINE & CORS KULLANIMI ---
// UYARI: app.UseCors her zaman Routing ve Authorization'dan ÖNCE çağrılmalıdır!
app.UseCors("AllowAll");

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();

app.Run();