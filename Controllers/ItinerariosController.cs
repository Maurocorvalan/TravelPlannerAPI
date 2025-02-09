using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using TravelPlannerAPI.Models;

public class ItinerariosController : ControllerBase
{
    private readonly TravelPlannerDbContext _context;

    public ItinerariosController(TravelPlannerDbContext context)
    {
        _context = context;
    }

    // Crear un nuevo itinerario
    [HttpPost]
    [Authorize]
    [Route("api/viajes/{idViaje}/itinerarios")]
    public async Task<IActionResult> CrearItinerario(int idViaje, [FromBody] Itinerario itinerario)
    {
        var viajeExistente = await _context.Viajes
            .Where(v => v.IdViaje == idViaje)
            .FirstOrDefaultAsync();

        if (viajeExistente == null)
        {
            return NotFound("El viaje no existe.");
        }

        itinerario.IdViaje = idViaje;

        _context.Itinerarios.Add(itinerario);
        await _context.SaveChangesAsync();

        return Ok(itinerario);
    }

    // Obtener todos los itinerarios de un viaje
    [HttpGet]
    [Authorize]
    [Route("api/viajes/{idViaje}/itinerarios")]
    public async Task<IActionResult> ObtenerItinerarios(int idViaje)
    {
        var viajeExistente = await _context.Viajes
            .Where(v => v.IdViaje == idViaje)
            .FirstOrDefaultAsync();

        if (viajeExistente == null)
        {
            return NotFound("El viaje no existe.");
        }

        var itinerarios = await _context.Itinerarios
            .Where(i => i.IdViaje == idViaje)
            .ToListAsync();

        return Ok(itinerarios);
    }

    // Obtener un itinerario por su id
    [HttpGet]
    [Authorize]
    [Route("api/viajes/{idViaje}/itinerarios/{id}")]
    public async Task<IActionResult> ObtenerItinerario(int idViaje, int id)
    {
        var viajeExistente = await _context.Viajes
            .Where(v => v.IdViaje == idViaje)
            .FirstOrDefaultAsync();

        if (viajeExistente == null)
        {
            return NotFound("El viaje no existe.");
        }

        var itinerario = await _context.Itinerarios
            .Where(i => i.IdViaje == idViaje && i.IdItinerario == id)
            .FirstOrDefaultAsync();

        if (itinerario == null)
        {
            return NotFound("El itinerario no existe o no pertenece a este viaje.");
        }

        return Ok(itinerario);
    }

    // Actualizar un itinerario
    [HttpPut]
    [Authorize]
    [Route("api/viajes/{idViaje}/itinerarios/{id}")]
    public async Task<IActionResult> ActualizarItinerario(int idViaje, int id, [FromBody] Itinerario itinerario)
    {
        var viajeExistente = await _context.Viajes
            .Where(v => v.IdViaje == idViaje)
            .FirstOrDefaultAsync();

        if (viajeExistente == null)
        {
            return NotFound("El viaje no existe.");
        }

        var itinerarioExistente = await _context.Itinerarios
            .Where(i => i.IdViaje == idViaje && i.IdItinerario == id)
            .FirstOrDefaultAsync();

        if (itinerarioExistente == null)
        {
            return NotFound("El itinerario no existe o no pertenece a este viaje.");
        }

        itinerarioExistente.Fecha = itinerario.Fecha;
        itinerarioExistente.Actividad = itinerario.Actividad;
        itinerarioExistente.Ubicacion = itinerario.Ubicacion;

        _context.Entry(itinerarioExistente).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(itinerarioExistente);
    }

    // Eliminar un itinerario
    [HttpDelete]
    [Authorize]
    [Route("api/viajes/{idViaje}/itinerarios/{id}")]
    public async Task<IActionResult> EliminarItinerario(int idViaje, int id)
    {
        var viajeExistente = await _context.Viajes
            .Where(v => v.IdViaje == idViaje)
            .FirstOrDefaultAsync();

        if (viajeExistente == null)
        {
            return NotFound("El viaje no existe.");
        }

        var itinerario = await _context.Itinerarios
            .Where(i => i.IdViaje == idViaje && i.IdItinerario == id)
            .FirstOrDefaultAsync();

        if (itinerario == null)
        {
            return NotFound("El itinerario no existe o no pertenece a este viaje.");
        }

        _context.Itinerarios.Remove(itinerario);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
