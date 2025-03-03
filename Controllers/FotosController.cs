using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using TravelPlannerAPI.Models;

[Route("api/[controller]")]
[ApiController]
public class FotosController : ControllerBase
{
    private readonly TravelPlannerDbContext _context;

    private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

    public FotosController(TravelPlannerDbContext context)
    {
        _context = context;
        if (!Directory.Exists(_uploadPath))
        {
            Directory.CreateDirectory(_uploadPath);
        }
    }

    //  Subir una foto
    [HttpPost("{idViaje}")]
    [Authorize]

    public async Task<IActionResult> SubirFoto(int idViaje, [FromForm] IFormFile archivo, [FromForm] string descripcion)
    {
        if (archivo == null || archivo.Length == 0)
            return BadRequest("Debe seleccionar una imagen válida.");

        string fileName = $"{Guid.NewGuid()}_{archivo.FileName}";
        string filePath = Path.Combine(_uploadPath, fileName);
        string relativePath = Path.Combine("uploads", fileName).Replace("\\", "/");

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await archivo.CopyToAsync(stream);
        }

        var foto = new Foto
        {
            IdViaje = idViaje,
            Url = relativePath,
            Descripcion = descripcion,
        };

        _context.Fotos.Add(foto);
        await _context.SaveChangesAsync();

        return Ok(foto);
    }

    // Obtener todas las fotos de un viaje
    [HttpGet("{idViaje}")]
    [Authorize]

    public async Task<IActionResult> ObtenerFotos(int idViaje)
    {
        var fotos = await _context.Fotos.Where(f => f.IdViaje == idViaje).ToListAsync();
        return Ok(fotos);
    }

    // Eliminar una foto
    [HttpDelete("{idFoto}")]
    [Authorize]

    public async Task<IActionResult> EliminarFoto(int idFoto)
    {
        var foto = await _context.Fotos.FindAsync(idFoto);
        if (foto == null)
            return NotFound("Foto no encontrada.");

        string filePath = Path.Combine(Directory.GetCurrentDirectory(), foto.Url);
        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }

        _context.Fotos.Remove(foto);
        await _context.SaveChangesAsync();

        return Ok("Foto eliminada correctamente.");
    }
    //Obtener foto por ID
    [HttpGet("foto/{idFoto}")]
    [Authorize]

    public async Task<IActionResult> ObtenerFoto(int idFoto)
    {
        var foto = await _context.Fotos.FindAsync(idFoto);

        if (foto == null)
        {
            return NotFound("La foto no existe.");
        }

        return Ok(foto);
    }
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> ActualizarFoto(int id, [FromForm] IFormFile archivo, [FromForm] string descripcion)
    {
        var fotoExistente = await _context.Fotos
            .FirstOrDefaultAsync(f => f.IdFoto == id);

        if (fotoExistente == null)
        {
            return NotFound("La foto no existe.");
        }

        if (archivo != null)
        {
            string nuevoNombreArchivo = $"{Guid.NewGuid()}_{archivo.FileName}";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "uploads", nuevoNombreArchivo);
            string relativePath = Path.Combine("uploads", nuevoNombreArchivo);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            fotoExistente.Url = relativePath;
        }

        if (!string.IsNullOrEmpty(descripcion))
        {
            fotoExistente.Descripcion = descripcion;
        }


        _context.Entry(fotoExistente).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return Ok(fotoExistente);
    }



    [HttpGet("{idViaje}/foto")]
    [Authorize]
    public async Task<IActionResult> ObtenerFotoRepresentativa(int idViaje)
    {
        var foto = await _context.Fotos
            .Where(f => f.IdViaje == idViaje)
            .OrderBy(f => f.IdFoto)
            .FirstOrDefaultAsync();

        if (foto == null)
        {
            return NotFound();
        }

        return Ok(foto);
    }


}

