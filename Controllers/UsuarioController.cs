using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

using TravelPlannerAPI.Models;

[Route("api/usuario")]
[ApiController]
[Authorize]
public class UsuarioController : ControllerBase
{
    private readonly TravelPlannerDbContext _context;

    public UsuarioController(TravelPlannerDbContext context)
    {
        _context = context;
    }

    [HttpGet("me")]

    public IActionResult GetUserProfile()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized(new { mensaje = "No se pudo obtener el ID del usuario" });
            }

            int userId = int.Parse(userIdClaim);

            var usuario = _context.Usuarios
                .Where(u => u.IdUsuario == userId)
                .Select(u => new
                {
                    u.IdUsuario,
                    u.Nombre,
                    u.Correo,
                })
                .FirstOrDefault();

            if (usuario == null)
            {
                return NotFound(new { mensaje = "Usuario no encontrado" });
            }

            return Ok(usuario);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error interno del servidor", error = ex.Message });
        }
    }
    [HttpPut("update")]
    public IActionResult UpdateUserProfile([FromBody] UsuarioUpdateDTO updatedUsuario)
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized(new { mensaje = "No se pudo obtener el ID del usuario" });
            }

            int userId = int.Parse(userIdClaim);

            // Buscar el usuario en la base de datos
            var usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == userId);
            if (usuario == null)
            {
                return NotFound(new { mensaje = "Usuario no encontrado" });
            }

            if (!string.IsNullOrEmpty(updatedUsuario.Nombre))
            {
                usuario.Nombre = updatedUsuario.Nombre;
            }

            if (!string.IsNullOrEmpty(updatedUsuario.Correo))
            {
                usuario.Correo = updatedUsuario.Correo;
            }
            _context.SaveChanges();

            return Ok(new { mensaje = "Perfil actualizado correctamente" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { mensaje = "Error interno del servidor", error = ex.Message });
        }
    }




}
