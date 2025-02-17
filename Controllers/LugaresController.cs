using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using TravelPlannerAPI.Models;

[Route("api/lugares")]
[ApiController]
public class LugaresController : ControllerBase
{
    private readonly TravelPlannerDbContext _context;

    public LugaresController(TravelPlannerDbContext context)
    {
        _context = context;
    }

    // Crear un nuevo lugar
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CrearLugar([FromBody] Lugar lugar)
    {
        if (lugar == null)
            return BadRequest("Datos del lugar inválidos.");

        _context.Lugares.Add(lugar);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(ObtenerLugar), new { id = lugar.IdLugar }, lugar);
    }

    // Obtener todos los lugares
    [HttpGet]
    public async Task<IActionResult> ObtenerLugares()
    {
        var lugares = await _context.Lugares.ToListAsync();
        return Ok(lugares);
    }

    // Obtener un lugar por ID
    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerLugar(int id)
    {
        var lugar = await _context.Lugares.FindAsync(id);
        if (lugar == null)
            return NotFound("Lugar no encontrado.");

        return Ok(lugar);
    }

    // Actualizar un lugar
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> ActualizarLugar(int id, [FromBody] Lugar lugar)
    {
        if (lugar == null || lugar.IdLugar != id)
            return BadRequest("Datos inválidos.");

        var lugarExistente = await _context.Lugares.FindAsync(id);
        if (lugarExistente == null)
            return NotFound("Lugar no encontrado.");

        lugarExistente.Nombre = lugar.Nombre;
        lugarExistente.Descripcion = lugar.Descripcion;
        lugarExistente.Ubicacion = lugar.Ubicacion;
        lugarExistente.Categoria = lugar.Categoria;

        _context.Entry(lugarExistente).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(lugarExistente);
    }

    // Eliminar un lugar
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> EliminarLugar(int id)
    {
        var lugar = await _context.Lugares.FindAsync(id);
        if (lugar == null)
            return NotFound("Lugar no encontrado.");

        _context.Lugares.Remove(lugar);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
