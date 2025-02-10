using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using TravelPlannerAPI.Models;

[Route("api/gastos")]
[ApiController]
public class GastosController : ControllerBase
{
    private readonly TravelPlannerDbContext _context;

    public GastosController(TravelPlannerDbContext context)
    {
        _context = context;
    }

    // Crear un nuevo gasto
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CrearGasto([FromBody] Gasto gasto)
    {
        if (gasto == null)
        {
            return BadRequest("Los datos del gasto son inválidos.");
        }
        var viajeExistente = await _context.Viajes
            .Where(v => v.IdViaje == gasto.IdViaje)
            .FirstOrDefaultAsync();

        if (viajeExistente == null)
        {
            return NotFound("El viaje relacionado no existe.");
        }

        _context.Gastos.Add(gasto);
        await _context.SaveChangesAsync();

        return CreatedAtAction("ObtenerGasto", new { id = gasto.IdGasto }, gasto);
    }

    // Obtener todos los gastos de un viaje
    [HttpGet("{idViaje}")]
    [Authorize]
    public async Task<IActionResult> ObtenerGastos(int idViaje)
    {
        var gastos = await _context.Gastos
            .Where(g => g.IdViaje == idViaje)
            .ToListAsync();

        if (gastos == null || !gastos.Any())
        {
            return NotFound("No se encontraron gastos para este viaje.");
        }

        return Ok(gastos);
    }

    // Obtener un gasto por su id
    [HttpGet("gasto/{id}")]
    [Authorize]
    public async Task<IActionResult> ObtenerGasto(int id)
    {
        var gasto = await _context.Gastos
            .FirstOrDefaultAsync(g => g.IdGasto == id);

        if (gasto == null)
        {
            return NotFound("El gasto no existe.");
        }

        return Ok(gasto);
    }

    // Actualizar un gasto
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> ActualizarGasto(int id, [FromBody] Gasto gasto)
    {

        var gastoExistente = await _context.Gastos
            .FirstOrDefaultAsync(g => g.IdGasto == id);

        if (gastoExistente == null)
        {
            return NotFound("El gasto no existe.");
        }

        gastoExistente.Categoria = gasto.Categoria;
        gastoExistente.Monto = gasto.Monto;
        gastoExistente.Descripcion = gasto.Descripcion;
        gastoExistente.FechaGasto = gasto.FechaGasto;

        _context.Entry(gastoExistente).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(gastoExistente);
    }

    // Eliminar un gasto
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> EliminarGasto(int id)
    {
        var gasto = await _context.Gastos
            .FirstOrDefaultAsync(g => g.IdGasto == id);

        if (gasto == null)
        {
            return NotFound("El gasto no existe.");
        }

        _context.Gastos.Remove(gasto);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
