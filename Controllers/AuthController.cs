using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using TravelPlannerAPI.Models;
using Microsoft.EntityFrameworkCore;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.AspNetCore.Authorization;
using MimeKit.Text;

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

    public IActionResult Login([FromForm] string Correo, [FromForm] string Contrasena)
    {
        if (string.IsNullOrEmpty(Correo) || string.IsNullOrEmpty(Contrasena))
        {
            return BadRequest(new { mensaje = "Datos inválidos" });
        }

        var usuario = _context.Usuarios.SingleOrDefault(u => u.Correo == Correo);

        if (usuario == null || !BCrypt.Net.BCrypt.Verify(Contrasena, usuario.Contrasena))
        {
            return Unauthorized(new { mensaje = "Credenciales incorrectas" });
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_config["JwtSettings:SecretKey"]);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
            new Claim(ClaimTypes.Email, usuario.Correo)
        }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _config["JwtSettings:Issuer"],
            Audience = _config["JwtSettings:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        string tokenString = tokenHandler.WriteToken(token);

        // Cambia la respuesta para retornar solo el token
        return Ok(tokenString);
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

    // Método para cambiar la contraseña
    [HttpPost("change-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (string.IsNullOrEmpty(request.ContrasenaActual) || string.IsNullOrEmpty(request.NuevaContrasena) || string.IsNullOrEmpty(request.ConfirmarContrasena))
        {
            return BadRequest(new { mensaje = "Datos inválidos" });
        }

        var usuarioId = GetUserIdFromToken(); 
        if (usuarioId == null)
        {
            return Unauthorized(new { mensaje = "Usuario no autorizado" });
        }

        var usuario = await _context.Usuarios.FindAsync(usuarioId.Value); // Se usa .Value para acceder al valor de int?
        if (usuario == null)
        {
            return BadRequest(new { mensaje = "Usuario no encontrado" });
        }

        // Verificar si la contraseña actual es correcta
        if (!BCrypt.Net.BCrypt.Verify(request.ContrasenaActual, usuario.Contrasena))
        {
            return BadRequest(new { mensaje = "La contraseña actual es incorrecta" });
        }

        // Verificar que las contraseñas nuevas coincidan
        if (request.NuevaContrasena != request.ConfirmarContrasena)
        {
            return BadRequest(new { mensaje = "Las nuevas contraseñas no coinciden" });
        }

        // Hashear la nueva contraseña y actualizarla en la base de datos
        usuario.Contrasena = BCrypt.Net.BCrypt.HashPassword(request.NuevaContrasena);
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();

        return Ok(new { mensaje = "Contraseña cambiada exitosamente." });
    }

    // Método para obtener el ID del usuario desde el token
    private int? GetUserIdFromToken()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdString, out var userId))
        {
            return userId; 
        }
        return null; 
    }




    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromForm] string email)
    {
        try
        {
            // Buscar al usuario por email
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == email);
            if (usuario == null)
            {
                return BadRequest(new { mensaje = "El correo ingresado no existe." });
            }

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new StringBuilder(6);
            Random random = new Random();
            for (int i = 0; i < 6; i++)
            {
                result.Append(chars[random.Next(chars.Length)]);
            }

            string enlace = result.ToString();

            string token = BCrypt.Net.BCrypt.HashPassword(enlace);
            usuario.Contrasena = token;

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            // Crear el mensaje de correo
            var message = new MimeKit.MimeMessage();
            message.To.Add(new MailboxAddress(usuario.Nombre, usuario.Correo));
            message.From.Add(new MailboxAddress("Sistema", _config["Smtp:User"]));
            message.Subject = "Restablecer contraseña";
            message.Body = new TextPart("html")
            {
                Text = $@"<p>Hola {usuario.Nombre}:</p>
            <p>Hemos recibido una solicitud para restablecer la contraseña de tu cuenta.</p>
            <p>Tu nuevo código de restablecimiento es: <strong>{enlace}</strong></p>
            <p>Por razones de seguridad, te recomendamos cambiar esta contraseña después de iniciar sesión.</p>"
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                await client.ConnectAsync(_config["Smtp:Host"], int.Parse(_config["Smtp:Port"]), MailKit.Security.SecureSocketOptions.Auto);
                await client.AuthenticateAsync(_config["Smtp:User"], _config["Smtp:Pass"]);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }

            return Ok(new { mensaje = "Correo enviado con éxito. Revisa tu bandeja de entrada para restablecer tu contraseña." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { mensaje = $"Error: {ex.Message}" });
        }
    }



}
