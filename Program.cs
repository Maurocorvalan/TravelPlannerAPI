using Microsoft.EntityFrameworkCore;
using TravelPlannerAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Configuration;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://10.0.2.2:5173") // Permite el acceso desde el emulador
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5173); // Escuchar en todas las IPs
});
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<TravelPlannerDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 21))));

builder.Services.AddEndpointsApiExplorer();

var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtSettings:SecretKey"]);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"]
        };
    });

builder.Services.AddAuthorization();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TravelPlannerDbContext>();
    try
    {
        dbContext.Database.CanConnect();
        Console.WriteLine("Conexión a la base de datos exitosa.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error al conectar a la base de datos: {ex.Message}");
    }
}

Console.WriteLine($"Aplicación levantada en el puerto: {builder.Configuration["ASPNETCORE_URLS"]}");
app.UseAuthentication();
app.UseAuthorization();
app.UseCors(MyAllowSpecificOrigins);
app.MapControllers();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "uploads")),
    RequestPath = "/uploads"
});
app.Run();

