using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TravelPlannerAPI.Models;
using Newtonsoft.Json;

public class ViajesController : ControllerBase
{
    private readonly TravelPlannerDbContext _context;

    public ViajesController(TravelPlannerDbContext context)
    {
        _context = context;
    }



    // Crear un nuevo viaje
    [HttpPost]
    [Route("api/viajes")]
    [Authorize]

    public async Task<IActionResult> CrearViaje([FromBody] Viaje viaje)
    {
        if (viaje == null)
        {
            return BadRequest("Los datos del viaje son inválidos.");
        }

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        viaje.IdUsuario = userId;

        _context.Viajes.Add(viaje);
        await _context.SaveChangesAsync();

        return CreatedAtAction("ObtenerViaje", new { id = viaje.IdViaje }, viaje);
    }

    // Obtener todos los viajes de un usuario
    [HttpGet]
    [Authorize]
    [Route("api/viajes/viajesuser")]
    public async Task<IActionResult> ObtenerViajes()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var viajes = await _context.Viajes
            .Where(v => v.IdUsuario == userId)
            .ToListAsync();

        return Ok(viajes.Select(v => new
        {
            v.IdViaje,
            v.IdUsuario,
            v.Nombre,
            v.Descripcion,
            FechaInicio = v.FechaInicio.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            FechaFin = v.FechaFin.ToString("yyyy-MM-ddTHH:mm:ssZ")
        }));
    }

    // Obtener un viaje por su id
    [HttpGet]
    [Authorize]

    [Route("api/viajes/{id}")]
    public async Task<IActionResult> ObtenerViaje(int id)
    {
        var viaje = await _context.Viajes
            .Where(v => v.IdViaje == id)
            .Select(v => new
            {
                IdViaje = v.IdViaje,
                Nombre = v.Nombre,
                FechaInicio = v.FechaInicio.ToString("yyyy-MM-dd'T'HH:mm:ss"), // Formato ISO 8601
                Descripcion = v.Descripcion,
                FechaFin = v.FechaFin.ToString("yyyy-MM-dd'T'HH:mm:ss"), // Formato ISO 8601
            })
            .FirstOrDefaultAsync();

        if (viaje == null)
        {
            return NotFound("El viaje no existe o no pertenece a este usuario.");
        }

        return Ok(viaje);
    }

    // Actualizar un viaje
    [HttpPut]
    [Authorize]

    [Route("api/viajes/{id}")]
    public async Task<IActionResult> ActualizarViaje(int id, [FromBody] Viaje viaje)
    {

        Console.WriteLine("este es el viaje" + viaje);  // Verifica los datos recibidos

        if (viaje == null || viaje.IdViaje != id)
        {
            Console.WriteLine("Datos inválidos para el viaje" + viaje);
            return BadRequest("Datos del viaje inválidos.");
        }


        var viajeExistente = await _context.Viajes
            .Where(v => v.IdViaje == id)
            .FirstOrDefaultAsync();

        if (viajeExistente == null)
        {
            return NotFound("El viaje no existe o no pertenece a este usuario.");
        }

        viajeExistente.Nombre = viaje.Nombre;
        viajeExistente.Descripcion = viaje.Descripcion;
        viajeExistente.FechaInicio = viaje.FechaInicio;
        viajeExistente.FechaFin = viaje.FechaFin;

        _context.Entry(viajeExistente).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(viajeExistente);
    }

    // Eliminar un viaje
    [HttpDelete]
    [Authorize]
    [Route("api/viajes/{id}")]
    public async Task<IActionResult> EliminarViaje(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var viaje = await _context.Viajes
            .Where(v => v.IdViaje == id && v.IdUsuario == userId)
            .FirstOrDefaultAsync();

        if (viaje == null)
        {
            return NotFound("El viaje no existe o no pertenece a este usuario.");
        }

        _context.Viajes.Remove(viaje);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
