using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Usuario
{
    [Key]
    public int IdUsuario { get; set; }

    [Required]
    public string Nombre { get; set; } 

    [Required]
    [EmailAddress]
    public string Correo { get; set; }

    [Required]
    public string Contrasena { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    // Relaciones
    public List<Viaje> Viajes { get; set; }
    public List<Notificacion> Notificaciones { get; set; }
    public List<Preferencia> Preferencias { get; set; }
}
