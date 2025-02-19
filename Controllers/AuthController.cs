using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using TravelPlannerAPI.Models;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly TravelPlannerDbContext _context;
    private readonly IConfiguration _config;

    public AuthController(TravelPlannerDbContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    [HttpPost("login")]
    
    public IActionResult Login([FromBody] UsuarioLoginDTO usuarioDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { mensaje = "Datos inválidos" });
        }

        var usuario = _context.Usuarios.SingleOrDefault(u => u.Correo == usuarioDto.Correo);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(usuarioDto.Contrasena, usuario.Contrasena)) 
        {
            return Unauthorized(new { mensaje = "Credenciales incorrectas" });
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config["JwtSettings:SecretKey"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new Claim(ClaimTypes.Email, usuario.Correo)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _config["JwtSettings:Issuer"],
            Audience = _config["JwtSettings:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return Ok(new { token = tokenHandler.WriteToken(token) });
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] Usuario usuario)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { mensaje = "Datos inválidos" });
        }

        if (_context.Usuarios.Any(u => u.Correo == usuario.Correo))
        {
            return BadRequest(new { mensaje = "El correo ya está registrado" });
        }

        usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasena);
        usuario.FechaRegistro = DateTime.UtcNow;

        _context.Usuarios.Add(usuario);
        _context.SaveChanges();

        return Ok(new { mensaje = "Usuario registrado exitosamente" });
    }
}
