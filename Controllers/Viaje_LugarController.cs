using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using TravelPlannerAPI.Models;

[Route("api/viajes/lugares")]
[ApiController]
public class ViajeLugarController : ControllerBase
{
    private readonly TravelPlannerDbContext _context;

    public ViajeLugarController(TravelPlannerDbContext context)
    {
        _context = context;
    }

    // Asociar un lugar a un viaje
    [Authorize]
    [HttpPost("agregar-lugar-viaje")]
    public async Task<IActionResult> AgregarLugarViaje([FromBody] Viaje_Lugar viajeLugar)
    {
        // Asegúrate de que los campos IdViaje y IdLugar no sean nulos o inválidos
        if (viajeLugar.IdViaje <= 0 || viajeLugar.IdLugar <= 0)
        {
            return BadRequest("Los identificadores de viaje y lugar son obligatorios.");
        }

        // Inserta el registro en la tabla intermedia Viaje_Lugar
        _context.Viaje_Lugar.Add(viajeLugar);
        await _context.SaveChangesAsync();

        return Ok("Lugar agregado al viaje exitosamente.");
    }


    // Obtener lugares de un viaje
    [HttpGet("viajes-lugares")]
    [Authorize]

    public async Task<IActionResult> ObtenerTodosLosViajesLugares()
    {
        var viajesLugares = await _context.Viaje_Lugar.ToListAsync();
        return Ok(viajesLugares);
    }





    [HttpGet("viaje/{idViaje}/lugares")]
    [Authorize]

    public async Task<IActionResult> ObtenerLugaresPorViaje(int idViaje)
    {
        var lugares = await _context.Viaje_Lugar
            .Where(vl => vl.IdViaje == idViaje)
            .ToListAsync();

        if (lugares == null || lugares.Count == 0)
        {
            return NotFound("No se encontraron lugares para este viaje.");
        }

        return Ok(lugares);
    }


    [HttpGet("lugar/{idLugar}/viajes")]
    [Authorize]

    public async Task<IActionResult> ObtenerViajesPorLugar(int idLugar)
    {
        var viajes = await _context.Viaje_Lugar
            .Where(vl => vl.IdLugar == idLugar)
            .ToListAsync();

        if (viajes == null || viajes.Count == 0)
        {
            return NotFound("No se encontraron viajes para este lugar.");
        }

        return Ok(viajes);
    }



    // Eliminar un lugar de un viaje

    [HttpDelete("viaje/{idViaje}/lugar/{idLugar}")]
    [Authorize]

    public async Task<IActionResult> EliminarLugarDeViaje(int idViaje, int idLugar)
    {
        var viajeLugar = await _context.Viaje_Lugar
            .FirstOrDefaultAsync(vl => vl.IdViaje == idViaje && vl.IdLugar == idLugar);

        if (viajeLugar == null)
        {
            return NotFound("No se encontró la relación entre este viaje y lugar.");
        }

        _context.Viaje_Lugar.Remove(viajeLugar);
        await _context.SaveChangesAsync();

        return Ok("La relación entre el viaje y el lugar ha sido eliminada.");
    }

}
