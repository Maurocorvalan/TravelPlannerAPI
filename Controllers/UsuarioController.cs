using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using TravelPlannerAPI.Models;

[Route("api/usuario")]
[ApiController]
[Authorize] // Asegura que el usuario debe estar autenticado
public class UsuarioController : ControllerBase
{
    private readonly TravelPlannerDbContext _context;

    public UsuarioController(TravelPlannerDbContext context)
    {
        _context = context;
    }

    // GET: api/usuario/me
    [HttpGet("me")]
    public IActionResult GetUserProfile()
    {
        try
        {
            // Obtiene el ID del usuario desde el JWT
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                return Unauthorized(new { mensaje = "No se pudo obtener el ID del usuario" });
            }

            int userId = int.Parse(userIdClaim);

            // Busca el usuario en la base de datos
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



    // 🔹 Actualizar perfil del usuario autenticado
    [HttpPut("update")]
    public async Task<IActionResult> UpdateUserProfile([FromBody] UsuarioUpdateDTO userUpdateDTO)
    {
        // Obtén el usuario logueado a partir del JWT
        var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var usuario = await _context.Usuarios.FindAsync(usuarioId);
        if (usuario == null)
        {
            return NotFound("Usuario no encontrado.");
        }

        // Actualiza los campos proporcionados en el request
        if (!string.IsNullOrEmpty(userUpdateDTO.Nombre))
            usuario.Nombre = userUpdateDTO.Nombre;

        if (!string.IsNullOrEmpty(userUpdateDTO.Correo))
            usuario.Correo = userUpdateDTO.Correo;

        if (!string.IsNullOrEmpty(userUpdateDTO.Contrasena))
            usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(userUpdateDTO.Contrasena); // Hash de la nueva contraseña

        // Guarda los cambios en la base de datos
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();

        return Ok(usuario);  // Retorna el usuario actualizado
    }
}
