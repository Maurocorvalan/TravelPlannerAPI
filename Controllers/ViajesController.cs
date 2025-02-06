using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TravelPlannerAPI.Models;

public class ViajesController : ControllerBase
{
    private readonly TravelPlannerDbContext _context;

    public ViajesController(TravelPlannerDbContext context)
    {
        _context = context;
    }



    // Crear un nuevo viaje
    [HttpPost]
    [Authorize]
    [Route("api/viajes")]
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
    [Route("api/viajes")]
    public async Task<IActionResult> ObtenerViajes()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var viajes = await _context.Viajes
            .Where(v => v.IdUsuario == userId)
            .ToListAsync();

        return Ok(viajes);
    }

    // Obtener un viaje por su id
    [HttpGet]
    [Authorize]
    [Route("api/viajes/{id}")]
    public async Task<IActionResult> ObtenerViaje(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var viaje = await _context.Viajes
            .Where(v => v.IdViaje == id && v.IdUsuario == userId)
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
        if (viaje == null || viaje.IdViaje != id)
        {
            return BadRequest("Datos del viaje inválidos.");
        }

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var viajeExistente = await _context.Viajes
            .Where(v => v.IdViaje == id && v.IdUsuario == userId)
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
