using Microsoft.EntityFrameworkCore;
using TravelPlannerAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TravelPlannerDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 21))));

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TravelPlannerDbContext>();
    try
    {
        // Intentamos hacer una consulta simple para verificar la conexión
        dbContext.Database.CanConnect();
        Console.WriteLine("Conexión a la base de datos exitosa.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al conectar a la base de datos: {ex.Message}");
    }
}

Console.WriteLine($"Aplicación levantada en el puerto: {builder.Configuration["ASPNETCORE_URLS"]}");

app.Run();

