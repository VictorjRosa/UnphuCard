using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using UnphuCard_Api.Models;
using UnphuCard_Api.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();  // Agregar Swagger
builder.Services.AddScoped<IServicioEmail, EmailService>();
builder.Services.AddScoped<IVerificarCedula, VerificarCedulaServices>();
builder.Services.AddScoped<ProductoService, ProductoService>();
builder.Services.AddSingleton<BlobStorageService>();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]);

// Configurar JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(secretKey)
    };
});

// Configura el contexto de la base de datos
builder.Services.AddDbContext<UnphuCardContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("UnphuCardContext"));
});

var app = builder.Build();

// Configura el pipeline de solicitudes HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Habilitar Swagger en producción (opcional)
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mi API V1");
        c.RoutePrefix = string.Empty; // Hace que Swagger esté disponible en la raíz del sitio
    });
}

app.UseHttpsRedirection();
app.UseCors(builder =>
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader()
);

app.UseAuthentication(); // Habilitar autenticación
app.UseAuthorization();

// Mapear rutas de los controladores
app.MapControllers();

app.Run();
