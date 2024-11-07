using Microsoft.EntityFrameworkCore;
using UnphuCard_Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();  // Agregar Swagger

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

app.UseAuthorization();

app.MapControllers();

app.Run();
