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
                    u.FechaRegistro
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
    public async Task<IActionResult> UpdateUserProfile([FromBody] UsuarioUpdateDTO userUpdateDTO)
    {
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var usuario = await _context.Usuarios.FindAsync(usuarioId);
        if (usuario == null)
        {
            return NotFound("Usuario no encontrado.");
        }

        if (!string.IsNullOrEmpty(userUpdateDTO.Nombre))
            usuario.Nombre = userUpdateDTO.Nombre;

        if (!string.IsNullOrEmpty(userUpdateDTO.Correo))
            usuario.Correo = userUpdateDTO.Correo;

        if (!string.IsNullOrEmpty(userUpdateDTO.Contrasena))
            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(userUpdateDTO.Contrasena);

        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();

        return Ok(usuario);
    }
}
